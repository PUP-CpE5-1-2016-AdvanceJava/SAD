using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using MySql.Data.MySqlClient;

namespace Student_Attendance_Final
{
    public partial class BandiClock : Form
    {
        string vidPath = "";
        string ipAddress = "";
        string connectionString = "";
        string savedString = "";
        AttendanceControl ac = new AttendanceControl();
        
        public BandiClock()
        {
            InitializeComponent();
            this.Location = Screen.AllScreens[0].WorkingArea.Location;
        }

        private void BandiClock_Load(object sender, EventArgs e)
        {

            _Play();
            int width, height;
            ac.setConnection(connectionString);
            //width = Screen.PrimaryScreen.Bounds.Width;
            //height = Screen.PrimaryScreen.Bounds.Height;
            _Display();
            //groupBox1.Width = width / 3;
            //groupBox1.Height = height;
            //groupBox1.Location = new Point(width - (width / 3), 0);
            //groupBox2.Parent = groupBox1;
            //groupBox2.Location = new Point(0, groupBox1.Height / 2);
            //groupBox2.Width = groupBox1.Width;
            //groupBox2.Height = groupBox1.Height / 2;
            //label1.Parent = groupBox1;
            //label1.Font = new Font(label1.Font.FontFamily, width / 25);
            //label2.Parent = groupBox1;
            //label2.Font = new Font(label2.Font.FontFamily, width / 70);
            //label3.Parent = groupBox2;
            //label3.Font = new Font(label3.Font.FontFamily, width / 50);
            setVideoPath();
            studName.Text = "--NAME--";

            _Update();
            _Relocate();
        }

        public void setConnection(string conf)
        {
            connectionString = conf;
        }
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            //if (DateTime.Now.Second == 0)
            //{
            //    _Relocate();
            //}
            if (axWindowsMediaPlayer1.playState == WMPLib.WMPPlayState.wmppsStopped)
            {
                _Play();
            }
            _Display();
        }

        private void _Display()
        {
            DateTime now = DateTime.Now;
            timeLabel.Text = now.ToString("hh:mm tt");
            dateLabel.Text = now.ToString("dddd, MMMM d, yyyy");
        }

        private void _Relocate()
        {
            //label1.Location = new Point((groupBox1.Width / 2) - label1.Width / 2, (groupBox1.Height / 3) - (label1.Height + label2.Height));
            //label2.Location = new Point((groupBox1.Width / 2) - label2.Width / 2, (groupBox1.Height / 3) - (label2.Height));
        }

        public void _Play()
        {
            axWindowsMediaPlayer1.uiMode = "none";
           // axWindowsMediaPlayer1.Width = Screen.PrimaryScreen.Bounds.Width - (Screen.PrimaryScreen.Bounds.Width / 3);
            //axWindowsMediaPlayer1.Height = Screen.PrimaryScreen.Bounds.Height;    
            axWindowsMediaPlayer1.URL = vidPath;
            axWindowsMediaPlayer1.stretchToFit = true;
        }
        public void setLabel(string name)
        {
            studName.Text = name;
            //label3.Location = new Point((groupBox2.Width / 2) - (label3.Width / 2), (groupBox2.Height / 2) - label3.Height);

        }
        public void _Update()
        {
            studName.Text = "Student Name";
            //label3.Location = new Point((groupBox2.Width / 2) - (label3.Width / 2), (groupBox2.Height / 2) - label3.Height);
        }

        public void setVideoPath()
        {

            vidPath = getVidPath().Replace("%","'");
            _Play();
        }

        public string getVidPath()
        {
            string query = "SELECT videoPathBox FROM settings";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, connection);

            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    return reader["videoPathBox"].ToString();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            return "";
        }

        private void scanCode()
        {
            string nameGradeSection = ac.studentIn(savedString);
            studName.Text = nameGradeSection.Split('%')[0];
            studGradeSection.Text = nameGradeSection.Split('%')[1];
            savedString = "";
        }

        private void BandiClock_KeyDown(object sender, KeyEventArgs e)
        {
            string newChar = (e.KeyValue - 48).ToString();
            if (e.KeyCode == Keys.Enter)
            {
                scanCode();
                return;
            }
            
            savedString += newChar;
        }

    }
}
