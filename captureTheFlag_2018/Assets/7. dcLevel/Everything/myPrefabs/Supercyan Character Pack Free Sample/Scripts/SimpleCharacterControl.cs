using UnityEngine;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;
using UnityEngine.UI;
using unityChan.CrossPlatformInput;

public class SimpleCharacterControl : MonoBehaviour {

    private enum ControlMode
    {
        Tank,
        Direct
    }

	protected float CameraAngle;
	protected float CameraAngleSpeed = 2f;

	protected int count;
    public Text countText;

	public InputField input;

	public FixedJoystick RightJoystick;

    private Boolean created;

    private Boolean isButton;

    public FixedButton JumpButton;

    [SerializeField] private float m_moveSpeed = 2;
    [SerializeField] private float m_turnSpeed = 200;
    [SerializeField] private float m_jumpForce = 4;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Rigidbody m_rigidBody;

    [SerializeField] private ControlMode m_controlMode = ControlMode.Direct;

    private float m_currentV = 0;
    private float m_currentH = 0;

    private readonly float m_interpolation = 10;
    private readonly float m_walkScale = 0.33f;
    private readonly float m_backwardsWalkScale = 0.16f;
    private readonly float m_backwardRunScale = 0.66f;

    private bool m_wasGrounded;
    private Vector3 m_currentDirection = Vector3.zero;

    private float m_jumpTimeStamp = 0;
    private float m_minJumpInterval = 0.25f;

    private bool m_isGrounded;
    private List<Collider> m_collisions = new List<Collider>();

    private MySqlConnection connection;
	void Start()
	{
        created = false;
		count = 0;
        countText.text = "Score = 0";
        StartDatabaseConnection();
	}

