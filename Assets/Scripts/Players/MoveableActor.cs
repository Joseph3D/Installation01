using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class MoveableActor : MonoBehaviour {
	[SerializeField]
	Animator anim;
	[SerializeField]
	float speed = 5.0f;
	[SerializeField]
	float jumpForce = 10.0f;
	[SerializeField]
	Vector3 direction = Vector3.zero;
	[SerializeField]
	ForceMode forceMode = ForceMode.Impulse;

	float actorSpeed;
	Vector3 previousSpeed;

	float currentTime = 0.0f;

	public void Move(Vector3 direction = default(Vector3)) {
		this.transform.localPosition += direction * this.speed * Time.deltaTime;
		this.actorSpeed = (this.transform.localPosition - this.previousSpeed).magnitude / Time.deltaTime;
		this.previousSpeed = this.transform.localPosition;
		this.anim.SetFloat("Actor Speed", this.actorSpeed);
	}

	bool grounded = false;
	
	public bool Grounded {
		get {
			return this.grounded;
		}
	}

	public void ResetSpeed() {
		this.actorSpeed = 0.0f;
		this.anim.SetFloat("Actor Speed", 0.0f);
	}
	
	void OnCollisionEnter(Collision collision) {
		this.grounded = true;
	}
	
	void OnCollisionExit(Collision collision) {
		this.grounded = false;
	}

	public void Jump() {
		if (this.grounded == true) {
			rigidbody.AddForce(Vector3.up * this.jumpForce, this.forceMode);
		}
	}
}
