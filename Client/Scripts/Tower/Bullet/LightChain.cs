using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightChain : MonoBehaviour {
	private bool Display = false;
	private const float Time = 5f;
	private LineRenderer Line;
	private Vector3[] Points;
	private Vector3 position;
	private int index = 0;
	private int LengthOfLineRenderer = 0;
	void Start() {
		Line = gameObject.GetComponent<LineRenderer>();
		Points = new Vector3[4];
		Line.positionCount = 4;
		Points[0] = new Vector3(10, 10, 0);
		Points[1] = new Vector3(50, 0, 0);
		Points[2] = new Vector3(80, 100, 0);
		Points[3] = new Vector3(-10, -10, 0);
		Display = true;
		StartCoroutine(Play());
	}

	void Update() {
		if(Display == false) return;
		for(int i = 0; i < Points.Length; i++) {
			Line.SetPosition(i, Points[i]);
		}
	}

	private void StartChain(params Transform[] args) {
		for(int i = 0; i < args.Length; i++) {
			Points[i] = args[i].position;
		}
		StartCoroutine(Play());
	}

	private IEnumerator Play() {
		yield return new WaitForSeconds(Time);
		Destroy(gameObject);
	}
}
