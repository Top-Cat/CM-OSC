namespace CMOSC
{
    public class Config
    {
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public int Chroma { get; private set; }

        public Config(string ip, int port, int chroma)
        {
            Ip = ip;
            Port = port;
            Chroma = chroma;
        }
    }
}
