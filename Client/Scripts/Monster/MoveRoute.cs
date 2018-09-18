using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoute : MonoBehaviour {
	public GameObject MapPanel;//地图区域
	public static float MoveSpeed = 2.5f;//移动速度
	public const byte UP = 0;//上
	public const byte LEFT = 1;//左
	public const byte DOWN = 2;//下
	public const byte RIGHT = 3;//右
	public const byte NONE = 255;//不相邻
	private byte currentDirection = NONE;//当前的方向
	private ushort currentIndex = 0;//当前位置索引
	private Position currentPosition;
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
	}

	void OnEnable() {

	}
	
	// Update is called once per frame
	void Update () {
		transform.Translate(Vector3.down * MoveSpeed * Time.deltaTime);
		if(currentIndex < LocalMessage.MonsterRoute.Count) {
			currentPosition = LocalMessage.MonsterRoute[currentIndex] as Position;
			byte dir = currentPosition.GetDirection();
			if(dir != currentDirection) {
				currentIndex++;
				if(currentDirection == UP && dir == LEFT) transform.Rotate(0, 0, -90);
				if(currentDirection == RIGHT && dir == UP) transform.Rotate(0, 0, -90);
				if(currentDirection == DOWN && dir == RIGHT) transform.Rotate(0, 0, -90);
				if(currentDirection == LEFT && dir == DOWN) transform.Rotate(0, 0, -90);
				currentDirection = dir;
			}
		}
		if(currentPosition.GetLine() == LocalMessage.EndGrid.line && currentPosition.GetColumn() == LocalMessage.EndGrid.column) {
			switch(currentDirection) {
				case UP:if(currentPosition.GetY() <= EndPoint.y) ArriveEndPoint();break;
				case DOWN:if(currentPosition.GetY() >= EndPoint.y) ArriveEndPoint();break;
				case LEFT:if(currentPosition.GetX() <= EndPoint.x) ArriveEndPoint();break;
				case RIGHT:if(currentPosition.GetX() >= EndPoint.x) ArriveEndPoint();break;
			}
		}
	}

	void OnDisable() {
		Destroy(gameObject);
	}
	
	private void ArriveEndPoint() {
		Destroy(gameObject);
	}
}
