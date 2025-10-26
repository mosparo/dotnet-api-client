![Icon](https://raw.githubusercontent.com/mosparo/mosparo/refs/heads/master/assets/images/mosparo-logo.svg)

# mosparo .NET API Client

This package provides an API client for communicating with mosparo to verify a submission.

-----

## Description

With this .NET package, you can connect to a mosparo installation and verify the submitted data.

## Installation with NuGet

    PM> Install-Package Mosparo.ApiClient

## Usage

1. Create a project in your mosparo installation
2. Include the mosparo script in your form
```html
<div id="mosparo-box"></div>

<script src="https://[URL]/build/mosparo-frontend.js" defer></script>
<script>
    var m;
    window.onload = function(){
        m = new mosparo('mosparo-box', 'https://[URL]', '[UUID]', '[PUBLIC_KEY]', {loadCssResource: true});
    };
</script>
```
3. Include the package in your project
4. After the form is submitted, verify the data before processing it
```csharp
SortedDictionary<string, IFormValue> formData = new SortedDictionary<string, IFormValue>()
{
    { "firstName", new StringFormValue("[FIELD_VALUE]") },
    { "lastName", new StringFormValue("[FIELD_VALUE]") },
};
FormRequest formRequest = new FormRequest("[SUBMIT_TOKEN]", "[VALIDATION_TOKEN]", formData);

ApiClient client = new ApiClient("https://[URL]", "[PUBLIC_KEY]", "[PRIVATE_KEY]");
VerificationResult result = client.verifySubmission(formRequest);

if (result.isSubmittable())
{
    // Send the email or process the data
} 
else
{
    // Show error message
}
```

Values written in square brackets are placeholders and need to be replaced. The values `[URL]`, `[UUID]`, `[PUBLIC_KEY]`, and `[PRIVATE_KEY]` are defined by your mosparo installation and your project in mosparo. `[SUBMIT_TOKEN]` and `[VALIDATION_TOKEN]` are two values that mosparo generates in the frontend when the user submits the form. These two values are sent with the POST HTTP request to your backend. `[FIELD_VALUE]` is the raw value of the field. The mosparo verification should occur before your normal form processing, since a processed form value can result in invalid form submissions.

### Customize HTTP request

If you need to customize the HTTP request, for example, to accept self-signed certificates, you can specify your own HTTP client and use it in `verifySubmission()` as the second argument:

```csharp
var handler = new HttpClientHandler();
handler.ClientCertificateOptions = ClientCertificateOption.Manual;
handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
{
    return true;
};
HttpClient httpClient = new HttpClient(handler);

VerificationResult result = client.verifySubmission(formRequest, httpClient);
```

## API Documentation

### `ApiClient`

#### Initialization

Create a new `ApiClient` object to use the API client:
```csharp
ApiClient client = new ApiClient(string url, string publicKey, string privateKey);
```

#### Verify form data

To verify the form data, call verifySubmission with a `FormRequest` object. The method will return a VerificationResult object. You can set the HTTP client, if required, as the second argument.

```csharp
client.verifySubmission(FormRequest formRequest);
client.verifySubmission(FormRequest formRequest, HttpClient httpClient);
```

### `FormRequest`

Represent the form request that mosparo should verify.

```csharp
FormRequest request = new FormRequest(string submitToken, string validationToken, SortedDictionary<string, IFormValue> formData);
```

### `IFormValue`

The package offers all possible types to represent all possible field values:

- `StringFormValue` (for any text field)
- `LongFormValue` (for integer number) 
- `DecimalFormValue` (for any decimal value)
- `BoolFormValue` (for `true` or `false`)
- `ArrayListFormValue` (for a list of `IFormValue` objects, for example, for a list of text values)
- `DictionaryFormValue` (as `SortedDictionary<string, IFormValue>`, for a subform, like `form[address][street]` and `form[address][number]`)
- `NullFormValue` (for `null` values)

### `VerificationResult`

Represent the result from the mosparo API.

#### `bool isSubmittable()`

Returns true if the data is valid and can be processed.

#### `bool isValid()`

Returns true if the form data is valid. Valid in this case means that the form data are not changed. A submission can still be not submittable, for example, when all fields are valid, but the process to generate the signatures generated a different signature.

#### `SortedDictionary<string, string> getVerifiedFields()`

Returns a `SortedDictionary` object with all the verified fields, with the field name as key and the field status as value.

#### `string getVerifiedField(string name)`

Returns the status of the field for the given name. Returns `not-verified`, if the field was not verified. Otherwise, returns `valid` if the field is valid or `invalid`, if it is invalid.

#### `ArrayList getIssues()`

Returns all issues encountered during submission verification.

#### `bool hasIssues()`

Returns true if any issues are present.

#### `object getDebugInformation()`

Returns the available debug information. The debug information needs to be enabled in the mosparo Project.

#### `string getDebugInformationAsString()`

Returns the debug information as a JSON string.

