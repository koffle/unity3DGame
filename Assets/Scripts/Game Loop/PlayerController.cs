using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {
	public float maxSpeed;
	public float moveForce;
	public float jumpForce;
	public Player MyPlayer;
	public bool isMine;
	public int collisionBuffer;
	float startTime;
	Vector2 startPoint;
	Vector2 cursorLoc;
	string name;
	float powSlider;

	int row;
	int col;
	int levelWidth = 1920;
	int levelHeight = 1080;
	int playerRadius = 12;
	
	Texture2D PlayerSprite;
	Color[] PlayerPix;

	List<Weapon> weapon = null;
	List<Hat> hat = null;
	List<Shield> shield = null;

	Chat chat;

	//UI Textures
	public Texture chatButton;
	public Texture zoomInButton;
	public Texture zoomOutButton;
	public Texture aimButton;
	public Texture swapButton;
	public Texture fireButton;

	//Resizer
	private ResponsiveGui rg;
	public GUISkin skin;
	
	public PlayerController() {
		moveForce = 1500f;
	}
	
	public PlayerController(float move) {
		moveForce = move;
	}

	void Start() {
		chat = GameObject.Find("NetworkManager").GetComponent<Chat>();
		MyPlayer = NetworkManager.Instance.MyPlayer;

		/*if(Network.isServer){
			int[] items = NetworkManager.Instance.MyPlayer.PlayerItems;
			GameObject[] go = new GameObject[5];
			
			
			for(int i = 0; i < 5; i++){
				go[i] = new GameObject();
				go[i].AddComponent<SpriteRenderer>();
				go[i].transform.parent = gameObject.transform;
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


			NetworkManager.Instance.networkView.RPC("Client_Itemize", RPCMode.Others, MyPlayer.PlayerName, MyPlayer.PlayerItems);

		}
		else{
			NetworkManager.Instance.networkView.RPC ("Rename", RPCMode.Server, MyPlayer.PlayerName, NetworkManager.Instance.MyPlayer.PlayerName);
		}*/
		//gameObject.name = NetworkManager.Instance.PlayerList.Find (x => x.OnlinePlayer == Network.player).PlayerName;
		//Itemize ();
		if(GetComponent<NetworkView>().isMine){
			name = MyPlayer.PlayerName;
			isMine = GetComponent<NetworkView>().isMine;
			MyPlayer.Manager = this;
			GameObject.Find ("Main Camera").GetComponent<CameraAnchor> ().init ();
		}
		MyPlayer.PlayerSprite = MyPlayer.Manager.transform.Find ("Body").GetComponent<SpriteRenderer> ().sprite.texture;
		PlayerPix = MyPlayer.PlayerSprite.GetPixels(0, 0, 250, 130);

		rg = new ResponsiveGui ();
	}

	void Update() {
		if(GetComponent<NetworkView>().isMine){
			if (Input.GetMouseButtonDown(0)) {
				startPoint = GetComponent<Rigidbody2D>().transform.position;
				
				cursorLoc = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				cursorLoc = Camera.main.ScreenToWorldPoint(cursorLoc);
				
				if (Input.mousePosition.y < Screen.height - Screen.height/10 - 20) {
					if(MyPlayer.moving || MyPlayer.aiming) {
						if(cursorLoc.x < startPoint.x)
							transform.localScale = new Vector3(-1,1,1);
						if(cursorLoc.x > startPoint.x)
							transform.localScale = new Vector3(1,1,1);
						if(MyPlayer.moving && MyPlayer.energy > 0) {
							if(cursorLoc.y > startPoint.y) {  
								Vector2 force = (cursorLoc - startPoint);
								GetComponent<Rigidbody2D>().AddForce(force.normalized * moveForce);
								MyPlayer.energy --;
							}
						}
					}
				}
			}
		}
	}

	void OnGUI () {
		rg.StartResizer ();
		GUI.Box (new Rect(0, 0, rg.getNativeWidth(), rg.getNativeHeight()/10), "");
		if(GUI.Button(new Rect(rg.getNativeWidth() - rg.getNativeWidth()/10 - 10, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), zoomInButton, "")) {
			if(Camera.main.orthographicSize > 10)
				Camera.main.orthographicSize -= 5;
		}
		if(GUI.Button(new Rect(rg.getNativeWidth() - 2*(rg.getNativeWidth()/10) - 20, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), zoomOutButton, "")) {
			if(Camera.main.orthographicSize < 60)
				Camera.main.orthographicSize += 5;
		}
		if(GUI.Button (new Rect(rg.getNativeWidth() - 3*(rg.getNativeWidth()/10) - 30, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), chatButton, "")) {
			chat.Show = !chat.Show;
			chat.InputString = "";
		}
		if(MyPlayer.moving) {
			GUI.Box (new Rect(10, 10, rg.getNativeWidth()/3, rg.getNativeHeight()/15), (MyPlayer.energy > 0) ? "" : "Out of energy!");
			if(MyPlayer.energy > 0) GUI.Box (new Rect(10 + (rg.getNativeHeight()/15)/4, 10 + (rg.getNativeHeight()/15)/4, (rg.getNativeWidth()/3 - 10 - (rg.getNativeHeight()/15)/4) * (MyPlayer.energy/5), (rg.getNativeHeight()/15)/2), "");
			if(GUI.Button(new Rect(10, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), aimButton, "")) {
				MyPlayer.aiming = true;
				MyPlayer.moving = false;
			}
		}
		if(MyPlayer.aiming) {
			if(GUI.Button(new Rect(10, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), fireButton, "")) {
				MyPlayer.aiming = false;
				MyPlayer.firing = true;
			}
			if(GUI.Button(new Rect(10 + rg.getNativeWidth()/10 + 10, 0, rg.getNativeWidth()/10, rg.getNativeHeight()/10), swapButton, "")) {
				GetComponent<NetworkView>().RPC ("SwitchWeapon", RPCMode.AllBuffered, MyPlayer.PlayerName);
			}
			powSlider = GUI.HorizontalSlider(new Rect(30 + 2*(rg.getNativeWidth()/10), 0, rg.getNativeWidth()/3, rg.getNativeHeight()/10), powSlider, 1, 100);
		}
		rg.EndResizer ();
	}

	[RPC]
	void SwitchWeapon(string name){
		GameObject.Find (name).GetComponentsInChildren<SpriteRenderer> ().First (x => x.name == "Weapon1").enabled = !GameObject.Find (name).GetComponentsInChildren<SpriteRenderer> ().First (x => x.name == "Weapon1").enabled;
		GameObject.Find (name).GetComponentsInChildren<SpriteRenderer> ().First (x => x.name == "Weapon2").enabled = !GameObject.Find (name).GetComponentsInChildren<SpriteRenderer> ().First (x => x.name == "Weapon2").enabled;
		//gameObject.GetComponents<GameObject> ().First (x => x.name == "Weapon2").GetComponent<SpriteRenderer> ().enabled = !gameObject.GetComponents<GameObject> ().First (x => x.name == "Weapon2").GetComponent<SpriteRenderer> ().enabled;
	}
	
	[RPC]
	void setupPlayer(string playerName, int[] items) {
		if(weapon == null)
			weapon = NetworkManager.Instance.weapon;
		if(hat == null)
			hat = NetworkManager.Instance.hat;
		if(shield == null)
			shield = NetworkManager.Instance.shield;

		transform.name = playerName;
		gameObject.AddComponent<HealthBar> ();

		GameObject[] go = new GameObject[5];
		
		for(int i = 0; i < 5; i++){
			go[i] = new GameObject();
			go[i].AddComponent<SpriteRenderer>();
			go[i].transform.parent = transform;
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
		go[1].AddComponent<WeaponController> ();
		go[2].GetComponent<SpriteRenderer>().sprite = Resources.Load(weapon[items[2]].spriteName, typeof(Sprite)) as Sprite;
		go[2].GetComponent<SpriteRenderer>().sortingLayerName = "Weapon";
		go[2].AddComponent<WeaponController> ();
		go[2].GetComponent<SpriteRenderer>().enabled = false;
		
		go[3].GetComponent<SpriteRenderer>().sprite = Resources.Load(shield[items[3]].spriteName, typeof(Sprite)) as Sprite;
		go[3].GetComponent<SpriteRenderer>().sortingLayerName = "Hat";
		go[4].GetComponent<SpriteRenderer>().sprite = Resources.Load("FaceNeutral", typeof(Sprite)) as Sprite;
		go[4].GetComponent<SpriteRenderer>().sortingLayerName = "Character";
	}

	[RPC]
	void TakeDamage(string name, float Damage){
		Debug.Log (name + "|" + Damage);
		NetworkManager.Instance.PlayerList.Find (x => x.PlayerName == name).health -= Damage;
		if(NetworkManager.Instance.GetPlayer(name).health <= 0){
			GetComponent<NetworkView>().RPC ("Die",RPCMode.AllBuffered);
		}
	}

	[RPC]
	void Die(){
		SpriteRenderer[] sr = gameObject.GetComponentsInChildren<SpriteRenderer> ();
		foreach (SpriteRenderer s in sr)
			s.enabled = false;
		gameObject.GetComponent<CircleCollider2D> ().enabled = false;
		gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
		//Debug.Log (MyPlayer.PlayerName + " has died");
	}
}