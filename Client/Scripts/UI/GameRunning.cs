using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunning : MonoBehaviour {

	public GameObject MapPanel;//地图区域

	private string[] Floor = {"Prefabs/ground", "Prefabs/glass", "Prefabs/stone"};
	private const float MapWidth = 668;
	private const float MapHeight = 668;

	void Start() {

	}
	// Use this for initialization
	void OnEnable () {
		LocalMessage.Map = new byte[10][];
		for(int i = 0; i < 10; i++){
			LocalMessage.Map[i] = new byte[10];
		}
		LoadMap();
	}
	//加载地图
	private void LoadMap(){
		int line = LocalMessage.Map.Length, column = LocalMessage.Map[0].Length;
		float width = MapWidth / column, height = MapHeight / line, halfWidth = width / 2, halfHeight = height / 2;

		Vector3 scale = new Vector3(width, height, 0.01f);
		for(int i = 0; i < line; i++){
			for(int j = 0; j < column; j++){
				GameObject go = (GameObject)Instantiate(Resources.Load(Floor[LocalMessage.Map[i][j]]));
				go.transform.SetParent(MapPanel.transform);
				UIFunction.SetScale(ref go, ref scale);
				UIFunction.Set3DPosition(ref go, new Vector3(halfWidth + i * width, - (halfHeight + j * height), -0.09f));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisable(){
		GameObject[] child = MapPanel.GetComponentsInChildren<GameObject>();
		Debug.Log(child.Length);
		foreach (GameObject go in child){
			Destroy(go);
		}
		LocalMessage.SetHandler(null);
	}
}
