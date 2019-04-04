using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class respawn : MonoBehaviour
{

	public AudioClip death;
	public AudioClip checkpoint;
	public AudioClip goal;

	AudioSource audioSource;

	private Vector3 startPos;
	private Quaternion startRot;

	// Use this for initialization
	void Start ()
	{
		startPos = transform.position;
		startRot = transform.rotation;

		audioSource = GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void OnTriggerEnter (Collider col)
	{
		if (col.tag == "death") {
			transform.position = startPos;
			transform.rotation = startRot;
			GetComponent<Animator> ().Play ("LOSE00", -1, 0f);
			GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
			GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0f, 0f, 0f);
			audioSource.PlayOneShot (death, 0.7f);
		} else if (col.tag == "checkpoint") {
			startPos = col.transform.position;
			startRot = col.transform.rotation;
			Destroy (col.gameObject);
			audioSource.PlayOneShot (checkpoint, 0.7f);
		} else if (col.tag == "goal") {
			GetComponent<Animator> ().Play ("WIN00", -1, 0f);
			Destroy (col.gameObject);
			audioSource.PlayOneShot (goal, 0.7f);
		}
	}
}
