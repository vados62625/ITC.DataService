using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Flurl.Http.Configuration;
using Keycloak.Net.Core.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Keycloak.Net
{
    public partial class KeycloakClient
    {
        private ISerializer _serializer = new NewtonsoftJsonSerializer(new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
        });

        private readonly Url _url;
        private readonly string _userName;
        private readonly string _password;
        private readonly string _clientSecret;
        private readonly Func<string> _getToken;
        private readonly KeycloakOptions _options;

        private KeycloakClient(string url, KeycloakOptions options)
        {
            _url = url;
            _options = options ?? new KeycloakOptions();
            _options = options ?? new KeycloakOptions();
        }

        public KeycloakClient(string url, string userName, string password, KeycloakOptions options = null)
            : this(url, options)
        {
            _userName = userName;
            _password = password;
        }

        public KeycloakClient(string url, string clientSecret, KeycloakOptions options = null)
            : this(url, options)
        {
            _clientSecret = clientSecret;
        }

        public KeycloakClient(string url, Func<string> getToken, KeycloakOptions options = null)
            : this(url, options)
        {
            _getToken = getToken;
        }

        public void SetSerializer(ISerializer serializer)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        private async Task<IFlurlRequest> GetBaseUrl(string authenticationRealm)
        {
            var request = new Url(_url)
                .AppendPathSegment(_options.Prefix)
                .ConfigureRequest(settings => settings.JsonSerializer = _serializer);

            request = await request.WithAuthentication(_getToken, _url, _options.AuthenticationRealm ?? authenticationRealm, _userName,
                _password, _clientSecret, _options);

            return request;
        }
    }

    public class KeycloakOptions
    {
        public string Prefix { get; }
        public string AdminClientId { get; }
        /// <summary>
        /// It is used only when the authorization realm differs from the target one
        /// </summary>
        public string AuthenticationRealm { get; }
        
        public  
            KeycloakOptions(string adminClientId = "admin-cli", string prefix = "", string authenticationRealm = default)
        {
            Prefix = prefix.TrimStart('/').TrimEnd('/');
            AdminClientId = adminClientId;
            AuthenticationRealm = authenticationRealm;
        }
    }
}