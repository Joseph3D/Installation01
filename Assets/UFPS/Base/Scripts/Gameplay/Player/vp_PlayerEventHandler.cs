/////////////////////////////////////////////////////////////////////////////////
//
//	vp_PlayerEventHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	this class declares events for communication between behaviours
//					that make up a basic player object. it also binds several object
//					component states to player activity events
//
///////////////////////////////////////////////////////////////////////////////// 

using System;
using UnityEngine;

public class vp_PlayerEventHandler : vp_StateEventHandler
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

	// basic properties
	public vp_Value<float> Health;
	public vp_Value<Vector3> Position;
	public vp_Value<Vector2> Rotation;
	public vp_Value<Vector3> Forward;
	public vp_Value<Vector3> MotorThrottle;
	public vp_Value<bool> MotorJumpDone;

	// activities
	public vp_Activity Dead;
	public vp_Activity Run;
	public vp_Activity Jump;
	public vp_Activity Crouch;
	public vp_Activity Zoom;
	public vp_Activity Attack;
	public vp_Activity Reload;
	public vp_Activity Climb;
	public vp_Activity Interact;
	public vp_Activity<int> SetWeapon;

	// inventory
	public vp_Message<string, int> GetItemCount;

	// physics
	public vp_Message<Vector3> Move;
	public vp_Value<Vector3> Velocity;
	public vp_Value<float> SlopeLimit;
	public vp_Value<float> StepOffset;
	public vp_Value<float> Radius;
	public vp_Value<float> Height;
	public vp_Value<float> FallSpeed;
	public vp_Message<float> FallImpact;
	public vp_Message<float> HeadImpact;
	public vp_Message<Vector3> ForceImpact;
	public vp_Message Stop;
	public vp_Value<Transform> Platform;

	// ground surface
	public vp_Value<Texture> GroundTexture;
	public vp_Value<vp_SurfaceIdentifier> SurfaceType;


	/// <summary>
	/// 
	/// </summary>
	protected override void Awake()
	{

		base.Awake();

		// TIP: please see the manual for the difference
		// between (player) activities and (component) states

		// --- activity state bindings ---
		// whenever these activities are toggled they will enable and
		// disable any component states with the same names. disable
		// these lines to make states independent of activities
		BindStateToActivity(Run);
		BindStateToActivity(Jump);
		BindStateToActivity(Crouch);
		BindStateToActivity(Zoom);
		BindStateToActivity(Reload);
		BindStateToActivity(Dead);
		BindStateToActivity(Climb);
		BindStateToActivityOnStart(Attack);	// <--
		// in this default setup the 'Attack' activity will enable
		// - but not disable - the component attack states when toggled.

		// --- activity AutoDurations ---
		// automatically stops an activity after a set timespan
		SetWeapon.AutoDuration = 1.0f;		// NOTE: altered at runtime by each weapon
		Reload.AutoDuration = 1.0f;			// NOTE: altered at runtime by each weapon

		// --- activity MinDurations ---
		// prevents player from aborting an activity too soon after starting
		Zoom.MinDuration = 0.2f;
		Crouch.MinDuration = 0.5f;

		// --- activity MinPauses ---
		// prevents player from restarting an activity too soon after stopping
		Jump.MinPause = 0.0f;			// increase this to enforce a certain delay between jumps
		SetWeapon.MinPause = 0.2f;

	}


}

