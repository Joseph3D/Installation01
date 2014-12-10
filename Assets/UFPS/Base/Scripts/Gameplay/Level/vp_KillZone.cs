/////////////////////////////////////////////////////////////////////////////////
//
//	vp_KillZone.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	a trigger to kill the local player on contact (singleplayer).
//					make sure to enable 'IsTrigger' on the gameobject's collider
//					
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class vp_KillZone : MonoBehaviour
{

	float nextAllowedKillTime = 0.0f;
	
	/// <summary>
	/// 
	/// </summary>
	void OnTriggerEnter(Collider col)
	{

		if (Time.time < nextAllowedKillTime)	// TEMP: quick hack to avoid killing player on respawn
			return;

		vp_PlayerDamageHandler d = col.transform.root.GetComponentInChildren<vp_PlayerDamageHandler>();
		
		if((d != null) && d.CurrentHealth > 0)
			d.Damage(d.CurrentHealth);

		nextAllowedKillTime = Time.time + 10.0f;

	}

}