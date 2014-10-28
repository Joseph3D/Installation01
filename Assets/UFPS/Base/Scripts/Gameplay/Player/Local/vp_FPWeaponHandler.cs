/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPWeaponHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	toggles between weapons and manipulates weapon states depending
//					on currentplayer events and activities. this component requires
//					a player event handler and atleast one child gameobject with a
//					vp_FPWeapon component
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


public class vp_FPWeaponHandler : MonoBehaviour
{

	public int StartWeapon = 0;

	// weapon timing
	public float AttackStateDisableDelay = 0.5f;		// delay until weapon attack state is disabled after firing ends
	public float SetWeaponRefreshStatesDelay = 0.5f;	// delay until component states are refreshed after setting a new weapon
	public float SetWeaponDuration = 0.1f;				// amount of time between previous weapon disappearing and next weapon appearing

	// forced pauses in player activity
	public float SetWeaponReloadSleepDuration = 0.3f;	// amount of time to prohibit reloading during set weapon
	public float SetWeaponZoomSleepDuration = 0.3f;		// amount of time to prohibit zooming during set weapon
	public float SetWeaponAttackSleepDuration = 0.3f;	// amount of time to prohibit attacking during set weapon
	public float ReloadAttackSleepDuration = 0.3f;		// amount of time to prohibit attacking during reloading

	// reloading
	public bool ReloadAutomatically = true;

	protected vp_FPPlayerEventHandler m_Player = null;
	protected List<vp_FPWeapon> m_Weapons = new List<vp_FPWeapon>();
	public List<vp_FPWeapon> Weapons { get { return m_Weapons; } }

	protected int m_CurrentWeaponIndex = -1;
	protected vp_FPWeapon m_CurrentWeapon = null;
	public vp_FPWeapon CurrentWeapon { get { return m_CurrentWeapon; } }

	// timers
	protected vp_Timer.Handle m_SetWeaponTimer = new vp_Timer.Handle();
	protected vp_Timer.Handle m_SetWeaponRefreshTimer = new vp_Timer.Handle();
	protected vp_Timer.Handle m_DisableAttackStateTimer = new vp_Timer.Handle();
	protected vp_Timer.Handle m_DisableReloadStateTimer = new vp_Timer.Handle();

	/// <summary> The index of a vp_FPWeapon as stored under the vp_FPCamera in alphabetical order. Indices start at 1. </summary>
	[Obsolete("Please use the 'CurrentWeaponIndex' parameter instead.")]
	public int CurrentWeaponID { get { return m_CurrentWeaponIndex; } }	// renamed to avoid confusion with vp_ItemType.ID


	/// <summary> The index of a vp_FPWeapon as stored under the vp_FPCamera in alphabetical order. Indices start at 1. </summary>
	public int CurrentWeaponIndex { get { return m_CurrentWeaponIndex; } }

	// comparer to sort the weapons alphabetically. this is used to
	// make ingame weapon order adhere to the alphabetical order of
	// weapon objects under the FPSCamera
	protected class WeaponComparer : IComparer
	{
		int IComparer.Compare(System.Object x, System.Object y)
		{ return ((new CaseInsensitiveComparer()).Compare(((vp_FPWeapon)x).gameObject.name, ((vp_FPWeapon)y).gameObject.name)); }
	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake()
	{

		if (GetComponent<vp_FPWeapon>())
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component should sit above any vp_FPWeapons in the gameobject hierarchy.");
			return;
		}

