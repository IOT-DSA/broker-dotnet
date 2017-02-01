namespace DSBroker.Platform
{
    public abstract class AbstractPlatform
    {
        public readonly AbstractFileSystem FileSystem;

        protected AbstractPlatform(AbstractFileSystem fileSystem = null)
        {
            FileSystem = fileSystem;
        }
    }
}
