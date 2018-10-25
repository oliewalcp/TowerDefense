using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerData : MonoBehaviour {
	static public Dictionary<string, Tower> AllTower = new Dictionary<string, Tower>();
	public const float UnitRadius = 0.0328f;//Collider一格范围的半径
	private const string TowerDataFile = "Data/Towers.xml";
	private const string NAME = "name";//名称
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
	private const string FEATURES = "features";//效果代号
	private const string VALUE = "value";//效果值
	// private const string MULTITARGET = "multitarget";//同时攻击的目标数量
	// private const string RANGE_ATTACK = "range_attack";//是否是范围攻击
	// private const string SPUTTER = "sputter";//攻击造成伤害时的溅射范围
	// private const string CATAPULT = "catapult";//攻击时子弹弹射的数量
	// private const string POISON = "poison";//每秒因中毒的真实伤害值
	// private const string SPEED_DOWN = "speeddown";//速度值减少的值
	// private const string DEFENSE_BREAK = "defense_break";//防御值减少的值
	// private const string STEAL = "steal";//杀死怪物时额外获得的金币数
	// private const string ATTACK_SPEED_UP = "attack_speedup";//己方塔增加的攻击速度
	// private const string ATTACK_DAMAGE_UP = "attack_damageup";//己方塔增加的攻击力

	// Use this for initialization
	void Start () {
		AllTower.Clear();
		XMLFileController xml = new XMLFileController();
		xml.Open(TowerDataFile);
		LinkedList<string> list = xml.GetChildren();
		foreach(string name in list) {
			xml.BeginParentNode(name);
			//获取塔的基本信息
			Tower temp_t = new Tower(xml.GetValue(NAME), xml.GetValue(DETAIL), int.Parse(xml.GetValue(BUILD_GOLD)), 
				double.Parse(xml.GetValue(DAMAGE)), int.Parse(xml.GetValue(ATTACK_TYPE)),
				double.Parse(xml.GetValue(CRIT_RATE)), double.Parse(xml.GetValue(CRIT_DAMAGE)), 
				int.Parse(xml.GetValue(ATTACK_RANGE)), long.Parse(xml.GetValue(ATTACK_INTERVAL)),
				int.Parse(xml.GetValue(DOMAIN_WITDH)), int.Parse(xml.GetValue(DOMAIN_HEIGHT)));
			//获取塔的效果信息
			long last_time = long.Parse(xml.GetValue(EFFECT_LAST_TIME));
			int features_num = int.Parse(xml.GetValue(FEATURES));
			double value = double.Parse(xml.GetValue(VALUE));
			temp_t.SetFeatures(features_num, last_time, value);
			AllTower.Add(name, temp_t);
			xml.EndParentNode();
		}
	}
}
