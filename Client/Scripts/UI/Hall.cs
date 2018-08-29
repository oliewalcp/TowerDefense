using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using System.Threading;
using System;
using System.Text;

public class Hall : MonoBehaviour {
	public GameObject MainMenuPanel;//主菜单面板
	public GameObject RoomListPanel;//房间列表面板
	public GameObject RoomPanel;//房间面板
	public InputField MyName;//本机的玩家昵称
	public Button BackButton;//返回按钮
	public Button RefreshButton;//刷新按钮
	public Button CreateRoomButton;//创建房间按钮
	private Dictionary<GameObject, ushort> RoomContainer1 = new Dictionary<GameObject, ushort>();//组件——房间号
	private Dictionary<ushort, GameObject> RoomContainer2 = new Dictionary<ushort, GameObject>();//房间号——组件
	// Use this for initialization
	void OnEnable() {
		if(LocalMessage.scom == null){
			LocalMessage.scom = new SocketCom(); 
			if(!LocalMessage.scom.Initialize()) {
				EditorUtility.DisplayDialog("网络连接失败", "无法连接到服务器", "返回主菜单");
				BackMainMenu();
				return;
			}
			LocalMessage.SetHandler(MessageHandle);
		}
		else LocalMessage.scom.SendMessage(MsgType.RoomMsgList);
		//EditorUtility.DisplayDialog("打开", "进入大厅", "关闭");
	}
	void OnDisable(){
		RoomContainer1.Clear();
		RoomContainer2.Clear();
		GameObject[] child = RoomPanel.transform.GetComponentsInChildren<GameObject>();
		foreach (GameObject go in child){
			Destroy(go);
		}
		//EditorUtility.DisplayDialog("关闭", "离开大厅", "关闭");
	}
	void Start () {
		string[] str = new string[]{"按钮1", "按钮2"};
		int i = 0;
		GameObject temp_go = CreateRoomBar("1111", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了1111", str[i], "关闭");
			i = 1 - i;
		});
		CreateRoomBar("2222", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了2222", "2222成功了", "关闭");
		});
		CreateRoomBar("3333", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了3333", "3333成功了", "关闭");
		});
		CreateRoomBar("4444", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了4444", "4444成功了", "关闭");
		});
		CreateRoomBar("5555", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了5555", "5555成功了", "关闭");
		});
		CreateRoomBar("6666", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了6666", "6666成功了", "关闭");
		});
		CreateRoomBar("7777", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了7777", "7777成功了", "关闭");
		});
		CreateRoomBar("8888", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了8888", "8888成功了", "关闭");
		});
		CreateRoomBar("9999", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了9999", "9999成功了", "关闭");
		});
		CreateRoomBar("1010", "1", "困难", "25", "加入", (ushort t) => {
			EditorUtility.DisplayDialog("点击了1010", "1010成功了", "关闭");
		});
		BackButton.onClick.AddListener(BackMainMenu);
		CreateRoomButton.onClick.AddListener(CreateRoomListener);
	}
	//创建一个房间条
	private GameObject CreateRoomBar(string room_number, string person, string difficulty, string checkpoint, string enter, UIFunction.Call call = null){
		if(call == null) call = RoomClick;
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/RoomBar"));
		UIFunction.SetRoomMessage(ref go, room_number, person, difficulty, checkpoint, enter, call);
		go.transform.SetParent(RoomListPanel.transform);
		//UIFunction.SetPosition(ref go, new Vector2(0, 0));
		go.transform.localScale = new Vector3(1, 1, 1);
		return go;
	}
	//创建房间事件
	private void CreateRoomListener(){
		byte[] msg = new byte[26];
		SmallTools.CopyArray(msg, SmallTools.ToByteArray(LocalMessage.LocalPlayerNumber));
		byte[] name = Encoding.ASCII.GetBytes(MyName.text);
		SmallTools.CopyArray(msg, name, 8);
		LocalMessage.scom.SendMessage(MsgType.MyRoomNumber, msg);
	}

	private void MessageHandle(byte[] message){
		switch(message[1]){
			case MsgType.PlayerNumber:GetMyPlayerNumber(BitConverter.ToUInt64(message, 2));break;
			case MsgType.RoomMsgList:GetRoomMsgList(message);break;
			case MsgType.DissolveRoom:GetDissolveRoom(BitConverter.ToUInt16(message, 2));break;
			case MsgType.EnterRoom:GetEnterRoomMsg(message);break;
			case MsgType.MyRoomNumber:GetMyRoomNumber(BitConverter.ToUInt16(message, 2));break;
		}
	}
	private void GetMyRoomNumber(ushort number){
		if(number == 0){
			EditorUtility.DisplayDialog("创建房间失败", "房间数量已满，无法创建房间", "关闭");
			return;
		}
		LocalMessage.LocalRoomNumber = number;
		LocalMessage.LocalIdentity = 0;
		this.gameObject.SetActive(false);
		RoomPanel.SetActive(true);
	}
	/* 接收到进入房间的消息
	   param[message]:包含版本号和类别号
	 */
	private void GetEnterRoomMsg(byte[] message){
		ulong player_id = BitConverter.ToUInt64(message, 4);
		if(LocalMessage.LocalPlayerNumber != player_id) return;
		ushort room_id = BitConverter.ToUInt16(message, 2);
		LocalMessage.LocalRoomNumber = room_id;
		this.gameObject.SetActive(false);
		RoomPanel.SetActive(true);
	}
	/* 接收到本机的玩家编号
	   param[number]:玩家编号
	 */
	private void GetMyPlayerNumber(ulong number){
		if(number == 0){
			EditorUtility.DisplayDialog("连接服务器失败", "服务器已满", "关闭");
			BackMainMenu();
		}
		LocalMessage.LocalPlayerNumber = number;
		LocalMessage.scom.SendMessage(MsgType.RoomMsgList);//获取房间列表
	}
	/* 接收到房间解散的消息
	   param[number]:房间号
	 */
	private void GetDissolveRoom(ushort number){
		GameObject temp_go = RoomContainer2[number];
		RoomContainer2.Remove(number);
		RoomContainer1.Remove(temp_go);
		Destroy(temp_go);
	}
	/* 获取到房间信息列表
	   param[list]:包含版本号和类别号
	 */
	private void GetRoomMsgList(byte[] list){
		int i = 2;
		for(i = 2; i < list.Length - 4; i += 4){
			uint temp_num = BitConverter.ToUInt32(list, i);
			if(temp_num == 0) break;
			RoomMessage temp_room = new RoomMessage(temp_num);
			ushort temp_room_num = temp_room.GetRoomNumber();
			GameObject temp_go = null;
			if(RoomContainer2.ContainsKey(temp_room_num)){
				temp_go = RoomContainer2[temp_room_num];
				UIFunction.SetRoomMessage(ref temp_go, temp_room);
			} else {
				//创建新的物体
				temp_go = (GameObject)Instantiate(Resources.Load("Prefabs/RoomBar"));
				temp_go.transform.parent = RoomListPanel.transform;
				UIFunction.SetPosition(ref temp_go, new Vector2(0, 0));
				temp_go.transform.localScale = new Vector3(1, 1, 1);
				UIFunction.SetRoomMessage(ref temp_go, temp_room, RoomClick);
				RoomContainer2.Add(temp_room_num, temp_go);
				RoomContainer1.Add(temp_go, temp_room_num);
			}
		}
	}
	//返回主菜单
	private void BackMainMenu(){
		LocalMessage.scom.Close();
		this.gameObject.SetActive(false);
		MainMenuPanel.SetActive(true);
	}
	//点击加入房间
	private void RoomClick(ushort roomNumber){
		byte[] msg = new byte[28]; 
		SmallTools.CopyArray(msg, SmallTools.ToByteArray(roomNumber));
		SmallTools.CopyArray(msg, SmallTools.ToByteArray(LocalMessage.LocalPlayerNumber), 2);
		byte[] name = Encoding.ASCII.GetBytes(MyName.text);
		SmallTools.CopyArray(msg, name, 10);
		LocalMessage.scom.SendMessage(MsgType.EnterRoom, msg);
	}
}
