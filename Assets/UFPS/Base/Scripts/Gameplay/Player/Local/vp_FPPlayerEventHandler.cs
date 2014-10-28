/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPPlayerEventHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	this class declares events for communication between behaviours
//					that make up a LOCAL, FIRST PERSON PLAYER.
//					
//					IMPORTANT: this class is NOT intended for use on a REMOTE player,
//					AI player etc., since such players have no access to input, camera
//					HUI, GUI or first person weapon systems. further events (dealing
//					with physics and activities that are non-exclusive to a first person
//					player) can be found in the parent class: vp_PlayerEventHandler
//
///////////////////////////////////////////////////////////////////////////////// 

using System;
using UnityEngine;

public class vp_FPPlayerEventHandler : vp_PlayerEventHandler
{

	// these declarations determine which events are supported by the
	// player event handler. it is then up to external classes to fill
	// them up with delegates for communication.

	// TIPS:
	//  1) mouse-over on the event types (e.g. vp_Message) for usage info.
	//  2) to find the places where an event is SENT, you can do 'Find All
	// References' on the event in your IDE. if this is not available, you
	// can search the project for the event name preceded by '.' (.Reload)
	//  3) to find the methods that LISTEN to an event, search the project
	// for its name preceded by '_' (_Reload)

	// gui
	public vp_Message<float> HUDDamageFlash;
	public vp_Message<string> HUDText;
	public vp_Value<Texture> Crosshair;

	// input
	public vp_Value<Vector2> InputMoveVector;
	public vp_Value<bool> AllowGameplayInput;
	public vp_Value<bool> Pause;

	// camera forces TODO: prefix "Camera_"
	public vp_Message<float> GroundStomp;
	public vp_Message<float> BombShake;
	public vp_Value<Vector3> EarthQuakeForce;
	public vp_Activity<Vector3> Earthquake;

	// first person weapon handler
	public vp_Attempt SetPrevWeapon;
	public vp_Attempt SetNextWeapon;
	public vp_Attempt<string> SetWeaponByName;
	[Obsolete("Please use the 'CurrentWeaponIndex' vp_Value instead.")]
	public vp_Value<int> CurrentWeaponID;	// renamed to avoid confusion with vp_ItemType.ID
	public vp_Value<int> CurrentWeaponIndex;
	public vp_Value<string> CurrentWeaponName;
	public vp_Value<bool> CurrentWeaponWielded;
	public vp_Attempt AutoReload;

	// fp reloader
	public vp_Value<float> CurrentWeaponReloadDuration;

	// fp interaction
	public vp_Value<vp_Interactable> Interactable;
	public vp_Value<bool> CanInteract;

	// fp inventory
		// NOTE: these events are considered 'first person' because they
		// require the weaponhandler which is a first-person-only
		// component in the current version
	public vp_Attempt RefillCurrentWeapon;
	public vp_Value<int> CurrentWeaponAmmoCount;
	public vp_Value<int> CurrentWeaponMaxAmmoCount;
	public vp_Value<int> CurrentWeaponClipCount;
	public vp_Attempt<object> AddItem;
	public vp_Attempt<object> RemoveItem;
	public vp_Attempt DepleteAmmo;

	// old inventory system
		// TIP: these events can be removed along with the old inventory system
	public vp_Value<string> CurrentWeaponClipType;
	public vp_Attempt<object> AddAmmo;
	public vp_Attempt RemoveClip;

}

