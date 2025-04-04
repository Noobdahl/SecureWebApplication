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
}