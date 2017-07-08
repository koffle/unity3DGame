using UnityEngine;
using System.Collections;

public class CameraAnchor : MonoBehaviour {
	Vector2 mPosStart;
	Vector2 mPosEnd;
	public bool freeLook = false;
	public bool followMouse = false;
	public bool followShot = false;
	public bool followPC = true;
	public bool shiftToPC = false;
	PlayerController pc;

	public void init(){
		pc = NetworkManager.Instance.MyPlayer.Manager;
	}

	void Update () {
		Vector3 start;
		Vector3 end;

		if(Input.GetMouseButtonDown(1) && freeLook) {
			mPosStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			followMouse = true;
		}

		if (Input.GetMouseButtonUp(1) && freeLook) {
			followMouse = false;
		}

		if (followMouse) {
			mPosEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			start = transform.position;
			end = new Vector3(start.x+(mPosStart.x - mPosEnd.x), start.y+(mPosStart.y - mPosEnd.y),start.z);

			transform.position = Vector3.Lerp(start,end,Time.deltaTime*60);
		} else if (shiftToPC) {
			start = transform.position;
			end = new Vector3(pc.transform.position.x, pc.transform.position.y, transform.position.z);
				
			transform.position = Vector3.Lerp(start, end, Time.deltaTime*5.0f);
			if(Mathf.Abs(transform.position.x - end.x) <= 0.25 && Mathf.Abs(transform.position.y - end.y) <= 0.25) {
				transform.position = end;
				shiftToPC = false;
				followPC = true;
			}
		} else if (followPC) {
			if(pc)
				transform.position = new Vector3(pc.transform.position.x, pc.transform.position.y, transform.position.z);
		} else if (followShot) {
			GameObject shot = GameObject.FindGameObjectWithTag("Shell");

			transform.position = new Vector3(shot.transform.position.x, shot.transform.position.y, transform.position.z);
		}
	}
}
