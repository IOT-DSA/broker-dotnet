namespace DSBroker.Platform.NETCore
{
    public class NETCorePlatform : AbstractPlatform
    {
        public NETCorePlatform() : base(new FileSystem())
        {
        }
    }
}
