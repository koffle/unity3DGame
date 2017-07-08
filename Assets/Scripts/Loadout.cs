using UnityEngine;
using System.Collections;
using System.IO;

using System.Collections.Generic;


public class Loadout : MonoBehaviour {
	Vector2 v = new Vector2(0,0);
	public string[] items ;
	public static int mechCounter,mechScroll,LarmCounter,LarmScroll,Rarmcounter,RarmScroll,hatCounter,hatScroll,legsCounter,legsScroll,shieldCounter,shieldScroll,miscCounter,miscScroll;
	public bool show;
	public int[] PlayerItems;
	public int [] mechs,Larms,Rarms,Hats,Legs,shields,Miscs;
	public static Texture2D [] mechsImg,LarmsImg,RarmsImg,HatsImg,LegsImg,shieldsImg,MiscsImg;
	public string [] mechName,larmName,rarmName,hatName,legsName,shieldName,miscName;
	public Texture2D [] img;
	const int CHUNK_SIZE = 2 * 1024;
	public byte[] buffer = new byte[CHUNK_SIZE];
	public string [] name;
	public string [] description;
	public int [] Inventory;
	public string [] type;
	public static Loadout instance;
	protected SqliteDatabase db = new SqliteDatabase();
	Menu menu;
	public string temp;
	public string [] loadout;
	string scrollSave;
	
	public string [] weaponItems;
	public string [] hatItems;
	public string [] shieldItems;
	public string [] legsItems;
	public string [] mechsItems;
	
	public List<Weapon> weapon;
	public List<Hat> hat;
	public List<Leg> leg;
	public List<Shield> shield;
	public List<Mech> mech;
	//WWW linkstream;
	//	Weapon[] weaps;
	
