using System.Collections;
using System.Collections.Generic;
public class BulletInfo {
	private const string FLY_SPEED = "fly_speed";
	private const string DataFile = "Data/BulletInfo.xml";
	static private Dictionary<string, float> BulletFlySpeed = new Dictionary<string, float>();
	static public void ReadFile() {
		if(BulletFlySpeed.Count > 0) return;
		XMLFileController file = new XMLFileController();
		file.Open(DataFile);
		file.BeginParentNode(FLY_SPEED);
		LinkedList<string> child = file.GetChildren();
		foreach (string temp in child) {
			BulletFlySpeed.Add(temp, float.Parse(file.GetValue(temp)));
		}
		file.Close();
	}
	static public float GetFlySpeed(string bullet) {
		if(BulletFlySpeed.ContainsKey(bullet))
			return BulletFlySpeed[bullet];
		return 0;
	}
}
