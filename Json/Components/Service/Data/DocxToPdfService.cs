using System.Diagnostics;

namespace Json.Components.Service.Data
{
    public class DocxToPdfService
    {
        private readonly IWebHostEnvironment _env;

        public DocxToPdfService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> UploadAndConvertDocxToPdf(Stream fileStream, string fileName)
        {
            if (fileStream == null || string.IsNullOrWhiteSpace(fileName))
                return null;

            var uploadPath = Path.Combine(_env.WebRootPath, "pdf-files");
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var docxPath = Path.Combine(uploadPath, fileName);
            using (var stream = new FileStream(docxPath, FileMode.Create))
            {
                await fileStream.CopyToAsync(stream);
            }

            return ConvertDocxToPdf(docxPath);
        }

        private string ConvertDocxToPdf(string docxPath)
        {
            var libreOfficePath = "/Applications/LibreOffice.app/Contents/MacOS/soffice";
            var pdfPath = Path.ChangeExtension(docxPath, ".pdf");
            var startInfo = new ProcessStartInfo
            {
                FileName = libreOfficePath,
                Arguments = $"--headless --convert-to pdf \"{docxPath}\" --outdir \"{Path.GetDirectoryName(docxPath)}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                using (var process = Process.Start(startInfo))
                {
                    process.WaitForExit();
                }

                return File.Exists(pdfPath) ? pdfPath : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error converting DOCX to PDF: {ex.Message}");
                return null;
            }
        }
    }
}