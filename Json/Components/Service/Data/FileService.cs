namespace Json.Components.Service.Data
{
    public class FileService
    {
        private readonly DocxToPdfService _docxToPdfService;

        public FileService(DocxToPdfService docxToPdfService)
        {
            _docxToPdfService = docxToPdfService;
        }

        public async Task<string> ProcessFile(Stream fileStream, string fileName)
        {
            if (fileStream == null || string.IsNullOrWhiteSpace(fileName))
                return null;

            return await _docxToPdfService.UploadAndConvertDocxToPdf(fileStream, fileName);
        }
    }
}