/////////////////////////////////////////////////////////////////////////////////
//
//	vp_DamageInfo.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

public class vp_DamageInfo
{
	public float Damage;
	public Transform Sender;
	public vp_DamageInfo(float damage, Transform sender)
	{
		Damage = damage;
		Sender = sender;
	}
}

