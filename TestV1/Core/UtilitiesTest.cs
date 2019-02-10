using System;
using OnlineShopV1.Core;
using Xunit;

namespace TestV1
{
    public class UtilitiesTest
    {
        [Fact]
        public void GenerateSmsCode_CheckCodeLength()
        {
            const int codeLength = 5;
            var code = Utilities.GenerateSmsCode("123", codeLength);
            Assert.Equal(codeLength, code.Length);
        }
        
        [Fact]
        public void GenerateRandomString_InvalidArgument_ThrowsException()
        {
            Assert.Throws<ArgumentException>(() => Utilities.GenerateSmsCode("", 2));
            Assert.Throws<ArgumentException>(() => Utilities.GenerateSmsCode("123", 0));
        }
    }
}