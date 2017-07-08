using UnityEngine;
using System.Collections;
using System.IO;

using System.Collections.Generic;

public class Store : MonoBehaviour 
{

	const int CHUNK_SIZE = 2 * 1024;
	public byte[] buffer = new byte[CHUNK_SIZE];
	DataTable dt = new DataTable();
	public string [] items;
	public string [] weaponItems;
	public string [] hatItems;
	public string [] shieldItems;
	public string [] legsItems;
	public string [] mechsItems;

	public string [] DisplayNames;
	public Texture2D [] DisplayImages;
	public string [] DisplayDescripton;
	public int [] DisplayInventory;

	public GUITexture ShowImage;
	public string saveTemp;

	public int numberOfRows;
	public Vector2 rowSize;
	public Vector2 windowSize;
	public Vector2 listVector;
	public Vector2 listSize;

	public Rect windowRect;   
	private Item item;

	string weaponHolder ="";
	string hatHolder="";
	string shieldHolder="";
	string legsHolder="";
	string mechsHolder="";

	public List<Weapon> weapon;
	public List<Hat> hat;
	public List<Leg> leg;
	public List<Shield> shield;
	public List<Mech> mech;

	public Vector2 scrollPosition;
	public bool select;
	public int selectId;
	public int selectRow;
	private int selected = -1;
	public GUIStyle rowSelectedStyle;
	int y = 0;
	int x = 0;
	bool display;
	public string temp;
	int j;
	bool inCheck;
	public bool showStore;
	Menu menu;
	public bool catagoryClick = false;
	protected SqliteDatabase db = new SqliteDatabase();
	WWW linkstream;

	//GUI Code
	private ResponsiveGui rg;
	public GUISkin skin;
	//UI Textures
	public Texture weaponButton;
	public Texture hatButton;
	public Texture shieldButton;
	public Texture backButton;
	
	void Awake()
	{
	//	Debug.Log (Application.streamingAssetsPath + "/Data.t7");
	//	linkstream = new WWW(Application.streamingAssetsPath+"/Data.t7");
	//	StartCoroutine(MyMethod ());
	}
	void Update() {
		items = PlayerPrefs.GetString("InventoryItems","1,2,3;1;1;1;1,2,3").Split(";"[0]);
		weaponItems = items[0].Split(","[0]);
		hatItems = items[1].Split(","[0]);
		shieldItems = items[2].Split(","[0]);
		legsItems = items[3].Split(","[0]);
		mechsItems = items[4].Split(","[0]);
	}
	void Start(){
		menu = gameObject.GetComponent<Menu>();

		weapon = NetworkManager.Instance.weapon;
		hat = NetworkManager.Instance.hat;
		shield = NetworkManager.Instance.shield;
		leg = NetworkManager.Instance.leg;
		mech = NetworkManager.Instance.mech;
		#if UNITY_ANDROID && !UNITY_EDITOR
		Debug.Log(Application.streamingAssetsPath+"/Data.t7");
		linkstream = new WWW(Application.streamingAssetsPath+"/Data.t7");
		StartCoroutine(AndroidInit());
		#endif

		#if UNITY_EDITOR
		db.Open("Data.t7");
		Init();
		#endif

		rg = new ResponsiveGui ();
	}

	

