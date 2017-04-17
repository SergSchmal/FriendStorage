using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using FriendStorage.UI.Command;
using FriendStorage.UI.Events;
using FriendStorage.UI.View.Services;
using Microsoft.Practices.Prism.PubSubEvents;

namespace FriendStorage.UI.ViewModel
{
    public class MainViewModel : Observable
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly Func<IFriendEditViewModel> _friendEditViewModelCreator;
        private IFriendEditViewModel _selectedFriendEditViewModel;

        public MainViewModel(IEventAggregator eventAggregator,
            IMessageDialogService messageDialogService,
            INavigationViewModel navigationViewModel,
            Func<IFriendEditViewModel> friendEditViewModelCreator)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<OpenFriendEditViewEvent>().Subscribe(OnOpenFriendTab);
            _eventAggregator.GetEvent<FriendDeletedEvent>().Subscribe(OnFriendDeleted);

            NavigationViewModel = navigationViewModel;
            _friendEditViewModelCreator = friendEditViewModelCreator;
            FriendEditViewModels = new ObservableCollection<IFriendEditViewModel>();
            CloseFriendTabCommand = new DelegateCommand(OnCloseFriendTabExecute);
            AddFriendCommand = new DelegateCommand(OnAddFriendExecute);
        }

        public ICommand CloseFriendTabCommand { get; }

        public ICommand AddFriendCommand { get; set; }

        public INavigationViewModel NavigationViewModel { get; }

        // Those ViewModels represent the Tab-Pages in the UI
        public ObservableCollection<IFriendEditViewModel> FriendEditViewModels { get; }

        public IFriendEditViewModel SelectedFriendEditViewModel
        {
            get => _selectedFriendEditViewModel;
            set
            {
                _selectedFriendEditViewModel = value;
                OnPropertyChanged();
            }
        }

        public void Load()
        {
            NavigationViewModel.Load();
        }

        private void OnAddFriendExecute(object obj)
        {
            var friendEditVm = _friendEditViewModelCreator();
            FriendEditViewModels.Add(friendEditVm);
            friendEditVm.Load();
            SelectedFriendEditViewModel = friendEditVm;
        }

        private void OnOpenFriendTab(int friendId)
        {
            var friendEditVm =
                FriendEditViewModels.SingleOrDefault(vm => vm.Friend.Id == friendId);
            if (friendEditVm == null)
            {
                friendEditVm = _friendEditViewModelCreator();
                FriendEditViewModels.Add(friendEditVm);
                friendEditVm.Load(friendId);
            }
            SelectedFriendEditViewModel = friendEditVm;
        }

        private void OnCloseFriendTabExecute(object parameter)
        {
            var friendEditVmToClose = parameter as IFriendEditViewModel;
            if (friendEditVmToClose != null)
            {
                if (friendEditVmToClose.Friend.IsChanged)
                {
                    var dialogResult = _messageDialogService.ShowYesNoDialog("Close tab", "Yoo'll lose your changes, if you close this tab. Close it?", MessageDialogResult.No);
                    if (dialogResult == MessageDialogResult.No)
                        return;
                }
            }
            FriendEditViewModels.Remove(friendEditVmToClose);
        }

        private void OnFriendDeleted(int friendId)
        {
            var friendDetailVmToClose
                = FriendEditViewModels.SingleOrDefault(f => f.Friend.Id == friendId);
            if (friendDetailVmToClose != null)
                FriendEditViewModels.Remove(friendDetailVmToClose);
        }

        public void OnClosing(CancelEventArgs e)
        {
            if (FriendEditViewModels.Any(f => f.Friend.IsChanged))
            {
                var result = _messageDialogService.ShowYesNoDialog("Close application?", "You'll lose your changes if you close the application. Close it?", MessageDialogResult.No);
                e.Cancel = result == MessageDialogResult.No;
            }
        }
    }
}