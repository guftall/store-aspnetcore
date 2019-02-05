using System;
using System.Text;

namespace OnlineShopV1.Core
{
    public class Utilities
    {
        public static string GenerateAuthCode()
        {
            return Guid.NewGuid().ToString("N");
        }
        
        
        public static string GenerateSmsCode(string smsCodeSequence, int length)
        {
            return GenerateRandomString(smsCodeSequence, length);
        }
        
        public static bool CalculateWasLast(int limit, int offset, int count)
        {
            return limit * offset + limit >= count;
        }
        
        private static string GenerateRandomString(string sequence, int length)
        {
            if (string.IsNullOrWhiteSpace(sequence))
            {
                throw new ArgumentException($"sequence can't be empty string");
            }

            if (length < 1)
            {
                throw new ArgumentException("length must be greater than 0");
            }
            var random = new Random();
            var builder = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                builder.Append(sequence[random.Next(0, sequence.Length)]);
            }

            return builder.ToString();
        }
    }
}