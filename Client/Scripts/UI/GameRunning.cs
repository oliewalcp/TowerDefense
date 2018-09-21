using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRunning : MonoBehaviour {

	public GameObject MapPanel;//地图区域

	private readonly string[] Floor = {"Prefabs/ground", "Prefabs/glass", "Prefabs/stone"};
	public const float MapWidth = 668;
	public const float MapHeight = 668;
	void Start() {
	}
	// Use this for initialization
	void OnEnable () {
		byte[] testMap = new byte[]{0, 0, 0, 0, 20, 20, 
								3, 0, 1, 18,//地图路线的起点和终点
								//地图开始
								85, 85, 85, 85,  85,
								85, 85, 85, 85,  69,
								85, 85, 85, 85,  69,
								64, 5,  0,  0,   69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								5,  64, 85, 21,  69,
								85, 85, 85, 21,  69,
								85, 85, 85, 21,  69,
								1,  0,  0,  0,   69,
								81, 85, 85, 85,  69,
								81, 85, 85, 85,  69,
								1,  0,  0,  0,   64,
								85, 85, 85, 85,  85,
								85, 85, 85, 85,  85,
								//地图结束
								0, 0, 0, 0, 0, 0, 0, 0};
		LocalMessage.SetMap(testMap);
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
				UIFunction.Set3DPosition(ref go, new Vector3(halfHeight + j * height, - (halfWidth + i * width), -0.09f));
			}
		}
		LocalMessage.grid.width = width;
		LocalMessage.grid.height = height;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisable(){
		UIFunction.ClearChild(ref MapPanel);
		LocalMessage.SetHandler(null);
	}
}
