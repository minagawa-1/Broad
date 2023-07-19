using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;

namespace woskni
{
    public class AESEncrypt
    {
        private const int m_key_size    = 256;                  // 256Bit
        private const int m_block_size  = 128;                  // 128Bit
        private const int m_buffer_size = m_block_size * 32;    // バッファーサイズはBlockSizeの倍数
        private const int m_salt_size   = 8;                    // 8以上

        //idとpass
        private static string m_UserID = "Akatsuki";

        /// <summary>Saltを生成する</summary>
        /// <param name="str">8文字以上の文字列を指定すること</param>
        private static byte[] CreateSalt(string str)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(str, m_salt_size);
            return deriveBytes.Salt;
        }

        /// <summary>初期化ベクトルを作成する</summary>
        /// <param name="str">8文字以上の文字列を指定すること</param>
        private static byte[] CreateIV(string str)
        {
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(str, m_block_size / 8);
            return deriveBytes.GetBytes(m_block_size / 8);
        }

        //パスワードとソルトから暗号鍵を求める
        private static byte[] GetKeyFromPassword(string textUserId, byte[] salt)
        {
            //Rfc2898DeriveBytesオブジェクトを作成する
            Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(textUserId, salt);

            //反復処理回数を指定する デフォルトで1000回
            deriveBytes.IterationCount = 1000;

            //キーを生成する
            return deriveBytes.GetBytes(m_key_size / 8);
        }

        /// <summary>暗号化</summary>
        /// <param name="plainStr">平文</param>
        /// <returns>暗号化された文</returns>
        public static string Encrypt(string plainStr)
        {
            byte[] salt = CreateSalt("");
            byte[] iv   = CreateIV("");
            byte[] key  = GetKeyFromPassword(m_UserID, salt);

            byte[] outBytes = null;
            AesManaged aes = new AesManaged();
            {
                // AESインスタンスのパラメータ設定
                aes.KeySize   = m_key_size;
                aes.BlockSize = m_block_size;
                aes.Mode      = CipherMode.CBC;
                aes.Key       = key;
                aes.IV        = iv;
                aes.Padding   = PaddingMode.PKCS7;

                // 暗号化オブジェクト生成
                ICryptoTransform ct = aes.CreateEncryptor(aes.Key, aes.IV);

                // 出力ストリーム
                MemoryStream outStream = new MemoryStream();
                {
                    using (CryptoStream cryptoStream = new CryptoStream(outStream, ct, CryptoStreamMode.Write))
                    {
                        byte[] buf = System.Text.Encoding.Unicode.GetBytes(plainStr);
                        cryptoStream.Write(buf, 0, buf.Length);
                    }
                    outBytes = outStream.ToArray();
                }
            }

            List<byte> vs = new List<byte>();
            vs.AddRange(salt);
            vs.AddRange(iv);
            vs.AddRange(outBytes);

            return System.Convert.ToBase64String(vs.ToArray());
        }

        /// <summary>復号化</summary>
        /// <param name="encryptedStr">暗号化された文字列</param>
        /// <returns>復号化された平文</returns>
        public static string Decrypt(string encryptedStr)
        {
            byte[] bytes = System.Convert.FromBase64String(encryptedStr);

            byte[] salt  = new byte[m_salt_size];
            byte[] iv    = new byte[m_block_size / 8];
            byte[] data  = new byte[bytes.Length - m_salt_size - m_block_size / 8];

            // ソルトとIVと暗号データを読み取る
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                ms.Read(salt, 0, m_salt_size);
                ms.Read(iv  , 0, m_block_size / 8);
                ms.Read(data, 0, bytes.Length - m_salt_size - m_block_size / 8);
            }

            string saltValue = System.Convert.ToBase64String(salt);
            string ivValue   = System.Convert.ToBase64String(iv);

            byte[] key       = GetKeyFromPassword(m_UserID, salt);
            string keyValue  = System.Convert.ToBase64String(key);

            string decodeString = "";

            using (AesManaged aes = new AesManaged())
            {
                // AESインスタンスのパラメータ設定
                aes.KeySize   = m_key_size;
                aes.BlockSize = m_block_size;
                aes.Mode      = CipherMode.CBC;
                aes.Key       = key;
                aes.IV        = iv;
                aes.Padding   = PaddingMode.PKCS7;

                // 暗号化オブジェクト生成
                ICryptoTransform ct = aes.CreateDecryptor(aes.Key, aes.IV);

                using (var inStream  = new MemoryStream(data, false))  // 入力ストリームを開く
                using (var outStream = new MemoryStream())   // 出力ストリーム
                {
                    using (CryptoStream cryptoStream = new CryptoStream(inStream, ct, CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[m_buffer_size];
                        int len = 0;
                        while ((len = cryptoStream.Read(buffer, 0, m_buffer_size)) > 0)
                            outStream.Write(buffer, 0, len);
                    }
                    byte[] outBytes = outStream.ToArray();
                    decodeString = System.Text.Encoding.Unicode.GetString(outBytes);
                }
            }
            return decodeString;
        }
    }
}