/////////////////////////////////////////////////////////////////////////////////
//
//	vp_3DUtility.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	miscellaneous 3D utility functions
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Diagnostics;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

public static class vp_3DUtility
{
	

	/// <summary>
	/// Zeroes the y property of a Vector3, for some cases where you want to
	/// make 2D physics calculations.
	/// </summary>
	public static Vector3 HorizontalVector(Vector3 value)
	{

		value.y = 0.0f;
		return value;

	}


	/// <summary>
	/// Determines whether the object of a certain renderer is visible to a
	/// certain camera and retrieves its screen position.
	/// </summary>
	public static bool OnScreen(Camera camera, Renderer renderer, Vector3 worldPosition, out Vector3 screenPosition)
	{

		screenPosition = Vector2.zero;

		if ((camera == null) || (renderer == null) || !renderer.isVisible)
			return false;

		// calculate the screen space position of the remote object
		screenPosition = camera.WorldToScreenPoint(worldPosition);

		// return false if screen position is behind camera
		if (screenPosition.z < 0.0f)
			return false;

		return true;

	}


	/// <summary>
	/// Performs a linecast from a world position to a target transform,
	/// returning true if the first hit collider is owned by that
	/// transform.
	/// </summary>
	public static bool InLineOfSight(Vector3 from, Transform target, Vector3 targetOffset, int layerMask)
	{

		RaycastHit hitInfo;
		Physics.Linecast(from, target.position + targetOffset, out hitInfo, layerMask);

		if (hitInfo.collider == null || hitInfo.collider.transform.root == target)
			return true;

		return false;

	}
	
	
	/// <summary>
	/// Determines whether the distance between two points is within
	/// a determined range and retrieves the distance.
	/// </summary>
	public static bool WithinRange(Vector3 from, Vector3 to, float range, out float distance)
	{

		distance = Vector3.Distance(from, to);

		if (distance > range)
			return false;

		return true;

	}
	

	/// <summary>
	/// returns the distance between a ray and a point
	/// </summary>
	public static float DistanceToRay(Ray ray, Vector3 point)
	{
		return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
	}


	/// <summary>
	/// returns the angle between a look vector and a target position.
	/// can be used for various aiming logic
	/// </summary>
	public static float LookAtAngle(Vector3 sourcePosition, Vector3 sourceDirection, Vector3 targetPosition)
	{

		return Mathf.Acos(Vector3.Dot((sourcePosition - targetPosition).normalized, -sourceDirection)) * Mathf.Rad2Deg;

	}


}

