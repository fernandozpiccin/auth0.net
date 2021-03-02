using Auth0.AuthenticationApi.Models;
using System;
using System.Collections.Generic;

namespace Auth0.ManagementApi
{
    public class TokenStorage : ITokenStorage
    {
        private Dictionary<string, TokenStorageItem> storage = new Dictionary<string, TokenStorageItem>();
        public TokenStorageItem Get(string clientId, string domain)
        {
            return storage.ContainsKey($"{domain}-{clientId}") ? storage[$"{domain}-{clientId}"] : null;
        }

        public void Remove(string clientId, string domain)
        {
            storage.Remove("{domain}-{clientId}");
        }

        public void Save(string clientId, string domain, AccessTokenResponse token)
        {
            var tokenStorageItem = new TokenStorageItem
            {
                AccessTokenResponse = token,
                ExpiresAt = DateTime.Now.AddSeconds(token.ExpiresIn)
            };
            if (storage.ContainsKey($"{domain}-{clientId}"))
            {
                storage[$"{domain}-{clientId}"] = tokenStorageItem;
            } else
            {

                storage.Add($"{domain}-{clientId}", tokenStorageItem);
            }
        }
    }
}