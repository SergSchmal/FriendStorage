using System.Collections.Generic;
using System.Linq;
using FriendStorage.Model;
using FriendStorage.UI.Wrapper;
using FriendStorage.UI.Wrapper.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FriendStorage.UITests.Wrapper
{
    [TestClass]
    public class ChangeNotificationCollectionProperty
    {
        private Friend _friend;
        private FriendEmail _friendEmail;

        [TestInitialize]
        public void Initialaze()
        {
            _friendEmail = new FriendEmail { Email = "elena@email.com" };
            _friend = new Friend
            {
                FirstName = "Sergej",
                Address = new Address(),
                Emails = new List<FriendEmail>
                {
                    new FriendEmail{Email = "sergej@email.com"},
                    _friendEmail
                }
            };
        }

        [TestMethod]
        public void ShouldInitializeEmailsProperty()
        {
            var wrapper = new FriendWrapper(_friend);
            Assert.IsNotNull(wrapper.Emails);
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterAddingEmail()
        {
            var wrapper = new FriendWrapper(_friend);
            var emailToRemove = wrapper.Emails.Single(ew => ew.Model == _friendEmail);
            wrapper.Emails.Remove(emailToRemove);
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterRemovingEmail()
        {
            _friend.Emails.Remove(_friendEmail);
            var wrapper = new FriendWrapper(_friend);
            wrapper.Emails.Add(new FriendEmailWrapper(_friendEmail));
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        [TestMethod]
        public void ShouldBeInSyncAfterClearingEmails()
        {
            var wrapper = new FriendWrapper(_friend);
            wrapper.Emails.Clear();
            CheckIfModelEmailsCollectionIsInSync(wrapper);
        }

        private void CheckIfModelEmailsCollectionIsInSync(FriendWrapper wrapper)
        {
            Assert.AreEqual(wrapper.Emails.Count, _friend.Emails.Count);
            Assert.IsTrue(_friend.Emails.All(e => wrapper.Emails.Any(we => we.Model == e)));
        }
    }
}