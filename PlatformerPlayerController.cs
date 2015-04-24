using UnityEngine;
using System.Collections;

public class PlatformerPlayerController : MonoBehaviour {
	
	private const float DEFAULT_GRAVITY_ACCELERATION = 1.1f;
	private const float DEFAULT_CURRENT_GRAVITY = .01f;
	
	private const float DEFAULT_WALK_ACCELERATION = 1f;
	private const float DEFAULT_RUN_ACCELERATION = 1.5f;
	private const float DEFAULT_JUMP_ACCELERATION = 1f;
	
	private const float DEFAULT_MAX_WALK_SPEED = 3f;
	private const float DEFAULT_MAX_RUN_SPEED = 5f;
	private const float DEFAULT_MAX_JUMP_HEIGHT = 10f;
	
	private const float DEFAULT_JUMP_TIME = 4;
	
	private const float DEFAULT_COLLISION_RAYCAST_SIZE = .001f;
	
	public float movementSpeed = .3f;
	
	public bool rising = false;
	public bool falling = false;
	public bool grounded = false;
	
	public bool collided = false;
	public bool jumping = false;
	public bool running = false;
	
	public float jumpTime = 10;
	
	//physics values
	//gravity values
	public float currentGravity = .01f;
	public float gravityAcceleration = 1.1f;
	public float maxGravity = .03f;
	//horizontal physics -calculates player movment inputs
	public float horizontalVelocity = .005f;
	public float horizontalAcceleration = 1.05f;
	public float horizontalDecay = 0.7f; 
	public float maxVelocity = .1f;
	
	//TODO see if this will work for the jump values
	//vertical physics = needed for jumps?
	public float verticalVelocity = .01f;
	
	//final movement amounts per update
	private float horizontalMovement;
	private float verticalMovement;
	
	public bool debug = false;
	
	// THE USE OF THESE VECTORS REQUIRE THAT THE PLAYER BOX COLLIDER ALWAYS BE 1*1*1 DIMENSIONS
	private GameObject bottomLeft;
	private GameObject bottomRight;
	private GameObject topLeft;
	private GameObject topRight;
	
	void Start () {
		gravityAcceleration = DEFAULT_GRAVITY_ACCELERATION;
		this.SetObject2DBoxColliderCorners();
	}
	
	void Update () {
		
		if(Input.GetButton("Run")){
			if(grounded){
				running = true;
			}
		}
		else{
			running = false;
		}
		
		//TODO add timer to make it, add a jump delay, and jump based on how clong it is held down 
		if(Input.GetButtonDown("Jump")){
			if(grounded){
				jumping = true;
			}
		}
	}
	
