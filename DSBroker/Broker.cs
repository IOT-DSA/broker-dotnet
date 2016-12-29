namespace DSBroker
{
    public class Broker
    {
        public readonly ClientHandler ClientHandler;
        public readonly HttpHandler HttpHandler;

        public Broker()
        {
            ClientHandler = new ClientHandler();
            HttpHandler = new HttpHandler(this);
        }
    }
}
