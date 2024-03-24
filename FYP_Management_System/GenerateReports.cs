using iTextSharp.text.pdf;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRUD_Operations;
using System.Data.SqlClient;

namespace FYP_Management_System
{
    public partial class GenerateReports : Form
    {
        public GenerateReports()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm(); 
            mainForm.Show();
            this.Close();
        }
        private void ExportToPDF(DataTable dt, string name, string description)
        {
            try
            {
                using (Document document = new Document(PageSize.A4, 20, 20, 20, 20))
                {
                    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    string pdfFilePath = Path.Combine(desktopPath, name + ".pdf");

                    PdfWriter.GetInstance(document, new FileStream(pdfFilePath, FileMode.Create));
                    document.Open();

                    // Add report name and description to the PDF
                    Paragraph reportNameParagraph = new Paragraph(name, new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.TIMES_ROMAN, 18, iTextSharp.text.Font.BOLD));
                    reportNameParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(reportNameParagraph);

                    Paragraph reportDescriptionParagraph = new Paragraph(description, FontFactory.GetFont("Times New Roman", 12));
                    reportDescriptionParagraph.Alignment = Element.ALIGN_CENTER;
                    document.Add(reportDescriptionParagraph);

                    // Add table with data from DataTable
                    PdfPTable table = new PdfPTable(dt.Columns.Count);
                    table.WidthPercentage = 100;

                    // Add column headers
                    foreach (DataColumn column in dt.Columns)
                    {
                        table.AddCell(new PdfPCell(new Phrase(column.ColumnName)));
                    }

                    // Add data rows
                    foreach (DataRow row in dt.Rows)
                    {
                        foreach (object item in row.ItemArray)
                        {
                            table.AddCell(new PdfPCell(new Phrase(item.ToString())));
                        }
                    }

                    document.Add(table);
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show("An error occurred while generating the report: " + exp.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                // Establish database connection
                var con = Configuration.getInstance().getConnection();
                if(cbxNo.SelectedIndex == 0)
                {
                    string query = "SELECT P.Title AS ProjectTitle, ISNULL(AdvP.FirstName + ' ' + AdvP.LastName, 'No Advisor Assigned') AS AdvisorName, ISNULL(L.Value, 'N/A') AS AdvisorRole, ISNULL(StuP.FirstName + ' ' + StuP.LastName, 'No Students Assigned') AS StudentName FROM Project AS P LEFT JOIN ProjectAdvisor AS PA ON P.Id = PA.ProjectId LEFT JOIN Advisor AS A ON PA.AdvisorId = A.Id LEFT JOIN GroupProject AS GP ON P.Id = GP.ProjectId LEFT JOIN GroupStudent AS GS ON GP.GroupId = GS.GroupId LEFT JOIN Student AS S ON GS.StudentId = S.Id LEFT JOIN Person AS AdvP ON A.Id = AdvP.Id LEFT JOIN Person AS StuP ON S.Id = StuP.Id LEFT JOIN Lookup AS L ON PA.AdvisorRole = L.Id ORDER BY P.Title;";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);

                    // Generate PDF report
                    ExportToPDF(dt, "Projects", "List of projects along with advisory board and list of students \n ...");

                    MessageBox.Show("PDF report generated successfully! You can find it on your Desktop.");
                }
                else if(cbxNo.SelectedIndex == 1)
                {
                    string query = "SELECT P.Title AS ProjectTitle, CONCAT(StuP.FirstName, ' ', StuP.LastName) AS StudentName, E.Name AS EvaluationName, ISNULL(CAST(GE.ObtainedMarks AS VARCHAR), 'Not Evaluated') AS ObtainedMarks FROM [Project] AS P INNER JOIN [GroupProject] AS GP ON P.Id = GP.ProjectId INNER JOIN [GroupStudent] AS GS ON GP.GroupId = GS.GroupId INNER JOIN [Student] AS S ON GS.StudentId = S.Id LEFT JOIN [Person] AS StuP ON S.Id = StuP.Id LEFT JOIN [GroupEvaluation] AS GE ON GP.GroupId = GE.GroupId LEFT JOIN [Evaluation] AS E ON GE.EvaluationId = E.Id ORDER BY P.Title, StuP.FirstName, StuP.LastName, E.Name;";
                    SqlCommand cmd = new SqlCommand(query, con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    // Generate PDF report
                    ExportToPDF(dt, "Evaluation of Projects", "Marks sheet of projects that shows the marks in each evaluation against each student and project \n ...");

                    MessageBox.Show("PDF report generated successfully! You can find it on your Desktop.");
                }
                else
                {
                    MessageBox.Show("Please select a report to generate.");
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}
