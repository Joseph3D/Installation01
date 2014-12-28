/////////////////////////////////////////////////////////////////////////////////
//
//	vp_DamageInfo.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	carries information about a single instance of damage done,
//					typically to a vp_DamageHandler-derived component. this class
//					is a long term work in progress
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class vp_DamageInfo
{
	public float Damage;				// how much damage was done?
	public Transform Source;			// from what object did it come (directly)? intended use: HUD / GUI
	public Transform OriginalSource;	// what object initially caused this to happen? intended use: game logic, score


	/// <summary>
	/// 
	/// </summary>
	public vp_DamageInfo(float damage, Transform source)
	{
		Damage = damage;
		Source = source;
		OriginalSource = source;
	}


	/// <summary>
	/// 
	/// </summary>
	public vp_DamageInfo(float damage, Transform source, Transform originalSource)
	{
		Damage = damage;
		Source = source;
		OriginalSource = originalSource;
	}

}

