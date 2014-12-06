/////////////////////////////////////////////////////////////////////////////////
//
//	vp_DamageHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	class for having a gameobject take damage, die and respawn.
//					any other object can do damage on this monobehaviour like so:
//					    hitObject.SendMessage(Damage, 1.0f, SendMessageOptions.DontRequireReceiver);
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;


public class vp_DamageHandler : MonoBehaviour
{

	// health and death
	public float MaxHealth = 1.0f;					// initial health of the object instance, to be reset on respawn
	public GameObject [] DeathSpawnObjects = null;	// gameobjects to spawn when object dies.
													// TIP: could be fx, could also be rigidbody rubble
	public float MinDeathDelay = 0.0f;				// random timespan in seconds to delay death. good for cool serial explosions
	public float MaxDeathDelay = 0.0f;
	public float CurrentHealth = 0.0f;			// current health of the object instance

	protected AudioSource m_Audio = null;
	public AudioClip DeathSound = null;				// sound to play upon death
	
	// impact damage
	public float ImpactDamageThreshold = 10;
	public float ImpactDamageMultiplier = 0.0f;

	// NOTE: these variables have been made obsolete and are now found in
	// the vp_Respawner component. there is temporary logic in this class
	// to help make the transition easier

	[HideInInspector]
	public bool Respawns = false;
	[HideInInspector]
	public float MinRespawnTime = -99999.0f;
	[HideInInspector]
	public float MaxRespawnTime = -99999.0f;
	[HideInInspector]
	public float RespawnCheckRadius = -99999.0f;
	[HideInInspector]
	public AudioClip RespawnSound = null;
	[HideInInspector]
	public GameObject DeathEffect = null;

