using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEditor;

public class GameRunning : MonoBehaviour {

	public GameObject RoomPanel;//房间界面
	public GameObject MapPanel;//地图区域
	public GameObject MonsterInfoPanel;//怪物信息界面
	public Text Timer;//倒计时
	public Text Gold_Text;//金币
	public Text CheckPoint_Text;//当前关卡数
	public Text KillNumber_Text;//杀敌数
	public Text HPNumber_Text;//剩余生命值

	private readonly string[] Floor = {"Prefabs/ground", "Prefabs/glass", "Prefabs/stone", "Prefabs/begin", "Prefabs/end"};
	public const string check_point_data = "Data/CheckPoints.xml";
	public const string tower_data = "Data/Towers.xml";
	private readonly string[] Armor = {"", "none", "leather", "wood", "iron", "steel", "diamond", "magic"};
	public static float MapWidth = ((float)Screen.width * (668.0f / 1024.0f));
	public static float MapHeight = ((float)Screen.height * (668.0f / 768.0f));
	public static float BaseLength = MapWidth > MapHeight ? MapHeight : MapWidth;
	public static float EnlargRatio = BaseLength / 668.0f;
	private static float max = MapWidth > MapHeight ? MapWidth : MapHeight;
	public static float y_offset = (max - MapWidth) / 2, x_offset = (max - MapHeight) / 2;
	private static int current_check_point = 1;//当前关卡数
	private XMLFileController xml = new XMLFileController();
	public static XMLFileController TowerFileXml = new XMLFileController();
	private Coroutine CreateMonsterThread = null;

	private const string READY_TIME = "ready_time";
	private const string HIT_POINT = "hit_point";
	private const string DEFENSE_POINT = "defense_point";
	private const string DEFENSE_TYPE = "defense_type";
	private const string MOVE_SPEED = "move_speed";
	private const string AMOUNT = "amount";
	private const string BONUS = "bonus";
	private const string INTERVAL_TIME = "interval_time";
	private const string BOSS_CHECK_POINT = "boss_ck";
	private const string BOSS_HIP_POINT = "boss_hp";
	private const string BOSS_DEFENSE_POINT = "boss_dp";
	private const string BOSS_MOVE_SPEED = "boss_ms";
	private const string BOSS_DEFENSE_TYPE = "boss_dt";
	private const string BOSS_BONUS = "boss_bonus";

