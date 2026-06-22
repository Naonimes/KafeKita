using System;
using System.Security.Cryptography;
using System.Text;

namespace TugasBesarKPL_Solution
{
    public class PaymentModule
    {
        public string GenerateReceiptID()
        {
            string rawStr = Guid.NewGuid().ToString();
            byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawStr));
            return Convert.ToHexString(bytes).Substring(0, 8);
        }
    }
}
