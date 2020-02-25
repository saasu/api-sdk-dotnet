using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Saasu.API.Client.Framework;
using Xunit;
using Saasu.API.Client.Proxies;

namespace Saasu.API.Client.IntegrationTests
{
    public class SerialisationTests
    {
        [Fact]
        public void ShouldBeAbleToReturnApiResultsAsJson()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken) {ContentType = RequestContentType.ApplicationJson};
            ContactTests.AssertContactProxy(proxy);

        }

        [Fact]
        public void ShouldBeAbleToReturnApiResultsAsXml()
        {
            var accessToken = TestHelper.SignInAndGetAccessToken();

            var proxy = new ContactsProxy(accessToken) {ContentType = RequestContentType.ApplicationXml};
            ContactTests.AssertContactProxy(proxy);

        }

        [Fact]
        public void ShouldBeAbleToPostToApiWithXml()
        {
            var contactHelper = new ContactHelper(false);
            var contact = contactHelper.GetMinimalContact();
            contact.GivenName = "SerializationTests";
            contact.FamilyName = "PostToApiWithXml";
            contact.EmailAddress = "PostToApiWithXml@SerializationTests.com";

            var proxy = new ContactProxy {ContentType = RequestContentType.ApplicationXml};
            var insertResponse = proxy.InsertContact(contact);

            Assert.True(insertResponse.IsSuccessfull);
            Assert.True(insertResponse.DataObject.InsertedContactId > 0);
        }

        [Fact]
        public void ShouldBeAbleToPutToApiWithXml()
        {
            var contactHelper = new ContactHelper(false);
            var contact = contactHelper.GetMinimalContact();
            contact.GivenName = "SerializationTests";
            contact.FamilyName = "PutToApiWithXml";
            contact.EmailAddress = "PutToApiWithXml@SerializationTests.com";

            var proxy = new ContactProxy {ContentType = RequestContentType.ApplicationXml};
            var insertResponse = proxy.InsertContact(contact);

            Assert.True(insertResponse.DataObject.InsertedContactId > 0);

            var insertedContact = proxy.GetContact(insertResponse.DataObject.InsertedContactId);

            Assert.True(insertedContact.IsSuccessfull && insertedContact.DataObject != null);

            insertedContact.DataObject.GivenName = "NewNameWithXml";

            var updateResult = proxy.UpdateContact(insertedContact.DataObject, insertedContact.DataObject.Id.GetValueOrDefault());

            Assert.True(updateResult.IsSuccessfull);

            var updatedContact = proxy.GetContact(updateResult.DataObject.UpdatedContactId);

            Assert.True(updatedContact.IsSuccessfull);
            Assert.Equal("NewNameWithXml",updatedContact.DataObject.GivenName);

        }

        [Fact]
        public void ShouldBeAbleToPostToApiWithJson()
        {
            var contactHelper = new ContactHelper(false);
            var contact = contactHelper.GetMinimalContact();
            contact.GivenName = "SerializationTests";
            contact.FamilyName = "PostToApiWithJson";
            contact.EmailAddress = "PostToApiWithJson@SerializationTests.com";

            var proxy = new ContactProxy();
            proxy.ContentType = RequestContentType.ApplicationJson;
            var insertResponse = proxy.InsertContact(contact);

            Assert.True(insertResponse.DataObject.InsertedContactId > 0);
        }

        [Fact]
        public void ShouldBeAbleToPutToApiWithJson()
        {
            var contactHelper = new ContactHelper(false);
            var contact = contactHelper.GetMinimalContact();
            contact.GivenName = "SerializationTests";
            contact.FamilyName = "PutToApiWithJson";
            contact.EmailAddress = "PutToApiWithJson@SerializationTests.com";

            var proxy = new ContactProxy {ContentType = RequestContentType.ApplicationJson};
            var insertResponse = proxy.InsertContact(contact);

            Assert.True(insertResponse.DataObject.InsertedContactId > 0);

            var insertedContact = proxy.GetContact(insertResponse.DataObject.InsertedContactId);

            Assert.True(insertedContact.IsSuccessfull && insertedContact.DataObject != null);

            insertedContact.DataObject.GivenName = "NewNameWithJson";

            var updateResult = proxy.UpdateContact(insertedContact.DataObject, insertedContact.DataObject.Id.GetValueOrDefault());

            Assert.True(updateResult.IsSuccessfull);

            var updatedContact = proxy.GetContact(updateResult.DataObject.UpdatedContactId);

            Assert.True(updatedContact.IsSuccessfull);
            Assert.Equal("NewNameWithJson", updatedContact.DataObject.GivenName);

        }
    }
}
