using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Abstraction;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Options;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;

namespace Doitsu.Ecommerce.Core.IdentityServer4.Data
{
    public class EcommerceIs4ConfigurationDbContext : ConfigurationDbContext
    {
        public EcommerceIs4ConfigurationDbContext(DbContextOptions<ConfigurationDbContext> options, ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.HasDefaultSchema("is4cf");

            #region Constants
            const int defaultAccessTokenLifetime = 28800;
            const string managerClientId = "2d916f81-43b0-42eb-b6ea-750a5ab7d3cc";
            const string userClientId = "68dcf419-d41b-4af6-9222-cfa0be6cb347";
            #endregion

            var now = new DateTime(2020, 06, 14, 00, 00, 00, 000, DateTimeKind.Utc).ToVietnamDateTime();

            #region ApiResource

            modelBuilder.Entity<ApiResource>().HasData(
                CreateApiResource(1, "doitsu_ecommerce_system", "Doitsu Ecommerce System", now));

            modelBuilder.Entity<ApiScope>().HasData(
                CreateApiScope(1, 1, Constants.EcommerceIs4Scopes.MANAGEMENT, "Doitsu Ecommerce System Management Scope"),
                CreateApiScope(2, 1, Constants.EcommerceIs4Scopes.USER, "Doitsu Ecommerce System User Scope"),
                CreateApiScope(3, 1, IdentityServerConstants.StandardScopes.OpenId, "Doitsu Ecommerce System OpenId"),
                CreateApiScope(4, 1, IdentityServerConstants.StandardScopes.Profile, "Doitsu Ecommerce System User Scope"));

            #endregion

            #region Client

            modelBuilder.Entity<Client>().HasData(
                CreateClient(1, managerClientId, "Manager Client", defaultAccessTokenLifetime, now, false),
                CreateClient(2, userClientId, "User Client", defaultAccessTokenLifetime, now, false));

            modelBuilder.Entity<ClientGrantType>().HasData(
                CreateClientGrantType(1, 1, GrantType.ResourceOwnerPassword),
                CreateClientGrantType(2, 2, GrantType.ResourceOwnerPassword));

            modelBuilder.Entity<ClientScope>().HasData(
                CreateClientScope(1, 1, IdentityServerConstants.StandardScopes.OpenId),
                CreateClientScope(2, 1, IdentityServerConstants.StandardScopes.Profile),
                CreateClientScope(3, 1, Constants.EcommerceIs4Scopes.MANAGEMENT),
                CreateClientScope(4, 1, Constants.EcommerceIs4Scopes.USER),
                CreateClientScope(5, 2, IdentityServerConstants.StandardScopes.OpenId),
                CreateClientScope(6, 2, IdentityServerConstants.StandardScopes.Profile),
                CreateClientScope(7, 2, Constants.EcommerceIs4Scopes.USER));

            modelBuilder.Entity<ClientSecret>().HasData(
                CreateClientSecret(1, 1, "R7(Lf(W9V]{P?Q?Z3AFeUt,3", now),
                CreateClientSecret(2, 2, "gCU_$w,&4E7L'YgjYuDG:.$)", now)
            );

            modelBuilder.Entity<ClientRedirectUri>().HasData(
                new ClientRedirectUri() { Id = 1, RedirectUri = "/nguoi-dung/dang-nhap", ClientId = 2 }
            );

            modelBuilder.Entity<ClientPostLogoutRedirectUri>().HasData(
                new ClientPostLogoutRedirectUri() { Id = 1, PostLogoutRedirectUri = "/nguoi-dung/dang-xuat", ClientId = 2 } 
            );

            #endregion
        }

        #region Helpers

        protected Client CreateClient(int id,
                                      string clientId,
                                      string name,
                                      int accessTokenLifetime,
                                      DateTime created,
                                      bool allowOfflineAccess = true,
                                      string clientClaimsPrefix = "client_") =>
            new Client
            {
                Id = id,
                Enabled = true,
                ClientId = clientId,
                ProtocolType = "oidc",
                RequireClientSecret = true,
                ClientName = name,
                RequireConsent = true,
                AllowRememberConsent = true,
                FrontChannelLogoutSessionRequired = true,
                BackChannelLogoutSessionRequired = true,
                AllowOfflineAccess = allowOfflineAccess,
                IdentityTokenLifetime = 300,
                AccessTokenLifetime = 28800,
                AuthorizationCodeLifetime = 300,
                AbsoluteRefreshTokenLifetime = 2592000,
                SlidingRefreshTokenLifetime = 1296000,
                RefreshTokenUsage = 1,
                RefreshTokenExpiration = 1,
                AccessTokenType = 0,
                EnableLocalLogin = true,
                ClientClaimsPrefix = clientClaimsPrefix,
                Created = created
            };

        protected ClientGrantType CreateClientGrantType(int id, int clientId, string grantType) =>
            new ClientGrantType { Id = id, ClientId = clientId, GrantType = grantType };

        protected ClientScope CreateClientScope(int id, int clientId, string scope) =>
            new ClientScope { Id = id, ClientId = clientId, Scope = scope };

        protected ClientSecret CreateClientSecret(int id, int clientId, string secret, DateTime created, string type = "SharedSecret", DateTime? expired = null) =>
            new ClientSecret { Id = id, ClientId = clientId, Value = secret.Sha256(), Created = created, Type = type, Expiration = expired };

        protected ApiResource CreateApiResource(int id, string name, string displayName, DateTime created) =>
            new ApiResource { Id = id, Name = name, DisplayName = displayName, Created = created, Enabled = true };

        protected ApiScope CreateApiScope(int id, int apiResourceId, string name, string displayName) =>
            new ApiScope { Id = id, ApiResourceId = apiResourceId, Name = name, DisplayName = displayName, ShowInDiscoveryDocument = true };

        #endregion
    }
}