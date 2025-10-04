namespace TextEncrypterDecrypter.IntegrationTests.Common;

/// <summary>
/// Test categories for organizing and filtering tests
/// </summary>
public static class TestCategories
{
    /// <summary>
    /// Fast unit tests that can run frequently
    /// </summary>
    public const string Unit = "unit";
    
    /// <summary>
    /// Integration tests that may be slower
    /// </summary>
    public const string Integration = "integration";
    
    /// <summary>
    /// UI tests that require headless rendering
    /// </summary>
    public const string UI = "ui";
    
    /// <summary>
    /// Slow tests that should be run less frequently
    /// </summary>
    public const string Slow = "slow";
    
    /// <summary>
    /// Tests that require external dependencies
    /// </summary>
    public const string External = "external";
}
