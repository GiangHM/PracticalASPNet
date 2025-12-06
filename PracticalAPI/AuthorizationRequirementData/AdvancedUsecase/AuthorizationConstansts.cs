namespace PracticalAPI.AuthorizationRequirementData.AdvancedUsecase
{
    /// <summary>
    /// Defines how multiple CustomAuthorize attributes on the same method/class should be combined.
    /// </summary>
    public enum CombinationOperator
    {
        /// <summary>
        /// All CustomAuthorize requirements must be satisfied (default ASP.NET Core behavior). 
        /// User must have (Role=Admin) AND (Feature=DeletePermission). 
        /// </summary>
        And = 0,

        /// <summary>
        /// At least one CustomAuthorize requirement must be satisfied.
        /// User must have (Role=Admin) OR (Feature=DeletePermission).
        /// </summary>
        Or = 1
    }
    public enum Operator
    {
        And = 0,
        Or = 1,
    }
    public enum ClaimTypeCheck
    {
        UserRole = 0,
        UserFeature = 1,
        Identity = 2
    }
    /// <summary>
    /// Constants for custom claim types used in authorization. 
    /// </summary>
    public static class CustomClaimTypes
    {
        /// <summary>
        /// Standard role claim type from Microsoft identity. 
        /// </summary>
        public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";

        /// <summary>
        /// Standard name/identity claim type. 
        /// </summary>
        public const string Identity = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name";

        /// <summary>
        /// Custom feature claim type for feature-based authorization.
        /// </summary>
        public const string Feature = "feature";
    }
}
