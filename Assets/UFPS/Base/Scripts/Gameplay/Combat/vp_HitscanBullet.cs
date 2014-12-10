/////////////////////////////////////////////////////////////////////////////////
//
//	vp_HitscanBullet.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	a script for hitscan projectiles. this script should be
//					attached to a gameobject with a mesh to be used as the impact
//					decal (bullet hole)
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(AudioSource))]

public class vp_HitscanBullet : MonoBehaviour
{

	public bool IgnoreLocalPlayer = true;

	// gameplay
	public float Range = 100.0f;			// max travel distance of this type of bullet in meters
	public float Force = 100.0f;			// force applied to any rigidbody hit by the bullet
	public float Damage = 1.0f;				// the damage transmitted to target by the bullet
	public string DamageMethodName = "Damage";	// user defined name of damage method on target
												// TIP: this can be used to apply different types of damage, i.e
												// magical, freezing, poison, electric
	protected Transform m_Sender = null;		// inflictor / source of the damage

	public float m_SparkFactor = 0.5f;		// chance of bullet impact generating a spark

	// these gameobjects will all be spawned at the point and moment
	// of impact. technically they could be anything, but their
	// intended uses are as follows:
	public GameObject m_ImpactPrefab = null;	// a flash or burst illustrating the shock of impact
	public GameObject m_DustPrefab = null;		// evaporating dust / moisture from the hit material
	public GameObject m_SparkPrefab = null;		// a quick spark, as if hitting stone or metal
	public GameObject m_DebrisPrefab = null;	// pieces of material thrust out of the bullet hole and / or falling to the ground

	// sound
	protected AudioSource m_Audio = null;
	public List<AudioClip> m_ImpactSounds = new List<AudioClip>();	// list of impact sounds to be randomly played
	public Vector2 SoundImpactPitch = new Vector2(1.0f, 1.5f);	// random pitch range for impact sounds

	public int [] NoDecalOnTheseLayers;
	
	protected Transform m_Transform = null;
	protected Renderer m_Renderer = null;
	protected bool m_Initialized = false;

	void Awake()
	{
	
		m_Transform = transform;
		m_Renderer = renderer;
		m_Audio = audio;
	
	}
	
	
	void Start()
	{
	
		m_Initialized = true;
		DoHit();
	
	}
	
	
	void OnEnable()
	{
	
		if(!m_Initialized)
			return;
	
		DoHit();
	
	}


