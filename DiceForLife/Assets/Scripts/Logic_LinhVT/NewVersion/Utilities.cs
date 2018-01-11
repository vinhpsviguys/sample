using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CoreLib
{
    static class Utilities// luu y litte end
    {
        public static byte[] convertToByteArr(object v)
        {
            if (v is byte) {
                Console.WriteLine("convert byte");
                return new byte[] {(byte)v};
            } else if (v is sbyte) {
                Console.WriteLine("convert sbyte");
                byte b = 0;
				unchecked
				{
					sbyte s;
					s = (sbyte)v;
					b = (byte)s;
				}


                return new byte[] { b };
            } else if (v is int)
            {
                Console.WriteLine("convert int");
                return BitConverter.GetBytes((int)v);
            }
            else if (v is uint)
            {
                Console.WriteLine("convert uint");
                return BitConverter.GetBytes((uint)v);
            }
            else if (v is short)
            {
                Console.WriteLine("convert short");
                return BitConverter.GetBytes((short)v);
            }
            else if (v is ushort)
            {
                Console.WriteLine("convert ushort");
                return BitConverter.GetBytes((ushort)v);
            }
            else if (v is long)
            {
                return BitConverter.GetBytes((long)v);
            }
            else if (v is float)
            {
                return BitConverter.GetBytes((float)v);
            }
            else if (v is double)
            {
                return BitConverter.GetBytes((double)v);
            }
            else if (v is char)
            {
                return BitConverter.GetBytes((char)v);
            }
            else if (v is bool)
            {
                return BitConverter.GetBytes((bool)v);
            }
            else if (v is string)
            {
                throw new Exception("Can't convert string to byte[]");
            }
            else if (v is decimal)
            {
                throw new Exception("Can't convert decimal to byte[]");
            }
            return null;
        }

        public static T[] SubArray<T>(this T[] data, int index, int length)
        {
            T[] result = new T[length];
            Array.Copy(data, index, result, 0, length);
            return result;
        }

        public static T[] Concat<T>(this T[] x, T[] y)
        {
            if (x == null) throw new ArgumentNullException("x");
            if (y == null) throw new ArgumentNullException("y");
            int oldLen = x.Length;
            Array.Resize<T>(ref x, x.Length + y.Length);
            Array.Copy(y, 0, x, oldLen, y.Length);
            return x;
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static string convertByteArrToString(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] convertStringToByteArr(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static float convertStringToFloat(string str) {
            try {
                return Convert.ToSingle(str);
            } catch (Exception e) {
                return 1.0f * Convert.ToInt32(str);
            }
        }
    }


}
