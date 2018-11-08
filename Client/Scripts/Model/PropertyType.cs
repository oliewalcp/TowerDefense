public enum AttackType : byte {
	NORMAL = 1, //普通
	PIERCE = 2, //穿刺
	MAGIC = 3, //魔法
	SIEGE = 4, //攻城
	CHAOS = 5, //混乱
	SMASH = 6 //粉碎

}

public enum ArmorType : ushort {
	NONE = 1,  //无甲
	LEATHER = 2, //皮甲
	WOOD = 3, //木甲
	IRON = 4, //铁甲
	STEEL = 5, //钢甲
	DIAMOND = 6, //钻甲
	MAGIC = 7 //魔法
}
//伤害减免率
public class DamageReduce {
	//普通攻击类型伤害率
	public const double NORMAL_NONE = 1.5;
	public const double NORMAL_LEATHER = 1;
	public const double NORMAL_WOOD = 0.85;
	public const double NORMAL_IRON = 0.75;
	public const double NORMAL_STEEL = 0.5;
	public const double NORMAL_DIAMOND = 0.25;
	public const double NORMAL_MAGIC = 1.25;
	//穿刺攻击类型伤害率
	public const double PIERCE_NONE = 1.5;
	public const double PIERCE_LEATHER = 1;
	public const double PIERCE_WOOD = 0.85;
	public const double PIERCE_IRON = 0.75;
	public const double PIERCE_STEEL = 0.5;
	public const double PIERCE_DIAMOND = 0.25;
	public const double PIERCE_MAGIC = 1.25;
	//魔法攻击类型伤害率
	public const double MAGIC_NONE = 0.1;
	public const double MAGIC_LEATHER = 0.3;
	public const double MAGIC_WOOD = 0.75;
	public const double MAGIC_IRON = 1.5;
	public const double MAGIC_STEEL = 2;
	public const double MAGIC_DIAMOND = 2.75;
	public const double MAGIC_MAGIC = 0.05;
	//攻城攻击类型伤害率
	public const double SIEGE_NONE = 0.1;
	public const double SIEGE_LEATHER = 0.4;
	public const double SIEGE_WOOD = 0.85;
	public const double SIEGE_IRON = 2;
	public const double SIEGE_STEEL = 1.5;
	public const double SIEGE_DIAMOND = 1;
	public const double SIEGE_MAGIC = 0.7;
	//混乱攻击类型伤害率
	public const double CHAOS_NONE = 1;
	public const double CHAOS_LEATHER = 1;
	public const double CHAOS_WOOD = 1;
	public const double CHAOS_IRON = 1;
	public const double CHAOS_STEEL = 1;
	public const double CHAOS_DIAMOND = 1;
	public const double CHAOS_MAGIC = 1;
	//粉碎攻击类型伤害率
	public const double SMASH_NONE = 3.5;
	public const double SMASH_LEATHER = 3;
	public const double SMASH_WOOD = 0.3;
	public const double SMASH_IRON = 0.1;
	public const double SMASH_STEEL = 0.1;
	public const double SMASH_DIAMOND = 0.1;
	public const double SMASH_MAGIC = 1.75;
}
//封装攻击的信息，用于消息传递
public struct AttackMessage {
	public long damage{get; set;}//攻击力
	public byte attack_type{get; set;}//攻击类型
	public uint attack_features{get; set;}//攻击附带的效果
	public double value{get; set;}//效果的值
	public long time{get; set;}//效果的持续时间
}
//效果信息，用于消息传递
public struct FeatureMessage {
	public bool create{get; set;}//true——增加效果，false——移除效果
	public int features{get; set;}//效果
	public double value{get; set;}//效果的值
	public long time{get; set;} //效果的持续时间
}
//效果类型的编号，保证效果不叠加
public enum FeatureTypeNumber {
	POISON = 0x1111,//毒性
	DISRUPTING = 0x2222, //破防
	DECELERATE = 0x4444, //减速

	TIMING_POISON = 0x01, //计时的毒性（毒性是主效果，血量在限定时间内定时减少）
	TIMING_DISRUPTING = 0x02,  //计时的破防（破防是主效果，防御值在限定时间内减低）
	TIMING_DECELERATE = 0x04, //计时的减速（减速是主效果，速度在限定时间内减慢）

	RANGE_POISON = 0x10, //范围的毒性（毒性是主效果，怪物在指定范围内定时减少血量）
	RANGE_DISRUPTING = 0x20, //范围的破防（破防是主效果，怪物在指定范围内减低防御值）
	RANGE_DECELERATE = 0x40, //范围的减速（减速是主效果，怪物在指定范围内减慢速度）

	ADDITION_TIMING_POISON = 0x0100, //附加的计时毒性（毒性是副效果，血量在限定时间内定时减少）
	ADDITION_TIMING_DISRUPTING = 0x0200, //附加的计时破防（破防是副效果，防御值在限定时间内减低）
	ADDITION_TIMING_DECELERAT = 0x0400, //附加的计时减速（减速是副效果，速度在限定时间内减慢）

	ADDITION_RANGE_POISON = 0x1000, //附加的范围毒性（毒性是副效果，怪物在指定范围内定时减少血量）
	ADDITION_RANGE_DISRUPTING = 0x2000, //附加的范围破防（破防是副效果，怪物在指定范围内减低防御值）
	ADDITION_RANGE_DECELERATE = 0x4000 //附加的范围减速（减速是副效果，怪物在指定范围内减慢速度）
}