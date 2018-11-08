using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMonitor : MonoBehaviour {
	private HashSet<GameObject> InnerMonster = new HashSet<GameObject>();//在攻击范围内的怪物
	private static readonly HashSet<string> AssistTower = new HashSet<string>();//辅助塔
	private HashSet<GameObject> ClosestMonster = new HashSet<GameObject>();//最近的怪物
	private int AttackNumber = 0;//同时攻击的数量
	private Coroutine AttackThread;
	private int AttackInterval;
	private const string BulletPath = "Prefabs/TowerBullet/";
	// Use this for initialization
	void Start () {
		if(AssistTower.Count < 4) {
			AssistTower.Add("DefenseDownTower");
			AssistTower.Add("SpeedDownTower");
			AssistTower.Add("AttackUpTower");
			AssistTower.Add("SpeedUpTower");
		}
		Tower t = TowerData.AllTower[gameObject.name];
		AttackInterval = (int)t.attack_interval;
		if((t.features.features & (int)Features.multitarget) > 0) {
			AttackNumber = (int)t.features.number;
		} else if((t.features.features & (int)Features.range_attack) > 0) {
			AttackNumber = -1;
		} else AttackNumber = 1;
		SphereCollider collider = GetComponent<SphereCollider>();
		collider.radius = (float)(collider.radius * (t.attack_range + 0.5f));
		AttackThread = StartCoroutine(Shot());
	}

	void OnDisable() {
		if(AttackThread != null) StopCoroutine(AttackThread);
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.create = false;
		LinkedList<GameObject> Died = new LinkedList<GameObject>();
		foreach(GameObject go in InnerMonster) {
			try {
				go.SendMessage("FeaturesWithoutHurt", fm);//MonsterProperties.cs
			} catch {
				Died.AddLast(go);
			}
		}
		ClearDie(Died);
	}

	void OnTriggerEnter(Collider other) {
		InnerMonster.Add(other.gameObject);
		if(AttackNumber == -1 || ClosestMonster.Count < AttackNumber) {
			ClosestMonster.Add(other.gameObject);
		}
	}
	void OnTriggerExit(Collider other) {
		Tower t = TowerData.AllTower[gameObject.name];
		FeatureMessage fm = new FeatureMessage();
		fm.features = t.features.features;
		fm.create = false;
		other.gameObject.SendMessage("FeaturesWithoutHurt", fm);//MonsterProperties.cs
		InnerMonster.Remove(other.gameObject);
		//如果塔当前正在攻击该物体
		if(ClosestMonster.Contains(other.gameObject)) {
			ClosestMonster.Remove(other.gameObject);
			GetClosestMonster();
		}
	}

	// void OnTriggerStay(Collider other) {
	// 	Tower t = TowerData.AllTower[gameObject.name];
	// 	FeatureMessage fm = new FeatureMessage();
	// 	fm.features = t.features.features;
	// 	fm.value = t.features.number;
	// 	fm.time = t.features.time;
	// 	fm.create = true;
	// 	other.gameObject.SendMessage("FeaturesWithoutHurt", fm);//MonsterProperties.cs
	// }

	void GetClosestMonster() {
		if(ClosestMonster.Count == InnerMonster.Count) return;
		Vector3 tower_pos = transform.position;
		float min = -1;
		LinkedList<GameObject> Died = new LinkedList<GameObject>();
		for(int i = ClosestMonster.Count; i < AttackNumber; i++) {
			min = -1;
			foreach(GameObject go in InnerMonster) {
				try {
					if(ClosestMonster.Contains(go)) continue;
					float temp = Vector3.Distance(go.transform.position, tower_pos);
					if(min > temp || min == -1) {
						min = temp;
					}
				} catch {
					Died.AddLast(go);
				}
			}
			ClearDie(Died);
		}
	}

	private void ClearDie(LinkedList<GameObject> list)
	{
		while(list.Count > 0)
		{
			if(ClosestMonster.Contains(list.First.Value))
				ClosestMonster.Remove(list.First.Value);
			if(InnerMonster.Contains(list.First.Value))
				InnerMonster.Remove(list.First.Value);
			list.RemoveFirst();
		}
	}

	private IEnumerator Shot() {
		Object temp = Resources.Load(BulletPath + gameObject.name + "Bullet");
		if(temp == null) yield break;
		Tower t = TowerData.AllTower[gameObject.name];
		AttackMessage am = new AttackMessage();
		am.attack_features = (uint)t.features.features;
		am.attack_type = (byte)t.attack_type;
		am.damage = (long)t.damage;
		am.time = t.features.time;
		am.value = t.features.number;
		object[] arg = new object[5]{transform.localPosition, null, BulletInfo.GetFlySpeed(gameObject.name + "Bullet"), am, null};
		LinkedList<GameObject> Died = new LinkedList<GameObject>();
		while(true) {
			yield return new WaitWhile(() => ClosestMonster.Count == 0);
			foreach(GameObject go in ClosestMonster) {
				try {
					arg[1] = go.transform.localPosition;
					arg[4] = go;
					GameObject bullet = (GameObject)Instantiate(temp);
					bullet.transform.parent = transform.parent;
					bullet.transform.localScale *= GameRunning.EnlargRatio;
					bullet.SendMessage("SetBulletInfo", arg);
				} catch {
					Died.AddLast(go);
				}
			}
			ClearDie(Died);
			yield return new WaitForSeconds((float)t.attack_interval / 1000.0f);
		}
	}
}
