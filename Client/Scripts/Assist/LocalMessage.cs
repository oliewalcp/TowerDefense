using System.Threading;
using System;
using UnityEngine;
public class LocalMessage {
	private static SocketCom.MessageHandler handler = null;
	public static ulong LocalPlayerNumber = 0;//本机玩家编号
	public static ushort LocalRoomNumber = 0;//本机所在房间号
	public static byte LocalIdentity = 1;//0房主，1普通玩家
	public static byte[][] Map = null;//地图
	public static SocketCom scom = new SocketCom();
	public static Thread comThread = new Thread(Loop);
	public static byte[] Bit_n = {0x00, 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80};

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
		int totalBytes = lineNumber * columnNumber * grid_bit / 8 + 6;
		int line, column, currentBit, lastShift = 0, currentByte = 6, tempShift = 0;
		byte temp = arg[currentByte++];
		for(line = 0; line < lineNumber; line++){
			column = 0;
			Map[line] = new byte[columnNumber];
			for(currentBit = 0; true; currentBit += grid_bit){
				if(currentBit + grid_bit > 8) {
					lastShift = 8 - currentBit;//保存已经获取到的位数
					tempShift = grid_bit - lastShift;//获取需要保留的位数
					Map[line][column] = GetLowBit(temp, lastShift);
					temp = arg[currentByte++];//读取下一字节
					currentBit = 0;
				}
				if(tempShift == 0) {
					currentBit += grid_bit;
					Map[line][column++] = GetLowBit(temp, grid_bit);
					temp >>= grid_bit;
				} else {
					currentBit += tempShift;
					Map[line][column] <<= lastShift;
					Map[line][column] |= GetLowBit(temp, tempShift);
					temp >>= tempShift;
				}
				lastShift = tempShift = 0;
				if(currentByte == totalBytes) return;
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
		for(int i = 1; i <= bit_num; i++){
			result |= Bit_n[i];
		}
		result &= arg;
		return result;
	}
}
