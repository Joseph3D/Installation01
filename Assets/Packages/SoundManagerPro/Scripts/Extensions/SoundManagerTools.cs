using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public static class SoundManagerTools {
	static readonly System.Random random = new System.Random();
	public static void Shuffle<T> ( ref List<T> theList )
	{
		int n = theList.Count;
		while (n > 1)
		{
			n--;
			int k = random.Next(n + 1);
			T val = theList[k];
			theList[k] = theList[n];
			theList[n] = val;
		}
	}
	
	public static void make2D ( ref AudioSource theAudioSource )
	{
		theAudioSource.panLevel = 0f;
	}
	
	public static void make3D ( ref AudioSource theAudioSource )
	{
		theAudioSource.panLevel = 1f;
	}
	
	/// <summary>
	/// Returns all instance fields on an object, including inherited fields
	/// http://stackoverflow.com/a/1155549/154165
	/// </summary>
	public static FieldInfo[] GetAllFieldInfos(this Type type)
	{
		if(type == null)
			return new FieldInfo[0];

		BindingFlags flags = 
			BindingFlags.Public | 
			BindingFlags.NonPublic | 
			BindingFlags.Instance | 
			BindingFlags.DeclaredOnly;

		return type.GetFields(flags)
			.Concat(GetAllFieldInfos(type.BaseType))
			.Where(f => !f.IsDefined(typeof(HideInInspector), true))
			.ToArray();
	}
}
