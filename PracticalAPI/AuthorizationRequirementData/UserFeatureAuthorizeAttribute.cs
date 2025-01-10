using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace PracticalAPI.AuthorizationRequirementData
{
    public enum FeatureOperator
    {
        And = 1,
        Or = 2
    }
    public class UserFeatureAuthorizeAttribute : AuthorizeAttribute
        , IAuthorizationRequirement
        , IAuthorizationRequirementData
    {
        internal const string PolicyPrefix = "UserFeature_";
        private const string Separator = "_";
        public FeatureOperator Operator { get; set; }
        public string[] Features { get; set; }
        public UserFeatureAuthorizeAttribute(FeatureOperator featureOperator, params string[] features)
        { 
            Operator = featureOperator;
            Features = features;
            Policy = $"{PolicyPrefix}{(int)featureOperator}{Separator}{string.Join(Separator, features)}";
        }
        public UserFeatureAuthorizeAttribute(string feature)
        {
            Operator = FeatureOperator.And;
            Features[0] = feature;
            Policy = $"{PolicyPrefix}{(int)FeatureOperator.And}{Separator}{feature}";
        }
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return this;
        }
    }
}
