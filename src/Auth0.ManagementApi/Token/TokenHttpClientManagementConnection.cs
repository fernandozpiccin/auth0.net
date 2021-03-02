using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Auth0.ManagementApi
{
    /// <summary>
    /// Implements <see cref="IManagementConnection"/> using <see cref="HttpClient"/>.
    /// </summary>
    public class TokenHttpClientManagementConnection : IManagementConnection, IDisposable
    {
        IManagementConnection connection;
        TokenService tokenService;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpClientManagementConnection"/> class.
        /// </summary>
        /// <param name="httpClient">Optional <see cref="HttpClient"/> to use. If not specified one will
        /// be created and be used for all requests made by this instance.</param>
        public TokenHttpClientManagementConnection(string clientId, string clientSecret, string domain, IManagementConnection connection)
        {
            this.connection = connection;
            this.tokenService = new TokenService(clientId, clientSecret, domain);
        }


        /// <inheritdoc />
        public async Task<T> GetAsync<T>(Uri uri, IDictionary<string, string> headers, JsonConverter[] converters = null)
        {
            var token = await tokenService.GetToken();

            if (!string.IsNullOrEmpty(token))
            {
                if (headers.ContainsKey("Authorization"))
                {
                    headers["Authorization"] = $"Bearer {token}";

                } else
                {
                    headers.Add("Authorization", $"Bearer {token}");
                }
            }
                

            return await connection.GetAsync<T>(uri, headers, converters);
        }

        /// <inheritdoc />
        public async Task<T> SendAsync<T>(HttpMethod method, Uri uri, object body, IDictionary<string, string> headers, IList<FileUploadParameter> files = null)
        {
            var token = await tokenService.GetToken();

            if (!string.IsNullOrEmpty(token))
                headers.Add("Authorization", $"Bearer {token}");

            return await connection.SendAsync<T>(method, uri, body, headers, files);
        }

        /// <summary>
        /// Disposes of any owned disposable resources such as the underlying <see cref="HttpClient"/> if owned.
        /// </summary>
        public void Dispose()
        {
            if (connection is IDisposable)
            {
                (connection as IDisposable).Dispose();
            }
        }
    }
}