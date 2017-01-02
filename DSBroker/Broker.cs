using DSBroker.Platform;

namespace DSBroker
{
    public class Broker
    {
        public readonly KeyPair KeyPair;
        public readonly string DsId;
        public readonly ClientHandler ClientHandler;
        public readonly HttpHandler HttpHandler;
        public readonly Handshake Handshake;
        public readonly AbstractPlatform Platform;

        private readonly SHA256 _sha256;

        /// <summary>
        /// Initializes broker for startup.
        /// </summary>
        /// <param name="platform">Platform implementations</param>
        public Broker(AbstractPlatform platform)
        {
            Platform = platform;
            _sha256 = new SHA256();
            KeyPair = new KeyPair(".broker.key");
            KeyPair.Load(Platform.FileSystem);
            DsId = "broker-" + UrlBase64.Encode(_sha256.ComputeHash(KeyPair.EncodedPublicKey));

            ClientHandler = new ClientHandler();
            HttpHandler = new HttpHandler(this);
            Handshake = new Handshake(this);
        }
    }
}
