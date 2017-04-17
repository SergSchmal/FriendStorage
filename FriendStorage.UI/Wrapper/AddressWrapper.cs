using System.ComponentModel.DataAnnotations;
using FriendStorage.Model;
using FriendStorage.UI.Wrapper.Base;

namespace FriendStorage.UI.Wrapper
{
    public class AddressWrapper : ModelWrapper<Address>
    {
        public AddressWrapper(Address model) : base(model)
        {
        }

        public int Id
        {
            get => GetValue<int>();
            set => SetValue(value);
        }

        public string IdOriginalValue => GetOriginalValue<string>(nameof(Id));

        public bool IdIsChanged => GetIsChanged(nameof(Id));

        [Required(ErrorMessage="City is requerid!")]
        public string City
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string CityOriginalValue => GetOriginalValue<string>(nameof(City));

        public bool CityIsChanged => GetIsChanged(nameof(City));

        public string Street
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string StreetOriginalValue => GetOriginalValue<string>(nameof(Street));

        public bool StreetIsChanged => GetIsChanged(nameof(Street));

        public string StreetNumber
        {
            get => GetValue<string>();
            set => SetValue(value);
        }

        public string StreetNumberOriginalValue => GetOriginalValue<string>(nameof(StreetNumber));

        public bool StreetNumberIsChanged => GetIsChanged(nameof(StreetNumber));
    }
}