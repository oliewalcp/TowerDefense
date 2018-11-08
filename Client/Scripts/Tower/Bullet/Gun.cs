using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//炮弹（不跟踪效果）发射效果，击中后溅射效果
public class Gun : MonoBehaviour {
	private const float SphereRadiusUnit = 3.33333f;//碰撞器一格的长度
	private const float BurstRadiusUnit = 20f;//爆炸范围一格的长度
	private AttackMessage Attack;

	/* 设置炮弹的位置与速度并传递攻击信息   MonsterMoniter.cs调用
	   param[0]:原位置  Vector3
	   param[1]:目标位置  Vector3
	   param[2]:炮弹速度  float
	   param[3]:攻击信息  AttackMessage
	   param[4]:目标物体  GameObject
	*/
	private void SetBulletInfo(object[] pos) {
		Attack = (AttackMessage)pos[3];
		transform.localPosition = (Vector3)pos[0];
		iTween.moveTo(gameObject, Vector3.Distance((Vector3)pos[0], (Vector3)pos[1]) / (float)pos[2], 0, ((Vector3)pos[1]).x, ((Vector3)pos[1]).y, ((Vector3)pos[1]).z, iTween.EasingType.linear, "Hit", (Vector3)pos[1]);
	}
	//炮弹击中目标位置  iTween.cs调用
	private void Hit(Vector3 pos) {
		GameObject Burst = (GameObject)Instantiate(Resources.Load("Prefabs/Effect/SputterBurst"));
		Burst.SendMessage("SetAttackInfo", Attack);
		Burst.transform.parent = transform.parent;
		Burst.transform.localPosition = pos;
		SphereCollider HurtRadius = Burst.GetComponent<SphereCollider>();
		float temp = 0;
		if(Attack.value >= 1) {
			temp = (float)Attack.value - 0.5f;
		} else {
			temp = (float)Attack.value / 2;
		}
		HurtRadius.radius = SphereRadiusUnit * temp;
		temp *= BurstRadiusUnit;
		transform.localScale = new Vector3(temp, temp, temp);
		Destroy(gameObject);
	}
}
