﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct MonsterProperty {
	public double hit_point;//生命值
	public double defense_point;//防御值
	public float speed;//移动速度
	public ushort armor_type;//护甲类型
	public ushort bonus;//打死的奖励
	public MonsterProperty ParseHipPoint(string arg) { hit_point = double.Parse(arg); return this; }
	public MonsterProperty ParseDefensePoint(string arg) { defense_point = double.Parse(arg); return this; }
	public MonsterProperty ParseSpeed(string arg) { speed = float.Parse(arg); return this; }
	public MonsterProperty ParseArmorType(string arg) { armor_type = ushort.Parse(arg); return this; }
	public MonsterProperty ParseBonus(string arg) { bonus = ushort.Parse(arg); return this; }
}

public struct FeaturePackage {
	public double value;//效果的值
	public float time;
	public Coroutine coroutine;//负责监听效果的协程
}

//记录当前关卡怪物的信息
public class MonsterProperties : MonoBehaviour {
	private MonsterProperty current;//当前数值
	private MonsterProperty origin;//原数值
	public Camera UICamera;
	private GameObject BloodBar;
	delegate IEnumerator Timer(double value, long time);
	private Dictionary<int, FeaturePackage> already_exists_features = new Dictionary<int, FeaturePackage>();//效果——效果值
	void Start() {
		UICamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
		BloodBar = (GameObject)Instantiate(Resources.Load("Prefabs/Monster/BloodBar"));
		BloodBar.transform.SetParent(transform);
	}
	void Update() {
		 Vector3 pt = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 0.3f, transform.position.z));
		 BloodBar.transform.position = UICamera.ScreenToWorldPoint(new Vector3(pt.x, pt.y, 1));
	}
	/* 加载当前关卡信息  GameRunning.cs调用
	   param[check_point]:当前关卡数
	 */
	private void LoadCurrentCheckPointMessage(MonsterProperty mp) {
		mp.speed *= GameRunning.EnlargRatio;
		current = mp;
		origin = mp;
		SetSpeed();
	}

	private void SetSpeed() {
		SendMessage("SetMoveSpeed", current.speed);//MoveRoute.cs
	}
	/* 当前物体受到攻击时，调用此函数   由Burst.cs调用
	   param[damage]:攻击力
	   param[attack_type]:攻击类型
	   param[attack_features]:攻击附带效果
	   param[value]:效果的值
	   param[time]:持续时间
	 */
	private void Damaged(AttackMessage arg) {
		long damage = arg.damage;
		byte attack_type = arg.attack_type;
		uint attack_features = arg.attack_features;
		double value = arg.value;
		long time = arg.time;
		switch(attack_type) {
			case (byte)AttackType.NORMAL:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.NORMAL_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.NORMAL_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.NORMAL_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.NORMAL_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.NORMAL_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.NORMAL_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.NORMAL_MAGIC);break;
			};break;
			case (byte)AttackType.PIERCE:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.PIERCE_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.PIERCE_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.PIERCE_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.PIERCE_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.PIERCE_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.PIERCE_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.PIERCE_MAGIC);break;
			};break;
			case (byte)AttackType.MAGIC:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.MAGIC_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.MAGIC_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.MAGIC_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.MAGIC_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.MAGIC_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.MAGIC_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.MAGIC_MAGIC);break;
			};break;
			case (byte)AttackType.SIEGE:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.SIEGE_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.SIEGE_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.SIEGE_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.SIEGE_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.SIEGE_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.SIEGE_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.SIEGE_MAGIC);break;
			};break;
			case (byte)AttackType.CHAOS:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.CHAOS_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.CHAOS_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.CHAOS_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.CHAOS_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.CHAOS_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.CHAOS_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.CHAOS_MAGIC);break;
			};break;
			case (byte)AttackType.SMASH:
			switch(current.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.SMASH_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.SMASH_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.SMASH_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.SMASH_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.SMASH_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.SMASH_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.SMASH_MAGIC);break;
			};break;
		}
		SetFeatures(attack_features, value, time);
	}
	/* 单纯受到效果影响而没有伤害  MonsterMonitor.cs调用
	 */
	private void FeaturesWithoutHurt(FeatureMessage fm) {
		//新增效果
		if(fm.create) {
			//减速效果
			if((fm.features & (int)Features.speeddown) > 0) {
				fm.value *= GameRunning.EnlargRatio;
				FeaturePackage fp;
				if(already_exists_features.ContainsKey(fm.features)) {
					fp = already_exists_features[fm.features];
					if(((fp.value < 1 && fm.value < 1) || (fp.value >= 1 && fm.value >= 1)) 
						&& fp.value < fm.value) {
						StartTimer(fp, fm, DecelerateFeaturesTimer);
					} else if(fp.value < 1 && fm.value >= 1) {
						if(fp.value * origin.speed < fm.value) {
							StartTimer(fp, fm, DecelerateFeaturesTimer);
						}
					} else if(fp.value >= 1 && fm.value < 1) {
						if(fp.value < fm.value * origin.speed) {
							StartTimer(fp, fm, DecelerateFeaturesTimer);
						}
					}
				} else {
					fp = new FeaturePackage();
					fp.value = fm.value;
					already_exists_features.Add(fm.features, fp);
					fp.coroutine = StartCoroutine(DecelerateFeaturesTimer(fm.value, fm.time));
				}
			}
			//减防效果
			if((fm.features & (int)Features.defense_break) > 0) {
				FeaturePackage fp;
				if(already_exists_features.ContainsKey(fm.features)) {
					fp = already_exists_features[fm.features];
					if(((fp.value < 1 && fm.value < 1) || (fp.value >= 1 && fm.value >= 1)) 
						&& fp.value < fm.value) {
						StartTimer(fp, fm, DisruptingFeaturesTimer);
					} else if(fp.value < 1 && fm.value >= 1) {
						if(fp.value * origin.speed < fm.value) {
							StartTimer(fp, fm, DisruptingFeaturesTimer);
						}
					} else if(fp.value >= 1 && fm.value < 1) {
						if(fp.value < fm.value * origin.speed) {
							StartTimer(fp, fm, DisruptingFeaturesTimer);
						}
					}
				} else {
					fp = new FeaturePackage();
					fp.value = fm.value;
					already_exists_features.Add(fm.features, fp);
					fp.coroutine = StartCoroutine(DisruptingFeaturesTimer(fm.value, fm.time));
				}
			}
		} 
		//移除效果
		else {
			if(!already_exists_features.ContainsKey(fm.features)) return;
			FeaturePackage fp = already_exists_features[fm.features];
			if((fm.features & (int)Features.speeddown) > 0) {
				current.speed = origin.speed;
				SendMessage("SetMoveSpeed", origin.speed);//MoveRoute.cs
			}
			already_exists_features.Remove(fm.features);
		}
	}
	private void StartTimer(FeaturePackage fp, FeatureMessage fm, Timer t) {
		fp.value = fm.value;
		if(fp.coroutine != null)
			StopCoroutine(fp.coroutine);
		fp.coroutine = StartCoroutine(t(fm.value, fm.time));
	}
	//设置当前血量
	private void SetCurrentHipPoint(double hp) {
		current.hit_point = hp;
	}
	//设置当前移动速度
	private void SetCurrentMoveSpeed(float s) {
		current.speed = s;
	}
	//设置当前防御值
	private void SetCurrentDefensePoint(long dp) {
		current.defense_point = dp;
	}
	/* 减血操作
	   param[damage]:攻击力
	   param[ratio]:伤害倍率
	 */
	private void DecreaseHipPoint(double damage, double ratio) {
		double hurt = damage * ratio;
		DecreaseOperator(hurt);
		if(current.hit_point <= 0)
			Die();
	}
	/* 减血操作
	   param[RealHurt]:真实伤害值
	 */
	private void DecreaseOperator(double RealHurt) {
		if(RealHurt <= current.defense_point)
			current.hit_point--;
		else {
			current.hit_point -= (RealHurt - current.defense_point);
		}
		Slider blood = BloodBar.GetComponentInChildren<Slider>();
		blood.value = (float)(current.hit_point / origin.hit_point);
	}
	/* 特效影响效果
	   param[features]:效果
	   param[value]:效果的值
	   param[time]:持续时间
	 */
	private LinkedList<Coroutine> SetFeatures(uint features, double value, long time = 0) {
		if(value == 0) return null;
		LinkedList<Coroutine> result = new LinkedList<Coroutine>();
		//减防效果
		if((features & (uint)Features.defense_break) > 0) {
			result.AddLast(StartCoroutine(DisruptingFeaturesTimer(value, time)));
		}
		//减速效果
		if((features & (uint)Features.speeddown) > 0) {
			result.AddLast(StartCoroutine(DecelerateFeaturesTimer(value, time)));
		}
		//中毒效果
		if((features & (uint)Features.poison) > 0) {
			result.AddLast(StartCoroutine(HipPointFeaturesTimer(value, time)));
		}
		return result;
	}
	/* 毒性效果的计时器
	   param[value]:每秒受到的伤害
	   param[time]:持续时间
	 */
	private IEnumerator HipPointFeaturesTimer(double value, long time) {
		while(time > 0) {
			yield return new WaitForSeconds(1);
			time--;
			DecreaseOperator(value);
			if(current.hit_point <= 0) {
				Die();
				break;
			}
		}
	}
	/* 减速效果的计时器
	   param[value]:速度的减少值
	   param[time]:持续时间
	 */
	private IEnumerator DecelerateFeaturesTimer(double value, long time) {
		if(value > 1) current.speed = origin.speed - (float)value;
		else current.speed = origin.speed * (float)(1 -  value);
		Loom.QueueOnMainThread(() => {
			SendMessage("SetMoveSpeed", current.speed);//MoveRoute.cs
		});
		if(time <= 0) yield break;
		while(time > 0) {
			if(time < 1) yield return new WaitForSeconds(time / 1000);
			else yield return new WaitForSeconds(1);
			time--;
		}
		current.speed = origin.speed;
		Loom.QueueOnMainThread(() => {
			SendMessage("SetMoveSpeed", current.speed);//MoveRoute.cs
		});
	}
	/* 破防效果的计时器
	   param[value]:防御值的减少值
	   param[time]:持续时间
	 */
	private IEnumerator DisruptingFeaturesTimer(double value, long time) {
		if(value > 1) current.defense_point = origin.defense_point - (long)value;
		else current.defense_point = origin.defense_point * (float)(1 - value);
		if(time <= 0) yield break;
		while(time > 0) {
			if(time < 1) yield return new WaitForSeconds(time / 1000);
			else yield return new WaitForSeconds(1);
			time--;
		}
		current.defense_point = origin.defense_point;
	}
	private void Die() {
		Destroy(gameObject);
	}
}
