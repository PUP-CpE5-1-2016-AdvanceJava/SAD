using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Student_Attendance_Final
{
    public partial class StudentProfile : Form
    {
        //for database connection
        AttendanceControl ac = new AttendanceControl();
        string ip = "";
        string connectionString = "";
        long currentStudId = 0;
        public StudentProfile()
        {
            InitializeComponent();
        }

        public void setFname(string Fname)
        {
            textBox1.Text = Fname;
        }
        public void setMname(string Mname)
        {
            textBox2.Text = Mname;
        }
        public void setSname(string Sname)
        {
            textBox3.Text = Sname;
        }
        public void setGrade(string grade)
        {
            textBox4.Text = grade;
        }
        public void setSection(string section)
        {
            textBox5.Text = section;
        }

        public void setId(string id)
        {
            textBox6.Text = id;
        }

        public string getFname()
        {
            return textBox1.Text;
        }

        public string getMname()
        {
            return textBox2.Text;
        }

        public string getSname()
        {
            return textBox3.Text;
        }

        public string getGrade()
        {
            return textBox4.Text;
        }

        public string getSection()
        {
            return textBox5.Text;
        }

        public string getId()
        {
            return textBox6.Text;
        }

        public void enableEdit()
        {
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;
            textBox4.ReadOnly = false;
            textBox5.ReadOnly = false;
            textBox6.ReadOnly = false;
        }

        public void disableEdit()
        {
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            textBox4.ReadOnly = true;
            textBox5.ReadOnly = true;
            textBox6.ReadOnly = true;
        }

        public void studProfEdit(object sender, EventArgs e)
        {
            enableEdit();
        }

        public void studProfEditSubmit(object sender, EventArgs e)
        {
            Int64 id = Convert.ToInt64(getId());

            if (editStudents(id, currentStudId) == true)
            {
                MessageBox.Show("Edit Success");
                disableEdit();
            }
        }

        public void setIp(string ipAddress)
        {
            ip = ipAddress;
            connectionString = "SERVER = " + ip + "; PORT = 3306; DATABASE = ija_student_attendance; UID = root; password = 99083915;";
        }

        public string getIp()
        {
            return ip;
        }

        private bool editStudents(long StudId, long currentStudCode)
        {
            int validator = 1;
            if (!(int.TryParse(getGrade().ToString(), out validator)))
            {
                MessageBox.Show("Invalid input for grade. Please check your inputs.");
                return false;
            }

            string fname = getFname();
            string mname = getMname();
            string sname = getSname();
            int grade = Convert.ToInt32(getGrade().ToString());
            string section = getSection();

            string query = "SELECT * FROM Students WHERE Id = " + StudId;
            MySqlConnection connection = new MySqlConnection(connectionString);
            MySqlCommand cmdcheck = new MySqlCommand(query, connection);
            try
            {
                connection.Open();
                MySqlDataReader reader = null;
                reader = cmdcheck.ExecuteReader();

                if (reader.HasRows)
                {
                    connection.Close();
                    string cmd = "UPDATE Students SET Fname = '" + fname + "',Mname = '" + mname + "',Sname = '" + sname + "', Grade = " + grade + ", Section = '" + section + "' WHERE Id = " + StudId;
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
                else
                {
                    connection.Close();
                    DialogResult result = MessageBox.Show("Do you really want to edit the student code \"" + currentStudCode + "\"?", "This will delete all records of this student and register a new one", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        string cmd = "INSERT INTO Students(Id,Fname,Mname,Sname,Grade,Section) VALUES (@val1,@val2,@val3,@val4,@val5,@val6)";
                        MySqlCommand command = new MySqlCommand(cmd, connection);
                        command.Parameters.AddWithValue("@val1", StudId);
                        command.Parameters.AddWithValue("@val2", fname);
                        command.Parameters.AddWithValue("@val3", mname);
                        command.Parameters.AddWithValue("@val4", sname);
                        command.Parameters.AddWithValue("@val5", grade);
                        command.Parameters.AddWithValue("@val6", section);
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

                        deleteStudentById(currentStudCode);
                    }
                    else return false;
                }
            }
            catch (Exception er)
            {
                MessageBox.Show(er.ToString());
                throw;
            }
            return true;
        }

        private void deleteStudentById(long code)
        {
            string cmd = "DELETE FROM Students WHERE Id = " + code;

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
        }

        private void StudentProfile_Load(object sender, EventArgs e)
        {
            currentStudId = Convert.ToInt64(textBox6.Text);
        }
    }
}
