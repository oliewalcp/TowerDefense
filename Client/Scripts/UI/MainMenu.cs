using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class MainMenu : MonoBehaviour {
	public Button SingleGameButton;//单人游戏按钮
	public Button LANGameButton;//局域网游戏按钮
	public Button WANGameButton;//广域网游戏按钮
	public Button ExitGameButton;//退出游戏按钮
	public GameObject HallPanel;//游戏大厅面板
	// Use this for initialization
	void Start () {
		WANGameButton.onClick.AddListener(() => {
			
			this.gameObject.SetActive(false);
			HallPanel.SetActive(true);
		});
		ExitGameButton.onClick.AddListener(() => {
			Application.Quit();
		});
		LANGameButton.onClick.AddListener(() => {
			EditorUtility.DisplayDialog("未开放", "功能未开放", "关闭");
		});
	}
}
