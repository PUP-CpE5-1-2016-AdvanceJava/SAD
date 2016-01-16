using System;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.IO;

namespace Attendance_GUI
{
    public partial class Configuration : Form
    {
        public Configuration()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (tbbIP.Text != "" && tbPort.Text != "" && tbUsername.Text != "" && tbPassword.Text != "" && tbDatabase.Text != "")
            {
                string[] lines = { encrypt(tbbIP.Text), encrypt(tbPort.Text), encrypt(tbUsername.Text), encrypt(tbPassword.Text), encrypt(tbDatabase.Text)};
                File.WriteAllLines(@"Files/config.txt", lines);
                MessageBox.Show("Settings successfully saved!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                //getConfig();
            }
            else
            {
                MessageBox.Show("Incomplete settings configuration.");
            }
        }

        //public void getconfig()
        //{
        //    dc.ip = tbbip.text;
        //    dc.port = tbport.text;
        //    dc.username = tbusername.text;
        //    dc.password = tbpassword.text;
        //    dc.database = tbdatabase.text;
        //}

        public void readConfig()
        {
            string[] text = File.ReadAllLines(@"Files/config.txt");
            tbbIP.Text = decrypt(text[0]);
            tbPort.Text = decrypt(text[1]);
            tbUsername.Text = decrypt(text[2]);
            tbPassword.Text = decrypt(text[3]);
            tbDatabase.Text = decrypt(text[4]);
        }

        public static string encrypt(string item)
        {
            string password = "05/23/1995";
            string encrypt = StringCipher.Encrypt(item, password);
            return encrypt;
        }

        public static string decrypt(string item)
        {
            string password = "05/23/1995";
            string decrypt = StringCipher.Decrypt(item, password);
            return decrypt;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                tbPassword.PasswordChar = '\0';
            }
            else
            {
                tbPassword.PasswordChar = '*';
            }
        }

        private void Configuration_Load(object sender, EventArgs e)
        {
            readConfig();
            //getConfig();
        }
    }

    public static class StringCipher
    {
        private const int Keysize = 256;

        private const int DerivationIterations = 1000;

        public static string Encrypt(string plainText, string passPhrase)
        {

            var saltStringBytes = Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();
                                // Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public static string Decrypt(string cipherText, string passPhrase)
        {
            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / 8).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / 8).Take(Keysize / 8).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / 8) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / 8) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = 256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream(cipherTextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                var plainTextBytes = new byte[cipherTextBytes.Length];
                                var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                            }
                        }
                    }
                }
            }
        }

        private static byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32]; // 32 Bytes will give us 256 bits.
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }
    }
}