	// NOTE: these variables are obsolete and will be removed
	protected Vector3 m_StartPosition;
	protected Quaternion m_StartRotation;

#if UNITY_EDITOR
	[vp_HelpBox(typeof(vp_DamageHandler), UnityEditor.MessageType.None, typeof(vp_DamageHandler), null, true, vp_PropertyDrawerUtility.Space.Nothing)]
	public float helpbox;
#endif



	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake()
	{
		
		m_Audio = audio;

		CurrentHealth = MaxHealth;

		// check for obsolete respawn-related parameters, create a vp_Respawner
		// component (if necessary) and disable such values on this component
		// NOTE: this check is temporary and will be removed in the future
		CheckForObsoleteParams();

	}
	

	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnEnable()
	{
	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnDisable()
	{
	}


	/// <summary>
	/// reduces current health by 'damage' points and kills the
	/// object if health runs out
	/// </summary>
	public virtual void Damage(float damage)
	{
		Damage(new vp_DamageInfo(damage, null));
	}
	public virtual void Damage(vp_DamageInfo projectileInfo)
	{

		if (!enabled)
			return;

		if (!vp_Utility.IsActive(gameObject))
			return;

		if (CurrentHealth <= 0.0f)
			return;

		CurrentHealth = Mathf.Min(CurrentHealth - projectileInfo.Damage, MaxHealth);

		if (vp_Gameplay.isMaster)
		{

			// only do this in multiplayer
			if (vp_Gameplay.isMultiplayer && (projectileInfo.Sender != null))
				vp_GlobalEvent<Transform, Transform, float>.Send("Damage", transform.root, projectileInfo.Sender, projectileInfo.Damage, vp_GlobalEventMode.REQUIRE_LISTENER);

			// do this in multiplayer and singleplayer
			if (CurrentHealth <= 0.0f)
				vp_Timer.In(UnityEngine.Random.Range(MinDeathDelay, MaxDeathDelay), delegate()
				{
					SendMessage("Die");		// picked up by vp_DamageHandlers and vp_Respawners
				});

		}


		// TIP: if you want to do things like play a special impact
		// sound upon every hit (but only if the object survives)
		// this is the place

	}


	/// <summary>
	/// removes the object, plays the death effect and schedules
	/// a respawn if enabled, otherwise destroys the object
	/// </summary>
	public virtual void Die()
	{

		if (!enabled || !vp_Utility.IsActive(gameObject))
			return;

		if (m_Audio != null)
		{
			m_Audio.pitch = Time.timeScale;
			m_Audio.PlayOneShot(DeathSound);
		}

		RemoveBulletHoles();
		
		vp_Utility.Activate(gameObject, false);

		foreach (GameObject o in DeathSpawnObjects)
		{
			if(o != null)
				vp_Utility.Instantiate(o, transform.position, transform.rotation);
		}
		
	}


	/// <summary>
	/// resets health, position, angle and motion
	/// </summary>
	protected virtual void Reset()
	{

		CurrentHealth = MaxHealth;
	
	}
	

	/// <summary>
	/// removes any bullet decals currently childed to this object
	/// </summary>
	protected virtual void RemoveBulletHoles()
	{

		vp_HitscanBullet[] bullets = GetComponentsInChildren<vp_HitscanBullet>(true);
		for(int i=0;i<bullets.Length; i++)
			vp_Utility.Destroy(bullets[i].gameObject);

	}


	/// <summary>
	/// 
	/// </summary>
	void OnCollisionEnter(Collision collision)
	{

		float force = collision.relativeVelocity.sqrMagnitude * 0.1f;

		float damage = (force > ImpactDamageThreshold) ? (force * ImpactDamageMultiplier) : 0.0f;

		if (damage > 0.0f)
		{
			if (CurrentHealth - damage <= 0.0f)
				MaxDeathDelay = MinDeathDelay = 0.0f;
			Damage(damage);
		}

	}


	/// <summary>
	/// Obsolete
	/// </summary>
	protected virtual void Respawn()
	{
	}


	/// <summary>
	/// Obsolete
	/// </summary>
	protected virtual void Reactivate()
	{
	}


	// -------- everything below this line is temp helper logic related to vp_Respawner transition --------
	

	/// <summary>
	/// 
	/// </summary>
	void CheckForObsoleteParams()
	{

		if (DeathEffect != null)
			Debug.LogWarning(this + "'DeathEffect' is obsolete! Please use the 'DeathSpawnObjects' array instead.");

		string parms = "";

		if (Respawns != false)
			parms += "Respawns, ";
		if (MinRespawnTime != -99999.0f)
			parms += "MinRespawnTime, ";
		if (MaxRespawnTime != -99999.0f)
			parms += "MaxRespawnTime, ";
		if (RespawnCheckRadius != -99999.0f)
			parms += "RespawnCheckRadius, ";
		if (RespawnSound != null)
			parms += "RespawnSound, ";

		if (parms != "")
		{
			parms = parms.Remove(parms.LastIndexOf(", "));
			Debug.LogWarning(string.Format("Warning + (" + this + ") The following parameters are obsolete: \"{0}\". Creating a temp vp_Respawner component. To remove this warning, see the UFPS menu -> Wizards -> Convert Old DamageHandlers.", parms));
			CreateTempRespawner();
		}

	}


	/// <summary>
	/// 
	/// </summary>
	public bool CreateTempRespawner()
	{

		if (GetComponent<vp_Respawner>() || GetComponent<vp_PlayerRespawner>())
		{
			DisableOldParams();	// we do this for the case where a prefab is updated with a vp_Respawner, but the damagehandler instance has overridden legacy params
			return false;
		}
		else
			CreateRespawnerForDamageHandler(this);
		DisableOldParams();
		return true;
	}


	/// <summary>
	/// 
	/// </summary>
	public static int GenerateRespawnersForAllDamageHandlers()
	{

		// --- update old vp_PlayerDamageHandlers to the new vp_FPPlayerDamageHandler ---

		vp_PlayerDamageHandler[] oldPlayerDamageHandlers = FindObjectsOfType(typeof(vp_PlayerDamageHandler)) as vp_PlayerDamageHandler[];
		if (oldPlayerDamageHandlers != null && oldPlayerDamageHandlers.Length > 0)
		{
			foreach (vp_PlayerDamageHandler p in oldPlayerDamageHandlers)
			{

				// if this vp_PlayerDamageHandler is on the same transform as a
				// vp_FPPlayerEventHandler we will boldly assume that it's an
				// object from UFPS 1.4.6b or older which needs to be updated.
				// if not, it might be a new and valid vp_PlayerDamageHandler
				// (added to something like a remote player) and we'll leave it

				if (p.transform.GetComponent<vp_FPPlayerEventHandler>() == null)
					continue;

				vp_FPPlayerDamageHandler n = p.gameObject.AddComponent<vp_FPPlayerDamageHandler>();

				n.AllowFallDamage = p.AllowFallDamage;
				n.DeathEffect = p.DeathEffect;
				n.DeathSound = p.DeathSound;
				n.DeathSpawnObjects = p.DeathSpawnObjects;
				n.FallImpactPitch = p.FallImpactPitch;
				n.FallImpactSounds = p.FallImpactSounds;
				n.FallImpactThreshold = p.FallImpactThreshold;
				n.ImpactDamageMultiplier = p.ImpactDamageMultiplier;
				n.ImpactDamageThreshold = p.ImpactDamageThreshold;
				n.m_Audio = p.m_Audio;
				n.CurrentHealth = p.CurrentHealth;
				n.m_StartPosition = p.m_StartPosition;
				n.m_StartRotation = p.m_StartRotation;
				n.MaxDeathDelay = p.MaxDeathDelay;
				n.MaxHealth = p.MaxHealth;
				n.MaxRespawnTime = p.MaxRespawnTime;
				n.MinDeathDelay = p.MinDeathDelay;
				n.MinRespawnTime = p.MinRespawnTime;
				n.RespawnCheckRadius = p.RespawnCheckRadius;
				n.Respawns = p.Respawns;
				n.RespawnSound = p.RespawnSound;

				DestroyImmediate(p);
			}

		}

		// --- move respawn variables of all damagehandlers to new respawner components ---

		vp_DamageHandler[] damageHandlers = FindObjectsOfType(typeof(vp_DamageHandler)) as vp_DamageHandler[];
		vp_DamageHandler[] FPPlayerDamageHandlers = FindObjectsOfType(typeof(vp_FPPlayerDamageHandler)) as vp_DamageHandler[];

		int amountOfObjectsUpdated = 0;

		foreach (vp_DamageHandler d in damageHandlers)
		{
			if (d.CreateTempRespawner())
				amountOfObjectsUpdated++;
		}

		foreach (vp_DamageHandler d in FPPlayerDamageHandlers)
		{
			if (d.CreateTempRespawner())
				amountOfObjectsUpdated++;
		}

		return amountOfObjectsUpdated;

	}


	/// <summary>
	/// 
	/// </summary>
	void DisableOldParams()
	{
		Respawns = false;
		MinRespawnTime = -99999.0f;
		MaxRespawnTime = -99999.0f;
		RespawnCheckRadius = -99999.0f;
		RespawnSound = null;
#if UNITY_EDITOR
		EditorUtility.SetDirty(this);
#endif
	}


	/// <summary>
	/// 
	/// </summary>
	static void CreateRespawnerForDamageHandler(vp_DamageHandler damageHandler)
	{

		if (damageHandler.gameObject.GetComponent<vp_Respawner>() || damageHandler.gameObject.GetComponent<vp_PlayerRespawner>())
			return;

		vp_Respawner respawner = null;

		if(damageHandler is vp_FPPlayerDamageHandler)
			respawner = damageHandler.gameObject.AddComponent<vp_PlayerRespawner>();
		else
			respawner = damageHandler.gameObject.AddComponent<vp_Respawner>();

		if (respawner == null)
			return;

		if (damageHandler.MinRespawnTime != -99999.0f)
			respawner.MinRespawnTime = damageHandler.MinRespawnTime;
		if (damageHandler.MaxRespawnTime != -99999.0f)
			respawner.MaxRespawnTime = damageHandler.MaxRespawnTime;
		if (damageHandler.RespawnCheckRadius != -99999.0f)
			respawner.ObstructionRadius = damageHandler.RespawnCheckRadius;
		if (damageHandler.RespawnSound != null)
			respawner.SpawnSound = damageHandler.RespawnSound;

	}


}

