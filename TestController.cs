using UnityEngine;
using System.Collections;

public class TestController : MonoBehaviour {

	public bool jump = false;

	public float moveForce = 365f;
	public float moveForceRun = 600f;
	public float maxSpeed = 5f;
	public float jumpForce = 1000f;

	private Transform groundDetector;
	private bool grounded = false;

	//my extra variables for fututre use
	private float DEFAULT_MAX_WALK_SPEED = 5f;
	private float DEFAULT_MAX_RUN_SPEED = 8f;
	/*
		DEFAULT_HORIZONTAL_ACCELERATION

		DEFAULT_MAX_JUMP_HEIGHT

	*/

	void Awake(){
		groundDetector = transform.Find ("GroundDetector");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// The player is grounded if a linecast to the groundcheck position hits anything on the ground layer.
		grounded = Physics2D.Linecast(transform.position, groundDetector.position, 1 << LayerMask.NameToLayer("Terrain"));  
		// If the jump button is pressed and the player is grounded then the player should jump.
		if(Input.GetButtonDown("Jump") && grounded)
			jump = true;
	}

	void FixedUpdate () {
		// Cache the horizontal input.
		float h = Input.GetAxis("Horizontal");
		if (Input.GetButton ("z")) {
			maxSpeed = DEFAULT_MAX_RUN_SPEED;
		} 
		else {
			maxSpeed = DEFAULT_MAX_WALK_SPEED;
		}

		if (h * rigidbody2D.velocity.x < maxSpeed) {
				rigidbody2D.AddForce(Vector2.right * h * moveForce);
		}

		if(Mathf.Abs(rigidbody2D.velocity.x) > maxSpeed)
			rigidbody2D.velocity = new Vector2(Mathf.Sign(rigidbody2D.velocity.x) * maxSpeed, rigidbody2D.velocity.y);
		
		// If the player should jump...
		if(jump){	
			rigidbody2D.AddForce(new Vector2(0f, jumpForce));
			jump = false;
		}
	}
}
