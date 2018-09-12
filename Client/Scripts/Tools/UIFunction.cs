using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UIFunction {
	public delegate void Call(ushort roomNumber);
	public delegate void SelectCall(GameObject go, string text);
	private static string[] Diff = {"入门", "简单", "中等", "困难", "极难"};
	/* 设置房间信息
	   param[gobject]:房间物体
	   param[room_number]:房间号
	   param[person]:人数
	   param[difficult]:难度
	   param[checkpoint]:关卡数
	   param[enter]:房间物体上的按钮文本
	   param[call]:房间物体上的按钮事件回调函数
	 */
	public static void SetRoomMessage(ref GameObject gobject, string room_number, string person, string difficulty, string checkpoint, string enter, Call call = null) {
		Text[] child = gobject.transform.GetComponentsInChildren<Text>();
		child[0].text = room_number;
		child[1].text = person;
		child[2].text = difficulty;
		child[3].text = checkpoint;
		Button bt = gobject.transform.GetComponentInChildren<Button>();
		Text t = bt.transform.GetComponentInChildren<Text>();
		t.text = enter;
		if(call != null) 
			bt.onClick.AddListener(() => {
				call(ushort.Parse(room_number));
			});
	}
	public static void SetRoomMessage(ref GameObject gobject, RoomMessage rm, Call call = null){
		SetRoomMessage(ref gobject, rm.GetRoomNumber().ToString(), rm.GetPersonNumber().ToString(), Diff[rm.GetDifficulty()], rm.GetCheckpoint().ToString(), "进入", call);
	}
	/* 设置y玩家信息
	   param[gobject]:玩家物体
	   param[name]:玩家名称
	   param[identity]:玩家的状态（身份）
	   param[call]:玩家物体上的下拉菜单选择事件的回调函数
	 */
	public static void SetPlayerMessage(GameObject gobject, string name, byte identity, SelectCall call = null){
		List<Text> uiText = new List<Text>();
		gobject.GetComponentsInChildren<Text>(true, uiText);
		Text nameText = uiText[0], statusText = uiText[1];
		//如果本机是房主
		if(LocalMessage.LocalIdentity == 0){
			Dropdown nameDropdown = gobject.GetComponentInChildren<Dropdown>();
			nameDropdown.captionText.text = name;
			List<string> option = new List<string>();
			option.Add(name);
			nameDropdown.AddOptions(option);
			nameDropdown.onValueChanged.AddListener((int index) => {
				call(gobject, nameDropdown.captionText.text);
			});
			nameText.gameObject.SetActive(false);
			nameDropdown.gameObject.SetActive(true);
		} else {
			nameText.text = name;
		}
		switch(identity){
			case 0:statusText.text = "房主";break;
			case 1:statusText.text = "准备";break;
			case 2:statusText.text = "未准备";break;
		}
	}

	public static void SetSize(ref GameObject go, Vector2 newSize) {
		RectTransform trans = go.GetComponent<RectTransform>();
		Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
	}
	public static void SetPosition(ref GameObject go, Vector2 newPosition) {
		Rect rect = go.GetComponent<RectTransform>().rect;
		rect.x = newPosition.x;
		rect.y = newPosition.y;
	}

	public static void Set3DPosition(ref GameObject go, Vector3 newPosition){
		go.transform.localPosition = newPosition;
	}

	public static void SetScale(ref GameObject go, ref Vector3 newScale){
		go.transform.localScale = newScale;
	}
}
