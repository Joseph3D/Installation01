using UnityEngine;
using System.Collections;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	
	private void Awake()
	{
		Setup();
		/* DO NOT PUT ANYTHING HERE */
	}
	
	/// <summary>
	/// Awake this instance.
	/// Destroy if it's not the singleton.
	/// Clear all AudioSources if any and initiate new ones.
	/// Call LevelLoad function on starting level.
	/// Clear SFX, start fresh
	/// Set name of SoundManager
	/// </summary>
	private void Setup()
	{
		if (Instance && Instance.gameObject != gameObject)
			Destroy(gameObject);
		else {
			gameObject.name = this.name;
			DontDestroyOnLoad(gameObject);
			ClearAudioSources();
			Init();
			SetupSoundFX();
			OnLevelWasLoaded(Application.loadedLevel);
		}
	}
	
	/// <summary>
	/// Init this instance with the appropriate AudioSources and settings.
	/// This should never have to be called, but if it does--Only call it once.
	/// </summary>
	private void Init()
	{
		if(Instance){
			audios = new AudioSource[2];
			audios[0] = gameObject.AddComponent<AudioSource>(); 
			audios[1] = gameObject.AddComponent<AudioSource>();
			
			audios[0].hideFlags = HideFlags.HideInInspector;
			audios[1].hideFlags = HideFlags.HideInInspector;
			
			SoundManagerTools.make2D(ref audios[0]);
			SoundManagerTools.make2D(ref audios[1]);
			
			audios[0].volume = 0f;
			audios[1].volume = 0f;
			
			audios[0].ignoreListenerVolume = true;
			audios[1].ignoreListenerVolume = true;
			
			maxVolume = AudioListener.volume;
			
			currentPlaying = CheckWhosPlaying();
		}
	}
	
	/// <summary>
	/// Clears the audio sources.
	/// </summary>
	private void ClearAudioSources()
	{
		AudioSource[] currentSources = gameObject.GetComponents<AudioSource>();
		foreach(AudioSource source in currentSources)
			Destroy(source);
	}
	
	/// <summary>
	/// Plays the SoundConnection right then, regardless of what you put at the level parameter of the SoundConnection.
	/// </summary>
	private void _PlayConnection(SoundConnection sc)
	{
		if(offTheBGM) return;
		if(string.IsNullOrEmpty(sc.level))
		{
			int i = 1;
			while(SoundConnectionsContainsThisLevel("CustomConnection"+i.ToString()) != SOUNDMANAGER_FALSE)
				i++;
			sc.level = "CustomConnection"+i.ToString();
		}
		StopPreviousPlaySoundConnection();
		StartCoroutine("PlaySoundConnection", sc);
	}
	
	/// <summary>
	/// Plays a SoundConnection on SoundManager that matches the level name.
	/// </summary>
	private void _PlayConnection(string levelName)
	{
		if(offTheBGM) return;
		int _indexOf = SoundConnectionsContainsThisLevel(levelName);
		if(_indexOf != SOUNDMANAGER_FALSE) {
			StopPreviousPlaySoundConnection();
			StartCoroutine("PlaySoundConnection", soundConnections[_indexOf]);
		} else {
			Debug.LogError("There are no SoundConnections with the name: " + levelName);
		}
	}
	
	private void StopPreviousPlaySoundConnection()
	{
		StopCoroutine("PlaySoundConnection");
	}
	
	/// <summary>
	/// Checks the who's not playing.  This will return SOUNDMANAGER_FALSE(-1) IFF both AudioSources are playing.  This is usually in a crossfade situation.
	/// </summary>
	/// <returns>
	/// The index of who's not playing.
	/// </returns>
	private int CheckWhosNotPlaying(){
		if(!audios[0].isPlaying) return 0;
		else if(!audios[1].isPlaying) return 1;
		else return SOUNDMANAGER_FALSE;
	}
	
	/// <summary>
	/// Checks the who's playing.  This will return SOUNDMANAGER_FALSE(-1) IFF no AudioSources are playing.  This is usually in a silent scene situation.
	/// </summary>
	/// <returns>
	/// The index of who's playing.
	/// </returns>
	private int CheckWhosPlaying(){
		if(audios[0].isPlaying) return 0;
		else if(audios[1].isPlaying) return 1;
		else return SOUNDMANAGER_FALSE;
	}
	
	/// <summary>
	/// Raises the level was loaded event.  It handles playing the right SoundConnection on level load.
	/// </summary>
	private void HandleLevel(int level)
	{
		if(gameObject != gameObject) return;
		
		if(Time.realtimeSinceStartup != 0f && lastLevelLoad == Time.realtimeSinceStartup)
			return;
		lastLevelLoad = Time.realtimeSinceStartup;
		
		if(showDebug)Debug.Log("(" +Time.time + ") In Level Loaded: " + Application.loadedLevelName);
		int _indexOf = SoundConnectionsContainsThisLevel(Application.loadedLevelName);
		if(_indexOf == SOUNDMANAGER_FALSE || soundConnections[_indexOf].isCustomLevel) {
			silentLevel = true;
		} else {
			silentLevel = false;
			currentLevel = Application.loadedLevelName;
			currentSoundConnection = soundConnections[_indexOf];
		}
		
		if(!silentLevel && !offTheBGM)
		{
			if(showDebug)Debug.Log("BGM activated.");
			StopPreviousPlaySoundConnection();
			StartCoroutine("PlaySoundConnection", currentSoundConnection);
		} 
		else 
		{
			if(showDebug)Debug.Log("BGM deactivated.");
			currentSoundConnection = null;
			audios[0].loop = false;
			audios[1].loop = false;
			if(showDebug)Debug.Log("Don't play anything in this scene, cross out.");
			currentPlaying = CheckWhosPlaying();
			StopAllCoroutines();
			if(currentPlaying == SOUNDMANAGER_FALSE) {
				if(showDebug)Debug.Log("Nothing is playing, don't do anything.");
				return;
			}
			else if(CheckWhosNotPlaying() == SOUNDMANAGER_FALSE) {
				if(showDebug)Debug.Log("Both sources are playing, probably in a crossfade. Crossfade them both out.");
				StartCoroutine("CrossoutAll",crossDuration);
			} else if(audios[currentPlaying].isPlaying)
			{
				if(showDebug)Debug.Log("Crossing out the source that is playing.");
				StartCoroutine("Crossout", new object[]{audios[currentPlaying],crossDuration});
			}	
		}
	}
	
	/// <summary>
	/// Plays the clip immediately regardless of a playing SoundConnection, with an option to loop.  Calls an event once the clip is done.
	/// You can resume a SoundConnection afterwards if you so choose, using currentSoundConnection.  However, it will not resume on it's own.
	/// Callbacks will only fire once.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	/// <param name='runOnEndFunction'>
	/// Function to run once the clip is done.  Is added to OnSongEnd
	/// </param>
	/// <param name='loop'>
	/// Whether the clip should loop
	/// </param>
	private void _PlayImmediately(AudioClip clip2play, bool loop, SongCallBack runOnEndFunction)
	{
		if(InternalCallback != null)
			OnSongEnd = InternalCallback;
		InternalCallback = runOnEndFunction;
		
		StopMusicImmediately();
		if(offTheBGM) return;
		SoundConnection sc;
		if(loop)
			sc = new SoundConnection("",PlayMethod.ContinuousPlayThrough,clip2play);
		else
			sc = new SoundConnection("",PlayMethod.OncePlayThrough,clip2play);
		PlayConnection(sc);
	}
	
	/// <summary>
	/// Plays the clip immediately regardless of a playing SoundConnection, with an option to loop.  It will not resume on it's own.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	/// <param name='loop'>
	/// Whether the clip should loop
	/// </param>
	private void _PlayImmediately(AudioClip clip2play, bool loop)
	{
		_PlayImmediately(clip2play, loop, null);
	}
	
	/// <summary>
	/// Plays the clip immediately regardless of a playing SoundConnection.  It will not resume on it's own.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	private void _PlayImmediately(AudioClip clip2play)
	{
		_PlayImmediately(clip2play, false);
	}
	
	/// <summary>
	/// Plays the clip by crossing out what's currently playing regardless of a playing SoundConnection, with an option to loop.  Calls an event once the clip is done.
	/// You can resume a SoundConnection afterwards if you so choose, using currentSoundConnection.  However, it will not resume on it's own.
	/// Callbacks will only fire once.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	/// <param name='runOnEndFunction'>
	/// Function to run once the clip is done.  Is added to OnSongEnd
	/// </param>
	/// <param name='loop'>
	/// Whether the clip should loop
	/// </param>
	private void _Play(AudioClip clip2play, bool loop, SongCallBack runOnEndFunction)
	{
		if(offTheBGM) return;
		if(InternalCallback != null)
			OnSongEnd = InternalCallback;
		InternalCallback = runOnEndFunction;
		
		SoundConnection sc;
		if(loop)
			sc = new SoundConnection(Application.loadedLevelName,PlayMethod.ContinuousPlayThrough,clip2play);
		else
			sc = new SoundConnection(Application.loadedLevelName,PlayMethod.OncePlayThrough,clip2play);
		PlayConnection(sc);
	}
	
	/// <summary>
	/// Plays the clip by crossing out what's currently playing regardless of a playing SoundConnection, with an option to loop.  It will not resume on it's own.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	/// <param name='loop'>
	/// Whether the clip should loop
	/// </param>
	private void _Play(AudioClip clip2play, bool loop)
	{
		_Play(clip2play, loop, null);
	}
	
	/// <summary>
	/// Plays the clip by crossing out what's currently playing regardless of a playing SoundConnection.  It will not resume on it's own.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	private void _Play(AudioClip clip2play)
	{
		_Play(clip2play, false);
	}
	
	/// <summary>
	/// Stops all sound immediately.
	/// </summary>
	private void _StopMusicImmediately()
	{
		StopAllCoroutines();
		StartCoroutine("CrossoutAll", 0f);
	}
	
	/// <summary>
	/// Crosses out all AudioSources.
	/// </summary>
	private void _StopMusic()
	{
		StopAllCoroutines();
		StartCoroutine("CrossoutAll",(crossDuration));
	}
	
	/// <summary>
	/// Pause's all sounds
	/// </summary>
	private void _Pause()
	{
		if(!isPaused)
		{
			isPaused = !isPaused;
			audios[0].Pause();
			audios[1].Pause();
			PSFX(true);
		}
	}
	
	/// <summary>
	/// Unpause's all sounds
	/// </summary>
	private void _UnPause()
	{
		if(isPaused)
		{
			audios[0].Play();
			audios[1].Play();
			PSFX(false);
			isPaused = !isPaused;
		}
	}
	
	/// <summary>
	/// Plays the clip.  This is a private method because it must be used appropriately with a SoundConnection.
	/// It's logic is dependent on it being used correctly.
	/// </summary>
	/// <param name='clip2play'>
	/// The clip to play.
	/// </param>
	/// <returns>
	/// The index of the new audiosource if the clip is already being played, otherwise SOUNDMANAGER_FALSE
	/// </returns>
	private int PlayClip(AudioClip clip2play)
	{
		if(showDebug)Debug.Log ("Playing: " + clip2play.name);
		currentPlaying = CheckWhosPlaying();
		int notPlaying = CheckWhosNotPlaying();
		if(currentPlaying != SOUNDMANAGER_FALSE) //If an AudioSource is playing...
		{
			if(notPlaying != SOUNDMANAGER_FALSE) //If one AudioSources is playing...
			{
				if((audios[currentPlaying].clip.Equals(clip2play) && audios[currentPlaying].isPlaying)) //If the current playing source is playing the clip...
				{
					if(showDebug)Debug.Log("Already playing BGM, check if crossing out("+crossingOut+") or in("+crossingIn+").");
					if(crossingOut) //If that source is crossing out, stop it and cross it back in.
					{
						StopAllNonSoundConnectionCoroutines();
						if(showDebug)Debug.Log("In the process of crossing out, so that is being changed to cross in now.");
						outCrossing[currentPlaying] = false;
						StartCoroutine("Crossin", new object[]{audios[currentPlaying],crossDuration});
						return currentPlaying;
					} else if (movingOnFromSong) {
						if(showDebug)Debug.Log("Current song is actually done, so crossfading to another instance of it.");
						if(audios[notPlaying] == null || audios[notPlaying].clip == null || !audios[notPlaying].clip.Equals(clip2play))
							audios[notPlaying].clip = clip2play;
						StartCoroutine("Crossfade", new object[]{audios[currentPlaying],audios[notPlaying],crossDuration});
						return notPlaying;
					}
					return currentPlaying;
				}
				else //If the current playing source is not playing the clip, crossfade to the clip normally.
				{
					StopAllNonSoundConnectionCoroutines();
					if(showDebug)Debug.Log("Playing another track, crossfading to that.");
					audios[notPlaying].clip = clip2play;
					StartCoroutine("Crossfade", new object[]{audios[currentPlaying],audios[notPlaying],crossDuration});
					return notPlaying;
				}
			}
			else //If both AudioSources are playing...
			{
				if(showDebug)Debug.Log("Both are playing (crossfade situation).");
				if(clip2play.Equals(audios[0].clip) && clip2play.Equals(audios[1].clip))
				{
					if(showDebug)Debug.Log("If clip == clip in audio1 AND audio2, then do nothing and let it finish.");
					int swapIn = (lastPlaying == 0) ? 0 : 1;
					
					if(!audios[0].isPlaying)audios[0].Play();
					if(!audios[1].isPlaying)audios[1].Play();
					
					return swapIn;
				}
				else if(clip2play.Equals(audios[0].clip)) //If the clip is the same clip playing in source1...
				{
					if(showDebug)Debug.Log("If clip == clip in audio1, then just switch them.");
					int swapIn = (lastPlaying == 0) ? 0 : 1;
					int swapOut = (swapIn == 0) ? 1 : 0;
					StopAllNonSoundConnectionCoroutines();
					if(swapIn != 0) //If the source crossing out is not the clip, just continue with crossfade
					{
						StartCoroutine("Crossfade", new object[]{audios[swapIn],audios[swapOut],crossDuration});
					} else { //If the source crossing out is the clip, swap them so that it's now crossing in
						StartCoroutine("Crossfade", new object[]{audios[swapOut],audios[swapIn],crossDuration});
					}
					if(!audios[0].isPlaying)audios[0].Play();
					if(!audios[1].isPlaying)audios[1].Play();
					if(swapIn != 0)
						return swapOut;
					return swapIn;
				}
				else if(clip2play.Equals(audios[1].clip)) //If the clip is the same clip playing in source2...
				{
					if(showDebug)Debug.Log("If clip == clip in audio2, then just switch them.");
					int swapIn = (lastPlaying == 0) ? 0 : 1;
					int swapOut = (swapIn == 0) ? 1 : 0;
					StopAllNonSoundConnectionCoroutines();
					if(swapIn != 1) //If the source crossing out is not the clip, just continue with crossfade
					{
						StartCoroutine("Crossfade", new object[]{audios[swapIn],audios[swapOut],crossDuration});
					} else { //If the source crossing out is the clip, swap them so that it's now crossing in
						StartCoroutine("Crossfade", new object[]{audios[swapOut],audios[swapIn],crossDuration});
					};
					if(!audios[0].isPlaying)audios[0].Play();
					if(!audios[1].isPlaying)audios[1].Play();
					if(swapIn != 1)
						return swapOut;
					return swapIn;
				} 
				else 
				{ // If the clip is not in either source1 or source2...
					StopAllNonSoundConnectionCoroutines();
					if(showDebug)Debug.Log("If clip is in neither, find the louder one and crossfade from that one.");
					if(audios[0].volume > audios[1].volume) //If source1 is louder than source2, then crossfade from source1.
					{
						audios[1].clip = clip2play;
						StartCoroutine("Crossfade", new object[]{audios[0],audios[1],crossDuration});
						return 1;
					}
					else //If source2 is louder than source1, then crossfade from source2.
					{
						audios[0].clip = clip2play;
						StartCoroutine("Crossfade", new object[]{audios[1],audios[0],crossDuration});
						return 0;
					}
				}
			}
		}
		else //If no AudioSource is playing (silent scene), crossin to the clip
		{
			if(showDebug)Debug.Log("Wasn't playing anything, crossing in.");
			audios[notPlaying].clip = clip2play;
			StartCoroutine("Crossin", new object[]{audios[notPlaying],crossDuration});
		}
		return SOUNDMANAGER_FALSE;
	}
	
	/// <summary>
	/// Crossfade from a1 to a2, for a duration.
	/// </summary>
	private IEnumerator Crossfade (object[] param)
	{
		if(OnCrossInBegin != null)
			OnCrossInBegin();
		OnCrossInBegin = null;
		
		if(OnCrossOutBegin != null)
			OnCrossOutBegin();
		OnCrossOutBegin = null;
		
		AudioSource a1 = param[0] as AudioSource;
		AudioSource a2 = param[1] as AudioSource;
		
		int index1 = GetAudioSourceIndex(a1);
		int index2 = GetAudioSourceIndex(a2);
		
		float duration = (float)param[2];
		
		if(OnSongBegin != null)
			OnSongBegin();
		OnSongBegin = null;
		
		if(index1 == SOUNDMANAGER_FALSE || index2 == SOUNDMANAGER_FALSE)
			Debug.LogWarning("You passed an AudioSource that is not used by the SoundManager May cause erratic behavior");
		outCrossing[index1] = true;
		inCrossing[index2] = true;
		outCrossing[index2] = false;
		inCrossing[index1] = false;
		
		var startTime = Time.realtimeSinceStartup;
	    var endTime = startTime + duration;
		if(!a2.isPlaying) a2.Play();
		float a1StartVolume = a1.volume, a2StartVolume = a2.volume, deltaPercent = 0f, a1DeltaVolume = 0f, a2DeltaVolume = 0f, startMaxMusicVolume = maxMusicVolume, volumePercent = 1f;
	    while (Time.realtimeSinceStartup < endTime) {
			if(startMaxMusicVolume == 0f)
				volumePercent = 1f;
			else
				volumePercent = maxMusicVolume / startMaxMusicVolume;
			
			if(endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
	    		endTime = startTime + duration;
			}			
	        deltaPercent = ((Time.realtimeSinceStartup - startTime) / duration);
			a1DeltaVolume = deltaPercent * a1StartVolume;
			a2DeltaVolume = deltaPercent * (startMaxMusicVolume - a2StartVolume);
	
	        a1.volume = Mathf.Clamp01((a1StartVolume - a1DeltaVolume) * volumePercent);
	        a2.volume = Mathf.Clamp01((a2DeltaVolume + a2StartVolume) * volumePercent);
	       	yield return null;
	    }
		a1.volume = 0f;
		a2.volume = maxMusicVolume;
		a1.Stop();
		lastPlaying = currentPlaying;
		currentPlaying = CheckWhosPlaying();
		
		outCrossing[index1] = false;
		inCrossing[index2] = false;
		
		if(OnSongEnd != null)
		{
			OnSongEnd();
			if(InternalCallback != null)
				OnSongEnd = InternalCallback;
			else
				OnSongEnd = null;
			InternalCallback = null;
		}
		
		if(InternalCallback != null)
			OnSongEnd = InternalCallback;
		InternalCallback = null;
		
		if(OnSongBegin != null)
			OnSongBegin();
		OnSongBegin = null;
		
		SetNextSongInQueue();
	}
	
	/// <summary>
	/// Crossout from a1 for duration.
	/// </summary>
	private IEnumerator Crossout (object[] param)
	{
		if(OnCrossOutBegin != null)
			OnCrossOutBegin();
		OnCrossOutBegin = null;
		
		AudioSource a1 = param[0] as AudioSource;
		float duration = (float)param[1];
		
		int index1 = GetAudioSourceIndex(a1);
		
		if(index1 == SOUNDMANAGER_FALSE)
			Debug.LogWarning("You passed an AudioSource that is not used by the SoundManager May cause erratic behavior");
		outCrossing[index1] = true;
		inCrossing[index1] = false;
		
	    var startTime = Time.realtimeSinceStartup;
	    var endTime = startTime + duration;
		float maxVolume = a1.volume, deltaVolume = 0f, startMaxMusicVolume = maxMusicVolume, volumePercent = 1f;
	    while (Time.realtimeSinceStartup < endTime) {
			if(startMaxMusicVolume == 0f)
				volumePercent = 1f;
			else
				volumePercent = maxMusicVolume / startMaxMusicVolume;
			
	        if(endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
	    		endTime = startTime + duration;
			}
			deltaVolume = ((Time.realtimeSinceStartup - startTime) / duration) * maxVolume;
	
	        a1.volume = Mathf.Clamp01((maxVolume - deltaVolume) * volumePercent);
	       	yield return null;
	    }
		a1.volume = 0f;
		a1.Stop();
		lastPlaying = currentPlaying;
		currentPlaying = CheckWhosPlaying();
		
		outCrossing[index1] = true;
		
		if(OnSongEnd != null)
			OnSongEnd();
		OnSongEnd = null;
		
		if(InternalCallback != null)
			InternalCallback();
		InternalCallback = null;
	}
	
	/// <summary>
	/// Crossout from all AudioSources for a duration.
	/// </summary>
	private IEnumerator CrossoutAll (float duration)
	{		
		if(CheckWhosPlaying() == SOUNDMANAGER_FALSE) yield break;
		
		if(OnCrossOutBegin != null)
			OnCrossOutBegin();
		OnCrossOutBegin = null;
		
		outCrossing[0] = true;
		outCrossing[1] = true;
		inCrossing[0] = false;
		inCrossing[1] = false;
		
		var startTime = Time.realtimeSinceStartup;
	    var endTime = Time.realtimeSinceStartup + duration;
		float a1MaxVolume = volume1, a2MaxVolume = volume2;
		float deltaPercent = 0f, a1DeltaVolume = 0f, a2DeltaVolume = 0f, startMaxMusicVolume = maxMusicVolume, volumePercent = 1f;
		while (Time.realtimeSinceStartup < endTime) {
			if(startMaxMusicVolume == 0f)
				volumePercent = 1f;
			else
				volumePercent = maxMusicVolume / startMaxMusicVolume;
			
			if(endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
	    		endTime = startTime + duration;
			}
			deltaPercent = ((Time.realtimeSinceStartup - startTime) / duration);
			a1DeltaVolume = deltaPercent * a1MaxVolume;
			a2DeltaVolume = deltaPercent * a2MaxVolume;
	
	        volume1 = Mathf.Clamp01((a1MaxVolume - a1DeltaVolume) * volumePercent);
			volume2 = Mathf.Clamp01((a2MaxVolume - a2DeltaVolume) * volumePercent);
	       	yield return null;
	    }
		volume1 = volume2 = 0f;
		audios[0].Stop();
		audios[1].Stop();
		lastPlaying = currentPlaying;
		currentPlaying = CheckWhosPlaying();
		
		outCrossing[0] = false;
		outCrossing[1] = false;
		
		if(OnSongEnd != null)
			OnSongEnd();
		OnSongEnd = null;
		
		if(InternalCallback != null)
			InternalCallback();
		InternalCallback = null;
	}
		
	/// <summary>
	/// Crossin from a1 for duration.
	/// </summary>
	private IEnumerator Crossin (object[] param)
	{
		if(OnCrossInBegin != null)
			OnCrossInBegin();
		OnCrossInBegin = null;
		
		AudioSource a1 = param[0] as AudioSource;
		float duration = (float)param[1];
		
		int index1 = GetAudioSourceIndex(a1);
		
		if(index1 == SOUNDMANAGER_FALSE)
			Debug.LogWarning("You passed an AudioSource that is not used by the SoundManager May cause erratic behavior");
		inCrossing[index1] = true;
		outCrossing[index1] = false;
		
		var startTime = Time.realtimeSinceStartup;
	    var endTime = startTime + duration;
		float a1StartVolume = a1.volume, startMaxMusicVolume = maxMusicVolume, volumePercent = 1f;
		if(!a1.isPlaying)
		{
			a1StartVolume = 0f;
			a1.Play();
		}
		float deltaVolume = 0f;
	    while (Time.realtimeSinceStartup < endTime) {
			if(startMaxMusicVolume == 0f)
				volumePercent = 1f;
			else
				volumePercent = maxMusicVolume / startMaxMusicVolume;
			
	        if(endTime - Time.realtimeSinceStartup > duration)
			{
				startTime = Time.realtimeSinceStartup;
	    		endTime = startTime + duration;
			}
			deltaVolume = ((Time.realtimeSinceStartup - startTime) / duration) * (startMaxMusicVolume - a1StartVolume);
	
	        a1.volume = Mathf.Clamp01((deltaVolume + a1StartVolume) * volumePercent);
	       	yield return null;
	    }
		a1.volume = maxMusicVolume;
		lastPlaying = currentPlaying;
		currentPlaying = CheckWhosPlaying();
		
		inCrossing[index1] = false;
		
		if(OnSongBegin != null)
			OnSongBegin();
		OnSongBegin = null;
		
		if(InternalCallback != null)
			OnSongEnd = InternalCallback;
		InternalCallback = null;
		
		SetNextSongInQueue();
	}
	
	/// <summary>
	/// Determines whether this instance is playing the specified clip, regardless if it's crossing out
	/// </summary>
	/// <param name='clip'>
	/// The clip to check.
	/// </param>
	/// <returns>
	/// The AudioSource that is playing this clip, null if not playing
	/// </returns>
	private AudioSource IsPlaying (AudioClip clip)
	{
		return IsPlaying(clip, true);
	}
	
	/// <summary>
	/// Determines whether this instance is playing the specified clip, with option to ignore if it's crossing out
	/// </summary>
	/// <param name='clip'>
	/// The clip to check.
	/// </param>
	/// <param name='regardlessOfCrossOut'>
	/// Whether or not to ignore if it's crossing out.
	/// </param>
	/// <returns>
	/// The AudioSource that is playing this clip, null if not playing
	/// </returns>
	private AudioSource IsPlaying (AudioClip clip, bool regardlessOfCrossOut)
	{
		for(int i = 0; i < audios.Length; i++)
			if((audios[i].isPlaying && audios[i].clip == clip) && (regardlessOfCrossOut || !outCrossing[i]))
				return audios[i];
		return null;
	}
	
	/// <summary>
	/// Determines whether this instance is playing something.
	/// </summary>
	private bool IsPlaying()
	{
		for(int i = 0; i < audios.Length; i++)
			if(audios[i].isPlaying)
				return true;
		return false;
	}
	
	
	private int GetAudioSourceIndex(AudioSource source)
	{
		for(int i = 0; i < audios.Length; i++)
			if(source == audios[i])
				return i;
		return SOUNDMANAGER_FALSE;
	}
	
	private void StopAllNonSoundConnectionCoroutines()
	{
		StopCoroutine("Crossfade");
		StopCoroutine("Crossout");
		StopCoroutine("Crossin");
		StopCoroutine("CrossoutAll");
	}

	/// <summary>
	/// Plays the SoundConnection according to it's PlayMethod.
	/// </summary>
	private IEnumerator PlaySoundConnection(SoundConnection sc) {
		currentSoundConnection = sc;
		if(sc.soundsToPlay.Count == 0) {
			Debug.LogWarning("The SoundConnection for this level has no sounds to play.  Will cross out.");
			StartCoroutine("CrossoutAll",crossDuration);
			yield break;
		}
		int songPlaying = 0;
		switch(sc.playMethod)
		{
			case PlayMethod.ContinuousPlayThrough:
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				int response = PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// Get the real song length using time samples, especially in the case where the song is already playing.
				float realSongLength;
				if(response != SOUNDMANAGER_FALSE)
					realSongLength = ((sc.soundsToPlay[songPlaying].samples - audios[response].timeSamples)*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);
				else
					realSongLength = (sc.soundsToPlay[songPlaying].samples*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);
				
				// If the cross duration is greater than the length of the song, error and stop [ EDIT ]
				float modifiedCrossDuration = crossDuration;
				if(modifiedCrossDuration > realSongLength)
				{
					Debug.LogError("The cross duration is longer than the track length! Stopping the SoundConnection. TO FIX: Change the cross duration beforehand.");
					StopMusicImmediately();
					yield break;
				}

				// While the clip is playing, wait until the time left is less than the cross duration
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute) && ((((currentSource.clip.samples - currentSource.timeSamples)*1f) / (currentSource.clip.frequency*1f)) > modifiedCrossDuration))
					{
						if(skipSongs)
							break;
						yield return null;
					}
				
				// Then go to the next song.
				movingOnFromSong = true;
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
			}
			break;
			case PlayMethod.ContinuousPlayThroughWithDelay:
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
			}
			break;
			case PlayMethod.ContinuousPlayThroughWithRandomDelayInRange:
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				float randomDelay = Random.Range(sc.minDelay,sc.maxDelay);
				yield return new WaitForSeconds(randomDelay);
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
			}
			break;
			case PlayMethod.OncePlayThrough:
			while(songPlaying < sc.soundsToPlay.Count) {
				currentSongIndex = songPlaying;
				int response = PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// Get the real song length using time samples, especially in the case where the song is already playing.
				float realSongLength;
				if(response != SOUNDMANAGER_FALSE)
					realSongLength = ((sc.soundsToPlay[songPlaying].samples - audios[response].timeSamples)*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);
				else
					realSongLength = (sc.soundsToPlay[songPlaying].samples*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);

				// If the cross duration is greater than the length of the song, error and stop [ EDIT ]
				float modifiedCrossDuration = crossDuration;
				if(modifiedCrossDuration > realSongLength)
				{
					Debug.LogError("The cross duration is longer than the track length! Stopping the SoundConnection. TO FIX: Change the cross duration beforehand.");
					StopMusicImmediately();
					yield break;
				}

				// While the clip is playing, wait until the time left is less than the cross duration
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute) && ((((currentSource.clip.samples - currentSource.timeSamples)*1f) / (currentSource.clip.frequency*1f)) > modifiedCrossDuration))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				if(!skipSongs)
					songPlaying++;
				else
				{
					songPlaying = (songPlaying+skipAmount);
					if(songPlaying < 0) songPlaying = 0;
					skipSongs = false;
					skipAmount = 0;
				}
				if(sc.soundsToPlay.Count <= songPlaying) {
					StartCoroutine("CrossoutAll",crossDuration);
				}
			}
			break;
			case PlayMethod.OncePlayThroughWithDelay:
			while(songPlaying < sc.soundsToPlay.Count) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if(!skipSongs)
					songPlaying++;
				else
				{
					songPlaying = (songPlaying+skipAmount);
					if(songPlaying < 0) songPlaying = 0;
					skipSongs = false;
					skipAmount = 0;
				}
				if(sc.soundsToPlay.Count <= songPlaying) {
					StartCoroutine("CrossoutAll",crossDuration);
				}
			}
			break;
			case PlayMethod.OncePlayThroughWithRandomDelayInRange:
			while(songPlaying < sc.soundsToPlay.Count) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				float randomDelay = Random.Range(sc.minDelay,sc.maxDelay);
				yield return new WaitForSeconds(randomDelay);
				if(!skipSongs)
					songPlaying++;
				else
				{
					songPlaying = (songPlaying+skipAmount);
					if(songPlaying < 0) songPlaying = 0;
					skipSongs = false;
					skipAmount = 0;
				}
				if(sc.soundsToPlay.Count <= songPlaying) {
					StartCoroutine("CrossoutAll",crossDuration);
				}
			}
			break;
			case PlayMethod.ShufflePlayThrough:
			SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				int response = PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// Get the real song length using time samples, especially in the c already playing.
				float realSongLength;
				if(response != SOUNDMANAGER_FALSE)
					realSongLength = ((sc.soundsToPlay[songPlaying].samples - audios[response].timeSamples)*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);
				else
					realSongLength = (sc.soundsToPlay[songPlaying].samples*1f) / (sc.soundsToPlay[songPlaying].frequency*1f);

				// If the cross duration is greater than the length of the song, error and stop [ EDIT ]
				float modifiedCrossDuration = crossDuration;
				if(modifiedCrossDuration > realSongLength)
				{
					Debug.LogError("The cross duration is longer than the track length! Stopping the SoundConnection. TO FIX: Change the cross duration beforehand.");
					StopMusicImmediately();
					yield break;
				}

				// While the clip is playing, wait until the time left is less than the cross duration
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute) && ((((currentSource.clip.samples - currentSource.timeSamples)*1f) / (currentSource.clip.frequency*1f)) > modifiedCrossDuration))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
				if(songPlaying == 0)
					SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			}
			break;
			case PlayMethod.ShufflePlayThroughWithDelay:
			SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				yield return new WaitForSeconds(sc.delay);
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
				if(songPlaying == 0)
					SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			}
			break;
			case PlayMethod.ShufflePlayThroughWithRandomDelayInRange:
			SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			while(Application.isPlaying) {
				currentSongIndex = songPlaying;
				PlayClip(sc.soundsToPlay[songPlaying]);
				movingOnFromSong = false;

				// While the clip is playing, wait until the song is done before moving on with the delay
				currentSource = IsPlaying(sc.soundsToPlay[songPlaying], false);
				if(currentSource != null)
					while(ignoreFromLosingFocus || (currentSource.isPlaying || isPaused || currentSource.mute))
					{
						if(skipSongs)
							break;
						yield return null;
					}

				// Then go to the next song.
				movingOnFromSong = true;
				float randomDelay = Random.Range(sc.minDelay,sc.maxDelay);
				yield return new WaitForSeconds(randomDelay);
				if(!skipSongs)
					songPlaying = (songPlaying+1) % sc.soundsToPlay.Count;
				else
				{
					songPlaying = (songPlaying+skipAmount) % sc.soundsToPlay.Count;
					if(songPlaying < 0) songPlaying += sc.soundsToPlay.Count;
					skipSongs = false;
					skipAmount = 0;
				}
				if(songPlaying == 0)
					SoundManagerTools.Shuffle<AudioClip>(ref sc.soundsToPlay);
			}
			break;
			default:
			Debug.LogError("This SoundConnection has an invalid PlayMethod.");
			break;
		}
		yield break;
	}
	
	private void SetNextSongInQueue()
	{
		if(currentSongIndex == -1)
			return;
		if(currentSoundConnection == null || currentSoundConnection.soundsToPlay == null || currentSoundConnection.soundsToPlay.Count <= 0)
			return;
		int nextSongPlaying = (currentSongIndex+1) % currentSoundConnection.soundsToPlay.Count;
		AudioClip nextSong = currentSoundConnection.soundsToPlay[nextSongPlaying];
		
		int notPlaying = CheckWhosNotPlaying();
		if(audios[notPlaying] == null || (audios[notPlaying].clip != null && !audios[notPlaying].clip.Equals(nextSong)))
			audios[notPlaying].clip = nextSong;
	}
}
