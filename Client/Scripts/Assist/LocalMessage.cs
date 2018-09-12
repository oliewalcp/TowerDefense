using System.Threading;
using System;
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
		int line, column, currentBit, lastShift = 0, currentByte = 6, tempShift = 0;
		for(line = 0; line < lineNumber; line++){
			column = 0;
			Map[line] = new byte[columnNumber];
			byte temp = arg[currentByte++];
			for(currentBit = 0; true; currentBit += grid_bit){
				byte t = 0;
				if(currentBit + grid_bit > 8) {
					lastShift = grid_bit + 8 - currentBit;//保存已经获取到的位数
					tempShift = grid_bit - lastShift;//获取需要保留的位数
					Map[line][column] = GetByte(temp >> (8 - lastShift));
					temp = arg[currentByte++];//读取下一字节
					currentBit = 0;
				}
				t = GetByte(temp << (currentBit + grid_bit) >> (8 - currentBit - grid_bit + tempShift));
				if(tempShift == 0) {
					currentBit += grid_bit;
					Map[line][column++] = t;
				} else {
					currentBit += tempShift;
					Map[line][column] = GetByte((Map[line][column] << lastShift) | t);
				}
				lastShift = tempShift = 0;
				if(column == columnNumber) break;
			}
		}
	}

	private static byte GetByte(int num){
		return BitConverter.GetBytes(num)[0];
	}
}
