using UnityEngine;
using System.Collections;

/// <summary>
/// CERTIFIED SHITTY CODE
/// TODO: REWRITE
/// </summary>
public class Footsteps : MonoBehaviour 
{
	public float AudioTimer;
	public float SoundNumber;

	public AudioClip DirtSound1;
	public AudioClip DirtSound2;
	public AudioClip MetalSound1;
	public AudioClip MetalSound2;

	public CharacterController CharCont;
	
	void Start () 
	{
		audio.volume = 0.1F;
	}
	
	
	void Update () 
	{

		if(AudioTimer > 0)
			AudioTimer -= Time.deltaTime;
		
		if(AudioTimer < 0)
			AudioTimer = 0;

		SoundNumber = Random.Range(0,10);
	}
	

	void OnControllerColliderHit(ControllerColliderHit col)
	{
		if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) && CharCont.isGrounded && CharCont.velocity.magnitude <= 3)
		{
			if(col.gameObject.CompareTag("Dirt") && AudioTimer == 0)
			{
				if(SoundNumber > 5)
				{
					audio.clip = DirtSound1;
					audio.PlayOneShot(DirtSound1);
					AudioTimer = 0.4f;
				}
				else
				{
					audio.clip = DirtSound2;
					audio.PlayOneShot(DirtSound2);
					AudioTimer = 0.4f;
				}

			}
		
			if(col.gameObject.CompareTag("Metal") && AudioTimer == 0 || col.gameObject.CompareTag("concrete") && AudioTimer == 0 )
			{
				if(SoundNumber > 5)
				{
					audio.clip = MetalSound1;
					audio.PlayOneShot(MetalSound1);
					AudioTimer = 0.4f;
				}
				else
				{
					audio.clip = MetalSound2;
					audio.PlayOneShot(MetalSound2);
					AudioTimer = 0.4f;
				}
			}

			if(col.gameObject.CompareTag("Terrain") && AudioTimer == 0)
			{
				if(SoundNumber > 5)
				{
					audio.clip = DirtSound1;
					audio.PlayOneShot(DirtSound1);
					AudioTimer = 0.4f;
				}
				else
				{
					audio.clip = DirtSound2;
					audio.PlayOneShot(DirtSound2);
					AudioTimer = 0.4f;
				}
			}
		}
	}
}
