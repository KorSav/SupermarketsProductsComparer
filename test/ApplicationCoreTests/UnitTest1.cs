namespace ApplicationCoreTests;

public class UnitTest1
{
    [Fact]
    public void ConsiderThisDecimalFactInViews() //TODO
    {
        var a = 3m;
        var b = 3.00m;
        Assert.Equal(a, b);
        Assert.Equal(a.ToString("G29"), b.ToString("G29"));
        Assert.Equal(a.ToString(), b.ToString()); // but this fails
    }
}
