using UnityEngine;
using System.Collections;

public class playermovement : MonoBehaviour {
	/*our speeds*/
	public int walkSpeed = 10;
	public int runSpeed = 20;
	public int jumpSpeed = 15;
	public int fallingSpeed = 3;
	
	/*isAir = Is player in air?*/
	public bool inAir;
	/*Is right/left arrowkey useable?*/
	public bool rightArrowKey;
	public bool leftArrowKey;
	
	public void Start()
	{
		Time.timeScale = 0.5f;
		/*In start we set inAir to false and our keys to useable*/
		inAir = false;
		rightArrowKey = true;
		leftArrowKey = true;
	}
	public void Update () {
		/*Check if Right arrow key is pressed and its useable*/
		if(Input.GetKey(KeyCode.RightArrow) && rightArrowKey == true)
		{       /*Checks if shift is pressed at the same time then it moves * runSpeed instead of walkSpeed. If not it will move by walkspeed*/
			if(Input.GetKey(KeyCode.LeftShift))
			{
				transform.Translate(Vector3.right * runSpeed * Time.deltaTime);
			}
			else
			{
				transform.Translate(Vector3.right * walkSpeed * Time.deltaTime);
			}
			
		}
		/*Same to left arrow key and movement*/
		if(Input.GetKey(KeyCode.LeftArrow) && leftArrowKey == true)
		{
			if(Input.GetKey(KeyCode.LeftShift))
			{transform.Translate(Vector3.left * runSpeed * Time.deltaTime);
			}
			else
			{
				transform.Translate(Vector3.left * walkSpeed * Time.deltaTime);
			}
		}
		/*Checks if space is pressed*/
		if(Input.GetKey(KeyCode.Space) && inAir == false)
		{
			/*moves player up * jumpSpeed*/
			transform.Translate(Vector3.up * jumpSpeed * Time.deltaTime);
			/*Starts Ienumerator jump*/
			StartCoroutine("jump");
		}
		/*Check if player collides anything under it*/
		if (Physics.Raycast(transform.position,Vector3.down,transform.localScale.y / 2) || Physics.Raycast(transform.position - new Vector3(transform.localScale.x /2,0,0),Vector3.down,transform.localScale.y / 2) || Physics.Raycast(transform.position + new Vector3(transform.localScale.x / 2,0,0),Vector3.down,transform.localScale.y / 2))
		{
			/*inAir is then set to false*/
			inAir = false;
			
		}
		/*if raycast dsnt collide anything under player it must be in ground so we set it to start falling down*/
		else
		{
			walkSpeed = 10;
			runSpeed = 15;
			fallingSpeed = 15;
			transform.Translate(Vector3.down * fallingSpeed * Time.deltaTime);
		}
		/*checks if there is anything infront of player and disables right arrow key so player cant move throught objects*/
		if (Physics.Raycast(transform.position,Vector3.right,transform.localScale.y /2))
		{
			//Debug.Log ("something infront of player");
			rightArrowKey = false;
			
		}
		/*If nothing is infront of player you can continue moving on*/
		else
		{
			rightArrowKey = true;
		}
		/*Same to left side*/
		if (Physics.Raycast(transform.position,Vector3.left,transform.localScale.y /2))
		{
			//Debug.Log ("something infront of player");
			leftArrowKey = false;
			
		}
		else
		{
			leftArrowKey = true;
		}
		
	}
	/*Coroutine that we start if we jump. It will wait one millisecond and then it realizes its in air and starts falling down, this is done so it doesn't start to fall down same time as it jumps*/
	IEnumerator jump()
	{
		yield return new WaitForSeconds(0.1f);
		inAir = true;
	}
}