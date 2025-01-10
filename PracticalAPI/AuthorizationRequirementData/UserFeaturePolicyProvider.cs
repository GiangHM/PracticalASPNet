using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using System;

namespace PracticalAPI.AuthorizationRequirementData
{
    public class UserFeaturePolicyProvider : IAuthorizationPolicyProvider
    {
        const string POLICY_PREFIX = "UserFeature";
        private const string Separator = "_";

        public DefaultAuthorizationPolicyProvider FallbackPolicyProvider { get; }
        public UserFeaturePolicyProvider(IOptions<AuthorizationOptions> options)
        {
            FallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }
        public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
                                FallbackPolicyProvider.GetDefaultPolicyAsync();
        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
                                FallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var policy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme);

                FeatureOperator @operator = GetOperatorFromPolicy(policyName);

                // Will extract the permissions from the string (Create, Update..)
                string[] featues = GetPermissionsFromPolicy(policyName);

                policy.AddRequirements(new UserFeatureRequirement(@operator, featues));
                return Task.FromResult<AuthorizationPolicy?>(policy.Build());
            }

            return Task.FromResult<AuthorizationPolicy?>(null);
        }

        public static FeatureOperator GetOperatorFromPolicy(string policyName)
        {
            var @operator = int.Parse(policyName.AsSpan(POLICY_PREFIX.Length, 1));
            return (FeatureOperator)@operator;
        }

        public static string[] GetPermissionsFromPolicy(string policyName)
        {
            return policyName.Substring(POLICY_PREFIX.Length + 2)
                .Split(new[] { Separator }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
