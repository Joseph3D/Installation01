/////////////////////////////////////////////////////////////////////////////////
//
//	vp_RemotePlayerWizard.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	launched from the main UFPS editor 'Wizards' menu, this
//					script creates a copy of the currently selected UFPS player
//					gameobject - stripped of all its 1st person functionality.
//					the new object can be used for AI or multiplayer remote
//					players. NOTE: Only default UFPS classes will be processed.
//
/////////////////////////////////////////////////////////////////////////////////
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;


public class vp_RemotePlayerWizard
{
	
	
	/// <summary>
	/// creates a copy of the currently selected UFPS player gameobject
	/// - stripped of all its 1st person functionality. the new object
	/// can be used for AI or multiplayer remote players
	/// </summary>
	public static void Generate()
	{

		GameObject target = Selection.activeObject as GameObject;

		if ((target == null) || (target.GetComponentInChildren<vp_FPCamera>() == null) && (target.GetComponentInChildren<vp_FPCamera>() == null))
		{
			EditorUtility.DisplayDialog("Failed to run wizard", "Please select the main gameobject of a 1st person player (make sure it's active) and try again.", "OK");
			return;
		}

		if (!EditorUtility.DisplayDialog("Generate Remote Player?", "This wizard will create a copy of the selected player object - stripped of all its 1st person functionality. This new object can be used for AI or multiplayer remote players.\n\nNOTE: Only default UFPS classes will be processed.", "OK", "Cancel"))
			return;

		string name = target.name + "(Remote)";

		// generate - and operate upon - a copy of the target
		target = (GameObject)GameObject.Instantiate(target);
		target.name = target.name.Replace("(Clone)", "");
		target.name = target.name + " (CONVERSION FAILED - see error log)";

		// convert weapons
		ConvertWeaponsTo3rdPerson(target);

		// convert weapon handler
		vp_FPWeaponHandler fpWHandler = target.GetComponent<vp_FPWeaponHandler>();
		if (fpWHandler != null)
		{
			vp_WeaponHandler wHandler = target.AddComponent<vp_WeaponHandler>();
			vp_EditorUtility.CopyValuesFromDerivedComponent(fpWHandler, wHandler, true, false, null);
		}

		// convert damage handler
		vp_FPPlayerDamageHandler fpDHandler = target.GetComponent<vp_FPPlayerDamageHandler>();
		if (fpDHandler != null)
		{
			vp_PlayerDamageHandler dHandler = target.AddComponent<vp_PlayerDamageHandler>();
			vp_EditorUtility.CopyValuesFromDerivedComponent(fpDHandler, dHandler, true, true, null);
		}

		// convert event handler
		vp_FPPlayerEventHandler fpEHandler = target.GetComponent<vp_FPPlayerEventHandler>();
		if (fpEHandler != null)
		{
			vp_PlayerEventHandler eHandler = target.AddComponent<vp_PlayerEventHandler>();
			vp_EditorUtility.CopyValuesFromDerivedComponent(fpEHandler, eHandler, true, false, null);
		}

		// convert body animator
		vp_FPBodyAnimator fpBAnimator = target.GetComponentInChildren<vp_FPBodyAnimator>();
		if (fpBAnimator != null)
		{
			vp_BodyAnimator bAnimator = fpBAnimator.gameObject.AddComponent<vp_BodyAnimator>();
			vp_EditorUtility.CopyValuesFromDerivedComponent(fpBAnimator, bAnimator, true, true, null);
		}

		// find any capsule collider and add a rigidbody to it
		CapsuleCollider capsule = target.GetComponentInChildren<CapsuleCollider>();
		if (capsule != null)
		{
			if (capsule.transform.GetComponent<Rigidbody>() == null)
			{
				Rigidbody r = capsule.gameObject.AddComponent<Rigidbody>();
				r.isKinematic = true;
				r.useGravity = false;
			}
			else
			{
				Debug.LogWarning("Warning: " + capsule.gameObject + " has a capsule collider with a Rigidbody on it. Local players should not have rigidbodies since they have CharacterControllers.");
			}
		}

		// delete 'remoteplayer-illegal' components
		DeleteComponentsOfType(target.transform, typeof(vp_FPCamera));		// these first due to dependencies
		DeleteComponentsOfType(target.transform, typeof(vp_FPController));	// these first due to dependencies
		DeleteComponentsOfTypes(target.transform, new System.Type[]
		{
			typeof(Camera),
			typeof(AudioListener),
			typeof(vp_FPInput),
			typeof(vp_SimpleCrosshair),
			typeof(vp_FootstepManager),
			typeof(vp_FPInteractManager),
			typeof(vp_SimpleHUD),
			typeof(vp_PainHUD),
			typeof(vp_FPEarthquake),
			typeof(CharacterController),
			typeof(vp_FPWeaponHandler),
			typeof(vp_FPPlayerDamageHandler),
			typeof(vp_FPPlayerEventHandler),
			typeof(vp_FPBodyAnimator)
		});

		Transform weaponCamera = vp_Utility.GetTransformByNameInChildren(target.transform, "WeaponCamera", true);
		if (weaponCamera != null)
		{
			GameObject.DestroyImmediate(weaponCamera.gameObject);
		}

		target.name = name;

	}

	
	/// <summary>
	/// sets up simplified 3rd person weapon child objects based on the
	/// current first person weapons under the vp_FPCamera component
	/// </summary>
	public static void ConvertWeaponsTo3rdPerson(GameObject player)
	{

		bool copyStates = EditorUtility.DisplayDialog("Duplicate states and presets?", "You have two options:\n\n1) If your 1st person weapons have STATES with PRESETS, any states with 3rd person COMPATIBLE values will be retained and new text files will be created for their presets in a temp folder with the name of your root player object.\n\n2) No states will be retained and no new text file presets created. This means 3rd person weapons (such as remote player ones) will behave differently from 1st person ones while e.g. Zooming, Crouching or Running. This is NOT a good idea for multiplayer.", "1) Retain States", "2) Ignore States");

		string name = player.transform.root.name;
		if (name.Contains("("))
			name = name.Remove(player.transform.root.name.IndexOf("("));

		string path = "Assets/" + vp_EditorUtility.NewValidFolderName("UFPS/Base/Scripts/Presets/3rdPerson/" + name);

		if (!System.IO.Directory.Exists(path))
			System.IO.Directory.CreateDirectory(path);

		int weaponsConverted = 0;

		Transform oldWeaponGroup = null;	// typically the 'FPSCamera' object
		GameObject newWeaponGroup = new GameObject("Weapons");

		foreach (vp_FPWeapon fpWeapon in player.transform.GetComponentsInChildren<vp_FPWeapon>())
		{

			// detect old weapons parent and put the new one alongside it in hierarchy
			if (oldWeaponGroup == null)
			{
				oldWeaponGroup = fpWeapon.transform.parent;
				newWeaponGroup.transform.parent = oldWeaponGroup.parent;
				newWeaponGroup.transform.localPosition = Vector3.zero;
				newWeaponGroup.transform.localEulerAngles = Vector3.zero;
				newWeaponGroup.transform.localScale = Vector3.one;
			}

			// create a new gameobject for each weapon. this way we get rid of
			// any custom components that may be hard for the editor code to
			// delete because of component interdependencies
			GameObject newWeaponGO = new GameObject(fpWeapon.gameObject.name);
			newWeaponGO.transform.parent = newWeaponGroup.transform;
			// NOTE: position of this transform is irrelevant because a 3rd
			// person vp_Weapon is just an invisible, locigal entity, represented
			// by its '3rdPersonWeapon' gameobject reference in the 3d world.
			// still, let's place the weapons in a fairly neutral position for
			// clarity: say, the center of the player facing forwards
			newWeaponGO.transform.localPosition = Vector3.up;
			newWeaponGO.transform.localEulerAngles = Vector3.zero;
			newWeaponGO.transform.localScale = Vector3.one;

			// convert vp_FPWeapon component into a vp_Weapon component on the new gameobject
			vp_Weapon tdpWeapon = newWeaponGO.AddComponent<vp_Weapon>();
			vp_EditorUtility.CopyValuesFromDerivedComponent(fpWeapon, tdpWeapon, true, true, null);
			if (copyStates)
				vp_EditorUtility.GenerateStatesAndPresetsFromDerivedComponent(fpWeapon, tdpWeapon, path);

			// shooters
			vp_FPWeaponShooter fpShooter = fpWeapon.GetComponent<vp_FPWeaponShooter>();
			if (fpShooter != null)
			{
				vp_WeaponShooter tdpShooter = newWeaponGO.AddComponent<vp_WeaponShooter>();
				vp_EditorUtility.CopyValuesFromDerivedComponent(fpShooter, tdpShooter, true, true, null);
				if (copyStates)
					vp_EditorUtility.GenerateStatesAndPresetsFromDerivedComponent(fpShooter, tdpShooter, path);
			}

			// reloaders
			vp_FPWeaponReloader fpReloader = fpWeapon.GetComponent<vp_FPWeaponReloader>();
			if (fpReloader != null)
			{
				vp_WeaponReloader tdpReloader = newWeaponGO.AddComponent<vp_WeaponReloader>();
				vp_EditorUtility.CopyValuesFromDerivedComponent(fpReloader, tdpReloader, true, true, null);
				// reloader is no vp_Component so don't generate states & presets
			}

			// copy item identifier
			vp_ItemIdentifier identifier = fpWeapon.GetComponent<vp_ItemIdentifier>();
			if (identifier != null)
			{
				vp_ItemIdentifier newIdentifier = newWeaponGO.AddComponent<vp_ItemIdentifier>();
				newIdentifier.Type = identifier.Type;
			}

			weaponsConverted++;

		}

		// delete the old weapon group (FPSCamera object)
		if (oldWeaponGroup != null)
			GameObject.DestroyImmediate(oldWeaponGroup.gameObject);

		EditorUtility.DisplayDialog(
			weaponsConverted + " weapons converted",
			(weaponsConverted > 0 ?
			(
				(copyStates ? ("Any states with compatible values have been retained and their presets can be found in the folder:\n\t\t'" + path + "'") : "")
			) : ""),
			"OK");

	}

	
	/// <summary>
	/// deletes all components of the types included in the array
	/// on the provided transform
	/// </summary>
	static void DeleteComponentsOfTypes(Transform trans, System.Type[] types)
	{
		foreach (System.Type t in types)
		{
			DeleteComponentsOfType(trans, t);
		}
	}


	/// <summary>
	/// deletes all components of 'type' on the provided transform
	/// </summary>
	static void DeleteComponentsOfType(Transform trans, System.Type type)
	{

		Component[] components = trans.GetComponentsInChildren(type);
		for (int v = components.Length - 1; v > -1; v--)
		{
			Object.DestroyImmediate(components[v]);
		}

	}


}