	void Start(){
		weapon = gameObject.GetComponent<Store>().weapon;
		hat = gameObject.GetComponent<Store>().hat;
		leg = gameObject.GetComponent<Store>().leg;
		shield = gameObject.GetComponent<Store>().shield;
		mech =  gameObject.GetComponent<Store>().mech;
		
		//PlayerPrefs.SetString ("loadout2", "0;1;2;0;0;0;");
		//	PlayerPrefs.SetString ("loadout3", "0;1;2;0;0;0;");

		//PlayerPrefs.SetString ("loadout1", "0;0;1;0;0;0;");
		loadout = PlayerPrefs.GetString("loadout1","0;0;1;0;0;0;").Split(";"[0]);
		
		//loadout = PlayerPrefs.GetString("loadout"+mechScroll+1);
		hatScroll = System.Int32.Parse(loadout[0]);
		LarmScroll = System.Int32.Parse(loadout[1]);
		RarmScroll = System.Int32.Parse(loadout[2]);
		legsScroll = System.Int32.Parse(loadout[3]);
		shieldScroll = System.Int32.Parse(loadout[4]);
		miscScroll = System.Int32.Parse(loadout[5]);
		mechScroll = 0;
		
		NetworkManager.Instance.MyPlayer.PlayerItems = new int[] {	hatScroll,
			LarmScroll,
			RarmScroll,
			shieldScroll,
			legsScroll,
			miscScroll };
		//		weaps = NetworkManager.Instance.Weapons;
		
		Debug.Log (NetworkManager.Instance.MyPlayer.PlayerItems[0]+"|"+NetworkManager.Instance.MyPlayer.PlayerItems[1]+"|"+NetworkManager.Instance.MyPlayer.PlayerItems[2]+"|"+NetworkManager.Instance.MyPlayer.PlayerItems[3]+"|"+NetworkManager.Instance.MyPlayer.PlayerItems[4]+"|"+NetworkManager.Instance.MyPlayer.PlayerItems[5]);
	}
	public void init() {
		
		
		
		
		mechCounter = 0;
		LarmCounter = 0;
		Rarmcounter = 0;
		legsCounter = 0;
		hatCounter = 0;
		shieldCounter = 0;
		miscCounter = 0;
		
		mechs = new int[10];
		Larms = new int[10];
		Rarms = new int[10];
		Hats = new int[10];
		Legs = new int[10];
		shields = new int[10];
		Miscs = new int[10];
		mechName = new string[10];
		larmName= new string[10];
		rarmName = new string[10];
		hatName = new string[10];
		legsName = new string[10];
		shieldName = new string[10];
		miscName = new string[10];
		mechsImg= new Texture2D[10];
		LarmsImg= new Texture2D[10];
		RarmsImg= new Texture2D[10];
		HatsImg= new Texture2D[10];
		LegsImg= new Texture2D[10];
		shieldsImg= new Texture2D[10];
		MiscsImg = new Texture2D[10];
		temp = PlayerPrefs.GetString("InventoryItems");
		items = PlayerPrefs.GetString("InventoryItems","1,2,3;1;1;1;1,2,3;").Split(";"[0]);
		weaponItems = items[0].Split(","[0]);
		hatItems = items[1].Split(","[0]);
		shieldItems = items[2].Split(","[0]);
		legsItems = items[3].Split(","[0]);
		mechsItems = items[4].Split(","[0]);
		
		
		instance = this;
		img = new Texture2D[items.Length];
		name = new string[items.Length];
		type = new string[items.Length];
		description = new string[items.Length];
		Inventory = new int[items.Length];
		
		menu = gameObject.GetComponent<Menu>();
		
		
		
		/*for (int i = 0; i < mechsItems.Length; i++){
			mechs[mechCounter]    = mech[int.Parse(mechsItems[i])-1].id;
			mechsImg[mechCounter] = mech[int.Parse(mechsItems[i])-1].image;
			mechName[mechCounter] = mech[int.Parse(mechsItems[i])-1].name;
			mechCounter++;
		}*/
		for (int i = 0; i < weaponItems.Length; i++){
			Larms[LarmCounter] = weapon[int.Parse(weaponItems[i])-1].id;
			Rarms[Rarmcounter] = weapon[int.Parse(weaponItems[i])-1].id;
			LarmsImg[LarmCounter] = weapon[int.Parse(weaponItems[i])-1].image;
			RarmsImg[Rarmcounter] = weapon[int.Parse(weaponItems[i])-1].image;
			rarmName[Rarmcounter] = weapon[int.Parse(weaponItems[i])-1].name;
			larmName[LarmCounter] = weapon[int.Parse(weaponItems[i])-1].name;
			Rarmcounter++;
			LarmCounter++;
		}
		for (int i = 0; i < hatItems.Length; i++){
			Hats[hatCounter] = hat[int.Parse(hatItems[i])-1].id;
			HatsImg[hatCounter] = hat[int.Parse(hatItems[i])-1].image;
			hatName[hatCounter] = hat[int.Parse(hatItems[i])-1].name;
			hatCounter++;
		}
		/*for (int i = 0; i < legsItems.Length; i++){
			Legs[legsCounter] = leg[int.Parse(legsItems[i])-1].id;
			LegsImg[legsCounter] = leg[int.Parse(legsItems[i])-1].image;
			legsName[legsCounter] = leg[int.Parse(legsItems[i])-1].name;
			legsCounter++;
		}*/
		for (int i = 0; i < shieldItems.Length; i++){
			shields[shieldCounter] = shield[int.Parse(shieldItems[i])-1].id;
			shieldsImg[shieldCounter] = shield[int.Parse(shieldItems[i])-1].image;
			shieldName[shieldCounter] = shield[int.Parse(shieldItems[i])-1].name;
			shieldCounter++;
		}
		
		
		
	}
	
	
	void OnGUI() {
		if (show){
			GUILayout.BeginArea (new Rect(0,0,Screen.width,Screen.height));
			
			Rect mechL= new Rect(0, 300, 60, 60); 
			Rect mechR= new Rect(170, 300, 60, 60); 
			Rect larmL= new Rect(240, 210, 60, 60); 
			Rect larmR= new Rect(360, 210, 60, 60); 
			Rect rarmL= new Rect(440, 210, 60, 60); 
			Rect rarmR= new Rect(560, 210, 60, 60); 
			Rect hatL= new Rect(640, 210, 60, 60); 
			Rect hatR= new Rect(760, 210, 60, 60); 
			Rect legsL= new Rect(240, 490, 60, 60); 
			Rect legsR= new Rect(360, 490, 60, 60);
			Rect shieldL= new Rect(440, 490, 60, 60); 
			Rect shieldR= new Rect(560, 490, 60, 60);
			Rect miscL= new Rect(640, 490, 60, 60); 
			Rect miscR= new Rect(760, 490, 60, 60);
			Rect mechTitle = new Rect(75,270,100,100);
			Rect larmTitle = new Rect(305,190,100,100);
			Rect rarmTitle = new Rect(505,190,100,100);					
			Rect hatTitle = new Rect(700 ,190 ,100 ,100);
			Rect legsTitle = new Rect(305,470,100,100);
			Rect shieldTitle = new Rect(505,470,100,100);
			
			GUIContent mech = new GUIContent();
			GUIContent larm = new GUIContent();
			GUIContent rarm = new GUIContent();
			GUIContent legs = new GUIContent();
			GUIContent hat = new GUIContent();
			GUIContent misc = new GUIContent();
			GUIContent shield = new GUIContent();
			
			mech.image = mechsImg[mechScroll];
			mech.text = mechName[mechScroll];
			
			
			larm.image = LarmsImg[LarmScroll];
			larm.text = larmName[LarmScroll];
			
			
			rarm.image = RarmsImg[RarmScroll];
			rarm.text = rarmName[RarmScroll];
			
			
			hat.image = HatsImg[hatScroll];
			hat.text = hatName[hatScroll];
			
			
			legs.image = LegsImg[legsScroll];
			legs.text = legsName[legsScroll];
			
			shield.image = shieldsImg[shieldScroll];
			shield.text = shieldName[shieldScroll];
			
			//				misc.image = MiscsImg[miscScroll];
			//				misc.text = miscName[miscScroll];
			
			
			//GUI.Box(new Rect (0, 0, 230, 360),"Mech");
			/*GUILayout.BeginHorizontal();
			GUI.DrawTexture(new Rect(10,50,210,190),mech.image);
			GUI.Label(mechTitle,mech.text);
			
			/*if (GUI.Button(mechL,"<-") && mechScroll > 0 && mechs[mechScroll-1] != null && mechs[mechScroll-1] != 0){
				mechScroll--;
				loadout = PlayerPrefs.GetString("loadout"+mechScroll+1).Split(";"[0]);

				hatScroll = System.Int32.Parse(loadout[0]);
				LarmScroll = System.Int32.Parse(loadout[1]);
				RarmScroll = System.Int32.Parse(loadout[2]);
				legsScroll = System.Int32.Parse(loadout[3]);
				shieldScroll = System.Int32.Parse(loadout[4]);
				miscScroll = System.Int32.Parse(loadout[5]);

			}
		

			GUILayout.Space(75);
			if (GUI.Button(mechR,"->") && mechScroll <= 10 && mechs[mechScroll+1] != null && mechs[mechScroll+1] != 0){
				mechScroll++;
				loadout = PlayerPrefs.GetString("loadout"+mechScroll+1).Split(";"[0]);
				
				hatScroll = System.Int32.Parse(loadout[0]);
				LarmScroll = System.Int32.Parse(loadout[1]);
				RarmScroll = System.Int32.Parse(loadout[2]);
				legsScroll = System.Int32.Parse(loadout[3]);
				shieldScroll = System.Int32.Parse(loadout[4]);
				miscScroll = System.Int32.Parse(loadout[5]);
			}
			
			
			GUILayout.EndHorizontal();*/
			GUILayout.Space(50);
			GUILayout.EndArea();
			//larm
			GUI.Box(new Rect (240, 0, 180, 270),"Left Arm");
			GUILayout.BeginHorizontal();
			GUI.DrawTexture(new Rect(250,50,160,140),larm.image);
			GUI.Label(larmTitle,larm.text);
			if (GUI.Button(larmL,"<-") && LarmScroll > 0 && Larms[LarmScroll-1] != null && Larms[LarmScroll-1] != 0){
				if(LarmScroll-1 == RarmScroll && LarmScroll-2 >= 0){
					LarmScroll -=2;
				}
				else if (LarmScroll-1 == RarmScroll){
					//keep larm from scrolling
				}
				else{
					LarmScroll--;
				}
			}
			GUILayout.Space(75);
			
			if (GUI.Button(larmR,"->") && LarmScroll <= 10 && Larms[LarmScroll+1] != null && Larms[LarmScroll+1] != 0){
				if(LarmScroll+1 == RarmScroll && Larms[LarmScroll+2] != 0){
					LarmScroll +=2;
				}
				else if(LarmScroll+1 == RarmScroll){
					
				}
				else{
					LarmScroll++;
				}
				
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(50);
			//
			//GUI.Box(new Rect (200, 0, 220, 360),mech);
			//rarm
			GUI.Box(new Rect (440, 0, 180, 270),"Right Arm");
			GUILayout.BeginHorizontal();
			GUI.DrawTexture(new Rect(450,50,160,140),rarm.image);
			GUI.Label(rarmTitle,rarm.text);
			if (GUI.Button(rarmL,"<-") && RarmScroll > 0 && Rarms[RarmScroll-1] != null && Rarms[RarmScroll-1] != 0){
				if(RarmScroll-1 == LarmScroll && RarmScroll-2 >= 0){
					RarmScroll -=2;
				}
				else if (RarmScroll-1 == LarmScroll){
					//keep larm from scrolling
				}
				else{
					RarmScroll--;
				}
				
			}
			GUILayout.Space(75);
			if (GUI.Button(rarmR,"->") && RarmScroll <= 10 && Rarms[RarmScroll+1] != null && Rarms[RarmScroll+1] != 0){
				if(RarmScroll+1 == LarmScroll && Rarms[RarmScroll+2] != 0){
					RarmScroll +=2;
				}
				else if (RarmScroll+1 == LarmScroll){
					
				}
				else{
					RarmScroll++;
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(50);
			//
			//hat
			
			GUI.Box(new Rect (640, 0, 180, 270),"Hat");
			GUILayout.BeginHorizontal();
			GUI.DrawTexture(new Rect(650,50,160,140),hat.image);
			GUI.Label(hatTitle,hat.text);
			if (GUI.Button(hatL,"<-") && hatScroll > 0 && Hats[hatScroll-1] != null && Hats[hatScroll-1] != 0){
				hatScroll--;
			}
			GUILayout.Space(75);
			if (GUI.Button(hatR,"->") && hatScroll <= 10 && Hats[hatScroll+1] != null && Hats[hatScroll+1] != 0){
				hatScroll++;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(50);
			//
			
			//legs
			/*GUI.Box(new Rect (240, 280, 180, 270),"Legs");
			GUILayout.BeginHorizontal();
			
			//GUI.DrawTexture(new Rect(250,330,160,140),legs[legCounter].image);
			//legs.text = legsName[legCounter].name;
			
			GUI.DrawTexture(new Rect(250,330,160,140),legs.image);
			GUI.Label(legsTitle,legs.text);
			if (GUI.Button(legsL,"<-") && legsScroll > 0 && Legs[legsScroll-1] != null && Legs[legsScroll-1] != 0){
				legsScroll--;
			}
			GUILayout.Space(75);
			if (GUI.Button(legsR,"->") && legsScroll <= 10 && Legs[legsScroll+1] != null && Legs[legsScroll+1] != 0){
				legsScroll++;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(50);*/
			//
			
			//shield
			GUI.Box(new Rect (440, 280, 180, 270),"shield");
			GUILayout.BeginHorizontal();
			GUI.DrawTexture(new Rect(450,330,160,140),shield.image);
			GUI.Label(shieldTitle,shield.text);
			if (GUI.Button(shieldL,"<-") && shieldScroll > 0 && shields[shieldScroll-1] != null && shields[shieldScroll-1] != 0){
				shieldScroll--;
			}
			GUILayout.Space(75);
			if (GUI.Button(shieldR,"->") && shieldScroll <= 10 && shields[shieldScroll+1] != null && shields[shieldScroll+1] != 0){
				shieldScroll++;
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(50);
			//
			//misc
			//			GUI.Box(new Rect (640, 280, 180, 270),"Misc");
			//			GUILayout.BeginHorizontal();
			//			GUI.DrawTexture(new Rect(650,330,160,140),misc.image);
			//			GUI.Label(mechTitle,misc.text);
			//			if (GUI.Button(miscL,"<-") && miscScroll > 0 && Miscs[miscScroll-1] != null && mechs[miscScroll-1] != 0){
			//				miscScroll--;
			//			}
			//			GUILayout.Space(75);
			//			if (GUI.Button(miscR,"->") && miscScroll <= 10 && Miscs[miscScroll+1] != null && Miscs[miscScroll+1] != 0){
			//				miscScroll++;
			//			}
			//			GUILayout.EndHorizontal();
			
			GUILayout.Space(110);
			if (GUILayout.Button("Save", GUILayout.Height(80),GUILayout.Width(230))){
				scrollSave = (hatScroll.ToString()+";"+LarmScroll.ToString()+";"+RarmScroll.ToString()
				              +";"+legsScroll.ToString()+";"+shieldScroll.ToString()+";"+miscScroll.ToString()+";");
				Debug.Log (mechScroll+1 + "|" + scrollSave);
				
				PlayerPrefs.SetString("loadout1",scrollSave);
				NetworkManager.Instance.MyPlayer.PlayerItems = new int[] {	hatScroll,
					LarmScroll,
					RarmScroll,
					shieldScroll,
					legsScroll,
					miscScroll };
				show = false;
				menu.ToMenu("Lobby");
				
			}
			
		}
	}
}
