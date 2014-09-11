using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	private Vector3 moveDirection;
	public CharacterController characterController;
	public float speed = 2;
	public float jumpSpeed = 3;
	private float vSpeed;
	public bool gravityEnabled = true;
	public float gravity = 10;

	void PlayerMovementController(){
		if(Input.GetAxis("Horizontal") != 0 && Input.GetAxis("Vertical") != 0){
			moveDirection = new Vector3(Input.GetAxis("Horizontal") * 0.707f, 0, Input.GetAxis("Vertical") * 0.707f);
		} else {
			moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
		}
		moveDirection = transform.TransformDirection(moveDirection);
		moveDirection *= speed;
		if(characterController.isGrounded){
			vSpeed = -1;
			if (Input.GetButtonDown ("Jump")) {
				vSpeed = jumpSpeed;
			}
		}
		if(gravityEnabled)
			vSpeed -= gravity * Time.deltaTime;
		moveDirection.y = vSpeed;
		characterController.Move(moveDirection * Time.deltaTime);
	}
}
