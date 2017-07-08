using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {
	private bool displayLabel = false;
	private int FontSize = 32;

	void Start () {
	
	}

	void Update () {
			Application.LoadLevel (1);
	}
	
}
