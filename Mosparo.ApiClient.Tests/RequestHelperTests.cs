using System.Collections;
using System.Text.Json;
using Xunit;

namespace Mosparo.ApiClient.Tests
{
    public class RequestHelperTests
    {
        string publicKey = "publicKey";
        string privateKey = "privateKey";

        [Fact()]
        public void getJsonSerializerOptionsTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            Xunit.Assert.IsType<JsonSerializerOptions>(requestHelper.getJsonSerializerOptions());
        }

        [Fact()]
        public void generateHmacHashTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            string data = "testData";

            Xunit.Assert.Equal("0646b5f2e09db205a8b3eb0e7429645561a1b9fdff1fcdb1fed9cd585108d850", requestHelper.generateHmacHash(data));
        }

        [Fact()]
        public void prepareFormDataTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            // Test data
            SortedDictionary<string, IFormValue> addressData = new SortedDictionary<string, IFormValue>()
            {
                { "street", new StringFormValue("Teststreet") },
                { "number", new StringFormValue("123") },
            };

            SortedDictionary<string, IFormValue> data = new SortedDictionary<string, IFormValue>
            {
                { "address", new DictionaryFormValue(addressData) },
                { "name", new StringFormValue("Test Tester") },
                { "email", new ArrayListFormValue(new ArrayList() { "test@example.com" }) },
                { "website", new StringFormValue("") },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
            };

            // Expected data
            SortedDictionary<string, IFormValue> targetAddressData = new SortedDictionary<string, IFormValue>()
            {
                { "street", new StringFormValue("cc0bdb0377d3ba87046028784e8a4319972a7c9df31c645e80e14e8dd8735b6b") },
                { "number", new StringFormValue("a665a45920422f9d417e4867efdc4fb8a04a1f3fff1fa07e998e86f7f7a27ae3") },
            };

            SortedDictionary<string, IFormValue> targetData = new SortedDictionary<string, IFormValue>
            {
                { "address", new DictionaryFormValue(targetAddressData) },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
                { "email", new ArrayListFormValue(new ArrayList() { "973dfe463ec85785f5f95af5ba3906eedb2d931c24e69824a89ea65dba4e813b" }) },
                { "name", new StringFormValue("153590093b8c278bb7e1fef026d8a59b9ba02701d1e0a66beac0938476f2a812") },
                { "website", new StringFormValue("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855") },
            };

            Xunit.Assert.Equivalent(targetData, requestHelper.prepareFormData(data));
        }

        [Fact()]
        public void cleanupFormDataTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            // Test data
            SortedDictionary<string, IFormValue> addressData = new SortedDictionary<string, IFormValue>()
            {
                { "street", new StringFormValue("Teststreet\r\nTest\r\nStreet") },
                { "number", new LongFormValue(123) },
            };

            SortedDictionary<string, IFormValue> data = new SortedDictionary<string, IFormValue>
            {
                { "_mosparo_submitToken", new StringFormValue("submitToken") },
                { "_mosparo_validationToken", new StringFormValue("validationToken") },
                { "name", new StringFormValue("Test Tester") },
                { "address", new DictionaryFormValue(addressData) },
                { "valid", new BoolFormValue(false) },
                { "email", new ArrayListFormValue(new ArrayList() { "test@example.com" }) },
                { "website", new StringFormValue("") },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
            };

            // Expected data
            SortedDictionary<string, IFormValue> targetAddressData = new SortedDictionary<string, IFormValue>()
            {
                { "number", new LongFormValue(123) },
                { "street", new StringFormValue("Teststreet\nTest\nStreet") },
            };

            SortedDictionary<string, IFormValue> targetData = new SortedDictionary<string, IFormValue>
            {
                { "address", new DictionaryFormValue(targetAddressData) },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
                { "email", new ArrayListFormValue(new ArrayList() { "test@example.com" }) },
                { "name", new StringFormValue("Test Tester") },
                { "valid", new BoolFormValue(false) },
                { "website", new StringFormValue("") },
            };

            Xunit.Assert.Equivalent(targetData, requestHelper.cleanupFormData(data));
        }

        [Fact()]
        public void toJsonTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            // Test data
            SortedDictionary<string, IFormValue> addressData = new SortedDictionary<string, IFormValue>()
            {
                { "street", new StringFormValue("Teststreet") },
                { "number", new LongFormValue(123) },
            };

            SortedDictionary<string, IFormValue> data = new SortedDictionary<string, IFormValue>
            {
                { "name", new StringFormValue("Test Tester") },
                { "address", new DictionaryFormValue(addressData) },
                { "valid", new BoolFormValue(false) },
                { "email", new NullFormValue() },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
            };

            // Expected data
            string targetJson = "{\"address\":{\"number\":123,\"street\":\"Teststreet\"},\"data\":{},\"email\":null,\"name\":\"Test Tester\",\"valid\":false}";

            Xunit.Assert.Equal(targetJson, requestHelper.toJson(data));
        }

        [Fact()]
        public void createFormDataHmacHashTest()
        {
            RequestHelper requestHelper = new RequestHelper(publicKey, privateKey);

            // Test data
            SortedDictionary<string, IFormValue> addressData = new SortedDictionary<string, IFormValue>()
            {
                { "street", new StringFormValue("Teststreet") },
                { "number", new LongFormValue(123) },
            };

            SortedDictionary<string, IFormValue> data = new SortedDictionary<string, IFormValue>
            {
                { "name", new StringFormValue("Test Tester") },
                { "address", new DictionaryFormValue(addressData) },
                { "valid", new BoolFormValue(false) },
                { "email", new NullFormValue() },
                { "data", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()) },
            };

            // Expected data
            string targetHash = "a7d6c71dbcfefecd7d76fa44df84445e45e1fd88620be537eb53d5be9fbe6d33";

            Xunit.Assert.Equal(targetHash, requestHelper.createFormDataHmacHash(data));
        }
    }
}
