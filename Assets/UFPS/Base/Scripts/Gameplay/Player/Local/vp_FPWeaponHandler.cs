/////////////////////////////////////////////////////////////////////////////////
//
//	vp_FPWeaponHandler.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;


public class vp_FPWeaponHandler : vp_WeaponHandler
{
	
	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnMessage_CameraToggle3rdPerson()
	{

		m_Player.IsFirstPerson.Set(!m_Player.IsFirstPerson.Get());

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



}


