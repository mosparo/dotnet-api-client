using System.Collections.Generic;

namespace Mosparo.ApiClient
{
    public class FormRequest
    {
        private string submitToken;

        private string validationToken;

        private SortedDictionary<string, IFormValue> formData;

        public FormRequest(string submitToken, string validationToken, SortedDictionary<string, IFormValue> formData)
        {
            this.submitToken = submitToken;
            this.validationToken = validationToken;
            this.formData = formData;
        }

        public string getSubmitToken()
        {
            return submitToken;
        }

        public string getValidationToken()
        {
            return validationToken;
        }

        public SortedDictionary<string, IFormValue> getFormData()
        {
            return formData;
        }
    }
}