	void Start() {
		BulletInfo.ReadFile();
		TowerFileXml.Open(tower_data);
	}
	// Use this for initialization
	void OnEnable () {
		MapWidth = (float)Screen.width * (668.0f / 1024.0f);
		MapHeight = (float)Screen.height * (668.0f / 768.0f);
		BaseLength = MapWidth > MapHeight ? MapHeight : MapWidth;
		max = MapWidth > MapHeight ? MapWidth : MapHeight;
		y_offset = (max - MapWidth) / 2;
		x_offset = (max - MapHeight) / 2;
		EnlargRatio = BaseLength / 668.0f;
		byte[] testMap = new byte[]{0, 0, 0, 0, 20, 20, 
								3, 0, 1, 18,//地图路线的起点和终点
								//地图开始
								85, 85, 85, 85,  85,
								85, 85, 85, 85,  69,
								85, 85, 85, 85,  69,
								64, 5,  0,  0,   69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								69, 69, 85, 21,  69,
								5,  64, 85, 21,  69,
								85, 85, 85, 21,  69,
								85, 85, 85, 21,  69,
								1,  0,  0,  0,   69,
								81, 85, 85, 85,  69,
								81, 85, 85, 85,  69,
								1,  0,  0,  0,   64,
								85, 85, 85, 85,  85,
								85, 85, 85, 85,  85,
								//地图结束
								0, 0, 0, 0, 0, 0, 0, 0};
		LocalMessage.SetMap(testMap);
		LoadMap();
		CreateMonsterThread = StartCoroutine(AttackOnMonster());
	}
	//加载地图
	private void LoadMap(){	
		// //设置地图区域的位置
		UIFunction.SetPosition(MapPanel, new Vector2(0, 0));
		//设置地图区域的大小
		UIFunction.SetSize(MapPanel, new Vector2(BaseLength, BaseLength));

		int line = LocalMessage.Map.Length, column = LocalMessage.Map[0].Length;
		float width = BaseLength / column, height = BaseLength / line, halfWidth = width / 2, halfHeight = height / 2;
		Vector3 scale = new Vector3(width, height, 0.01f);
		for(int i = 0; i < line; i++){
			for(int j = 0; j < column; j++){
				string temp = Floor[LocalMessage.Map[i][j]];
				if(i == LocalMessage.StartGrid.line && j == LocalMessage.StartGrid.column)
					temp = Floor[3];
				else if(i == LocalMessage.EndGrid.line && j == LocalMessage.EndGrid.column)
					temp = Floor[4];
				GameObject go = (GameObject)Instantiate(Resources.Load(temp));
				go.transform.localEulerAngles = new Vector3(0, 0, 180);
				go.transform.SetParent(MapPanel.transform);
				go.name = go.name.Replace("(Clone)", "");
				UIFunction.SetScale(go, scale);
				UIFunction.Set3DPosition(go, new Vector3(halfHeight + j * height + x_offset, - (halfWidth + i * width) + y_offset, -0.09f));
			}
		}
		LocalMessage.grid.width = width;
		LocalMessage.grid.height = height;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisable(){
		xml.Close();
		TowerFileXml.Close();
		UIFunction.ClearChild(MapPanel);
		LocalMessage.SetHandler(null);
	}
	//产生怪物
	private IEnumerator AttackOnMonster() {
		xml.Open(check_point_data);
		//先加载开始信息
		xml.BeginParentNode("start_setting");
		Gold_Text.text = xml.GetValue("coin");
		HPNumber_Text.text = xml.GetValue("hp");
		xml.EndParentNode();

		LinkedList<string> cp = xml.GetChildren();
		float small = LocalMessage.grid.width > LocalMessage.grid.height ? LocalMessage.grid.height : LocalMessage.grid.width;
		Vector3 scale = new Vector3(small, small, small);
		for(current_check_point = 1; current_check_point <= cp.Count - 1; current_check_point++) {
			ushort new_type = 0;
			//获取下一关的护甲
			if(xml.HasNode("number_" + (current_check_point + 1).ToString())) {
				xml.BeginParentNode("number_" + (current_check_point + 1).ToString());
				new_type = ushort.Parse(xml.GetValue(DEFENSE_TYPE));
				xml.EndParentNode();
			}
			//先加载准备时间
			xml.BeginParentNode("number_" + current_check_point.ToString());
			int ready_time = int.Parse(xml.GetValue(READY_TIME));
			for(; ready_time > 0; ready_time--) {
				Timer.text = ready_time.ToString();
				yield return new WaitForSeconds(1);
			}
			Timer.text = "0";
			CheckPoint_Text.text = current_check_point.ToString();
			//准备时间结束，产生怪物
			string defense_type = Armor[byte.Parse(xml.GetValue(DEFENSE_TYPE))];
			int amount = int.Parse(xml.GetValue(AMOUNT));
			MonsterProperty temp_mp = new MonsterProperty().ParseArmorType(xml.GetValue(DEFENSE_TYPE))
				.ParseBonus(xml.GetValue(BONUS)).ParseDefensePoint(xml.GetValue(DEFENSE_POINT))
				.ParseHipPoint(xml.GetValue(HIT_POINT)).ParseSpeed(xml.GetValue(MOVE_SPEED));
			MonsterInfoPanel.SendMessage("SetMonsterProperties", temp_mp);//MonsterInfo.cs
			MonsterInfoPanel.SendMessage("SetNextArmorType", new_type);//MonsterInfo.cs
			float interval_time = float.Parse(xml.GetValue(INTERVAL_TIME)) / 1000;
			for(;amount > 0; amount--) {
				CreateMonster("Monster/" + defense_type, temp_mp, scale);
				yield return new WaitForSeconds(interval_time);
			}
			//如果是boss关卡，则产生一个BOSS
			if(byte.Parse(xml.GetValue(BOSS_CHECK_POINT)) == 1) {
				temp_mp = new MonsterProperty().ParseArmorType(xml.GetValue(BOSS_DEFENSE_TYPE))
					.ParseBonus(xml.GetValue(BOSS_BONUS)).ParseDefensePoint(xml.GetValue(BOSS_DEFENSE_POINT))
					.ParseHipPoint(xml.GetValue(BOSS_HIP_POINT)).ParseSpeed(xml.GetValue(BOSS_MOVE_SPEED));
				CreateMonster("BOSS/BOSS" + current_check_point.ToString(), temp_mp, scale);
			}
			xml.EndParentNode();
		}
		xml.Close();
	}
	private void CreateMonster(string monster, MonsterProperty message, Vector3 scale) {
		GameObject go = (GameObject)Instantiate(Resources.Load("Prefabs/" + monster));
		go.transform.SetParent(MapPanel.transform);
		UIFunction.SetScale(go, scale);
		go.SetActive(true);
		go.SendMessage("SetMessageReceiver", gameObject);//MoveRoute.cs
		go.SendMessage("LoadCurrentCheckPointMessage", message);//MonsterProperties.cs
	}
	/* 跑掉一个怪物，减少生命值   MoveRoute.cs调用
	   param[num]:减少的生命值数
	 */
	private void EscapeMonster(int num = 1) {
		int hp = int.Parse(HPNumber_Text.text) - num;
		HPNumber_Text.text = hp.ToString();
		if(hp <= 0) 
			LoseGame();
	}
	//宣告游戏失败
	private void LoseGame() {
		StopCoroutine(CreateMonsterThread);
		xml.Close();
		EditorUtility.DisplayDialog("游戏失败", "游戏失败，请回去练练吧", "关闭");
		
		//gameObject.SetActive(false);
		//RoomPanel.SetActive(true);
	}
}
