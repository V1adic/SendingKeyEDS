using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
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

        private static BigInteger Generate()
        {
            BigInteger Key;
            BigInteger d;
            do
            {
                byte[] buffer = new byte[32];
                rng.GetBytes(buffer);
                d = new BigInteger(buffer, true);

            } while (d > n);

            Point Q = P * d;
            Console.WriteLine(Q);


            BigInteger X;
            BigInteger Y;

            while (true)
            {
                string? text = Console.ReadLine();
                if (text != null)
                {
                    var param = text.Split(' ');
                    if (param.Length == 2)
                    {
                        X = BigInteger.Parse(param[0].Trim());
                        Y = BigInteger.Parse(param[1].Trim());
                        break;
                    }
                }
            }

            Point Q1 = new(X, Y, 0, 7, BigInteger.Parse("115792089237316195423570985008687907853269984665640564039457584007908834671663"));
            Q1 *= d;

            Key = Q1.GetX ^ Q1.GetY; // Xor координат
            return Key;
        }

        private static string Help()
        {
            string result = "\n\n\nPress 1 to Encrypt message\nPress 2 to Decrypt message\n\n\n";

            return result;
        }

        public static void Worker()
        {
            BigInteger Key = Generate();

            byte[] bytes = Key.ToByteArray();
            ulong[] ulongArray = new ulong[bytes.Length / 4];

            for (int i = 0; i < ulongArray.Length; i++)
            {
                ulongArray[i] = BitConverter.ToUInt32(bytes, i * 4);
            }

            var buffer = new byte[8];
            rng.GetBytes(buffer);

            Magma Cryper = new(ulongArray, BitConverter.ToUInt64(buffer, 0));

            Console.WriteLine(Help());

            while (true)
            {
                var data = Console.ReadLine();

                switch (data)
                {
                    case "1":
                        {
                            data = Console.ReadLine();
                            if (data != null)
                            {
                                var res = Cryper.Magma_Encrypt(Encoding.Unicode.GetBytes(data.Trim()));

                                Console.WriteLine(new BigInteger(res, true));

                                Console.WriteLine(Help());
                            }
                        }
                        break;

                    case "2":
                        {
                            data = Console.ReadLine();
                            if (data != null)
                            {
                                BigInteger val = BigInteger.Parse(data.Trim());
                                var res = Cryper.Magma_Decrypt(val.ToByteArray(true));

                                Console.WriteLine(Encoding.Unicode.GetString(res));
                                Console.WriteLine(Help());
                            }
                        }
                        break;

                    default:
                        {
                            Console.WriteLine(Help());
                        }
                        break;
                }
            }
        }
    }
}
