using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
	private AttackMessage Attack;
	private GameObject DestinationPosition;
	private float FlySpeed;

	/* 设置炮弹的位置与速度并传递攻击信息   MonsterMoniter.cs调用
	   param[0]:原位置  Vector3
	   param[1]:目标位置  Vector3
	   param[2]:炮弹速度  float
	   param[3]:攻击信息  AttackMessage
	   param[4]:目标物体  GameObject
	*/
	private void SetBulletInfo(object[] pos) {
		Attack = (AttackMessage)pos[3];
		FlySpeed = (float)pos[2];
		DestinationPosition = (GameObject)pos[4];
		transform.localPosition = (Vector3)pos[0];
	}
	void FixedUpdate() {
		try {
			Vector3 direction = DestinationPosition.transform.localPosition - transform.localPosition;
			//Vector3 angle = new Vector3(0, 0, Vector3.Angle(new Vector3(0, -16.7f, 0), direction));
			//transform.localEulerAngles = angle;
			transform.Translate(direction * FlySpeed * Time.deltaTime);
		} catch {}
	}
	//当击中怪物时
	void OnTriggerEnter(Collider other) {
		other.gameObject.SendMessage("Damaged", Attack);
		Destroy(gameObject);
	}
}
