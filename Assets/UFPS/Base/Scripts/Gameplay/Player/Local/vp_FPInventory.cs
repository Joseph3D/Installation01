/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPInventory.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	a version of vp_Inventory that's aware of the FPPlayerEventHandler
//					and uses its events
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vp_FPInventory : vp_Inventory
{

	private vp_FPPlayerEventHandler m_Player = null;	// should never be referenced directly
	protected vp_FPPlayerEventHandler Player	// lazy initialization of the event handler field
	{
		get
		{
			if (m_Player == null)
				m_Player = transform.GetComponent<vp_FPPlayerEventHandler>();
			return m_Player;
		}
	}

	private vp_FPWeaponHandler m_WeaponHandler = null;	// should never be referenced directly
	protected vp_FPWeaponHandler WeaponHandler	// lazy initialization of the weapon handler field
	{
		get
		{
			if (m_WeaponHandler == null)
				m_WeaponHandler = transform.GetComponent<vp_FPWeaponHandler>();
			return m_WeaponHandler;
		}
	}


	protected Dictionary<int, vp_ItemIdentifier> m_WeaponIdentifiers = new Dictionary<int, vp_ItemIdentifier>();
	protected vp_ItemIdentifier m_WeaponIdentifierResult;
	protected string m_MissingHandlerError = "Error (vp_FPInventory) this component must be on the same transform as a vp_FPPlayerEventHandler + vp_FPWeaponHandler.";

	public vp_ItemIdentifier CurrentWeaponIdentifier
	{
		get
		{
			if (!Application.isPlaying)
				return null;
			return GetWeaponIdentifier(WeaponHandler.CurrentWeaponIndex);
		}
	}


	////////////// 'Misc' section ////////////////
	[System.Serializable]
	public class MiscSection
	{
		public bool ResetOnRespawn = true;
	}
	[SerializeField]
	protected MiscSection m_Misc;

	
	/// <summary>
	/// 
	/// </summary>
	protected virtual vp_ItemIdentifier GetWeaponIdentifier(int index)
	{
		if (!Application.isPlaying)
			return null;
		if (!m_WeaponIdentifiers.TryGetValue(index, out m_WeaponIdentifierResult))
		{

			if ((index < 1) || index > (WeaponHandler.Weapons.Count))
				return null;

			if (WeaponHandler.Weapons[index-1] == null)
				return null;

			m_WeaponIdentifierResult = WeaponHandler.Weapons[index-1].GetComponent<vp_ItemIdentifier>();

			if (m_WeaponIdentifierResult == null)
				return null;

			if (m_WeaponIdentifierResult.Type == null)
				return null;

			m_WeaponIdentifiers.Add(index, m_WeaponIdentifierResult);

		}

		return m_WeaponIdentifierResult;
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void Awake()
	{
		base.Awake();


		// NOTE: if either handler is missing we'll dump an error to the
		// console rather than use 'RequireComponent' (auto-adding these
		// could lead to real messy player setups)
		if (Player == null || WeaponHandler == null)
			Debug.LogError(m_MissingHandlerError);

	}


	/// <summary>
	/// registers this component with the event handler (if any)
	/// </summary>
	protected override void OnEnable()
	{

		base.OnEnable();

		// allow this monobehaviour to talk to the player event handler
		if (Player != null)
			Player.Register(this);

		UnwieldMissingWeapon();

	}


	/// <summary>
	/// unregisters this component from the event handler (if any)
	/// </summary>
	protected override void OnDisable()
	{

		base.OnDisable();

		// unregister this monobehaviour from the player event handler
		if (Player != null)
			Player.Unregister(this);
		
	}

	
	/// <summary>
	/// 
	/// </summary>
	protected virtual bool MissingIdentifierError(int weaponIndex = 0)
	{
		//Debug.Log("count: " + WeaponHandler.Weapons.Count + ", index: " + weaponIndex);

		if (!Application.isPlaying)
			return false;

		string weaponName = "";
		if ((weaponIndex > 0) &&
			(WeaponHandler != null) &&
			(WeaponHandler.Weapons.Count > weaponIndex - 1))
		{
			weaponName = "'" + WeaponHandler.Weapons[weaponIndex - 1].name + "' ";
		}
		Debug.LogWarning(string.Format("Warning: ({0}{1}) Weapon gameobject " + weaponName + "lacks a properly set up vp_ItemIdentifier component!", vp_Utility.GetErrorLocation(1, true), ((weaponIndex == 0) ? "" : "(" + weaponIndex.ToString() + ")")));
		return false;
	}

	
	/// <summary>
	/// 
	/// </summary>
	protected override void DoAddItem(vp_ItemType type, int id)
	{
		bool hadItBefore = HaveItem(type, id);
		base.DoAddItem(type, id);
		if(!hadItBefore)
			TryWield(GetItem(type, id));
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void DoRemoveItem(vp_ItemInstance item)
	{

		Unwield(item);
		base.DoRemoveItem(item);

	}

	
	/// <summary>
	/// 
	/// </summary>
	protected override void DoAddUnitBank(vp_UnitBankType unitBankType, int id, int unitsLoaded)
	{
		bool hadItBefore = HaveItem(unitBankType, id);
		base.DoAddUnitBank(unitBankType, id, unitsLoaded);
		if (!hadItBefore)
			TryWield(GetItem(unitBankType, id));
	}


	/// <summary>
	/// 
	/// </summary>
	protected override void DoRemoveUnitBank(vp_UnitBankInstance bank)
	{

		Unwield(bank);
		base.DoRemoveUnitBank(bank);

	}


	/// <summary>
	/// 
	/// </summary>
	public override bool DoAddUnits(vp_UnitBankInstance bank, int amount)
	{
		bool result = base.DoAddUnits(bank, amount);

		// if units were added to the inventory (and not to a weapon)
		if ((result == true && bank.IsInternal) && !((Application.isPlaying) && WeaponHandler.CurrentWeaponIndex == 0))
		{
			// fetch the inventory record for the current weapon to see
			// if we should reload it straight away
			vp_UnitBankInstance weapon = (CurrentWeaponInstance as vp_UnitBankInstance);
			if (weapon != null)
			{
				// if the currently wielded weapon uses the same kind of units,
				// and is currently out of ammo ...
				if ((bank.UnitType == weapon.UnitType) && (weapon.Count == 0))
					Player.AutoReload.Try();	// try to auto-reload (success determined by weaponhandler)
			}
		}

		return result;
	}


	/// <summary>
	/// 
	/// </summary>
	public override bool DoRemoveUnits(vp_UnitBankInstance bank, int amount)
	{

		bool result = base.DoRemoveUnits(bank, amount);

		if (bank.Count == 0)
			vp_Timer.In(0.3f, delegate() { Player.AutoReload.Try(); });		// try to auto-reload (success determined by weaponhandler)

		return result;
	}


	/// <summary>
	/// unwields the currently wielded weapon if it's not present
	/// in the inventory
	/// </summary>
	protected virtual void UnwieldMissingWeapon()
	{

		if (!Application.isPlaying)
			return;

		if (WeaponHandler.CurrentWeaponIndex < 1)
			return;

		if ((CurrentWeaponIdentifier != null) &&
			HaveItem(CurrentWeaponIdentifier.Type, CurrentWeaponIdentifier.ID))
			return;

		if (CurrentWeaponIdentifier == null)
			MissingIdentifierError(WeaponHandler.CurrentWeaponIndex);

		Player.SetWeapon.TryStart(0);

	}


	/// <summary>
	/// wields the vp_FPWeapon mapped to 'item' (if any)
	/// </summary>
	protected virtual void TryWield(vp_ItemInstance item)
	{

		if (!Application.isPlaying)
			return;

		if (Player.Dead.Active)
			return;

		int index;
		vp_ItemIdentifier identifier;
		for (index = 1; index < WeaponHandler.Weapons.Count+1; index++)
		{

			identifier = GetWeaponIdentifier(index);

			if (identifier == null)
				continue;

			if (item.Type != identifier.Type)
				continue;

			if (identifier.ID == 0)
				goto found;

			if (item.ID != identifier.ID)
				continue;

			goto found;

		}

		return;

		found:

		Player.SetWeapon.TryStart(index);

	}
	

	/// <summary>
	/// if 'item' is a currently wielded weapon, unwields it
	/// </summary>
	protected virtual void Unwield(vp_ItemInstance item)
	{

		if (!Application.isPlaying)
			return;

		if (WeaponHandler.CurrentWeaponIndex == 0)
			return;

		if (CurrentWeaponIdentifier == null)
		{
			MissingIdentifierError();
			return;
		}

		if (item.Type != CurrentWeaponIdentifier.Type)
			return;

		if ((CurrentWeaponIdentifier.ID != 0) && (item.ID != CurrentWeaponIdentifier.ID))
			return;

		Player.SetWeapon.TryStart(0);
		vp_Timer.In(0.35f, delegate() { Player.SetNextWeapon.Try(); });

		vp_Timer.In(1.0f, UnwieldMissingWeapon);

	}


	/// <summary>
	/// 
	/// </summary>
	public override void Refresh()
	{
		base.Refresh();

		UnwieldMissingWeapon();
	}


	/// <summary>
	/// returns true if the inventory contains a weapon by the
	/// index fed as an argument to the 'SetWeapon' activity.
	/// false if not. this is used to regulate which weapons the
	/// player currently has access to.
	/// </summary>
	protected virtual bool CanStart_SetWeapon()
	{

		int index = (int)Player.SetWeapon.Argument;
		if (index == 0)
			return true;

		vp_ItemIdentifier weapon = GetWeaponIdentifier(index);
		if (weapon == null)
		{
			if((index < 1) || index > (WeaponHandler.Weapons.Count))
				return false;
			return MissingIdentifierError(index);
		}

		return HaveItem(weapon.Type, weapon.ID);

	}


	/// <summary>
	/// tries to remove one unit from ammo level of current weapon
	/// </summary>
	protected virtual bool OnAttempt_DepleteAmmo()
	{

		if (CurrentWeaponIdentifier == null)
			return MissingIdentifierError();

		return TryDeduct(CurrentWeaponIdentifier.Type as vp_UnitBankType, CurrentWeaponIdentifier.ID, 1);

	}


	/// <summary>
	/// tries to reload current weapon with any compatible units left
	/// in the inventory.
	/// </summary>
	protected virtual bool OnAttempt_RefillCurrentWeapon()
	{

		if (CurrentWeaponIdentifier == null)
			return MissingIdentifierError();

		return TryReload(CurrentWeaponIdentifier.Type as vp_UnitBankType, CurrentWeaponIdentifier.ID);

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual vp_ItemInstance CurrentWeaponInstance
	{
		get
		{
			if (Application.isPlaying && (WeaponHandler.CurrentWeaponIndex == 0))
				return null;
			if (CurrentWeaponIdentifier == null)
			{
				MissingIdentifierError();
				return null;
			}
			return GetItem(CurrentWeaponIdentifier.Type, CurrentWeaponIdentifier.ID);
		}
	}


	/// <summary>
	/// 
	/// </summary>
	public override void Reset()
	{

		if (!m_Misc.ResetOnRespawn)
			return;

		base.Reset();

	}


	/// <summary>
	/// gets or sets the current weapon's ammo count
	/// </summary>
	protected virtual int OnValue_CurrentWeaponAmmoCount
	{
		get
		{
			vp_UnitBankInstance weapon = CurrentWeaponInstance as vp_UnitBankInstance;
			if (weapon == null)
				return 0;
			return weapon.Count;
		}
		set
		{
			vp_UnitBankInstance weapon = CurrentWeaponInstance as vp_UnitBankInstance;
			if (weapon == null)
				return;
			weapon.TryGiveUnits(value);
		}
	}


	/// <summary>
	/// gets or sets the current weapon's ammo count
	/// </summary>
	protected virtual int OnValue_CurrentWeaponMaxAmmoCount
	{
		get
		{
			vp_UnitBankInstance weapon = CurrentWeaponInstance as vp_UnitBankInstance;
			if (weapon == null)
				return 0;
			return weapon.Capacity;
		}
	}


	/// <summary>
	/// returns the amount of bullets for the current weapon
	/// that is currently available in an internal unit bank
	/// </summary>
	protected virtual int OnValue_CurrentWeaponClipCount
	{
		get
		{

			vp_UnitBankInstance weapon = CurrentWeaponInstance as vp_UnitBankInstance;
			if (weapon == null)
				return 0;

			return GetUnitCount(weapon.UnitType);

		}

	}


	/// <summary>
	/// returns the amount of items or units in the inventory by
	/// ItemType object name. WARNING: this event is potentially
	/// quite slow
	/// </summary>
	protected virtual int OnMessage_GetItemCount(string itemTypeObjectName)
	{

		vp_ItemInstance item = GetItem(name);
		if (item == null)
			return 0;

		// if item is an internal unitbank, return its unit count
		vp_UnitBankInstance unitBank = (item as vp_UnitBankInstance);
		if ((unitBank != null) && (unitBank.IsInternal))
			return GetItemCount(unitBank.UnitType);

		// if it's a regular item or unitbank, return the amount
		// of similar instances
		return GetItemCount(item.Type);

	}


	/// <summary>
	/// tries to add an amount of items to the item count.
	/// NOTE: this event should be passed an object array where
	/// the first object is of type 'vp_ItemType', and the second
	/// (optional) object is of type 'int', representing the amount
	/// of items to add
	/// </summary>
	protected virtual bool OnAttempt_AddItem(object args)
	{

		object[] arr = (object[])args;

		// fail if item type is unknown
		vp_ItemType type = arr[0] as vp_ItemType;
		if (type == null)
			return false;

		int amount = (arr.Length == 2) ? (int)arr[1] : 1;

		return TryGiveItems(type, amount);

	}


	/// <summary>
	/// tries to remove an amount of items to the item count.
	/// NOTE: this event should be passed an object array where
	/// the first object is of type 'vp_ItemType', and the second
	/// (optional) object is of type 'int', representing the amount
	/// of items to remove
	/// </summary>
	protected virtual bool OnAttempt_RemoveItem(object args)
	{

		object[] arr = (object[])args;

		// fail if item type is unknown
		vp_ItemType type = arr[0] as vp_ItemType;
		if (type == null)
			return false;

		int amount = (arr.Length == 2) ? (int)arr[1] : 1;

		return TryRemoveItems(type, amount);

	}


}