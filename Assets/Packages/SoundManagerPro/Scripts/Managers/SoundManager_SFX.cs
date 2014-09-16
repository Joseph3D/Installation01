using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	
	/// <summary>
	/// Sets the SFX cap.
	/// </summary>
	/// <param name='cap'>
	/// Cap.
	/// </param>
	public static void SetSFXCap(int cap)
	{
		Instance.capAmount = cap;
	}
    
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(AudioClip clip, float volume, float pitch, Vector3 location)
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null)
            return null;
        
        return Instance.PlaySFXAt(clip, volume, pitch, location);
    }
	
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioClip clip, float volume, float pitch)
    {
        return PlaySFX(clip, volume, pitch, Vector3.zero);
    }
	
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioClip clip, float volume)
    {
        return PlaySFX(clip, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX on an owned object, will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioClip clip)
    {
        return PlaySFX(clip, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume, float pitch, Vector3 location)
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null)
            return null;
		
		if(string.IsNullOrEmpty(cappedID))
			return null;
        
        // Play the clip if not at capacity
		if(!Instance.IsAtCapacity(cappedID, clip.name))
            return Instance.PlaySFXAt(clip, volume, pitch, location, true, cappedID);
		else
			return null;
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume, float pitch)
    {
        return PlayCappedSFX(clip, cappedID, volume, pitch, Vector3.zero);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID, float volume)
    {
        return PlayCappedSFX(clip, cappedID, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioClip clip, string cappedID)
    {
        return PlayCappedSFX(clip, cappedID, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID, float volume, float pitch)
    {
        if (Instance.offTheSFX)
            return null;
            
        if (clip == null || aS == null)
            return null;
		
		if(string.IsNullOrEmpty(cappedID))
			return null;
		
		// Keep reference of unownedsfx objects
		if(!Instance.unOwnedSFXObjects.Contains(aS.gameObject))
			Instance.unOwnedSFXObjects.Add(aS.gameObject);
        
        // Play the clip if not at capacity
		if(!Instance.IsAtCapacity(cappedID, clip.name))
		{
			aS.Stop();
		    aS.pitch = pitch;
		    aS.clip = clip;
		    aS.volume = volume;
			aS.mute = Instance.mutedSFX;
		    aS.Play();
			
			return aS;
		}
		else
			return null;
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID, float volume)
    {
        return PlayCappedSFX(aS, clip, cappedID, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX IFF other SFX with the same cappedID are not over the cap limit. Will default the location to (0,0,0), pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlayCappedSFX(AudioSource aS, AudioClip clip, string cappedID)
    {
        return PlayCappedSFX(aS, clip, cappedID, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping, float volume, float pitch)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (aS == null))
            return null;
		
		// Keep reference of unownedsfx objects
		if(!Instance.unOwnedSFXObjects.Contains(aS.gameObject))
			Instance.unOwnedSFXObjects.Add(aS.gameObject);
            
        aS.Stop();
        aS.pitch = pitch;
        aS.clip = clip;
        aS.loop = looping;
        aS.volume = volume;
		aS.mute = Instance.mutedSFX;
        aS.Play();
		
		return aS;
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping, float volume)
    {
        return PlaySFX(aS, clip, looping, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip, bool looping)
    {
        return PlaySFX(aS, clip, looping, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX another audiosource of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(AudioSource aS, AudioClip clip)
    {
        return PlaySFX(aS, clip, false);
    }
	
	/// <summary>
	/// Stops the SFX on another audiosource
	/// </summary>
    public static void StopSFXObject(AudioSource aS)
    {
        if (aS == null)
            return;
            
        if (aS.isPlaying)
            aS.Stop();
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
    public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping, float volume, float pitch)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (gO == null))
            return null;
        
        if (gO.audio == null)
            gO.AddComponent<AudioSource>();
		
		return PlaySFX(gO.audio, clip, looping, volume, pitch);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping, float volume)
    {
        return PlaySFX(gO, clip, looping, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip, bool looping)
    {
        return PlaySFX(gO, clip, looping, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX another gameObject of your choice, will default the looping to false, pitch to 1f, volume to 1f
	/// </summary>
	public static AudioSource PlaySFX(GameObject gO, AudioClip clip)
    {
        return PlaySFX(gO, clip, false);
    }
	
	/// <summary>
	/// Stops the SFX on another gameObject
	/// </summary>
    public static void StopSFXObject(GameObject gO)
    {
        if (gO == null)
            return;
        
        if (gO.audio == null)
            return;
            
        if (gO.audio.isPlaying)
            gO.audio.Stop();
    }
	
	/// <summary>
	/// Stops all SFX.
	/// </summary>
	public static void StopSFX()
	{
		Instance._StopSFX();
	}
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
    public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume, float pitch, float maxDuration)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (aS == null))
            return null;
		
		if(!Instance.unOwnedSFXObjects.Contains(aS.gameObject))
			Instance.unOwnedSFXObjects.Add(aS.gameObject);
		
		aS.Stop();
		aS.clip = clip;
		aS.pitch = pitch;
		aS.volume = volume;
		aS.mute = Instance.mutedSFX;
		aS.loop = true;
		aS.Play();

        Instance.StartCoroutine(Instance._PlaySFXLoopTillDestroy(aS.gameObject, aS, tillDestroy, maxDuration));
		return aS;
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume, float pitch)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, volume, pitch, 0f);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy, float volume)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip, bool tillDestroy)
    {
        return PlaySFXLoop(aS, clip, tillDestroy, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another audiosource of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(AudioSource aS, AudioClip clip)
    {
        return PlaySFXLoop(aS, clip, true);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
    public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume, float pitch, float maxDuration)
    {
        if (Instance.offTheSFX)
            return null;
            
        if ((clip == null) || (gO == null))
            return null;
		
		if (gO.audio == null)
            gO.AddComponent<AudioSource>();
		
		if(!Instance.unOwnedSFXObjects.Contains(gO))
			Instance.unOwnedSFXObjects.Add(gO);
		
		AudioSource aSource = gO.audio;
		aSource.Stop();
		aSource.clip = clip;
		aSource.pitch = pitch;
		aSource.volume = volume;
		aSource.mute = Instance.mutedSFX;
		aSource.loop = true;
		aSource.Play();

        Instance.StartCoroutine(Instance._PlaySFXLoopTillDestroy(gO, aSource, tillDestroy, maxDuration));
		return aSource;
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume, float pitch)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, volume, pitch, 0f);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy, float volume)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, volume, Instance.pitchSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip, bool tillDestroy)
    {
        return PlaySFXLoop(gO, clip, tillDestroy, Instance.volumeSFX);
    }
	
	/// <summary>
	/// Plays the SFX in a loop on another gameObject of your choice.  This function is cattered more towards customizing a loop.
	/// You can set the loop to end when the object dies or a maximum duration, whichever comes first.
	/// tillDestroy defaults to true, volume to 1f, pitch to 1f, maxDuration to 0f
	/// </summary>
	public static AudioSource PlaySFXLoop(GameObject gO, AudioClip clip)
    {
        return PlaySFXLoop(gO, clip, true);
    }
	
	/// <summary>
	/// Sets mute on all the SFX to 'mute'. Returns the result.
	/// </summary>
	public static bool MuteSFX(bool toggle)
    {
        Instance.mutedSFX = toggle;
		return Instance.mutedSFX;
    }
	
	/// <summary>
	/// Toggles mute on SFX. Returns the result.
	/// </summary>
	public static bool MuteSFX()
    {
        return MuteSFX(!Instance.mutedSFX);
    }
	
	/// <summary>
	/// Determines whether this instance is SFX muted.
	/// </summary>
	public static bool IsSFXMuted()
	{
		return Instance.mutedSFX;
	}
	
	/// <summary>
	/// Sets the maximum volume of SFX in the game relative to the global volume.
	/// </summary>
	public static void SetVolumeSFX(float setVolume)
	{
		setVolume = Mathf.Clamp01(setVolume);
		
		float currentPercentageOfVolume;
		currentPercentageOfVolume = Instance.volumeSFX / Instance.maxSFXVolume;
		
		Instance.maxSFXVolume = setVolume * Instance.maxVolume;
		
		if(float.IsNaN(currentPercentageOfVolume) || float.IsInfinity(currentPercentageOfVolume))
			currentPercentageOfVolume = 1f;
		
		Instance.volumeSFX = Instance.maxSFXVolume * currentPercentageOfVolume;
	}
	
	/// <summary>
	/// Gets the volume SFX.
	/// </summary>
	public static float GetVolumeSFX()
	{
		return Instance.maxSFXVolume;
	}
	
	/// <summary>
	/// Sets the pitch of SFX in the game.
	/// </summary>
	public static void SetPitchSFX(float setPitch)
	{
		Instance.pitchSFX = setPitch;
	}
	
	/// <summary>
	/// Gets the pitch SFX.
	/// </summary>
	public static float GetPitchSFX()
	{
		return Instance.pitchSFX;
	}
	
	/////////////////////////////////////////////////////
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.  Will register the SFX to the group.
	/// </summary>
	public static void SaveSFX(AudioClip clip, string grpName)
	{
		if(clip == null)
			return;
		
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
			Debug.LogWarning("The SFXGroup, "+grpName+", does not exist. Creating it as a new group");
		
		SaveSFX(clip);
		Instance.AddClipToGroup(clip.name, grpName);
	}
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.  Will register the SFX to the group specified.
	/// If the group doesn't exist, it will be added to SoundManager.
	public static void SaveSFX(AudioClip clip, SFXGroup grp)
	{
		if(clip == null)
			return;
		
		if(grp != null)
		{
			if(!Instance.groups.ContainsKey(grp.groupName))
			{
				Instance.groups.Add(grp.groupName, grp);
#if UNITY_EDITOR
				Instance.sfxGroups.Add(grp);
#endif
			}
			else if(Instance.groups[grp.groupName] != grp)
				Debug.LogWarning("The SFXGroup, "+grp.groupName+", already exists. This new group will not be added.");
		}
		
		SaveSFX(clip);
		Instance.AddClipToGroup(clip.name, grp.groupName);
	}
	
	/// <summary>
	/// Saves the SFX to the SoundManager prefab for easy access for frequently used SFX.
	/// </summary>
	public static void SaveSFX(AudioClip clip)
	{
		if(clip == null)
			return;
		
		if(!Instance.allClips.ContainsKey(clip.name))
		{
			Instance.allClips.Add(clip.name, clip);
			Instance.prepools.Add(clip.name, 0);
#if UNITY_EDITOR
			Instance.storedSFXs.Add(clip);
			Instance.sfxPrePoolAmounts.Add(0);
			Instance.showSFXDetails.Add(false);
#endif
		}
	}
	
	/// <summary>
	/// Creates the SFX group and adds it to SoundManager.
	/// </summary>
	public static SFXGroup CreateSFXGroup(string grpName, int capAmount)
	{
		if(!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup grp = new SFXGroup(grpName, capAmount);
			Instance.groups.Add(grpName, grp);
#if UNITY_EDITOR
			Instance.sfxGroups.Add(grp);
#endif
			return grp;
		}
		Debug.LogWarning("This group already exists. Cannot add it.");
		return null;
	}
	
	/// <summary>
	/// Creates the SFX group and adds it to SoundManager.
	/// </summary>
	public static SFXGroup CreateSFXGroup(string grpName)
	{
		if(!Instance.groups.ContainsKey(grpName))
		{
			SFXGroup grp = new SFXGroup(grpName);
			Instance.groups.Add(grpName, grp);
#if UNITY_EDITOR
			Instance.sfxGroups.Add(grp);
#endif
			return grp;
		}
		Debug.LogWarning("This group already exists. Cannot add it.");
		return null;
	}
	
	/// <summary>
	/// Moves a clip to the specified group. If the group doesn't exist, it will make the group.
	/// </summary>
	public static void MoveToSFXGroup(string clipName, string newGroupName)
	{
		Instance.SetClipToGroup(clipName, newGroupName);
	}
	
	/// <summary>
	/// Loads a random SFX from a specified group.
	/// </summary>
	public static AudioClip LoadFromGroup(string grpName)
	{
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
		{
			Debug.LogError("There is no group by this name: "+grpName+".");
			return null;
		}
		
		AudioClip result = null;
		
		// check if clips is empty
		if(grp.clips.Count == 0)
		{
			Debug.LogWarning("There are no clips in this group: " + grpName);
			return null;
		}
		
		// Get random clip from list
		result = grp.clips[Random.Range(0, grp.clips.Count)];
		
		// return result
		return result;
	}
	
	/// <summary>
	/// Loads all SFX from a specified group.
	/// </summary>
	public static AudioClip[] LoadAllFromGroup(string grpName)
	{
		SFXGroup grp = Instance.GetGroupByGroupName(grpName);
		if(grp == null)
		{
			Debug.LogError("There is no group by this name, "+grpName+".");
			return null;
		}
		
		// check if group is empty
		if(grp.clips.Count == 0)
		{
			Debug.LogWarning("There are no clips in this group: " + grpName);
			return null;
		}
		
		// return all clips in array
		return grp.clips.ToArray();
	}
	
	/// <summary>
	/// Load the specified clipname, at a custom path if you do not want to use resourcesPath.
	/// If custompath fails or is empty/null, it will query the stored SFXs.  If that fails, it'll query the default
	/// resourcesPath.  If all else fails, it'll return null.
	/// </summary>
	/// <param name='clipname'>
	/// Clipname.
	/// </param>
	/// <param name='customPath'>
	/// Custom path.
	/// </param>
	public static AudioClip Load(string clipname, string customPath)
	{
		AudioClip result = null;
		
		// Attempt to use custom path if provided
		if(!string.IsNullOrEmpty(customPath))
			if(customPath[customPath.Length-1] == '/')
				result = (AudioClip)Resources.Load(customPath.Substring(0,customPath.Length) + "/" + clipname);
			else
				result = (AudioClip)Resources.Load(customPath + "/" + clipname);
				
		if(result)
			return result;
		
		// If custom path fails, attempt to find it in our stored SFXs
		if(Instance.allClips.ContainsKey(clipname))
			result = Instance.allClips[clipname];
		
		if(result)
			return result;
		
		// If it is not in our stored SFX, attempt to find it in our default resources path
		result = (AudioClip)Resources.Load(Instance.resourcesPath + "/" + clipname);
		
		return result;
	}
	
	/// <summary>
	/// Load the specified clipname from the stored SFXs.  If that fails, it'll query the default
	/// resourcesPath.  If all else fails, it'll return null.
	/// </summary>
	/// <param name='clipname'>
	/// Clipname.
	/// </param>
	public static AudioClip Load(string clipname)
	{
		return Load(clipname, "");
	}
	
	/// <summary>
	/// Resets the SFX object.
	/// </summary>
	public static void ResetSFXObject(GameObject sfxObj)
	{
		if(sfxObj.audio == null)
			return;
		
		sfxObj.audio.mute = false;
		sfxObj.audio.bypassEffects = false;
		sfxObj.audio.playOnAwake = false;
		sfxObj.audio.loop = false;
		
		sfxObj.audio.priority = 128;
		sfxObj.audio.volume = 1f;
		sfxObj.audio.pitch = 1f;
		
		sfxObj.audio.dopplerLevel = 1f;
		sfxObj.audio.rolloffMode = AudioRolloffMode.Logarithmic;
		sfxObj.audio.minDistance = 1f;
		sfxObj.audio.panLevel = 1f;
		sfxObj.audio.spread = 0f;
		sfxObj.audio.maxDistance = 500f;
		
		sfxObj.audio.pan = 0f;
	}
}
