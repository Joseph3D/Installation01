using UnityEngine;
using System.Collections;

public class Crouch : MonoBehaviour 
{
	public static Crouch Instance;
	
	public float walkSpeed = 5;
	public float crouchSpeed = 3;
	
	private CharacterMotor charMotor;
	private CharacterController charController;
	private Transform theTransform;
	private float charHeight;
	
	public bool isCrouching;
	
	// Use this for initialization
	void Start () 
	{
		Instance = this;
		
		charMotor = GetComponent<CharacterMotor>();
		theTransform = transform;
		charController = GetComponent<CharacterController>();
		charHeight = charController.height;
	}
	
	// Update is called once per frame
	void Update () 
	{
		float h = charHeight;
		float speed = walkSpeed;

		charMotor.movement.maxForwardSpeed = speed;
		charMotor.movement.maxSidewaysSpeed = speed;
		charMotor.movement.maxBackwardsSpeed = speed - 0.5f;
		
		if (Input.GetKey(KeyCode.LeftControl) )
		{
			h = charHeight*0.6f;
			speed = crouchSpeed;
			charMotor.movement.maxForwardSpeed = speed;
			charMotor.movement.maxSidewaysSpeed = speed;
			charMotor.movement.maxBackwardsSpeed = speed - 0.5f;
		}
		else 
		{
			
			h = charHeight/0.6f;
			speed = walkSpeed;
		}
		
		 // Setting the max speed
		float lastHeight = charController.height; //Stand up crouch smoothly
		charController.height = Mathf.Lerp(charController.height, h, 5*Time.deltaTime);
		theTransform.position = new Vector3(theTransform.position.x, theTransform.position.y + (charController.height-lastHeight)/2,theTransform.position.z); //Fix vertical position
	}
}
