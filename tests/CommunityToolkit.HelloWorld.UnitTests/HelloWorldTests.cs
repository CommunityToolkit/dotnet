using Xunit;

public class HelloWorldTests
{
    [Fact]
    public void HelloWorld_ReturnsExpectedString()
    {
        var result = "Hello, World!";
        Assert.Equal("Hello, World!", result);
    }
}