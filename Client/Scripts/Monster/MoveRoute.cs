using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRoute : MonoBehaviour {
	public GameObject MapPanel;//地图区域
	public static float MoveSpeed = 2.5f;//移动速度
	public const byte UP = 0;//上
	public const byte DOWN = 1;//下
	public const byte LEFT = 2;//左
	public const byte RIGHT = 3;//右
	private string[] dir = {"上", "下", "左", "右"};
	private GPosition LastPosition;
	private GPosition tempPosition;
	private byte currentDirection = 255;//当前的方向
	private bool read = false;
	// Use this for initialization
	void Start () {
		transform.SetParent(MapPanel.transform);
		float small = LocalMessage.grid.height > LocalMessage.grid.width ? LocalMessage.grid.width : LocalMessage.grid.height;	
		Vector3 scale = new Vector3(small, small, small);
		UIFunction.SetScale(gameObject, scale);
		UIFunction.SetMapPosition(gameObject, LocalMessage.StartGrid);
		LastPosition = new GPosition(){
			line = LocalMessage.StartGrid.line,
			column = LocalMessage.StartGrid.column
		};
		//为物体设置最初的方向
		if(UpTurn(LocalMessage.StartGrid.line, LocalMessage.StartGrid.column))
			currentDirection = UP;
		else if(RightTurn(LocalMessage.StartGrid.line, LocalMessage.StartGrid.column))
			currentDirection = RIGHT;
		else if(DownTurn(LocalMessage.StartGrid.line, LocalMessage.StartGrid.column))
			currentDirection = DOWN;
		else if(LeftTurn(LocalMessage.StartGrid.line, LocalMessage.StartGrid.column))
			currentDirection = LEFT;
		//Debug.Log("最初的方向：" + dir[currentDirection]);
	}

	void OnEnable() {

	}
	
	// Update is called once per frame
	void Update () {
		GPosition gp = UIFunction.GetMapPosition(gameObject);//获取物体当前位置
		if(!tempPosition.Equals(gp)) {
			LastPosition.CopyFrom(tempPosition);
			tempPosition.CopyFrom(gp);
		}
		//判断物体的下一格是在哪一个方向
		switch(currentDirection) {
			case UP:{
				if(!UpTurn(LastPosition, gp) && UpTurn(gp.line, gp.column))
					transform.Rotate(0, 0, 0);
				else if(!RightTurn(LastPosition, gp) && RightTurn(gp.line + 1, gp.column)) {
					transform.Rotate(0, 0, 90);
					currentDirection = RIGHT;
				}
				else if(!LeftTurn(LastPosition, gp) && LeftTurn(gp.line + 1, gp.column)) {
					transform.Rotate(0, 0, -90);
					currentDirection = LEFT;
				}
			};break;
			case DOWN:{
				if(!DownTurn(LastPosition, gp) && DownTurn(gp.line, gp.column))
					transform.Rotate(0, 0, 0);
				else if(!RightTurn(LastPosition, gp) && RightTurn(gp.line, gp.column)) {
					transform.Rotate(0, 0, -90);
					currentDirection = RIGHT;
				}
				else if(!LeftTurn(LastPosition, gp) && LeftTurn(gp.line, gp.column)) {
					transform.Rotate(0, 0, 90);
					currentDirection = LEFT; 
				}
			};break;
			case LEFT:{
				if(!LeftTurn(LastPosition, gp) && LeftTurn(gp.line, gp.column))
					transform.Rotate(0, 0, 0);
				else if(!UpTurn(LastPosition, gp) && UpTurn(gp.line, gp.column + 1)) {
					transform.Rotate(0, 0, 90);
					currentDirection = UP;
				}
				else if(!DownTurn(LastPosition, gp) && DownTurn(gp.line, gp.column + 1)) {
					transform.Rotate(0, 0, -90);
					currentDirection = DOWN; 
				}
			};break;
			case RIGHT:{
				if(!RightTurn(LastPosition, gp) && RightTurn(gp.line, gp.column))
					transform.Rotate(0, 0, 0);
				else if(!DownTurn(LastPosition, gp) && DownTurn(gp.line, gp.column)) {
					transform.Rotate(0, 0, 90);
					currentDirection = DOWN;
				}
				else if(!UpTurn(LastPosition, gp) && UpTurn(gp.line, gp.column)) {
					transform.Rotate(0, 0, -90);
					currentDirection = UP; 
				}
			};break;
		}
		transform.Translate(Vector3.down * MoveSpeed * Time.deltaTime);
		//判断是否已到终点
		switch(currentDirection){
			case UP:if(UpTurn(gp, LocalMessage.EndGrid)) Destroy(gameObject);break;
			case DOWN:if(gp.Equals(LocalMessage.EndGrid)) Destroy(gameObject);break;
			case LEFT:if(LeftTurn(gp, LocalMessage.EndGrid)) Destroy(gameObject);break;
			case RIGHT:if(gp.Equals(LocalMessage.EndGrid)) Destroy(gameObject);break;
		}
	}

	void OnDisable() {
		Destroy(gameObject);
	}
	//判断上方是否怪物路线
	private bool UpTurn(int line, int column) {
		return (line - 1 >= 0) && (LocalMessage.Map[line - 1][column] == 0);
	}
	//判断下方是否怪物路线
	private bool DownTurn(int line, int column) {
		return (line + 1 < LocalMessage.Map.Length) && (LocalMessage.Map[line + 1][column] == 0);
	}
	//判断左方是否怪物路线
	private bool LeftTurn(int line, int column) {
		return (column - 1 >= 0) && (LocalMessage.Map[line][column - 1] == 0);
	}
	//判断右方是否怪物路线
	private bool RightTurn(int line, int column) {
		return (column + 1 < LocalMessage.Map[0].Length) && (LocalMessage.Map[line][column + 1] == 0);
	}
	//判断当前位置是不是在目标位置的上方
	private bool UpTurn(GPosition current, GPosition target) {
		return current.line == target.line - 1 && current.column == target.column;
	}
	//判断当前位置是不是在目标位置的下方
	private bool DownTurn(GPosition current, GPosition target) {
		return current.line == target.line + 1 && current.column == target.column;
	}
	//判断当前位置是不是在目标位置的左方
	private bool LeftTurn(GPosition current, GPosition target) {
		return current.column == target.column - 1 && current.line == target.line;
	}
	//判断当前位置是不是在目标位置的右方
	private bool RightTurn(GPosition current, GPosition target) {
		return current.column == target.column + 1 && current.line == target.line;
	}
}
