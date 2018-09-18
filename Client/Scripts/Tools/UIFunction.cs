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
	/* 设置物体的大小
	   param[go]:需要改变的物体
	   param[newSize]:新的大小
	 */
	public static void SetSize(ref GameObject go, Vector2 newSize) {
		RectTransform trans = go.GetComponent<RectTransform>();
		Vector2 oldSize = trans.rect.size;
        Vector2 deltaSize = newSize - oldSize;
        trans.offsetMin = trans.offsetMin - new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
        trans.offsetMax = trans.offsetMax + new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));
	}
	/* 设置物体的二维坐标
	   param[go]:需要改变的物体
	   param[newPosition]:新的二维坐标
	 */
	public static void SetPosition(ref GameObject go, Vector2 newPosition) {
		Rect rect = go.GetComponent<RectTransform>().rect;
		rect.x = newPosition.x;
		rect.y = newPosition.y;
	}
	/* 设置物体的三维坐标
	   param[go]:需要改变的物体
	   param[newPosition]:新的三维坐标
	 */
	public static void Set3DPosition(ref GameObject go, Vector3 newPosition){
		go.transform.localPosition = newPosition;
	}
	/* 设置物体在地图上的像素位置
	   param[go]:需要改变的物体
	   param[gp]:地图的位置
	 */
	public static void SetMapPosition(GameObject go, GPosition gp) {
		Vector3 newPosition = new Vector3();
		newPosition.z = -8.5f;
		newPosition.x = LocalMessage.grid.width / 2 + gp.column * LocalMessage.grid.width;
		newPosition.y = - (LocalMessage.grid.height / 2 + gp.line * LocalMessage.grid.height);
		go.transform.localPosition = newPosition;
	}
	/* 获取指定地图位置的实际像素位置
	   param[gp]:地图位置
	   return:像素位置
	 */
	public static Vector2 GetPixelPosition(GPosition gp) {
		Vector2 newPosition = new Vector2();
		newPosition.x = LocalMessage.grid.width / 2 + gp.column * LocalMessage.grid.width;
		newPosition.y = - (LocalMessage.grid.height / 2 + gp.line * LocalMessage.grid.height);
		return newPosition;
	}
	/* 获取物体在地图上的位置
	   param[go]:需要获取位置的物体
	   param[delta]:偏差值
	 */
	public static GPosition GetMapPosition(GameObject go, float line_delta = 0, float column_delta = 0) {
		Vector3 pos = go.transform.localPosition;
		GPosition gp = new GPosition();
		float tempCol = (pos.x - LocalMessage.grid.width / 2) / LocalMessage.grid.width;
		float tempLin = (pos.y + LocalMessage.grid.height / 2) / ( - LocalMessage.grid.height);
		gp.column = (byte)(tempCol + line_delta);
		gp.line = (byte)(tempLin + column_delta);
		return gp;
	}
	/* 设置物体大小倍率
	   param[go]:需要改变的物体
	   param[newScale]:新的大小倍率
	 */
	public static void SetScale(ref GameObject go, ref Vector3 newScale){
		go.transform.localScale = newScale;
	}
	public static void SetScale(GameObject go, Vector3 newScale){
		go.transform.localScale = newScale;
	}
	//清空子组件
	public static void ClearChild(ref GameObject go) {
		Transform temp = go.transform;
		int num = temp.childCount;
		for(int i = 0; i < num; i++)
			MonoBehaviour.Destroy(temp.GetChild(0).gameObject);
	}
}
