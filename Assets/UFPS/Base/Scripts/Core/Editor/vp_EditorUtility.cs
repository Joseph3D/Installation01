/////////////////////////////////////////////////////////////////////////////////
//
//	vp_EditorUtility.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	misc editor utility methods
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public static class vp_EditorUtility
{


	/// <summary>
	/// 
	/// </summary>
	public static Object CreateAsset(string path, System.Type type)
	{

		if (!System.IO.Directory.Exists("Assets/" + path))
		{
			path = "UFPS";
			if (!System.IO.Directory.Exists("Assets/UFPS"))
				System.IO.Directory.CreateDirectory("Assets/UFPS");
		}

		string fileName = path + "/" + "New " + ((type.Name.StartsWith("vp_") ? type.Name.Substring(3) : type.Name));

		fileName = NewValidFileName(fileName);

		Object asset = ScriptableObject.CreateInstance(type);

		AssetDatabase.CreateAsset(asset, "Assets/" + fileName + ".asset");
		if (asset != null)
		{
			EditorGUIUtility.PingObject(asset);
			Selection.activeObject = asset;
		}

		return asset;
	}


	/// <summary>
	/// creates a new filename from a base filename by appending
	/// the next number that will result in a non-existing filename
	/// </summary>
	public static string NewValidFileName(string baseFileName)
	{

		string fileName = baseFileName;

		int n = 1;
		FileInfo fileInfo = null;
		fileInfo = new FileInfo("Assets/" + fileName + ".asset");
		while ((fileInfo.Exists))
		{
			n++;
			fileName = fileName.Substring(0, baseFileName.Length) + " " + n.ToString();
			fileInfo = new FileInfo("Assets/" + fileName + ".asset");
		}

		return fileName;

	}


}