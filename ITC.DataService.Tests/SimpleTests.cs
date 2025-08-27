using NUnit.Framework;

namespace ITC.DataService.Tests;

[TestFixture]
public class SimpleTests
{
    [Test]
    public void SimpleTest_AlwaysPasses()
    {
        // Arrange & Act
        var result = 2 + 2;
        
        // Assert
        Assert.That(result, Is.EqualTo(4));
    }

    [Test]
    public void SimpleTest_StringOperations()
    {
        // Arrange
        var text = "Hello, World!";
        
        // Act
        var upperText = text.ToUpper();
        var containsHello = text.Contains("Hello");
        
        // Assert
        Assert.That(upperText, Is.EqualTo("HELLO, WORLD!"));
        Assert.That(containsHello, Is.True);
    }

    [Test]
    public void SimpleTest_CollectionOperations()
    {
        // Arrange
        var numbers = new[] { 1, 2, 3, 4, 5 };
        
        // Act
        var sum = numbers.Sum();
        var count = numbers.Length;
        
        // Assert
        Assert.That(sum, Is.EqualTo(15));
        Assert.That(count, Is.EqualTo(5));
    }
}
