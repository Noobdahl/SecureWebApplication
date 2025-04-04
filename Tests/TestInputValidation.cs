using NUnit.Framework; // For NUnit testing framework

[TestFixture]
public class TestInputValidation
{
    [Test]
    public void TestForSQLInjection()
    {
        string maliciousInput = "'; DROP TABLE Users; --";
        string sanitizedInput = InputSanitizer.SanitizeInput(maliciousInput);

        Assert.That(sanitizedInput, Is.Not.EqualTo(maliciousInput));
        Assert.That(sanitizedInput, Does.Not.Contain("DROP TABLE"));
    }

    [Test]
    public void TestForXSS()
    {
        string maliciousInput = "<script>alert('XSS');</script>";
        string sanitizedInput = InputSanitizer.SanitizeInput(maliciousInput);

        Assert.That(sanitizedInput, Is.Not.EqualTo(maliciousInput));
        Assert.That(sanitizedInput, Does.Not.Contain("<script>"));
    }

    [Test]
    public void SanitizeInput_AllowsSafeCharacters()
    {
        // Arrange
        string safeInput = "user.name@example.com";
        string expectedOutput = "user.name@example.com"; // Safe input should remain unchanged

        // Act
        string sanitizedInput = InputSanitizer.SanitizeInput(safeInput);

        // Assert
        Assert.That(sanitizedInput, Is.EqualTo(expectedOutput));
    }

    [Test]
    public void SanitizeInput_RemovesUnsafeCharactersFromMixedInput()
    {
        // Arrange
        string mixedInput = "user.name@example.com<script>alert('XSS');</script> OR 1=1";
        string expectedOutput = "user.name@example.com"; // Only safe characters should remain

        // Act
        string sanitizedInput = InputSanitizer.SanitizeInput(mixedInput);

        // Assert
        Assert.That(sanitizedInput, Is.EqualTo(expectedOutput));
    }
}