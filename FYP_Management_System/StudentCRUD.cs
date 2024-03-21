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
            if (txtRegNo.Text == "" || txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {

                var con = Configuration.getInstance().getConnection();
                // Check if the Student record already exists
                SqlCommand checkRegNoCmd = new SqlCommand("SELECT COUNT(*) FROM Student WHERE RegistrationNo = @RegistrationNo", con);
                checkRegNoCmd.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                int existingRegNoCount = (int)checkRegNoCmd.ExecuteScalar();

                if (existingRegNoCount > 0)
                {
                    MessageBox.Show("Registration number already exists!");
                    return;
                }
                // Check if the Person record already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Person WHERE FirstName = @FirstName AND LastName = @LastName AND Contact = @Contact AND Email = @Email AND DateOfBirth = @DateOfBirth AND Gender = @Gender", con);

                checkCmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                checkCmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                checkCmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                checkCmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
                checkCmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                int existingRecordsCount = (int)checkCmd.ExecuteScalar();
                if (existingRecordsCount > 0)
                {
                    MessageBox.Show("Record already exists!");
                    return;
                }

                else
                {
                    // Validation for FirstName: Should not contain numbers
                    string firstName = txtFName.Text;
                    if (ContainsNumber(firstName))
                    {
                        MessageBox.Show("First name should not contain numbers");
                        return;
                    }
                    // Validation for LastName: Should not contain numbers
                    string lastName = txtLName.Text;
                    if (ContainsNumber(lastName))
                    {
                        MessageBox.Show("Last name should not contain numbers");
                        return;
                    }

                    // Validation for Contact: Should not contain alphabets
                    string contact = txtContact.Text;
                    if (ContainsAlphabet(contact))
                    {
                        MessageBox.Show("Contact should not contain alphabets");
                        return;
                    }

                    // Validation for Email: Should end with @gmail.com
                    string email = txtEmail.Text;
                    if (!email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Email should end with @gmail.com");
                        return;
                    }


                    SqlCommand cmd = new SqlCommand("INSERT INTO Person VALUES (@FirstName, @LastName, @Contact, @Email, @DateOfBirth, @Gender)", con);
                    cmd.Parameters.AddWithValue("@FirstName", firstName);
                    cmd.Parameters.AddWithValue("@LastName", lastName);
                    cmd.Parameters.AddWithValue("@Contact", contact);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
                    cmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                    cmd.ExecuteNonQuery();

                    SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM Person", con);
                    int PersonId = (int)countCmd.ExecuteScalar();


                    SqlCommand cmd2 = new SqlCommand("INSERT INTO Student VALUES (@Id, @RegistrationNo)", con);
                    cmd2.Parameters.AddWithValue("@Id", PersonId);
                    cmd2.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                    cmd2.ExecuteNonQuery();
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
            else if (txtRegNo.Text == "" || txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                if (ContainsNumber(txtFName.Text))
                {
                    MessageBox.Show("First name should not contain numbers");
                    return;
                }
                if (ContainsNumber(txtLName.Text))
                {
                    MessageBox.Show("Last name should not contain numbers");
                    return;
                }
                if (ContainsAlphabet(txtContact.Text))
                {
                    MessageBox.Show("Contact should not contain alphabets");
                    return;
                }
                if (!txtEmail.Text.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Email should end with @gmail.com");
                    return;
                }

                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("UPDATE Person SET FirstName = @FirstName, LastName = @LastName, Contact = @Contact, Email = @Email, DateOfBirth= @DateOfBirth WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
                cmd.ExecuteNonQuery();

                SqlCommand cmd2 = new SqlCommand("UPDATE Student SET RegistrationNo = @RegistrationNo WHERE Id = @Id", con);
                cmd2.Parameters.AddWithValue("@Id", txtId.Text);
                cmd2.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                cmd2.ExecuteNonQuery();
                MessageBox.Show("Record Updated Successfully");
                LoadStudentData();
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
                        else if(searchFilter == "Gender")
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
    }
}
