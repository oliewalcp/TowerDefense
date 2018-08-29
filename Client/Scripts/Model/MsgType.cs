
public class MsgType {
	public const byte DissolveRoom = 150;//解散房间
	public const byte PlayerNumber = 0;//玩家编号
	public const byte RoomMsgList = 1;//房间信息列表
	public const byte MyRoomNumber = 2;//创建房间
	public const byte ReadyMsg = 51;//准备消息
	public const byte EnterRoom = 52;//进入房间
	public const byte ExitRoom = 53;//离开房间
	public const byte PlayerMsgInRoom = 54;//房间内玩家的信息列表
	public const byte ChangeRoomMsg = 55;//修改房间信息
	public const byte ChangeMapMsg = 56;//修改地图信息
	public const byte AttackSignal = 104;//攻击事件
	public const byte Synchronous = 255;//同步
}