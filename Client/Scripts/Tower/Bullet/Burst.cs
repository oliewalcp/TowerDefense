using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burst : MonoBehaviour {
	private AttackMessage am;

	void Update() {
		ParticleSystem particleSystems = GetComponentInChildren<ParticleSystem>();
		if(!particleSystems.isPlaying) {
			Destroy(gameObject);
		}
	}
	// Gun.cs调用
	private void SetAttackInfo(AttackMessage am) {
		this.am = am;
	}
	//当有怪物进入爆炸范围圈时调用
	void OnTriggerEnter(Collider other) {
		other.gameObject.SendMessage("Damaged", am);
	}
}
