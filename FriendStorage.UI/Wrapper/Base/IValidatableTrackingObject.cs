using System.ComponentModel;

namespace FriendStorage.UI.Wrapper.Base
{
    public interface IValidatableTrackingObject : IRevertibleChangeTracking, INotifyPropertyChanged
    {
        bool IsValid { get; }
    }
}