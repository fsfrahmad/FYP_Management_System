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
    public partial class GroupEvaluation : Form
    {
        public GroupEvaluation()
        {
            InitializeComponent();
        }

        private void LoadButton_Click(object sender, EventArgs e)
        {
            LoadData();
        }
        private void LoadData()
        {
            // Data Load From GroupEvaluation Table
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT * FROM GroupEvaluation", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            dataGridView1.DataSource = dt;
            // Set all column width 200
            for (int i = 2; i < dataGridView1.Columns.Count; i++)
            {
                dataGridView1.Columns[i].Width = 172;
            }

        }

        private void GroupEvaluation_Load(object sender, EventArgs e)
        {
            // Load ComboBox values from database tables Group and Evaluation
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT Id FROM [Group]", con);
            SqlDataAdapter adapter = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapter.Fill(dt);
            cbxGroupId.DataSource = dt;
            cbxGroupId.DisplayMember = "Id";
            cbxGroupId.ValueMember = "Id";
            cbxGroupId.DataSource = dt;
            SqlCommand cmd1 = new SqlCommand("SELECT Id FROM Evaluation", con);
            SqlDataAdapter adapter1 = new SqlDataAdapter(cmd1);
            DataTable dt1 = new DataTable();
            adapter1.Fill(dt1);
            cbxEvaluationId.DataSource = dt1;
            cbxEvaluationId.DisplayMember = "Id";
            cbxEvaluationId.ValueMember = "Id";
            cbxEvaluationId.DataSource = dt1;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            cbxGroupId.SelectedIndex = 0;
            cbxEvaluationId.SelectedIndex = 0;
            txtObtainedMarks.Text = "";
            dateTimePicker1.Value = DateTime.Now;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }

        private void AssignMarksButton_Click(object sender, EventArgs e)
        {
            if(cbxGroupId.SelectedIndex == -1 || cbxEvaluationId.SelectedIndex == -1 || txtObtainedMarks.Text == "")
            {
                MessageBox.Show("Please fill all fields");
            }
            else
            {
                // Check if the GroupId and EvaluationId already exists in GroupEvaluation Table
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM GroupEvaluation WHERE GroupId = @GroupId AND EvaluationId = @EvaluationId", con);
                cmd.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
                cmd.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
                int count = (int)cmd.ExecuteScalar();
                if(count > 0)
                {
                    MessageBox.Show("Group and Evaluation already exists in GroupEvaluation Table");
                }
                else
                {
                    //make sure that the obtained marks are not  greater than total marks of evaluation
                    SqlCommand cmd1 = new SqlCommand("SELECT TotalMarks FROM Evaluation WHERE Id = @Id", con);
                    cmd1.Parameters.AddWithValue("@Id", cbxEvaluationId.SelectedValue);
                    int totalMarks = (int)cmd1.ExecuteScalar();
                    if (Convert.ToInt32(txtObtainedMarks.Text) > totalMarks)
                    {
                        MessageBox.Show("Obtained Marks should not be greater than Total Marks of Evaluation");
                    }
                    else
                    {
                        // Insert GroupId, EvaluationId, ObtainedMarks and EvaluationDate in GroupEvaluation Table
                        SqlCommand cmd2 = new SqlCommand("INSERT INTO GroupEvaluation (GroupId, EvaluationId, ObtainedMarks, EvaluationDate) VALUES (@GroupId, @EvaluationId, @ObtainedMarks, @EvaluationDate)", con);
                        cmd2.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
                        cmd2.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
                        cmd2.Parameters.AddWithValue("@ObtainedMarks", txtObtainedMarks.Text);
                        cmd2.Parameters.AddWithValue("@EvaluationDate", dateTimePicker1.Value);
                        cmd2.ExecuteNonQuery();
                        MessageBox.Show("Marks Assigned Successfully");
                        LoadData();
                    }
                }
                

            }
        }

        private void UpdateMarksButton_Click(object sender, EventArgs e)
        {
            if (cbxGroupId.SelectedIndex == -1 || cbxEvaluationId.SelectedIndex == -1 || txtObtainedMarks.Text == "")
            {
                MessageBox.Show("Please fill all fields");
                return;
            }
            // Check if the GroupId and EvaluationId already exists in GroupEvaluation Table
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM GroupEvaluation WHERE GroupId = @GroupId AND EvaluationId = @EvaluationId", con);
            cmd.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
            cmd.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                MessageBox.Show("Group and Evaluation does not exists in GroupEvaluation Table");
            }
            else
            {
                //make sure that the obtained marks are not  greater than total marks of evaluation
                SqlCommand cmd1 = new SqlCommand("SELECT TotalMarks FROM Evaluation WHERE Id = @Id", con);
                cmd1.Parameters.AddWithValue("@Id", cbxEvaluationId.SelectedValue);
                int totalMarks = (int)cmd1.ExecuteScalar();
                if (Convert.ToInt32(txtObtainedMarks.Text) > totalMarks)
                {
                    MessageBox.Show("Obtained Marks should not be greater than Total Marks of Evaluation");
                }
                else
                {
                    // Update ObtainedMarks and EvaluationDate in GroupEvaluation Table
                    SqlCommand cmd2 = new SqlCommand("UPDATE GroupEvaluation SET ObtainedMarks = @ObtainedMarks, EvaluationDate = @EvaluationDate WHERE GroupId = @GroupId AND EvaluationId = @EvaluationId", con);
                    cmd2.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
                    cmd2.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
                    cmd2.Parameters.AddWithValue("@ObtainedMarks", txtObtainedMarks.Text);
                    cmd2.Parameters.AddWithValue("@EvaluationDate", dateTimePicker1.Value);
                    cmd2.ExecuteNonQuery();
                    MessageBox.Show("Marks Updated Successfully");
                    LoadData();
                }
            }

        }

        private void RemoveEvaluationButton_Click(object sender, EventArgs e)
        {
            if (cbxGroupId.SelectedIndex == -1 || cbxEvaluationId.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill all fields");
                return;
            }
            // Check if the GroupId and EvaluationId already exists in GroupEvaluation Table
            var con = Configuration.getInstance().getConnection();
            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM GroupEvaluation WHERE GroupId = @GroupId AND EvaluationId = @EvaluationId", con);
            cmd.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
            cmd.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
            int count = (int)cmd.ExecuteScalar();
            if (count == 0)
            {
                MessageBox.Show("Group and Evaluation does not exists in GroupEvaluation Table");
            }
            else
            {
                // Delete GroupId and EvaluationId from GroupEvaluation Table
                SqlCommand cmd2 = new SqlCommand("DELETE FROM GroupEvaluation WHERE GroupId = @GroupId AND EvaluationId = @EvaluationId", con);
                cmd2.Parameters.AddWithValue("@GroupId", cbxGroupId.SelectedValue);
                cmd2.Parameters.AddWithValue("@EvaluationId", cbxEvaluationId.SelectedValue);
                cmd2.ExecuteNonQuery();
                MessageBox.Show("Evaluation Removed Successfully");
                LoadData();
            }

        }
    }
}
