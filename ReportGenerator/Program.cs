using System;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Paths
            string templatePath = @"F:/LTUD/MiniSupermarket-main/BaoCao_KitchenExpress.docx";
            string outputPath = @"F:/LTUD/MiniSupermarket-main/BaoCao_KitchenExpress_Final.docx";
            string evalPath = @"C:/Users/PC/.gemini/antigravity/brain/5381572c-3f3e-4751-8983-de30e78d832a/DanhGia_DoAn_MiniSupermarket.md";

            // Copy template to output (overwrite if exists)
            File.Copy(templatePath, outputPath, true);

            // Read evaluation markdown
            string evalContent = File.ReadAllText(evalPath);

            // Open document for editing
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(outputPath, true))
            {
                MainDocumentPart mainPart = wordDoc.MainDocumentPart;
                Body body = mainPart.Document.Body;

                // Insert a page break before the evaluation section
                body.AppendChild(new Paragraph(new Run(new Break() { Type = BreakValues.Page })));

                // Add heading for evaluation
                Paragraph heading = new Paragraph(new Run(new Text("Full Rubric Evaluation")));
                heading.ParagraphProperties = new ParagraphProperties(new ParagraphStyleId() { Val = "Heading1" });
                body.AppendChild(heading);

                // Add each line of the markdown as a separate paragraph (preserve line breaks)
                foreach (string line in evalContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None))
                {
                    // Create a run with Vietnamese language and a common font
                    Run run = new Run();
                    RunProperties runProps = new RunProperties();
                    // Set language to Vietnamese (Vietnamese locale)
                    runProps.Languages = new Languages() { Val = "vi-VN" };
                    // Set a Unicode‑compatible font (Arial)
                    runProps.RunFonts = new RunFonts() { Ascii = "Arial", HighAnsi = "Arial", ComplexScript = "Arial" };
                    run.Append(runProps);
                    run.Append(new Text(line));
                    Paragraph p = new Paragraph(run);
                    body.AppendChild(p);
                }

                mainPart.Document.Save();
            }

            Console.WriteLine($"Report generated: {outputPath}");
        }
    }
}

