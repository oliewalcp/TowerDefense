using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MoveRoute : MonoBehaviour {
	private GameObject GameRootPanel;
	private float MoveSpeed = 100f;//移动速度
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
	private uint currentIndex = 1;//下一个位置索引
	private bool StartMove = true;
	static private Vector2 EndPoint = new Vector2() {
		x = 0,
		y = 0
	};//终点坐标
	// Use this for initialization
	void Start () {
		float small = LocalMessage.grid.height > LocalMessage.grid.width ? LocalMessage.grid.width : LocalMessage.grid.height;	
		UIFunction.SetScale(gameObject, new Vector3(small, small, small));
		UIFunction.SetMapPosition(gameObject, LocalMessage.StartGrid);
		if(EndPoint.x == 0 && EndPoint.y == 0) EndPoint = UIFunction.GetPixelPosition(LocalMessage.EndGrid);
		transform.localEulerAngles = DIRECTION[LocalMessage.MonsterRoute[0].GetDirection()];
	}

	private void CompleteMove() {
		Position temp1 = LocalMessage.MonsterRoute[currentIndex];
		if(temp1.GetLine() == LocalMessage.EndGrid.line && temp1.GetColumn() == LocalMessage.EndGrid.column) {
			ArriveEndPoint();
			return;
		}
		Position temp2 = LocalMessage.MonsterRoute[++currentIndex];
		transform.localEulerAngles = DIRECTION[temp1.GetDirection()];
		iTween.moveTo(gameObject, Vector3.Distance(temp1.GetPixel(), temp2.GetPixel()) / MoveSpeed, 0, temp2.GetX(), temp2.GetY(), -8.5f, iTween.EasingType.linear, "CompleteMove", null);
	}
	void OnDisable() {
		Destroy(gameObject);
	}
	//GameRunning.cs调用
	private void SetMessageReceiver(GameObject go) {
		GameRootPanel = go;
	}
	private void ArriveEndPoint(int num = 1) {
		GameRootPanel.SendMessage("EscapeMonster", num);//GameRunning.cs
		Destroy(gameObject);
	}
	//MonsterProperties.cs调用
	public void SetMoveSpeed(float speed) {
		MoveSpeed = speed;
		Position temp1 = new Position();
		if(StartMove) {
			temp1.SetPixelPosition(UIFunction.GetPixelPosition(LocalMessage.StartGrid));
			StartMove = false;
		} else {
			temp1.SetPixelPosition(transform.localPosition);
		}
		Position temp2 = LocalMessage.MonsterRoute[currentIndex];
		iTween.moveTo(gameObject, Vector3.Distance(temp1.GetPixel(), temp2.GetPixel()) / MoveSpeed, 0, temp2.GetX(), temp2.GetY(), -8.5f, iTween.EasingType.linear, "CompleteMove", null);
	}
}
