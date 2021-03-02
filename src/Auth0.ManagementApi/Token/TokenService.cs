using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using System;
using System.Threading.Tasks;

namespace Auth0.ManagementApi
{
    public class TokenService
    {
        private readonly string clientId;
        private readonly string clientSecret;
        private readonly string domain;

        private ITokenStorage storage;

        public TokenService(string clientId, string clientSecret, string domain)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;
            this.domain = domain;
            this.storage = new TokenStorage();
        }
        public async Task<string> GetToken()
        {
            var authClient = new AuthenticationApiClient(domain);
            var tokenStorageItem = this.storage.Get(clientId, domain);
            if(tokenStorageItem == null || !this.isValid(tokenStorageItem))
            {
                if (tokenStorageItem != null)
                {
                    storage.Remove(clientId, domain);
                }

                var token = await authClient.GetTokenAsync(new ClientCredentialsTokenRequest
                {
                    Audience = $"https://{domain}/api/v2/",
                    ClientId = clientId,
                    ClientSecret = clientSecret
                });


                this.storage.Save(clientId, domain, token);
                return token.AccessToken;
            }
            

            return tokenStorageItem.AccessTokenResponse.AccessToken;
        }

        private bool isValid(TokenStorageItem token)
        {
            var leewayInSeconds = 60;

            var diff = (DateTime.Now - token.ExpiresAt);

            return diff.TotalSeconds < leewayInSeconds;
        }
    }
}