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

namespace FYP_Management_System
{
    public partial class EvaluationCRUD : Form
    {
        public EvaluationCRUD()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadEvaluation();
        }
        private void LoadEvaluation()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Evaluation", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 150;
            dataGridView1.Columns[1].Width = 200;
            dataGridView1.Columns[2].Width = 150;
            dataGridView1.Columns[3].Width = 150;
        }
        private bool ContainsAlphabet(string input)
        {
            return input.Any(char.IsLetter);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Check that Evaluation Title is not empty and Description is not empty
            if (txtName.Text == "" || txtTM.Text == "" || txtTW.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if(ContainsAlphabet(txtTM.Text) == true || ContainsAlphabet(txtTM.Text) == true)
            {
                MessageBox.Show("Please enter valid values for Total Marks and Total Weightage");
            }
            else
            {
                // Check that Evaluation Name is unique
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) from Evaluation where Name = @Name", con);
                cmd.Parameters.AddWithValue("@Name", txtName.Text);
                int existingEvaluation = (int)cmd.ExecuteScalar();
                if (existingEvaluation == 1)
                {
                    MessageBox.Show("Evaluation already exists");
                }
                else
                {
                    SqlCommand sqlCommand = new SqlCommand("Insert into Evaluation(Name, TotalMarks, TotalWeightage) values(@Name, @TotalMarks, @TotalWeightage)", con);
                    sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
                    sqlCommand.Parameters.AddWithValue("@TotalMarks", txtTM.Text);
                    sqlCommand.Parameters.AddWithValue("@TotalWeightage", txtTW.Text);
                    sqlCommand.ExecuteNonQuery();
                    MessageBox.Show("Evaluation Added Successfully");
                    LoadEvaluation();
                }
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells[0].Value.ToString();
                txtName.Text = row.Cells[1].Value.ToString();
                txtTM.Text = row.Cells[2].Value.ToString();
                txtTW.Text = row.Cells[3].Value.ToString();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            txtName.Text = "";
            txtTM.Text = "";
            txtTW.Text = "";
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Please enter Id to Update");
            }
            else if (txtName.Text == "" || txtTM.Text == "" || txtTW.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else if (ContainsAlphabet(txtTM.Text) == true || ContainsAlphabet(txtTM.Text) == true)
            {
                MessageBox.Show("Please enter valid values for Total Marks and Total Weightage");
            }
            else
            {
                // Check That Given Id is correct

                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) from Evaluation where Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                int existingEvaluation = (int)cmd.ExecuteScalar();
                if (existingEvaluation == 0)
                {
                    MessageBox.Show("Evaluation does not exists");
                }
                else
                {
                    SqlCommand sqlCommand = new SqlCommand("Update Evaluation set Name = @Name, TotalMarks = @TotalMarks, TotalWeightage = @TotalWeightage where Id = @Id", con);
                    sqlCommand.Parameters.AddWithValue("@Name", txtName.Text);
                    sqlCommand.Parameters.AddWithValue("@TotalMarks", txtTM.Text);
                    sqlCommand.Parameters.AddWithValue("@TotalWeightage", txtTW.Text);
                    sqlCommand.Parameters.AddWithValue("@Id", txtId.Text);
                    sqlCommand.ExecuteNonQuery();
                    MessageBox.Show("Evaluation Updated Successfully");
                    LoadEvaluation();
                }
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (txtId.Text == "")
            {
                MessageBox.Show("Please enter Id to Delete");
                return;
            }
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select COUNT(*) from Evaluation where Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", txtId.Text);
            int existingEvaluation = (int)cmd.ExecuteScalar();
            if (existingEvaluation == 0)
            {
                MessageBox.Show("Evaluation does not exists");
            }
            else
            {
                SqlCommand sqlCommand = new SqlCommand("Delete from Evaluation where Id = @Id", con);
                sqlCommand.Parameters.AddWithValue("@Id", txtId.Text);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Evaluation Deleted Successfully");
                LoadEvaluation();
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
                    SqlCommand cmd = new SqlCommand("Select * FROM Evaluation", con);

                    string value = GetTextBoxValue(searchFilter);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (searchFilter == "Id")
                        {
                            cmd.CommandText = "Select * FROM Evaluation E WHERE E.Id" + " = @value";
                            cmd.Parameters.AddWithValue("@value", Convert.ToInt32(value));
                        }
                        else
                        {
                            cmd.CommandText = "Select * FROM Evaluation WHERE " + searchFilter + " = @value";
                            cmd.Parameters.AddWithValue("@value", value);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid value");
                        return;
                    }
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    dataGridView1.DataSource = dt;
                    dataGridView1.Columns[0].Width = 150;
                    dataGridView1.Columns[1].Width = 200;
                    dataGridView1.Columns[2].Width = 150;
                    dataGridView1.Columns[3].Width = 150;
                    LoadEvaluation();
                }
            }
        }
        private string GetTextBoxValue(string columnName)
        {
            switch (columnName)
            {
                case "Id":
                    return txtId.Text;
                case "Name":
                    return txtName.Text;
                case "TotalMarks":
                    return txtTM.Text;
                case "TotalWeightage":
                    return txtTW.Text;
                default:
                    return string.Empty;
            }
        }
    }
}
