using Microsoft.AspNetCore.Http;

namespace Session04_MVC.Helpers
{
    // Helper to handle uploading and deleting files (images/documents)
    public static class DocumentSettings
    {
        public static string UploadFile(IFormFile file, string folderName)
        {
            // 1. Get folder path (e.g. wwwroot/files/images)
            string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName);

            // Ensure folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 2. Generate unique file name using Guid
            string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";

            // 3. Combine folder path with file name
            string filePath = Path.Combine(folderPath, fileName);

            // 4. Save file to disk stream
            using var fileStream = new FileStream(filePath, FileMode.Create);
            file.CopyTo(fileStream);

            return fileName;
        }

        public static void DeleteFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files", folderName, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
