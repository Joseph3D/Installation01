/////////////////////////////////////////////////////////////////////////////////
//
//	vp_Explosion.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	death effect for exploding objects. applies damage and a
//					physical force to all rigidbodies and players within its
//					range, plays a sound and instantiates a list of gameobjects
//					for special effects (e.g. particle fx). destroys itself when
//					the sound has stopped playing
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class vp_Explosion : MonoBehaviour
{

	// gameplay
	public float Radius = 15.0f;					// any objects within radius will be affected by the explosion
	public float Force = 1000.0f;					// amount of motion force to apply to affected objects
	public float UpForce = 10.0f;					// how much to push affected objects up in the air
	public float Damage = 10;						// amount of damage to apply to objects via their 'Damage' method
	public float CameraShake = 1.0f;				// how much of a shockwave impulse to apply to the camera
	public string DamageMessageName = "Damage";		// user defined name of damage method on affected object
														// TIP: this can be used to apply different types of damage, i.e
														// magical, freezing, poison, electric
	
	// sound
	AudioSource m_Audio = null;
	public AudioClip Sound = null;
	public float SoundMinPitch = 0.8f;				// random pitch range for explosion sound
	public float SoundMaxPitch = 1.2f;

	// fx
	public List <GameObject> FXPrefabs = new List<GameObject>();	// list of special effects objects to spawn
	
	protected Transform m_Transform = null;
	
	
	protected virtual void Awake()
	{
	
		m_Transform = transform;
		m_Audio = audio;
	
	}


	/// <summary>
	/// 
	/// </summary>
	void OnEnable()
	{

		// spawn effects gameobjects
		foreach(GameObject fx in FXPrefabs)
		{
			if (fx != null)
			{
				Component[] c;
				c = fx.GetComponents<vp_Explosion>();
				if (c.Length == 0)
					vp_Utility.Instantiate(fx, m_Transform.position, m_Transform.rotation);
				else
					Debug.LogError("Error: vp_Explosion->FXPrefab must not be a vp_Explosion (risk of infinite loop).");
			}
		}

		// apply shockwave to all rigidbodies and FPSPlayers within range, but
		// ignore small and walk-thru objects such as debris, triggers and water
		Collider[] colliders = Physics.OverlapSphere(m_Transform.position, Radius, vp_Layer.Mask.IgnoreWalkThru);
		foreach (Collider hit in colliders)
		{
			if (hit != this.collider)
			{

				float distanceModifier = (1 - Vector3.Distance(m_Transform.position,
							hit.transform.position) / Radius);

				//Debug.Log(hit.transform.name + Time.time);	// snippet to dump affected objects
				if (hit.rigidbody)
				{

					// explosion up-force should only work on grounded objects,
					// otherwise object may acquire extreme speeds. also, this
					// gives a more profound effect of explosion force being
					// deflected off the ground
					Ray ray = new Ray(hit.transform.position, -Vector3.up);
					RaycastHit hit2;
					if (!Physics.Raycast(ray, out hit2, 1))
						UpForce = 0.0f;

					// bash the found object
					hit.rigidbody.AddExplosionForce((Force / Time.timeScale) / vp_TimeUtility.AdjustedTimeScale, m_Transform.position, Radius, UpForce);

				}
				else
				{

					// bash things that listen to the 'ForceImpact' message (e.g. players)
					vp_TargetEvent<Vector3>.Send(hit.transform.root, "ForceImpact", (hit.transform.position -
																							m_Transform.position).normalized *
																							Force * 0.001f * distanceModifier);

					//// shake things that listen to the 'CameraBombShake' message (e.g. cameras)
					vp_TargetEvent<float>.Send(hit.transform.root, "CameraBombShake", (distanceModifier * CameraShake));

				}

				// damage the object, if applicable
				if (hit.gameObject.layer != vp_Layer.Debris)
				{

					hit.gameObject.BroadcastMessage(DamageMessageName, new vp_DamageInfo(distanceModifier * Damage, transform),
																SendMessageOptions.DontRequireReceiver);	// TODO: use targetevent

				}
				
			}

		}

		// play explosion sound
		m_Audio.clip = Sound;
		m_Audio.pitch = Random.Range(SoundMinPitch, SoundMaxPitch)* Time.timeScale;
		if (!m_Audio.playOnAwake)
			m_Audio.Play();

	}


	/// <summary>
	/// 
	/// </summary>
	void Update()
	{

		// the explosion should be removed as soon as the sound has
		// stopped playing. NOTE: this implementation assumes that
		// the sound is always longer in seconds than the explosion
		// effect. should be OK in most cases.
		if(!m_Audio.isPlaying)
			vp_Utility.Destroy(gameObject);

	}
	

}

	