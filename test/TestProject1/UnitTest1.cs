using System.Text;
using Hikari.Common.Cryptography;
using Hikari.Common.IO;

namespace TestProject1
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string configurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DbTool.config");

            var text = FileHelper.Read(configurationPath);
            var crypto = new AESCrypto("awerfdgg");
            var b = crypto.DecryptStr(text);
           
            //var dkProSet = System.Text.Json.JsonSerializer.Deserialize<DKProSet>(text);
            Assert.True(true);

        }
    }
}
