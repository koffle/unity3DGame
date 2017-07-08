using UnityEngine;
using System.Linq;

public class Game : MonoBehaviour {
	private SpriteRenderer level;
	public bool init = false;
	bool colSet = false;
	
	private Texture2D texture;

	void Start () {
		//Application.targetFrameRate = 30;
        texture = (Texture2D)Resources.Load("alphatest");

		Sprite sprite = Sprite.Create (texture, new Rect(0,0,1920,1080),new Vector2(0.5f, 0.5f), 10f);

		level = gameObject.GetComponent<SpriteRenderer> ();
		level.sprite = sprite;
		EdgeCollider2D col = gameObject.AddComponent<EdgeCollider2D> ();

		NetworkManager.Instance.update32 = true;

		//GameObject Player = PrefabUtility.InstantiatePrefab (NetworkManager.Instance.SpawnPlayer) as GameObject;
		//Debug.Log (Player);
		//Debug.Log (Player.transform.Find ("Body").GetComponent<SpriteRenderer>().GetType());
		//GameObject hat = new GameObject ();
		//hat.transform.name = "Hat";
		//SpriteRenderer hatS = hat.AddComponent<SpriteRenderer> ();
		//hatS.sprite = Resources.Load ("CowbodyHat") as Sprite;
		//hat.transform.parent = Player.transform;

		//hat.
		//<SpriteRenderer> ();
		//hat.sprite = Resources.Load("CowboyHat") as Sprite;
		//PrefabUtility.ReplacePrefab (Player, NetworkManager.Instance.SpawnPlayer);
		//Destroy (Player);
		//PrefabUtility.ReplacePrefab (Player, NetworkManager.Instance.SpawnPlayer);
		//GameObject Player = Pref

		/*GameObject Player = Object.Instantiate(NetworkManager.Instance.SpawnPlayer) as GameObject;
		GameObject test = new GameObject();
		test.transform.parent = Player.transform;
		test.transform.name = "Hat";
		test.AddComponent<SpriteRenderer>();
		test.GetComponent<SpriteRenderer>().sprite = Resources.Load("CowboyHat", typeof(Sprite)) as Sprite;
		*/

		//Destroy( Player );
		//if(networkView.isMine){
		GameObject pl = (GameObject)Network.Instantiate(NetworkManager.Instance.SpawnPlayer, new Vector3((int)Random.Range(-80, 80),45,0), Quaternion.identity, 0);
		pl.GetComponent<NetworkView>().RPC("setupPlayer", RPCMode.AllBuffered, NetworkManager.Instance.MyPlayer.PlayerName, NetworkManager.Instance.MyPlayer.PlayerItems);
		/*if(Network.isServer){
			NetworkManager.Instance.networkView.RPC ("Client_Itemize", RPCMode.OthersBuffered, NetworkManager.Instance.MyPlayer.OnlinePlayer, NetworkManager.Instance.MyPlayer.PlayerItems);
		}
		else {
			NetworkManager.Instance.networkView.RPC ("Itemize", RPCMode.Server, NetworkManager.Instance.MyPlayer.OnlinePlayer, NetworkManager.Instance.MyPlayer.PlayerItems);
		}*/

		//GetComponent<NetworkView>().viewID = Network.AllocateViewID();
		//}
	}

	public void Update(){
		if(init && !colSet){
			Debug.Log ("Collider Initializing");
			EdgeCollider2D co = GetComponent<EdgeCollider2D> ();
			Vector2[] newPts = new Vector2[1920];
			
			for(int i = 0; i < 1920; i++){
				newPts[i] = new Vector2((float)(-96 + i*0.1),53.9f);
				while (newPts[i].y > -53.9 && NetworkManager.Instance.LevelPix[((int)(newPts[i].y*10) + 540)*1920 + (int)(newPts[i].x*10) + 960].a != 255){
					newPts[i].y = newPts[i].y - 0.1f;
					//Debug.Log ("lowering");
				}
			}
			//newPts[1920] = new Vector2((float)-96,(float)-53.9);
			co.points = newPts;
			colSet = true;
		}
	}
}

