using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SelectTower : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
	
	private GameObject child = null;
	private const string NAME = "name";
	private const string DETAIL = "detail";
	private bool enter = false;
	private static readonly Color HightLight = new Color(255, 0, 255);
	private static readonly Color Normal = new Color(255, 255, 255);

	public void OnPointerDown(PointerEventData eventData) {
		if(MapContent.ClickedTower != null) {
			MapContent.ClickedTower.SendMessage("OnPointerClick");
		}
		MapContent.CurrentSelectTower = child.name;
		MapContent.ClickedTower = gameObject;
		GetComponent<Image>().color = HightLight;
	}

	public void OnPointerClick() {
		GetComponent<Image>().color = Normal;
	}

	public void OnPointerEnter(PointerEventData eventData) {
		enter = true;
	}

	public void OnPointerExit(PointerEventData eventData) {
		enter = false;
	}

	void OnGUI() {
		if(enter) {
			Rect rect = new Rect(Input.mousePosition.x + 30, Screen.height - Input.mousePosition.y, 100, 60);
			GameRunning.TowerFileXml.BeginParentNode(child.name.Replace("(Clone)", ""));
			string display = GameRunning.TowerFileXml.GetValue(NAME) + "  " + GameRunning.TowerFileXml.GetValue(DETAIL);
			GameRunning.TowerFileXml.EndParentNode();
			GUI.Label(rect, display);
		}
	}

	private void AddImage(GameObject image) {
		image.transform.SetParent(transform);
		child = image;
	}
}