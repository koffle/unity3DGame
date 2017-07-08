using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.SocialPlatforms;
using System.Collections.Generic;

public class Menu : MonoBehaviour {
	//private int FontSize = 16;
	public string CurMenu;
	private string Name;
	public string temp;
	bool buttonPress = false;
	public string MatchName;
	public string ClickedDescripton;
	public string ClickedName;
	public int Players;
	Rect windowRect = new Rect(20, 20, 220, 150);
	GUI.WindowFunction windowFunction;
	GUI.WindowFunction WindowFunction;
	int y = 0;
	int x = 0;
	Chat chat;
	Store store;
	Loadout loadout;

	private string testLeaderBoard = "CgkIgbTEuJ0VEAIQAg";
	private string testAchievement = "CgkIgbTEuJ0VEAIQAQ";

	//GUI
	private ResponsiveGui rg;
	public GUISkin skin;
	//Main Menu Textures
	public Texture menuBG;
	public Texture titleImage;
	public Texture playButton;
	public Texture storeButton;
	public Texture optionsButton;
	//Options Menu Textures
	public Texture optionsTitle;
	public Texture optionsWindow;
	//Play Menu Textures
	public Texture playTitle;
	public Texture createButton;
	public Texture joinButton;
	//Host Menu Textures
	public Texture hostTitle;
	public Texture hostWindow;
	public Texture plusButton;
	public Texture minusButton;
	public Texture startButton;
	//Join Menu Textures
	public Texture joinTitle;
	public Texture joinWindow;
	public Texture refreshButton;
	public Texture joinRoomButton;
	//Lobby Meny Textures
	public Texture startLobbyButton;
	public Texture chatLobbyButton;
	public Texture loadoutLobbyButton;
	public Texture backLobbyButton;
	public Texture lobbyTitle;
	public Texture lobbyWindow;
	//Universal Textures
	public Texture playerMenu;
	public Texture backButton;
	
	//Volume
	public AudioSource SFX;
	public AudioSource Music;
	
	//Volume Slider
	public float sfx = 0.0f;
	public float music = 0.0f;

	void Start(){
		GameObject go = GameObject.Find("NetworkManager");
		chat = go.GetComponent<Chat>();
		loadout = GetComponent<Loadout>();
		store = GetComponent<Store>();
		windowFunction = MovableWindow;
		//PlayerPrefs.SetString("PlayerName","");
		if(string.IsNullOrEmpty(PlayerPrefs.GetString ("PlayerName")))
			PlayerPrefs.SetString("PlayerName", "Guest"); 
		Name = PlayerPrefs.GetString ("PlayerName");
		if(PlayerPrefs.GetFloat ("SFXVolume", -1) >= 0) 
		{
			SFX.volume = PlayerPrefs.GetFloat ("SFXVolume");
		}
		else
		{
			SFX.volume = 1.0f;
		}
		if(PlayerPrefs.GetFloat ("MusicVolume", -1) >= 0)
		{
			Music.volume = PlayerPrefs.GetFloat ("MusicVolume");
		}
		else
		{
			Music.volume = 1.0f;
		}
		PlayerPrefs.SetString ("Authenticated","false");
		Social.Active = new UnityEngine.SocialPlatforms.GPGSocial();
		ToMenu ("Main");
		rg = new ResponsiveGui ();
	}

	public void ToMenu (string menu){
		CurMenu = menu;
	}

	public void OptionsSaveAndApply (string newName, float sfxPercent, float musicPercent) 
	{
		if (string.IsNullOrEmpty (newName)) 
		{
			//Leave Empty
		}
		else 
		{
			PlayerPrefs.SetString ("PlayerName", newName);
			Name = newName;
		}
		PlayerPrefs.SetFloat ("SFXVolume", sfxPercent);
		PlayerPrefs.SetFloat ("MusicVolume", musicPercent);
		SFX.volume = sfxPercent;
		Music.volume = musicPercent;
		
	}

