using System.Collections;
using UnityEngine;

namespace ethanAI.Characters.ThirdPerson
{
	public class enemySight : MonoBehaviour {

		public UnityEngine.AI.NavMeshAgent agent;
		public ThirdPersonCharacter character;

		public enum State
		{
			PATROL,
			CHASE,
			INVESTIGATE
		}

		public State state;
		private bool alive;

		public GameObject[] waypoints;
		private int waypointInd;
		public float patrolSpeed = 0.5f;

		public float chaseSpeed = 1f;
		public GameObject target;

		private Vector3 artiStartPos;
		private Quaternion artiStartRot;
		private Vector3 targetStartPos;
		private Quaternion targetStartRot;
		public float radius = 1.0f;

		public AudioClip death;
		AudioSource audioSource;

		private Vector3 investigateSpot;
		private float timer = 0;
		public float investigateWait = 10;

		public float heightMultiplier;
		public float sightDist = 14;

		// Use this for initialization
		void Start () {
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			character = GetComponent<ThirdPersonCharacter>();

			agent.updatePosition = true;
			agent.updateRotation = false;

			waypoints = GameObject.FindGameObjectsWithTag ("Waypoint");
			waypointInd = Random.Range (0,waypoints.Length);

			state = enemySight.State.PATROL;

			alive = true;

			heightMultiplier = 1.41f;

			targetStartPos = target.transform.position;
			targetStartRot = target.transform.rotation;

			artiStartPos = this.transform.position;
			artiStartRot = this.transform.rotation;

			audioSource = GetComponent<AudioSource> ();

			StartCoroutine ("FSM");
		}

		IEnumerator FSM(){
			while (alive) {
				switch (state) {
				case State.PATROL:
					Patrol ();
					break;
				case State.CHASE:
					Chase ();
					break;
				case State.INVESTIGATE:
					Investigate ();
					break;
				}
				yield return null;
			}
		}

		void Patrol()
		{
			agent.speed = patrolSpeed;
			if (Vector3.Distance (this.transform.position, waypoints [waypointInd].transform.position) >= 2) {
				agent.SetDestination (waypoints [waypointInd].transform.position);
				character.Move (agent.desiredVelocity, false, false);
			} else if (Vector3.Distance(this.transform.position, waypoints [waypointInd].transform.position) <= 2 )
			{
				waypointInd = Random.Range (0,waypoints.Length);
			}
			else
			{
				character.Move(Vector3.zero, false, false);	
			}
		}

		void Chase ()
		{
			agent.speed = chaseSpeed;
			agent.SetDestination (target.transform.position);
			character.Move (agent.desiredVelocity, false, false);
		}

		void Investigate ()
		{
			timer += Time.deltaTime;

			agent.SetDestination (this.transform.position);
			character.Move (Vector3.zero, false, false);
			transform.LookAt (investigateSpot);
			if (timer >= investigateWait)
			{
				state = enemySight.State.PATROL;
				timer = 0;
			}

		}

		void OnTriggerEnter (Collider coll)
		{
			if (coll.tag == "Player")
			{
				state = enemySight.State.INVESTIGATE;
				investigateSpot = coll.gameObject.transform.position;
			}
		}

		void Update ()
		{
			if (Vector3.Distance (this.transform.position, target.transform.position) < radius)
			{
				target.transform.position = targetStartPos;
				target.transform.rotation = targetStartRot;
				//this.transform.position = artiStartPos;
				//this.transform.rotation = artiStartRot;
				target.GetComponent<Animator> ().Play ("LOSE00", -1, 0f);
				target.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
				target.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0f, 0f, 0f);
				state = enemySight.State.PATROL;
				audioSource.PlayOneShot (death, 0.7f);
			}
		}

		void FixedUpdate ()
		{
			RaycastHit hit;
			Debug.DrawRay (transform.position + Vector3.up * heightMultiplier, transform.forward * sightDist, Color.green);
			Debug.DrawRay (transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized * sightDist, Color.green);
			Debug.DrawRay (transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized * sightDist, Color.green);

			if (Physics.Raycast (transform.position + Vector3.up * heightMultiplier, transform.forward, out hit, sightDist))
			{
				if (hit.collider.gameObject.tag == "Player")
				{
					state = enemySight.State.CHASE;
					target = hit.collider.gameObject;
				}
			}

			if (Physics.Raycast (transform.position + Vector3.up * heightMultiplier, (transform.forward + transform.right).normalized, out hit, sightDist))
			{
				if (hit.collider.gameObject.tag == "Player")
				{
					state = enemySight.State.CHASE;
					target = hit.collider.gameObject;
				}
			}

			if (Physics.Raycast (transform.position + Vector3.up * heightMultiplier, (transform.forward - transform.right).normalized, out hit, sightDist))
			{
				if (hit.collider.gameObject.tag == "Player")
				{
					state = enemySight.State.CHASE;
					target = hit.collider.gameObject;
				}
			}
		}
	}
}

