using Microsoft.AspNetCore.Authorization;
using System;

namespace PracticalAPI.AuthorizationRequirementData
{
    public class UserFeatureRequirement : IAuthorizationRequirement
    {
        public FeatureOperator Operator { get; private set; }
        public string[] Features { get; private set; }

        public UserFeatureRequirement(FeatureOperator featureOperator, string[] features)
        {
            if(features == null || features.Length == 0)
                throw new ArgumentException("At least one user feature is required", nameof(features));

            Operator = featureOperator;
            Features = features;
        }
    }
}
