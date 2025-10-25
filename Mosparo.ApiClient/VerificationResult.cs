using System.Collections;
using System.Collections.Generic;

namespace Mosparo.ApiClient
{
    public class VerificationResult
    {
        public const string NOT_VERIFIED = "not-verified";
        public const string VALID = "valid";
        public const string INVALID = "invalid";

        private bool submittable;

        private bool valid;

        private SortedDictionary<string, string> verifiedFields;

        private ArrayList issues;

        private object debugInformation;

        public VerificationResult(bool submittable, bool valid, SortedDictionary<string, string> verifiedFields, ArrayList issues, object debugInformation)
        {
            this.submittable = submittable;
            this.valid = valid;
            this.verifiedFields = verifiedFields;
            this.issues = issues;
            this.debugInformation = debugInformation;
        }

        public bool isSubmittable()
        {
            return submittable;
        }

        public bool isValid()
        {
            return valid;
        }

        public SortedDictionary<string, string> getVerifiedFields()
        {
            return verifiedFields;
        }

        public string getVerifiedField(string name)
        {
            bool result = verifiedFields.TryGetValue(name, out string fieldVal);

            if (result)
            {
                return fieldVal;
            }

            return NOT_VERIFIED;
        }

        public ArrayList getIssues()
        {
            return issues;
        }

        public bool hasIssues()
        {
            return issues.Count > 0;
        }

        public object getDebugInformation()
        {
            return debugInformation;
        }
    }
}