	void FixedUpdate(){
		
		//FIRST DO ALL THE PLAYER MOVEMENT CALCULATIONS
		
		//apply gravity
		if(currentGravity < maxGravity){
			currentGravity = currentGravity * gravityAcceleration;
			if(currentGravity > maxGravity) currentGravity = maxGravity;
		}
		
		//slow down if no horizontal input is going in
		if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) == 0){
			if(horizontalVelocity > .01f){
				horizontalVelocity = horizontalVelocity * horizontalDecay;
				if(horizontalVelocity < .01f) horizontalVelocity = .01f;
			}
		}
		//speed up with any horizontal input
		else{
			if(running){
				if(horizontalVelocity < maxVelocity){
					horizontalVelocity = horizontalVelocity * horizontalAcceleration;
					if(horizontalVelocity > maxVelocity) horizontalVelocity = maxVelocity;
				}
			}
			else{ //Walking
				if(horizontalVelocity < maxVelocity){
					horizontalVelocity = horizontalVelocity * horizontalAcceleration;
					if(horizontalVelocity > maxVelocity) horizontalVelocity = maxVelocity;
				}
			}
		}
		
		//TODO fix the jumping, rather than have a jump time, apply a set amount of verticle velocity that slow decreases
		if(jumping){
			Debug.Log("jumping");
			grounded = false;
			
			verticalVelocity = verticalVelocity + 5f
			
			/* while(jumpTime > 0){
				verticalMovement = verticalMovement + .15f;
				jumpTime = jumpTime - 1;
			}
			if(jumpTime <= 0){
				//reset variables
				jumpTime = DEFAULT_JUMP_TIME;
				jumping = false;
			} */
		}
		
		//calculate the final movement values here
		verticalMovement = verticalVelocity - currentGravity;
		horizontalMovement = horizontalVelocity * Input.GetAxisRaw("Horizontal");
		
		//THEN USE THE RAY CASTS TO CHECK IF THE MOVEMENT IS POSSIBLE
		RaycastHit2D bottomRaycastHit;
		RaycastHit2D topRaycastHit;
		RaycastHit2D leftRaycastHit;
		RaycastHit2D rightRaycastHit;
		
		if(Mathf.Abs(verticalMovement) < DEFAULT_COLLISION_RAYCAST_SIZE){
			bottomRaycastHit = RaycastInALine(bottomLeft.transform.position,bottomRight.transform.position,6,transform.TransformDirection(Vector3.down),-DEFAULT_COLLISION_RAYCAST_SIZE);
			topRaycastHit = RaycastInALine(topLeft.transform.position,topRight.transform.position,6,transform.TransformDirection(Vector3.up),DEFAULT_COLLISION_RAYCAST_SIZE);
		}
		else{
			bottomRaycastHit = RaycastInALine(bottomLeft.transform.position,bottomRight.transform.position,6,transform.TransformDirection(Vector3.down),-verticalMovement);
			topRaycastHit = RaycastInALine(topLeft.transform.position,topRight.transform.position,6,transform.TransformDirection(Vector3.up),verticalMovement);
		}
		if(Mathf.Abs(horizontalMovement) < DEFAULT_COLLISION_RAYCAST_SIZE){
			leftRaycastHit = RaycastInALine(bottomLeft.transform.position,topLeft.transform.position,6,transform.TransformDirection(Vector3.left),-DEFAULT_COLLISION_RAYCAST_SIZE);
			rightRaycastHit = RaycastInALine(bottomRight.transform.position,topRight.transform.position,6,transform.TransformDirection(Vector3.right),DEFAULT_COLLISION_RAYCAST_SIZE);
		}
		else{
			leftRaycastHit = RaycastInALine(bottomLeft.transform.position,topLeft.transform.position,6,transform.TransformDirection(Vector3.left),-horizontalMovement);
			rightRaycastHit = RaycastInALine(bottomRight.transform.position,topRight.transform.position,6,transform.TransformDirection(Vector3.right),horizontalMovement);
		}
		
		RaycastHit2D bottomLeftAngleRaycast = Physics2D.Raycast(bottomLeft.transform.position,Vector3.down,DEFAULT_COLLISION_RAYCAST_SIZE+.001f);
		RaycastHit2D bottomRightAngleRaycast = Physics2D.Raycast(bottomRight.transform.position,Vector3.down,DEFAULT_COLLISION_RAYCAST_SIZE+.001f);
	
		if(debug){
			Debug.DrawRay(bottomLeft.transform.position,Vector3.down,Color.red);
			Debug.DrawRay(bottomRight.transform.position,Vector3.down,Color.red);
			Debug.Log("Left: "+bottomLeftAngleRaycast.normal);
			Debug.Log("Right: "+bottomRightAngleRaycast.normal);
		}
		
		//TODO make sure that when grounded we edit the translation so the object does not go through
		//moving downwards
		if(verticalMovement < 0){
			if(bottomRaycastHit){
				if(debug){
					//Debug.Log("grounded with:"+ bottomRaycastHit.collider.gameObject.name );
					//Debug.Log(bottomRaycastHit.distance);
				}
				
				if(!grounded)transform.Translate(horizontalMovement,DEFAULT_COLLISION_RAYCAST_SIZE-bottomRaycastHit.distance,0);
				//if(!grounded)transform.rotation = Quaternion.FromToRotation(Vector3.up, bottomLeftAngleRaycast.normal);
				
				verticalMovement = 0;
				currentGravity = DEFAULT_CURRENT_GRAVITY;
				grounded = true;
				
				if(bottomLeftAngleRaycast){
					
					Debug.Log("Left touching");
				}
				if(bottomRightAngleRaycast){
					
					Debug.Log("Right touching");
					int time = 0;
					if(!bottomLeftAngleRaycast){
						//transform.RotateAround(bottomRight.transform.localPosition,Vector3.back,1f);
						transform.RotateAround(bottomRight.transform.position,Vector3.forward,5f);
						Debug.Log("ROTATING!!");
					}
				}
				
			}
			else{
				grounded = false;
			}
		}
		
		if(verticalMovement > 0){
			if(topRaycastHit){
				if(debug){
					Debug.Log("hit cieling with:"+ topRaycastHit.collider.gameObject.name );
					Debug.Log(topRaycastHit.normal);
				}
				
				transform.Translate(horizontalMovement,topRaycastHit.distance,0);
				verticalMovement = 0;
				jumping = false;
			}
		}
		
		
		
		if(leftRaycastHit && (horizontalMovement < 0)){
			if(debug) Debug.Log("left collision");
			if(debug) Debug.Log(leftRaycastHit.collider.gameObject.name);
			transform.Translate(leftRaycastHit.distance,verticalMovement,0);
			//move against the wall
			horizontalMovement = 0;
		}
		if(rightRaycastHit && (horizontalMovement > 0)){
			if(debug) Debug.Log("right collision");
			if(debug) Debug.Log(rightRaycastHit.collider.gameObject.name);
			transform.Translate(leftRaycastHit.distance,verticalMovement,0);
			//move against the wall
			horizontalMovement = 0;
		}
		
		//the finalized movement
		transform.Translate(horizontalMovement,verticalMovement,0);
		
		
		//resets
		collided = false; 
		verticalMovement = 0;
		horizontalMovement = 0;
		//should reset the verticalmovment and horizontalmovements per update as well?
	}
	
	/// <summary>
	///  HELPER METHODS
	/// </summary>
	
	//method needs uses variables local to this class
	void SetObject2DBoxColliderCorners() {
		
		this.transform.localRotation.Set(0,0,0,0);
		
		bottomLeft = new GameObject("Bottom Left");
		bottomRight = new GameObject("Bottom Right");
		topLeft = new GameObject("Top Left");
		topRight = new GameObject("Top Right");
		
		bottomLeft.transform.SetParent(this.transform);
		bottomRight.transform.SetParent(this.transform);
		topLeft.transform.SetParent(this.transform);
		topRight.transform.SetParent(this.transform);
		
		bottomLeft.transform.Translate(new Vector2(this.collider2D.bounds.min.x,this.collider2D.bounds.min.y));
		bottomRight.transform.Translate(new Vector2(this.collider2D.bounds.max.x,this.collider2D.bounds.min.y));
		topLeft.transform.Translate(new Vector2(this.collider2D.bounds.min.x,this.collider2D.bounds.max.y));
		topRight.transform.Translate(new Vector2(this.collider2D.bounds.max.x,this.collider2D.bounds.max.y));
	}
	
	//TODO move to a common library?
	/// <summary>
	///  MODULAR 
	/// </summary>
	RaycastHit2D RaycastInALine(Vector2 point1, Vector2 point2, int numberOfRayCasts){
		float lerpAmount = 0;
		float lerpRate = 1;
		float previousLerpRate = 1;
		
		RaycastHit2D hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),Vector3.down);
		
		for(int i = 0; i <= numberOfRayCasts; i++){
			
			hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),Vector3.down);
			if(hit){
				return hit;
			}
			
			lerpAmount = lerpAmount + previousLerpRate;
			
			if(lerpAmount > 1){
				previousLerpRate = lerpRate;
				lerpRate = lerpRate/2;
				lerpAmount = lerpRate;
			}
		}
		
		return hit;
	}
	RaycastHit2D RaycastInALine(Vector2 point1, Vector2 point2, int numberOfRayCasts, Vector3 direction){
		float lerpAmount = 0;
		float lerpRate = 1;
		float previousLerpRate = 1;
		
		RaycastHit2D hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),direction,.01f);
		
		for(int i = 0; i <= numberOfRayCasts; i++){
			hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),direction);
			if(hit){
				return hit;
			}
			
			lerpAmount = lerpAmount + previousLerpRate;
			
			if(lerpAmount > 1){
				previousLerpRate = lerpRate;
				lerpRate = lerpRate/2;
				lerpAmount = lerpRate;
			}
		}
		
		return hit;
	}
	RaycastHit2D RaycastInALine(Vector2 point1, Vector2 point2, int numberOfRayCasts, Vector3 direction, float distance){
		float lerpAmount = 0;
		float lerpRate = 1;
		float previousLerpRate = 1;
		
		RaycastHit2D hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),direction,distance);
		
		for(int i = 0; i <= numberOfRayCasts; i++){
			hit = Physics2D.Raycast(Vector2.Lerp(point1,point2,lerpAmount),direction,distance);
			if(hit){
				return hit;
			}
			
			lerpAmount = lerpAmount + previousLerpRate;
			
			if(lerpAmount > 1){
				previousLerpRate = lerpRate;
				lerpRate = lerpRate/2;
				lerpAmount = lerpRate;
			}
		}
		
		return hit;
	}
	
}
