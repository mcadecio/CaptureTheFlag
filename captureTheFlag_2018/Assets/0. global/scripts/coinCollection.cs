using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class coinCollection : MonoBehaviour {

	// Use this for initialization
	void OnTriggerEnter (Collider coll) {
		ScoreTextScript.coinAmount += 1;
		Destroy (gameObject);
	}
}