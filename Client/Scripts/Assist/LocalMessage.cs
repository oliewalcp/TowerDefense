using System.Threading;
public class LocalMessage {
	private static SocketCom.MessageHandler handler = null;
	public static ulong LocalPlayerNumber = 0;//本机玩家编号
	public static ushort LocalRoomNumber = 0;//本机所在房间号
	public static byte LocalIdentity = 1;//0房主，1普通玩家
	public static byte[][] Map = null;//地图
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
	/* 根据数据生成地图
	   param[arg]:地图的数据
	   param[grid_bit]:一格地图占用的位数
	*/
	public static void SetMap(byte[] arg, int grid_bit = 2){
		ushort lineNumber = arg[4], columnNumber = arg[5];
		Map = new byte[lineNumber][];
		int i, j;
		for(i = 0; i < lineNumber; i++){
			Map[i] = new byte[columnNumber];
			ushort lastShift = 0;//上一次移位
			byte temp = arg[i + 6];
			for(j = 0; j + grid_bit < 8; j++){
				//未完待续
			}
		}
	}
}
