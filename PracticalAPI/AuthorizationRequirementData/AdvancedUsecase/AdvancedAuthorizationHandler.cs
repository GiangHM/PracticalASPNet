using Microsoft.AspNetCore.Authorization;

namespace PracticalAPI.AuthorizationRequirementData.AdvancedUsecase
{
    public class AdvancedAuthorizationHandler : AuthorizationHandler<CombinationAuthorizationRequirement>
    {
        private readonly ILogger<AdvancedAuthorizationHandler> _logger;

        public AdvancedAuthorizationHandler(ILogger<AdvancedAuthorizationHandler> logger)
        {
            _logger = logger;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CombinationAuthorizationRequirement requirement)
        {
            {
                if (requirement == null)
                {
                    _logger.LogWarning("Authorization requirement is null.");
                    context.Fail(new AuthorizationFailureReason(this, "No authorization requirement provided."));
                    return Task.CompletedTask;
                }

                var userName = context.User?.Identity?.Name ?? "Anonymous";

                // Get all CustomAuthorizationRequirement instances from the context
                var allCustomRequirements = context.Requirements
                    .OfType<CombinationAuthorizationRequirement>()
                    .ToList();

                // If this is the first requirement being evaluated, handle combination logic
                if (allCustomRequirements.Count > 1 && requirement == allCustomRequirements.First())
                {
                    HandleMultipleRequirements(context, allCustomRequirements, userName);
                }
                else if (allCustomRequirements.Count == 1)
                {
                    // Single requirement - evaluate normally
                    HandleSingleRequirement(context, requirement, userName);
                }
                // Otherwise, this requirement is part of a multi-requirement set
                // and will be handled by the first requirement's evaluation

                return Task.CompletedTask;
            }
        }

        private void HandleSingleRequirement(
        AuthorizationHandlerContext context,
        CombinationAuthorizationRequirement requirement,
        string userName)
        {
            _logger.LogDebug(
                "Evaluating single authorization requirement for user {UserName}",
                userName);

            var result = requirement.Evaluate(context.User);

            if (result.Success)
            {
                _logger.LogDebug(
                    "Authorization succeeded for user {UserName}",
                    userName);

                context.Succeed(requirement);
            }
            else
            {
                _logger.LogWarning(
                    "Authorization failed for user {UserName}.  Reason: {FailureReason}",
                    userName,
                    result.FailureReason);

                context.Fail(new AuthorizationFailureReason(this, result.FailureReason ?? "Unknown failure"));
            }
        }

        private void HandleMultipleRequirements(
            AuthorizationHandlerContext context,
            List<CombinationAuthorizationRequirement> requirements,
            string userName)
        {
            _logger.LogDebug(
                "Evaluating {Count} combined authorization requirements for user {UserName}",
                requirements.Count,
                userName);

            // Group requirements by their Group property
            var grouped = requirements
                .GroupBy(r => r.Group ?? Guid.NewGuid().ToString()) // Ungrouped items get unique groups
                .ToList();

            // Check if any requirement has OR combination mode
            var hasOrMode = requirements.Any(r => r.CombinationMode == CombinationOperator.Or);

            if (hasOrMode && requirements.All(r => r.CombinationMode == CombinationOperator.Or))
            {
                // Pure OR mode: succeed if ANY requirement passes
                HandleOrCombination(context, requirements, userName);
            }
            else if (grouped.Count > 1)
            {
                // Group-based combination: OR within groups, AND between groups
                HandleGroupCombination(context, grouped, userName);
            }
            else
            {
                // Pure AND mode (default): all requirements must pass
                HandleAndCombination(context, requirements, userName);
            }
        }

        private void HandleOrCombination(
            AuthorizationHandlerContext context,
            List<CombinationAuthorizationRequirement> requirements,
            string userName)
        {
            var results = requirements.Select(r => (requirement: r, result: r.Evaluate(context.User))).ToList();

            var successfulRequirement = results.FirstOrDefault(r => r.result.Success);

            if (successfulRequirement.requirement != null)
            {
                _logger.LogDebug(
                    "Authorization succeeded (OR mode) for user {UserName} - at least one requirement satisfied",
                    userName);

                // Mark all requirements as succeeded since we're in OR mode
                foreach (var req in requirements)
                {
                    context.Succeed(req);
                }
            }
            else
            {
                var allFailures = string.Join(" OR ", results.Select(r => r.result.FailureReason));

                _logger.LogWarning(
                    "Authorization failed (OR mode) for user {UserName}. All requirements failed: {Failures}",
                    userName,
                    allFailures);

                context.Fail(new AuthorizationFailureReason(
                    this,
                    $"All authorization requirements failed (OR mode): {allFailures}"));
            }
        }

        private void HandleAndCombination(
            AuthorizationHandlerContext context,
            List<CombinationAuthorizationRequirement> requirements,
            string userName)
        {
            var results = requirements.Select(r => (requirement: r, result: r.Evaluate(context.User))).ToList();

            var failedRequirements = results.Where(r => !r.result.Success).ToList();

            if (!failedRequirements.Any())
            {
                _logger.LogDebug(
                    "Authorization succeeded (AND mode) for user {UserName} - all requirements satisfied",
                    userName);

                // Mark all requirements as succeeded
                foreach (var req in requirements)
                {
                    context.Succeed(req);
                }
            }
            else
            {
                var allFailures = string.Join(" AND ", failedRequirements.Select(r => r.result.FailureReason));

                _logger.LogWarning(
                    "Authorization failed (AND mode) for user {UserName}. Failed requirements: {Failures}",
                    userName,
                    allFailures);

                context.Fail(new AuthorizationFailureReason(
                    this,
                    $"Multiple authorization requirements failed (AND mode): {allFailures}"));
            }
        }

        private void HandleGroupCombination(
            AuthorizationHandlerContext context,
            List<IGrouping<string, CombinationAuthorizationRequirement>> groups,
            string userName)
        {
            var groupResults = new List<(string groupName, bool success, string? failureReason)>();

            foreach (var group in groups)
            {
                // Within each group, use OR logic
                var groupRequirements = group.ToList();
                var results = groupRequirements.Select(r => r.Evaluate(context.User)).ToList();

                var groupSuccess = results.Any(r => r.Success);
                var groupFailureReason = groupSuccess
                    ? null
                    : $"Group '{group.Key}': {string.Join(" OR ", results.Select(r => r.FailureReason))}";

                groupResults.Add((group.Key, groupSuccess, groupFailureReason));
            }

            // Between groups, use AND logic
            var failedGroups = groupResults.Where(g => !g.success).ToList();

            if (!failedGroups.Any())
            {
                _logger.LogDebug(
                    "Authorization succeeded (GROUP mode) for user {UserName} - all groups satisfied",
                    userName);

                // Mark all requirements as succeeded
                foreach (var req in groups.SelectMany(g => g))
                {
                    context.Succeed(req);
                }
            }
            else
            {
                var allFailures = string.Join(" AND ", failedGroups.Select(g => g.failureReason));

                _logger.LogWarning(
                    "Authorization failed (GROUP mode) for user {UserName}. Failed groups: {Failures}",
                    userName,
                    allFailures);

                context.Fail(new AuthorizationFailureReason(
                    this,
                    $"Authorization failed (GROUP mode): {allFailures}"));
            }
        }
    }
}
