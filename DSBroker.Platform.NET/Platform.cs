namespace DSBroker.Platform.NET
{
    public class Platform : AbstractPlatform
    {
        public Platform()
            : base(new FileSystem())
        {
        }
    }
}
