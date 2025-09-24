using CsvHelper;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using renkimsumatin.Models;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

namespace renkimsumatin.Services
{
    public static class ExportService
    {
        public static void ExportToCSV(List<ReportData> data, string filename)
        {
            try
            {
                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteField("Student Name");
                csv.WriteField("Student ID");
                csv.WriteField("Subject");
                csv.WriteField("Assessment");
                csv.WriteField("Total Marks");
                csv.WriteField("Score");
                csv.WriteField("Grade");
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteField(item.StudentName);
                    csv.WriteField(item.StudentId);
                    csv.WriteField(item.SubjectName);
                    csv.WriteField(item.AssessmentName);
                    csv.WriteField(item.TotalMarks);
                    csv.WriteField(item.MarksObtained?.ToString() ?? "N/A");
                    csv.WriteField(item.Grade.ToString());
                    csv.NextRecord();
                }

                var csvString = writer.ToString();
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"{filename}.csv",
                    Filter = "CSV Files (*.csv)|*.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, csvString, Encoding.UTF8);
                    MessageBox.Show("CSV exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ExportToPDF(List<ReportData> data, string filename)
        {
            try
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Fonts
                var font = new XFont("Arial", 12);
                var boldFont = new XFont("Arial", 14, XFontStyleEx.Bold);

                // Title
                gfx.DrawString("Student Grade Report", boldFont, XBrushes.Black,
                    new XRect(XUnit.FromPoint(0), XUnit.FromPoint(40), XUnit.FromPoint(page.Width.Point), XUnit.FromPoint(page.Height.Point)), XStringFormats.TopCenter);

                // Table headers
                string[] headers = { "Student Name", "Student ID", "Subject", "Assessment", "Score", "Grade" };
                double[] columnWidths = { 100, 80, 80, 80, 60, 40 };
                double yPosition = 80;
                double xPosition = 20;

                // Draw headers
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XBrushes.LightGray,
                        new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)));
                    gfx.DrawString(headers[i], boldFont, XBrushes.Black,
                        new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)),
                        XStringFormats.Center);
                    xPosition += columnWidths[i];
                }

                yPosition += 25;

                // Draw data rows
                foreach (var item in data)
                {
                    xPosition = 20;
                    string[] rowData = {
                        item.StudentName,
                        item.StudentId,
                        item.SubjectName,
                        item.AssessmentName,
                        $"{item.MarksObtained?.ToString() ?? "N/A"} / {item.TotalMarks}",
                        item.Grade.ToString()
                    };

                    for (int i = 0; i < rowData.Length; i++)
                    {
                        gfx.DrawRectangle(XBrushes.White,
                            new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)));
                        gfx.DrawString(rowData[i], font, XBrushes.Black,
                            new XRect(XUnit.FromPoint(xPosition + 2), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i] - 4), XUnit.FromPoint(20)),
                            XStringFormats.CenterLeft);
                        xPosition += columnWidths[i];
                    }
                    yPosition += 25;

                    // Add new page if needed
                    if (yPosition > page.Height.Point - 50)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40;
                    }
                }

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"{filename}.pdf",
                    Filter = "PDF Files (*.pdf)|*.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    document.Save(saveDialog.FileName);
                    MessageBox.Show("PDF exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // New: Export only Student ID, Subject, Assessment, Score, Grade (no Student Name)
        public static void ExportRanksToCSV(List<ReportData> data, string filename)
        {
            try
            {
                using var writer = new StringWriter();
                using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

                csv.WriteField("Student ID");
                csv.WriteField("Subject");
                csv.WriteField("Assessment");
                csv.WriteField("Score");
                csv.WriteField("Grade");
                csv.NextRecord();

                foreach (var item in data)
                {
                    csv.WriteField(item.StudentId);
                    csv.WriteField(item.SubjectName);
                    csv.WriteField(item.AssessmentName);
                    // format as Obtained/Total (e.g., 25/30)
                    var scoreText = (item.MarksObtained.HasValue ? item.MarksObtained.Value.ToString() : "N/A") + " / " + item.TotalMarks;
                    csv.WriteField(scoreText);
                    csv.WriteField(item.Grade.ToString());
                    csv.NextRecord();
                }

                var csvString = writer.ToString();
                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"{filename}.csv",
                    Filter = "CSV Files (*.csv)|*.csv"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, csvString, Encoding.UTF8);
                    MessageBox.Show("CSV exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting CSV: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public static void ExportRanksToPDF(List<ReportData> data, string filename)
        {
            try
            {
                var document = new PdfDocument();
                var page = document.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Fonts
                var font = new XFont("Arial", 12);
                var boldFont = new XFont("Arial", 14, XFontStyleEx.Bold);

                // Title
                gfx.DrawString("Student Ranking Report", boldFont, XBrushes.Black,
                    new XRect(XUnit.FromPoint(0), XUnit.FromPoint(40), XUnit.FromPoint(page.Width.Point), XUnit.FromPoint(page.Height.Point)), XStringFormats.TopCenter);

                // Table headers (no Student Name)
                string[] headers = { "Student ID", "Subject", "Assessment", "Score", "Grade" };
                double[] columnWidths = { 100, 120, 120, 80, 60 };
                double yPosition = 80;
                double xPosition = 20;

                // Draw headers
                for (int i = 0; i < headers.Length; i++)
                {
                    gfx.DrawRectangle(XBrushes.LightGray,
                        new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)));
                    gfx.DrawString(headers[i], boldFont, XBrushes.Black,
                        new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)),
                        XStringFormats.Center);
                    xPosition += columnWidths[i];
                }

                yPosition += 25;

                // Draw data rows
                foreach (var item in data)
                {
                    xPosition = 20;
                    string[] rowData = {
                        item.StudentId,
                        item.SubjectName,
                        item.AssessmentName,
                        $"{item.MarksObtained?.ToString() ?? "N/A"} / {item.TotalMarks}",
                        item.Grade.ToString()
                    };

                    for (int i = 0; i < rowData.Length; i++)
                    {
                        gfx.DrawRectangle(XBrushes.White,
                            new XRect(XUnit.FromPoint(xPosition), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i]), XUnit.FromPoint(20)));
                        gfx.DrawString(rowData[i], font, XBrushes.Black,
                            new XRect(XUnit.FromPoint(xPosition + 2), XUnit.FromPoint(yPosition), XUnit.FromPoint(columnWidths[i] - 4), XUnit.FromPoint(20)),
                            XStringFormats.CenterLeft);
                        xPosition += columnWidths[i];
                    }

                    yPosition += 25;

                    // New page if needed
                    if (yPosition > page.Height.Point - 50)
                    {
                        page = document.AddPage();
                        gfx = XGraphics.FromPdfPage(page);
                        yPosition = 40;
                    }
                }

                var saveDialog = new Microsoft.Win32.SaveFileDialog
                {
                    FileName = $"{filename}.pdf",
                    Filter = "PDF Files (*.pdf)|*.pdf"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    document.Save(saveDialog.FileName);
                    MessageBox.Show("PDF exported successfully!", "Export Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting PDF: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}