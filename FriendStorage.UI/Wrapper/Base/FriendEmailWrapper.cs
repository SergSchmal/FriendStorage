using System.ComponentModel.DataAnnotations;
using FriendStorage.Model;

namespace FriendStorage.UI.Wrapper.Base
{
    public class FriendEmailWrapper : ModelWrapper<FriendEmail>
    {
        public FriendEmailWrapper(FriendEmail model) : base(model)
        {
        }

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string IdOriginalValue => GetOriginalValue<string>(nameof(Id));

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Email is not valid email address")]
        public string Email
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string EmailOriginalValue => GetOriginalValue<string>(nameof(Email));

        public bool EmailIsChanged => GetIsChanged(nameof(Email));

        public string Comment
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string CommentOriginalValue => GetOriginalValue<string>(nameof(Comment));

        public bool CommentIsChanged => GetIsChanged(nameof(Comment));
    }
}