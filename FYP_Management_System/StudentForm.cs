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
    public partial class StudentForm : Form
    {
        public StudentForm()
        {
            InitializeComponent();
            LoadPersonData();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            DataGridViewRow selectedRow = dataGridView1.Rows[e.RowIndex];
            txtId.Text = selectedRow.Cells[0].Value.ToString();
        }
        private void LoadPersonData()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select * from Person", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
        }
        private void LoadStudentData()
        {           
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select S.Id,S.RegistrationNo, P.FirstName,P.LastName, P.Contact, P.Email, P.DateOfBirth, P.Gender from Student S JOIN Person P ON S.Id = P.Id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView2.DataSource = dt;
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadStudentData();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Insert into Student values (@Id, @RegistrationNo)", con);
            if (txtId.Text == "" || txtRegNo.Text == "")
            {
                MessageBox.Show("No Input Field can be empty!!!");
            }
            else
            {
                cmd.Parameters.AddWithValue("@Id", txtId.Text);
                cmd.Parameters.AddWithValue("@RegistrationNo", txtRegNo.Text);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Successfully saved");
                LoadStudentData();
            }
        }
    }
}
