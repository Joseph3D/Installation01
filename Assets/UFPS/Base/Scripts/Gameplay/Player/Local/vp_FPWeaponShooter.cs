/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPWeaponShooter.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	this class adds firearm features to a vp_FPWeapon. it has all
//					the capabilities of its inherited class (vp_Shooter), adding
//					recoil, animations and an extended rule set for when it can fire
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;


[RequireComponent(typeof(vp_FPWeapon))]

public class vp_FPWeaponShooter : vp_WeaponShooter
{

	protected vp_FPWeapon m_FPWeapon = null;			// the weapon affected by the shooter
	protected vp_FPCamera m_FPCamera = null;			// the main first person camera view


	// motion
	public float MotionPositionReset = 0.5f;			// how much to reset weapon to its normal position upon firing (0-1)
	public float MotionRotationReset = 0.5f;
	public float MotionPositionPause = 1.0f;			// time interval over which to freeze and fade swaying forces back in upon firing
	public float MotionRotationPause = 1.0f;
	public float MotionRotationRecoilCameraFactor = 0.0f;
	public float MotionPositionRecoilCameraFactor = 0.0f;

	// animation
	public AnimationClip AnimationFire = null;

	// event handler property cast as an FPPlayerEventHandler
	protected vp_FPPlayerEventHandler Player
	{
		get
		{
			if (m_Player == null)
			{
				if (EventHandler != null)
					m_Player = (vp_FPPlayerEventHandler)EventHandler;
			}
			return (vp_FPPlayerEventHandler)m_Player;
		}
	}

	public new vp_FPWeapon Weapon
	{
		get
		{
			if (m_FPWeapon == null)
				m_FPWeapon = transform.GetComponent<vp_FPWeapon>();
			return m_FPWeapon;
		}
	}

	/// <summary>
	/// 
	/// </summary>
	protected override void Awake()
	{

		base.Awake();

		m_FPCamera = transform.root.GetComponentInChildren<vp_FPCamera>();

		if (m_ProjectileSpawnPoint == null)
			m_ProjectileSpawnPoint = m_FPCamera.gameObject;

		m_ProjectileDefaultSpawnpoint = m_ProjectileSpawnPoint;

		// reset the next allowed fire time
		m_NextAllowedFireTime = Time.time;

		ProjectileSpawnDelay = Mathf.Min(ProjectileSpawnDelay, (ProjectileFiringRate - 0.1f));
		
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void OnEnable()
	{
		RefreshFirePoint();
		base.OnEnable();
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void OnDisable()
	{
		base.OnDisable();
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void Start()
	{

		base.Start();

		// defaults for using animation length as the fire and reload delay
		if (ProjectileFiringRate == 0.0f && AnimationFire != null)
			ProjectileFiringRate = AnimationFire.length;

		// defaults for using animation length as the fire delay
		if (ProjectileFiringRate == 0.0f && AnimationFire != null)
			ProjectileFiringRate = AnimationFire.length;
		
	}

	
	/// <summary>
	/// in addition to spawning the projectile in the base class,
	/// plays a fire animation on the weapon and applies recoil
	/// to the weapon spring. also regulates tap fire
	/// </summary>
	protected override void Fire()
	{

		m_LastFireTime = Time.time;

		// play fire animation
		if (AnimationFire != null)
		{
			if (Weapon.WeaponModel.animation[AnimationFire.name] == null)
				Debug.LogError("Error (" + this + ") No animation named '" + AnimationFire.name + "' is listed in this prefab. Make sure the prefab has an 'Animation' component which references all the clips you wish to play on the weapon.");
			else
			{
				Weapon.WeaponModel.animation[AnimationFire.name].time = 0.0f;
				Weapon.WeaponModel.animation.Sample();
				Weapon.WeaponModel.animation.Play(AnimationFire.name);
			}
		}

		// apply recoil
		if (MotionRecoilDelay == 0.0f)
			ApplyRecoil();
		else
			vp_Timer.In(MotionRecoilDelay, ApplyRecoil);

		base.Fire();
		
	}


	/// <summary>
	/// applies some advanced recoil motions on the weapon when fired
	/// </summary>
	protected override void ApplyRecoil()
	{

		// return the weapon to its forward looking state by certain
		// position, rotation and velocity factors
		Weapon.ResetSprings(MotionPositionReset, MotionRotationReset,
							MotionPositionPause, MotionRotationPause);

		// add a positional and angular force to the weapon for one frame
		if (MotionRotationRecoil.z == 0.0f)
		{
			Weapon.AddForce2(MotionPositionRecoil, MotionRotationRecoil);

			// if we have positional camera recoil factor, also shake the camera
			if(MotionPositionRecoilCameraFactor != 0.0f )
				m_FPCamera.AddForce2(MotionPositionRecoil * MotionPositionRecoilCameraFactor);
		}
		else
		{

			// if we have rotation recoil around the z vector, also do dead zone logic
			Weapon.AddForce2(MotionPositionRecoil,
				Vector3.Scale(MotionRotationRecoil, (Vector3.one + Vector3.back)) +	// recoil around x & y
				(((Random.value < 0.5f) ? Vector3.forward : Vector3.back) *	// spin direction (left / right around z)
				Random.Range(MotionRotationRecoil.z * MotionRotationRecoilDeadZone,
												MotionRotationRecoil.z)));		// spin force

			// if we have positional camera recoil factor, also shake the camera
			if (MotionPositionRecoilCameraFactor != 0.0f)
				m_FPCamera.AddForce2(MotionPositionRecoil * MotionPositionRecoilCameraFactor);
			
			// if we have angular camera recoil factor, also twist the camera left / right
			if(MotionRotationRecoilCameraFactor!= 0.0f)
				m_FPCamera.AddRollForce((Random.Range(MotionRotationRecoil.z * MotionRotationRecoilDeadZone, MotionRotationRecoil.z)	// dead zone
												* MotionRotationRecoilCameraFactor) *	// camera rotation factor
												((Random.value < 0.5f) ? 1.0f : -1.0f));		// direction

		}
				

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnMessage_CameraToggle3rdPerson()
	{
		RefreshFirePoint();
	}


	/// <summary>
	/// 
	/// </summary>
	void RefreshFirePoint()
	{


		if (Player.IsFirstPerson == null)
			return;

		// --- 1st PERSON ---
		if (Player.IsFirstPerson.Get())
		{
			m_ProjectileSpawnPoint = m_FPCamera.gameObject;
			if (MuzzleFlash != null)
				MuzzleFlash.layer = vp_Layer.Weapon;
			m_MuzzleFlashSpawnPoint = null;
			m_ShellEjectSpawnPoint = null;
			Refresh();
		}

		// --- 3rd PERSON ---
		else
		{
			m_ProjectileSpawnPoint = m_ProjectileDefaultSpawnpoint;
			if (MuzzleFlash != null)
				MuzzleFlash.layer = vp_Layer.Default;
			m_MuzzleFlashSpawnPoint = null;
			m_ShellEjectSpawnPoint = null;
			Refresh();
		}

		if (Player.CurrentWeaponName.Get() != name)
			m_ProjectileSpawnPoint = m_FPCamera.gameObject;

	}

}

