using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NetworkManager : MonoBehaviour {
	public string PlayerName;
	public string MatchName;
	public static NetworkManager Instance;
	public List<Player> PlayerList = new List<Player>();
	public Player MyPlayer;
	public GameObject SpawnPlayer;
	public Color32[] LevelPix;
	public bool update32 = false;
	public float[] updateCollider;
	Texture2D texture;
	public GameObject[] shells; 

	public List<Weapon> weapon;
	public List<Hat> hat;
	public List<Leg> leg;
	public List<Shield> shield;
	public List<Mech> mech;

	private TurnManager TurnManager;

	void Start () {
		weapon = new List<Weapon>();
		hat = new List<Hat>();
		leg = new List<Leg>();
		shield = new List<Shield>();
		mech = new List<Mech>();

		TurnManager = gameObject.GetComponent<TurnManager>();
		updateCollider = new float[3] {0f,0f,0f};
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	void Update () {
		if(PlayerName == null || PlayerName == "" || PlayerName == "Guest")
			PlayerName = PlayerPrefs.GetString("PlayerName");
		if (update32){
			Debug.Log("updated");
			update32 = false;
			texture = GameObject.Find ("Game").GetComponent<SpriteRenderer> ().sprite.texture;
			LevelPix = texture.GetPixels32();
			GameObject.Find("Game").GetComponent<Game>().init = true;
		}
		if(updateCollider[0] == 1f){
			EdgeCollider2D col = GameObject.Find ("Game").GetComponent<EdgeCollider2D> ();
			Vector2[] points = col.points;
			var pts = from p in points where p.x > (updateCollider[1] - updateCollider[2]) && p.x < (updateCollider[1] + updateCollider[2]) select p;
			foreach(var v in pts){
				Vector2 pt = v;
				//Debug.Log ("old: " + pt);
				while (pt.y > -53.9 && LevelPix[((int)(pt.y*10) + 540)*1920 + (int)(pt.x*10) + 960].a != 255 && LevelPix[((int)(pt.y*10) + 539)*1920 + (int)(pt.x*10) + 960].a != 255){
					points[System.Array.FindIndex(points, a => a.x == pt.x && a.y == pt.y)].y -= 0.1f;
					pt.y = pt.y - 0.1f;
					//Debug.Log ("lowering");
				}
				//Debug.Log ("new: " + pt);
			}
			col.points = points;
			updateCollider = new float[] {0f, 0f, 0f};
		}
	}

	public void StartServer(string ServerName, int MaxPlayers){
		//Network.InitializeSecurity();
		Network.InitializeServer(MaxPlayers, 25000, true);
		MasterServer.RegisterHost("Team7WorkingTitle", ServerName);
	}

	void OnPlayerConnected(NetworkPlayer id){
		foreach (Player pl in PlayerList)
			GetComponent<NetworkView>().RPC("Client_PlayerJoined", id, pl.PlayerName, pl.OnlinePlayer);
	}

	void OnServerInitialized(){
		Server_PlayerJoined (PlayerName, Network.player);
	}

	void OnConnectedToServer(){
		GetComponent<NetworkView>().RPC ("Client_SendMessage",RPCMode.Others, "*** " + PlayerName + " has connected ***", "");
		GetComponent<NetworkView>().RPC("Server_PlayerJoined", RPCMode.Server, PlayerName, Network.player);
	}

	void OnPlayerDisconnected(NetworkPlayer id){
		GetComponent<NetworkView>().RPC ("Client_SendMessage",RPCMode.All, "*** " + GetPlayer(id).PlayerName + " has disconnected ***", "");
		GetComponent<NetworkView>().RPC ("RemovePlayer", RPCMode.All, id);
		Network.RemoveRPCs(id);
	}

	void OnDisconnectedFromServer(NetworkDisconnection info){
		PlayerList.Clear();
		if (Application.loadedLevel == 1)
			GameObject.Find ("Main Camera").GetComponent<Menu> ().CurMenu = "Play";
		else
			Application.LoadLevel (1);
	}	

	[RPC]
	public void Server_PlayerJoined(string Username, NetworkPlayer id){
		GetComponent<NetworkView>().RPC ("Client_PlayerJoined", RPCMode.All, Username, id);
	}

	[RPC]
	public void Client_PlayerJoined(string Username, NetworkPlayer id){
		Player temp = new Player();
		temp.PlayerName = Username;
		temp.OnlinePlayer = id;

		PlayerList.Add (temp);
		if (Network.player == id){
			MyPlayer = temp;
    	}
	}

	[RPC]
	public void RemovePlayer(NetworkPlayer id){
		Player temp = new Player();
		foreach (Player pl in PlayerList)
			if (pl.OnlinePlayer == id)
				temp = pl;
		if (temp != null){
			Destroy (GameObject.Find (PlayerList.Find(x => x.OnlinePlayer == id).PlayerName));
			PlayerList.Remove(temp);
		}
	}

	[RPC]
	public void StartGame(){
		TurnManager.Init ();
		Application.LoadLevel (2);
	}

	[RPC]
	public void Server_Fire(NetworkPlayer pl, int id, Vector3 spawn, Quaternion rotation){
		foreach (Player p in PlayerList) {
			if (p.OnlinePlayer == pl && p.firing == false){
				p.firing = true;
				GetComponent<NetworkView>().RPC ("Client_Fire", pl, id, spawn, rotation);
			}
		}
	}

	[RPC]
	public void Client_Fire(int id, Vector3 spawn, Quaternion rotation){
		Network.Instantiate(shells[id], spawn, rotation, 0);
	}

	[RPC]
	void EndTurn(NetworkMessageInfo info){
		Debug.Log ("Ending" + info.sender.ToString ());
		TurnManager.endTurn (int.Parse (info.sender.ToString ()));
	}

	public void EndServerTurn(){
		Debug.Log ("Ending server " + 0);
		TurnManager.endTurn (0);
	}

	/*[RPC]
	public void Itemize(NetworkPlayer pl, int[] items){
		Debug.Log (pl + "||" + items [0] + "|" + items [1] + "|" + items [2] + "|" + items [3] + "|" + items [4] + "|" + items [5]);
		foreach (Player p in PlayerList){
			if (p.OnlinePlayer == pl){
				p.PlayerItems = items;
			}
		}
		NetworkManager.Instance.networkView.RPC ("Client_Itemize", RPCMode.OthersBuffered, pl, items);
	}

	[RPC]
	public void Client_Itemize(string pl, int[] items){
		GameObject[] go = new GameObject[5];
		
		
		for(int i = 0; i < 5; i++){
			go[i] = new GameObject();
			go[i].AddComponent<SpriteRenderer>();
			go[i].transform.parent = GameObject.
			go[i].transform.localPosition = new Vector3(0,0,0);
		}
		go[0].name = "Hat";
		go[1].name = "Weapon1";
		go[2].name = "Weapon2";
		go[3].name = "Shield";
		go[4].name = "Face";
		
		go[0].GetComponent<SpriteRenderer>().sprite = Resources.Load(hat[items[0]].spriteName, typeof(Sprite)) as Sprite;
		go[0].GetComponent<SpriteRenderer>().sortingLayerName = "Hat";
		go[1].GetComponent<SpriteRenderer>().sprite = Resources.Load(weapon[items[1]].spriteName, typeof(Sprite)) as Sprite;
		go[1].GetComponent<SpriteRenderer>().sortingLayerName = "Weapon";
		go[2].GetComponent<SpriteRenderer>().sprite = Resources.Load(weapon[items[2]].spriteName, typeof(Sprite)) as Sprite;
		go[2].GetComponent<SpriteRenderer>().sortingLayerName = "Weapon";
		go[2].GetComponent<SpriteRenderer>().enabled = false;
		
		go[3].GetComponent<SpriteRenderer>().sprite = Resources.Load(shield[items[3]].spriteName, typeof(Sprite)) as Sprite;
		go[3].GetComponent<SpriteRenderer>().sortingLayerName = "Hat";
		go[4].GetComponent<SpriteRenderer>().sprite = Resources.Load("FaceNeutral", typeof(Sprite)) as Sprite;
		go[4].GetComponent<SpriteRenderer>().sortingLayerName = "Character";

		foreach (Player p in PlayerList){
			if (p.PlayerName == pl){
				p.PlayerItems = items;
			}
		}
	}

	[RPC]
	public void Rename (NetworkPlayer pl, string Name){
		Debug.Log (pl + "|" + Name);
		GameObject p = GameObject.FindGameObjectsWithTag("Player").FirstOrDefault(x => x.networkView.owner == pl);
		if(p != null)
			p.GetComponent<NetworkRename>().SetName(Name);
	}*/

	public void Fire(int id, Vector3 spawn, Quaternion rotation){
		Network.Instantiate(shells[id], spawn, rotation, 0);
	}

	public Player GetPlayer(NetworkPlayer id){
		foreach (Player pl in PlayerList)
			if (pl.OnlinePlayer == id)
				return pl;
		return null;
	}

	public Player GetPlayer(string name){
			foreach (Player pl in PlayerList)
			if (pl.PlayerName == name)
				return pl;
		return null;
	}
}