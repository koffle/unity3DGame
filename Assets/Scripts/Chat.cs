using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chat : MonoBehaviour {
	public string InputString = "";
	public List<ChatMessage> Messages = new List<ChatMessage>();
	public bool Show;
	public static Chat Instance;
	Vector2 v = new Vector2(0,0);

	void Start () {
		Instance = this;
	}

	void Update () {

	}

	void OnGUI(){
		if(Show){
			GUILayout.BeginArea (new Rect(0,Screen.height/2,Screen.width,Screen.height/2));
			v = GUILayout.BeginScrollView(v, "box", GUILayout.Width(Screen.width), GUILayout.Height (Screen.height/2-60));
			foreach (ChatMessage cm in Messages)
				cm.Draw ();
			GUILayout.EndScrollView();

			GUILayout.BeginHorizontal ();
			InputString = GUILayout.TextField(InputString, GUILayout.Width (Screen.width-100), GUILayout.Height (59));
			if (GUILayout.Button("Send", GUILayout.Width (100), GUILayout.Height (59)))
						if (!string.IsNullOrEmpty(InputString)){
				    		Say (InputString, NetworkManager.Instance.MyPlayer.PlayerName);
							v.y+=30;
						}
			GUILayout.EndHorizontal();
			GUILayout.EndArea ();
		}
	}

	public void Say(string msg, string Sender){
		GetComponent<NetworkView>().RPC ("Client_SendMessage",RPCMode.All, msg, Sender);
		InputString = "";
	}

	[RPC]
	void Client_SendMessage(string msg, string Sender){
		Convert (msg, Sender);
	}

	void Convert(string msg, string Sender){
		ChatMessage cm = new ChatMessage(Sender, msg);
		Messages.Add (cm);
	}
}
