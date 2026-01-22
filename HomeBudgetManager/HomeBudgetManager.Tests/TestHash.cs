using Xunit;
using Xunit.Abstractions; 
namespace HomeBudgetManager.Tests;
using HomeBudgetManager.Core;
public class TestHash
{
    private readonly ITestOutputHelper _output;

    // 3. Wstrzykujemy helper w konstruktorze
    public TestHash(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void ValidateHashing(){
        
        var Hasher = new HashPassword();

        List<string> passwords = new List<string>{"ala ma kota", "co innego", ""};
        
        foreach (string pass in passwords){
            string hash = Hasher.hash(pass);
            _output.WriteLine($"Hasło: '{pass}' -> Hash: {hash}");
            Assert.False(pass==hash);
        }
    }
}