		// store the first player event handler found in the top of our transform hierarchy
		m_Player = (vp_FPPlayerEventHandler)transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));

		// add the gameobjects of any weapon components to the weapon list
		foreach (vp_FPWeapon w in GetComponentsInChildren<vp_FPWeapon>(true))
		{
			m_Weapons.Insert(m_Weapons.Count, w);
		}

		if (m_Weapons.Count == 0)
		{
			Debug.LogError("Error: (" + this + ") Hierarchy error. This component must be added to a gameobject with vp_FPWeapon components in child gameobjects.");
			return;
		}

		// sort the weapons alphabetically
		IComparer comparer = new WeaponComparer();
		m_Weapons.Sort(comparer.Compare);

		StartWeapon = Mathf.Clamp(StartWeapon, 0, m_Weapons.Count);

	}


	/// <summary>
	/// registers this component with the event handler (if any).
	/// also, sets any weapon that may have been active on this
	/// component the last time it was disabled
	/// </summary>
	protected virtual void OnEnable()
	{

		// allow this monobehaviour to talk to the player event handler
		if (m_Player != null)
			m_Player.Register(this);

	}


	/// <summary>
	/// unregisters this component from the event handler (if any)
	/// </summary>
	protected virtual void OnDisable()
	{

		// unregister this monobehaviour from the player event handler
		if (m_Player != null)
			m_Player.Unregister(this);

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update()
	{

		// clear and refresh weapon in first frame
		if (m_CurrentWeaponIndex == -1)
		{
			SetWeapon(0);

			// set start weapon (if specified, and if inventory allows it)
			vp_Timer.In(SetWeaponDuration + 0.1f, delegate()
			{
				if (StartWeapon != 0)
				{
					if (!m_Player.SetWeapon.TryStart(StartWeapon))
						Debug.LogWarning("Warning (" + this + ") Requested 'StartWeapon' (" + m_Weapons[StartWeapon-1].name + ") was denied, likely by the inventory. Make sure it's present in the inventory from the beginning.");
				}
			});

		}

	}


	/// <summary>
	/// this method will disable the currently activated weapon
	/// and activate the one with 'weaponIndex'. if index is zero,
	/// no weapon will be activated. NOTE: this method will make
	/// any old weapon instantly pop away and make the new one
	/// pop into view. for smooth transitions, please instead
	/// use the vp_FPPlayerEventHandler 'SetWeapon' event.
	/// example: m_Player.SetWeapon.TryStart(3);
	/// </summary>
	public virtual void SetWeapon(int weaponIndex)
	{

		if (m_Weapons.Count < 1)
		{
			Debug.LogError("Error: (" + this + ") Tried to set weapon with an empty weapon list.");
			return;
		}

		if (weaponIndex < 0 || weaponIndex > m_Weapons.Count)
		{
			Debug.LogError("Error: (" + this + ") Weapon list does not have a weapon with index: " + weaponIndex);
			return;
		}

		// before putting old weapon away, make sure it's in a neutral
		// state next time it is activated
		if (m_CurrentWeapon != null)
			m_CurrentWeapon.ResetState();

		// deactivate all weapons
		foreach (vp_FPWeapon weapon in m_Weapons)
		{
			weapon.ActivateGameObject(false);
		}

		// activate new weapon
		m_CurrentWeaponIndex = weaponIndex;
		m_CurrentWeapon = null;
		if (m_CurrentWeaponIndex > 0)
		{
			m_CurrentWeapon = m_Weapons[m_CurrentWeaponIndex - 1];
			if (m_CurrentWeapon != null)
				m_CurrentWeapon.ActivateGameObject(true);
		}

	}


	/// <summary>
	/// 
	/// </summary>
	public virtual void CancelTimers()
	{

		vp_Timer.CancelAll("EjectShell");
		m_DisableAttackStateTimer.Cancel();
		m_SetWeaponTimer.Cancel();
		m_SetWeaponRefreshTimer.Cancel();

	}


	/// <summary>
	/// sets layer of the weapon model, for controlling which
	/// camera the weapon is rendered by
	/// </summary>
	public virtual void SetWeaponLayer(int layer)
	{

		if (m_CurrentWeaponIndex < 1 || m_CurrentWeaponIndex > m_Weapons.Count)
			return;

		vp_Layer.Set(m_Weapons[m_CurrentWeaponIndex - 1].gameObject, layer, true);

	}


	/// <summary>
	/// this callback is triggered right after the activity in question
	/// has been approved for activation. this event usually results
	/// from player input, but may also be sent by things that give
	/// ammo to the player, e.g. weapon pickups
	/// </summary>
	protected virtual void OnStart_Reload()
	{

		// prevent attacking for a while after reloading
		m_Player.Attack.Stop(m_Player.CurrentWeaponReloadDuration.Get() + ReloadAttackSleepDuration);


	}
	

	/// <summary>
	/// this callback is triggered right after the activity in question
	/// has been approved for activation. it moves the current weapon
	/// model to its exit offset, changes the weapon model and moves
	/// the new weapon into view. this message is usually broadcast
	/// by vp_FPInput, but may also be sent by things that have given
	/// weapons to the player, e.g. weapon pickups
	/// </summary>
	protected virtual void OnStart_SetWeapon()
	{

		// abort timers that won't be needed anymore
		CancelTimers();

		// prevent these player activities during the weapon switch
		m_Player.Reload.Stop(SetWeaponDuration + SetWeaponReloadSleepDuration);
		m_Player.Zoom.Stop(SetWeaponDuration + SetWeaponZoomSleepDuration);
		m_Player.Attack.Stop(SetWeaponDuration + SetWeaponAttackSleepDuration);

		// instantly unwield current weapon. this moves the weapon
		// to exit offset and plays an unwield sound
		if (m_CurrentWeapon != null)
			m_CurrentWeapon.Wield(false);

		// make 'OnStop_SetWeapon' trigger in 'SetWeaponDuration' seconds
		// (it will set the new weapon and refresh component states)
		m_Player.SetWeapon.AutoDuration = SetWeaponDuration;

	}


	/// <summary>
	/// this callback is triggered when the activity in question
	/// deactivates
	/// </summary>
	protected virtual void OnStop_SetWeapon()
	{

		// fetch weapon index from when 'SetWeapon.TryStart' was called
		int weapon = (int)m_Player.SetWeapon.Argument;

		// hides the old weapon and activates the new one (at its exit offset)
		SetWeapon(weapon);

		// smoothly moves the new weapon into view and plays a wield sound
		if (m_CurrentWeapon != null)
			m_CurrentWeapon.Wield();

		// make all player components resume their states from before
		// the weapon switch
		vp_Timer.In(SetWeaponRefreshStatesDelay, delegate()
		{
			m_Player.RefreshActivityStates();

			if (m_CurrentWeapon != null)
			{
				if (m_Player.CurrentWeaponAmmoCount.Get() == 0)
				{
					// the weapon came empty, but if we have ammo clips for it,
					// try reloading in 0.5 secs
					m_Player.AutoReload.Try();	// try to auto-reload
				}
			}

		}, m_SetWeaponRefreshTimer);

	}



	/// <summary>
	/// adds a condition (a rule set) that must be met for the
	/// event handler 'SetWeapon' activity to successfully activate.
	/// NOTE: other scripts may have added conditions to this
	/// activity aswell
	/// </summary>
	protected virtual bool CanStart_SetWeapon()
	{

		// fetch weapon index from when 'SetWeapon.TryStart' was called
		int weapon = (int)m_Player.SetWeapon.Argument;

		// can't set a weapon that is already set
		if (weapon == m_CurrentWeaponIndex)
			return false;

		// can't set an unexisting weapon
		if (weapon < 0 || weapon > m_Weapons.Count)
			return false;

		// can't set a new weapon while reloading
		if (m_Player.Reload.Active)
			return false;

		//// can' set a weapon that we don't have
		//if(!vp_TargetEventReturn<bool>.SendUpwards(m_Transform, "vp_Inventory"))
		//    return true;

		return true;

	}


	/// <summary>
	/// adds a condition (a rule set) that must be met for the
	/// player to be allowed to pull the trigger: that is, whether
	/// the event handler 'Attack' activity is allowed to activate.
	/// NOTES: 1) other scripts may have added conditions to this
	/// activity aswell. 2) if you are looking for firing logic:
	/// by default this is handled in the 'TryFire' method of the
	/// script 'vp_FPWeaponShooter'
	/// </summary>
	protected virtual bool CanStart_Attack()
	{

		// can't attack if there's no weapon
		if (m_CurrentWeapon == null)
			return false;

		// can't attack if we're already attacking
		if (m_Player.Attack.Active)
			return false;

		// can't attack if we're switching weapons
		if (m_Player.SetWeapon.Active)
			return false;

		// can't attack while reloading
		if (m_Player.Reload.Active)
			return false;

		// attacking is allowed
		return true;

	}


	/// <summary>
	/// this callback is triggered when the activity in question
	/// deactivates
	/// </summary>
	protected virtual void OnStop_Attack()
	{

		// the Attack activity does not automatically disable the
		// component's Attack state, so schedule disabling it in
		// 'AttackStateDisableDelay' seconds
		vp_Timer.In(AttackStateDisableDelay, delegate()
		{
			if (!m_Player.Attack.Active)
			{
				if (m_CurrentWeapon != null)
					m_CurrentWeapon.SetState("Attack", false);
			}
		}, m_DisableAttackStateTimer);

	}


	/// <summary>
	/// toggles to the previous weapon if currently allowed,
	/// otherwise attempts to skip past it
	/// </summary>
	protected virtual bool OnAttempt_SetPrevWeapon()
	{

		int i = m_CurrentWeaponIndex - 1;

		// skip past weapon '0'
		if (i < 1)
			i = m_Weapons.Count;

		int iterations = 0;
		while (!m_Player.SetWeapon.TryStart(i))
		{

			i--;
			if (i < 1)
				i = m_Weapons.Count;
			iterations++;
			if (iterations > m_Weapons.Count)
				return false;

		}

		return true;

	}


	/// <summary>
	/// toggles to the next weapon if currently allowed,
	/// otherwise attempts to skip past it
	/// </summary>
	protected virtual bool OnAttempt_SetNextWeapon()
	{

		int i = m_CurrentWeaponIndex + 1;

		int iterations = 0;
		while (!m_Player.SetWeapon.TryStart(i))
		{

			if (i > m_Weapons.Count + 1)
				i = 0;

			i++;
			iterations++;
			if (iterations > m_Weapons.Count)
				return false;
		}

		return true;

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual bool OnAttempt_SetWeaponByName(string name)
	{

		for(int v=0; v< m_Weapons.Count; v++)
		{

			if (m_Weapons[v].name == (string)name)
				return m_Player.SetWeapon.TryStart(v+1);
		}

		return false;

	}


	/// <summary>
	/// 
	/// </summary>
	//protected virtual bool OnAttempt_RemoveItem(object item)
	//{

	//    for (int v = 0; v < m_Weapons.Count; v++)
	//    {

	//        if (m_Weapons[v].name == (string)name)
	//            return m_Player.SetWeapon.TryStart(v + 1);
	//    }

	//    return false;

	//}


	/// <summary>
	/// 
	/// </summary>
	protected virtual bool OnValue_CurrentWeaponWielded
	{
		get
		{
			if (m_CurrentWeapon == null)
				return false;
			return m_CurrentWeapon.Wielded;
		}
	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual string OnValue_CurrentWeaponName
	{
		get
		{
			if (m_CurrentWeapon == null || m_Weapons == null)
				return "";
			return m_CurrentWeapon.name;

		}
	}
	

	/// <summary>
	/// 
	/// </summary>
	protected virtual int OnValue_CurrentWeaponID
	{
		get
		{
			return m_CurrentWeaponIndex;
		}
	}
	

	/// <summary>
	/// 
	/// </summary>
	protected virtual int OnValue_CurrentWeaponIndex
	{
		get
		{
			return m_CurrentWeaponIndex;
		}
	}
	

	/// <summary>
	/// 
	/// </summary>
	protected virtual bool OnAttempt_AutoReload()
	{

		if (!ReloadAutomatically)
			return false;

		return m_Player.Reload.TryStart();

	}

	/// <summary>
	/// 
	/// </summary>
	//protected virtual bool OnAttempt_DepleteAmmo()
	//{

	//    return true;
	//}


}


