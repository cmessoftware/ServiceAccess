using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Crypto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceAccess.Entities;
using System.Configuration;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml;
using System.Xml.Serialization;

namespace ServiceAccess.Helpers
{
    internal class Helper
    {
        internal struct ServicesHelper
        {
            internal static StringContent GetContent<T>(T request)
            where T : class
            {
                return new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json");
            }

            internal static ResponseBase<List<T>> CastHttpMessageBodyToEntity<T>(HttpResponseMessage messageBody)
               where T : class
            {
                var response = new ResponseBase<T>();
                var result = messageBody.Content.ReadAsStringAsync().Result;
                response.StatusCode = Convert.ToInt32(messageBody.StatusCode);
                response.ReasonPhrase = messageBody.ReasonPhrase;
                response.Lista = new List<T>();

                var resultT = JsonConvert.DeserializeObject<T>(result);

                if (resultT is IEnumerable<T>)
                    response.Lista = JsonConvert.DeserializeObject<IList<T>>(result);
                else
                    response.Lista = new List<T>() { resultT };

                //var result = new JObject()
                //{
                //    { JsonConvert.DeserializeObject<T>(messageBody.Content.ReadAsStringAsync().Result) ?? throw new CastException("T object can't be null when you're trying to cast its type") },
                //    { "StatusCode", JToken.FromObject(messageBody.StatusCode) },
                //    { "ReasonPhrase", messageBody.ReasonPhrase }
                //};

                return response;
            }

            internal static ResponseBase<T> CastHttpMessageBodyToEntity<T>(HttpResponseMessage messageBody)
                where T : class
            {
                var response = new ResponseBase<T>();
                var result = messageBody.Content.ReadAsStringAsync().Result;
                response.StatusCode = Convert.ToInt32(messageBody.StatusCode);
                response.ReasonPhrase = messageBody.ReasonPhrase;
                response.Lista = new List<T>();

                var resultT = JsonConvert.DeserializeObject<T>(result);

                if (resultT is IEnumerable<T>)
                    response.Lista = JsonConvert.DeserializeObject<IList<T>>(result);
                else
                    response.Lista = new List<T>() { resultT };
          
                //var result = new JObject()
                //{
                //    { JsonConvert.DeserializeObject<T>(messageBody.Content.ReadAsStringAsync().Result) ?? throw new CastException("T object can't be null when you're trying to cast its type") },
                //    { "StatusCode", JToken.FromObject(messageBody.StatusCode) },
                //    { "ReasonPhrase", messageBody.ReasonPhrase }
                //};

                return response;
            }

            internal static T CastXmlToEntity<T>(string xml)
               where T : class
            {
                XmlSerializer serializer = new(typeof(T));

                FileStream fs = new(xml, FileMode.OpenOrCreate);
                TextReader reader = new StreamReader(fs);

                return serializer.Deserialize(reader) as T ?? throw new CastException("T object can't be null when you're trying to cast its type");
            }

            internal static string SerializeXml<T>(T request)
                where T : class
            {
                using StringWriter xml = new();
                new XmlSerializer(typeof(T)).Serialize(XmlWriter.Create(xml), request);

                return xml.ToString();
            }

            internal static string SerializeXmlRemoveSchema<T>(T request)
                where T : class
            {
                XmlSerializerNamespaces xmlNamespace = new();
                xmlNamespace.Add("", "");
                StringWriter stringWriter = new();
                XmlSerializer xmlSerializer = new(typeof(T));
                xmlSerializer.Serialize(stringWriter, request, xmlNamespace);

                return stringWriter.ToString();
            }
        }

        internal struct Data
        {
            internal static T ParseEnum<T>(string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

            internal static string GetDecryptedValue(string value)
            {
                return new Cryptography().Decrypt(value);
            }

            //internal static T GetConfigurationValue<T>(IConfiguration configuration, 
            //                                           string key)
            //{
            
            //    if (string.IsNullOrEmpty(key))
            //        throw new ConfigurationErrorsException("Configuration key wasn't found");
                
            //    if ((T)configuration.GetSection(key) == null)
            //        throw new ConfigurationErrorsException($"Configuration key {key} doesn't have a value or doesn't exist on configuration file");

            //    var configurationValue = configuration.GetSection(key);

            //    Type typeInt = typeof(int);
            //    Type typeString = typeof(string);
            //    Type typFloat = typeof(float);

            //    return (T)configurationValue;

            //    //switch (typeof(T))
            //    //{
            //    //    case :
            //    //        configurationValue = (int)configurationValue;
            //    //        break;

            //    //}
            //}


            internal static T GetConfigurationValue<T>(IConfiguration configuration, string key)
            {
                return string.IsNullOrEmpty(key)
                ? throw new ConfigurationErrorsException("Configuration key doesn't have a value or doesn't exist on configuration file")
                : (T)configuration.GetSection(key) ?? throw new ConfigurationErrorsException("Configuration key wasn't found");
            }

            internal static string GetEnvironmentVariable(string variable)
            {
                return string.IsNullOrEmpty(variable)
                    ? throw new EnvironmentVariableException("Environment variable key can't be blank or null")
                    : Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine) ?? string.Empty;
            }

            internal static string GetKeyVaultSecret(string keyVaultName, string secretName)
            {
                return string.IsNullOrEmpty(keyVaultName)
                    ? throw new KeyVaultEception("Key vault value can't be blank or null")
                    : new SecretClient(new($"https://{keyVaultName}.vault.azure.net/"), new DefaultAzureCredential()).GetSecretAsync(secretName).Result.Value.Value;
            }
        }
    }
}