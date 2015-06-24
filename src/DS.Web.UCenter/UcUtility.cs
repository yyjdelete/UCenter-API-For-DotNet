using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DS.Web.UCenter
{
    /// <summary>
    /// Dozer ��Ȩ����
    /// �����ơ��޸ģ����뱣���ҵ���ϵ��ʽ��
    /// http://www.dozer.cc
    /// mailto:dozer.cc@gmail.com
    /// </summary>
    public static class UcUtility
    {
        private static Encoding Encode
        {
            get { return UcConfig.UcEncoding; }
        }

        #region AuthCode

        /// <summary>
        /// AuthCode
        /// </summary>
        /// <param name="sourceStr">Դ�ַ���</param>
        /// <param name="operation">����</param>
        /// <param name="keyStr">KEY</param>
        /// <param name="expiry">����ʱ��</param>
        /// <returns></returns>
        private static string authCode(string sourceStr, AuthCodeMethod operation, string keyStr, int expiry = 0)
        {
            if (string.IsNullOrEmpty(sourceStr)) return "";
            const int ckeyLength = 4;
            var source = Encode.GetBytes(sourceStr);
            var key = Encode.GetBytes(keyStr);

            key = Md5(key);

            var keya = Md5(SubBytes(key, 0, 0x10));
            var keyb = Md5(SubBytes(key, 0x10, 0x10));
            var keyc = getKeyc(ckeyLength, operation, source);

            var cryptkey = AddBytes(keya, Md5(AddBytes(keya, keyc)));
            var keyLength = cryptkey.Length;

            source = getCKey(expiry, ckeyLength, operation, source, keyb);

            var sourceLength = source.Length;

            var box = getBox();

            var rndkey = getRndkey(cryptkey, keyLength);

            rndBox(box, rndkey);

            var result = getResult(source, box, sourceLength);


            return operation == AuthCodeMethod.Decode
                       ? check(keyb, result)
                       : BytesToString(keyc) + Convert.ToBase64String(result).TrimEnd('=');
        }

        /// <summary>
        /// ������֤
        /// </summary>
        /// <param name="keyb">Keyb</param>
        /// <param name="result">���</param>
        /// <returns></returns>
        private static string check(byte[] keyb, byte[] result)
        {
            var time = long.Parse(BytesToString(result).Substring(0, 10));
            if ((time == 0 ||
                 time - PhpTimeNow() > 0) &&
                BytesToString(SubBytes(result, 10, 16)) == BytesToString(SubBytes(Md5(AddBytes(SubBytes(result, 26), keyb)), 0, 16)))
            {
                return BytesToString(SubBytes(result, 26));
            }
            return "";
        }

        /// <summary>
        /// ������
        /// </summary>
        /// <param name="source">����Դ</param>
        /// <param name="box">Box</param>
        /// <param name="sourceLength">����Դ����</param>
        /// <returns></returns>
        private static byte[] getResult(IList<byte> source, IList<int> box, int sourceLength)
        {
            var result = new byte[sourceLength];
            for (int a = 0, j = 0, i = 0; i < sourceLength; i++)
            {
                a = (a + 1) % 256;
                j = (j + box[a]) % 256;
                var tmp = box[a];
                box[a] = box[j];
                box[j] = tmp;

                result[i] = (byte)(source[i] ^ (box[(box[a] + box[j]) % 256]));
            }
            return result;
        }

        /// <summary>
        /// Box����
        /// </summary>
        /// <param name="box"></param>
        /// <param name="rndkey"></param>
        private static void rndBox(IList<int> box, IList<int> rndkey)
        {
            for (int j = 0, i = 0; i < 256; i++)
            {
                j = (j + box[i] + rndkey[i]) % 256;
                var tmp = box[i];
                box[i] = box[j];
                box[j] = tmp;
            }
        }

        /// <summary>
        /// �õ�RandomKey
        /// </summary>
        /// <param name="cryptkey">cryptkey</param>
        /// <param name="keyLength">keyLength</param>
        /// <returns></returns>
        private static int[] getRndkey(byte[] cryptkey, int keyLength)
        {
            var rndkey = new int[256];
            for (var i = 0; i < 256; i++)
            {
                rndkey[i] = cryptkey[i % keyLength];
            }
            return rndkey;
        }

        /// <summary>
        /// �õ�Box
        /// </summary>
        /// <returns></returns>
        private static int[] getBox()
        {
            var box = new int[256];
            for (var k = 0; k < 256; k++)
            {
                box[k] = k;
            }
            return box;
        }

        /// <summary>
        /// �õ�CKey
        /// </summary>
        /// <param name="expiry">����ʱ��</param>
        /// <param name="ckeyLength">CKey����</param>
        /// <param name="operation">����</param>
        /// <param name="source">����Դ</param>
        /// <param name="keyb">Keyb</param>
        /// <returns></returns>
        private static byte[] getCKey(int expiry, int ckeyLength, AuthCodeMethod operation, byte[] source, byte[] keyb)
        {
            if (operation == AuthCodeMethod.Decode)
            {
                while (source.Length % 4 != 0)
                {
                    source = AddBytes(source, Encode.GetBytes("="));
                }
                source = Convert.FromBase64String(BytesToString(SubBytes(source, ckeyLength)));
            }
            else
            {
                source =
                    AddBytes(
                        (expiry != 0
                             ? Encode.GetBytes((expiry + PhpTimeNow()).ToString())
                             : Encode.GetBytes("0000000000")),
                        SubBytes(Md5(AddBytes(source, keyb)), 0, 0x10), source);
            }
            return source;
        }

        /// <summary>
        /// �õ�keyc
        /// </summary>
        /// <param name="ckeyLength">ckey����</param>
        /// <param name="operation">����</param>
        /// <param name="source">����Դ</param>
        /// <returns></returns>
        private static byte[] getKeyc(int ckeyLength, AuthCodeMethod operation, byte[] source)
        {
            return (ckeyLength > 0)
                       ? ((operation == AuthCodeMethod.Decode)
                              ? SubBytes(source, 0, ckeyLength)
                              : RandomBytes(ckeyLength))
                       : new byte[0];
        }


        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns></returns>
        public static string AuthCodeDecode(string str)
        {
            return authCode(str, AuthCodeMethod.Decode, UcConfig.UcKey);
        }
        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="key">UC KEY</param>
        /// <returns></returns>
        public static string AuthCodeDecode(string str, string key)
        {
            return authCode(str, AuthCodeMethod.Decode, key);
        }
        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="key">UC KEY</param>
        /// <param name="expiry">����ʱ�� 0������������</param>
        /// <returns></returns>
        public static string AuthCodeDecode(string str, string key, int expiry)
        {
            return authCode(str, AuthCodeMethod.Decode, key, expiry);
        }


        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns></returns>
        public static string AuthCodeEncode(string str)
        {
            return authCode(str, AuthCodeMethod.Encode, UcConfig.UcKey);
        }
        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="key">UC KEY</param>
        /// <returns></returns>
        public static string AuthCodeEncode(string str, string key)
        {
            return authCode(str, AuthCodeMethod.Encode, key);
        }
        /// <summary>
        /// UCenter ����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <param name="key">UC KEY</param>
        /// <param name="expiry">����ʱ�� 0������������</param>
        /// <returns></returns>
        public static string AuthCodeEncode(string str, string key, int expiry)
        {
            return authCode(str, AuthCodeMethod.Encode, key, expiry);
        }
        #endregion


        #region ���ú���
        /// <summary>
        /// �õ� UserAgent �ַ���
        /// </summary>
        /// <returns></returns>
        public static string GetUserAgent()
        {
            //TODO: ��UA����������ʱ��ʹ��UA
            return ((HttpContext.Current != null) ? HttpContext.Current.Request.ServerVariables["Http_User_Agent"] : null) ?? "Mozilla/4.0(compatible;MSIE6.0;)";
        }

        /// <summary>
        /// Byte����ת�ַ���
        /// </summary>
        /// <param name="b">����</param>
        /// <returns></returns>
        public static string BytesToString(byte[] b)
        {
            return Encode.GetString(b);
        }


        private static string[] _hexMap;
        private static string[] GetHexMap()
        {
            if (_hexMap == null)
            {
                var tmp = new string[256];

                for (int a = 0; a <= 0xff; ++a)
                {
                    if (a < 16)
                    { tmp[a] = "0" + a.ToString("x"); }
                    else
                    { tmp[a] = a.ToString("x"); }
                }
                _hexMap = tmp;
            }
            return _hexMap;
        }

        private static string Md5Raw(byte[] b)
        {
            byte[] hash;
            using (var cryptHandler = MD5.Create())
            {
                hash = cryptHandler.ComputeHash(b);
            }
            StringBuilder sb = new StringBuilder(hash.Length * 2);
            var hexMap = GetHexMap();
            foreach (var a in hash)
            {
                sb.Append(hexMap[a]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// ����Md5
        /// </summary>
        /// <param name="b">byte����</param>
        /// <returns>����õ��ַ���(Сд)���ֽ�����</returns>
        public static byte[] Md5(byte[] b)
        {
            return Encode.GetBytes(Md5Raw(b));
        }

        /// <summary>
        /// ����Md5
        /// </summary>
        /// <param name="str">byte����</param>
        /// <returns>����õ��ַ���(Сд)</returns>
        public static string Md5(string str)
        {
            return Md5Raw(Encode.GetBytes(str));
        }

        /// <summary>
        /// Byte�������
        /// </summary>
        /// <param name="bytes">����</param>
        /// <returns></returns>
        public static byte[] AddBytes(params byte[][] bytes)
        {
            var index = 0;
            var length = bytes.Sum(b => b.Length);
            var result = new byte[length];

            foreach (var bs in bytes)
            {
                //Array.Copy(bs, 0, result, index, bs.Length);
                Buffer.BlockCopy(bs, 0, result, index * sizeof(byte), bs.Length * sizeof(byte));
                index += bs.Length;
            }
            return result;
        }

        /// <summary>
        /// Byte����ָ�
        /// </summary>
        /// <param name="b">����</param>
        /// <param name="start">��ʼ</param>
        /// <param name="length">����</param>
        /// <returns></returns>
        public static byte[] SubBytes(byte[] b, int start, int length = int.MaxValue)
        {
            if (start >= b.Length) return new byte[0];
            if (start < 0) start = 0;
            if (length < 0) length = 0;
            if (length > b.Length || start + length > b.Length) length = b.Length - start;
            var result = new byte[length];
            //Array.Copy(b, start, result, 0, length);
            Buffer.BlockCopy(b, start * sizeof(byte), result, 0, length * sizeof(byte));
            return result;
        }


        /// <summary>
        /// ����Php��ʽ�ĵ�ǰʱ��
        /// </summary>
        /// <returns>Php��ʽ��ʱ��</returns>
        public static long PhpTimeNow()
        {
            return DateTimeToPhpTime(DateTime.UtcNow);
        }

        ////new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks
        private const long UNIX_EPOCH = 621355968000000000;
        private const int e7 = 10000000;
        /// <summary>
        /// PhpTimeתDataTime
        /// </summary>
        /// <returns></returns>
        public static DateTime PhpTimeToDateTime(long time)
        {
            var t = time * e7 + UNIX_EPOCH;
            return new DateTime(t, DateTimeKind.Utc).ToLocalTime();
        }

        /// <summary>
        /// DataTimeתPhpTime
        /// </summary>
        /// <param name="datetime">ʱ��</param>
        /// <returns></returns>
        public static long DateTimeToPhpTime(DateTime datetime)
        {
            return (datetime.Ticks - UNIX_EPOCH) / e7;
        }

        private static readonly Random random = new Random();

        /// <summary>
        /// ����ֽ�����
        /// </summary>
        /// <param name="lens">����</param>
        /// <returns></returns>
        public static byte[] RandomBytes(int lens)
        {
            var chArray = new[]
                        {
                            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q',
                            'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G',
                            'H', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
                            'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'
                        };
            var length = chArray.Length;
            var result = new byte[lens];
            for (var i = 0; i < lens; i++)
            {
                result[i] = (byte)chArray[random.Next(length)];
            }
            return result;
        }


        /// <summary>
        /// PhpUrl����
        /// </summary>
        /// <param name="str">�ַ���</param>
        /// <returns></returns>
        public static string PhpUrlEncode(string str)
        {
            StringBuilder sb = new StringBuilder(str.Length);
            const string keys = "_-.1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (var i = 0; i < str.Length; i++)
            {
                var str4 = str.Substring(i, 1);
                if (keys.Contains(str4))
                {
                    sb.Append(str4);
                }
                else
                {
                    foreach (var n in Encode.GetBytes(str4))
                    {
                        sb.Append('%').Append(n.ToString("X"));
                    }
                }
            }
            return sb.ToString();
        }

        #endregion
    }


    /// <summary>
    /// ��������
    /// </summary>
    enum AuthCodeMethod
    {
        Encode,
        Decode,
    }
}