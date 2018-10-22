using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMonitor : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Tower t = TowerData.AllTower[gameObject.name];
		SphereCollider collider = GetComponent<SphereCollider>();
		collider.radius = TowerData.UnitRadius * t.attack_range;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.value = t.features.number;
		fm.time = t.features.time;
		fm.create = true;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);
		//Debug.Log("Enter");
	}
	void OnTriggerExit(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.create = false;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);
		//Debug.Log("Exit");
	}

	void OnTriggerStay(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.value = t.features.number;
		fm.time = t.features.time;
		fm.create = true;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);
	}
}
