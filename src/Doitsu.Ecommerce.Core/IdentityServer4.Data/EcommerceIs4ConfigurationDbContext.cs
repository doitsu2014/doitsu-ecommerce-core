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
using IdentityClaim = IdentityServer4.EntityFramework.Entities.IdentityClaim;
using IdentityProperty = IdentityServer4.EntityFramework.Entities.IdentityResourceProperty;
using System.Linq;

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
            const string spaAdminApp = "2d916f81-43b0-42eb-b6ea-750a5ab7d3cc";
            const string mvcFrontEndApp = "68dcf419-d41b-4af6-9222-cfa0be6cb347";
            #endregion

            var now = new DateTime(2020, 06, 14, 00, 00, 00, 000, DateTimeKind.Utc).ToVietnamDateTime();

            #region ApiResource

            modelBuilder.Entity<ApiResource>().HasData(
                CreateApiResource(1, "doitsu_ecommerce_system", "Doitsu Ecommerce System", now));

            modelBuilder.Entity<ApiScope>().HasData(
                CreateApiScope(1, 1, Constants.EcommerceIs4Scopes.MANAGEMENT, "Doitsu Ecommerce System Management Scope"),
                CreateApiScope(2, 1, Constants.EcommerceIs4Scopes.USER, "Doitsu Ecommerce System User Scope"));

            #endregion

            #region Client

            modelBuilder.Entity<Client>().HasData(
                CreateClient(1, spaAdminApp, "doitsu.ecommerce.spa_admin_app", defaultAccessTokenLifetime, now, false),
                CreateClient(2, mvcFrontEndApp, "doitsu.ecommerce.mvc_front_end_app", defaultAccessTokenLifetime, now, false));

            modelBuilder.Entity<ClientGrantType>().HasData(
                CreateClientGrantType(1, 1, GrantType.Hybrid),
                CreateClientGrantType(2, 1, GrantType.ResourceOwnerPassword),
                CreateClientGrantType(3, 1, GrantType.AuthorizationCode),
                CreateClientGrantType(4, 2, GrantType.Hybrid),
                CreateClientGrantType(5, 2, GrantType.ClientCredentials));

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
                new ClientRedirectUri() { Id = 1, RedirectUri = "https://localhost:5001/signin-oidc", ClientId = 2 }
            );

            modelBuilder.Entity<ClientPostLogoutRedirectUri>().HasData(
                new ClientPostLogoutRedirectUri() { Id = 1, PostLogoutRedirectUri = "https://localhost:5001/signout-oidc", ClientId = 2 }
            );

            #endregion


            #region Identity Resource
            var openIdIdentityResource = new IdentityResources.OpenId();
            var profileIdentityResource = new IdentityResources.Profile();

            modelBuilder.Entity<IdentityResource>().HasData(
               new IdentityResource()
               {
                   Id = 1,
                   Name = openIdIdentityResource.Name,
                   DisplayName = openIdIdentityResource.DisplayName,
                   Enabled = openIdIdentityResource.Enabled,
                   Emphasize = openIdIdentityResource.Emphasize,
                   Required = openIdIdentityResource.Required,
                   ShowInDiscoveryDocument = openIdIdentityResource.ShowInDiscoveryDocument,
                   Created = now,
                   Description = openIdIdentityResource.Description,

               },
                new IdentityResource()
                {
                    Id = 2,
                    Name = profileIdentityResource.Name,
                    DisplayName = profileIdentityResource.DisplayName,
                    Enabled = profileIdentityResource.Enabled,
                    Emphasize = profileIdentityResource.Emphasize,
                    Required = profileIdentityResource.Required,
                    ShowInDiscoveryDocument = profileIdentityResource.ShowInDiscoveryDocument,
                    Created = now,
                    Description = profileIdentityResource.Description
                }
            );

            var identityResourceClaimCount = 0;
            modelBuilder.Entity<IdentityClaim>().HasData(
                openIdIdentityResource.UserClaims.Select(uc =>
                {
                    ++identityResourceClaimCount;
                    return new IdentityClaim()
                    {
                        Id = identityResourceClaimCount,
                        IdentityResourceId = 1,
                        Type = uc
                    };
                }));

            modelBuilder.Entity<IdentityClaim>().HasData(
                profileIdentityResource.UserClaims.Select(uc =>
                {
                    ++identityResourceClaimCount;
                    return new IdentityClaim()
                    {
                        Id = identityResourceClaimCount,
                        IdentityResourceId = 2,
                        Type = uc
                    };
                }));

            var identityResourcePropertyCount = 0;
            modelBuilder.Entity<IdentityResource>().HasData(
                openIdIdentityResource.Properties.Select(prop =>
                {
                    return new IdentityProperty()
                    {
                        Id = ++identityResourcePropertyCount,
                        IdentityResourceId = 1,
                        Key = prop.Key,
                        Value = prop.Value
                    };
                })
            );

            modelBuilder.Entity<IdentityProperty>().HasData(
                profileIdentityResource.Properties.Select(prop =>
                {
                    return new IdentityProperty()
                    {
                        Id = ++identityResourcePropertyCount,
                        IdentityResourceId = 2,
                        Key = prop.Key,
                        Value = prop.Value
                    };
                }));
            #endregion
        }

        #region Helpers

        protected Client CreateClient(int id,
                                      string clientId,
                                      string name,
                                      int accessTokenLifetime,
                                      DateTime created,
                                      bool allowOfflineAccess = true) =>
            new Client
            {
                Id = id,
                Enabled = true,
                ClientId = clientId,
                ProtocolType = "oidc",
                RequireClientSecret = true,
                ClientName = name,
                RequireConsent = false,
                AllowRememberConsent = false,
                FrontChannelLogoutSessionRequired = false,
                BackChannelLogoutSessionRequired = false,
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
                Created = created,
                RedirectUris = {},
                PostLogoutRedirectUris = {},
                FrontChannelLogoutUri = {},
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