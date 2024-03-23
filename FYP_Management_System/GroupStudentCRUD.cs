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
    public partial class GroupStudentCRUD : Form
    {
        public GroupStudentCRUD()
        {
            InitializeComponent();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
            MainForm mainForm = new MainForm();
            mainForm.Show();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadGroupStudentData();
        }
        private void LoadGroupStudentData()
        {
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select G.Id, GS.StudentId, GS.Status, GS.AssignmentDate, G.Created_On FROM GroupStudent GS JOIN [GROUP] G ON GS.GroupId = G.Id", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            dataGridView1.DataSource = dt;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            cbxSId.SelectedIndex = 0;
            cbxGroup.SelectedIndex = 0;
            cbxStatus.SelectedIndex = 0;
            dateTimePicker1.Value = DateTime.Now;
        }

        private void AddIntoExistingButton_Click(object sender, EventArgs e)
        {
            // Check if any combobox data items are none
            if (cbxSId.Items.Count == 0 || cbxGroup.Items.Count == 0 || cbxStatus.Items.Count == 0)
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // Check If Group has less than 4 students

                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) FROM GroupStudent WHERE GroupId = @GroupId", con);
                cmd.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                
                int count = (int)cmd.ExecuteScalar();
                
                if (count == 4)
                {
                    MessageBox.Show("Group has already 4 students");
                }
                else
                {
                    //Check if Student is already in Group
                    SqlCommand cmd1 = new SqlCommand("Select COUNT(*) FROM GroupStudent WHERE GroupId = @GroupId AND StudentId = @StudentId", con);
                    cmd1.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                    cmd1.Parameters.AddWithValue("@StudentId", cbxSId.SelectedValue);
                    
                    int count1 = (int)cmd1.ExecuteScalar();
                    
                    if (count1 > 0)
                    {
                        MessageBox.Show("Student is already in Group");
                        return;
                    }
                    //Check if Student is already in any other Group
                    SqlCommand cmd3 = new SqlCommand("Select COUNT(*) FROM GroupStudent WHERE StudentId = @StudentId", con);
                    cmd3.Parameters.AddWithValue("@StudentId", cbxSId.SelectedValue);
                    
                    int count3 = (int)cmd3.ExecuteScalar();
                    
                    if (count3 > 0)
                    {
                        MessageBox.Show("Student is already in another Group");
                        return;
                    }
                    // Add Student into Group
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO GroupStudent (GroupId, StudentId, Status, AssignmentDate) VALUES (@GroupId, @StudentId, @Status, @AssignmentDate)", con);
                    cmd2.Parameters.AddWithValue("@GroupId", cbxGroup.SelectedValue);
                    cmd2.Parameters.AddWithValue("@StudentId", cbxSId.SelectedValue);
                    // if Status is Active then 3 else 4
                    if (cbxStatus.Text == "Active")
                    {
                        cmd2.Parameters.AddWithValue("@Status", 3);
                    }
                    else
                    {
                        cmd2.Parameters.AddWithValue("@Status", 4);
                    }
                    cmd2.Parameters.AddWithValue("@AssignmentDate", dateTimePicker1.Value);
                    
                    cmd2.ExecuteNonQuery();
                    
                    MessageBox.Show("Student Added to Group");
                    LoadGroupStudentData();
                }

            }
        }

        private void GroupStudentCRUD_Load(object sender, EventArgs e)
        {
            // When form Loads Fill the ComboBoxes from database tables
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("Select Id FROM Student", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            cbxSId.DataSource = dt;
            cbxSId.DisplayMember = "Id";
            cbxSId.ValueMember = "Id";
            cbxSId.DataSource = dt;
            // Same for Group
            SqlCommand cmd2 = new SqlCommand("Select Id FROM [Group]", con);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd2);
            DataTable dt2 = new DataTable();
            sda2.Fill(dt2);
            cbxGroup.DataSource = dt2;
            cbxGroup.DisplayMember = "Id";
            cbxGroup.ValueMember = "Id";
            cbxGroup.DataSource = dt2;
            
        }

        private void AddIntoNewButton_Click(object sender, EventArgs e)
        {
            if (cbxSId.SelectedItem == null || cbxStatus.SelectedItem == null)
            {
                MessageBox.Show("Please fill all the fields");
            }
            else
            {
                // Restrict Maximum 10 Groups
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("Select COUNT(*) FROM [Group]", con);
                
                int count = (int)cmd.ExecuteScalar();
                
                if (count == 10)
                {
                    MessageBox.Show("Maximum 10 Groups are allowed");
                    return;
                }
                //Check if Student is already in any other Group
                SqlCommand cmd3 = new SqlCommand("Select COUNT(*) FROM GroupStudent WHERE StudentId = @StudentId", con);
                cmd3.Parameters.AddWithValue("@StudentId", cbxSId.SelectedValue);
                
                int count3 = (int)cmd3.ExecuteScalar();
                
                if (count3 > 0)
                {
                    MessageBox.Show("Student is already in another Group");
                    return;
                }

                // Create New Group
                SqlCommand cmd4 = new SqlCommand("INSERT INTO [Group] (Created_On) VALUES (@Created_On)", con);
                cmd4.Parameters.AddWithValue("@Created_On", DateTime.Now);
                
                cmd4.ExecuteNonQuery();
                
                // Get the Id of New Group
                SqlCommand cmd2 = new SqlCommand("Select MAX(Id) FROM [Group]", con);
                
                int GroupId = (int)cmd2.ExecuteScalar();
                
                // Add Student into New Group
                SqlCommand cmd5 = new SqlCommand("INSERT INTO GroupStudent (GroupId, StudentId, Status, AssignmentDate) VALUES (@GroupId, @StudentId, @Status, @AssignmentDate)", con);
                cmd5.Parameters.AddWithValue("@GroupId", GroupId);
                cmd5.Parameters.AddWithValue("@StudentId", cbxSId.SelectedValue);
                // if Status is Active then 3 else 4
                if (cbxStatus.Text == "Active")
                {
                    cmd5.Parameters.AddWithValue("@Status", 3);
                }
                else
                {
                    cmd5.Parameters.AddWithValue("@Status", 4);
                }
                cmd5.Parameters.AddWithValue("@AssignmentDate", dateTimePicker1.Value);
                
                cmd5.ExecuteNonQuery();
                
                MessageBox.Show("Student Added to Group");
                LoadGroupStudentData();
            }
        }
    }
}
