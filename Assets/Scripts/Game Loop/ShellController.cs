using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ShellController : MonoBehaviour {
	CameraAnchor cam;
	ReticuleController Reticule;
	SpriteRenderer level;
	Texture2D texture;
	Color32[] LevelPix;
	int explodeX;
	List<int[]> collapse;
	List<string> hit;
	int fall = 0;
	Player Player;
	int radius = 30;
	bool print = true;
	List<Player> PlayerList;

	//void OnCollisionEnter2D(Collision2D col) {
	//	if(col.gameObject.tag == "Player") {
	//		Debug.Log ("I WORK !");
	//		Player.firing = false;
	//		Destroy(gameObject);
	//	}
	//}

	void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.tag == "Player"){
			if(hit != null){
				if(!hit.Contains(col.gameObject.name)){
					hit.Add(col.gameObject.name);
					if(GetComponent<NetworkView>().isMine)
						col.gameObject.GetComponent<NetworkView>().RPC("TakeDamage", RPCMode.AllBuffered, col.gameObject.name, (float)Random.Range(25,30));
				}
			}
	  	}
	}

	void OnDestroy() {
		NetworkManager.Instance.updateCollider = new float[] {1, transform.position.x, radius / 10};
		if(GetComponent<NetworkView>().isMine)
			if(Network.isClient)
				NetworkManager.Instance.GetComponent<NetworkView>().RPC ("EndTurn", RPCMode.Server);
			else
				NetworkManager.Instance.EndServerTurn();
	}
	
	void Start(){
		hit = new List<string> ();
		PlayerList = NetworkManager.Instance.PlayerList;
		Player = PlayerList.Find (x => x.OnlinePlayer == GetComponent<NetworkView>().owner);
		Reticule = GameObject.Find("Reticule").GetComponent<ReticuleController> ();
		GetComponent<Rigidbody2D>().AddForce (Reticule.force);
		texture = GameObject.Find ("Game").GetComponent<SpriteRenderer> ().sprite.texture;
		LevelPix = NetworkManager.Instance.LevelPix;
		cam = GameObject.Find ("Main Camera").GetComponent<CameraAnchor> ();
		cam.followShot = true;
		cam.shiftToPC = false;
		cam.followPC = false;
	}
	
	void Update(){
		Vector3 pos = transform.position;
		if((int)pos.x*10+960 >= 1920 || (int)pos.x*10+960 <= 0 || (int)(pos.y*10+540) <= 0) {
			Network.RemoveRPCs(this.gameObject.GetComponent<NetworkView>().viewID);
			Destroy (gameObject);
			cam.followShot = false;
			cam.shiftToPC = true;
			Player.firing = false;
		}
		else {
			if(LevelPix[(int)(pos.y * 10 + 540) * 1920 + (int)(pos.x * 10 + 960)].a == 255 && transform.GetComponent<Renderer>().enabled == true) {
				collapse = new List<int[]> ();
				explodeX = (int)(pos.x*10);
				explode ((int)(pos.x*10+960), (int)(pos.y*10+540), radius);
				transform.GetComponent<Rigidbody2D>().isKinematic = false;
				CircleCollider2D collider = gameObject.GetComponent<CircleCollider2D>();
				collider.isTrigger = true;
				collider.radius = radius/10;
				caveIn ((int)(pos.x*10+960), (int)(pos.y*10+540));

				//if(networkView.isMine)
				//	NetworkManager.Instance.networkView.RPC ("Fall", RPCMode.All, explodeX, radius);
					//networkView.rigidbody2D.gravityScale = 1.0f;
				//	Debug.Log (networkView.owner +"|"+ NetworkManager.Instance.MyPlayer.PlayerName +"|"+ NetworkManager.Instance.MyPlayer.Manager.transform.position.x + "|" + NetworkManager.Instance.MyPlayer.Manager.transform.position.y);
				//	if(Player.Manager.transform.position.x - 13 >= explodeX - radius || Player.Manager.transform.position.x + 13 <= explodeX + radius)
				//		Player.Manager.rigidbody2D.gravityScale = 1.0f;
				//}
				transform.GetComponent<Rigidbody2D>().gravityScale = 0.0f;
				transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0,0);
				transform.GetComponent<Rigidbody2D>().isKinematic = false;

				if (fall == 0){


					Debug.Log("zerofall");
					Network.RemoveRPCs(this.gameObject.GetComponent<NetworkView>().viewID); 

					NetworkManager.Instance.update32 = true;
					Destroy(gameObject);
					//gameObject = null;
					Player.firing = false;
					cam.followShot = false;
					cam.shiftToPC = true;
				}
				else {
					//transform.renderer.enabled = false;
				}
			}
		}
	}
	
	void LateUpdate(){
		if (fall > 0){
			//Debug.Log (fall);
			int fallHeight = 3;
			for (int i=0; i < collapse.Count; i++) {
				if (collapse[i][3] >= fallHeight) {
					Color[] cols = new Color[collapse[i][2]+1];
					
					//cols = texture.GetPixels(collapse[i][0], collapse[i][1], 1, collapse[i][2]+1);
					//texture.SetPixel (collapse[i][0], collapse[i][1], Color.cyan);
					//for (int k = 0; k<collapse[i][2]; k++){
					//	cols[k] = Color.cyan;
					//	cols[k].a = 1.0f;
					//}
					//texture.SetPixels (collapse[i][0], collapse[i][1] - 1, 1, collapse[i][2], cols);
					cols = texture.GetPixels(collapse[i][0], collapse[i][1], 1, collapse[i][2] + 1);
					texture.SetPixels (collapse[i][0], collapse[i][1] - fallHeight, 1, collapse[i][2], cols);
					
					Color temp = Color.clear;
					temp.a = 0.0f;
					for(int j = 1; j <= fallHeight; j++){
						texture.SetPixel (collapse[i][0], collapse[i][1] + collapse[i][2] - j, temp);
					}
					//texture.SetPixel (collapse[i][0], collapse[i][1] + collapse[i][2] - 2, temp);
					//texture.SetPixel (collapse[i][0], collapse[i][1] + collapse[i][2] - 3, temp);
					
					//LevelPix[(collapse[i][1] + collapse[i][2]) * 1920 + collapse[i][0]].a = 0;
					//LevelPix. = texture.GetPixels32();
					//Debug.Log (texture.GetPixel(collapse[i][0], collapse[i][1]+1).a);
					collapse[i] = new int[5] {collapse[i][0], collapse[i][1] - fallHeight, collapse[i][2], collapse[i][3] - fallHeight, collapse[i][4]};
				}
				else if (collapse[i][3] < fallHeight && collapse[i][3] > 0) {
					Color[] cols = new Color[collapse[i][2]+1];
					
					//cols = texture.GetPixels(collapse[i][0], collapse[i][1], 1, collapse[i][2]+1);
					//texture.SetPixel (collapse[i][0], collapse[i][1], Color.cyan);
					//for (int k = 0; k<collapse[i][2]; k++){
					//	cols[k] = Color.cyan;
					//	cols[k].a = 1.0f;
					//}
					//texture.SetPixels (collapse[i][0], collapse[i][1] - 1, 1, collapse[i][2], cols);
					cols = texture.GetPixels(collapse[i][0], collapse[i][1], 1, collapse[i][2] + 1);
					texture.SetPixels (collapse[i][0], collapse[i][1] - collapse[i][3], 1, collapse[i][2], cols);
					
					Color temp = Color.clear;
					temp.a = 0.0f;
					
					for(int j = 1; j <= collapse[i][3]; j++){
						texture.SetPixel (collapse[i][0], collapse[i][1] + collapse[i][2] - j, temp);
					}
					//LevelPix[(collapse[i][1] + collapse[i][2]) * 1920 + collapse[i][0]].a = 0;
					//LevelPix. = texture.GetPixels32();
					//Debug.Log (texture.GetPixel(collapse[i][0], collapse[i][1]+1).a);
					collapse[i] = new int[5] {collapse[i][0], collapse[i][1] - collapse[i][3], collapse[i][2], collapse[i][3] - collapse[i][3], collapse[i][4]};
				}
				/*else if (collapse[i][3] == 0 && collapse[i][4] == 1){
					//Color[] cols = new Color[collapse[i][2]+1];

					//cols = texture.GetPixels(collapse[i][0], collapse[i][1], 1, collapse[i][2] + 1);
					//texture.SetPixels (collapse[i][0], collapse[i][1] - 1, 1, collapse[i][2], cols);
					
					//Color temp = Color.clear;
					//temp.a = 0.0f;
					//texture.SetPixel (collapse[i][0], collapse[i][1] + collapse[i][2] - 1, temp);
					//collapse[i] = new int[4] {collapse[i][0], collapse[i][1] - 1, collapse[i][2], collapse[i][3] - 1};
					//Debug.Log ("IM A ZERO!!!");
				}*/
				//Debug.Log (collapse.Count + "||" + collapse[i][0] + "  ||  "+collapse[i][1] + "  ||  "+collapse[i][2]+"  ||  "+collapse[i][3]);
			}
			
			texture.Apply();
			fall = fall-fallHeight;
			print = false;
			if (fall <= 0){

				//NetworkManager.Instance.networkView.RPC ("Fall", RPCMode.All, explodeX, radius);
				NetworkManager.Instance.update32 = true;
				Network.RemoveRPCs(this.gameObject.GetComponent<NetworkView>().viewID); 
				Destroy (gameObject);
				//gameObject = null;
				Player.firing = false;
				cam.followShot = false;
				cam.shiftToPC = true;
			}
		}
	}
	
	
	void caveIn(int cx, int cy) {	
		fall = 0;
		for (int i=0; i < collapse.Count; i++) {
			//texture.SetPixel(collapse[i][0],collapse[i][1], Color.white);
			if (texture.GetPixel (collapse [i] [0], collapse [i] [1] + 1).a != 0){
				bool topFound = false;
				bool botFound = false;
				int j = 1;
				while(!topFound || !botFound) {
					if (!topFound && collapse [i] [1] + j + 1 < 1080) {
						if (texture.GetPixel(collapse[i][0],collapse[i][1]+j).a == 0f && texture.GetPixel(collapse[i][0],collapse[i][1]+j+1).a == 0f){
							topFound = true;
							collapse[i][2] = j;
							//texture.SetPixel(collapse[i][0],collapse[i][1]+collapse[i][2],Color.cyan);
						}
					}
					if (!botFound && collapse [i] [1] - j > 0) {
						if (texture.GetPixel(collapse[i][0],collapse[i][1]-j).a == 1f){
							botFound = true;
							collapse[i][3] = j;
							//texture.SetPixel(collapse[i][0],collapse[i][1]-collapse[i][3],Color.red);
						}
					}
					else if (!botFound && collapse [i] [1] - j == 0) {
						botFound = true;
						collapse[i][3] = j;
						collapse[i][4] = 1;
						//texture.SetPixel(collapse[i][0],collapse[i][1] - j, Color.yellow);
					}
					j++;
				}
				fall = Mathf.Max (fall, collapse[i][3]);
			}
		}
		//texture.Apply ();
		
	}
	
	void explode(int cx, int cy, int radius){
		int error = -radius;
		int x = radius;
		int y = 0;
		while (x >= y) {
			int lastY = y;
			error += y;
			++y;
			error += y;
			plot4points(cx, cy, x, lastY);
			if (error >= 0) {
				if (x != lastY)
					plot4points(cx, cy, lastY, x);
				error -= x;
				--x;
				error -= x;
			}
		}
		texture.Apply();
	}
	
	void plot4points(int cx, int cy, int x, int y){
		
		int x1 = cx - x;
		int x2 = cx - x;
		int length = x*2;
		int length2 = x*2;
		bool top = false;
		bool bottom = false;
		if (cy + y < 1080) {
			if (cx - x < 0) {
				x1 = 0;
			}
			else {
				x1 = cx - x;
			}
			if (cx + x > 1920) {
				length = (1920 - x1 > 0) ? 1920 - x1 : 0;
			}
			else {
				length = cx + x - x1;
			}
			top = true;
		}
		if (x != 0 && y != 0 && cy - y >= 0) {		       
			if (cx - x < 0) {
				x2 = 0;
			}
			else {
				x2 = cx - x;
			}
			if (cx + x > 1920) {
				length2 = (1920 - x2 > 0) ? 1920 - x2 : 0;
			}
			else {
				length2 = cx + x - x2;
			}
			bottom = true;
		}
		if (top && length > 0) {
			Color[] cols = new Color[length];
			for(int i = 0;i < length; i++){
				cols[i] = Color.clear;
				cols[i].a = 0.0f;
			}
			texture.SetPixels(x1, cy + y, length, 1, cols);
			
			for(int i = 0;i < length; i++){
				LevelPix[(cy + y) * 1920 + x1 + i].a = 0;
				int index = collapse.FindIndex(pix => pix[0] == (x1 + i));
				if(index == -1)
					collapse.Add (new int[5] {x1 + i, cy + y + 1, 0, 0, 0});
				else
				if(cy + y + 1 > collapse[index][1]){
					collapse[index] = new int[5] {x1 + i, cy + y + 1, 0, 0, 0};
				}
			}
		}
		if (bottom && length2 > 0) {
			Color[] cols2 = new Color[length2];
			for (int i = 0; i < length2; i++) {
				cols2 [i] = Color.clear;
				cols2 [i].a = 0.0f;
			}
			texture.SetPixels (x2, cy - y, length2, 1, cols2);
			for(int i=0;i<length2;i++){
				LevelPix[(cy-y)*1920 + x2 + i].a = 0;
			}
		}
	}    
}
