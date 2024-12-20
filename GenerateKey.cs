using System;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Elliptic_curve;

/* 
    Выбор параметров.

    1. a и b должны быть любыми, только чтобы соблюдалось неравенство 4*a^3+27*b^2 != 0
    2. n должно быть большим простым положительным числом.
    3. P должна являться точкой на этой кривой, причем порядка n.

    Bitcoin в своей сети использует параметры (будем использовать их)
    a=0, b=7,
    n=115792089237316195423570985008687907853269984665640564039457584007908834671663, 256 бит
    P=x, y, где
    x=55066263022277343669578718895168534326250603453777594175500187360389116729240, 255 бит
    y=32670510020758816978083085130507043184471273380659243275938904335757337482424, 255 бит
    n=115792089237316195423570985008687907852837564279074904382605163141518161494337
*/

namespace SendingKeyEDS
{
    public static class GenerateKey
    {
        private static readonly RandomNumberGenerator rng = RandomNumberGenerator.Create();
        private static readonly Point P = new(BigInteger.Parse("55066263022277343669578718895168534326250603453777594175500187360389116729240"), BigInteger.Parse("32670510020758816978083085130507043184471273380659243275938904335757337482424"), 0, 7, BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663"));
        private static readonly BigInteger n = BigInteger.Parse("115792089237316195423570985008687907852837564279074904382605163141518161494337");
        private static BigInteger Key;
        private static Magma Crypter;
        public static void SetKey(BigInteger Keys)
        {
            Key = Keys;
        }

        public static Point GetP => P;
        public static RandomNumberGenerator GetRng => rng;
        public static BigInteger GetN => n;

        public static void Generate(BigInteger X, BigInteger Y)
        {
            Point Q = new(X, Y, 0, 7, BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663"));
            Q *= Key;

            BigInteger dat = Q.GetX ^ Q.GetY; // Xor координат
            SetKey(dat);
            byte[] bytes = Key.ToByteArray();
            ulong[] ulongArray = new ulong[bytes.Length / 4];

            for (int i = 0; i < ulongArray.Length; i++)
            {
                ulongArray[i] = BitConverter.ToUInt32(bytes, i * 4);
            }
            Crypter = new(ulongArray, 0xff2753253654553f);

        }

        public static BigInteger Encrypt(string text)
        {
            var res = Crypter.Magma_Encrypt(Encoding.Unicode.GetBytes(text.Trim()));

            return new BigInteger(res, true);
        }

        public static string Decrypt(string text)
        {
            BigInteger val = BigInteger.Parse(text.Trim());
            var res = Crypter.Magma_Decrypt(val.ToByteArray(true));

            return Encoding.Unicode.GetString(res);
        }
    }
}
