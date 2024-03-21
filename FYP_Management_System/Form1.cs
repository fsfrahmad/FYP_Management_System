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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace FYP_Management_System
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();

            // Check if the record already exists
            SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Person WHERE FirstName=@FirstName AND LastName=@LastName AND Contact=@Contact AND Email=@Email AND DateOfBirth=@DateOfBirth AND Gender=@Gender", con);
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

            // If the record doesn't exist, proceed with insertion
            SqlCommand cmd = new SqlCommand("INSERT INTO Person VALUES (@FirstName, @LastName, @Contact, @Email, @DateOfBirth, @Gender)", con);
            cmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
            cmd.Parameters.AddWithValue("@LastName", txtLName.Text);
            cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
            cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
            cmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
            cmd.Parameters.AddWithValue("@Gender", comboBox2.Text == "Male" ? 1 : 2);

            cmd.ExecuteNonQuery();
            MessageBox.Show("Successfully saved");
        }

        private void LoadData()
        { 
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Person", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
        }


        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            txtId.Text = selectedRow.Cells[0].Value.ToString(); 
            txtFName.Text = selectedRow.Cells[1].Value.ToString();
            txtLName.Text = selectedRow.Cells[2].Value.ToString();
            txtContact.Text = selectedRow.Cells[3].Value.ToString();
            txtEmail.Text = selectedRow.Cells[4].Value.ToString();
            dateTimePicker1.Text = selectedRow.Cells[5].Value.ToString();
            if (selectedRow.Cells[6].Value.ToString() == "1")
            {
                comboBox2.Text = "Male";
            }
            else
            {
                comboBox2.Text = "Female";
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Update Person set FirstName=@FirstName, LastName=@LastName, Contact=@Contact, Email=@Email, DateOfBirth=@DateOfBirth, Gender=@Gender WHERE Id = @Id" , con);
            if (txtFName.Text == "" || txtLName.Text == "" || txtContact.Text == "" || txtEmail.Text == "" || comboBox2.Text == "")
            {
                MessageBox.Show("No Input Field can be empty!!!");
            }
            else
            {
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@FirstName", txtFName.Text);
                cmd.Parameters.AddWithValue("@LastName", txtLName.Text);
                cmd.Parameters.AddWithValue("@Contact", txtContact.Text);
                cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                cmd.Parameters.AddWithValue("@DateofBirth", dateTimePicker1.Text);
                if (comboBox2.Text == "Male")
                {
                    cmd.Parameters.AddWithValue("@Gender", 1);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Gender", 2);
                }
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully Updated");
            }
        }
    }
}