	void OnGUI(){

		if(showStore){
			rg.StartResizer();
			GUI.depth = 1;
			Rect arms = new Rect(241, 40, 235, 160); 
			//Rect legs = new Rect(x, y+130, 235, 160); 
			Rect shields = new Rect(244, 240, 235, 160); 
			Rect Hats = new Rect(244, 440, 235, 160); 
			Rect back = new Rect(212, 657, 399, 272);
		
		GUIContent content = new GUIContent();
		
			if(GUI.Button(arms, weaponButton, "noStyle")){
			
				saveTemp ="weaponItems";
			
			display = true;
			rowSize.x = 250;
			rowSize.y = 80;

				inCheck = false;
				j = 0;
				DisplayNames = new string[weapon.Count-weaponItems.Length];
				DisplayImages = new Texture2D[weapon.Count-weaponItems.Length];
				DisplayDescripton = new string[weapon.Count-weaponItems.Length];
				DisplayInventory = new int[weapon.Count-weaponItems.Length];
				for (int i = 0; i < weapon.Count; i++){

					for (int temp = 0; temp < weaponItems.Length; temp++){
						
						if(weaponItems[temp].ToString() == weapon[i].id.ToString()){
							inCheck = true;
						}
					}

				if(!inCheck){
					DisplayNames[j] = weapon[i].name;
					DisplayImages[j] = weapon[i].image;
					DisplayDescripton[j]= weapon[i].desc;
					DisplayInventory[j] = weapon[i].id;
					j++;
				}
				else{
					inCheck = false;
				}
				}
				numberOfRows = weapon.Count-weaponItems.Length;
			}
		

			if(GUI.Button(Hats,hatButton, "noStyle")){
			saveTemp ="hatItems";
			display = true;
		
			rowSize.x = 250;
			rowSize.y = 80;
			DisplayNames = new string[hat.Count-hatItems.Length];
			DisplayImages = new Texture2D[hat.Count-hatItems.Length];
			DisplayDescripton = new string[hat.Count-hatItems.Length];
			DisplayInventory = new int[hat.Count-hatItems.Length];
			inCheck = false;
			j = 0;
				for (int i = 0; i < hat.Count; i++){
							
							for (int temp = 0; temp < hatItems.Length; temp++){
								
						if(hatItems[temp].ToString() == hat[i].id.ToString()){
									inCheck = true;
								}
							}
				if(!inCheck){
						Debug.Log("j : "+j+" i : "+i);
						DisplayNames[j] = hat[i].name;
						DisplayImages[j] = hat[i].image;
						DisplayDescripton[j]= hat[i].desc;
						DisplayInventory[j] = hat[i].id;
				j++;
			}
			else{
				inCheck = false;
			}
		}
				numberOfRows = hat.Count-hatItems.Length;
	}
		
			if(GUI.Button(shields,shieldButton, "noStyle")){
			saveTemp ="shieldItems";
			display = true;
			
			rowSize.x = 250;
			rowSize.y = 80;
			DisplayNames = new string[shield.Count-shieldItems.Length];
			DisplayImages = new Texture2D[shield.Count-shieldItems.Length];
			DisplayDescripton = new string[shield.Count-shieldItems.Length];
			DisplayInventory = new int[shield.Count-shieldItems.Length];
			inCheck = false;
			j = 0;
			for (int i = 0; i < shield.Count; i++){
				
				for (int temp = 0; temp < shieldItems.Length; temp++){
					
					if(shieldItems[temp].ToString() == shield[i].id.ToString()){
						inCheck = true;
					}
				}
				if(!inCheck){
					DisplayNames[j] = shield[i].name;
					DisplayImages[j] = shield[i].image;
					DisplayDescripton[j]= shield[i].desc;
					DisplayInventory[j] = shield[i].id;
					j++;
				}
				else{
					inCheck = false;
				}
			}
				numberOfRows = shield.Count-shieldItems.Length;
		}
		if (GUI.Button(back,"Back")){
			display = false;
			showStore = false;
			menu.ToMenu ("Main");
		}
		
		if(display){
			windowSize.x = 980;
			windowSize.y = 30;
			listVector.y = 20;
			listVector.x = 10;
			windowRect = new Rect(350, 10,Screen.width - (2*windowSize.x)/5, Screen.height - (2*windowSize.y)/2-80);
			listSize = new Vector2(windowRect.width - 2*listVector.x, (windowRect.height - 2*listVector.y));
			
			GUI.Window(0, windowRect, (GUI.WindowFunction)MakeWindow, "Store");
		}
			rg.EndResizer ();	
		}
	}

