namespace CMOSC
{
    public class Config
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }

        public Config(string ip, int port)
        {
            Ip = ip;
            Port = port;
        }
    }
}
