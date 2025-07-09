using Xunit;

namespace BlogFlow.Tests;

public class SampleTests
{
    [Fact]
    public void Should_Add_Two_Numbers_Correctly()
    {
        var a = 2;
        var b = 3;

        var result = a + b;

        Assert.Equal(5, result);
    }
}
