using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.AI
{
	/// <summary>
	/// Movement directions
	/// </summary>
	public enum MovementDirection
	{
		Foreward = 0,
		Backward = 1,
		Left = 2,
		Right = 3,
	}


	/// <summary>
	/// Static class to get common directions in a Vector3
	/// </summary>
	public sealed class Directions
	{
		public static Vector3 Foreward;
		public static Vector3 Backward;
		public static Vector3 Left;
		public static Vector3 Right;
		
		static Directions()
		{
			Foreward = new Vector3(0,0,1);
			Backward = new Vector3(0,0,-1);
			Left     = new Vector3(-1,0,0);
			Right    = new Vector3(1,0,0);
		}
	}
}