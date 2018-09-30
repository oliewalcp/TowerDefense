using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerProperties : MonoBehaviour {
	static private Dictionary<string, Tower> AllTower = new Dictionary<string, Tower>();
	private Tower current_tower = null;
	private const string TowerData = "Data/Towers.xml";
	private const string DETAIL = "detail";//描述
	private const string BUILD_GOLD = "build_gold";//建造所需金币
	private const string DAMAGE = "damage";//攻击力
	private const string ATTACK_TYPE = "attack_type";//攻击类型
	private const string CRIT_RATE = "crit_rate";//暴击概率
	private const string CRIT_DAMAGE = "crit_damage";//暴击伤害倍率
	private const string ATTACK_RANGE = "attack_range";//攻击范围
	private const string ATTACK_INTERVAL = "attack_interval";//攻击间隔时间
	private const string DOMAIN_WITDH = "domain_width";//占用格子的宽度
	private const string DOMAIN_HEIGHT = "domain_height";//占用格子的高度
	private const string EFFECT_LAST_TIME = "effect_last_time";//效果的持续时间
	private const string MULTITARGET = "multitarget";//同时攻击的目标数量
	private const string RANGE_ATTACK = "range_attack";//是否是范围攻击
	private const string SPUTTER = "sputter";//攻击造成伤害时的溅射范围
	private const string CATAPULT = "catapult";//攻击时子弹弹射的数量
	private const string POISON = "poison";//每秒因中毒的真实伤害值
	private const string SPEED_DOWN = "speeddown";//速度值减少的值
	private const string DEFENSE_BREAK = "defense_break";//防御值减少的值
	private const string STEAL = "steal";//杀死怪物时额外获得的金币数
	private const string ATTACK_SPEED_UP = "attack_speedup";//己方塔增加的攻击速度
	private const string ATTACK_DAMAGE_UP = "attack_damageup";//己方塔增加的攻击力

	// Use this for initialization
	void Start () {
		AllTower.Clear();
		XMLFileController xml = new XMLFileController();
		xml.Open(TowerData);
		LinkedList<string> list = xml.GetChildren();
		foreach(string name in list) {
			xml.BeginParentNode(name);
			//获取塔的基本信息
			Tower temp_t = new Tower(name, xml.GetValue(DETAIL), int.Parse(xml.GetValue(BUILD_GOLD)), 
				double.Parse(xml.GetValue(DAMAGE)), int.Parse(xml.GetValue(ATTACK_TYPE)),
				double.Parse(xml.GetValue(CRIT_RATE)), double.Parse(xml.GetValue(CRIT_DAMAGE)), 
				int.Parse(xml.GetValue(ATTACK_RANGE)), long.Parse(xml.GetValue(ATTACK_INTERVAL)),
				int.Parse(xml.GetValue(DOMAIN_WITDH)), int.Parse(xml.GetValue(DOMAIN_HEIGHT)));
			//获取塔的效果信息
			string last_time = xml.GetValue(EFFECT_LAST_TIME);
			string multitarget_features = xml.GetValue(MULTITARGET);
			string range_attack_features = xml.GetValue(RANGE_ATTACK);
			string sputter_features = xml.GetValue(SPUTTER);
			string catapult_features = xml.GetValue(CATAPULT);
			string poison_features = xml.GetValue(POISON);
			string speed_down_features = xml.GetValue(SPEED_DOWN);
			string defense_break_features = xml.GetValue(DEFENSE_BREAK);
			string steal_features = xml.GetValue(STEAL);
			string attack_speed_up_features = xml.GetValue(ATTACK_SPEED_UP);
			string attack_damage_up_features = xml.GetValue(ATTACK_DAMAGE_UP);
			//多目标攻击
			if(!multitarget_features.Equals("1")) {
				temp_t.AddFeatures((int)Features.multitarget, 0, int.Parse(multitarget_features));
			}
			//范围攻击
			if(!range_attack_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.range_attack, 0, 0);
			}
			//溅射伤害范围
			if(!sputter_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.sputter, 0, int.Parse(sputter_features));
			}
			//弹射攻击
			if(!catapult_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.catapult, 0, int.Parse(catapult_features));
			}
			//毒性攻击
			if(!poison_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.poison, long.Parse(last_time), double.Parse(poison_features));
			}
			//减速效果
			if(!speed_down_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.speeddown, long.Parse(last_time), double.Parse(speed_down_features));
			}
			//减防效果
			if(!defense_break_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.defense_break, long.Parse(last_time), double.Parse(defense_break_features));
			}
			//偷盗效果
			if(!steal_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.steal, long.Parse(last_time), double.Parse(steal_features));
			}
			//加速效果
			if(!attack_speed_up_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.attack_speedup, long.Parse(last_time), double.Parse(attack_speed_up_features));
			}
			//加攻效果
			if(!attack_damage_up_features.Equals("0")) {
				temp_t.AddFeatures((int)Features.attack_damageup, long.Parse(last_time), double.Parse(attack_damage_up_features));
			}
			AllTower.Add(name, temp_t);
			xml.EndParentNode();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	/* 设置塔的类型
	   param[tower_name]:塔的名称
	 */
	private void SetTower(string tower_name) {
		current_tower = AllTower[tower_name];
	}
}
