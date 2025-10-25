using RichardSzalay.MockHttp;
using Xunit;

namespace Mosparo.ApiClient.Tests
{
    public class ApiClientTests
    {
        [Fact()]
        public void verifySubmissionWithoutTokensTest()
        {
            var request = new FormRequest("", "", new SortedDictionary<string, IFormValue>()
            {
                { "name", new StringFormValue("John Example") }
            });

            var client = new ApiClient("http://test.local", "testPublicKey", "testPrivateKey");
            Exception exception = Xunit.Assert.Throws<Exception>(() => client.verifySubmission(request));
            Xunit.Assert.Equal("Submit or validation token not available.", exception.Message);
        }

        [Fact()]
        public void verifySubmissionFormTokensEmptyResponseTest()
        {
            var handler = new MockHttpMessageHandler();
            handler.When("http://test.local/api/v1/verification/verify").Respond("application/json", "");

            var request = new FormRequest("submitToken", "validationToken", new SortedDictionary<string, IFormValue>()
            {
                { "name", new StringFormValue("John Example") }
            });

            var client = new ApiClient("http://test.local", "testPublicKey", "testPrivateKey");
            Exception exception = Xunit.Assert.Throws<Exception>(() => client.verifySubmission(request, handler.ToHttpClient()));
            Xunit.Assert.Equal("Response from API invalid.", exception.Message);
        }

        [Fact()]
        public void verifySubmissionConnectionErrorTest()
        {
            var handler = new MockHttpMessageHandler();

            var request = new FormRequest("submitToken", "validationToken", new SortedDictionary<string, IFormValue>()
            {
                { "name", new StringFormValue("John Example") }
            });

            var client = new ApiClient("http://test.local", "testPublicKey", "testPrivateKey");
            Exception exception = Xunit.Assert.Throws<Exception>(() => client.verifySubmission(request, handler.ToHttpClient()));
            Xunit.Assert.Equal("An error occurred while sending the request to mosparo.", exception.Message);
        }

        [Fact()]
        public void verifySubmissionIsValidTest()
        {
            var publicKey = "testPublicKey";
            var privateKey = "testPrivateKey";
            var submitToken = "submitToken";
            var validationToken = "validationToken";
            var formData = new SortedDictionary<string, IFormValue>(){
                { "name", new StringFormValue("John Example") }
            };

            var requestHelper = new RequestHelper(publicKey, privateKey);

            var preparedFormData = requestHelper.prepareFormData(formData);
            var formSignature = requestHelper.createFormDataHmacHash(preparedFormData);

            var validationSignature = requestHelper.generateHmacHash(validationToken);
            var verificationSignature = requestHelper.generateHmacHash(validationSignature + formSignature);

            var json = "{\"valid\":true,\"verificationSignature\":\"" + verificationSignature + "\",\"verifiedFields\":{\"name\":\"valid\"},\"issues\":[]}";

            var handler = new MockHttpMessageHandler();
            var request = handler
                .When("http://test.local/api/v1/verification/verify")
                .WithPartialContent("submitToken=submitToken&")
                .WithPartialContent("validationSignature=" + validationSignature + "&")
                .WithPartialContent("formSignature=" + formSignature + "&")
                .Respond("application/json", json);

            var formRequest = new FormRequest(submitToken, validationToken, formData);
            var client = new ApiClient("http://test.local", publicKey, privateKey);
            var result = client.verifySubmission(formRequest, handler.ToHttpClient());

            Xunit.Assert.IsType<VerificationResult>(result);
            Xunit.Assert.True(result.isSubmittable());
            Xunit.Assert.True(result.isValid());
            Xunit.Assert.Equal(VerificationResult.VALID, result.getVerifiedField("name"));
            Xunit.Assert.False(result.hasIssues());
            Xunit.Assert.Equal(1, handler.GetMatchCount(request));
        }