	/// <summary>
	/// everything happens in the DoHit method. the script that
	/// spawns the bullet is responsible for setting its position 
	/// and angle. after being instantiated, the bullet immediately
	/// raycasts ahead for its full range, then snaps itself to
	/// the surface of the first object hit. it then spawns a
	/// number of particle effects and plays a random impact sound.
	/// </summary>
	void DoHit()
	{

		Ray ray = new Ray(m_Transform.position, m_Transform.forward);
		RaycastHit hit;

		// raycast against all big, solid objects except the player itself
		// SNIPPET: using this instead may be useful in cases where bullets
		// fail to hit colliders (however likely at a performance cost)
		//if (Physics.Linecast(m_Transform.position, m_Transform.position + (m_Transform.forward * Range), out hit, (IgnoreLocalPlayer ? vp_Layer.Mask.BulletBlockers : vp_Layer.Mask.IgnoreWalkThru)))

		if (Physics.Raycast(ray, out hit, Range, (IgnoreLocalPlayer ? vp_Layer.Mask.BulletBlockers : vp_Layer.Mask.IgnoreWalkThru)))
			{

			// NOTE: we can't bail out of this if-statement based on !collider.isTrigger,
			// because that would make bullets _disappear_ if they hit a trigger. to make a
			// trigger not interfere with bullets, put it in the layer: 'vp_Layer.Trigger'
			// (default: 27)

			// move this gameobject instance to the hit object
			Vector3 scale = m_Transform.localScale;	// save scale to handle scaled parent objects
			m_Transform.parent = hit.transform;
			m_Transform.localPosition = hit.transform.InverseTransformPoint(hit.point);
			m_Transform.rotation = Quaternion.LookRotation(hit.normal);					// face away from hit surface
			if (hit.transform.lossyScale == Vector3.one)								// if hit object has normal scale
				m_Transform.Rotate(Vector3.forward, Random.Range(0, 360), Space.Self);	// spin randomly
			else
			{
				// rotated child objects will get skewed if the parent object has been
				// unevenly scaled in the editor, so on scaled objects we don't support
				// spin, and we need to unparent, rescale and reparent the decal.
				m_Transform.parent = null;
				m_Transform.localScale = scale;
				m_Transform.parent = hit.transform;
			}
			
			// if hit object has physics, add the bullet force to it
			Rigidbody body = hit.collider.attachedRigidbody;
			if (body != null && !body.isKinematic)
				body.AddForceAtPosition(((ray.direction * Force) / Time.timeScale) / vp_TimeUtility.AdjustedTimeScale, hit.point);

			// spawn impact effect
			if (m_ImpactPrefab != null)
				vp_Utility.Instantiate(m_ImpactPrefab, m_Transform.position, m_Transform.rotation);

			// spawn dust effect
			if (m_DustPrefab != null)
				vp_Utility.Instantiate(m_DustPrefab, m_Transform.position, m_Transform.rotation);

			// spawn spark effect
			if (m_SparkPrefab != null)
			{
				if (Random.value < m_SparkFactor)
					vp_Utility.Instantiate(m_SparkPrefab, m_Transform.position, m_Transform.rotation);
			}

			// spawn debris particle fx
			if (m_DebrisPrefab != null)
				vp_Utility.Instantiate(m_DebrisPrefab, m_Transform.position, m_Transform.rotation);

			// play impact sound
			if (m_ImpactSounds.Count > 0)
			{
				m_Audio.pitch = Random.Range(SoundImpactPitch.x, SoundImpactPitch.y) * Time.timeScale;
				m_Audio.clip = m_ImpactSounds[(int)Random.Range(0, (m_ImpactSounds.Count))];
				m_Audio.Stop();
				m_Audio.Play();
			}

			// do damage on the target
			if(m_Sender != null)
				hit.collider.SendMessageUpwards(DamageMethodName, new vp_DamageInfo(Damage, m_Sender), SendMessageOptions.DontRequireReceiver);
			else
				hit.collider.SendMessageUpwards(DamageMethodName, Damage, SendMessageOptions.DontRequireReceiver);

			// prevent adding decals to objects based on layer
			if (NoDecalOnTheseLayers.Length > 0)
			{
				foreach (int layer in NoDecalOnTheseLayers)
				{

					if (hit.transform.gameObject.layer != layer)
						continue;

					m_Renderer.enabled = false;
					TryDestroy();
					return;

				}
			}

			// if bullet is visible (i.e. has a decal), queue it for deletion later
			if(m_Renderer != null)
				vp_DecalManager.Add(gameObject);
			else
				vp_Timer.In(1, TryDestroy);		// we have no renderer, so destroy object in 1 sec

		}
		else
			vp_Utility.Destroy(gameObject);	// hit nothing, so self destruct immediately

	}


	/// <summary>
	/// 
	/// </summary>
	public void SetSender(Transform sender)
	{
		m_Sender = sender;
		if (sender.transform.root == Camera.main.transform.root)
			IgnoreLocalPlayer = true;
	}


	/// <summary>
	/// sees if the impact sound is still playing and, if not,
	/// destroys the object. otherwise tries again in 1 sec
	/// </summary>
	private void TryDestroy()
	{

		if (this == null)
			return;

		if (!m_Audio.isPlaying)
		{
			m_Renderer.enabled = true;
			vp_Utility.Destroy(gameObject);
		}
		else
			vp_Timer.In(1, TryDestroy);

	}


}

