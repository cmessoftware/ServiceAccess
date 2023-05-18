using Microsoft.Extensions.Configuration;
using ServiceAccess.Entities;
using System.Net;
using System.Security;
using static ServiceAccess.Helpers.Helper;

namespace ServiceAccess
{
    public class Communication
    {
        #region Properties

        public IConfiguration Configuration { get; }
        private string UserKey { get; }
        private string PasswordKey { get; }
        private string DomainKey { get; }
        private static string TimeOutKey { get; set; } = string.Empty;
        private string KeyVaultName { get; }
        private TimeSpan DataBaseTimeOut { get; }
        private CredentialsType CredentialType { get; }

        private int DefaulTimeOutMinutes = 10000;

        public enum HttpContentMethods
        {
            Put,
            Post
        }
        public enum HttpUrlMethods
        {
            Get,
            Delete
        }

        public enum AuthenticationTypes
        {
            NoCredentials,
            Anonymous,
            Basic,
            Digest,
            NTLM
        }

        public enum CredentialsType
        {
            ServerVariable,
            KeyVault
        }

        #endregion

        #region Constructors

        public Communication(IConfiguration configuration, string timeOutKeyValue, string userKey, string passwordKey, string domainKey, CredentialsType credentialType, string keyVaultName)
        {
            Configuration = configuration;
            TimeOutKey = timeOutKeyValue;
            UserKey = userKey;
            PasswordKey = passwordKey;
            DomainKey = domainKey;
            DataBaseTimeOut = TimeSpan.FromMinutes(string.IsNullOrEmpty(TimeOutKey) ? DefaulTimeOutMinutes : Data.GetConfigurationValue<int>(Configuration, TimeOutKey));
            CredentialType = credentialType;
            KeyVaultName = keyVaultName;
        }

        public Communication(IConfiguration configuration)
        {
            Configuration = configuration;
            DataBaseTimeOut = TimeSpan.FromMilliseconds(DefaulTimeOutMinutes);
        }

        public Communication(IConfiguration configuration, string timeOutKeyValue, string userKey, string passwordKey, string domainKey, CredentialsType credentialType)
            : this(configuration, timeOutKeyValue, userKey, passwordKey, domainKey, credentialType, string.Empty)
        {
        }

        #endregion

        #region Public Methods

        public async Task<ResponseBase<T>> GetServiceResponse<T>(HttpUrlMethods httpMethod, string url, string methodName)
            where T : class
        {
            Uri uri = GetUri(url, methodName);

            return await GetServiceResponse<T>(httpMethod, uri, AuthenticationTypes.NoCredentials);
        }

        public async Task<ResponseBase<T>> GetServiceResponse<T>(HttpUrlMethods httpMethod, string url, string methodName, AuthenticationTypes authenticationType)
            where T : class
        {
            Uri uri = GetUri(url, methodName);

            return await GetServiceResponse<T>(httpMethod, uri, GetClient(uri, authenticationType));
        }

        public async Task<ResponseBase<T>> GetServiceResponse<T>(HttpUrlMethods httpMethod, Uri uri)
            where T : class
        {
            return await GetServiceResponse<T>(httpMethod, uri, GetClient(uri, AuthenticationTypes.NoCredentials));
        }

        public async Task<ResponseBase<T>> GetServiceResponse<T>(HttpUrlMethods httpMethod, Uri uri, AuthenticationTypes authenticationType)
            where T : class
        {
            return await GetServiceResponse<T>(httpMethod, uri, GetClient(uri, authenticationType));
        }

        //public static async Task<ResponseBase<T>> GetServiceResponse<T>(HttpContentMethods httpMethod, string url, string methodName, string parameters)
        //    where T : class
        //    where T1 : class
        //{
        //    return await GetServiceResponse<T, T1>(httpMethod, GetUri(url, methodName), parameters);
        //}

        public async Task<ResponseBase<T>> GetServiceResponse<T, T1>(HttpContentMethods httpMethod, string url, string methodName, T1 request, AuthenticationTypes authenticationType)
            where T : class
            where T1 : class
        {
            Uri uri = GetUri(url, methodName);

            return await GetServiceResponse<T, T1>(httpMethod, uri, request, GetClient(uri, authenticationType));
        }

        public async Task<ResponseBase<T>> GetServiceResponse<T, T1>(HttpContentMethods httpMethod, Uri uri, T1 request)
            where T : class
            where T1 : class
        {
            return await GetServiceResponse<T, T1>(httpMethod, uri, request, GetClient(uri, AuthenticationTypes.NoCredentials));
        }

