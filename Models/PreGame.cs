using Microsoft.JSInterop;

namespace game.Models
{
    // class to manage things before compilation
    public class PreGame
    {
        public void setAnimationsToClasses()
        {
            // look at models folder
            // sets the animations
            
        }

        public async static void PreLoadAssets(IJSRuntime jsRuntime) 
        {
            List<string> imagePaths = new List<string>();
            ListImages(imagePaths);
            Console.Out.WriteLine(imagePaths);
            await jsRuntime.InvokeVoidAsync("preloadImages", imagePaths);
        }

        public static void ListImages(List<string> imagePaths)
        {
            string workingDirectory = Environment.CurrentDirectory;
            string assetsDirectory = Directory.GetParent(workingDirectory).Parent.Parent.FullName;

            // Recursively get all images from the assets folder and subfolders
            GetImagesFromDirectory(assetsDirectory, imagePaths);
        }

        private static void GetImagesFromDirectory(string directory, List<string> imagePaths)
        {
            // Get all files from the current directory
            var files = Directory.GetFiles("../../../../");
            var f= Directory.GetDirectories(directory);
            foreach (var file in files)
            {
                // Add the relative path of the image (removing root part)
                string relativePath = file.Replace(Directory.GetCurrentDirectory(), "").Replace("\\", "/");
                imagePaths.Add(relativePath);
            }

            // Recursively process subdirectories
            var subDirectories = Directory.GetDirectories(directory);
            foreach (var subDir in subDirectories)
            {
                GetImagesFromDirectory(subDir, imagePaths);
            }
        }
    }
}
