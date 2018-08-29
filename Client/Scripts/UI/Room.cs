using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Room : MonoBehaviour {
	public Dropdown Difficult;
	public Dropdown CheckPoint;
	public InputField PlayerGoldInputField;
	public InputField TowerAttackInputField;
	public InputField TowerSpeedInputField;
	public InputField TowerBuildCostInputField;
	public InputField MonsterHipPointInputField;
	public InputField MonsterDefenseInputField;
	public InputField MonsterSpeedInputField;
	public Text PlayerGoldText;
	public Text TowerAttackText;
	public Text TowerSpeedText;
	public Text TowerBuildCostText;
	public Text MonsterHipPointText;
	public Text MonsterDefenseText;
	public Text MonsterSpeedText;
	public Dropdown PlayerGoldDropdown;
	public Dropdown TowerAttackDropdown;
	public Dropdown TowerSpeedDropdown;
	public Dropdown TowerBuildCostDropdown;
	public Dropdown MonsterHipPointDropdown;
	public Dropdown MonsterDefenseDropdown;
	public Dropdown MonsterSpeedDropdown;
	public Button ExitRoomButton;//退出房间按钮
	public GameObject PlayerListPanel;//玩家列表面板
	public GameObject HallPanel;//游戏大厅界面
	public GameObject GameMapPanel;//游戏进行界面
	private byte OtherReady = 0;//已准备的玩家数量
	private Dictionary<ulong, GameObject> PlayerContainer = new Dictionary<ulong, GameObject>();//玩家编号——玩家物体
	private Dictionary<GameObject, ulong> ObjectContainer = new Dictionary<GameObject, ulong>();//玩家物体——玩家编号

	void OnEnable() {
		LocalMessage.SetHandler(MessageHandler);
		InputField[] tempIF = {PlayerGoldInputField, TowerAttackInputField, TowerSpeedInputField, TowerBuildCostInputField, MonsterDefenseInputField, MonsterHipPointInputField, MonsterSpeedInputField};
		Dropdown[] tempD = {PlayerGoldDropdown, TowerAttackDropdown, TowerSpeedDropdown, TowerBuildCostDropdown, MonsterDefenseDropdown, MonsterHipPointDropdown, MonsterSpeedDropdown, Difficult, CheckPoint};
		if(LocalMessage.LocalIdentity != 0){
			foreach(InputField field in tempIF)
				field.enabled = false;
			foreach(Dropdown d in tempD)
				d.enabled = false;
		} else {
			foreach(InputField field in tempIF)
				field.enabled = true;
			foreach(Dropdown d in tempD)
				d.enabled = true;
		}
		byte[] msg = new byte[8];
		SmallTools.CopyArray(msg, SmallTools.ToByteArray(LocalMessage.LocalPlayerNumber)); 
		LocalMessage.scom.SendMessage(MsgType.PlayerMsgInRoom, msg);
	}

	void OnDisable(){
		GameObject[] child = PlayerListPanel.transform.GetComponentsInChildren<GameObject>();
		foreach (GameObject go in child){
			Destroy(go);
		}
		PlayerContainer.Clear();
		ObjectContainer.Clear();
		LocalMessage.SetHandler(null);
	}

	// Use this for initialization
	void Start () {
		PlayerGoldDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(PlayerGoldDropdown, PlayerGoldText);});
		TowerAttackDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(TowerAttackDropdown, TowerAttackText);});
		TowerSpeedDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(TowerSpeedDropdown, TowerSpeedText);});
		TowerBuildCostDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(TowerBuildCostDropdown, TowerBuildCostText);});
		MonsterHipPointDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(MonsterHipPointDropdown, MonsterHipPointText);});
		MonsterDefenseDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(MonsterDefenseDropdown, MonsterDefenseText);});
		MonsterSpeedDropdown.onValueChanged.AddListener((int index) => {DropdownSelect(MonsterSpeedDropdown, MonsterSpeedText);});

		PlayerGoldInputField.onValueChanged.AddListener((string t) => {UserInput(t, PlayerGoldText);});
		TowerAttackInputField.onValueChanged.AddListener((string t) => {UserInput(t, TowerAttackText);});
		TowerSpeedInputField.onValueChanged.AddListener((string t) => {UserInput(t, TowerSpeedText);});
		TowerBuildCostInputField.onValueChanged.AddListener((string t) => {UserInput(t, TowerBuildCostText);});
		MonsterHipPointInputField.onValueChanged.AddListener((string t) => {UserInput(t, MonsterHipPointText);});
		MonsterDefenseInputField.onValueChanged.AddListener((string t) => {UserInput(t, MonsterDefenseText);});
		MonsterSpeedInputField.onValueChanged.AddListener((string t) => {UserInput(t, MonsterSpeedText);});
	}
	//房主通过下拉菜单改变地图信息
	private void DropdownSelect(Dropdown dropdown, Text displayText){
		string currentText = dropdown.captionText.text;
		UserInput(currentText, displayText);
	}
	//房主自己输入数值改变地图信息
	private void UserInput(string t, Text displayText) {
		displayText.text = t;
		LocalMessage.scom.SendMessage(MsgType.ChangeMapMsg, GetRoomSetterMsg());
	}

	private void MessageHandler(byte[] msg){
		switch(msg[1]){
			case MsgType.ReadyMsg:ReadyMessage(msg);break;
			case MsgType.EnterRoom:EnterRoomMessage(msg);break;
			case MsgType.ExitRoom:ExitRoomMessage(msg);break;
			case MsgType.ChangeRoomMsg:ChangeRoomMsgMessage(msg);break;
			case MsgType.ChangeMapMsg:SetRoomSetterMessage(msg);break;
			case MsgType.PlayerMsgInRoom:GetPlayerMsgMessage(msg);break;
		}
	}

	//获取房间中地图所有加成信息
	private byte[] GetRoomSetterMsg(){
		Text[] displayText = {PlayerGoldText, TowerAttackText, TowerSpeedText, TowerBuildCostText, MonsterHipPointText, MonsterDefenseText, MonsterSpeedText};
		byte[] result = new byte[displayText.Length * 4];
		for(int i = 0; i < displayText.Length; i++){
			int start = i * 4;
			int end = (i + 1) * 4;
			byte[] array = GetOptionByte(displayText[i]);
			for(int j = start; j < end; j++)
				result[j] = array[j - start];
		}
		return result;
	}
	//获取地图某一项加成信息
	//return:返回4个字节
	private byte[] GetOptionByte(Text displayText){
		byte[] result = new byte[4];
		int displayNumber = int.Parse(displayText.text);
		SmallTools.CopyArray(result, SmallTools.ToByteArray(displayNumber));
		return result;
	}
	//地图信息更改事件
	private void SetRoomSetterMessage(byte[] msg, int startIndex = 2){
		Text[] displayText = {PlayerGoldText, TowerAttackText, TowerSpeedText, TowerBuildCostText, MonsterHipPointText, MonsterDefenseText, MonsterSpeedText};
		for(int i = 0; i < displayText.Length; i++){
			int temp = BitConverter.ToInt32(msg, startIndex + i * 4);
			displayText[i].text = temp.ToString();
		}
	}
	//房间信息更改事件
	private void ChangeRoomMsgMessage(byte[] msg, int startIndex = 2){
		uint roomMsg = BitConverter.ToUInt32(msg, startIndex);
		RoomMessage rm = new RoomMessage(roomMsg);
		ushort roomNum = rm.GetRoomNumber();
		if(roomNum != LocalMessage.LocalRoomNumber) return;
		Difficult.captionText.text = rm.GetDifficulty().ToString();
		CheckPoint.captionText.text = rm.GetCheckpoint().ToString();
	}
	//准备或开始消息
	private void ReadyMessage(byte[] msg){
		switch(msg[10]){
			case 0:OtherReady--;break;
			case 1:OtherReady++;break;
			case 2:{
				ulong playerNumber = BitConverter.ToUInt64(msg, 2);
				this.gameObject.SetActive(false);
				GameMapPanel.SetActive(true);
			}break;
		}
	}
	//进入房间消息
	private void EnterRoomMessage(byte[] msg){
		ushort roomNum = BitConverter.ToUInt16(msg, 2);
		if(roomNum != LocalMessage.LocalRoomNumber) return;
		ulong playerNum = BitConverter.ToUInt64(msg, 4);
		string playerName = BitConverter.ToString(msg, 12);
		GameObject go = CreatePlayerBar(playerName, 1);
		PlayerContainer.Add(playerNum, go);
		ObjectContainer.Add(go, playerNum);
	}
	//退出房间消息
	private void ExitRoomMessage(byte[] msg){
		ushort roomNumber = BitConverter.ToUInt16(msg, 10);
		if(roomNumber != LocalMessage.LocalRoomNumber) return;
		ulong playerNumber = BitConverter.ToUInt64(msg, 2);
		ObjectContainer.Remove(PlayerContainer[playerNumber]);
		PlayerContainer.Remove(playerNumber);
		Destroy(PlayerContainer[playerNumber]);
		if(playerNumber == LocalMessage.LocalPlayerNumber) 
		 	ExitRoom();
	}
	//退出房间
	private void ExitRoom(){
		LocalMessage.SetHandler(null);
		this.gameObject.SetActive(false);
		HallPanel.gameObject.SetActive(true);
	}
	//获取到房间内所有玩家的信息（附带地图和游戏信息）
	private void GetPlayerMsgMessage(byte[] msg){
		int i, j;
		for(i = 3, j = 0; j < msg[2]; j++){
			GameObject go = CreatePlayerBar(BitConverter.ToString(msg, i + 9, 18), msg[i]);
			ulong tempNum = BitConverter.ToUInt64(msg, i + 1);
			PlayerContainer.Add(tempNum, go);
			ObjectContainer.Add(go, tempNum);
			i += 19;
		}
		ChangeRoomMsgMessage(msg, i);
		SetRoomSetterMessage(msg, i + 4);
	}
	//创建一个玩家条
	private GameObject CreatePlayerBar(string name, byte identity){
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/PlayerBar"));
		UIFunction.SetPlayerMessage(go, name, identity, KickPlayer);
		go.transform.SetParent(PlayerListPanel.transform);
		go.transform.localScale = new Vector3(1, 1, 1);
		return go;
	}
	//房主踢人
	private void KickPlayer(GameObject go, string text){
		if(text.Equals("打开")){
			byte[] message = new byte[10];
			SmallTools.CopyArray(message, SmallTools.ToByteArray(ObjectContainer[go]));
			SmallTools.CopyArray(message, SmallTools.ToByteArray(LocalMessage.LocalRoomNumber), 8);
			//向被踢玩家发送离开房间的消息
			LocalMessage.scom.SendMessage(MsgType.ExitRoom, message);
		}
	}
}
