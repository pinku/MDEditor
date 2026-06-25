using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace MDEditor.Models;

/// <summary>
/// Servizio per l'esportazione dei documenti in formato Word (DOCX).
/// </summary>
public static class ExportService
{
    /// <summary>
    /// Esporta il contenuto HTML in un file DOCX usando AltChunk.
    /// Word aprirà il file e renderizzerà l'HTML incorporato.
    /// </summary>
    public static void ExportToWord(string filePath, string htmlContent, string? cssContent = null)
    {
        var fullHtml = BuildFullHtml(htmlContent, cssContent);

        using var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
        var body = mainPart.Document.AppendChild(new Body());

        // Imposta i margini del documento
        var sectionProps = new SectionProperties(
            new PageMargin
            {
                Top = 1134,    // 2 cm in twips
                Right = 1134,
                Bottom = 1134,
                Left = 1134
            });
        body.Append(sectionProps);

        // Aggiungi l'HTML come AltChunk
        var altChunkPart = mainPart.AddAlternativeFormatImportPart(
            AlternativeFormatImportPartType.Html);

        using (var stream = altChunkPart.GetStream())
        using (var writer = new StreamWriter(stream))
        {
            writer.Write(fullHtml);
            writer.Flush();
        }

        var altChunk = new AltChunk { Id = mainPart.GetIdOfPart(altChunkPart) };
        body.InsertBefore(altChunk, sectionProps);
    }

    private static string BuildFullHtml(string htmlBody, string? css)
    {
        var cssBlock = !string.IsNullOrEmpty(css)
            ? $"<style>{css}</style>"
            : @"
<style>
body { font-family: 'Segoe UI', Arial, sans-serif; font-size: 12pt; line-height: 1.5; color: #333; }
h1 { font-size: 20pt; }
h2 { font-size: 16pt; }
h3 { font-size: 14pt; }
pre { background-color: #f5f5f5; padding: 10pt; border-radius: 4pt; }
code { font-family: 'Consolas', monospace; font-size: 10pt; }
table { border-collapse: collapse; width: 100%; }
th, td { border: 1pt solid #ccc; padding: 6pt; }
</style>";

        return $@"<!DOCTYPE html>
<html><head><meta charset='utf-8'>
{cssBlock}
</head>
<body>{htmlBody}</body></html>";
    }
}