using System.Collections;
using Xunit;

namespace Mosparo.ApiClient.Tests
{
    public class VerificationResultTests
    {
        [Fact()]
        public void generateHmacHashTest()
        {
            SortedDictionary<string, string> verifiedFields = new SortedDictionary<string, string>()
            {
                { "name", VerificationResult.VALID },
                { "street", VerificationResult.INVALID },
            };
            ArrayList issues = new ArrayList() {
                new Dictionary<string, string>()
                {
                    { "name", "street" },
                    { "message", "Missing in form data, verification not possible." }
                },
            };
            Dictionary<string, string> debugInformation = new Dictionary<string, string>()
            {
                { "expected", "test" },
                { "received", "test2" },
            };

            VerificationResult result = new VerificationResult(false, true, verifiedFields, issues, debugInformation);

            Xunit.Assert.False(result.isSubmittable());
            Xunit.Assert.True(result.isValid());
            Xunit.Assert.Equal(verifiedFields, result.getVerifiedFields());
            Xunit.Assert.Equal(VerificationResult.VALID, result.getVerifiedField("name"));
            Xunit.Assert.Equal(VerificationResult.INVALID, result.getVerifiedField("street"));
            Xunit.Assert.Equal(VerificationResult.NOT_VERIFIED, result.getVerifiedField("number"));
            Xunit.Assert.True(result.hasIssues());
            Xunit.Assert.Equal(issues, result.getIssues());
            Xunit.Assert.Equal(debugInformation, result.getDebugInformation());
        }
    }
}
