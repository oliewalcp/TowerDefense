using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterInfo : MonoBehaviour {
	public Text HitpointText;
	public Text DefensePointText;
	public Text SpeedText;
	public Text ArmorTypeText;
	public Text NextArmorTypeText;
	private void SetNextArmorType(ushort type) {
		NextArmorTypeText.text = type > 0 ? ("下一关：" + GetArmorType(type)) : "通关啦";
	}
	private void SetMonsterProperties(MonsterProperty arg) {
		SetHitpoint(arg.hit_point);
		SetDefensePoint(arg.defense_point);
		SetSpeed(arg.speed);
		SetArmorType(arg.armor_type);
	}
	private void SetHitpoint(double hitpoint) {
		HitpointText.text = "生命值：" + hitpoint.ToString();
	}
	private void SetDefensePoint(double defensepoint) {
		DefensePointText.text = "防御值：" + defensepoint.ToString();
	}
	private void SetSpeed(float speed) {
		SpeedText.text = "移动速度：" + speed.ToString();
	}
	private void SetArmorType(ushort type) {
		ArmorTypeText.text = "护甲类型：" + GetArmorType(type);
	}
	private string GetArmorType(ushort type) {
		switch(type) {
			case (ushort)ArmorType.NONE: return "无甲";
			case (ushort)ArmorType.LEATHER: return "皮甲";
			case (ushort)ArmorType.WOOD: return "木甲";
			case (ushort)ArmorType.IRON: return "铁甲";
			case (ushort)ArmorType.STEEL: return "钢甲";
			case (ushort)ArmorType.DIAMOND: return "钻甲";
			case (ushort)ArmorType.MAGIC: return "魔法";
		}
		return "";
	}
}
