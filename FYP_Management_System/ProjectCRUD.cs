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

namespace FYP_Management_System
{
    public partial class ProjectCRUD : Form
    {
        public ProjectCRUD()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadProjects();
        }
        private void LoadProjects()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Project", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Width = 50;
            dataGridView1.Columns[1].Width = 400;
            dataGridView1.Columns[2].Width = 200;
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            // Check that Project Title is not empty and Description is not empty
            if (txtTitle.Text == "" || txtDescription.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // Check that Project Title is unique
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) from Project where Title = @Title", con);
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                int existingProject = (int)cmd.ExecuteScalar();
                if (existingProject == 1)
                {
                    MessageBox.Show("Project already exists");
                }
                else
                {
                    SqlCommand sqlCommand = new SqlCommand("Insert into Project(Title, Description) values(@Title, @Description)", con);
                    sqlCommand.Parameters.AddWithValue("@Title", txtTitle.Text);
                    sqlCommand.Parameters.AddWithValue("@Description", txtDescription.Text);
                    sqlCommand.ExecuteNonQuery();
                    MessageBox.Show("Project Added Successfully");
                    LoadProjects();
                }
            }

        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = this.dataGridView1.Rows[e.RowIndex];
                txtId.Text = row.Cells[0].Value.ToString();
                txtTitle.Text = row.Cells[2].Value.ToString();
                txtDescription.Text = row.Cells[1].Value.ToString();
            }
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            txtDescription.Text = "";
            txtTitle.Text = "";
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            // Check that Project Id is not empty
            if (txtId.Text == "")
            {
                MessageBox.Show("Please enter Id to Update");
            }
            else if (txtTitle.Text == "" || txtDescription.Text == "")
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // Check That Given Id is correct

                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) from Project where Id = @Id", con);
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                int existingProject = (int)cmd.ExecuteScalar();
                if (existingProject == 0)
                {
                    MessageBox.Show("Project does not exists");
                }
                else
                {
                    SqlCommand sqlCommand = new SqlCommand("Update Project set Title = @Title, Description = @Description where Id = @Id", con);
                    sqlCommand.Parameters.AddWithValue("@Title", txtTitle.Text);
                    sqlCommand.Parameters.AddWithValue("@Description", txtDescription.Text);
                    sqlCommand.Parameters.AddWithValue("@Id", txtId.Text);
                    sqlCommand.ExecuteNonQuery();
                    MessageBox.Show("Project Updated Successfully");
                    LoadProjects();
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
            SqlCommand cmd = new SqlCommand("Select COUNT(*) from Project where Id = @Id", con);
            cmd.Parameters.AddWithValue("@Id", txtId.Text);
            int existingProject = (int)cmd.ExecuteScalar();
            if (existingProject == 0)
            {
                MessageBox.Show("Project does not exists");
            }
            else
            {
                SqlCommand sqlCommand = new SqlCommand("Delete from Project where Id = @Id", con);
                sqlCommand.Parameters.AddWithValue("@Id", txtId.Text);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Project Deleted Successfully");
                LoadProjects();
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
                    SqlCommand cmd = new SqlCommand("Select * FROM Project", con);

                    string value = GetTextBoxValue(searchFilter);
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (searchFilter == "Id")
                        {
                            cmd.CommandText = "Select * FROM Project P WHERE P.Id" + " = @value";
                            cmd.Parameters.AddWithValue("@value", Convert.ToInt32(value));
                        }
                        else
                        {
                            cmd.CommandText = "Select * FROM Project WHERE " + searchFilter + " = @value";
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
                    dataGridView1.Columns[0].Width = 50;
                    dataGridView1.Columns[1].Width = 400;
                    dataGridView1.Columns[2].Width = 200;

                }
            }
        }
        private string GetTextBoxValue(string columnName)
        {
            switch (columnName)
            {
                case "Id":
                    return txtId.Text;
                case "Title":
                    return txtTitle.Text;
                case "Description":
                    return txtDescription.Text;
                default:
                    return string.Empty;
            }
        }
    }
}
