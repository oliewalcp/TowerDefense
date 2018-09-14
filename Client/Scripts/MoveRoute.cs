using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoute : MonoBehaviour {
	public GameObject MapPanel;//地图区域
	private int CurrentLine = 0;//物体当前所在的格子（行）
	private int CurrentColumn = 0;//物体当前所在的格子（列）
	// Use this for initialization
	void Start () {
		transform.SetParent(MapPanel.transform);
		float small = LocalMessage.grid.height > LocalMessage.grid.width ? LocalMessage.grid.width : LocalMessage.grid.height;
		Vector3 scale = new Vector3(small, small, small);
		UIFunction.SetScale(gameObject, scale);
		UIFunction.SetMapPosition(gameObject, LocalMessage.StartGrid);
	}

	void OnEnable() {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisable() {
		Destroy(gameObject);
	}
}
