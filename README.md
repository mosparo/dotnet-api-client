![Icon](https://raw.githubusercontent.com/mosparo/mosparo/refs/heads/master/assets/images/mosparo-logo.svg)

# mosparo .NET API Client

This package provides an API client for communicating with mosparo to verify a submission.

-----

## Description

With this .NET package, you can connect to a mosparo installation and verify the submitted data.

## Installation with NuGet

    PM> Install-Package Mosparo.ApiClient

## Usage

### Standard integration

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


### Blazor Web App Interactive integration

Since Blazor Web Apps with Interactive as rendermode (and Blazor Server) establish a realtime Websocket connection between the client and the server, the integration with mosparo requires a different approach compared to standard form submissions.
The form is usually not sent with HTTP Post to the backend but is bound to a model on the server, where the values are stored and evaluated.

1. Create a project in your mosparo installation

2. Include the mosparo script in your form

```javascript
<script src="https://[URL]/build/mosparo-frontend.js" defer></script>
<script>
    var m;
    window.onload = function(){
        m = new mosparo('mosparo-box', 'https://[URL]', '[UUID]', '[PUBLIC_KEY]', {loadCssResource: true});
    };
</script>
```

Include this div in your form, where you want the mosparo box to be rendered.
```html
<div 
    style="margin:5px;"
    id="mosparo-box"
></div>
```

If your form is not visible when your page is loaded, consider this alternative approach:

Call the mosparo constructor when the form and the div is visible and fully rendered. 
You can use the `@ref` directive to get a reference to the div and check if it is rendered before calling the mosparo constructor.

In the mosparo constructor, set the callback function for the event `onCheckForm`. 
This event is triggered when the user checks the mosparo-box. In this callback function, you can set the values of the hidden fields `submit-token` 
and `validation-token` with the values from the mosparo instance. Then, you can dispatch a change event for both fields to trigger 
the Blazor data binding and update the model values on the server.

Be aware that in Blazor Web App Interactive javascript calls from the server are not possible, until the page is fully rendered (Event OnAfterRender/OnAfterRenderAsync).

```javascript
<script src="https://[URL]/build/mosparo-frontend.js" defer></script>

<script>
var m;

function LoadCaptcha(uuid, publicKey) {
    m = new mosparo('mosparo-box', 'https://[URL]', uuid, publicKey, {
        onCheckForm: function (result) {
            if (!result) {
                return;
            }

            let submitToken = document.getElementById('submit-token');
            let validationToken = document.getElementById('validation-token');

            submitToken.value = m.submitTokenElement.value;
            validationToken.value = m.validationTokenElement.value;

            submitToken.dispatchEvent(new Event('change'));
            validationToken.dispatchEvent(new Event('change'));
        },
        loadCssResource: true,
    });
}
</script>
```

##### Your page:
```html
@page "/myPage"
@inject IJSRuntime JavascriptRuntime

<!-- Your form goes here, see below-->

@code{
    
    private Model ModelValues = new Model();

    private ElementReference? CaptchaBoxElement;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && this.CaptchaBoxElement.Context != null)
        {
            await JavascriptRuntime.InvokeVoidAsync("LoadCaptcha", "[UUID]", "[PUBLIC_KEY]");
        }
    }


    private void OnSubmitButtonClick()
    {
        // Create FormRequest object with the model values and call the API client to verify the submission
    }
}

```


3. Add to your form two hidden input fields with the names `submit-token` and `validation-token` and the css class `mosparo__ignored-field`. 

These fields will be filled with the values from the mosparo instance when the user checks the mosparo box. 
The css class `mosparo__ignored-field` ensures that mosparo ignores these fields when verifying the form data.

The values of these fields are then sent to the server and can be used to create a `FormRequest` object for verification.

Note for using mosparo with MudBlazor UI components: MudBlazor should not be used for the two hidden fields, use standard HTML input fields instead.


##### Example form:
```html
<EditForm
    OnValidSubmit="@this.OnSubmitButtonClick"
    id="some-form"
    Model="@this.ModelValues"
    FormName="SomeForm"
>
    <div>
        <label 
            for="Name"
        >
            Name
        </label>

        <InputText 
            id="Name"
            name="Name"
            @bind-Value="@this.ModelValues.Name"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="EmailAdress"
        >
            Email-Adress
        </label>

        <InputText 
            id="EmailAdress"
            name="EmailAdress"
            @bind-Value="@this.ModelValues.EmailAdress"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="Subject"
        >
            Subject
        </label>

        <InputText 
            id="Subject"
            name="Subject"
            @bind-Value="@this.ModelValues.Subject"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="Message"
        >
            Message
        </label>

        <InputText 
            id="Message"
            name="Message"
            @bind-Value="@this.ModelValues.Message"
            autocomplete="off"
            max="300"
            required
        />
    </div>

    <div 
        style="margin:5px;"
        @ref="this.CaptchaBoxElement"
        id="mosparo-box"
    ></div>

    <input
        name="SubmitToken"
        type="hidden"
        autocomplete="off"
        class="mosparo__ignored-field"
        id="submit-token"
        @bind-value="@this.ModelValues.MosparoSubmitToken"
    >

    <input 
        name="ValidationToken"
        type="hidden"
        class="mosparo__ignored-field"
        autocomplete="off"
        id="validation-token"
        @bind-value="@this.ModelValues.MosparoValidationToken"
    >

    <button
        type="submit"
    >
        Absenden
    </button>

</EditForm>
```

<details>

<summary>Full Code Sample</summary>

```html
@page "/myPage"
@inject IJSRuntime JavascriptRuntime

<script src="https://[URL]/build/mosparo-frontend.js" defer></script>

<script>

var m;

function LoadCaptcha(uuid, publicKey) {
    m = new mosparo('mosparo-box', 'https://[URL]', uuid, publicKey, {
        onCheckForm: function (result) {
            if (!result) {
                return;
            }

            let submitToken = document.getElementById('submit-token');
            let validationToken = document.getElementById('validation-token');

            submitToken.value = m.submitTokenElement.value;
            validationToken.value = m.validationTokenElement.value;

            submitToken.dispatchEvent(new Event('change'));
            validationToken.dispatchEvent(new Event('change'));
        },
        loadCssResource: true,
    });
}

</script>

<EditForm
    OnValidSubmit="@this.OnSubmitButtonClick"
    id="some-form"
    Model="@this.ModelValues"
    FormName="SomeForm"
>
    <div>
        <label 
            for="Name"
        >
            Name
        </label>

        <InputText 
            id="Name"
            name="Name"
            @bind-Value="@this.ModelValues.Name"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="EmailAdress"
        >
            Email-Adress
        </label>

        <InputText 
            id="EmailAdress"
            name="EmailAdress"
            @bind-Value="@this.ModelValues.EmailAdress"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="Subject"
        >
            Subject
        </label>

        <InputText 
            id="Subject"
            name="Subject"
            @bind-Value="@this.ModelValues.Subject"
            autocomplete="off"
            max="50"
            required
        />
    </div>

    <div>
        <label 
            for="Message"
        >
            Message
        </label>

        <InputText 
            id="Message"
            name="Message"
            @bind-Value="@this.ModelValues.Message"
            autocomplete="off"
            max="300"
            required
        />
    </div>

    <div 
        style="margin:5px;"
        @ref="this.CaptchaBoxElement"
        id="mosparo-box"
    ></div>

    <input
        name="SubmitToken"
        type="hidden"
        autocomplete="off"
        class="mosparo__ignored-field"
        id="submit-token"
        @bind-value="@this.ModelValues.MosparoSubmitToken"
    >

    <input 
        name="ValidationToken"
        type="hidden"
        class="mosparo__ignored-field"
        autocomplete="off"
        id="validation-token"
        @bind-value="@this.ModelValues.MosparoValidationToken"
    >

    <button
        type="submit"
    >
        Absenden
    </button>

</EditForm>

@code{
    
    private Model ModelValues = new Model();

    private ElementReference? CaptchaBoxElement;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && this.CaptchaBoxElement.Context != null)
        {
            await JavascriptRuntime.InvokeVoidAsync("LoadCaptcha", "[UUID]", "[PUBLIC_KEY]");
        }
    }

    private void OnSubmitButtonClick()
    {
        // Create FormRequest object with the model values and call the API client to verify the submission
    }
}

```

</details>

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

