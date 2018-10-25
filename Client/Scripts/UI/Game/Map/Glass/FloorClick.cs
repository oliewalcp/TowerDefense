using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorClick : MonoBehaviour {

	private string current_tower = null;
	public GameObject TowerListScroll;
	public GameObject TowerMsgPanel;

	void Start() {
		TowerListScroll = GameObject.FindGameObjectWithTag("TowerList");
		GameObject temp = GameObject.Find("Info");
		TowerMsgPanel = temp.transform.Find("TowerMsg").gameObject;
	}

	private void BuildTower(string tower) {
		if(current_tower != null) {
			TowerListScroll.SetActive(false);
			TowerMsgPanel.SetActive(true);
			TowerMsgPanel.SendMessage("SetDisplayValue", current_tower);//CheckTowerMsg.cs
			return;
		}
		if(tower == "") return;
		tower = tower.Replace("(Clone)", "");
		float small = LocalMessage.grid.width > LocalMessage.grid.height ? LocalMessage.grid.height : LocalMessage.grid.width;
		small /= GameRunning.EnlargRatio;
		Vector3 scale = new Vector3(small, small, small);

		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/Tower/" + tower));
		go.transform.parent = transform;
		go.name = tower;
		UIFunction.SetScale(ref go, ref scale);
		go.transform.localEulerAngles = new Vector3(0, 0, 0);
		UIFunction.Set3DPosition(ref go, new Vector3(0, 0, -7));

		current_tower = tower;
	}
}
