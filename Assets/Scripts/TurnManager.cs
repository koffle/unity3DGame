using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TurnManager : MonoBehaviour {

	private int turn;
	private int lastTurn;
	public List<Player> Players = new List<Player>();
	private bool winnar = false;
	private string winName = "";

	void OnGUI(){
		if(winnar){
			GUILayout.BeginArea (new Rect(Screen.width/2 - 90,Screen.height/2 - 25,180,50));
			GUILayout.BeginVertical ();
			GUI.Label (new Rect (10, 10, 160, 20), winName + " is WINNER !");
			if (GUILayout.Button("Disconnect", GUILayout.Height(60))){
				winnar = false;
				winName = "";
				NetworkManager.Instance.GetComponent<Chat>().Show = false;
				NetworkManager.Instance.GetComponent<Chat>().InputString = "";
				NetworkManager.Instance.GetComponent<Chat>().Messages.Clear ();
				if(Network.isServer)
					MasterServer.UnregisterHost();
				Network.Disconnect();
			}
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
	}

	public void Init(){
		Players = NetworkManager.Instance.PlayerList;
		if(Network.isServer){
			turn = Random.Range(0, Players.Count);
			GetComponent<NetworkView>().RPC("GiveTurn", RPCMode.All, turn);
		}
	}

	[RPC]
	void GiveTurn(int turn){
		Debug.Log ("Giving turn to: " + turn);
		NetworkManager.Instance.PlayerList [turn].moving = true;
		NetworkManager.Instance.PlayerList [turn].energy = 5;
	}

	[RPC]
	void Win(string name){
		winName = name;
		winnar = true;
	}

	public void endTurn(int ended){
		lastTurn = ended;
		turn++;
		if (turn == Players.Count){
			turn = 0;
			Debug.Log ("Turn set to 0");
		}
		while (turn < Players.Count){
			Debug.Log ("while " + turn);
			if(Players[turn].health > 0){
				if(lastTurn == turn){
					GetComponent<NetworkView>().RPC ("Win", RPCMode.All, Players[turn].PlayerName);
					return;
				}
				else{
					GetComponent<NetworkView>().RPC("GiveTurn", RPCMode.All, turn);
					return;
				}
			}
			turn ++;
			if (turn == Players.Count)
				turn = 0;
		}
	}
}
