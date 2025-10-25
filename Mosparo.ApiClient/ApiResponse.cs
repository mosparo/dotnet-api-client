using System.Collections;
using System.Collections.Generic;

namespace Mosparo.ApiClient
{
    public class ApiResponse
    {
        public bool valid { get; set; } = false;

        public string verificationSignature { get; set; }

        public SortedDictionary<string, string> verifiedFields { get; set; }

        public ArrayList issues { get; set; }

        public bool error { get; set; }

        public string errorMessage { get; set; }

        public object debugInformation { get; set; } = null;
    }
}
