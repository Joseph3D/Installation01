using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SoundConnection {
	public string level;
	public bool isCustomLevel;
	public List<AudioClip> soundsToPlay;
	public SoundManager.PlayMethod playMethod;
	public float minDelay;
	public float maxDelay;
	public float delay;
	
	/*Ties the level name to a list of AudioClips. It defaults to continuous play through with no delay between clips.*/
	public SoundConnection(string lvl, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = SoundManager.PlayMethod.ContinuousPlayThrough;
		minDelay = 0f;
		maxDelay = 0f;
		delay = 0f;
		soundsToPlay = new List<AudioClip>();
		foreach(AudioClip audio in audioList)
		{
			if(!soundsToPlay.Contains(audio))
			{
				soundsToPlay.Add(audio);
			}
		}
	}
	
	public SoundConnection(string lvl, SoundManager.PlayMethod method, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		switch(playMethod)
		{
		case SoundManager.PlayMethod.ContinuousPlayThrough:
		case SoundManager.PlayMethod.OncePlayThrough:
		case SoundManager.PlayMethod.ShufflePlayThrough:
			break;
		default:
			Debug.LogWarning("No delay was set in the constructor so there will be none.");
			break;
		}
		minDelay = 0f;
		maxDelay = 0f;
		delay = 0f;
		soundsToPlay = new List<AudioClip>();
		foreach(AudioClip audio in audioList)
		{
			if(!soundsToPlay.Contains(audio))
			{
				soundsToPlay.Add(audio);
			}
		}
	}
	
	public SoundConnection(string lvl, SoundManager.PlayMethod method, float delayPlay, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		minDelay = 0f;
		maxDelay = delayPlay;
		delay = delayPlay;
		soundsToPlay = new List<AudioClip>();
		foreach(AudioClip audio in audioList)
		{
			if(!soundsToPlay.Contains(audio))
			{
				soundsToPlay.Add(audio);
			}
		}
	}
	
	public SoundConnection(string lvl, SoundManager.PlayMethod method, float minDelayPlay, float maxDelayPlay, params AudioClip[] audioList)
	{
		level = lvl;
		isCustomLevel = false;
		playMethod = method;
		minDelay = minDelayPlay;
		maxDelay = maxDelayPlay;
		delay = (maxDelayPlay+minDelayPlay) / 2f;
		soundsToPlay = new List<AudioClip>();
		foreach(AudioClip audio in audioList)
		{
			if(!soundsToPlay.Contains(audio))
			{
				soundsToPlay.Add(audio);
			}
		}
	}
	
	/// <summary>
	/// Sets this SoundConnection to a Custom SoundConnection.  Is not tied to a scene.  Must be called with SoundManager.CustomEvent
	/// Be careful not to use a level name or SoundManager will get confused.  Call this after calling Initialize only.
	/// </summary>
	public void SetToCustom()
	{
		isCustomLevel = true;
	}
}
