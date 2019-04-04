using System.Collections;
using UnityEngine;

namespace ethanAI.Characters.ThirdPerson
{
	public class basicAI : MonoBehaviour {

		public UnityEngine.AI.NavMeshAgent agent;
		public ThirdPersonCharacter character;

		public enum State
		{
			PATROL,
			CHASE
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

		// Use this for initialization
		void Start () {
			agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			character = GetComponent<ThirdPersonCharacter>();

			agent.updatePosition = true;
			agent.updateRotation = false;

			waypoints = GameObject.FindGameObjectsWithTag ("Waypoint");
			waypointInd = Random.Range (0,waypoints.Length);

			state = basicAI.State.PATROL;

			alive = true;

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

		void OnTriggerEnter (Collider coll)
		{
			if (coll.tag == "Player")
			{
				state = basicAI.State.CHASE;
				target = coll.gameObject;
			}
		}

		void Update ()
		{
			if (Vector3.Distance (this.transform.position, target.transform.position) < radius)
			{
				target.transform.position = targetStartPos;
				target.transform.rotation = targetStartRot;
				this.transform.position = artiStartPos;
				this.transform.rotation = artiStartRot;
				target.GetComponent<Animator> ().Play ("LOSE00", -1, 0f);
				target.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, 0f);
				target.GetComponent<Rigidbody> ().angularVelocity = new Vector3 (0f, 0f, 0f);
				state = basicAI.State.PATROL;
				audioSource.PlayOneShot (death, 0.7f);
			}
		}			
	}
}

