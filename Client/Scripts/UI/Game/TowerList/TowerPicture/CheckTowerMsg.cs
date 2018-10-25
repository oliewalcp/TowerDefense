using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckTowerMsg : MonoBehaviour {
	public GameObject TowerListPanel;
	public Text NameText;
	public Text AttackValueText;
	public Text AttackSpeedText;
	public Text AttackTypeText;
	public Text AttackRangeText;
	public Button CloseButton;
	private readonly string[] attack_type = {"", "普通", "穿刺", "魔法", "工程", "混乱", "粉碎"};

	// Use this for initialization
	void Start () {
		CloseButton.onClick.AddListener(CloseButtonEvent);
	}

	private void CloseButtonEvent() {
		gameObject.SetActive(false);
		TowerListPanel.SetActive(true);
	}
	private void SetDisplayValue(string name) {
		Tower t = TowerData.AllTower[name];
		NameText.text = "塔名称：" + t.name;
		if(name == "DefenseDownTower" || name == "SpeedDownTower" || name == "AttackUpTower" || name == "SpeedUpTower")
			AttackValueText.text = "攻击力：" + t.features.number;
		else AttackValueText.text = "攻击力：" + t.damage.ToString();
		if(t.attack_interval != 0)
			AttackSpeedText.text = "攻击速度：" +  (1.0f / ((float)t.attack_interval / 1000.0f)).ToString();
		else AttackSpeedText.text = "攻击速度：无";
		AttackTypeText.text = "攻击类型：" + attack_type[t.attack_type];
		AttackRangeText.text = "攻击范围：" + t.attack_range.ToString();
	}
}
