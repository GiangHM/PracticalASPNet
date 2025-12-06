using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace PracticalAPI.AuthorizationRequirementData.AdvancedUsecase
{
    /// <summary>
    /// Interface for claim-based authorization requirements.
    /// </summary>
    public interface IClaimRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Evaluates whether the user satisfies this requirement.
        /// </summary>
        /// <param name="user">The claims principal to evaluate.</param>
        /// <returns>Result containing success status and failure reason if applicable.</returns>
        AuthorizationResult Evaluate(ClaimsPrincipal user);
    }

    /// <summary>
    /// Represents the result of an authorization evaluation.
    /// </summary>
    public class AuthorizationResult
    {
        public bool Success { get; init; }
        public string? FailureReason { get; init; }

        public static AuthorizationResult Succeeded()
            => new() { Success = true };

        public static AuthorizationResult Failed(string reason)
            => new() { Success = false, FailureReason = reason };
    }
    /// <summary>
    /// Represents a requirement to check specific claims with AND/OR logic.
    /// </summary>
    public class AdvancedClaimRequirement : IClaimRequirement
    {
        public string ClaimType { get; }
        public string[] RequiredValues { get; }
        public Operator Operator { get; }

        public AdvancedClaimRequirement(
            string claimType,
            string[] requiredValues,
            Operator op = Operator.Or)
        {
            if (string.IsNullOrWhiteSpace(claimType))
                throw new ArgumentException("Claim type cannot be null or empty.", nameof(claimType));

            ClaimType = claimType;
            RequiredValues = requiredValues ?? Array.Empty<string>();
            Operator = op;
        }

        public AuthorizationResult Evaluate(ClaimsPrincipal user)
        {
            if (user?.Identity?.IsAuthenticated != true)
            {
                return AuthorizationResult.Failed("User is not authenticated.");
            }

            if (RequiredValues.Length == 0)
            {
                return AuthorizationResult.Succeeded();
            }

            var userClaimValues = user.Claims
                .Where(c => c.Type.Equals(ClaimType, StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            return Operator switch
            {
                Operator.And => EvaluateAndOperator(userClaimValues),
                Operator.Or => EvaluateOrOperator(userClaimValues),
                _ => AuthorizationResult.Failed($"Unknown operator: {Operator}")
            };
        }

        private AuthorizationResult EvaluateAndOperator(HashSet<string> userClaimValues)
        {
            var missingClaims = RequiredValues.Where(rv => !userClaimValues.Contains(rv)).ToList();

            if (missingClaims.Any())
            {
                return AuthorizationResult.Failed(
                    $"User is missing required(s): {string.Join(", ", missingClaims)}.  " +
                    $"All of the following are required: {string.Join(", ", RequiredValues)}.");
            }

            return AuthorizationResult.Succeeded();
        }

        private AuthorizationResult EvaluateOrOperator(HashSet<string> userClaimValues)
        {
            if (RequiredValues.Any(rv => userClaimValues.Contains(rv)))
            {
                return AuthorizationResult.Succeeded();
            }

            return AuthorizationResult.Failed(
                $"User does not have any of the required(s): {string.Join(", ", RequiredValues)}.");
        }
    }

    /// <summary>
    /// Wrapper requirement that includes combination metadata for handling multiple attributes.
    /// </summary>
    public class CombinationAuthorizationRequirement : IAuthorizationRequirement
    {
        public IClaimRequirement InnerRequirement { get; }
        public CombinationOperator CombinationMode { get; }
        public string? Group { get; }

        public CombinationAuthorizationRequirement(
            IClaimRequirement innerRequirement,
            CombinationOperator combinationMode,
            string? group = null)
        {
            InnerRequirement = innerRequirement ?? throw new ArgumentNullException(nameof(innerRequirement));
            CombinationMode = combinationMode;
            Group = group;
        }

        public AuthorizationResult Evaluate(ClaimsPrincipal user)
        {
            return InnerRequirement.Evaluate(user);
        }
    }
}
