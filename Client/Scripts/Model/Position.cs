using UnityEngine;
//地图的一格
public struct Grid {
	public float width;//宽度
	public float height;//高度
}
//按格分地图
public struct GPosition {
	public byte line;//行号
	public byte column;//列号
	public bool Equals(GPosition arg) {
		return line == arg.line && column == arg.column;
	}
	public void CopyFrom(GPosition src) {
		line = src.line;
		column = src.column;
	}
	public byte Adjacent(GPosition arg) {return Adjacent(this, arg);}
	static public GPosition operator <<(GPosition arg, int num) {
		if(num > 0) arg.line += (byte)num;
		else arg.line -= (byte)num;
		return arg;
	}
	static public GPosition operator >>(GPosition arg, int num) {
		if(num > 0) arg.column += (byte)num;
		else arg.column -= (byte)num;
		return arg;
	}
	//获取dst在src的旁边哪一个方向
	//return:返回方向，若不相邻则返回NONE
	static public byte Adjacent(GPosition dst, GPosition src) {
		if(dst.line == src.line) {
			if(dst.column == src.column + 1)
				return MoveRoute.RIGHT;
			else if(dst.column == src.column - 1)
				return MoveRoute.LEFT;
		} else if(dst.column == src.column) {
			if(dst.line == src.line + 1)
				return MoveRoute.DOWN;
			else if(dst.line == src.line - 1)
				return MoveRoute.UP;
		}
		return MoveRoute.NONE;
	}
}
public class Position {
	private GPosition grid_pos = new GPosition(){
		line = MoveRoute.NONE,
		column = MoveRoute.NONE
	};//地图中的格子位置
	private Vector2 pixel_pos = new Vector2(){
		x = 0,
		y = 0
	};//像素位置
	private byte direction = MoveRoute.NONE;//当前格子的移动方向
	public Position(){}
	public Position(GPosition gpos, Vector2 ppos, byte dirc) {
		SetGridPosition(gpos);
		SetPixelPosition(ppos);
		direction = dirc;
	}
	public void SetDirection(byte dirc) {
		direction = dirc;
	}

	public void CopyFrom(Position arg) {
		SetGridPosition(arg.grid_pos);
		SetPixelPosition(arg.pixel_pos);
	}
	public void SetGridPosition(GPosition arg) {
		grid_pos.CopyFrom(arg);
	}
	public void SetPixelPosition(Vector2 arg) {
		pixel_pos.x = arg.x;
		pixel_pos.y = arg.y;
	}
	public byte GetDirection() {
		return direction;
	}
	public byte GetLine() {
		return grid_pos.line;
	}
	public byte GetColumn() {
		return grid_pos.column;
	}
	public float GetX() {
		return pixel_pos.x;
	}
	public float GetY() {
		return pixel_pos.y;
	}
	public Vector2 GetPixel() {
		return pixel_pos;
	}
}
