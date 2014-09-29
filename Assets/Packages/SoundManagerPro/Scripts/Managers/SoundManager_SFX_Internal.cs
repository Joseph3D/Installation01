using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	private void SetupSoundFX()
    {
		SetupDictionary();
		
		foreach(KeyValuePair<AudioClip, SFXPoolInfo> pair in ownedPools)
			pair.Value.ownedAudioClipPool.Clear();
		ownedPools.Clear();
		unOwnedSFXObjects.Clear();
		cappedSFXObjects.Clear();
		
		foreach(KeyValuePair<string, AudioClip> entry in allClips)
			PrePoolClip(entry.Value, prepools[entry.Key]);
    }
	
	private void PrePoolClip(AudioClip clip, int prepoolAmount)
	{
		for(int i =0; i<prepoolAmount; i++)
			AddOwnedSFXObject(clip);
	}
	
	private void RemoveSFXObject(SFXPoolInfo info, int index)
	{
		GameObject gO = info.ownedAudioClipPool[index];
		info.ownedAudioClipPool.RemoveAt(index);
		info.timesOfDeath.RemoveAt(index);
		
		if(info.currentIndexInPool >= index)
			info.currentIndexInPool = 0;
		
		Destroy(gO);
	}
	
	/* these functions convert the editor dictionaries to efficient dictionaries while in play */
	private void SetupDictionary()
	{
		allClips.Clear();
		prepools.Clear();
		for(int i = 0; i < storedSFXs.Count; i++)
		{
			if(storedSFXs[i] == null) continue;
			allClips.Add(storedSFXs[i].name, storedSFXs[i]);
			prepools.Add(storedSFXs[i].name, sfxPrePoolAmounts[i]);
		}		
#if !UNITY_EDITOR
		storedSFXs.Clear();	
#endif
		
		if(clipToGroupKeys.Count != clipToGroupValues.Count) //this should never be the case, but in case they are out of sync, sync them.
		{
			if(clipToGroupKeys.Count > clipToGroupValues.Count)
				clipToGroupKeys.RemoveRange(clipToGroupValues.Count, clipToGroupKeys.Count - clipToGroupValues.Count);
			else if(clipToGroupValues.Count > clipToGroupKeys.Count)
				clipToGroupValues.RemoveRange(clipToGroupKeys.Count, clipToGroupValues.Count - clipToGroupKeys.Count);
		}
		
		clipsInGroups.Clear();
		groups.Clear();
		
		for(int i = 0; i < clipToGroupValues.Count; i++)
		{
			if(!ClipNameIsValid(clipToGroupKeys[i]))
				continue;
			
			// Set up clipsInGroups, which maps clip names to group names if they are in a group
			clipsInGroups.Add(clipToGroupKeys[i], clipToGroupValues[i]);
			
			// Set up groups, which maps group names to SFXGroups and populates the clip lists
			if(!groups.ContainsKey(clipToGroupValues[i]))
				groups.Add(clipToGroupValues[i], new SFXGroup(clipToGroupValues[i], new AudioClip[]{Load(clipToGroupKeys[i])}));
			else
			{
				if(groups[clipToGroupValues[i]] == null)
					groups[clipToGroupValues[i]] = new SFXGroup(clipToGroupValues[i], new AudioClip[]{Load(clipToGroupKeys[i])});
				else
					groups[clipToGroupValues[i]].clips.Add(Load(clipToGroupKeys[i]));
			}
		}
		
		foreach(SFXGroup sfxGroup in sfxGroups)
			if(sfxGroup != null && groups.ContainsKey(sfxGroup.groupName))
				groups[sfxGroup.groupName].specificCapAmount = sfxGroup.specificCapAmount;
#if !UNITY_EDITOR
		sfxGroups.Clear();	
#endif
	}
	
	private void AddClipToGroup(string clipName, string groupName)
	{
		// if the clips in a group, set the clip instead.
		if(clipsInGroups.ContainsKey(clipName))
		{
			Debug.LogWarning("This AudioClip("+clipName+") is already assigned to a group: "+GetClipToGroup(clipName)+". It will be moved to the new group.");
			SetClipToGroup(clipName, groupName);
			return;
		}
		
		// if group doesn't exist, create one and add the clip.  Otherwise, add it to the group's clip list.
		SFXGroup grp = GetGroupByGroupName(groupName);
		if(grp == null)
			groups.Add(groupName, new SFXGroup(groupName, new AudioClip[]{Load(clipName)}));
		else
			grp.clips.Add(Load(clipName));
		clipsInGroups.Add(clipName, groupName);
		
#if UNITY_EDITOR
		clipToGroupKeys.Add(clipName);
		clipToGroupValues.Add(groupName);
		if(grp == null)
			sfxGroups.Add(groups[groupName]);
#endif
	}
	
	private void SetClipToGroup(string clipName, string groupName)
	{
		// if in a group, remove it from the group before adding it.
		SFXGroup grp = GetGroupForClipName(clipName);
		if(grp != null)
			RemoveClipFromGroup(clipName);
		AddClipToGroup(clipName, groupName);
	}
	
	private void RemoveClipFromGroup(string clipName)
	{
		// if not in a group, do nothing
		SFXGroup grp = GetGroupForClipName(clipName);
		if(grp == null)
			return;
		else
		{
			// if in a group, remove it
			grp.clips.Remove(Load(clipName));
			clipsInGroups.Remove(clipName);
		}
		
#if UNITY_EDITOR
		int index = clipToGroupKeys.IndexOf(clipName);
		clipToGroupKeys.RemoveAt(index);
		clipToGroupValues.RemoveAt(index);
#endif
	}
	
	private string GetClipToGroup(string clipName)
	{
		return clipsInGroups[clipName];
	}
	/* end of editor necessary functions */	

	private void PSFX(bool pause)
	{
		foreach(KeyValuePair<AudioClip, SFXPoolInfo> pair in ownedPools)
		{
			foreach(GameObject ownedSFXObject in pair.Value.ownedAudioClipPool)
			{
#if UNITY_3_4 || UNITY_3_5
				if(ownedSFXObject != null && ownedSFXObject.active)
#else
				if(ownedSFXObject != null && ownedSFXObject.activeSelf)
#endif
					if(ownedSFXObject.audio != null)
						if(pause)
							ownedSFXObject.audio.Pause();
						else
							ownedSFXObject.audio.Play();
			}
		}
		foreach(GameObject unOwnedSFXObject in unOwnedSFXObjects)
		{
#if UNITY_3_4 || UNITY_3_5
			if(unOwnedSFXObject != null && unOwnedSFXObject.active)
#else
			if(unOwnedSFXObject != null && unOwnedSFXObject.activeSelf)
#endif
				if(unOwnedSFXObject.audio != null)
					if(pause)
						unOwnedSFXObject.audio.Pause();
					else
						unOwnedSFXObject.audio.Play();
		}
	}
	
	private void HandleSFX()
    {
		if(isPaused)
			return;
		
		// Deactivate objects
		foreach(KeyValuePair<AudioClip, SFXPoolInfo> pair in ownedPools)
		{
			for (int i=0; i< pair.Value.ownedAudioClipPool.Count; ++i)
	        {
#if UNITY_3_4 || UNITY_3_5
	            if (pair.Value.ownedAudioClipPool[i].active)
#else
				if(pair.Value.ownedAudioClipPool[i].activeSelf)
#endif
				{
	                if (!pair.Value.ownedAudioClipPool[i].audio.isPlaying)
				    {
						int instanceID = pair.Value.ownedAudioClipPool[i].GetInstanceID();
					    if(cappedSFXObjects.ContainsKey(instanceID))
						     cappedSFXObjects.Remove(instanceID);
#if UNITY_3_4 || UNITY_3_5
	                    pair.Value.ownedAudioClipPool[i].SetActiveRecursively(false);
#else
						pair.Value.ownedAudioClipPool[i].SetActive(false);
#endif
						if(pair.Value.prepoolAmount <= i)
							pair.Value.timesOfDeath[i] = Time.time+SFXObjectLifetime;
				    }
				}
				else if(pair.Value.prepoolAmount <= i && Time.time > pair.Value.timesOfDeath[i])
				{
					RemoveSFXObject(pair.Value, i);
				}
	        }
		}
		
		// Handle removing unowned audio sfx
		for(int i = unOwnedSFXObjects.Count - 1; i >= 0; i--)
		{
			if(unOwnedSFXObjects[i] != null)
				if(unOwnedSFXObjects[i].audio != null)
					if(unOwnedSFXObjects[i].audio.isPlaying || unOwnedSFXObjects[i].audio.mute)
						continue;
			unOwnedSFXObjects.RemoveAt(i);
		}
    }

    private GameObject GetNextInactiveSFXObject(AudioClip clip)
    {
		if(!ownedPools.ContainsKey(clip) || ownedPools[clip].ownedAudioClipPool.Count == 0)
			return AddOwnedSFXObject(clip);
		SFXPoolInfo info = ownedPools[clip];
        for (int i = (info.currentIndexInPool + 1)% info.ownedAudioClipPool.Count; i != info.currentIndexInPool; i = (i + 1) % info.ownedAudioClipPool.Count)
        {
#if UNITY_3_4 || UNITY_3_5
            if (!info.ownedAudioClipPool[i].active)
#else
			if (!info.ownedAudioClipPool[i].activeSelf)
#endif
            {
                ownedPools[clip].currentIndexInPool = i;
				ResetSFXObject(info.ownedAudioClipPool[i]);
                return info.ownedAudioClipPool[i];
            }
        }
        return AddOwnedSFXObject(clip);
    }
	
	private GameObject AddOwnedSFXObject(AudioClip clip)
	{
		GameObject SFXObject = new GameObject("SFX-["+clip.name+"]", typeof(AudioSource));
		SFXObject.name += "(" + SFXObject.GetInstanceID() + ")";
		SFXObject.audio.playOnAwake = false;
		GameObject.DontDestroyOnLoad(SFXObject);
		
		if(ownedPools.ContainsKey(clip))
		{
			ownedPools[clip].ownedAudioClipPool.Add(SFXObject);
			ownedPools[clip].timesOfDeath.Add(0f);
		}
		else
		{
			int thisPrepoolAmount = 0;
			if(allClips.ContainsKey(clip.name))
				thisPrepoolAmount = prepools[clip.name];
			ownedPools.Add(clip, new SFXPoolInfo(0,thisPrepoolAmount,new List<float>(){0f},new List<GameObject>(){SFXObject}));
		}
		ResetSFXObject(SFXObject);
		SFXObject.audio.clip = clip;
		return SFXObject;
	}

    private AudioSource PlaySFXAt(AudioClip clip, float volume, float pitch, Vector3 location, bool capped, string cappedID)
    {
        
        GameObject tempGO = GetNextInactiveSFXObject(clip);
        if (tempGO == null)
            return null;
		
		tempGO.transform.position = location;
#if UNITY_3_4 || UNITY_3_5
        tempGO.SetActiveRecursively(true);
#else
		tempGO.SetActive(true);
#endif
        AudioSource aSource = tempGO.audio;
        aSource.Stop();
        aSource.pitch = pitch;
        aSource.volume = volume;
		aSource.mute = mutedSFX;
        aSource.Play();
		
		if(capped && !string.IsNullOrEmpty(cappedID))
			cappedSFXObjects.Add(tempGO.GetInstanceID(), cappedID);
		
        return aSource;
    }
	
	private AudioSource PlaySFXAt(AudioClip clip, float volume, float pitch, Vector3 location)
    {
        return PlaySFXAt(clip, volume, pitch, location, false, "");
    }
	
	private AudioSource PlaySFXAt(AudioClip clip, float volume, float pitch)
    {
        return PlaySFXAt(clip, volume, pitch, Vector3.zero);
    }

	private IEnumerator _PlaySFXLoopTillDestroy(GameObject gO, AudioSource source, bool tillDestroy, float maxDuration)
	{
		
		bool trackEndTime = false;
		float endTime = Time.time + maxDuration;
		if(!tillDestroy || maxDuration > 0.0f)
			trackEndTime = true;
		
		
		while(ShouldContinueLoop(gO, trackEndTime, endTime))
		{
			yield return null;
		}
		
		source.Stop();
	}
	
	private void _StopSFX()
	{
		foreach(KeyValuePair<AudioClip, SFXPoolInfo> pair in ownedPools)
		{
			foreach(GameObject ownedSFXObject in pair.Value.ownedAudioClipPool)
#if UNITY_3_4 || UNITY_3_5
				if(ownedSFXObject != null && ownedSFXObject.active)
#else
				if(ownedSFXObject != null && ownedSFXObject.activeSelf)
#endif
					if(ownedSFXObject.audio != null)
						ownedSFXObject.audio.Stop();
		}
		
		foreach(GameObject unOwnedSFXObject in unOwnedSFXObjects)
#if UNITY_3_4 || UNITY_3_5
			if(unOwnedSFXObject != null && unOwnedSFXObject.active)
#else
			if(unOwnedSFXObject != null && unOwnedSFXObject.activeSelf)
#endif
				if(unOwnedSFXObject.audio != null)
					unOwnedSFXObject.audio.Stop();
	}
	
	private bool ShouldContinueLoop(GameObject gO, bool trackEndTime, float endTime)
	{
#if UNITY_3_4 || UNITY_3_5
		bool shouldContinue = (gO != null && gO.active);
#else
		bool shouldContinue = (gO != null && gO.activeSelf);
#endif
		if(trackEndTime)
			shouldContinue = (shouldContinue && Time.time < endTime);
		return shouldContinue;
	}
	
	/// <summary>
	/// Determines whether the specified cappedID is at capacity.
	/// </summary>
	private bool IsAtCapacity(string cappedID, string clipName)
	{
		int thisCapAmount = capAmount;
		
		// Check if in a group and has a specific cap amount
		SFXGroup grp = GetGroupForClipName(clipName);
		if(grp != null)
		{
			if(grp.specificCapAmount == 0) // If no cap amount on this group
				return false;
			if(grp.specificCapAmount != -1) // If it is a specific cap amount
				thisCapAmount = grp.specificCapAmount;
		}
		
		// If there are no other capped objects with this cappedID, then it can't be at capacity
		if(!cappedSFXObjects.ContainsValue(cappedID))
			return false;
		
		// Check the count of capped objects with the same cappedID, if >= return true
		int count = 0;
		foreach(string id in cappedSFXObjects.Values)
		{
			if(id == cappedID)
			{
				count++;
				if(count >= thisCapAmount)
					return true;
			}
		}
		return false;
	}
	
	private SFXGroup GetGroupForClipName(string clipName)
	{
		if(!clipsInGroups.ContainsKey(clipName))
			return null;
		return groups[clipsInGroups[clipName]];
	}
	
	private SFXGroup GetGroupByGroupName(string grpName)
	{
		if(!groups.ContainsKey(grpName))
			return null;
		return groups[grpName];
	}
}