	void OnGUI(){
		//GUI.skin.label.fontSize = GUI.skin.textField.fontSize = GUI.skin.button.fontSize = FontSize;
		GUI.skin=skin;

		rg.StartResizer();
		GUI.depth = 10;
		GUI.DrawTexture (new Rect (0, 0, rg.getNativeWidth (), rg.getNativeHeight()), menuBG, ScaleMode.ScaleToFit, true, 0); 
		GUI.DrawTexture (new Rect (1142, 494, 460, 504), playerMenu);
		//Main/ Start Menu
		if (CurMenu == "Main") 
		{
			GUI.DrawTexture (new Rect(941, 41, 947, 308), titleImage, ScaleMode.ScaleToFit, true, 0);
			if (GUI.Button (new Rect(212, 34, 399, 272), playButton))
			{
				ToMenu ("Play");
			}
			else if (GUI.Button (new Rect(212, 346, 399, 272), storeButton))
			{
				ToMenu ("Store");
			}
			else if (GUI.Button (new Rect(212, 657, 399, 272), optionsButton))
			{
				ToMenu ("Options");
			}
		}
		else if (CurMenu == "Play")
		{
			GUI.DrawTexture (new Rect(941, 41, 947, 308), playTitle, ScaleMode.ScaleToFit, true, 0);
			if (GUI.Button (new Rect(212, 34, 399, 272), createButton))
			{
				ToMenu ("Host");
			}
			else if (GUI.Button (new Rect(212, 346, 399, 272), joinButton))
			{
				MasterServer.RequestHostList("Team7WorkingTitle");
				ToMenu("Join");
			}
			else if (GUI.Button (new Rect(212, 657, 399, 272), backButton))
			{
				ToMenu ("Main");
			}
		}
		else if (CurMenu == "Options") 
		{
			GUI.depth = 2;
			GUI.DrawTexture (new Rect(941, 41, 947, 308), optionsTitle, ScaleMode.ScaleToFit, true, 0);
			GUI.DrawTexture (new Rect(0, 0, 855, 651), optionsWindow, ScaleMode.ScaleToFit, true, 0);
			GUI.depth = 0;
			sfx = GUI.HorizontalSlider(new Rect(60,96,700,150), sfx, 0.0f, 1.0f);
			music = GUI.HorizontalSlider (new Rect(60,280,700,150), music, 0.0f, 1.0f);
			Name = GUI.TextField(new Rect(60, 480, 700, 100), Name);
			
			if (GUI.Button (new Rect(212, 657, 399, 272), backButton))
			{
				OptionsSaveAndApply (Name, sfx, music);
				ToMenu("Main");
			}
		}
		else if (CurMenu == "Store")
		{
			store.showStore = true;
		}
		else if (CurMenu == "Host")
		{
			GUI.depth = 2;
			GUI.DrawTexture (new Rect(941, 41, 947, 308), hostTitle, ScaleMode.ScaleToFit, true, 0);
			GUI.DrawTexture (new Rect(0, 0, 855, 651), hostWindow, ScaleMode.ScaleToFit, true, 0);
			GUI.depth = 1;
			MatchName = GUI.TextField (new Rect(60, 96, 700, 100), MatchName);
			Players = Mathf.Clamp (Players, 2, 8);
			if (GUI.Button (new Rect(60, 280, 139, 97), plusButton))
			{
				Players += 2;
			}
			else if (GUI.Button (new Rect(300, 280, 139, 97), minusButton)) 
			{
				Players -= 2;
			}
			GUI.depth = 0;
			GUI.Label(new Rect(235, 260, 75, 97), Players.ToString ());
			if (GUI.Button (new Rect(199, 449, 235, 160), startButton)) 
			{
				if(!string.IsNullOrEmpty(MatchName) && Players != 0)
				{
					NetworkManager.Instance.StartServer(MatchName, Players-1);
					MatchName = "";
					Players = 0;
					ToMenu ("Lobby");
				}
			}
			else if (GUI.Button (new Rect(212, 657, 399, 272), backButton))
			{
				ToMenu ("Play");
			}
		}
		else if (CurMenu == "Join")
		{
			GUI.depth = 2;
			GUI.DrawTexture (new Rect(941, 41, 947, 308), joinTitle, ScaleMode.ScaleToFit, true, 0);
			GUI.DrawTexture (new Rect(0, 0, 855, 651), joinWindow, ScaleMode.ScaleToFit, true, 0);
			GUI.depth = 1;
			GUILayout.BeginArea (new Rect(15,90,850,590));
			foreach (HostData hd in MasterServer.PollHostList()) 
			{

				GUILayout.BeginHorizontal();
				string name = hd.gameName + " " + hd.connectedPlayers + "/" + hd.playerLimit;
				GUILayout.Label(name, "noLabelSmall", null);
				//GUILayout.Space(20);
				//string hostInfo = "[" + hd.ip[0] + ":" + hd.port + "]";
				//GUILayout.Label(hostInfo, "noLabelSmall", null);
				GUILayout.Space(20);
				GUILayout.FlexibleSpace();
				if (GUILayout.Button(joinRoomButton, GUILayout.Height(139))){
					Network.Connect (hd);
					ToMenu ("Lobby");
				}
				GUILayout.EndHorizontal();
			}
			GUILayout.EndArea ();
			if (GUI.Button (new Rect(199, 449, 235, 160), refreshButton))
			{
				MasterServer.ClearHostList ();
				MasterServer.RequestHostList("Team7WorkingTitle");	
			}
			else if (GUI.Button (new Rect(212, 657, 399, 272), backButton))
			{
				MasterServer.ClearHostList();
				ToMenu ("Play");
			}

		}
		else if (CurMenu == "Lobby")
		{
			if (Network.isServer)
			{
				if (GUI.Button (new Rect(137,82,235, 160), startLobbyButton))
				{
					NetworkManager.Instance.GetComponent<NetworkView>().RPC ("StartGame", RPCMode.All);
				}
			}
			GUI.depth = 2;
			GUI.DrawTexture (new Rect(941, 41, 947, 308), lobbyTitle, ScaleMode.ScaleToFit, true, 0);
			GUI.DrawTexture (new Rect(137,431, 815, 567), lobbyWindow, ScaleMode.ScaleToFit, true, 0);
			if (GUI.Button (new Rect(460,82,235, 160), chatLobbyButton))
			{
				chat.Show = !chat.Show;
				chat.InputString = "";
			}
			else if (GUI.Button (new Rect(137,262,235, 160), loadoutLobbyButton))
			{
				loadout.show = !loadout.show;
				loadout.init();
				chat.Show = false;
				chat.InputString = "";
				ToMenu("Loadout");
			}
			else if (GUI.Button (new Rect(460,262,235, 160), backLobbyButton))
			{
				chat.Show = false;
				chat.InputString = "";
				chat.Messages.Clear ();
				if(Network.isServer)
					MasterServer.UnregisterHost();
				Network.Disconnect();
				ToMenu ("Play");
			}
			GUILayout.BeginArea (new Rect(151,516, 790, 457));
			GUI.contentColor = Color.black;
			foreach(Player pl in NetworkManager.Instance.PlayerList)
				GUILayout.Label(pl.PlayerName, "noLabel", null);
			GUILayout.EndArea ();
		}
		//rg.EndResizer() has to be last line of OnGUI()
		rg.EndResizer ();
	}

