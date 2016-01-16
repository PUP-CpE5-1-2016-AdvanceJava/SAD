using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.IO;

namespace Student_Attendance_Final
{
    public partial class Login : Form
    {
        string connectionString;
        AttendanceControl adminConsole = new AttendanceControl();
        BandiClock bandi = new BandiClock();

        public void setUpConnection(string conf)
        {
            string[] arr = conf.Split(',');
            connectionString = "SERVER = " + arr[0] + "; PORT = " + arr[1] + "; DATABASE = " + arr[4] + "; UID = " + arr[2] + "; password = " + arr[3] + ";";
        }

        public string getConfig()
        {
            string[] text = File.ReadAllLines(@"Files/config.txt");
            string ip_ = decrypt(text[0]);
            string Port_ = decrypt(text[1]);
            string Username_ = decrypt(text[2]);
            string Password_ = decrypt(text[3]);
            string Database_ = decrypt(text[4]);
            return ip_ + "," + Port_ + "," + Username_ + "," + Password_ + "," + Database_;
        }

        public Login()
        {
            InitializeComponent();
        }


        private void loginBtn_Click(object sender, EventArgs e)
        {

            string authority = isAdmin(usernameBox.Text, passwordBox.Text);
            if (authority != "")
            {
                //MessageBox.Show("Admin");
                this.Hide();
                adminConsole.setConnection(connectionString);
                adminConsole.setUserAuthority(authority);
                bandi.setConnection(connectionString);

                if (authority == "Monitor")
                {
                    bandi.Show();
                }
                else
                {
                    adminConsole.Show();
                }
                //MessageBox.Show("Login Success!");
            }

            else
            {
                MessageBox.Show("Username or Password is incorrect");
                passwordBox.Clear();
            }
        }

        private string isAdmin(string name, string pass)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            string query = "SELECT * FROM Users";

            MD5 md5Hash = MD5.Create();
            try
            {
                connection.Open();

                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //MessageBox.Show(reader["Username"].ToString());
                    if (reader["Username"].ToString() == name && VerifyMd5Hash(md5Hash, pass, reader["Userpass"].ToString()))
                    //if (reader["Username"].ToString() == name && reader["Userpass"].ToString() == pass)
                    {
                        return reader["Usertype"].ToString();
                    }
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            return "";
        }

        private string GetMd5Hash(MD5 md5Hash, string input)
        {

            // Convert the input string to a byte array and compute the hash.
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            // Create a new Stringbuilder to collect the bytes
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data 
            // and format each one as a hexadecimal string.
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // Return the hexadecimal string.
            return sBuilder.ToString();
        }

        // Verify a hash against a string.
        private bool VerifyMd5Hash(MD5 md5Hash, string input, string hash)
        {
            // Hash the input.
            string hashOfInput = GetMd5Hash(md5Hash, input);

            // Create a StringComparer an compare the hashes.
            StringComparer comparer = StringComparer.OrdinalIgnoreCase;

            if (0 == comparer.Compare(hashOfInput, hash))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string decrypt(string item)
        {
            string password = "05/23/1995";
            string decrypt = StringCipher.Decrypt(item, password);
            return decrypt;
        }

        public static class StringCipher
        {
            private const int Keysize = 256;
            private const int DerivationIterations = 1000;

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

        private void Login_Shown(object sender, EventArgs e)
        {
            setUpConnection(getConfig());
        }
    }
}
