using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Mosparo.ApiClient
{
    public class RequestHelper
    {
        private string publicKey;

        private string privateKey;

        private JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            Converters =
                {
                    new FormValueConverterFactory(),
                }
        };

        public RequestHelper(string publicKey, string privateKey)
        {
            this.publicKey = publicKey;
            this.privateKey = privateKey;
        }

        public SortedDictionary<string, IFormValue> prepareFormData(SortedDictionary<string, IFormValue> formData)
        {
            SortedDictionary<string, IFormValue> cleanedFormData = cleanupFormData(formData);
            SortedDictionary<string, IFormValue> hashedFormData = new SortedDictionary<string, IFormValue>();

            foreach (var pair in cleanedFormData)
            {
                if (pair.Value is IDictionaryFormValue)
                {
                    SortedDictionary<string, IFormValue> value = prepareFormData(((IDictionaryFormValue)pair.Value).getValueAsDictionary());

                    hashedFormData.Add(pair.Key, new DictionaryFormValue(value));
                }
                else if (pair.Value is IArrayListFormValue)
                {
                    ArrayList hashedData = new ArrayList();
                    foreach (string value in ((IArrayListFormValue)pair.Value).getValueAsArrayList())
                    {
                        string hashValue = generateSha256Hash(value);

                        hashedData.Add(new StringFormValue(hashValue));
                    }

                    hashedFormData.Add(pair.Key, new ArrayListFormValue(hashedData));
                }
                else if (pair.Value is IStringFormValue)
                {
                    string hashValue = generateSha256Hash(((IStringFormValue)pair.Value).getValueAsString());

                    hashedFormData.Add(pair.Key, new StringFormValue(hashValue));
                }
            }

            return hashedFormData;
        }

        public SortedDictionary<string, IFormValue> cleanupFormData(SortedDictionary<string, IFormValue> formData)
        {
            SortedDictionary<string, IFormValue> cleanedFormData = new SortedDictionary<string, IFormValue>();

            foreach (var pair in formData)
            {
                if (pair.Key.StartsWith("_mosparo_"))
                {
                    continue;
                }
                
                if (pair.Value is IDictionaryFormValue)
                {
                    SortedDictionary<string, IFormValue> value = cleanupFormData(((IDictionaryFormValue)pair.Value).getValueAsDictionary());

                    cleanedFormData.Add(pair.Key, new DictionaryFormValue(value));
                }
                else if (pair.Value is IArrayListFormValue)
                {
                    ArrayList cleanedData = new ArrayList();
                    foreach (string value in ((IArrayListFormValue)pair.Value).getValueAsArrayList())
                    {
                        cleanedData.Add(value.Replace("\r\n", "\n"));
                    }

                    cleanedFormData.Add(pair.Key, new ArrayListFormValue(cleanedData));
                }
                else if (pair.Value is IStringFormValue)
                {
                    string value = ((IStringFormValue)pair.Value).getValueAsString();

                    value = value.Replace("\r\n", "\n");

                    cleanedFormData.Add(pair.Key, new StringFormValue(value));
                }
                else
                {
                    string value = pair.Value.getValue().ToString();

                    cleanedFormData.Add(pair.Key, new StringFormValue(value));
                }
            }

            return cleanedFormData;
        }

        public string generateHmacHash(string data)
        {
            using (HMACSHA256 hmacHash = new HMACSHA256(Encoding.UTF8.GetBytes(privateKey)))
            {
                byte[] bytes = hmacHash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string createFormDataHmacHash(SortedDictionary<string, IFormValue> preparedFormData)
        {
            string jsonData = toJson(preparedFormData);

            return generateHmacHash(jsonData);
        }

        public string generateSha256Hash(string data)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(data));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public string toJson(SortedDictionary<string, IFormValue> data)
        {
            string jsonData = JsonSerializer.Serialize(data, jsonSerializerOptions);

            return jsonData.Replace("[]", "{}");
        }
    }
}
