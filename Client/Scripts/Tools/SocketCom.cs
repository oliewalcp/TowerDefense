using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
public class SocketCom {
    private static Mutex SocketMutex = new Mutex();
    private const byte Version = 0;//通信版本号
    private const int Port = 33333;//端口号
    private const string ServerIP = "47.106.72.134";//服务器IP地址
    private IPEndPoint ServerPoint = new IPEndPoint(IPAddress.Parse(ServerIP), Port);//服务端

    public delegate void MessageHandler(byte[] message);
    private MessageHandler handler = null;
    private bool StopLoop = false;//标记是否停止通信
    private Socket socket = null;

    public SocketCom() {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    }
    public bool Initialize(){
        try{
            socket.Connect(ServerPoint);
        } catch {
            return false;
        }
        return true;
    }
    public void SetMessageHandler(MessageHandler handler){
        this.handler = handler;
    }
    //接收消息循环
    public void ReceiveMessageLoop(MessageHandler handler) {
        this.handler = handler;
        while(!StopLoop) {
            byte[] msg = new byte[1024];
            SocketMutex.WaitOne();
            socket.Receive(msg, 1024, SocketFlags.None);
            SocketMutex.ReleaseMutex();
            if(msg[0] != Version) continue;
            if(this.handler != null)
                this.handler(msg);
        }
    }

    public void SendMessage(byte type, string message, string target_ip = ServerIP) {
        IPEndPoint remote = null;
        if(target_ip == ServerIP) 
            remote = ServerPoint;
        else remote = new IPEndPoint(IPAddress.Parse(target_ip), Port);
        byte[] msg = Encoding.ASCII.GetBytes(message);
        SendMessage(type, msg, remote);
    }
    public void SendMessage(byte type, string message) {
        byte[] msg = Encoding.ASCII.GetBytes(message);
        SendMessage(type, msg);
    }
    public void SendMessage(byte type, byte[] message = null, IPEndPoint remote = null) {
        byte[] msg = new byte[1024];
        msg[0] = Version;
        msg[1] = type;
        if(message != null)
            message.CopyTo(msg, 2);
        SocketMutex.WaitOne();
        if(remote == null)
            socket.Send(msg);
        else socket.SendTo(msg, remote);
        SocketMutex.ReleaseMutex();
    }
    public void Close(){
        try{
            StopLoop = true;
            handler = null;
            socket.Close();
        } catch{}
    }
}
