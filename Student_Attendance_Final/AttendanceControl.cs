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
using System.IO;
using System.Net;
using System.Net.Mail;
using Excel = Microsoft.Office.Interop.Excel;
using MySql.Data.MySqlClient;

//using ExcelLibrary.SpreadSheet;
//using ExcelLibrary.CompoundDocumentFormat;

namespace Student_Attendance_Final
{
    public partial class AttendanceControl : Form
    {
        //initialization for email sending
        NetworkCredential mailLogin;
        SmtpClient client;
        MailMessage msg;
        bool autoMailEnable = false;
        //current date
        DateTime now = DateTime.Now;
        string currDate = getCurrentDate();
        //BandiClock bandi = new BandiClock();
        //variable of filepath for exporting
        string latestfile = "";
        //database mysql connection string
        string connectionString;
        //authority type
        string authority = "";
        string ipAddress;
        //security
        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public AttendanceControl()
        {
            InitializeComponent();
        }

        private void AttendanceControl_Load(object sender, EventArgs e)
        {

            DateLabel.Text = currDate;
            timer1.Enabled = true;
            timer1.Interval = 1000;
            showStudents(0, "");
            showUsers();
            showAttendance();
            tbStudCode.Select();
            setSectionFilter(StudFiltSection);
            setSectionFilter(studFiltSectionReview);
            getSettings();
            //bandiclock
            //bandi.setLabel("--NAME--");
            Screen[] screens = Screen.AllScreens;
            //bandi.setFormLocation(bandi, screens[1]); ===>extend screen
            //play video
            //bandi.setVideoPath(videoPathBox.Text);
            //bandi._Play();
        }

        public void setSectionFilter(ComboBox section)
        {
            string query = "SELECT Section FROM `students` ORDER BY Section";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //MessageBox.Show(reader["Section"].ToString());
                    if (!section.Items.Contains((reader["Section"].ToString())))
                    {
                        section.Items.Add(reader["Section"].ToString());
                    }
                    
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }

        }

        public string getIP()
        {
            return ipAddress;
        }

        public void setConnection(string conf)
        {
            connectionString = conf;
        }

        public void setUserAuthority(string type)
        {
            authority = type;
        }

        public string getUserAuthority()
        {
            return authority;
        }

