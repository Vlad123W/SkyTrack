using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using Spire.Doc;
using Spire.Doc.Documents;

namespace SkyTrack
{
    static class WordExport
    {
        public static bool ExportToWord(params Flight[] flights)
        {
            try
            {
                Document document = new Document();
                Section section = document.AddSection();

                string[] headers = { "ID", "Звідки", "Куди", "Виліт", "Приліт", "Ціна", "Місця" };

                Table table = section.AddTable(true);
                table.ResetCells(flights.Length + 1, headers.Length);

                TableRow headerRow = table.Rows[0];
                for (int i = 0; i < headers.Length; i++)
                {
                    Paragraph p = headerRow.Cells[i].AddParagraph();
                    p.AppendText(headers[i]);
                    headerRow.Cells[i].CellFormat.BackColor = System.Drawing.Color.LightGray;
                    p.Format.HorizontalAlignment = HorizontalAlignment.Center;
                }

                for (int i = 0; i < flights.Length; i++)
                {
                    var flight = flights[i];
                    TableRow row = table.Rows[i + 1];
                    row.Cells[0].AddParagraph().AppendText(flight.FlightId.ToString());
                    row.Cells[1].AddParagraph().AppendText(flight.Origin);
                    row.Cells[2].AddParagraph().AppendText(flight.Destination);
                    row.Cells[3].AddParagraph().AppendText(flight.DepartureTime.ToString("g"));
                    row.Cells[4].AddParagraph().AppendText(flight.ArrivalTime.ToString("g"));
                    row.Cells[5].AddParagraph().AppendText($"{flight.Price:C}");
                    row.Cells[6].AddParagraph().AppendText(flight.AvailableSeats.ToString());
                }

                document.SaveToFile(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "Рейси.docx", FileFormat.Docx);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error exporting to Word: {ex.Message}");
                return false;
            }
        }
    }
}
