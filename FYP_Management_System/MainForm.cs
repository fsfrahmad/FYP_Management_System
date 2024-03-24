using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FYP_Management_System
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            StudentCRUD studentCRUD = new StudentCRUD();
            studentCRUD.Show();
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            AdvisorCRUD advisorCRUD = new AdvisorCRUD();
            advisorCRUD.Show();
            this.Hide();
        }
        private bool closingConfirmed = false;

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!closingConfirmed)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to close the application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.No)
                {
                    e.Cancel = true;
                }
                else
                {
                    closingConfirmed = true;
                }
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            GroupStudentCRUD groupStudentCRUD = new GroupStudentCRUD();
            groupStudentCRUD.Show();
            this.Hide();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProjectCRUD projectCRUD = new ProjectCRUD();
            projectCRUD.Show();
            this.Hide();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ProjectAdvisorCRUD projectAdvisorCRUD = new ProjectAdvisorCRUD();
            projectAdvisorCRUD.Show();
            this.Hide();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Group_Project group_Project = new Group_Project();
            group_Project.Show();
            this.Hide();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            EvaluationCRUD evaluationCRUD = new EvaluationCRUD();
            evaluationCRUD.Show();
            this.Hide();
        }
    }
}