        private void StudRegBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tbStudId.Text) || string.IsNullOrEmpty(tbStudFname.Text) || string.IsNullOrEmpty(tbStudMname.Text) || string.IsNullOrEmpty(tbStudSname.Text) || string.IsNullOrEmpty(tbStudGrade.Text) || string.IsNullOrEmpty(tbStudSection.Text))
            {
                MessageBox.Show("Please fill in all student details.");
                return;
            }
            long StudId = Convert.ToInt64(tbStudId.Text);
            if (registerStudents(StudId))
            {
                tbStudId.Clear();
                tbStudFname.Clear();
                tbStudMname.Clear();
                tbStudSname.Clear();
                tbStudGrade.Clear();
                tbStudSection.Clear();
            }
        }

        private bool registerStudents(long StudId)
        {
            int validator = 1;

            if (!(int.TryParse(tbStudGrade.Text, out validator)))
            {
                MessageBox.Show("Invalid input for grade. Please input an integer.");
                return false;
            }
            string StudFname = tbStudFname.Text;
            string StudMname = tbStudMname.Text;
            string StudSname = tbStudSname.Text;
            int StudGrade = Convert.ToInt32(tbStudGrade.Text);
            string StudSection = tbStudSection.Text;


            //if the id is already used by another student
            if (isCodeExist(StudId))
            {
                MessageBox.Show("Student is already registered. If you want to edit just click the name on the table.");
            }
            else
            {
                string cmd = "INSERT INTO Students(Id,Fname,Mname,Sname,Grade,Section) VALUES (@val1,@val2,@val3,@val4,@val5,@val6)";
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand(cmd, connection);
                command.Parameters.AddWithValue("@val1", StudId);
                command.Parameters.AddWithValue("@val2", StudFname);
                command.Parameters.AddWithValue("@val3", StudMname);
                command.Parameters.AddWithValue("@val4", StudSname);
                command.Parameters.AddWithValue("@val5", StudGrade);
                command.Parameters.AddWithValue("@val6", StudSection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Register Success");
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                    throw;
                }
            }
            showStudents(0, "");
            return true;
        }

        //private bool editStudents(long StudId)
        //{
        //    int validator = 1;
        //    if (!(int.TryParse(tbStudGradeForEdit.Text, out validator) && int.TryParse(tbStudSectionForEdit.Text, out validator)))
        //    {
        //        MessageBox.Show("Invalid input for grade or section. Please check your inputs.");
        //        return false;
        //    }
        //    string StudName = tbStudNameForEdit.Text;
        //    int StudGrade = Convert.ToInt32(tbStudGradeForEdit.Text);
        //    int StudSection = Convert.ToInt32(tbStudSectionForEdit.Text);

        //    string query = "SELECT * FROM Students WHERE Id = "+StudId;
        //    MySqlConnection connection = new MySqlConnection(connectionString);
        //    MySqlCommand cmdcheck = new MySqlCommand(query, connection);
        //    try
        //    {
        //        connection.Open();
        //        MySqlDataReader reader = null;
        //        reader = cmdcheck.ExecuteReader();

        //        if (reader.HasRows)
        //        {
        //            connection.Close();
        //            string cmd = "UPDATE Students SET Name = '" + StudName + "', Grade = " + StudGrade + ", Section = " + StudSection + " WHERE Id = " + StudId;
        //            MySqlCommand command = new MySqlCommand(cmd, connection);
        //            try
        //            {
        //                connection.Open();
        //                command.ExecuteNonQuery();
        //                MessageBox.Show("Edit Success");
        //            }
        //            catch (Exception er)
        //            {
        //                MessageBox.Show(er.ToString());
        //                throw;
        //            }
        //        }
        //        else
        //        {
        //            MessageBox.Show("StudentId is not yet registered. Please register first.");
        //        }
        //    }
        //    catch (Exception er)
        //    {
        //        MessageBox.Show(er.ToString());
        //        throw;
        //    }
        //    showStudents(0, "");
        //    return true;
        //}

        private bool registerUsers()
        {
            if (string.IsNullOrEmpty(tbUsername.Text) || string.IsNullOrEmpty(tbUserpass.Text))
            {
                MessageBox.Show("Please fill in the information needed");
                return false;
            }

            if (tbUsertype.SelectedItem == null)
            {
                MessageBox.Show("Please choose a user type");
                return false;
            }

            string Username = tbUsername.Text;
            string Password = tbUserpass.Text;
            string Usertype = tbUsertype.SelectedItem.ToString();


            string cmd = "INSERT INTO Users VALUES (@val1,@val2,@val3,@val4)";

            MD5 md5Hash = MD5.Create();
            string hash = GetMd5Hash(md5Hash, Password);

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);
            command.Parameters.AddWithValue("@val1", null);
            command.Parameters.AddWithValue("@val2", Username);
            command.Parameters.AddWithValue("@val3", hash);
            command.Parameters.AddWithValue("@val4", Usertype);

            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                MessageBox.Show("Register Success");
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            connection.Close();
            showUsers();
            return true;
        }

        public void showStudents(int grade, string section)
        {

            string cmd = "SELECT * FROM Students";
            //for filters
            if (grade > 0 || section != "")
            {
                if (grade > 0 && section != "") cmd = "SELECT * FROM Students WHERE Grade = " + grade + " AND Section LIKE '" + section + "'";
                else if (grade > 0 && section == "") cmd = "SELECT * FROM Students WHERE Grade = " + grade;
                else if (grade == 0 && section != "") cmd = "SELECT * FROM Students WHERE Section LIKE '" + section + "'";
            }
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);

            DataTable data = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(data);

            studentTable.DataSource = data;
        }

        //public void setStudTable(DataTable dt)
        //{
        //    this.studentTable.DataSource = dt;
        //}

        private void showUsers()
        {

            string cmd = "SELECT Username FROM Users";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);

            DataTable data = new DataTable();
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(data);

            dataGridView1.DataSource = data;
        }

        private void UserRegBtn_Click(object sender, EventArgs e)
        {
            if (registerUsers())
            {
                tbUsername.Clear();
                tbUserpass.Clear();
                tbUsertype.SelectedItem = null;
                tbUsertype.Text = "Select user type";
            }

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

        public void ScanBtn_Click(object sender, EventArgs e)
        {
            studentIn(tbStudCode.Text);
            showAttendance();
            tbStudCode.Clear();
        }

        public string studentIn(string code)
        {
            insertStudent(code);
            return getStudentName(currDate, code);
        }

        private void insertStudent(string code)
        {

            MySqlConnection connection = new MySqlConnection(connectionString);

            //check if the code box is empty
            if (string.IsNullOrEmpty(code))
            {
                MessageBox.Show("Please Input the code");
                return;
            }
            long studCode = Convert.ToInt64(code);

            //check if the student code is registered
            if (!isCodeExist(studCode))
            {
                MessageBox.Show("Student code is not yet registered. Please proceed on Students tab");
                return;
            }

            connection.Close();
            DateTime currTime = DateTime.Now;
            //time conditions
            DateTime tc = timeClass.Value;
            DateTime tl = timeLate.Value;
            DateTime ta = autoMailTime.Value;

            TimeSpan timeToPresent = new TimeSpan(tc.Hour, tc.Minute, tc.Second); //time of attending class
            TimeSpan timeToLate = new TimeSpan(tl.Hour, tl.Minute, tl.Second);   //time where students will be marked as late
            TimeSpan timeToAbsent = new TimeSpan(ta.Hour, ta.Minute-1, 0); //until this time students will be marked late then after 12 its absent
            TimeSpan timeNow = currTime.TimeOfDay;

            string status = "Present";
            if (timeNow >= timeToLate && timeNow < timeToAbsent)
            {
                status = "Late";
                //MessageBox.Show("Present");
            }

            //check student code if already scanned in current date
            string query = "SELECT * From Attendance WHERE StudentId = " + studCode + " AND Date = '" + currDate + "'";
            MySqlCommand cmdcheck = new MySqlCommand(query, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmdcheck.ExecuteReader();
                //if student already scanned this day
                if (reader.HasRows)
                {
                    connection.Close();
                    string cmd = "UPDATE Attendance SET TimeOut = '" + timeNow.ToString() + "', TimeSwipes = CONCAT(IFNULL(TimeSwipes,''),'" + timeNow.ToString() + ",') WHERE StudentId = " + studCode + " AND Date = '" + currDate + "'";
                    MySqlCommand command = new MySqlCommand(cmd, connection);
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.ToString());
                        throw;
                    }

                }
                //else first time to scan this day
                else
                {
                    connection.Close();
                    string cmd = "INSERT INTO Attendance (StudentId,Status,Date,TimeIn,TimeSwipes) VALUES (@val1,@val2,@val3,@val4,@val5)";

                    MySqlCommand command = new MySqlCommand(cmd, connection);
                    command.Parameters.AddWithValue("@val1", studCode);
                    command.Parameters.AddWithValue("@val2", status);
                    command.Parameters.AddWithValue("@val3", currDate);
                    command.Parameters.AddWithValue("@val4", timeNow);
                    command.Parameters.AddWithValue("@val5", timeNow.ToString() + ",");

                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();

                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.ToString());
                        throw;
                    }
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }

            connection.Close();
        }

        private string getStudentName(string date, string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return "--NAME--";
            }
            string name = "";
            string query = "SELECT Students.Fname, Students.Mname, Students.Sname, Students.Grade, Students.Section FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.StudentId = " + Convert.ToInt64(code) + " AND Attendance.Date = '" + date + "'";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string fullName = reader["Fname"].ToString() + " " + reader["Mname"].ToString().Substring(0, 1) + ". " + reader["Sname"].ToString();
                    name = fullName + "   %" + reader["Grade"].ToString() + "-" + reader["Section"].ToString();
                    scannedName.Text = name.Replace("%","");
                    
                    return name;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }

            return "";

        }

        private void showAttendance()
        {
            string cmd = "SELECT Attendance.Date, Students.Fname, Students.Mname, Students.Sname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "'";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);

            DataTable data = new DataTable();

            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            adapter.Fill(data);
            AttendanceTable.DataSource = data;
        }

        private void remarksBtn_Click(object sender, EventArgs e)
        {

            string remarks = tbRemarks.Text;
            string cmd = "UPDATE Attendance SET Remarks = '" + remarks + "'";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }

            tbRemarks.Clear();
            showAttendance();
        }

        private void StudFiltBtn_Click(object sender, EventArgs e)
        {
            int filtGrade = 0;
            string filtSection = "";
            //MessageBox.Show(StudFiltSection.Text);
            if (StudFiltSection.Text == "Section") filtSection = "";
            else filtSection = StudFiltSection.SelectedItem.ToString();
            if (StudFiltGrade.Text != "Grade") filtGrade = Convert.ToInt32(StudFiltGrade.SelectedItem);
            //MessageBox.Show(StudFiltSection.SelectedItem.ToString());
            showStudents(filtGrade, filtSection);
            //MessageBox.Show(filtGrade.ToString());
            //MessageBox.Show(filtSection.ToString());
        }

        private void StudFiltGrade_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void StudFiltRstBtn_Click(object sender, EventArgs e)
        {
            StudFiltGrade.Text = "Grade";
            StudFiltSection.Text = "Section";
            StudFiltSection.SelectedValue = "";
            showStudents(0, "");
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            DateTime now = DateTime.Now;
            DateTime mailTime = autoMailTime.Value;
            TimeLabel.Text = now.ToLongTimeString();

            TimeSpan tnow = new TimeSpan(now.Hour, now.Minute, now.Second);
            TimeSpan tmail = new TimeSpan(mailTime.Hour, mailTime.Minute, mailTime.Second);

            if (tnow == tmail && autoMailEnable == true)
            {
                fillStatusAbsent();
                autoExport();
            }

        }

        private void reviewAttendance(string dateTimeFilter, string statusFilter, int gradeFilter, string sectionFilter, bool forExport, bool delQuery)
        {
            dateRemarksBox.Clear();
            string remarks = "";

            MySqlConnection connection = new MySqlConnection(connectionString);

            string queryDate = "SELECT * FROM Attendance WHERE Date = '" + dateTimeFilter + "'";
            MySqlCommand cmdCheckDate = new MySqlCommand(queryDate, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmdCheckDate.ExecuteReader();
                if (reader.HasRows || chkBoxRevDate.Checked == false)
                {
                    while (reader.Read())
                    {
                        remarks = reader["Remarks"].ToString();
                        break;
                    }
                    connection.Close();
                    string query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Attendance.Status LIKE '" + statusFilter + "' AND Students.Grade = " + gradeFilter + " AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                    if (chkBoxRevDate.Checked == true)
                    {
                        if (statusFilter == "" && gradeFilter == 0 && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "" && gradeFilter == 0) query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "" && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Students.Grade = " + gradeFilter + " ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (gradeFilter == 0 && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Attendance.Status LIKE '" + statusFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Students.Grade = " + gradeFilter + " AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (gradeFilter == 0) query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Attendance.Status LIKE '" + statusFilter + "' AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' AND Attendance.Status LIKE '" + statusFilter + "' AND Students.Grade = " + gradeFilter + " ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                    }
                    else
                    {
                        if (statusFilter == "" && gradeFilter == 0 && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "" && gradeFilter == 0) query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "" && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Grade = " + gradeFilter + " ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (gradeFilter == 0 && sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Status LIKE '" + statusFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (statusFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Grade = " + gradeFilter + " AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (gradeFilter == 0) query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Status LIKE '" + statusFilter + "' AND Students.Section LIKE '" + sectionFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                        else if (sectionFilter == "") query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Status LIKE '" + statusFilter + "' AND Students.Grade = " + gradeFilter + " ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
                    }

                    if (delQuery == true)
                    {
                        string qryDel = "";

                        if (chkBoxRevDate.Checked == true)
                        {
                            if (statusFilter == "" && gradeFilter == 0 && sectionFilter == "") qryDel = "DELETE FROM Attendance WHERE Attendance.Date = '" + currDate + "'";
                            else if (statusFilter == "" && gradeFilter == 0) qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "' AND Students.Section LIKE '" + sectionFilter + "'";
                            else if (statusFilter == "" && sectionFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "' AND Students.Grade = " + gradeFilter;
                            else if (gradeFilter == 0 && sectionFilter == "") qryDel = "DELETE FROM Attendance WHERE Attendance.Date = '" + currDate + "' AND Attendance.Status LIKE '" + statusFilter + "'";
                            else if (statusFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "' AND Students.Section LIKE '" + sectionFilter + "' AND Students.Grade = " + gradeFilter;
                            else if (gradeFilter == 0) qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "' AND Students.Section LIKE '" + sectionFilter + "' AND Attendance.Status LIKE '" + statusFilter + "'";
                            else if (sectionFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + currDate + "' AND Students.Grade = " + gradeFilter + " AND Attendance.Status LIKE '" + statusFilter + "'";
                        }
                        else
                        {
                            if (statusFilter == "" && gradeFilter == 0 && sectionFilter == "") qryDel = "DELETE FROM Attendance";
                            else if (statusFilter == "" && gradeFilter == 0) qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Section LIKE '" + sectionFilter + "'";
                            else if (statusFilter == "" && sectionFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Grade = " + gradeFilter;
                            else if (gradeFilter == 0 && sectionFilter == "") qryDel = "DELETE FROM Attendance WHERE Attendance.Status LIKE '" + statusFilter + "'";
                            else if (statusFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Section LIKE '" + sectionFilter + "' AND Students.Grade = " + gradeFilter;
                            else if (gradeFilter == 0) qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Section LIKE '" + sectionFilter + "' AND Attendance.Status LIKE '" + statusFilter + "'";
                            else if (sectionFilter == "") qryDel = "DELETE Attendance FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Students.Grade = " + gradeFilter + " AND Attendance.Status LIKE '" + statusFilter + "'";
                        }
                        
                        MySqlCommand command = new MySqlCommand(qryDel, connection);
                        DialogResult result = MessageBox.Show("Do you really want to delete this query?", "This will delete all records of this query", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (result == DialogResult.Yes)
                        {
                            try
                            {
                                connection.Open();
                                command.ExecuteNonQuery();
                                MessageBox.Show("Query has successfully deleted.");
                            }
                            catch (Exception er)
                            {
                                MessageBox.Show(er.ToString());
                                throw;
                            }
                        }
                        connection.Close();
                        return;
                    }

                    if (forExport == true)
                    {
                        //StringBuilder sb = new StringBuilder();
                        MySqlDataAdapter da = new MySqlDataAdapter(query, connection);
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        ds.Tables[0].TableName = "Attendance";

                        ////initialize headers
                        //sb.Append("Date" + strDelimiter);
                        //sb.Append("Student Name" + strDelimiter);
                        //sb.Append("Grade" + strDelimiter);
                        //sb.Append("Section" + strDelimiter);
                        //sb.Append("Time In" + strDelimiter);
                        //sb.Append("Time Out" + strDelimiter);
                        //sb.Append("Status");
                        //sb.Append("\r\n");
                        //foreach (DataRow AttendanceDR in ds.Tables["Attendance"].Rows)
                        //{
                        //    //int attendanceId = Convert.ToInt32(AttendanceDR["Id"]);
                        //    //sb.Append(attendanceId.ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["Date"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["Name"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["Grade"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["Section"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["TimeIn"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["TimeOut"].ToString() + strDelimiter);
                        //    sb.Append(AttendanceDR["Status"].ToString());
                        //    sb.Append("\r\n");
                        //}
                        //DateTime dTFilter = Convert.ToDateTime(dateTimePicker1.Text);
                        //string d_filter = dTFilter.ToString("yyyy-MM-dd");
                        ////MessageBox.Show(d_filter);
                        //string strfilename = (strDelimiter == ",") ? (d_filter + "_" + gradeFilter + "_" + sectionFilter + "_" + statusFilter + "_" + "Data.csv") : (d_filter + "_" + gradeFilter + "_" + sectionFilter + "_" + statusFilter + "_" + "Data.txt");

                        //StreamWriter file = new StreamWriter(@"exports\" + strfilename);
                        //file.WriteLine(sb.ToString());
                        //file.Close();
                        DateTime dTFilter = Convert.ToDateTime(dateTimePicker1.Text);
                        string d_filter = dTFilter.ToString("yyyy-MM-dd");
                        string strfilename = d_filter + "_" + gradeFilter + "_" + sectionFilter + "_" + statusFilter + "_" + "Data.xlsx";
                        string filepath = Directory.GetCurrentDirectory() + @"\exports\" + strfilename;

                        if (File.Exists(filepath))
                        {
                            File.Delete(filepath);
                        }

                        Excel.Application oApp;
                        Excel.Worksheet oSheet;
                        Excel.Workbook oBook;

                        oApp = new Excel.Application();
                        oBook = oApp.Workbooks.Add();
                        oSheet = null;

                        bool initExcelSheet = true;
                        int worksheet = 0;
                        int grade = 0;
                        string section = "";
                        int row = 0;
                        int numStudents = 0;
                        int numPresent = 0;

                        foreach (DataRow AttendanceDR in ds.Tables["Attendance"].Rows)
                        {
                            //MessageBox.Show(ds.Tables["Attendance"].Select("Section LIKE '" + AttendanceDR["Section"].ToString() +"'").Length.ToString());
                            //MessageBox.Show(AttendanceDR["Section"].ToString());
                            if (grade != Convert.ToInt32(AttendanceDR["Grade"]))
                            {
                                if (section != AttendanceDR["Section"].ToString() && row > 1)
                                {
                                    row++;
                                    oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;
                                    row++; ;
                                }
                                grade = Convert.ToInt32(AttendanceDR["Grade"]);

                                //MessageBox.Show("Grade"+grade);
                                initExcelSheet = false;
                                worksheet++;
                            }

                            if (initExcelSheet == false)
                            {
                                //adding of sheets per grade
                                if (worksheet > 3)
                                {
                                    oSheet = (Excel.Worksheet)oBook.Worksheets.Add(After: oBook.Sheets[oBook.Sheets.Count]);
                                }
                                oSheet = (Excel.Worksheet)oBook.Worksheets.get_Item(worksheet); //sheets
                                //MessageBox.Show("Add sheetname");
                                oSheet.Name = "Grade " + grade;
                                initExcelSheet = true;
                                section = "";
                                row = 0;
                            }
                            if (section != AttendanceDR["Section"].ToString() && row > 1)
                            {
                                row++;
                                oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;
                                row++; ;
                            }

                            if (section != AttendanceDR["Section"].ToString())
                            {
                                numPresent = 0;
                                row++;
                                numStudents = ds.Tables["Attendance"].Select("Section LIKE '" + AttendanceDR["Section"].ToString() + "'").Length;
                                section = AttendanceDR["Section"].ToString();
                                oSheet.Cells[row, 1] = "Section: " + section;
                                row++;
                                oSheet.Cells[row, 1] = "Date";
                                oSheet.Cells[row, 2] = "Surname";
                                oSheet.Cells[row, 3] = "Firstname";
                                oSheet.Cells[row, 4] = "Middlename";
                                oSheet.Cells[row, 5] = "Time In";
                                oSheet.Cells[row, 6] = "Time Out";
                                oSheet.Cells[row, 7] = "Status";
                                row++;
                                oSheet.Cells[row, 1] = Convert.ToDateTime(AttendanceDR["Date"].ToString()).ToShortDateString();
                                oSheet.Cells[row, 2] = AttendanceDR["Sname"].ToString();
                                oSheet.Cells[row, 3] = AttendanceDR["Fname"].ToString();
                                oSheet.Cells[row, 4] = AttendanceDR["Mname"].ToString()[0] + ".";
                                oSheet.Cells[row, 5] = AttendanceDR["TimeIn"].ToString();
                                oSheet.Cells[row, 6] = AttendanceDR["TimeOut"].ToString();
                                oSheet.Cells[row, 7] = AttendanceDR["Status"].ToString();
                            }
                            else
                            {
                                row++;
                                oSheet.Cells[row, 1] = Convert.ToDateTime(AttendanceDR["Date"].ToString()).ToShortDateString();
                                oSheet.Cells[row, 2] = AttendanceDR["Sname"].ToString();
                                oSheet.Cells[row, 3] = AttendanceDR["Fname"].ToString();
                                oSheet.Cells[row, 4] = AttendanceDR["Mname"].ToString()[0] + ".";
                                oSheet.Cells[row, 5] = AttendanceDR["TimeIn"].ToString();
                                oSheet.Cells[row, 6] = AttendanceDR["TimeOut"].ToString();
                                oSheet.Cells[row, 7] = AttendanceDR["Status"].ToString();
                            }
                            if (AttendanceDR["Status"].ToString() == "Present" || AttendanceDR["Status"].ToString() == "Late") numPresent++;
                            oSheet.Range["C:D"].NumberFormat = "[$-409]h:mm:ss AM/PM";
                            oSheet.Columns.AutoFit();
                        }
                        oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;
                        row++; ;

                        oBook.SaveAs(filepath);
                        oBook.Close();
                        oApp.Quit();

                        MessageBox.Show("File Successfully Exported.");
                    }
                    else
                    {
                        MySqlCommand command = new MySqlCommand(query, connection);
                        DataTable data = new DataTable();

                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        adapter.Fill(data);
                        
                        reviewTable.DataSource = data;
                        dateRemarksBox.Text = remarks;
                    }

                }
                else
                {
                    MessageBox.Show("No attendance made in such date");

                    reviewTable.DataSource = null;
                    connection.Close();
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
        }

        private void dataGridView3_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("Data grid view 3 is selected");
            DataGridView dgv = sender as DataGridView;
            if (dgv == null)
                return;
            if (dgv.CurrentRow.Selected)
            {
                MessageBox.Show("Row" + dgv.CurrentRow + " is selected");
            }
        }

        private void resetRevBtn_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedItem = null;
            studFiltSectionReview.SelectedItem = null;
            comboBox3.SelectedItem = null;
            comboBox1.Text = "All";
            studFiltSectionReview.Text = "All";
            comboBox3.Text = "All";

            reviewAttendance(Convert.ToDateTime(dateTimePicker1.Text).ToShortDateString(), "", 0, "", false, false);
        }

        private void delStudId_Click(object sender, EventArgs e)
        {
            deleteStudentById();
            deleteStudBox.Text = "(type student Id here)";
        }

        private void deleteStudentById()
        {
            if (string.IsNullOrEmpty(deleteStudBox.Text) || deleteStudBox.Text == "(type student Id here)")
            {
                MessageBox.Show("Please input the code to be deleted");
                return;
            }

            long code = Convert.ToInt64(deleteStudBox.Text);
            string cmd = "DELETE FROM Attendance WHERE StudentId = " + code + "; DELETE FROM Students WHERE Id = " + code;
            if (isCodeExist(code))
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand(cmd, connection);
                DialogResult result = MessageBox.Show("Do you really want to delete the student code \"" + code + "\"?", "This will delete all records of this student", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    try
                    {
                        connection.Open();
                        command.ExecuteNonQuery();
                        MessageBox.Show("Student has been successfully deleted.");
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.ToString());
                        throw;
                    }

                    connection.Close();
                }
            }

            else
            {
                MessageBox.Show("Student cannot be deleted because it is not yet registered");
            }
            deleteStudBox.Clear();
            showStudents(0, "");
            showAttendance();
        }

        private void deleteAttendanceById()
        {
            if (string.IsNullOrEmpty(delAttendanceBox.Text))
            {
                MessageBox.Show("Please input attendance id.");
                return;
            }
            int code = Convert.ToInt32(delAttendanceBox.Text);

            string cmd = "DELETE FROM Attendance WHERE Id = " + code;
            DialogResult result = MessageBox.Show("Do you really want to delete the attendance \"" + code + "\"?", "This will delete the record of this attendance", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand(cmd, connection);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                    throw;
                }
                connection.Close();
                delAttendanceBox.Clear();
                reviewAttendance(Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd"), "", 0, "", false, false);
            }
            else return;
        }

        private void delAllQuery_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(dateTimePicker1.Text);
            DateTime dTFilter = Convert.ToDateTime(dateTimePicker1.Text);
            string dateTimeFilter = dTFilter.ToString("yyyy-MM-dd");
            //MessageBox.Show(dateTimeFilter.ToShortDateString());
            string statusFilter = Convert.ToString(comboBox3.SelectedItem);
            int revGradeFilter = Convert.ToInt32(comboBox1.SelectedItem);
            string revSectionFilter = (studFiltSectionReview.Text == "All") ? "" : studFiltSectionReview.SelectedItem.ToString();
            bool forExport = false;
            bool delQuery = true;

            reviewAttendance(dateTimeFilter, statusFilter, revGradeFilter, revSectionFilter, forExport, delQuery); // (str,str,int,int,bool,bool)
            delQuery = false;
            reviewAttendance(dateTimeFilter, statusFilter, revGradeFilter, revSectionFilter, forExport, delQuery);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBoxRevDate.Checked == true) dateTimePicker1.Enabled = true;
            else dateTimePicker1.Enabled = false;
        }

        private void exportBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(dateTimePicker1.Text);
            DateTime dTFilter = Convert.ToDateTime(dateTimePicker1.Text);
            string dateTimeFilter = dTFilter.ToString("yyyy-MM-dd");
            //MessageBox.Show(dateTimeFilter.ToShortDateString());
            string statusFilter = Convert.ToString(comboBox3.SelectedItem);
            int revGradeFilter = Convert.ToInt32(comboBox1.SelectedItem);
            string revSectionFilter = (studFiltSectionReview.Text == "All") ? "" : studFiltSectionReview.SelectedItem.ToString();
            bool forExport = true;
            bool delQuery = false;

            reviewAttendance(dateTimeFilter, statusFilter, revGradeFilter, revSectionFilter, forExport, delQuery); // (str,str,int,int,bool,bool)
        }

        private void mailSendBtn_Click(object sender, EventArgs e)
        {
            sendMailManual();
        }

        private void sendMailManual()
        {
            mailLogin = new NetworkCredential(mailUsername.Text, mailPassword.Text);
            client = new SmtpClient(mailSmtp.Text);
            client.Port = Convert.ToInt32(mailPort.Text);
            client.EnableSsl = mailSSL.Checked;
            client.Credentials = mailLogin;

            string senderName = mailUsername.Text.Substring(0, mailUsername.Text.IndexOf('@'));

            msg = new MailMessage { From = new MailAddress(mailUsername.Text + mailSmtp.Text.Replace("Smtp", "@"), senderName, Encoding.UTF8) };

            if (!string.IsNullOrEmpty(attachmentBox1.Text)) msg.Attachments.Add(new Attachment(attachmentBox1.Text));
            msg.To.Add(new MailAddress(mailTo.Text));
            if (!string.IsNullOrEmpty(attachmentBox2.Text)) msg.Attachments.Add(new Attachment(attachmentBox2.Text));
            msg.To.Add(new MailAddress(mailTo.Text));
            if (!string.IsNullOrEmpty(attachmentBox3.Text)) msg.Attachments.Add(new Attachment(attachmentBox3.Text));
            msg.To.Add(new MailAddress(mailTo.Text));

            if (!string.IsNullOrEmpty(mailCC.Text))
            {
                string receivers = mailCC.Text;
                string[] mails = receivers.Split(',');
                if (mails.Length > 0)
                {
                    foreach (string names in mails)
                    {
                        msg.To.Add(new MailAddress(names));
                    }
                }
                else msg.To.Add(new MailAddress(mailCC.Text));
            }
            msg.Subject = mailSubject.Text;
            msg.Body = mailMessage.Text;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.Normal;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            client.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            string userstate = "sending...";
            client.SendAsync(msg, userstate);
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
                MessageBox.Show(string.Format("{0} send canceled.", e.UserState), "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (e.Error != null)
                MessageBox.Show(string.Format("{0} {1}", e.UserState, e.Error), "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                MessageBox.Show("Mail has been successfully sent.", "Message", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void attachBtn1_Click(object sender, EventArgs e)
        {
            attachFile(attachmentBox1);
        }

        private void attachBtn2_Click_1(object sender, EventArgs e)
        {
            attachFile(attachmentBox2);
        }

        private void attachBtn3_Click(object sender, EventArgs e)
        {
            attachFile(attachmentBox3);
        }

        private void attachFile(TextBox tb)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            //dlg.Filter = "JPG Files(*.jpg)|*.jpg|PNG Files(*.png)|*.png|All Files(*.)";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                string filepath = dlg.FileName.ToString();
                tb.Text = filepath;
            }
        }

        private void reviewTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("Row " + dataGridView3.CurrentRow);
            int rowIndex = reviewTable.CurrentRow.Index;
            int dateColIndex = 1, nameColIndex = 2;              //indexes of date and name necessary for displaying timeswipes function

            string date = reviewTable.Rows[rowIndex].Cells[dateColIndex].Value.ToString();
            string sname = reviewTable.Rows[rowIndex].Cells[nameColIndex].Value.ToString();
            string fname = reviewTable.Rows[rowIndex].Cells[nameColIndex+1].Value.ToString();
            string mname = reviewTable.Rows[rowIndex].Cells[nameColIndex+2].Value.ToString()[0] + ".";
            string name = sname + ", " + fname + ", " + mname;
            DateTime formattedDate = Convert.ToDateTime(date);
            string shortDate = formattedDate.ToString("yyyy-MM-dd");

            var th = new TimeHistory();

            //auto generate objects

            Label label1 = new Label();
            label1.AutoSize = true;
            label1.Location = new Point(10, 50);
            label1.Name = "label1";
            label1.Size = new Size(35, 13);
            label1.TabIndex = 0;
            label1.Text = "Name: ";
            th.Controls.Add(label1);

            TextBox textBox1 = new TextBox();
            textBox1.Location = new Point(80, 51);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(200, 20);
            textBox1.Text = name;
            textBox1.TabIndex = 9;
            textBox1.ReadOnly = true;
            th.Controls.Add(textBox1);

            Label label2 = new Label();
            label2.AutoSize = true;
            label2.Location = new Point(10, 85);
            label2.Name = "label2";
            label2.Size = new Size(35, 13);
            label2.TabIndex = 0;
            label2.Text = "Date: ";
            th.Controls.Add(label2);

            DateTimePicker dateLabel = new DateTimePicker();
            dateLabel.Location = new Point(80, 84);
            dateLabel.Value = formattedDate;
            dateLabel.Enabled = false;
            th.Controls.Add(dateLabel);


            string query = "SELECT Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut, Attendance.TimeSwipes FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + shortDate + "' AND Students.Sname LIKE '" + sname + "'";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    //MessageBox.Show(reader["Grade"].ToString() + " " + reader["Section"].ToString());

                    Label label3 = new Label();
                    label3.AutoSize = true;
                    label3.Location = new Point(10, 120);
                    label3.Name = "label3";
                    label3.Size = new Size(35, 13);
                    label3.TabIndex = 0;
                    label3.Text = "Grade: ";
                    th.Controls.Add(label3);

                    TextBox textBox2 = new TextBox();
                    textBox2.Location = new Point(80, 120);
                    textBox2.Name = "textBox2";
                    textBox2.Size = new Size(70, 20);
                    textBox2.Text = reader["Grade"].ToString();
                    textBox2.TabIndex = 10;
                    textBox2.ReadOnly = true;
                    th.Controls.Add(textBox2);

                    Label label4 = new Label();
                    label4.AutoSize = true;
                    label4.Location = new Point(150, 120);
                    label4.Name = "label4";
                    label4.Size = new Size(35, 13);
                    label4.TabIndex = 0;
                    label4.Text = "Section: ";
                    th.Controls.Add(label4);

                    TextBox textBox3 = new TextBox();
                    textBox3.Location = new Point(210, 120);
                    textBox3.Name = "textBox3";
                    textBox3.Size = new Size(70, 20);
                    textBox3.Text = reader["Section"].ToString();
                    textBox3.TabIndex = 11;
                    textBox3.ReadOnly = true;
                    th.Controls.Add(textBox3);

                    Label label5 = new Label();
                    label5.AutoSize = true;
                    label5.Location = new Point(10, 155);
                    label5.Name = "label5";
                    label5.Size = new Size(35, 13);
                    label5.TabIndex = 0;
                    label5.Text = "Time In: ";
                    th.Controls.Add(label5);

                    TextBox textBox4 = new TextBox();
                    textBox4.Location = new Point(80, 155);
                    textBox4.Name = "textBox4";
                    textBox4.Size = new Size(200, 20);
                    textBox4.Text = reader["TimeIn"].ToString();
                    textBox4.TabIndex = 13;
                    textBox4.ReadOnly = true;
                    th.Controls.Add(textBox4);

                    Label label6 = new Label();
                    label6.AutoSize = true;
                    label6.Location = new Point(10, 185);
                    label6.Name = "label6";
                    label6.Size = new Size(35, 13);
                    label6.TabIndex = 0;
                    label6.Text = "Time Out: ";
                    th.Controls.Add(label6);

                    TextBox textBox5 = new TextBox();
                    textBox5.Location = new Point(80, 185);
                    textBox5.Name = "textBox5";
                    textBox5.Size = new Size(200, 20);
                    textBox5.Text = reader["TimeOut"].ToString();
                    textBox5.TabIndex = 14;
                    textBox5.ReadOnly = true;
                    th.Controls.Add(textBox5);

                    Label label7 = new Label();
                    label7.AutoSize = true;
                    label7.Location = new Point(10, 220);
                    label7.Name = "label7";
                    label7.Size = new Size(35, 13);
                    label7.TabIndex = 6;
                    label7.Text = "Status: ";
                    th.Controls.Add(label7);

                    TextBox textBox6 = new TextBox();
                    textBox6.Location = new Point(80, 225);
                    textBox6.Name = "textBox6";
                    textBox6.Size = new Size(200, 20);
                    textBox6.Text = reader["Status"].ToString();
                    textBox6.TabIndex = 15;
                    textBox6.ReadOnly = true;
                    th.Controls.Add(textBox6);

                    Label label8 = new Label();
                    label8.AutoSize = true;
                    label8.Location = new Point(10, 255);
                    label8.Name = "label8";
                    label8.Size = new Size(35, 13);
                    label8.TabIndex = 7;
                    label8.Text = "Swipes: ";
                    th.Controls.Add(label8);

                    string swipesText = reader["TimeSwipes"].ToString();
                    string[] timeSwipes = swipesText.Split(',');


                    ComboBox comboBox1 = new ComboBox();
                    comboBox1.FormattingEnabled = true;
                    foreach (string time in timeSwipes)
                    {
                        comboBox1.Items.Add(time);
                    }
                    comboBox1.Location = new Point(80, 255);
                    comboBox1.Name = "comboBox1";
                    comboBox1.Size = new Size(200, 21);
                    comboBox1.TabIndex = 16;
                    comboBox1.Text = "Time History";
                    th.Controls.Add(comboBox1);
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }

            th.Show();
        }

        private void autoMailChkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (autoMailChkBox.Checked == true)
            {
                autoMailTo.Enabled = true;
                autoMailCC.Enabled = true;
                autoMailSubj.Enabled = true;
                autoMailMsg.Enabled = true;
                autoMailUser.Enabled = true;
                autoMailPass.Enabled = true;
                autoMailTime.Enabled = true;
                autoMailEnable = true;
            }

            else
            {
                autoMailTo.Enabled = false;
                autoMailCC.Enabled = false;
                autoMailSubj.Enabled = false;
                autoMailMsg.Enabled = false;
                autoMailUser.Enabled = false;
                autoMailPass.Enabled = false;
                autoMailTime.Enabled = false;
                autoMailEnable = false;
            }
        }

        private void tabPage2_Click(object sender, EventArgs e)
        {

        }

        private void delReviewId_Click(object sender, EventArgs e)
        {
            deleteAttendanceById();
            delAttendanceBox.Text = "(type Id here)";
        }

        private void deleteStudBox_MouseClick(object sender, MouseEventArgs e)
        {
            deleteStudBox.Clear();
        }

        private void deleteStudBox_TabStopChanged(object sender, EventArgs e)
        {
            deleteStudBox.Clear();
        }


        private void setLatestFileName(string filename)
        {
            latestfile = filename;
        }

        private string getLatestFileName()
        {
            return latestfile;
        }

        private void autoExport()
        {
            DateTime dTFilter = DateTime.Now;
            string dateTimeFilter = dTFilter.ToString("yyyy-MM-dd");

            MySqlConnection connection = new MySqlConnection(connectionString);

            string query = "SELECT Attendance.Id, Attendance.Date, Students.Sname, Students.Fname, Students.Mname, Students.Grade, Students.Section, Attendance.Status, Attendance.TimeIn, Attendance.TimeOut FROM Attendance INNER JOIN Students ON Attendance.StudentId = Students.Id WHERE Attendance.Date = '" + dateTimeFilter + "' ORDER BY Students.Grade ASC, Students.Section ASC, Students.Sname ASC";
            MySqlDataAdapter da = new MySqlDataAdapter(query, connection);

            DataSet ds = new DataSet();
            da.Fill(ds);
            ds.Tables[0].TableName = "Attendance";

            string d_filter = dTFilter.ToString("yyyy-MM-dd");
            string strfilename = d_filter + "_0_0__Data.xlsx";
            string filepath = Directory.GetCurrentDirectory() + @"\exports\" + strfilename;

            if (File.Exists(filepath))
            {
                File.Delete(filepath);
            }

            Excel.Application oApp;
            Excel.Worksheet oSheet;
            Excel.Workbook oBook;

            oApp = new Excel.Application();
            oBook = oApp.Workbooks.Add();
            oSheet = null;

            bool initExcelSheet = true;
            int worksheet = 0;
            int grade = 0;
            string section = "";
            int row = 0;
            int numStudents = 0;
            int numPresent = 0;

            foreach (DataRow AttendanceDR in ds.Tables["Attendance"].Rows)
            {
                //MessageBox.Show(ds.Tables["Attendance"].Select("Section LIKE '" + AttendanceDR["Section"].ToString() +"'").Length.ToString());
                //MessageBox.Show(AttendanceDR["Section"].ToString());
                if (grade != Convert.ToInt32(AttendanceDR["Grade"]))
                {
                    if (row > 1)
                    {
                        row++;
                        oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;
                    }
                    grade = Convert.ToInt32(AttendanceDR["Grade"]);

                    //MessageBox.Show("Grade"+grade);
                    initExcelSheet = false;
                    worksheet++;
                }

                if (initExcelSheet == false)
                {
                    //adding of sheets per grade
                    if (worksheet > 3)
                    {
                        oSheet = (Excel.Worksheet)oBook.Worksheets.Add(After: oBook.Sheets[oBook.Sheets.Count]);
                    }
                    oSheet = (Excel.Worksheet)oBook.Worksheets.get_Item(worksheet); //sheets
                    //MessageBox.Show("Add sheetname");
                    oSheet.Name = "Grade " + grade;
                    initExcelSheet = true;
                    section = "";
                    row = 0;
                }
                if (section != AttendanceDR["Section"].ToString())
                {
                    row++;
                    if (row > 1)
                    {
                        oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;
                        row += 2;
                    }
                    numStudents = ds.Tables["Attendance"].Select("Section LIKE '" + AttendanceDR["Section"].ToString() + "'").Length;
                    numPresent = 0;
                    section = AttendanceDR["Section"].ToString();
                    oSheet.Cells[row, 1] = "Section: " + section;
                    row++;
                    oSheet.Cells[row, 1] = "Date";
                    oSheet.Cells[row, 2] = "Surname";
                    oSheet.Cells[row, 3] = "Firstname";
                    oSheet.Cells[row, 4] = "Middlename";
                    oSheet.Cells[row, 5] = "Time In";
                    oSheet.Cells[row, 6] = "Time Out";
                    oSheet.Cells[row, 7] = "Status";
                    row++;
                    oSheet.Cells[row, 1] = Convert.ToDateTime(AttendanceDR["Date"].ToString()).ToShortDateString();
                    oSheet.Cells[row, 2] = AttendanceDR["Sname"].ToString();
                    oSheet.Cells[row, 3] = AttendanceDR["Fname"].ToString();
                    oSheet.Cells[row, 4] = AttendanceDR["Mname"].ToString()[0] + ".";
                    oSheet.Cells[row, 5] = AttendanceDR["TimeIn"].ToString();
                    oSheet.Cells[row, 6] = AttendanceDR["TimeOut"].ToString();
                    oSheet.Cells[row, 7] = AttendanceDR["Status"].ToString();
                    if (AttendanceDR["Status"].ToString() == "Present" || AttendanceDR["Status"].ToString() == "Late") numPresent++;
                }
                else
                {
                    row++;
                    oSheet.Cells[row, 1] = Convert.ToDateTime(AttendanceDR["Date"].ToString()).ToShortDateString();
                    oSheet.Cells[row, 2] = AttendanceDR["Sname"].ToString();
                    oSheet.Cells[row, 3] = AttendanceDR["Fname"].ToString();
                    oSheet.Cells[row, 4] = AttendanceDR["Mname"].ToString();//[0] + ".";
                    oSheet.Cells[row, 5] = AttendanceDR["TimeIn"].ToString();
                    oSheet.Cells[row, 6] = AttendanceDR["TimeOut"].ToString();
                    oSheet.Cells[row, 7] = AttendanceDR["Status"].ToString();
                    if (AttendanceDR["Status"].ToString() == "Present" || AttendanceDR["Status"].ToString() == "Late") numPresent++;
                }
                oSheet.Range["C:D"].NumberFormat = "[$-409]h:mm:ss AM/PM";
                oSheet.Columns.AutoFit();
            }
            row++;
            oSheet.Cells[row, 1] = "Students Attended: " + numPresent + "/" + numStudents;

            oBook.SaveAs(filepath);
            oBook.Close();
            oApp.Quit();


            mailLogin = new NetworkCredential(autoMailUser.Text, autoMailPass.Text);
            client = new SmtpClient("smtp.gmail.com");
            client.Port = Convert.ToInt32("587");
            client.EnableSsl = true;
            client.Credentials = mailLogin;

            string senderName = autoMailUser.Text.Substring(0, autoMailUser.Text.IndexOf('@'));

            msg = new MailMessage { From = new MailAddress(autoMailUser.Text + mailSmtp.Text.Replace("Smtp", "@"), senderName, Encoding.UTF8) };

            msg.Attachments.Add(new Attachment(filepath));
            msg.To.Add(new MailAddress(autoMailTo.Text));

            if (!string.IsNullOrEmpty(autoMailCC.Text))
            {
                string receivers = autoMailCC.Text;
                string[] mails = receivers.Split(',');
                if (mails.Length > 0)
                {
                    foreach (string names in mails)
                    {
                        msg.To.Add(new MailAddress(names));
                    }
                }
                else msg.To.Add(new MailAddress(autoMailCC.Text));
            }
            msg.Subject = autoMailSubj.Text;
            msg.Body = autoMailMsg.Text;
            msg.BodyEncoding = Encoding.UTF8;
            msg.IsBodyHtml = true;
            msg.Priority = MailPriority.Normal;
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            string userstate = "sending...";
            client.SendAsync(msg, userstate);

        }

        private static string getCurrentDate()
        {
            DateTime now = DateTime.Now;
            return now.ToString("yyyy-MM-dd");
        }

        private void logoutBtn_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Show();
            this.Hide();
        }

        private void addVid_Click(object sender, EventArgs e)
        {
            videoPathBox.Clear();
            attachFile(videoPathBox);
            //bandi.setVideoPath(videoPathBox.Text);
            //bandi._Play();
        }

        public string getVideoPath()
        {
            return videoPathBox.Text;
        }

        public void studProfEditSubmit(object sender, EventArgs e)
        {
            showStudents(0, "");
        }

        private void studentTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show("Row " + dataGridView3.CurrentRow);
            int rowIndex = studentTable.CurrentRow.Index;
            int fnameCol = 1, mnameCol = 2, snameCol = 3;          //indexes of date and name necessary for displaying timeswipes function

            string fname = studentTable.Rows[rowIndex].Cells[fnameCol].Value.ToString();
            string mname = studentTable.Rows[rowIndex].Cells[mnameCol].Value.ToString();
            string sname = studentTable.Rows[rowIndex].Cells[snameCol].Value.ToString();

            string query = "SELECT * FROM students WHERE Fname = '" + fname + "' AND Mname = '" + mname + "' AND Sname = '" + sname + "'";

            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(query, connection);
            var sp = new StudentProfile();
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    sp.setIp(ipAddress);
                    sp.setId(reader["Id"].ToString());
                    sp.setFname(reader["Fname"].ToString());
                    sp.setMname(reader["Mname"].ToString());
                    sp.setSname(reader["Sname"].ToString());
                    sp.setGrade(reader["Grade"].ToString());
                    sp.setSection(reader["Section"].ToString());

                    // 
                    // button1
                    // 
                    Button studEditProfileBtn = new Button();
                    studEditProfileBtn.Location = new System.Drawing.Point(120, 319);
                    studEditProfileBtn.Name = "editBtn";
                    studEditProfileBtn.Size = new System.Drawing.Size(75, 23);
                    studEditProfileBtn.TabIndex = 12;
                    studEditProfileBtn.Text = "Edit";
                    studEditProfileBtn.UseVisualStyleBackColor = true;
                    studEditProfileBtn.TabIndex = 5;
                    studEditProfileBtn.Click += new System.EventHandler(sp.studProfEdit);
                    studEditProfileBtn.Cursor = System.Windows.Forms.Cursors.Hand;
                    sp.Controls.Add(studEditProfileBtn);
                    // 
                    // button2
                    // 
                    Button studEditSubmitBtn = new Button();
                    studEditSubmitBtn.Location = new System.Drawing.Point(201, 319);
                    studEditSubmitBtn.Name = "submitBtn";
                    studEditSubmitBtn.Size = new System.Drawing.Size(75, 23);
                    studEditSubmitBtn.TabIndex = 13;
                    studEditSubmitBtn.Text = "Submit";
                    studEditSubmitBtn.UseVisualStyleBackColor = true;
                    studEditSubmitBtn.TabIndex = 6;
                    studEditSubmitBtn.Click += new System.EventHandler(sp.studProfEditSubmit);
                    studEditSubmitBtn.Click += new System.EventHandler(studProfEditSubmit);
                    studEditSubmitBtn.Cursor = System.Windows.Forms.Cursors.Hand;
                    sp.Controls.Add(studEditSubmitBtn);

                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            sp.Show();
        }

        public bool isCodeExist(long code)
        {
            string query = "SELECT * FROM Students WHERE Id = " + code;
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmdcheck = new MySqlCommand(query, connection);

            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmdcheck.ExecuteReader();

                //if the id is already used by another student
                if (reader.HasRows)
                {
                    connection.Close();
                    return true;
                }
                else
                {
                    connection.Close();
                    return false;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
        }

        private void deleteAllBtn_Click(object sender, EventArgs e)
        {
            string cmd = "DELETE FROM Students";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);
            DialogResult result = MessageBox.Show("Do you really want to delete all student codes?", "This will delete all student records", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Student has been successfully deleted.");
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                    throw;
                }

                connection.Close();
            }
            else return;
        }

        private void settingsSaveBtn_Click(object sender, EventArgs e)
        {
            string cmd = "SELECT * FROM settings";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(cmd, connection);

            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();

                string classTime = timeClass.Value.ToString("H:mm");
                string lateTime = timeLate.Value.ToString("H:mm");
                string mailTime = autoMailTime.Value.ToString("H:mm");
                string mailTo = autoMailTo.Text;
                string mailCC = autoMailCC.Text;
                string mailSubj = autoMailSubj.Text;
                string mailMsg = autoMailMsg.Text;
                string mailUser = autoMailUser.Text;
                string mailPass = Encrypt(autoMailPass.Text);
                bool mailCheckBox = autoMailChkBox.Checked;
                string videoPath = videoPathBox.Text;
                if (videoPathBox.Text.IndexOf('%') >= 0)
                {
                    MessageBox.Show("Invalid video path name replace '%' with a proper name");
                    return;
                }
                videoPath = videoPath.Replace("'", "%");
                videoPath = videoPath.Replace(@"\", @"\\");

                if (reader.HasRows)
                {
                    connection.Close();
                    string update = @"UPDATE settings SET classTime = '" + classTime
                        + "', lateTime = '" + lateTime + "', autoMailTime = '" + mailTime
                        + "', autoMailTo = '" + mailTo + "', autoMailCC = '" + mailCC
                        + "', autoMailSubj = '" + mailSubj + "', autoMailMsg = '" + mailMsg
                        + "', autoMailUser = '" + mailUser + "', autoMailPass = '" + mailPass
                        + "', autoMailChkBox = " + mailCheckBox + ", videoPathBox = '" + videoPath + "' WHERE Id = 5";

                    MySqlCommand cmd_ = new MySqlCommand(update, connection);
                    try
                    {
                        connection.Open();
                        cmd_.ExecuteNonQuery();
                        MessageBox.Show("Settings has been edited successfully.");
                    }
                    catch (Exception er)
                    {
                        MessageBox.Show(er.ToString());
                        throw;
                    }
                }
                else
                {
                    connection.Close();
                    DialogResult result = MessageBox.Show("Do you really want to save the following settings?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        string insert = "INSERT INTO settings(classTime,lateTime,autoMailTime,autoMailTo,autoMailCC,autoMailSubj,autoMailMsg,autoMailUser,autoMailPass,autoMailChkBox,videoPathBox) VALUES (@val1,@val2,@val3,@val4,@val5,@val6,@val7,@val8,@val9,@val10,@val11)";
                        MySqlCommand _cmd = new MySqlCommand(insert, connection);
                        _cmd.Parameters.AddWithValue("@val1", classTime);
                        _cmd.Parameters.AddWithValue("@val2", lateTime);
                        _cmd.Parameters.AddWithValue("@val3", mailTime);
                        _cmd.Parameters.AddWithValue("@val4", mailTo);
                        _cmd.Parameters.AddWithValue("@val5", mailCC);
                        _cmd.Parameters.AddWithValue("@val6", mailSubj);
                        _cmd.Parameters.AddWithValue("@val7", mailMsg);
                        _cmd.Parameters.AddWithValue("@val8", mailUser);
                        _cmd.Parameters.AddWithValue("@val9", mailPass);
                        _cmd.Parameters.AddWithValue("@val10", mailCheckBox);
                        _cmd.Parameters.AddWithValue("@val11", videoPath);

                        try
                        {
                            connection.Open();
                            _cmd.ExecuteNonQuery();
                            MessageBox.Show("Settings has bee saved successfully.");
                        }
                        catch (Exception er)
                        {
                            MessageBox.Show(er.ToString());
                            throw;
                        }
                    }
                    else return;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
        }

        private void getSettings()
        {
            string query = "SELECT * FROM settings";
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);

            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();

                while (reader.Read())
                {

                    //MessageBox.Show(reader["classTime"].ToString());
                    timeClass.Text = reader["classTime"].ToString();
                    timeLate.Text = reader["lateTime"].ToString();
                    autoMailTime.Text = reader["autoMailTime"].ToString();
                    autoMailTo.Text = reader["autoMailTo"].ToString();
                    autoMailCC.Text = reader["autoMailCC"].ToString();
                    autoMailSubj.Text = reader["autoMailSubj"].ToString();
                    autoMailMsg.Text = reader["autoMailMsg"].ToString();
                    autoMailUser.Text = reader["autoMailUser"].ToString();
                    autoMailPass.Text = Decrypt(reader["autoMailPass"].ToString());
                    autoMailChkBox.Checked = Convert.ToBoolean(reader["autoMailChkBox"]);
                    videoPathBox.Text = reader["videoPathBox"].ToString().Replace("%", "'");
                }

            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
        }

        public static string Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        public static string Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
        }

        private void settingsResetBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you really want to reset the following settings?", "The following data will be erased", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (result == DialogResult.Yes)
            {
                DateTime tc = new DateTime();
                DateTime tl = new DateTime();
                DateTime amt = new DateTime();

                timeClass.Text = tc.Add(new TimeSpan(7, 0, 0)).ToString("h:mm tt");
                timeLate.Text = tl.Add(new TimeSpan(7, 30, 0)).ToString("h:mm tt");
                autoMailTime.Text = amt.Add(new TimeSpan(8, 0, 0)).ToString("h:mm tt");
                autoMailTo.Clear();
                autoMailCC.Clear();
                autoMailSubj.Clear();
                autoMailMsg.Clear();
                autoMailUser.Clear();
                autoMailPass.Clear();
                autoMailChkBox.Checked = false;
                videoPathBox.Clear();
            }
            else return;
        }

        private void initializeSettings()
        {

        }

        private void delAttendanceBox_TabStopChanged(object sender, EventArgs e)
        {
            delAttendanceBox.Clear();
        }

        private void delAttendanceBox_MouseClick(object sender, MouseEventArgs e)
        {
            delAttendanceBox.Clear();
        }

        private void deleteStudBox_Leave(object sender, EventArgs e)
        {
            deleteStudBox.Text = "(type student Id here)";
        }

        private void delAttendanceBox_Leave(object sender, EventArgs e)
        {
            delAttendanceBox.Text = "(type Id here)";
        }

        private void confirmRevBtn_Click(object sender, EventArgs e)
        {
            //MessageBox.Show(dateTimePicker1.Text);
            string dateTimeFilter = Convert.ToDateTime(dateTimePicker1.Text).ToString("yyyy-MM-dd");
            //MessageBox.Show(dateTimeFilter.ToShortDateString());
            string statusFilter = Convert.ToString(comboBox3.SelectedItem);
            int revGradeFilter = Convert.ToInt32(comboBox1.SelectedItem);
            string revSectionFilter = (studFiltSectionReview.Text == "All") ? "" : studFiltSectionReview.SelectedItem.ToString();
            bool forExport = false;
            bool delQuery = false;
            //MessageBox.Show(statusFilter + " " + revGradeFilter + " " + revSectionFilter);
            reviewAttendance(dateTimeFilter, statusFilter, revGradeFilter, revSectionFilter, forExport, delQuery); // (str,str,int,int,bool,bool)
        }

        private void fillStatusAbsent()
        {
            string query = "SELECT Students.Id FROM `students` WHERE Students.Id NOT IN(Select Attendance.StudentId FROM Attendance WHERE Attendance.Date = '"+currDate+"')";
            
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand command = new MySqlCommand(query, connection);
            List<string> collectedId = new List<string>();
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = command.ExecuteReader();
                
                while (reader.Read())
                {
                    collectedId.Add(reader["Id"].ToString());
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            connection.Close();

            string[] arrId = collectedId.ToArray();
            foreach (var id in arrId)
            {
                string queryInsert = "INSERT INTO Attendance (StudentId,Status,Date) VALUES (@val1,@val2,@val3)";

                MySqlCommand cmd = new MySqlCommand(queryInsert, connection);
                cmd.Parameters.AddWithValue("@val1", id);
                cmd.Parameters.AddWithValue("@val2", "Absent");
                cmd.Parameters.AddWithValue("@val3", currDate);

                try
                {
                    connection.Open();
                    cmd.ExecuteNonQuery();
                }
                catch (Exception er)
                {
                    MessageBox.Show(er.ToString());
                    throw;
                }
                connection.Close();
            }
        }



    }
}
