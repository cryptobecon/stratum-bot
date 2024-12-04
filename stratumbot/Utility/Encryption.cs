using stratumbot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stratumbot
{
    class Encryption
    {
        public static string MD5(string _data)
        {
            byte[] encodedData = new UTF8Encoding().GetBytes(_data);
            byte[] hash = ((System.Security.Cryptography.HashAlgorithm)System.Security.Cryptography.CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedData);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        // Из HEX в строку
        public static string HEXToString(string _hex)
        {
            var icon = new byte[_hex.Length / 2];
            for (var i = 0; i < icon.Length; i++) { icon[i] = Convert.ToByte(_hex.Substring(i * 2, 2), 16); }
            return Encoding.Unicode.GetString(icon);
        }

        // Из HEX в число
        public static decimal HEXToNum(decimal _hex)
        {
            return decimal.Parse(HEXToString(_hex.ToString()));
        }

        // HexHex to int
        public static int HEXx2ToInt(string _hexX2)
        {
            return int.Parse(HEXToString(HEXToString(_hexX2)));
        }

        // string to XOR or XOR to string
        public static string XOR(string data)
        {
            return XORCoding(XORCoding(data, 666), Settings.SecretKey);
        }
        public static string XORCoding(string data, ushort secretKey)
        {
            var chars = data.ToArray();
            string newData = "";
            foreach (var c in chars)
                newData += XOROperation(c, secretKey);
            return newData;
        }
        public static char XOROperation(char character, ushort secretKey)
        {
            character = (char)(character ^ secretKey);
            return character;
        }
    }
}
