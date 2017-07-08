using UnityEngine;

public class WeaponController : MonoBehaviour {

	void Update(){
		gameObject.transform.rotation = transform.parent.GetComponentInChildren<ReticuleController> ().transform.rotation;
	}

}