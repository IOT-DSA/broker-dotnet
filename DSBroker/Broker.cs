using PCLStorage;
using System.Threading.Tasks;

namespace DSBroker
{
    public class Broker
    {
        public readonly KeyPair KeyPair;
        public readonly string DsId;
        public readonly ClientHandler ClientHandler;
        public readonly HttpHandler HttpHandler;
        public readonly Handshake Handshake;

        private readonly SHA256 _sha256;

        public Broker()
        {
            _sha256 = new SHA256();
            KeyPair = new KeyPair();
            DsId = "broker-" + UrlBase64.Encode(_sha256.ComputeHash(KeyPair.EncodedPublicKey));
            /*Task.Run(async () =>
            {
                KeyPair = new KeyPair(await FileSystem.Current.GetFolderFromPathAsync("."), ".broker.key");
                await KeyPair.Load();
            }).Wait();*/

            ClientHandler = new ClientHandler();
            HttpHandler = new HttpHandler(this);
            Handshake = new Handshake(this);
        }
    }
}
