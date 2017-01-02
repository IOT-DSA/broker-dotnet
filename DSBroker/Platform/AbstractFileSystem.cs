namespace DSBroker.Platform
{
    public abstract class AbstractFileSystem
    {
        public abstract string ReadFileAsString(string path);
        public abstract void WriteFileFromString(string path, string data);
        public abstract bool FileExists(string path);
    }
}
