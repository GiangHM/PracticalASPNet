using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;

namespace PracticalAPI.AuthorizationRequirementData.AdvancedUsecase
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class AdvancedAuthorizeAttribute : AuthorizeAttribute, IAuthorizationRequirementData
    {
        private readonly IClaimRequirement _requirement;

        /// <summary>
        /// The type of claim to check (Role, Feature, or Identity).
        /// </summary>
        public ClaimTypeCheck TypeCheck { get; }

        /// <summary>
        /// The logical operator to apply to values within this attribute (And/Or).
        /// </summary>
        public Operator RequiredOperator { get; }

        /// <summary>
        /// The required claim values. 
        /// </summary>
        public string[] RequiredValues { get; }

        /// <summary>
        /// Defines how this attribute should be combined with other CustomAuthorize attributes.
        /// Default is And (all attributes must be satisfied).
        /// Set to Or if any one attribute should be sufficient.
        /// </summary>
        public CombinationOperator CombinationMode { get; set; } = CombinationOperator.And;

        /// <summary>
        /// Optional group identifier.  Attributes with the same group will be combined with OR logic,
        /// while different groups are combined with AND logic.
        /// </summary>
        /// <example>
        /// <code>
        /// // (Admin OR Manager) AND (DeletePermission OR EditPermission)
        /// [CustomAuthorize(ClaimTypeCheck.UserRole, "Admin", Group = "RoleGroup")]
        /// [CustomAuthorize(ClaimTypeCheck.UserRole, "Manager", Group = "RoleGroup")]
        /// [CustomAuthorize(ClaimTypeCheck.UserFeature, "DeletePermission", Group = "FeatureGroup")]
        /// [CustomAuthorize(ClaimTypeCheck.UserFeature, "EditPermission", Group = "FeatureGroup")]
        /// public IActionResult Action() { }
        /// </code>
        /// </example>
        public string? Group { get; set; }

        /// <summary>
        /// Creates a custom authorization attribute with specified claim type and values.
        /// </summary>
        /// <param name="typeCheck">The type of claim to verify.</param>
        /// <param name="requiredOperator">Logical operator for values within this attribute (default: Or).</param>
        /// <param name="values">Required claim values.</param>
        /// <exception cref="ArgumentException">Thrown when typeCheck is invalid or values are empty.</exception>
        public AdvancedAuthorizeAttribute(
            ClaimTypeCheck typeCheck,
            Operator requiredOperator = Operator.Or,
            params string[] values)
        {
            if (values == null || values.Length == 0)
            {
                throw new ArgumentException(
                    "At least one value must be provided for authorization check.",
                    nameof(values));
            }

            TypeCheck = typeCheck;
            RequiredOperator = requiredOperator;
            RequiredValues = values;

            // Create the appropriate requirement based on claim type
            _requirement = typeCheck switch
            {
                ClaimTypeCheck.UserRole => new AdvancedClaimRequirement(
                    CustomClaimTypes.Role,
                    values,
                    requiredOperator),

                ClaimTypeCheck.UserFeature => new AdvancedClaimRequirement(
                    CustomClaimTypes.Feature,
                    values,
                    requiredOperator),

                ClaimTypeCheck.Identity => new AdvancedClaimRequirement(
                    CustomClaimTypes.Identity,
                    values,
                    requiredOperator),

                _ => throw new ArgumentException(
                    $"Unsupported claim type check: {typeCheck}",
                    nameof(typeCheck))
            };
        }

        /// <summary>
        /// Convenience constructor using default OR operator.
        /// </summary>
        public AdvancedAuthorizeAttribute(ClaimTypeCheck typeCheck, params string[] values)
            : this(typeCheck, Operator.Or, values)
        {
        }

        /// <summary>
        /// Returns the authorization requirements for this attribute.
        /// If multiple attributes exist with CombinationMode or Groups, they will be combined accordingly.
        /// </summary>
        public IEnumerable<IAuthorizationRequirement> GetRequirements()
        {
            yield return new CombinationAuthorizationRequirement(_requirement, CombinationMode, Group);
        }
    }
}
