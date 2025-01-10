using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Threading.Tasks;
using System;

namespace PracticalAPI.AuthorizationRequirementData
{
    public class UserFeatureAuthorizationHandler : AuthorizationHandler<UserFeatureAuthorizeAttribute>
    {
        private readonly ILogger<UserFeatureAuthorizationHandler> _logger;
        private static string featureType = "feature";

        public UserFeatureAuthorizationHandler(ILogger<UserFeatureAuthorizationHandler> logger)
        {
            _logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   UserFeatureAuthorizeAttribute requirement)
        {
            // Log as a warning so that it's very clear in sample output which authorization
            // policies(and requirements/handlers) are in use.

            if (requirement.Operator == FeatureOperator.And)
            {
                foreach (var feature in requirement.Features)
                {
                    _logger.LogWarning("Evaluating authorization requirement for feature: {Feature}", feature);
                    if (!context.User.HasClaim(featureType, feature))
                    {
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }

                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            foreach (var feature in requirement.Features)
            {
                _logger.LogWarning("Evaluating authorization requirement for feature: {Feature}", feature);
                if (context.User.HasClaim(featureType, feature))
                {
                    context.Succeed(requirement);
                    return Task.CompletedTask;
                }
            }

            context.Fail();
            return Task.CompletedTask;
        }
    }
}
