using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MonsterProperty {
	public double hit_point;//生命值
	public long defense_point;//防御值
	public float speed;//移动速度
	public ushort armor_type;//护甲类型
	public ushort bonus;//打死的奖励
	public MonsterProperty ParseHipPoint(string arg) { hit_point = double.Parse(arg); return this; }
	public MonsterProperty ParseDefensePoint(string arg) { defense_point = long.Parse(arg); return this; }
	public MonsterProperty ParseSpeed(string arg) { speed = float.Parse(arg); return this; }
	public MonsterProperty ParseArmorType(string arg) { armor_type = ushort.Parse(arg); return this; }
	public MonsterProperty ParseBonus(string arg) { bonus = ushort.Parse(arg); return this; }
}
//记录当前关卡怪物的信息
public class MonsterProperties : MonoBehaviour {
	private MonsterProperty mp;
	// private double hit_point;//生命值
	// private long defense_point;//防御值
	// private float speed;//移动速度
	// private ushort armor_type;//护甲类型
	// private ushort bonus;//打死的奖励

	/* 加载当前关卡信息
	   param[check_point]:当前关卡数
	 */
	private void LoadCurrentCheckPointMessage(MonsterProperty mp) {
		this.mp = mp;
		// string[] temp = message.Split(':');
		// hit_point = double.Parse(temp[0]);
		// defense_point = long.Parse(temp[1]);
		// speed = float.Parse(temp[2]);
		// armor_type = int.Parse(temp[3]);
		// bonus = int.Parse(temp[4]);
		SetSpeed();
	}

	private void SetSpeed() {
		SendMessage("SetMoveSpeed", mp.speed);
	}
	/* 当前物体受到攻击时，调用此函数
	   param[damage]:攻击力
	   param[attack_type]:攻击类型
	   param[attack_features]:攻击附带效果
	   param[value]:效果的值
	   param[time]:持续时间
	 */
	private void Damaged(long damage, byte attack_type, uint attack_features = 0, double value = 0, long time = 0) {
		switch(attack_type) {
			case (byte)AttackType.NORMAL:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.NORMAL_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.NORMAL_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.NORMAL_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.NORMAL_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.NORMAL_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.NORMAL_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.NORMAL_MAGIC);break;
			};break;
			case (byte)AttackType.PIERCE:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.PIERCE_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.PIERCE_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.PIERCE_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.PIERCE_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.PIERCE_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.PIERCE_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.PIERCE_MAGIC);break;
			};break;
			case (byte)AttackType.MAGIC:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.MAGIC_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.MAGIC_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.MAGIC_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.MAGIC_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.MAGIC_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.MAGIC_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.MAGIC_MAGIC);break;
			};break;
			case (byte)AttackType.SIEGE:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.SIEGE_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.SIEGE_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.SIEGE_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.SIEGE_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.SIEGE_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.SIEGE_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.SIEGE_MAGIC);break;
			};break;
			case (byte)AttackType.CHAOS:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.CHAOS_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.CHAOS_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.CHAOS_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.CHAOS_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.CHAOS_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.CHAOS_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.CHAOS_MAGIC);break;
			};break;
			case (byte)AttackType.SMASH:
			switch(mp.armor_type) {
				case (ushort)ArmorType.NONE:DecreaseHipPoint(damage, DamageReduce.SMASH_NONE);break;
				case (ushort)ArmorType.LEATHER:DecreaseHipPoint(damage, DamageReduce.SMASH_LEATHER);break;
				case (ushort)ArmorType.WOOD:DecreaseHipPoint(damage, DamageReduce.SMASH_WOOD);break;
				case (ushort)ArmorType.IRON:DecreaseHipPoint(damage, DamageReduce.SMASH_IRON);break;
				case (ushort)ArmorType.STEEL:DecreaseHipPoint(damage, DamageReduce.SMASH_STEEL);break;
				case (ushort)ArmorType.DIAMOND:DecreaseHipPoint(damage, DamageReduce.SMASH_DIAMOND);break;
				case (ushort)ArmorType.MAGIC:DecreaseHipPoint(damage, DamageReduce.SMASH_MAGIC);break;
			};break;
		}
		GetFeatures(attack_features, value, time);
	}
	/* 减血操作
	   param[damage]:攻击力
	   param[ratio]:减伤比率
	 */
	private void DecreaseHipPoint(double damage, double ratio) {
		double hurt = damage * (1 - ratio);
		DecreaseOperator(hurt);
		if(mp.hit_point <= 0)
			Die();
	}
	/* 减血操作
	   param[RealHurt]:真实伤害值
	 */
	private void DecreaseOperator(double RealHurt) {
		if(RealHurt <= mp.defense_point)
			mp.hit_point--;
		else {
			mp.hit_point -= (RealHurt - mp.defense_point);
		}
	}
	/* 特效影响效果
	   param[features]:效果
	   param[value]:效果的值
	   param[time]:持续时间
	 */
	private void GetFeatures(uint features, double value, long time = 0) {
		if(value == 0) return;
		//减防效果
		if((features & (uint)Features.defense_break) > 0) {
			if(value > 1) mp.defense_point -= (long)value;
			else mp.defense_point -= (long)(mp.defense_point * value);
		}
		//减速效果
		if((features & (uint)Features.speeddown) > 0) {
			if(value > 1) mp.speed -= (float)value;
			else mp.speed -= (float)(mp.speed * value);
		}
		//中毒效果
		if((features & (uint)Features.poison) > 0) {
			if(value > 1) mp.hit_point -= value;
			else mp.hit_point -= mp.hit_point * value;
		}
		if(time != 0)
			StartCoroutine(FeaturesTimer(value, time));
	}
	/* 效果的持续时间
	   param[value]:每秒受到的伤害
	   param[time]:持续时间
	 */
	private IEnumerator FeaturesTimer(double value, long time) {
		while(time > 0) {
			yield return new WaitForSeconds(1);
			DecreaseOperator(value);
			if(mp.hit_point <= 0) break;
		}
		Die();
	}
	private void Die() {
		Destroy(gameObject);
	}
}
