using System.IO;

namespace DSBroker.Platform.NET
{
    public class FileSystem : AbstractFileSystem
    {
        public override string ReadFileAsString(string path)
        {
            return File.ReadAllText(path);
        }

        public override void WriteFileFromString(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        public override bool FileExists(string path)
        {
            return File.Exists(path);
        }
    }
}
