using ProniaBackEnd.Models;

namespace ProniaBackEnd.Utilities.Extensions
{
    public static class FileValidatator
    {
        public static bool ValidateType(this IFormFile file,string type="image")
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }

            return false;
        }

        public static bool ValidateSize(this IFormFile file,int limitkb)
        {
            if (file.Length > limitkb * 1024)
            {
                return true;
            }

            return false;
        }

        public static async Task<string> CreateFile(this IFormFile file,string root,params string[] folders)
        {
            string fileName = Guid.NewGuid().ToString() + file.FileName;
            string path = root;

            for (int i = 0; i < folders.Length; i++)
            {
                path=Path.Combine(path, folders[i]);
            }

            path=Path.Combine(path, fileName);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
                
            return fileName;
            
        }

        public static async void DeleteFile(this string fileName,string root,params string[] folders)
        {
            string path= root;
            for (int i = 0; i < folders.Length; i++)
            {
                path = Path.Combine(path, folders[i]);
            }

            path=Path.Combine(path, fileName);

            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
