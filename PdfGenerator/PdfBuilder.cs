using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;

namespace PdfGenerator
{

    public class FillerText
    {
        public static string Text = @"Loboreet autpat, C./ Núñez de Balboa 35-A, 3rd floor B quis adigna conse dipit la consed exeril et utpatetuer autat, voloboreet, consequamet ilit nos aut in henit ullam, sim doloreratis dolobore tat, venim quissequat. Nisci tat laor ametumsan vulla feuisim ing eliquisi tatum autat, velenisit iustionsed tis dunt exerostrud dolore verae.";
        public static string ShortText = @"Núñez de Balboa 35-A";
    }

    public class PdfBuilder
    {
        public Document BuildDocument()
        {
            // Create a MigraDoc document
            Document document = CreateDocument();
            document.UseCmykColor = true;

            return document;

        }

        private Document CreateDocument()
        {
            // Create a new MigraDoc document
            Document document = new Document();
            document.Info.Title = "Hello World";

            // Add a section to the document
            Section section = document.AddSection();

            // Add a paragraph to the section
            Paragraph paragraph = section.AddParagraph();

            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);

            var text =
                @"Loboreet autpat, C./ Núñez de Balboa 35-A, 3rd floor B quis adigna conse dipit la consed exeril et utpatetuer autat, voloboreet, consequamet ilit nos aut in henit ullam, sim doloreratis dolobore tat, venim quissequat. Nisci tat laor ametumsan vulla feuisim ing eliquisi tatum autat, velenisit iustionsed tis dunt exerostrud dolore verae.";

            // Add some text to the paragraph
            paragraph.AddText(text);
            //paragraph.AddFormattedText("Hello, World!", TextFormat.Bold);

            for (int i = 0; i < 15; i++)
                AddParagraph(document, text);

            DemonstrateSimpleTable(document);
            return document;
        }

        private void AddParagraph(Document document, string text)
        {
            document.LastSection.AddParagraph("Justified", "Heading3");

            Paragraph paragraph = document.LastSection.AddParagraph();
            paragraph.Format.Font.Color = Color.FromCmyk(100, 30, 20, 50);
            paragraph.Format.Alignment = ParagraphAlignment.Justify;
            paragraph.AddText(text);
        }

        private void DemonstrateSimpleTable(Document document)
        {
            document.LastSection.AddParagraph("Simple Tables", "Heading2");

            Table table = new Table();
            table.Borders.Width = 0.75;

            Column column = table.AddColumn(Unit.FromCentimeter(2));
            column.Format.Alignment = ParagraphAlignment.Center;

            table.AddColumn(Unit.FromCentimeter(5));

            Row row = table.AddRow();
            row.Shading.Color = Colors.PaleGoldenrod;
            Cell cell = row.Cells[0];
            cell.AddParagraph("Itemus");
            cell = row.Cells[1];
            cell.AddParagraph("Descriptum");

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("1");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.ShortText);

            row = table.AddRow();
            cell = row.Cells[0];
            cell.AddParagraph("2");
            cell = row.Cells[1];
            cell.AddParagraph(FillerText.Text);

            table.SetEdge(0, 0, 2, 3, Edge.Box, BorderStyle.Single, 1.5, Colors.Black);

            document.LastSection.Add(table);
        }

    }
}
