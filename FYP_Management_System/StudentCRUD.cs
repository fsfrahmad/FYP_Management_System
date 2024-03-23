using CRUD_Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace FYP_Management_System
{
    public partial class StudentCRUD : Form
    {
        public StudentCRUD()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            txtId.Text = selectedRow.Cells[0].Value.ToString();
            txtRegNo.Text = selectedRow.Cells[1].Value.ToString();
            txtFName.Text = selectedRow.Cells[2].Value.ToString();
            txtLName.Text = selectedRow.Cells[3].Value.ToString();
            txtContact.Text = selectedRow.Cells[4].Value.ToString();
            txtEmail.Text = selectedRow.Cells[5].Value.ToString();
            dateTimePicker1.Text = selectedRow.Cells[6].Value.ToString();
            if (selectedRow.Cells[7].Value.ToString() == "1")
            {
                comboBox2.Text = "Male";
            }
            else
            {
                comboBox2.Text = "Female";
            }
        }
        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadStudentData();
        }
        private void LoadStudentData()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select S.Id,S.RegistrationNo, P.FirstName, P.LastName, P.Contact, P.Email, P.DateOfBirth, P.Gender from Student S JOIN Person P ON S.Id = P.Id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void StudentCRUD_Load(object sender, EventArgs e)
        {
            LoadStudentData();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || txtRegNo.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if (ContainsNumber(txtFName.Text))
            {
                MessageBox.Show("First name should not contain numbers");
                return;
            }
            else if (ContainsNumber(txtLName.Text))
            {
                MessageBox.Show("Last name should not contain numbers");
                return;
            }
            else if (ContainsAlphabet(txtContact.Text))
            {
                MessageBox.Show("Contact should not contain alphabets");
                return;
            }
            else if(txtContact.Text.StartsWith("-"))
            {
                MessageBox.Show("Contact should not be -ve");
            }
            else if (!txtEmail.Text.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email should end with @gmail.com");
                return;
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                
                // Check if the Person already exists and is not a advisor
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Person WHERE Contact = @Contact AND Email = @Email", con);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                int exisngPersonCount = (int)cmd.ExecuteScalar();
                // Check if Student with same Registration No already exists
                SqlCommand cmd9 = new SqlCommand("SELECT COUNT(*) FROM Student WHERE RegistrationNo = @RegistrationNo", con);
                cmd9.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                int existingStudentCount2 = (int)cmd9.ExecuteScalar();
                if (existingStudentCount2 == 1)
                {
                    MessageBox.Show("Student already exists");
                    return;
                }
                //Get Person Id if Record Already Exists and check based on that is not a Advisor
                SqlCommand cmd2 = new SqlCommand("SELECT Id FROM Person WHERE Contact = @Contact AND Email = @Email", con);
                cmd2.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd2.Parameters.AddWithValue("@Email", txtEmail.Text);
                object result = cmd2.ExecuteScalar();
                int personId = result != null ? (int)result : -1; // Replace -1 with appropriate default value
                if (exisngPersonCount == 1)
                {
                    SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM Student WHERE Id = @Id", con);
                    cmd3.Parameters.AddWithValue("@Id", personId);
                    int existingStudentCount = (int)cmd3.ExecuteScalar();
                    if (existingStudentCount == 1)
                    {
                        MessageBox.Show("Student already exists");
                        return;
                    }
                    else
                    {
                        // Check if the person is a Advisor
                        SqlCommand cmd4 = new SqlCommand("SELECT COUNT(*) FROM Advisor WHERE Id = @Id", con);
                        cmd4.Parameters.AddWithValue("@Id", personId);
                        int existingAdvisorCount = (int)cmd4.ExecuteScalar();
                        if (existingAdvisorCount == 1)
                        {
                            MessageBox.Show("This person is a advisor and cannot be an student");
                            return;
                        }
                        else
                        {
                            SqlCommand cmd5 = new SqlCommand("INSERT INTO Student(Id, RegistrationNo) VALUES(@Id, @RegistrationNo)", con);
                            cmd5.Parameters.AddWithValue("@Id", personId);
                            cmd5.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                            cmd5.ExecuteNonQuery();
                            MessageBox.Show("Record Added Successfully");
                            LoadStudentData();
                        }
                    }
                }
                else
                {
                    SqlCommand cmd6 = new SqlCommand("INSERT INTO Person(FirstName, LastName, Contact, Email, DateOfBirth, Gender) VALUES (@FirstName, @LastName, @Contact, @Email, @DateOfBirth, @Gender)", con);
                    cmd6.Parameters.AddWithValue("@FirstName", txtFName.Text);
                    cmd6.Parameters.AddWithValue("@LastName", txtLName.Text);
                    cmd6.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd6.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd6.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Text);
                    cmd6.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);

                    cmd6.ExecuteNonQuery();

                    SqlCommand cmd7 = new SqlCommand("SELECT Id FROM Person WHERE Contact = @Contact AND Email = @Email", con);
                    cmd7.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd7.Parameters.AddWithValue("@Email", txtEmail.Text);
                    object result2 = cmd7.ExecuteScalar();
                    int personId2 = result2 != null ? (int)result2 : -1; // Replace -1 with appropriate default value

                    // Check if Student with same Registration No already exists

                    SqlCommand cmd8 = new SqlCommand("INSERT INTO Student(Id, RegistrationNo) VALUES(@Id, @RegistrationNo)", con);
                    cmd8.Parameters.AddWithValue("@Id", personId2);
                    cmd8.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                    cmd8.ExecuteNonQuery();
                    MessageBox.Show("Record Added Successfully");
                    LoadStudentData();
                }
                
            }
        }
        private bool ContainsNumber(string input)
        {
            return input.Any(char.IsDigit);
        }

        private bool ContainsAlphabet(string input)
        {
            return input.Any(char.IsLetter);
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearData();
        }
        private void ClearData()
        {
            txtId.Text = "";
            txtFName.Text = "";
            txtLName.Text = "";
            txtContact.Text = "";
            txtEmail.Text = "";
            txtRegNo.Text = "";
            comboBox2.Text = "";
            dateTimePicker1.Text = "";
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Please select a record to update");
            }
            else if(txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || txtRegNo.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if (ContainsNumber(txtFName.Text))
            {
                MessageBox.Show("First name should not contain numbers");
                return;
            }
            else if (ContainsNumber(txtLName.Text))
            {
                MessageBox.Show("Last name should not contain numbers");
                return;
            }
            else if (ContainsAlphabet(txtContact.Text))
            {
                MessageBox.Show("Contact should not contain alphabets");
                return;
            }
            else if (txtContact.Text.StartsWith("-"))
            {
                MessageBox.Show("Contact should not be -ve");
            }
            else if (!txtEmail.Text.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Email should end with @gmail.com");
                return;
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                // Check if the updated data does not already exists in person table

                SqlCommand cmd10 = new SqlCommand("SELECT COUNT(*) FROM Person WHERE Contact = @Contact AND Email = @Email AND Id != @Id", con);
                cmd10.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd10.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd10.Parameters.AddWithValue("@Id", txtId.Text);
                int exisngPersonCount = (int)cmd10.ExecuteScalar();
                if (exisngPersonCount == 1)
                {
                    MessageBox.Show("Person already exists");
                    return;
                }
                SqlCommand cmd = new SqlCommand("UPDATE Person SET FirstName = @FirstName, LastName = @LastName, Contact = @Contact, Email = @Email, DateOfBirth= @DateOfBirth, Gender = @Gender WHERE Id=@Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Text);
                cmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                cmd.ExecuteNonQuery();
                //Same for Student Table Check updated Record does not already exists
                SqlCommand cmd11 = new SqlCommand("SELECT COUNT(*) FROM Student WHERE RegistrationNo = @RegistrationNo AND Id != @Id", con);
                cmd11.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                cmd11.Parameters.AddWithValue("@Id", txtId.Text);
                int existingStudentCount = (int)cmd11.ExecuteScalar();
                if (existingStudentCount == 1)
                {
                    MessageBox.Show("Student already exists");
                    return;
                }


                SqlCommand cmd2 = new SqlCommand("UPDATE Student SET RegistrationNo = @RegistrationNo WHERE Id=@Id", con);
                cmd2.Parameters.AddWithValue("@Id", txtId.Text);
                cmd2.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                cmd2.ExecuteNonQuery();
                LoadStudentData();
                MessageBox.Show("Record Updated Successfully");
                ClearData();
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Please select a record to delete");
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("DELETE FROM Student WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.ExecuteNonQuery();
                LoadStudentData();
                MessageBox.Show("Record Deleted Successfully");
                ClearData();
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a search filter");
            }
            else
            {
                string searchFilter = comboBox1.SelectedItem.ToString();
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    var con = Configuration.getInstance().getConnection();
                    SqlCommand cmd = new SqlCommand("Select S.Id,S.RegistrationNo, P.FirstName, P.LastName, P.Contact, P.Email, P.DateOfBirth, P.Gender from Student S JOIN Person P ON S.Id = P.Id", con);
                    string value = GetTextBoxValue(searchFilter);
                    if (value == null || value == "")
                    {
                        MessageBox.Show("Please enter a valid value");
                    }
                    else
                    {
                        if (searchFilter == "Id")
                        {

                            cmd.CommandText += " WHERE S.Id = @value";
                        }
                        else if (searchFilter == "Gender")
                        {
                            if (value == "Male")
                            {
                                value = "1";
                            }
                            else
                            {
                                value = "2";
                            }
                            cmd.CommandText += " WHERE " + searchFilter + " = @value";
                        }
                        else
                        {
                            cmd.CommandText += " WHERE " + searchFilter + " = @value";
                        }
                        cmd.Parameters.AddWithValue("@value", value);
                        SqlDataAdapter sda = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        dataGridView1.DataSource = dt;

                    }

                }
            }
        }
        private string GetTextBoxValue(string columnName)
        {
            switch (columnName)
            {
                case "Id":
                    return txtId.Text;
                case "RegistrationNo":
                    return txtRegNo.Text;
                case "FullName":
                    return txtFName.Text;
                case "LastName":
                    return txtLName.Text;
                case "Contact":
                    return txtContact.Text;
                case "Email":
                    return txtEmail.Text;
                case "DateOfBirth":
                    return dateTimePicker1.Text;
                case "Gender":
                    return comboBox2.Text;
                default:
                    return string.Empty;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            MainForm mainForm = new MainForm();
            mainForm.Show();

        }
    }
}
