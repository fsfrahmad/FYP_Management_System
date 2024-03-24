using CRUD_Operations;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FYP_Management_System
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                // Establish database connection
                var con = Configuration.getInstance().getConnection();
                SqlCommand cmd = new SqlCommand("SELECT GroupId, ProjectId, Title, AssignmentDate FROM GroupProject JOIN Project ON ProjectId = Id", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Generate PDF
                using (Document doc = new Document())
                {
                    PdfWriter.GetInstance(doc, new FileStream("report.pdf", FileMode.Create));
                    doc.Open();

                    // Add data to PDF
                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        table.AddCell(new Phrase(dt.Columns[i].ColumnName));
                    }
                    table.HeaderRows = 1;

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            table.AddCell(dt.Rows[i][j].ToString());
                        }
                    }

                    doc.Add(table);
                }

                MessageBox.Show("PDF report generated successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