	public void MakeWindow (int windowID) 
	{
		Rect Scroll = new Rect(listVector.x, listVector.y, listSize.x, listSize.y);
		Rect List   = new Rect(0, 0, rowSize.x, numberOfRows*rowSize.y);
		Rect button = new Rect(5, 18, rowSize.x, rowSize.y);

		for (int row = 0; row < numberOfRows; row++)
		{

			if (DisplayInventory[row] == 0)
					{
						if (GUI.Button(button, DisplayNames[row] + "Purchased")){

						}
					}
					else{
				if (GUI.Button(button, DisplayNames[row])){
							select = true;
							selectId = row;
							selectRow = row;
						}

					}
				button.y += rowSize.y;
				
		}
				
				if ( select && DisplayInventory.Length > 0)
				{
					GUI.Box(new Rect (260, 18, 400, 450),DisplayNames[selectRow]);
					
					GUI.DrawTexture(new Rect(270,50,160,160),DisplayImages[selectRow]);
					GUI.Label(new Rect(450,50,200,200),DisplayDescripton[selectRow]);
					
					if (GUI.Button(new Rect(295,380,150,50),"Cancel")){
						display =false;
					}
					if (DisplayInventory[selectRow] != 0){
						if (GUI.Button(new Rect(485,380,150,50),"Buy")){
							
							temp = ","+DisplayInventory[selectRow].ToString();
							weaponHolder ="";
							hatHolder ="";
							shieldHolder ="";
							legsHolder = "";
							mechsHolder ="";
								for (int i = 0; i<weaponItems.Length; i++){
								if(i<weaponItems.Length-1)
									weaponHolder += weaponItems[i].ToString()+",";
								else
									weaponHolder += weaponItems[i].ToString();
								}
								for (int i = 0; i<hatItems.Length; i++){
								if(i<hatItems.Length-1)
									hatHolder += hatItems[i].ToString()+",";
								else
									hatHolder += hatItems[i].ToString();

								}
								for (int i = 0; i<shieldItems.Length; i++){
								if(i<shieldItems.Length-1)
									shieldHolder += shieldItems[i].ToString()+",";
								else
									shieldHolder += shieldItems[i].ToString();
								}
								for (int i = 0; i<legsItems.Length; i++){
								if(i<legsItems.Length-1)
									legsHolder += legsItems[i].ToString()+",";
								else
									legsHolder += legsItems[i].ToString();
								}
								for (int i = 0; i<mechsItems.Length; i++){
								if(i<mechsItems.Length-1)
									mechsHolder += mechsItems[i].ToString()+",";
								else
									mechsHolder += mechsItems[i].ToString();
							}
								
							if (saveTemp == "weaponItems"){
								PlayerPrefs.SetString("InventoryItems",weaponHolder+temp+";"+hatHolder+";"+shieldHolder+";"
								                      +legsHolder+";"+mechsHolder);
							}
							if (saveTemp == "hatItems"){
								PlayerPrefs.SetString("InventoryItems",weaponHolder+";"+hatHolder+temp+";"+shieldHolder+";"
								                      +legsHolder+";"+mechsHolder);
							}
							if (saveTemp == "shieldItems"){
								PlayerPrefs.SetString("InventoryItems",weaponHolder+";"+hatHolder+";"+shieldHolder+temp+";"
								                      +legsHolder+";"+mechsHolder);
							}
							if (saveTemp == "legsItems"){
								PlayerPrefs.SetString("InventoryItems",weaponHolder+";"+hatHolder+";"+shieldHolder+";"
								                      +legsHolder+temp+";"+mechsHolder);
							}

							DisplayInventory[selectRow] = 0;

						}
					}

				}
		
	}

IEnumerator AndroidInit() {
	yield return new WaitForSeconds(1);
	Debug.Log (Application.persistentDataPath + "/Data.t7");
	System.IO.File.WriteAllBytes(Application.persistentDataPath+"/Data.t7",linkstream.bytes);
	//yield return new WaitForSeconds(1);
	db.Open("/data/data/com.Team7.WorkingTitle/files/Data.t7");
	Init ();
}

void Init() {
		items = PlayerPrefs.GetString("InventoryItems").Split(";"[0]);


		//dt = db.ExecuteQuery("SELECT * FROM [TABLE]);
		dt = db.ExecuteQuery("SELECT * FROM Weapons");
			for(int i = 0; i<dt.Rows.Count;i++)
				{		
					weapon.Add(new Weapon(dt[i]));
				}
		dt = db.ExecuteQuery("SELECT * FROM Hats");
		
		
		for(int i = 0; i<dt.Rows.Count;i++)
		{		
			hat.Add(new Hat(dt[i]));
			
		}
		dt = db.ExecuteQuery("SELECT * FROM Shields");
		
		
		for(int i = 0; i<dt.Rows.Count;i++)
		{		
			
			shield.Add(new Shield(dt[i]));
		}
		dt = db.ExecuteQuery("SELECT * FROM Legs");

		for(int i = 0; i<dt.Rows.Count;i++)
		{	
			leg.Add(new Leg(dt[i]));
		}

		dt = db.ExecuteQuery("SELECT * FROM Mechs");
		
		for(int i = 0; i<dt.Rows.Count;i++)
		{	
			mech.Add(new Mech(dt[i]));
		}

	db.Close();

		
	}	
}