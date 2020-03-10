using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace Maikelvdb.Core.Transip.Api.Helpers
{
    public static class EncryptionHelper
    {
        private static readonly Regex PrivateKeyRegex =
            new Regex(@"-----BEGIN (RSA )?PRIVATE KEY-----(.*)-----END (RSA )?PRIVATE KEY-----", RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Compiled);

        public static string Sign(string privateKey, string body)
        {
            var matches = PrivateKeyRegex.Matches(privateKey);
            if (matches.Count == 0)
            {
                throw new Exception("Invalid private key");
            }

            var digest = Sha512Asn1(body);
            var signature = Encrypt(digest, privateKey);

            return Convert.ToBase64String(signature);
        }

        private static byte[] Sha512Asn1(string data)
        {
            var signature = new byte[] { 0x30, 0x51, 0x30, 0x0d, 0x06, 0x09, 0x60, 0x86, 0x48, 0x01, 0x65, 0x03, 0x04, 0x02, 0x03, 0x05, 0x00, 0x04, 0x40 };
            var hashAlg = SHA512.Create();
            var hash = hashAlg.ComputeHash(Encoding.ASCII.GetBytes(data));

            return signature.Select(x => x).Concat(hash).ToArray();
        }

        private static byte[] Encrypt(byte[] digest, string key)
        {
            var keyReader = new StringReader(key);
            var pemReader = new PemReader(keyReader);

            var pemObject = pemReader.ReadObject();
            ICipherParameters cipherParameters;

            switch (pemObject)
            {
                case RsaPrivateCrtKeyParameters parameters:
                    cipherParameters = parameters;
                    break;
                case AsymmetricCipherKeyPair _:
                    var keyPair = (AsymmetricCipherKeyPair)pemObject;
                    cipherParameters = keyPair.Private;
                    break;
                default:
                    throw new Exception($"Unsupported private key format. [{pemObject.GetType()}]");
            }

            var cipher = CipherUtilities.GetCipher("RSA/None/PKCS1Padding");
            cipher.Init(true, cipherParameters);

            return cipher.DoFinal(digest);
        }
    }
}
