using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FloorClick : MonoBehaviour, IPointerDownHandler {
	static public string CurrentSelectTower = null;//当前选择的塔的名称
	private string current_tower = null;//当前已建造的塔
	public void OnPointerDown(PointerEventData eventData) {
		string tower = CurrentSelectTower;
		if(!Input.GetKeyDown(KeyCode.LeftShift) && !Input.GetKeyDown(KeyCode.RightShift)) {
			CurrentSelectTower = null;
		}
		if(current_tower != null) return;
		else SendMessage("SetTower", tower);
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/Tower/" + tower));
		Debug.Log("click");
	}
}
