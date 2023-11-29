﻿using System.Security.Claims;
using IdentityModel;
using IdentityServer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace IdentityServer.Factories
{
    public class ApplicationUserClaimsPrincipleFactory : UserClaimsPrincipalFactory<ApplicationUser>
    {
        public ApplicationUserClaimsPrincipleFactory(UserManager<ApplicationUser> userManager, IOptions<IdentityOptions> optionsAccessor) : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var claimsIdentity = await base.GenerateClaimsAsync(user);

            if (user.GiveName is not null)
                claimsIdentity.AddClaim(new Claim(JwtClaimTypes.GivenName, user.GiveName));

            if (user.FamilyName is not null)
                claimsIdentity.AddClaim(new Claim(JwtClaimTypes.FamilyName, user.FamilyName));

            return claimsIdentity;
        }
    }
}
