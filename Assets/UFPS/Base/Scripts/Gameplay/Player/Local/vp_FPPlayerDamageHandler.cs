/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPPlayerDamageHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	a version of the vp_PlayerDamageHandler class extended for use
//					with the local player (vp_FPPlayerEventHandler) via which it
//					talks to the player HUD, weapon handler, controller and camera
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(vp_FPPlayerEventHandler))]

public class vp_FPPlayerDamageHandler : vp_PlayerDamageHandler 
{
	
	private vp_FPPlayerEventHandler m_FPPlayer = null;	// should never be referenced directly
	protected vp_FPPlayerEventHandler FPPlayer	// lazy initialization of the event handler field
	{
		get
		{
			if(m_FPPlayer == null)
				m_FPPlayer = transform.GetComponent<vp_FPPlayerEventHandler>();
			return m_FPPlayer;
		}
	}


	/// <summary>
	/// registers this component with the event handler (if any)
	/// </summary>
	protected override void OnEnable()
	{

		if (FPPlayer != null)
			FPPlayer.Register(this);

	}


	/// <summary>
	/// unregisters this component from the event handler (if any)
	/// </summary>
	protected override void OnDisable()
	{

		if (FPPlayer != null)
			FPPlayer.Unregister(this);

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void Update()
	{

		// fade timescale back to normal if dead during slomo (this needs
		// to be iterated every frame which is why it's in Update)
		// NOTE: remember that slow motion only works for single player
		if (FPPlayer.Dead.Active && Time.timeScale < 1.0f)
			vp_TimeUtility.FadeTimeScale(1.0f, 0.05f);

	}


	/// <summary>
	/// applies damage to the player and sends a message to the
	/// HUD that a damage flash should be played
	/// </summary>
	public override void Damage(float damage)
	{

		if (!enabled)
			return;

		if (!vp_Utility.IsActive(gameObject))
			return;

		base.Damage(damage);

		FPPlayer.HUDDamageFlash.Send(damage);

	}


	/// <summary>
	/// instantiates the player's death effect, clears the current
	/// weapon, activates the 'Dead' activity and prevents gameplay
	/// input
	/// </summary>
	public override void Die()
	{

		base.Die();

		if (!enabled || !vp_Utility.IsActive(gameObject))
			return;

		FPPlayer.AllowGameplayInput.Set(false);

	}


	/// <summary>
	/// restores gameplay input and HUD color. this gets called
	/// in response to respawning
	/// </summary>
	protected override void Reset()
	{

		base.Reset();

		if (!Application.isPlaying)
			return;

		FPPlayer.AllowGameplayInput.Set(true);
		FPPlayer.HUDDamageFlash.Send(0.0f);

	}

		
}

