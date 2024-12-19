using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Elliptic_curve
{
    abstract class Elliptic_curve(BigInteger a, BigInteger b, BigInteger p)
    {
        protected readonly BigInteger a = a;
        protected readonly BigInteger b = b;
        protected readonly BigInteger p = p;

        public BigInteger GetA => a;
        public BigInteger GetB => b;
        public BigInteger GetP => p;

        public override bool Equals(object? obj) // Перегрузка
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode() // Перегрузка
        {
            throw new NotImplementedException();
        }
    }

    class Point : Elliptic_curve
    {
        private readonly BigInteger y;
        private readonly BigInteger x;
        public BigInteger GetY => y;
        public BigInteger GetX => x;

        public Point(BigInteger x, BigInteger y, BigInteger a, BigInteger b, BigInteger p) : base(a, b, p)
        {
            if (Check(y, x))
            {
                this.y = y;
                this.x = x;
            }
            else throw new ArgumentException("На кривой нет такой точки");
        }

        public Point(Point a) : base(a.a, a.b, a.p)
        {
            y = a.y;
            x = a.x;
        }

        private bool Check(BigInteger y, BigInteger x) // Проверка принадлежит ли такая точка кривой
        {
            if ((x == long.MinValue) && (y == long.MinValue)) // Если точка O
            {
                return true;
            }

            BigInteger Y = (y * y) % p;
            if (Y < 0) Y = p + Y; // Если Y отрицательна

            BigInteger X = ((x * x * x) + (a * x) + b) % p;
            if (X < 0) X = p + X; // Если X отрицательна

            if (Y == X)
            {
                return true;
            }
            return false;
        }

        public static bool operator ==(Point a, Point b) // Перегрузка оператора равенства
        {
            if ((a.a != b.a) || (a.b != b.b) || (a.p != b.p))
                throw new ArgumentException("Разные эллиптические кривые");

            if ((a.y == b.y) && (a.x == b.x))
            {
                return true;
            }
            return false;
        }

        public static bool operator !=(Point a, Point b) // Перегрузка оператора неравенства
        {
            if ((a.a != b.a) || (a.b != b.b) || (a.p != b.p))
                throw new ArgumentException("Разные эллиптические кривые");

            if ((a.y == b.y) && (a.x == b.x))
            {
                return false;
            }
            return true;
        }

        public static Point operator +(Point a, Point b) // Перегрузка оператора сложения
        {
            BigInteger l;

            if ((a.x == long.MinValue) && (a.y == long.MinValue) && (b.x != long.MinValue) && (b.y != long.MinValue)) return b; // Если точка a это O
            if ((a.x != long.MinValue) && (a.y != long.MinValue) && (b.x == long.MinValue) && (b.y == long.MinValue)) return a; // Если точка b это O
            if ((a.x == long.MinValue) && (a.y == long.MinValue) && (b.x == long.MinValue) && (b.y == long.MinValue)) return b; // Если и точка a и точка b являются O

            if (a == b) // Если точки равны
            {
                if (a.y == 0) return new Point(long.MinValue, long.MinValue, a.a, a.b, a.p); // Если получается точка O

                l = ((3 * a.x * a.x) + a.a) * Invmod(2 * a.y, a.p) % a.p;
                if (l < 0) l = a.p + l; // l отрицательная
            }
            else
            {
                if ((b.x - a.x) % a.p == 0) return new Point(long.MinValue, long.MinValue, a.a, a.b, a.p); // Если получается точка O

                BigInteger temp = (b.x - a.x) % a.p;
                if (temp < 0) temp = a.p + temp;

                l = (((b.y - a.y) % a.p) * Invmod(temp, a.p)) % a.p;
                if (l < 0) l = a.p + l; // l отрицательная
            }

            BigInteger x = (l * l - a.x - b.x) % a.p;
            if (x < 0) x = a.p + x; // x отрицательный

            BigInteger y = (l * (a.x - x) - a.y) % a.p;
            if (y < 0) y = a.p + y; // y отрицательный

            return new Point(x, y, a.a, a.b, a.p);
        }

        public static Point operator *(Point a, BigInteger n)
        {
            Point result = new(a);
            n--;
            while (n > 0)
            {
                if ((n & 1) != 0)
                {
                    result += a;
                }
                a += a; // Удвоить точку
                n >>= 1;
            }
            return result;
        }

        public static Point operator *(BigInteger n, Point a)
        {
            Point result = new(a);
            n--;
            while (n > 0)
            {
                if ((n & 1) != 0)
                {
                    result += a;
                }
                a += a; // Удвоить точку
                n >>= 1;
            }
            return result;
        }

        private static (BigInteger, BigInteger, BigInteger) Gcdex(BigInteger a, BigInteger b) // Вспомогательная рекурсивная функция для вычисления обратного по модулю числа
        {
            if (a == 0)
                return (b, 0, 1);
            (BigInteger gcd, BigInteger x, BigInteger y) = Gcdex(b % a, a);
            return (gcd, y - (b / a) * x, x);
        }
        private static BigInteger Invmod(BigInteger a, BigInteger m) // Функция определения обратного по модулю числа
        {
            (BigInteger g, BigInteger x, _) = Gcdex(a, m);
            return g > 1 ? 0 : (x % m + m) % m;
        }

        public override string ToString() // Перегрузка ToString
        {
            if (x == long.MinValue && y == long.MinValue) return $"Point is \'O\'";
            return $"{x} {y}";
        }

        public override bool Equals(object? obj) // Перегрузка
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj is null)
            {
                return false;
            }

            throw new NotImplementedException();
        }

        public override int GetHashCode() // Перегрузка
        {
            throw new NotImplementedException();
        }
    }
}