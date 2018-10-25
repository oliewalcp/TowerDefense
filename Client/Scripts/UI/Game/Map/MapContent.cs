using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapContent : MonoBehaviour {
	public static string CurrentSelectTower = "";
	public static GameObject ClickedTower = null;
	private readonly LayerMask Mask = 3 << 8;
	private const int MaxDistance = 600;
	// Update is called once per frame
	void Update () {
		if(Input.GetMouseButtonDown(0)) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit rayhit;
			if(Physics.Raycast(ray, out rayhit, MaxDistance, Mask)) {
				GameObject go = rayhit.collider.gameObject;
				go.SendMessage("BuildTower", CurrentSelectTower, SendMessageOptions.DontRequireReceiver);//FloorClick.cs
				if((ClickedTower != null) && (!Input.GetKey(KeyCode.LeftShift)) && (!Input.GetKey(KeyCode.RightShift))) {
					CurrentSelectTower = "";
					ClickedTower.SendMessage("OnPointerClick", SendMessageOptions.DontRequireReceiver);//SelectTower.cs
					ClickedTower = null;
				}
			}
		}
	}
}
