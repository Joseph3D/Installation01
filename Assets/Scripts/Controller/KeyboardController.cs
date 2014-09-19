using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour {
	#region Movement Codes
	KeyCode back = KeyCode.None;
	KeyCode forward = KeyCode.None;
	KeyCode left = KeyCode.None;
	KeyCode right = KeyCode.None;
	#endregion

	#region Action
	KeyCode action = KeyCode.None;
	KeyCode crouch = KeyCode.None;
	KeyCode flashLight = KeyCode.None;
	KeyCode jump = KeyCode.None;
	KeyCode melee = KeyCode.None;
	KeyCode primarySwitch = KeyCode.None;
	KeyCode reload = KeyCode.None;
	KeyCode secondarySwitch = KeyCode.None;
	KeyCode swap = KeyCode.None;
	#endregion

	#region Mouse
	MouseButtons primary = MouseButtons.LeftClick;
	MouseButtons secondary = MouseButtons.RightClick;
	MouseButtons zoom = MouseButtons.MiddleClick;
	#endregion

	void Start() {
		this.BuildController();
	}

	void BuildController() {
		KeyboardBinds kbs = new KeyboardBinds();
		kbs = kbs.LoadXML();

		this.back = kbs.Back;
		this.forward = kbs.Forward;
		this.left = kbs.Left;
		this.right = kbs.Right;

		this.action = kbs.Action;
		this.crouch = kbs.Crouch;
		this.flashLight = kbs.FlashLight;
		this.jump = kbs.Jump;
		this.melee = kbs.Melee;
		this.primarySwitch = kbs.PrimarySwitch;
		this.reload = kbs.Reload;
		this.secondarySwitch = kbs.SecondarySwitch;
		this.swap = kbs.Swap;

		this.primary = kbs.Primary;
		this.secondary = kbs.Secondary;
		this.zoom = kbs.Zoom;
	}

	private float smooth = 0.0f;
	private float speed = 2.0f;

	void Update() {
		#region Action
		if(Input.GetKey(this.action)) {

		}

		if(Input.GetKeyDown(this.crouch)) {

		}

		if(Input.GetKeyDown(this.flashLight)) {

		}

		if(Input.GetKeyDown(this.jump)) {
			this.GetComponent<MoveableActor>().Jump();
		}

		if(Input.GetKeyDown(this.melee)) {

		}

		if(Input.GetKeyDown(this.primarySwitch)) {

		}

		if(Input.GetKeyDown(this.swap)) {

		}
		#endregion

		#region Mouse
		if(Input.GetMouseButton((int)this.primary)) {

		}

		if(Input.GetMouseButton((int)this.secondary)) {

		}

		if(Input.GetMouseButtonDown((int)this.zoom)) {

		}
		#endregion

		#region Movement
		// Checks for Diagnol movement
		if((Input.GetKey(this.forward) || Input.GetKey(this.back)) == true && (Input.GetKey(this.left) || Input.GetKey(this.right)) == true) {
			if(Input.GetKey(this.forward) && (Input.GetKey(this.left) || Input.GetKey(this.right)) == true) {
				this.smooth += Time.deltaTime * this.speed;
				
				if(this.smooth > 1.0f) {
					this.smooth = 1.0f;
				}
				
				if(Input.GetKey(this.left)) {
					this.GetComponent<MoveableActor>().Move(new Vector3((-this.smooth / Mathf.Sqrt(2.0f)) * this.transform.right.x, 0.0f, (this.smooth / Mathf.Sqrt(2.0f)) * this.transform.forward.z));
				}
				
				if(Input.GetKey(this.right)) {
					this.GetComponent<MoveableActor>().Move(new Vector3((this.smooth / Mathf.Sqrt(2.0f)) * this.transform.right.x, 0.0f, (this.smooth / Mathf.Sqrt(2.0f)) * this.transform.forward.z));
				}
				
				return;
			}
			
			if(Input.GetKey(this.back) && (Input.GetKey(this.left) || Input.GetKey(this.right)) == true) {
				this.smooth += Time.deltaTime * this.speed;
				
				if(this.smooth > 1.0f) {
					this.smooth = 1.0f;
				}
				
				if(Input.GetKey(this.left)) {
					this.GetComponent<MoveableActor>().Move(new Vector3((-this.smooth / Mathf.Sqrt(2.0f)) * this.transform.right.x, 0.0f, (-this.smooth / Mathf.Sqrt(2.0f)) * this.transform.forward.z));
				}
				
				if(Input.GetKey(this.right)) {
					this.GetComponent<MoveableActor>().Move(new Vector3((this.smooth / Mathf.Sqrt(2.0f)) * this.transform.right.x, 0.0f, (-this.smooth / Mathf.Sqrt(2.0f)) * this.transform.forward.z));
				}
				
				return;
			}
		}
		
		// Checks if the player is moving up.
		if(Input.GetKey(this.forward)) {
			this.smooth += Time.deltaTime * this.speed;
			
			if(this.smooth > 1.0f) {
				this.smooth = 1.0f;
			}
			this.GetComponent<MoveableActor>().Move(new Vector3(0.0f, 0.0f, this.smooth * this.transform.forward.z));
		}
		
		// Checks if the player is moving down.
		if(Input.GetKey(this.back)) {
			this.smooth += Time.deltaTime * this.speed;
			
			if(this.smooth > 1.0f) {
				this.smooth = 1.0f;
			}
			this.GetComponent<MoveableActor>().Move(new Vector3(0.0f, 0.0f, -this.smooth * this.transform.forward.z));
		}
		
		// Checks if the player is moving left.
		if(Input.GetKey(this.left)) {
			this.smooth += Time.deltaTime * this.speed;
			
			if(this.smooth > 1.0f) {
				this.smooth = 1.0f;
			}
			this.GetComponent<MoveableActor>().Move(new Vector3(-this.smooth * this.transform.right.x, 0.0f, 0.0f));
		}
		
		// Checks if the player is moving right.
		if(Input.GetKey(this.right)) {
			this.smooth += Time.deltaTime * this.speed;
			
			if(this.smooth > 1.0f) {
				this.smooth = 1.0f;
			}
			this.GetComponent<MoveableActor>().Move(new Vector3(this.smooth * this.transform.right.x, 0.0f, 0.0f));
		}
		
		// Checks if the player is no longer trying to move.
		if(!Input.GetKey(this.left) && !Input.GetKey(this.right) && !Input.GetKey(this.forward) && !Input.GetKey(this.back)) {
			this.smooth = 0.0f;
			this.GetComponent<MoveableActor>().ResetSpeed();
		}
		#endregion
	}
}