    private void StartDatabaseConnection()
    {
        

        string server = "uk-ctf.uksouth.cloudapp.azure.com";
        string database = "capture_flag_game";
        string user = "root";
        string password = "pass";
        string port = "3306";
        string sslM = "none";
        string connectionString = String.Format("server={0};port={1};user id={2}; password={3}; database={4}; SslMode={5}",
            server, port, user, password, database, sslM);

        connection = new MySqlConnection(connectionString);
        try
        {
            connection.Open();
            Debug.Log("Connected to database");
        }catch(Exception e)
        {
            Debug.Log("Exception");
            Debug.Log(e.Message);
        }

        
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        for(int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                if (!m_collisions.Contains(collision.collider)) {
                    m_collisions.Add(collision.collider);
                }
                m_isGrounded = true;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        ContactPoint[] contactPoints = collision.contacts;
        bool validSurfaceNormal = false;
        for (int i = 0; i < contactPoints.Length; i++)
        {
            if (Vector3.Dot(contactPoints[i].normal, Vector3.up) > 0.5f)
            {
                validSurfaceNormal = true; break;
            }
        }

        if(validSurfaceNormal)
        {
            m_isGrounded = true;
            if (!m_collisions.Contains(collision.collider))
            {
                m_collisions.Add(collision.collider);
            }
        } else
        {
            if (m_collisions.Contains(collision.collider))
            {
                m_collisions.Remove(collision.collider);
            }
            if (m_collisions.Count == 0) { m_isGrounded = false; }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(m_collisions.Contains(collision.collider))
        {
            m_collisions.Remove(collision.collider);
        }
        if (m_collisions.Count == 0) { m_isGrounded = false; }
    }

	void FixedUpdate()
	{
       

        

    }

    private void CreateUser(string user)
    {


        MySqlCommand command = connection.CreateCommand();
     
            var t = user;

            command.CommandText = "INSERT INTO Leaderboard (name, totalscore, schoollevel, forestlevel) SELECT * FROM (SELECT @textValue, 0, 15, -1) AS tmp WHERE NOT EXISTS ( SELECT name FROM Leaderboard WHERE name = @textValue) LIMIT 1";
            command.Parameters.AddWithValue("@textValue", t);
        
     

        try
        {
            command.ExecuteNonQuery();
           
            
        }

        catch (Exception e)
        {
            Debug.Log(e.Message);

        }

    }


	void Update () {
        m_animator.SetBool("Grounded", m_isGrounded);

        if (Input.GetKey(KeyCode.Space))
        {
            //Debug.Log("space pressed");
            //CreateUser("hi");
        }
        if (isButton && !created)
        {
            var se = new InputField.SubmitEvent();
            se.AddListener(SubmitName);
            input.onEndEdit = se;
            CreateUser(input.text);
            // input.gameObject.SetActive(false);
        }
        if (isButton)
        {
            UpdateUserScore();
        }

        switch (m_controlMode)
        {
            case ControlMode.Direct:
                DirectUpdate();
                break;

            case ControlMode.Tank:
                TankUpdate();
                break;

            default:
                Debug.LogError("Unsupported state");
                break;
        }
        isButton = JumpButton.Pressed;

        m_wasGrounded = m_isGrounded;

		CameraAngle += RightJoystick.inputVector.x * CameraAngleSpeed;

		Camera.main.transform.position = transform.position + Quaternion.AngleAxis(CameraAngle, Vector3.up) * new Vector3(0, 3, 4);
		Camera.main.transform.rotation = Quaternion.LookRotation(transform.position + Vector3.up * 2f - Camera.main.transform.position, Vector3.up);
    }

    private void TankUpdate()
    {
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        bool walk = Input.GetKey(KeyCode.LeftShift);

        if (v < 0) {
            if (walk) { v *= m_backwardsWalkScale; }
            else { v *= m_backwardRunScale; }
        } else if(walk)
        {
            v *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        transform.position += transform.forward * m_currentV * m_moveSpeed * Time.deltaTime;
        transform.Rotate(0, m_currentH * m_turnSpeed * Time.deltaTime, 0);

        m_animator.SetFloat("MoveSpeed", m_currentV);

        JumpingAndLanding();
    }

    private void DirectUpdate()
    {
		float v = CrossPlatformInputManager.GetAxisRaw("Vertical");
		float h = CrossPlatformInputManager.GetAxisRaw("Horizontal");

        Transform camera = Camera.main.transform;

        if (Input.GetKey(KeyCode.LeftShift))
        {
            v *= m_walkScale;
            h *= m_walkScale;
        }

        m_currentV = Mathf.Lerp(m_currentV, v, Time.deltaTime * m_interpolation);
        m_currentH = Mathf.Lerp(m_currentH, h, Time.deltaTime * m_interpolation);

        Vector3 direction = camera.forward * m_currentV + camera.right * m_currentH;

        float directionLength = direction.magnitude;
        direction.y = 0;
        direction = direction.normalized * directionLength;

        if(direction != Vector3.zero)
        {
            m_currentDirection = Vector3.Slerp(m_currentDirection, direction, Time.deltaTime * m_interpolation);

            transform.rotation = Quaternion.LookRotation(m_currentDirection);
            transform.position += m_currentDirection * m_moveSpeed * Time.deltaTime;

            m_animator.SetFloat("MoveSpeed", direction.magnitude);
        }

        JumpingAndLanding();
    }

    private void JumpingAndLanding()
    {
        bool jumpCooldownOver = (Time.time - m_jumpTimeStamp) >= m_minJumpInterval;

        if (jumpCooldownOver && m_isGrounded && isButton)
        {
            m_jumpTimeStamp = Time.time;
            m_rigidBody.AddForce(Vector3.up * m_jumpForce, ForceMode.Impulse);
        }

        if (!m_wasGrounded && m_isGrounded)
        {
            m_animator.SetTrigger("Land");
        }

        if (!m_isGrounded && m_wasGrounded)
        {
            m_animator.SetTrigger("Jump");
        }
    }

	void OnTriggerEnter(Collider other) 
	{
		// ..and if the game object we intersect has the tag 'Pick Up' assigned to it..
		if (other.gameObject.CompareTag ("Pick up"))
		{
			// Make the other game object (the pick up) inactive, to make it disappear
			other.gameObject.SetActive (false);

			// Add one to the score variable 'count'
			count = count + 1;

			// Run the 'SetCountText()' function (see below)
			SetCountText();
		}
	}
	void SetCountText()
	{
        countText.text = "Score: " + count;        
	}

	void UpdateUserScore()
	{

		MySqlCommand command = connection.CreateCommand();
		

		command.CommandText = "Update Leaderboard SET schoollevel = @score WHERE name = @textValue";
		command.Parameters.AddWithValue("@textValue", input.text);
		command.Parameters.AddWithValue("@score", count);


		try
		{
			command.ExecuteNonQuery();
            
		}

		catch (Exception e)
		{
			Debug.Log(e.Message);

		}

	}
	void SubmitName(string arg0)
	{
		input.text = arg0;

	}
}
