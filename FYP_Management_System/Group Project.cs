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
    public partial class Group_Project : Form
    {
        public Group_Project()
        {
            InitializeComponent();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            cbxGroup.SelectedIndex = -1;
            cbxProject.SelectedIndex = -1;
        }

        private void Group_Project_Load(object sender, EventArgs e)
        {
            // When form Loads Fill the ComboBoxes from database tables
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd2 = new SqlCommand("Select Id FROM [Group]", con);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            sda2.Fill(dt2);
            cbxGroup.DataSource = dt2;
            cbxGroup.DisplayMember = "Id";
            cbxGroup.ValueMember = "Id";
            cbxGroup.DataSource = dt2;
            //Same for Project
            SqlCommand cmd3 = new SqlCommand("Select Id FROM Project", con);
            SqlDataAdapter sda3 = new SqlDataAdapter(cmd3);

            DataTable dt3 = new DataTable();
            sda3.Fill(dt3);
            cbxProject.DataSource = dt3;
            cbxProject.DisplayMember = "Id";
            cbxProject.ValueMember = "Id";
            cbxProject.DataSource = dt3;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            // Load Data from GroupProject Table and Project title from Project Table
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select GroupId, ProjectId, Title, AssignmentDate FROM GroupProject JOIN Project ON ProjectId = Id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            // Set Column Widths
            dataGridView1.Columns[2].Width = 200;
            dataGridView1.Columns[3].Width = 200;
        }

        private void AssignProjectButton_Click(object sender, EventArgs e)
        {
            if (cbxGroup.SelectedIndex != -1 && cbxProject.SelectedIndex != -1)
            {
                var con = Configuration.getInstance().getConnection();
                // make sure the group is not already assigned the project
                SqlCommand cmd = new SqlCommand("Select COUNT(*) FROM GroupProject WHERE GroupId = @GroupId AND ProjectId = @ProjectId", con);
                cmd.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                cmd.Parameters.AddWithValue("@ProjectId", cbxProject.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Group is already assigned this project");
                    return;
                }
                // check if the project is already assigned to amother group
                cmd = new SqlCommand("Select COUNT(*) FROM GroupProject WHERE ProjectId = @ProjectId", con);
                cmd.Parameters.AddWithValue("@ProjectId", cbxProject.SelectedValue);
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Project is already assigned to another group");
                    return;
                }
                //make sure one group is assigned only one project
                cmd = new SqlCommand("Select COUNT(*) FROM GroupProject WHERE GroupId = @GroupId", con);
                cmd.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Group is already assigned a project");
                    return;
                }
                // Assign the project to the group
                cmd = new SqlCommand("Insert INTO GroupProject(GroupId, ProjectId, AssignmentDate) VALUES(@GroupId, @ProjectId, @AssignmentDate)", con);
                cmd.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                cmd.Parameters.AddWithValue("@ProjectId", cbxProject.SelectedValue);
                cmd.Parameters.AddWithValue("@AssignmentDate", DateTime.Now);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Project Assigned to Group");
                LoadData();
            }
            else
            {
                MessageBox.Show("Please Select Group and Project");
            }
        }
    }
}
