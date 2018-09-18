using System.Threading;
using System;
using UnityEngine;
using System.Collections;

public class LocalMessage {
	private static SocketCom.MessageHandler handler = null;
	public static ulong LocalPlayerNumber = 0;//本机玩家编号
	public static ushort LocalRoomNumber = 0;//本机所在房间号
	public static byte LocalIdentity = 1;//0房主，1普通玩家
	public static byte[][] Map = null;//地图
	public static ArrayList MonsterRoute = null;
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
		StartGrid.line = arg[6];
		StartGrid.column = arg[7];
		EndGrid.line = arg[8];
		EndGrid.column = arg[9];
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
		SetMonsterRoute();
	}
	//设置怪物的路线
	private static void SetMonsterRoute() {
		byte currentDirection = GetNextGridDirection(StartGrid, MoveRoute.NONE);
		MonsterRoute = new ArrayList();
		GPosition currentPosition = new GPosition(), lastPosition = new GPosition(){
				line = MoveRoute.NONE, column = MoveRoute.NONE
			};
		for(currentPosition.CopyFrom(StartGrid); true;) {
			//判断lastPosition是在currentPosition的哪个方向
			byte tempDirection = currentPosition.Adjacent(lastPosition);
			if(currentDirection != tempDirection) {
				currentDirection = tempDirection;
				MonsterRoute.Add(new Position(currentPosition, UIFunction.GetPixelPosition(currentPosition), currentDirection));
				if(currentPosition.Equals(EndGrid)) 
					break;
			}
			lastPosition.CopyFrom(currentPosition);
			currentDirection = GetNextGridDirection(currentPosition, currentDirection);
			switch(currentDirection) {
				case MoveRoute.UP: currentPosition.line -= 1;break;
				case MoveRoute.DOWN: currentPosition.line += 1;break;
				case MoveRoute.LEFT: currentPosition.column -= 1;break;
				case MoveRoute.RIGHT: currentPosition.column += 1;break;
			}
		}
	}
	/* 获取下一格的方向
	   param[currentPosition]:当前位置
	   param[currentDirection]:当前方向
	   return:下一格在当前格的方向
	 */
	public static byte GetNextGridDirection(GPosition currentPosition, byte currentDirection) {
		byte result = MoveRoute.NONE;
		if(currentDirection != MoveRoute.DOWN && currentPosition.line >= 1 && Map[currentPosition.line - 1][currentPosition.column] == 0)
			result = MoveRoute.UP;
		else if(currentDirection != MoveRoute.LEFT && currentPosition.column + 1 < Map[0].Length && Map[currentPosition.line][currentPosition.column + 1] == 0)
			result = MoveRoute.RIGHT;
		else if(currentDirection != MoveRoute.UP && currentPosition.line + 1 < Map.Length && Map[currentPosition.line + 1][currentPosition.column] == 0)
			result = MoveRoute.DOWN;
		else if(currentDirection != MoveRoute.RIGHT && currentPosition.column >= 1 && Map[currentPosition.line][currentPosition.column - 1] == 0)
			result = MoveRoute.LEFT;
		return result;
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
