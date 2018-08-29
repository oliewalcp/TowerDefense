using System.Threading;
public class LocalMessage {
	private static SocketCom.MessageHandler handler = null;
	public static ulong LocalPlayerNumber = 0;//本机玩家编号
	public static ushort LocalRoomNumber = 0;//本机所在房间号
	public static byte LocalIdentity = 1;//0房主，1普通玩家
	public static SocketCom scom = new SocketCom();
	public static Thread comThread = new Thread(Loop);

	private static void Loop(){
		if(handler != null)
			scom.ReceiveMessageLoop(handler);
	}
	public static void SetHandler(SocketCom.MessageHandler h){
		if(handler == null){
			handler = h;
			comThread.Start();
		} else {
			handler = h;
			scom.SetMessageHandler(handler);
		}
	}
}