        [Fact()]
        public void verifySubmissionIsInvalidTest()
        {
            var publicKey = "testPublicKey";
            var privateKey = "testPrivateKey";
            var submitToken = "submitToken";
            var validationToken = "validationToken";
            var formData = new SortedDictionary<string, IFormValue>(){
                { "name", new StringFormValue("John Example") }
            };

            var requestHelper = new RequestHelper(publicKey, privateKey);

            var preparedFormData = requestHelper.prepareFormData(formData);
            var formSignature = requestHelper.createFormDataHmacHash(preparedFormData);

            var validationSignature = requestHelper.generateHmacHash(validationToken);
            var verificationSignature = requestHelper.generateHmacHash(validationSignature + formSignature);

            var json = "{\"error\":true,\"errorMessage\":\"Validation failed.\"}";

            var handler = new MockHttpMessageHandler();
            var request = handler
                .When("http://test.local/api/v1/verification/verify")
                .WithPartialContent("submitToken=submitToken&")
                .WithPartialContent("validationSignature=" + validationSignature + "&")
                .WithPartialContent("formSignature=" + formSignature + "&")
                .Respond("application/json", json);

            var formRequest = new FormRequest(submitToken, validationToken, formData);
            var client = new ApiClient("http://test.local", publicKey, privateKey);
            var result = client.verifySubmission(formRequest, handler.ToHttpClient());

            Xunit.Assert.IsType<VerificationResult>(result);
            Xunit.Assert.False(result.isSubmittable());
            Xunit.Assert.False(result.isValid());
            Xunit.Assert.True(result.hasIssues());
            Xunit.Assert.Equal("Validation failed.", result.getIssues()[0]);
            Xunit.Assert.Equal(1, handler.GetMatchCount(request));
        }

        [Fact()]
        public void verifySubmissionIsValidExtendedRequestDataTest()
        {
            var publicKey = "testPublicKey";
            var privateKey = "testPrivateKey";
            var submitToken = "submitToken";
            var validationToken = "validationToken";
            var formData = new SortedDictionary<string, IFormValue>(){
                { "name", new StringFormValue("John Example") },
                { "age", new LongFormValue(30) },
                { "confirmTos", new BoolFormValue(true) },
                { "selectedCountries", new ArrayListFormValue(new System.Collections.ArrayList() { "DE", "NL", "CA" }) },
                { "address", new DictionaryFormValue(new SortedDictionary<string, IFormValue>()
                    {
                        { "street", new StringFormValue("Teststreet") },
                        { "nr", new DecimalFormValue(123.22M) }
                    }
                )}
            };

            var requestHelper = new RequestHelper(publicKey, privateKey);

            var preparedFormData = requestHelper.prepareFormData(formData);
            var formSignature = requestHelper.createFormDataHmacHash(preparedFormData);

            var validationSignature = requestHelper.generateHmacHash(validationToken);
            var verificationSignature = requestHelper.generateHmacHash(validationSignature + formSignature);

            var json = "{\"valid\":true,\"verificationSignature\":\"" + verificationSignature + "\",\"verifiedFields\":{\"name\":\"valid\"},\"issues\":[]}";

            var handler = new MockHttpMessageHandler();
            var request = handler
                .When("http://test.local/api/v1/verification/verify")
                .WithPartialContent("submitToken=submitToken&")
                .WithPartialContent("validationSignature=" + validationSignature + "&")
                .WithPartialContent("formSignature=" + formSignature + "&")
                .Respond("application/json", json);

            var formRequest = new FormRequest(submitToken, validationToken, formData);
            var client = new ApiClient("http://test.local", publicKey, privateKey);
            var result = client.verifySubmission(formRequest, handler.ToHttpClient());

            Xunit.Assert.IsType<VerificationResult>(result);
            Xunit.Assert.True(result.isSubmittable());
            Xunit.Assert.True(result.isValid());
            Xunit.Assert.Equal(VerificationResult.VALID, result.getVerifiedField("name"));
            Xunit.Assert.False(result.hasIssues());
            Xunit.Assert.Equal(1, handler.GetMatchCount(request));
        }
    }
}