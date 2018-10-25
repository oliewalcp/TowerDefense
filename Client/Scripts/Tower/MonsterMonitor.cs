using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMonitor : MonoBehaviour {
	private HashSet<GameObject> InnerMonster = new HashSet<GameObject>();//在攻击范围内的怪物
	private static readonly HashSet<string> AssistTower = new HashSet<string>();
	private GameObject CurrentAttack = null;//当前正在攻击的怪物
	// Use this for initialization
	void Start () {
		if(AssistTower.Count < 4) {
			AssistTower.Add("DefenseDownTower");
			AssistTower.Add("SpeedDownTower");
			AssistTower.Add("AttackUpTower");
			AssistTower.Add("SpeedUpTower");
		}
		Tower t = TowerData.AllTower[gameObject.name];
		SphereCollider collider = GetComponent<SphereCollider>();
		collider.radius = TowerData.UnitRadius * t.attack_range;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		InnerMonster.Add(other.gameObject);
	}
	void OnTriggerExit(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.create = false;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);
		InnerMonster.Remove(other.gameObject);
		//如果塔当前正在攻击该物体
		if(other.gameObject == CurrentAttack) {
			CurrentAttack = GetClosestMonster();
		}
	}

	void OnTriggerStay(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.value = t.features.number;
		fm.time = t.features.time;
		fm.create = true;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);
		if(CurrentAttack != null) {
			Shot();
		}
	}

	GameObject GetClosestMonster() {
		Vector3 tower_pos = transform.position;
		GameObject closest = null;
		float min = -1;
		foreach(GameObject go in InnerMonster) {
			float temp = UIFunction.GetDistance(go.transform.position, tower_pos);
			if(min > temp || min == -1) {
				min = temp;
				closest = go;
			}
		}
		return closest;
	}

	private void Shot() {

	}
}