        public async Task<ResponseBase<T>> GetServiceResponse<T, T1>(HttpContentMethods httpMethod, Uri uri, T1 request, AuthenticationTypes authenticationType)
            where T : class
            where T1 : class
        {
            return await GetServiceResponse<T, T1>(httpMethod, uri, request, GetClient(uri, authenticationType));
        }

        #endregion

        #region Private Methods

        private static async Task<ResponseBase<T>> GetServiceResponse<T, T1>(HttpContentMethods httpMethod, Uri uri, T1 request, HttpClient client)
            where T : class
            where T1 : class
        {
            HttpResponseMessage response;

            switch (httpMethod)
            {
                case HttpContentMethods.Put:
                    response = await client.PutAsync(uri, ServicesHelper.GetContent(request));
                    break;
                case HttpContentMethods.Post:
                    response = await client.PostAsync(uri, ServicesHelper.GetContent(request));
                    break;
                default:
                    throw new NotImplementedException("The Http method invoked is not implemented");
            }
            if (response.IsSuccessStatusCode)
            {
                return ServicesHelper.CastHttpMessageBodyToEntity<T>(response);
            }
            throw new WebServiceException($"Status code: {response.StatusCode} received on web service call");
        }

        private static async Task<ResponseBase<T>> GetServiceResponse<T>(HttpUrlMethods httpMethod, Uri uri, HttpClient client)
            where T : class
        {
            HttpResponseMessage response;

            switch (httpMethod)
            {
                case HttpUrlMethods.Get:
                    response = await client.GetAsync(uri);
                    break;
                case HttpUrlMethods.Delete:
                    response = await client.DeleteAsync(uri);
                    break;
                default:
                    throw new NotImplementedException("The Http method invoked is not implemented");
            }
            if (response.IsSuccessStatusCode)
            {
                return ServicesHelper.CastHttpMessageBodyToEntity<T>(response);
            }
            throw new WebServiceException($"Status code: {response.StatusCode} received on web service call");
        }

        private HttpClient GetClient(Uri uri, AuthenticationTypes authenticationType)
        {
            NetworkCredential networkCredential;

            switch (authenticationType)
            {
                case AuthenticationTypes.Anonymous:
                    networkCredential = new NetworkCredential(null, null as string);
                    break;
                case AuthenticationTypes.Basic:
                    networkCredential = new NetworkCredential(GetUser(), GetPassword());
                    break;
                case AuthenticationTypes.Digest:
                    networkCredential = new NetworkCredential(GetUser(), GetPassword(), GetDomain());
                    break;
                case AuthenticationTypes.NTLM:
                    networkCredential = new NetworkCredential(GetUser(), GetPassword(), GetDomain());
                    break;
                case AuthenticationTypes.NoCredentials:
                default:
                    return new HttpClient() { Timeout = DataBaseTimeOut };
            }
            CredentialCache credentials = new()
                {
                    { uri, authenticationType.ToString(), networkCredential }
                };

            return new(new HttpClientHandler { Credentials = credentials }) { Timeout = DataBaseTimeOut };
        }

        private static Uri GetUri(string url, string methodName)
        {
            return new($"{url}/{methodName}");
        }

        private static SecureString GetSecureString(string value)
        {
            SecureString secureString = new();
            foreach (char c in value)
            {
                secureString.AppendChar(c);
            }
            secureString.MakeReadOnly();

            return secureString;
        }

        private string GetUser()
        {
            string value;
            switch (CredentialType)
            {
                case CredentialsType.ServerVariable:
                    value = Data.GetEnvironmentVariable(Data.GetConfigurationValue<string>(Configuration, UserKey));
                    break;
                case CredentialsType.KeyVault:
                    value = Data.GetKeyVaultSecret(KeyVaultName, UserKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Data.GetDecryptedValue(value);
        }
         
        private SecureString GetPassword()
        {
            string value;
            switch (CredentialType)
            {
                case CredentialsType.ServerVariable:
                    value = Data.GetEnvironmentVariable(Data.GetConfigurationValue<string>(Configuration, PasswordKey));
                    break;
                case CredentialsType.KeyVault:
                    value = Data.GetKeyVaultSecret(KeyVaultName, PasswordKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return GetSecureString(Data.GetDecryptedValue(value));
        }

        private string GetDomain()
        {
            string value;
            switch (CredentialType)
            {
                case CredentialsType.ServerVariable:
                    value = Data.GetEnvironmentVariable(Data.GetConfigurationValue<string>(Configuration, DomainKey));
                    break;
                case CredentialsType.KeyVault:
                    value = Data.GetKeyVaultSecret(KeyVaultName, DomainKey);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Data.GetDecryptedValue(value);
        }

        #endregion
    }
}