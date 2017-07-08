using UnityEngine;
using System.Collections;

public class HealthBar : MonoBehaviour {
	public Transform target;
	public float health;
	public float maxHealth;

	void Start(){
		health = transform.GetComponent<PlayerController> ().MyPlayer.health;
		maxHealth = health;
	}

	void OnGUI () {
		health = NetworkManager.Instance.GetPlayer (transform.name).health;
		Vector2 targetPos;
		targetPos = Camera.main.WorldToScreenPoint (transform.position);			
		GUI.Box(new Rect(targetPos.x, targetPos.y +20, 60, 40), transform.name + "\n" + health + "/" + maxHealth);	
	}
}
