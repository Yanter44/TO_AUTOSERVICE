using System.Security.Cryptography;
using System.Text;
using ToMainApi.Interfaces;

namespace ToMainApi.Services
{
    public static class CodeGeneratorService
    {
        public static string GenerateSixDigitCode()
        {
            var random = new Random();
            var builder = new StringBuilder();

            for (int i = 0; i < 6; i++)
            {
                int number = random.Next(0, 10);
                builder.Append(number);
            }
            return builder.ToString();
        }
    }
}
