using UnityEngine;

[System.Serializable]
public class ChatMessage
{
	public string Timestamp;
	public string Sender;
	public string Message;
	
	public ChatMessage(string SenderName, string message)
	{
		Timestamp = System.DateTime.Now.ToLongTimeString();
		Sender = SenderName;
		Message = message;
	}
	
	public void Draw()
	{
		string label = "[" + Timestamp + "]  ";
		if(!string.IsNullOrEmpty(Sender))
			label += Sender + ": ";
		label += Message;
		GUILayout.Label(label);
	}
}