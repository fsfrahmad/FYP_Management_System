using CRUD_Operations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace FYP_Management_System
{
    public partial class AdvisorCRUD : Form
    {
        public AdvisorCRUD()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            txtId.Text = "";
            txtFName.Text = "";
            txtLName.Text = "";
            txtContact.Text = "";
            txtEmail.Text = "";
            dateTimePicker1.Text = "";
            comboBox2.Text = "";
            txtSalary.Text = "";
            txtDesignation.Text = "";
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadAdvisorData();
        }
        private void LoadAdvisorData()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select A.Id, A.Designation, A.Salary, P.FirstName, P.LastName, P.Contact, P.Email, P.DateOfBirth, P.Gender from Advisor A JOIN Person P ON A.Id = P.Id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
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
                SqlCommand cmd = new SqlCommand("DELETE FROM Advisor WHERE Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Record Deleted Successfully");
                LoadAdvisorData();
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

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            txtId.Text = selectedRow.Cells[0].Value.ToString();
            txtDesignation.Text = selectedRow.Cells[1].Value.ToString();
            txtSalary.Text = selectedRow.Cells[2].Value.ToString();
            txtFName.Text = selectedRow.Cells[3].Value.ToString();
            txtLName.Text = selectedRow.Cells[4].Value.ToString();
            txtContact.Text = selectedRow.Cells[5].Value.ToString();
            txtEmail.Text = selectedRow.Cells[6].Value.ToString();
            dateTimePicker1.Text = selectedRow.Cells[7].Value.ToString();
            if (selectedRow.Cells[8].Value.ToString() == "1")
            {
                comboBox2.Text = "Male";
            }
            else
            {
                comboBox2.Text = "Female";
            }

        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            if (txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || txtDesignation.Text == "" || txtSalary.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if (ContainsNumber(txtFName.Text) || ContainsNumber(txtLName.Text))
            {
                MessageBox.Show("First Name and Last Name cannot contain numbers");
            }
            else if (ContainsAlphabet(txtContact.Text))
            {
                MessageBox.Show("Contact cannot contain alphabets");
            }
            else if (ContainsAlphabet(txtSalary.Text))
            {
                MessageBox.Show("Salary cannot contain alphabets");
            }
            else if (txtEmail.Text.EndsWith("@gmail.com") == false)
            {
                MessageBox.Show("Email must end with @gmail.com");
            }
            else if (txtContact.Text.StartsWith("-"))
            {
                MessageBox.Show("Contact cannot be negataive");
            }
            else if (txtSalary.Text.StartsWith("-"))
            {
                MessageBox.Show("Salary cannot be negative");
            }
            else
            {
                var con = Configuration.getInstance().getConnection();
                // Check if Person Already Record Already exists and is not a Student
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Person WHERE FirstName = @FirstName AND LastName = @LastName AND Contact = @Contact AND Email = @Email AND DateOfBirth = @DateOfBirth AND Gender = @Gender", con);
                checkCmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                checkCmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                checkCmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                checkCmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                checkCmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
                checkCmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                int existingRecordsCount = (int)checkCmd.ExecuteScalar();

                //Get Person Id if Record Already Exists and check based on that is not a Student
                SqlCommand getPersonId = new SqlCommand("SELECT Id FROM Person WHERE FirstName = @FirstName AND LastName = @LastName AND Contact = @Contact AND Email = @Email AND DateOfBirth = @Date AND Gender = @Gender", con);
                getPersonId.Parameters.AddWithValue("@FirstName", txtFName.Text);
                getPersonId.Parameters.AddWithValue("@LastName", txtLName.Text);
                getPersonId.Parameters.AddWithValue("@Contact", txtContact.Text);
                getPersonId.Parameters.AddWithValue("@Email", txtEmail.Text);
                getPersonId.Parameters.AddWithValue("@Date", dateTimePicker1.Text);
                getPersonId.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                object result = getPersonId.ExecuteScalar();
                int personId = result != null ? (int)result : -1; // Replace -1 with appropriate default value


                if (existingRecordsCount == 1)
                {
                    //Check if the Person is a Student
                    SqlCommand checkStudent = new SqlCommand("SELECT COUNT(*) FROM Student WHERE Id = @Id", con);
                    checkStudent.Parameters.AddWithValue("@Id", personId);
                    int studentCount = (int)checkStudent.ExecuteScalar();
                    if (studentCount > 0)
                    {
                        MessageBox.Show("This person is a student and cannot be an advisor");
                    }
                    else
                    {
                        //Add Advisor
                        SqlCommand cmd = new SqlCommand("INSERT INTO Advisor (Id, Designation, Salary) VALUES (@Id, @Designation, @Salary)", con);
                        cmd.Parameters.AddWithValue("@Id", personId);
                        if (txtDesignation.Text == "Professor")
                        {
                            cmd.Parameters.AddWithValue("@Designation", 6);
                        }
                        else if (txtDesignation.Text == "Associate Professor")
                        {
                            cmd.Parameters.AddWithValue("@Designation", 7);
                        }
                        else if (txtDesignation.Text == "Assistant Professor")
                        {
                            cmd.Parameters.AddWithValue("@Designation", 8);
                        }
                        else if (txtDesignation.Text == "Lecturer")
                        {
                            cmd.Parameters.AddWithValue("@Designation", 9);
                        }
                        else if (txtDesignation.Text == "Industry Professional")
                        {
                            cmd.Parameters.AddWithValue("@Designation", 10);
                        }
                        else
                        {
                            MessageBox.Show("Invalid Designation");
                            return;
                        }
                        cmd.Parameters.AddWithValue("@Salary", txtSalary.Text);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    //Add Person
                    SqlCommand cmd = new SqlCommand("INSERT INTO Person (FirstName, LastName, Contact, Email, DateOfBirth, Gender) VALUES (@FirstName, @LastName, @Contact, @Email, @Date, @Gender)", con);
                    cmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                    cmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                    cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                    cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@Date", dateTimePicker1.Text);
                    cmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                    cmd.ExecuteNonQuery();

                    //Get Person Id
                    SqlCommand countCmd = new SqlCommand("SELECT COUNT(*) FROM Person", con);
                    int PersonId = (int)countCmd.ExecuteScalar();

                    // Add Advisor
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO Advisor (Id, Designation, Salary) VALUES (@Id, @Designation, @Salary)", con);
                    cmd2.Parameters.AddWithValue("@Id", PersonId);
                    if (txtDesignation.Text == "Professor")
                    {
                        cmd2.Parameters.AddWithValue("@Designation", 6);
                    }
                    else if (txtDesignation.Text == "Associate Professor")
                    {
                        cmd2.Parameters.AddWithValue("@Designation", 7);
                    }
                    else if (txtDesignation.Text == "Assistant Professor")
                    {
                        cmd2.Parameters.AddWithValue("@Designation", 8);
                    }
                    else if (txtDesignation.Text == "Lecturer")
                    {
                        cmd2.Parameters.AddWithValue("@Designation", 9);
                    }
                    else if (txtDesignation.Text == "Industry Professional")
                    {
                        cmd2.Parameters.AddWithValue("@Designation", 10);
                    }
                    else
                    {
                        MessageBox.Show("Invalid Designation");
                        return;
                    }
                    cmd2.Parameters.AddWithValue("@Salary", txtSalary.Text);
                    cmd2.ExecuteNonQuery();
                }
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || txtDesignation.Text == "" || txtSalary.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if (ContainsNumber(txtFName.Text) || ContainsNumber(txtLName.Text))
            {
                MessageBox.Show("First Name and Last Name cannot contain numbers");
            }
            else if (ContainsAlphabet(txtContact.Text))
            {
                MessageBox.Show("Contact cannot contain alphabets");
            }
            else if (ContainsAlphabet(txtSalary.Text))
            {
                MessageBox.Show("Salary cannot contain alphabets");
            }
            else if (txtEmail.Text.EndsWith("@gmail.com") == false)
            {
                MessageBox.Show("Email must end with @gmail.com");
            }
            else if (txtContact.Text.StartsWith("-"))
            {
                MessageBox.Show("Contact cannot be negataive");
            }
            else if (txtSalary.Text.StartsWith("-"))
            {
                MessageBox.Show("Salary cannot be negative");
            }
            else
            {
                // Update Person Data and Advisor Data Based On Id
                var con = Configuration.getInstance().getConnection();
                SqlCommand checkId = new SqlCommand("SELECT COUNT(*) FROM Person WHERE Id = @Id", con);
                checkId.Parameters.AddWithValue("@Id", txtId.Text);
                object result = checkId.ExecuteScalar();
                int personId = result != null ? (int)result : -1; // Replace -1 with appropriate default value

                if (personId == -1 || personId == 0)
                {
                    MessageBox.Show("No record found with this Id");
                    return;
                }

                SqlCommand sqlCommand = new SqlCommand("UPDATE Person SET FirstName = @FirstName, LastName = @LastName, Contact = @Contact, Email = @Email, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE Id = @Id", con);
                sqlCommand.Parameters.AddWithValue("@Id", txtId.Text);
                sqlCommand.Parameters.AddWithValue("@FirstName", txtFName.Text);
                sqlCommand.Parameters.AddWithValue("@LastName", txtLName.Text);
                sqlCommand.Parameters.AddWithValue("@Contact", txtContact.Text);
                sqlCommand.Parameters.AddWithValue("@Email", txtEmail.Text);
                sqlCommand.Parameters.AddWithValue("@DateOfBirth", dateTimePicker1.Text);
                sqlCommand.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);
                sqlCommand.ExecuteNonQuery();

                // Check if Advisor Exists

                SqlCommand checkAdvisor = new SqlCommand("SELECT COUNT(*) FROM Advisor WHERE Id = @Id", con);
                checkAdvisor.Parameters.AddWithValue("@Id", txtId.Text);
                object result2 = checkAdvisor.ExecuteScalar();
                int advisorCount = result2 != null ? (int)result2 : -1; // Replace -1 with appropriate default value
                if (advisorCount == -1 || advisorCount == 0)
                {
                    MessageBox.Show("No record found with this Id");
                }


                SqlCommand sqlCommand2 = new SqlCommand("UPDATE Advisor SET Designation = @Designation, Salary = @Salary WHERE Id = @Id", con);
                if (txtDesignation.Text == "Professor")
                {
                    sqlCommand2.Parameters.AddWithValue("@Designation", 6);
                }
                else if (txtDesignation.Text == "Associate Professor")
                {
                    sqlCommand2.Parameters.AddWithValue("@Designation", 7);
                }
                else if (txtDesignation.Text == "Assistant Professor")
                {
                    sqlCommand2.Parameters.AddWithValue("@Designation", 8);
                }
                else if (txtDesignation.Text == "Lecturer")
                {
                    sqlCommand2.Parameters.AddWithValue("@Designation", 9);
                }
                else if (txtDesignation.Text == "Industry Professional")
                {
                    sqlCommand2.Parameters.AddWithValue("@Designation", 10);
                }
                else
                {
                    MessageBox.Show("Invalid Designation");
                    return;
                }
                sqlCommand2.Parameters.AddWithValue("@Salary", txtSalary.Text);
                sqlCommand2.ExecuteNonQuery();
                MessageBox.Show("Record Updated Successfully");

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
                    SqlCommand cmd = new SqlCommand("Select A.Id, A.Designation, A.Salary, P.FirstName, P.LastName, P.Contact, P.Email, P.DateOfBirth, P.Gender from Advisor A JOIN Person P ON A.Id = P.Id", con);


                    string value = GetTextBoxValue(searchFilter);
                    if (value == null || value == "")
                    {
                        MessageBox.Show("Please enter a valid value");
                    }
                    else
                    {
                        if (searchFilter == "Id")
                        {
                            cmd.CommandText += " WHERE A.Id = @value";
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
                        else if (searchFilter == "Designation")
                        {
                            if (value == "Professor")
                            {
                                value = "6";
                            }
                            else if (value == "Associate Professor")
                            {
                                value = "7";
                            }
                            else if (value == "Assistant Professor")
                            {
                                value = "8";
                            }
                            else if (value == "Lecturer")
                            {
                                value = "9";
                            }
                            else if (value == "Industry Professional")
                            {
                                value = "10";
                            }
                            cmd.CommandText += " WHERE " + searchFilter + " = @value";
                        }
                        else
                        {
                            cmd.CommandText += " WHERE " + searchFilter + " = @value";
                        }
                    }
                    cmd.Parameters.AddWithValue("@value", value);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dataGridView1.DataSource = dt;
                }
            }
        }
        private string GetTextBoxValue(string columnName)
        {
            switch (columnName)
            {
                case "Id":
                    return txtId.Text;
                case "Designation":
                    return txtDesignation.Text;
                case "Salary":
                    return txtSalary.Text;
                case "FirstName":
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

