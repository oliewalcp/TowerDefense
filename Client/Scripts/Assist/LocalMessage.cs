using System.Threading;
using System;
using UnityEngine;
//地图每一格
public struct Grid {
	public float width;
	public float height;
}
public struct GPosition {
	public byte x;
	public byte y;
}
public class LocalMessage {
	private static SocketCom.MessageHandler handler = null;
	public static ulong LocalPlayerNumber = 0;//本机玩家编号
	public static ushort LocalRoomNumber = 0;//本机所在房间号
	public static byte LocalIdentity = 1;//0房主，1普通玩家
	public static byte[][] Map = null;//地图
	public static Grid grid = new Grid();//地图每一格的大小
	public static GPosition StartGrid = new GPosition();//地图的起点坐标（格子数坐标）
	public static GPosition EndGrid = new GPosition();//地图的终点坐标（格子数坐标）
	public static SocketCom scom = new SocketCom();
	public static Thread comThread = new Thread(Loop);
	public static byte[] Bit_n = {0x00, 0x01, 0x03, 0x07, 0x0F, 0x1F, 0x3F, 0x7F, 0xFF};

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
		StartGrid.x = arg[6];
		StartGrid.y = arg[7];
		EndGrid.x = arg[8];
		EndGrid.y = arg[9];
		Map = new byte[lineNumber][];
		int line, column, currentBit = 0, currentByte = 10, tempShift = 0;
		byte temp = arg[currentByte++];
		for(line = 0; line < lineNumber; line++){
			Map[line] = new byte[columnNumber];
			for(column = 0; true;){
				if(currentBit + grid_bit > 8) {
					int lastShift = 8 - currentBit;//保存已经获取到的位数
					tempShift = grid_bit - lastShift;//获取需要保留的位数
					Map[line][column] = GetLowBit(temp, lastShift);
					if(tempShift == 0) column++;
					temp = arg[currentByte++];//读取下一字节
					currentBit = 0;
				}
				if(tempShift == 0) {
					currentBit += grid_bit;
					Map[line][column++] = GetLowBit(temp, grid_bit);
					temp >>= grid_bit;
				} else {
					currentBit += tempShift;
					Map[line][column] <<= tempShift;
					Map[line][column++] |= GetLowBit(temp, tempShift);
					temp >>= tempShift;
				}
				tempShift = 0;
				if(column == columnNumber) break;
			}
		}
	}
	/* 获取一个数的低n位
	   param[arg]:原数
	   param[bit_num]:要获取的最低的位数
	 */
	private static byte GetLowBit(byte arg, int bit_num){
		byte result = 0;
		result |= Bit_n[bit_num];
		result &= arg;
		return result;
	}
}
