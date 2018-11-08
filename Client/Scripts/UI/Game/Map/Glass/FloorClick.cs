using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorClick : MonoBehaviour {

	private string current_tower = null;
	public GameObject TowerListScroll;
	public GameObject TowerMsgPanel;
	private GameObject TowerObject;

	void Start() {
		TowerListScroll = GameObject.FindGameObjectWithTag("TowerList");
		GameObject temp = GameObject.Find("Info");
		TowerMsgPanel = temp.transform.Find("TowerMsg").gameObject;
	}
	//MapContent.cs调用
	private void BuildTower(string tower) {
		if(current_tower != null) {
			TowerListScroll.SetActive(false);
			TowerMsgPanel.SetActive(true);
			TowerMsgPanel.SendMessage("SetDisplayValue", current_tower);//CheckTowerMsg.cs
			TowerMsgPanel.SendMessage("SetCurrentPos", gameObject);//CheckTowerMsg.cs
			return;
		}
		if(tower == "") return;
		tower = tower.Replace("(Clone)", "");
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/Tower/" + tower));
		Vector3 temp = go.transform.localScale;
		go.transform.parent = transform.parent;
		//设置缩放度
		temp.x *= GameRunning.EnlargRatio;
		temp.y *= GameRunning.EnlargRatio;
		temp.z *= GameRunning.EnlargRatio;
		go.transform.localScale = temp;
		go.name = tower;
		//设置位置
		Vector3 target_pos = transform.localPosition;
		target_pos.z = -10;
		go.transform.localPosition = target_pos;
		current_tower = tower;
		TowerObject = go;
	}
	//CheckTowerMsg.cs调用
	private void DestroyTower() {
		Destroy(TowerObject);
		current_tower = null;
	}
}