	void Update(){

	}

	void OnAuthCB(bool result) {
		Debug.Log("GPGUI: Got Login Response: " + result);
		if (result) {
			PlayerPrefs.SetString ("Authenticated","true");
			//for(int i=0;i<4;i++)
			//	NerdGPG.Instance().loadFromCloud(i);
			Social.LoadAchievements (OnLoadAC);
		}
	}
	
	public void OnLoadAC(IAchievement[] achievements) {
		Debug.Log("GPGUI: Loaded Achievements: " + achievements.Length);
	}
	
	public void OnLoadACDesc(IAchievementDescription[] acDesc) {
		Debug.Log("GPGUI: Loaded Achievement Description: " + acDesc.Length);
	}

	public void MovableWindow(int windowID){
		GUI.Label(new Rect(10,20,150,230),ClickedDescripton);
		if(GUI.Button(new Rect(15,110,80,20), "Confirm")){
		}
		if(GUI.Button(new Rect(125,110,80,20), "Cancel")){
			windowRect = new Rect(20, 20, 220, 150);
			buttonPress = false;
		}
//			Debug.Log("Got a click");
		GUI.DragWindow();

	}


	public void OnSubmitScore(bool result) {
		Debug.Log("GPGUI: OnSubmitScore: " + result);
	}
	
	public void OnUnlockAC(bool result) {
		Debug.Log("GPGUI: OnUnlockAC " + result);
	}
}