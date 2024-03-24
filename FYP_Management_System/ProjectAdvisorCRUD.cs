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

namespace FYP_Management_System
{
    public partial class ProjectAdvisorCRUD : Form
    {
        public ProjectAdvisorCRUD()
        {
            InitializeComponent();
            LoadComboBoxItemsFromDatabaseTable();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadProjectAdvisorData();
        }
        private void LoadProjectAdvisorData()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT * FROM ProjectAdvisor", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
            // Set width size of columns
            dataGridView1.Columns[0].Width = 140;
            dataGridView1.Columns[1].Width = 140;
            dataGridView1.Columns[2].Width = 140;
            dataGridView1.Columns[3].Width = 250;
        }
        private void LoadComboBoxItemsFromDatabaseTable()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select Id FROM Advisor", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cbxAId.DataSource = dt;
            cbxAId.DisplayMember = "Id";
            cbxAId.ValueMember = "Id";
            cbxAId.DataSource = dt;
            // Same for Project
            SqlCommand cmd2 = new SqlCommand("Select Id FROM [Project]", con);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            sda2.Fill(dt2);
            cbxProject.DataSource = dt2;
            cbxProject.DisplayMember = "Id";
            cbxProject.ValueMember = "Id";
            cbxProject.DataSource = dt2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            cbxAId.SelectedIndex = 0;
            cbxProject.SelectedIndex = 0;
            cbxRole.SelectedIndex = 0;
        }

        private void AddIntoExistingButton_Click(object sender, EventArgs e)
        {
            if (cbxAId.Items.Count == 0 || cbxProject.Items.Count == 0 || cbxRole.Items.Count == 0)
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // check if the project advisor already exists
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId AND ProjectId = @ProjectId", con);
                cmd.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
                cmd.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count = (int)cmd.ExecuteScalar();
                if (count > 0)
                {
                    MessageBox.Show("Project Advisor already has a project");
                    return;
                }
                // Each Project can have one Main Advisor, one Co-Advisor and one Industry Advisor
                // Check if the project already has a Main Advisor
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 11", con);
                cmd2.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count2 = (int)cmd2.ExecuteScalar();
                if (count2 > 0 && cbxRole.Text == "Main Advisor")
                {
                    MessageBox.Show("Project already has a Main Advisor");
                    return;
                }
                // Check if the project already has a Co-Advisor
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 13", con);
                cmd3.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count3 = (int)cmd3.ExecuteScalar();
                if (count3 > 0 && cbxRole.Text == "Co-Advisror")
                {
                    MessageBox.Show("Project already has a Co-Advisor");
                    return;
                }
                // Check if the project already has a Industry Advisor
                SqlCommand cmd5 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 14", con);
                cmd5.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count4 = (int)cmd5.ExecuteScalar();
                if (count4 > 0 && cbxRole.Text == "Industry Advisor")
                {
                    MessageBox.Show("Project already has a Industry Advisor");
                    return;
                }

                // check if Project advisor is advisor of another project
                SqlCommand cmd6 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId", con);
                cmd6.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
                int count5 = (int)cmd6.ExecuteScalar();
                if (count5 > 0)
                {
                    MessageBox.Show("Advisor is already assigned to a project");
                    return;
                }



                // Insert the project advisor
                SqlCommand cmd4 = new SqlCommand("INSERT INTO ProjectAdvisor(AdvisorId, ProjectId, AdvisorRole, AssignmentDate) VALUES(@AdvisorId, @ProjectId, @AdvisorRole, @AssignmentDate)", con);
                cmd4.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
                cmd4.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                // if the role is Main Advisor then assign  11, Co - Advisror then 12 , Industry Advisor then 14
                if (cbxRole.Text == "Main Advisor")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 11);
                }
                else if (cbxRole.Text == "Co-Advisror")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 12);
                }
                else if (cbxRole.Text == "Industry Advisor")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 14);
                }
                cmd4.Parameters.AddWithValue("@AssignmentDate", DateTime.Now);
                cmd4.ExecuteNonQuery();
                MessageBox.Show("Project Advisor Added Successfully");
                LoadProjectAdvisorData();
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            if (cbxAId.Items.Count == 0 || cbxProject.Items.Count == 0 || cbxRole.Items.Count == 0)
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // check if the project advisor already exists
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId", con);
                cmd.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
                cmd.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count = (int)cmd.ExecuteScalar();
                if (count == 0)
                {
                    MessageBox.Show("Project Advisor does not exist");
                    return;
                }
                // Each Project can have one Main Advisor, one Co-Advisor and one Industry Advisor
                // Check if the project already has a Main Advisor
                SqlCommand cmd2 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 11", con);
                cmd2.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count2 = (int)cmd2.ExecuteScalar();
                if (count2 > 0 && cbxRole.Text == "Main Advisor")
                {
                    MessageBox.Show("Project already has a Main Advisor");
                    return;
                }
                // Check if the project already has a Co-Advisor
                SqlCommand cmd3 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 13", con);
                cmd3.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count3 = (int)cmd3.ExecuteScalar();
                if (count3 > 0 && cbxRole.Text == "Co-Advisror")
                {
                    MessageBox.Show("Project already has a Co-Advisor");
                    return;
                }
                // Check if the project already has a Industry Advisor
                SqlCommand cmd5 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = 14", con);
                cmd5.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                int count4 = (int)cmd5.ExecuteScalar();
                if (count4 > 0 && cbxRole.Text == "Industry Advisor")
                {
                    MessageBox.Show("Project already has a Industry Advisor");
                    return;
                }
                //Update the project of project advisor
                SqlCommand cmd4 = new SqlCommand("UPDATE ProjectAdvisor SET ProjectId = @ProjectId, AdvisorRole = @AdvisorRole WHERE AdvisorId = @AdvisorId", con);
                cmd4.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
                cmd4.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                // if the role is Main Advisor then assign  11, Co - Advisror then 12 , Industry Advisor then 14
                if (cbxRole.Text == "Main Advisor")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 11);
                }
                else if (cbxRole.Text == "Co-Advisror")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 12);
                }
                else if (cbxRole.Text == "Industry Advisor")
                {
                    cmd4.Parameters.AddWithValue("@AdvisorRole", 14);
                }
                cmd4.ExecuteNonQuery();
                MessageBox.Show("Project Advisor Updated Successfully");
                LoadProjectAdvisorData();

            }
        }

        private void UpdateAdvisorRoleButton_Click(object sender, EventArgs e)
        {
            if (cbxRole.Items.Count == 0 || cbxAId.Items.Count == 0)
            {
                MessageBox.Show("Please fill all the fields");

                return;
            }
            var con = Configuration.getInstance().getConnection();
            // Check if the new role is already assigned to the advisor
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId AND AdvisorRole = @AdvisorRole", con);
            cmd.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
            if (cbxRole.Text == "Main Advisor")
            {
                cmd.Parameters.AddWithValue("@AdvisorRole", 11);
            }
            else if (cbxRole.Text == "Co-Advisror")
            {
                cmd.Parameters.AddWithValue("@AdvisorRole", 12);
            }
            else if (cbxRole.Text == "Industry Advisor")
            {
                cmd.Parameters.AddWithValue("@AdvisorRole", 14);
            }
            int count = (int)cmd.ExecuteScalar();
            if (count > 0)
            {
                MessageBox.Show("Advisor already has this role");
                return;
            }
            // Check if the new role for the projct is already assigned to the  another project advisor
            SqlCommand cmd1 = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE ProjectId = @ProjectId AND AdvisorRole = @AdvisorRole", con);
            cmd1.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
                
            if (cbxRole.Text == "Main Advisor")
            {
                cmd1.Parameters.AddWithValue("@AdvisorRole", 11);
            }
            else if (cbxRole.Text == "Co-Advisror")
            {
                cmd1.Parameters.AddWithValue("@AdvisorRole", 12);
            }
            else if (cbxRole.Text == "Industry Advisor")
            {
                cmd1.Parameters.AddWithValue("@AdvisorRole", 14);
            }
            int count1 = (int)cmd1.ExecuteScalar();
            if (count1 > 0)
            {
                MessageBox.Show("Project already has a project advisor with this role");
                return;
            }

            // Update the role of the advisor
            SqlCommand cmd2 = new SqlCommand("UPDATE ProjectAdvisor SET AdvisorRole = @AdvisorRole WHERE AdvisorId = @AdvisorId", con);
            if (cbxRole.Text == "Main Advisor")
            {
                cmd2.Parameters.AddWithValue("@AdvisorRole", 11);
            }
            else if (cbxRole.Text == "Co-Advisror")
            {
                cmd2.Parameters.AddWithValue("@AdvisorRole", 12);
            }
            else if (cbxRole.Text == "Industry Advisor")
            {
                cmd2.Parameters.AddWithValue("@AdvisorRole", 14);
            }
            cmd2.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
            cmd2.ExecuteNonQuery();
            MessageBox.Show("Advisor Role Updated Successfully");
            LoadProjectAdvisorData();
        }

        private void DeleteProjectAdvisorButton_Click(object sender, EventArgs e)
        {
            if(cbxAId.Items.Count == 0 || cbxProject.Items.Count == 0)
            {
                MessageBox.Show("Please fill all the fields");
                return;
            }
            var con = Configuration.getInstance().getConnection();
            // Check if the project advisor exists
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId", con);
            cmd.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
            cmd.Parameters.AddWithValue("@ProjectId", cbxProject.Text);
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                MessageBox.Show("Project Advisor does not exist");
                return;
            }
            // Delete the project advisor
            SqlCommand cmd2 = new SqlCommand("DELETE FROM ProjectAdvisor WHERE AdvisorId = @AdvisorId", con);
            cmd2.Parameters.AddWithValue("@AdvisorId", cbxAId.Text);
            cmd2.ExecuteNonQuery();
            MessageBox.Show("Project Advisor Deleted Successfully");
            LoadProjectAdvisorData();
        }
    }
}
