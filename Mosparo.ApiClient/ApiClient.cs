using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mosparo.ApiClient
{
    public class ApiClient
    {
        private string host;
        private string publicKey;
        private string privateKey;

        public ApiClient(string host, string publicKey, string privateKey)
        {
            this.host = host;
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public VerificationResult verifySubmission(FormRequest formRequest)
        {
            return verifySubmission(formRequest, new HttpClient(new HttpClientHandler()));
        }

        public VerificationResult verifySubmission(FormRequest formRequest, HttpClient client)
        {
            if (formRequest.getSubmitToken().Equals("") || formRequest.getValidationToken().Equals(""))
            {
                throw new Exception("Submit or validation token not available.");
            }

            RequestHelper rh = new RequestHelper(publicKey, privateKey);

            // Prepare the data
            SortedDictionary<string, IFormValue> preparedFormData = rh.prepareFormData(formRequest.getFormData());
            string formSignature = rh.createFormDataHmacHash(preparedFormData);
            string validationSignature = rh.generateHmacHash(formRequest.getValidationToken());
            string apiEndpoint = "/api/v1/verification/verify";

            // Build the request signature
            Dictionary<string, object> requestDataForSignature = new Dictionary<string, object>
            {
                { "submitToken", formRequest.getSubmitToken() },
                { "validationSignature", validationSignature },
                { "formSignature", formSignature },
                { "formData", preparedFormData }
            };
            string requestSignature = rh.generateHmacHash(apiEndpoint + JsonSerializer.Serialize(requestDataForSignature, rh.getJsonSerializerOptions()));

            // Build the request data
            Dictionary<string, string> requestData = new Dictionary<string, string>
            {
                { "submitToken", formRequest.getSubmitToken() },
                { "validationSignature", validationSignature },
                { "formSignature", formSignature }
            };

            addFormData(requestData, preparedFormData, "formData");

            string verificationSignature = rh.generateHmacHash(validationSignature + formSignature);

            // Build the request
            var request = new HttpRequestMessage()
            {
                RequestUri = new Uri(host + apiEndpoint),
                Method = HttpMethod.Post,
                Content = new FormUrlEncodedContent(requestData),
            };
            request.Headers.Add("Authorization", "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes(publicKey + ":" + requestSignature)));
            request.Headers.Add("Accept", "application/json");

            // Send the request and store the response
            var result = "";
            try
            {
                var task = Task.Run(() => client.SendAsync(request).ContinueWith((taskResponse) =>
                    {
                        return taskResponse.Result.Content.ReadAsStringAsync();
                    }
                ));
                task.Wait();

                result = task.Result.Result;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while sending the request to mosparo.", ex);
            }

            ApiResponse responseData;
            try
            {
                responseData = JsonSerializer.Deserialize<ApiResponse>(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Response from API invalid.", ex);
            }

            bool submittable = false;
            ArrayList issues = new ArrayList();
            object debugInformation = null;

            if (responseData.valid && responseData.verificationSignature == verificationSignature)
            {
                submittable = true;
            }
            else
            {
                if (responseData.errorMessage != null)
                {
                    issues.Add(responseData.errorMessage);
                }
                
                debugInformation = responseData.debugInformation;
            }

            return new VerificationResult(submittable, responseData.valid, responseData.verifiedFields, issues, debugInformation);
        }

        private void addFormData(Dictionary<string, string> requestData, SortedDictionary<string, IFormValue> preparedFormData, string prefix)
        {
            foreach (var pair in preparedFormData)
            {
                if (pair.Value is IDictionaryFormValue)
                {
                    addFormData(requestData, ((IDictionaryFormValue)pair.Value).getValueAsDictionary(), prefix + "[" + pair.Key + "]");
                }
                else if (pair.Value is IArrayListFormValue)
                {
                    var arr = ((IArrayListFormValue)pair.Value).getValueAsArrayList();

                    for (int idx = 0; idx < arr.Count; idx++) 
                    {
                        requestData.Add(prefix + "[" + pair.Key + "][" + idx + "]", arr[idx].ToString());
                    }
                }
                else if (pair.Value is IStringFormValue)
                {
                    requestData.Add(prefix + "[" + pair.Key + "]", ((IStringFormValue)pair.Value).getValueAsString());
                }
            }
        }
    }
}
