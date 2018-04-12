// -----------------------------------------------------------------------
// <copyright file="ServicePrincipal.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace ServiceCatalog
{
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Web.Configuration;

    public static class ServicePrincipal
    {
        private static readonly string ClientId = WebConfigurationManager.AppSettings[ConfigurationConstants.ClientIdName];
        private static readonly string ClientSecret = WebConfigurationManager.AppSettings[ConfigurationConstants.ClientSecretName];
        private static readonly string AadInstance = WebConfigurationManager.AppSettings[ConfigurationConstants.AadInstanceName];
        private static readonly string ManagementResource = WebConfigurationManager.AppSettings[ConfigurationConstants.ManagementResourceName];
        private static readonly string Tenant = WebConfigurationManager.AppSettings[ConfigurationConstants.TenantName];
        private static readonly string Authority = string.Format(CultureInfo.InvariantCulture, AadInstance, Tenant);

        public static async Task<string> GetAccessToken()
        {
            ClientCredential clientCredential = new ClientCredential(ClientId, ClientSecret);
            AuthenticationContext context = new AuthenticationContext(Authority, false);
            AuthenticationResult authenticationResult = await context.AcquireTokenAsync(ManagementResource,clientCredential);
            
            return authenticationResult.AccessToken;
        }

        public static async Task<string> GetGraphAccessToken()
        {
            ClientCredential clientCredential = new ClientCredential(ClientId, ClientSecret);
            AuthenticationContext context = new AuthenticationContext(Authority, false);
            AuthenticationResult authenticationResult = await context.AcquireTokenAsync("https://graph.windows.net", clientCredential);

            return authenticationResult.AccessToken;
        }
    }
}