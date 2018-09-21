using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveRoute : MonoBehaviour {
	public GameObject MapPanel;//地图区域
	public static float MoveSpeed = 100f;//移动速度
	public const byte UP = 0;//上
	public const byte LEFT = 1;//左
	public const byte DOWN = 2;//下
	public const byte RIGHT = 3;//右
	public const byte NONE = 255;//不相邻
	public static Vector3 UP_D = new Vector3(180, 0, 0);
	public static Vector3 DOWN_D = new Vector3(180, 0, 180);
	public static Vector3 LEFT_D = new Vector3(180, 0, 270);
	public static Vector3 RIGHT_D = new Vector3(180, 0, 90);
	public static Vector3[] DIRECTION = {UP_D, LEFT_D, DOWN_D, RIGHT_D};
	private uint currentIndex = 1;//当前位置索引
	//private Position currentPosition;
	static private Vector2 EndPoint = new Vector2() {
		x = 0,
		y = 0
	};//终点坐标
	private bool read = false;
	// Use this for initialization
	void Start () {
		transform.SetParent(MapPanel.transform);
		float small = LocalMessage.grid.height > LocalMessage.grid.width ? LocalMessage.grid.width : LocalMessage.grid.height;	
		UIFunction.SetScale(gameObject, new Vector3(small, small, small));
		UIFunction.SetMapPosition(gameObject, LocalMessage.StartGrid);
		if(EndPoint.x == 0 && EndPoint.y == 0) EndPoint = UIFunction.GetPixelPosition(LocalMessage.EndGrid);
		transform.localEulerAngles = new Vector3(180, 0, 0);
		Turn(UP, LocalMessage.MonsterRoute[0].GetDirection());
		Position p = LocalMessage.MonsterRoute[currentIndex];
		iTween.moveTo(gameObject, (float)GetDistance(p.GetPixel(), UIFunction.GetPixelPosition(LocalMessage.StartGrid)) / MoveSpeed, 0, p.GetX(), p.GetY(), -8.5f, iTween.EasingType.linear, "CompleteMove", null);
	}

	void OnEnable() {

	}
	
	// Update is called once per frame
	void Update () {

	}
	private void Turn(byte now, byte next) {
		if(now == UP) {
			if(next == DOWN) transform.Rotate(0, 0, 180);
			else if(next == LEFT) transform.Rotate(0, 0, -90);
			else if(next == RIGHT) transform.Rotate(0, 0, 90);
		} else if(now == DOWN) {
			if(next == UP) transform.Rotate(0, 0, 180);
			else if(next == LEFT) transform.Rotate(0, 0, 90);
			else if(next == RIGHT) transform.Rotate(0, 0, -90);
		} else if(now == LEFT) {
			if(next == RIGHT) transform.Rotate(0, 0, 180);
			else if(next == UP) transform.Rotate(0, 0, 90);
			else if(next == DOWN) transform.Rotate(0, 0, -90);
		} else if(now == RIGHT) {
			if(next == UP) transform.Rotate(0, 0, -90);
			else if(next == LEFT) transform.Rotate(0, 0, 180);
			else if(next == DOWN) transform.Rotate(0, 0, 90);
		}
	}

	private void CompleteMove() {
		Position temp1 = LocalMessage.MonsterRoute[currentIndex];
		if(temp1.GetLine() == LocalMessage.EndGrid.line && temp1.GetColumn() == LocalMessage.EndGrid.column) {
			ArriveEndPoint();
			return;
		}
		Position temp2 = LocalMessage.MonsterRoute[++currentIndex];
		transform.localEulerAngles = DIRECTION[temp1.GetDirection()];
		iTween.moveTo(gameObject, (float)GetDistance(temp1.GetPixel(), temp2.GetPixel()) / MoveSpeed, 0, temp2.GetX(), temp2.GetY(), -8.5f, iTween.EasingType.linear, "CompleteMove", null);
	}
	void OnDisable() {
		Destroy(gameObject);
	}
	
	private void ArriveEndPoint() {
		Destroy(gameObject);
	}

	private double GetDistance(Vector2 v1, Vector2 v2) {
		return Math.Sqrt(Math.Pow(v1.x - v2.x, 2) + Math.Pow(v1.y - v2.y, 2));
	}
}
