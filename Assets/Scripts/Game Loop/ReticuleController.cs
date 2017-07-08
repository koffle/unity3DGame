using UnityEngine;
using System.Collections;

public class ReticuleController : MonoBehaviour {
	float rotation;
	float maxAngle = 70;
	float minAngle = 1;
	float shellSpeed = 2000;
	public GameObject[] shells;
	GameObject platform;
	Transform reticule;
	public Vector2 force;
	Vector2 cursorLoc;
	Vector2 startPoint;
	GameObject player;

	void Start () {
		reticule = GameObject.Find ("Reticule").transform;
		player = GameObject.Find(NetworkManager.Instance.MyPlayer.PlayerName);
	}

	void Update () {
		if(GetComponent<NetworkView>().isMine) {
			if(Input.GetMouseButtonDown(0) && NetworkManager.Instance.MyPlayer.aiming) {
				startPoint = new Vector2(transform.position.x, transform.position.y);
				
				cursorLoc = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				cursorLoc = Camera.main.ScreenToWorldPoint(cursorLoc);
				
				if (Input.mousePosition.y < Screen.height - Screen.height/10 - 20) {
					Vector2 direction = cursorLoc - startPoint;
					float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
					if(player.transform.localScale.x == -1) {
						angle = 180 - angle;
					}
					if(angle <= maxAngle && angle >= minAngle)
						transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
				}
			}
		}
		if (NetworkManager.Instance.MyPlayer.firing == true) {
			NetworkManager.Instance.MyPlayer.firing = false;
			Matrix4x4 matrix = reticule.worldToLocalMatrix;
			Vector3 shellSpawn = new Vector3(transform.position.x + matrix[0] + (matrix[0] < 0 ? -1.0f : 1.0f),
			                                 transform.position.y + Mathf.Abs(matrix[1]) + 1.0f, 
			                                 transform.position.z); 
			force = new Vector2(matrix[0], Mathf.Abs(matrix[1]))*shellSpeed;
			if (Network.isClient) {
				NetworkManager.Instance.GetComponent<NetworkView>().RPC ("Server_Fire", RPCMode.Server, NetworkManager.Instance.MyPlayer.OnlinePlayer, 0, shellSpawn, transform.rotation);
			}
			else if (Network.isServer) {
				NetworkManager.Instance.Fire(0, shellSpawn, transform.rotation);
			}
		}
	}
}
