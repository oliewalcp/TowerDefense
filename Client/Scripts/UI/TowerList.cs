using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerList : MonoBehaviour {

	private const string TowerPicturePath = "Prefabs/LittleTower/";
	private const string Template = "Prefabs/LittleTower/Template";

	// Use this for initialization
	void Start () {
		if(!GameRunning.TowerFileXml.IsOpen) {
			GameRunning.TowerFileXml.Open(GameRunning.tower_data);
		}
		LinkedList<string> node = GameRunning.TowerFileXml.GetChildren();
		Vector3 one = new Vector3(1, 1, 1);
		foreach(string str in node) {
			try{
				GameObject go = (GameObject)Instantiate(Resources.Load(TowerPicturePath + str));
				GameObject parent = (GameObject)Instantiate(Resources.Load(Template));
				Vector3 temp = parent.transform.localPosition;
				temp.z = 0;
				parent.transform.SetParent(transform);
				parent.SendMessage("AddImage", go);
				parent.transform.localPosition = temp;
				UIFunction.SetScale(ref parent, ref one);
			} catch {
				continue;
			}
		}
	}
}
