using System.Collections.Generic;
//效果编号
public enum Features {
	multitarget = 1, //同时攻击的目标数量
	range_attack = 2, //是否是范围攻击
	sputter = 4, //攻击造成伤害时溅射的范围
	catapult = 8, //攻击时子弹弹射的数量
	poison = 16, //中毒的真实伤害值（小于1则为最大生命值的百分数，大于等于1则为确定的数值）
	speeddown = 32, //速度值减少的百分数
	defense_break = 64, //防御值减少的百分数
	steal = 128, //杀死怪物额外获得的金币数
	attack_speedup = 256, //己方塔增加的攻击速度（百分数）
	attack_damageup = 512 //己方塔增加的攻击力（百分数）
}
public class SpecialEffect {
	public int features{get; set;}//所包含的特性编号
	public long time{get; set;}//持续时间（-1表示永久有效）
	public double number{get; set;}//特性对应的数值
	public SpecialEffect(int features = 0, long time = 0, double number = 0){
		this.features = features;
		this.time = time;
		this.number = number;
	}
	public static bool operator ==(SpecialEffect se1, SpecialEffect s2){
		return se1.features == s2.features;
	}
	public static bool operator !=(SpecialEffect se1, SpecialEffect s2){
		return se1.features != s2.features;
	}
	public static bool operator ==(SpecialEffect se, int arg){
		return se.features == arg;
	}
	public static bool operator !=(SpecialEffect se, int arg){
		return se.features != arg;
	}
	public override bool Equals(object o){
		return this.features == ((SpecialEffect)o).features;
	}
	public override int GetHashCode(){
		return this.features;
	}
}
//塔
public class Tower {
	public string name{get; set;} //塔的名称
	public int build_gold{get; set;}//建造所需的金币
	public string detail{get; set;}//塔的描述
	public double damage{get; set;} //攻击力
	public int attack_type{get; set;} //攻击类型
	public double crit_rate{get; set;} //暴击率
	public double crit_damage{get; set;} //暴击伤害倍率
	public double attack_range{get; set;} //攻击范围
	public long attack_interval{get; set;} //攻击间隔时间（毫秒）
	public int domain_width{get; set;} //占用宽度的格子数
	public int domain_height{get; set;} //占用高度的格子数
	public SpecialEffect features = null;//塔本身具备的特性
	public Tower(string name, string detail = "", int build_gold = 0, double damage = 0, int attack_type = 0, double crit_rate = 0, double crit_damage = 0, 
		double attack_range = 0, long attack_interval = 0, int domain_width = 1, int domain_height = 1){
		this.name = name;
		this.build_gold = build_gold;
		this.detail = detail;
		this.damage = damage;
		this.attack_type = attack_type;
		this.crit_rate = crit_rate;
		this.crit_damage = crit_damage;
		this.attack_range = attack_range;
		this.attack_interval = attack_interval;
		this.domain_width = domain_width;
		this.domain_height = domain_height;
	}
	/* 为塔添加额外的攻击效果
	   param[f]:效果
	   param[time]:效果持续时间
	   param[number]:效果对应的值
	 */
	public void SetFeatures(int f, long time, double number){
		features = new SpecialEffect(f, time, number);
	}
	/* 为塔移除攻击效果
	   param[f]:效果
	 */
	public void ClearFeatures(int f){
		features = null;
	}
}
