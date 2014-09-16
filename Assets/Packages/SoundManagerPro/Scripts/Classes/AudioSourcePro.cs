using UnityEngine;
using System.Collections;
using System.Linq;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

[AddComponentMenu( "AntiLunchBox/AudioSourcePro" )]
[Serializable]
[ExecuteInEditMode()]
public class AudioSourcePro : MonoBehaviour {
	#region clip info
	public AudioSource audioSource;
	public ClipType clipType = ClipType.AudioClip;
	public AudioSourceAction actionType = AudioSourceAction.None;
	
	public string clipName = "";
	public string groupName = "";
	#endregion
	
	#region private fields
	private bool isBound = false;	
	private bool isVisible;
	#endregion
	
	#region event activation variables
	public bool OnStartActivated = false;
	public bool OnVisibleActivated = false;
	public bool OnInvisibleActivated = false;
	public bool OnCollisionEnterActivated = false;
	public bool OnCollisionExitActivated = false;
	public bool OnTriggerEnterActivated = false;
	public bool OnTriggerExitActivated = false;
	public bool OnMouseEnterActivated = false;
	public bool OnMouseClickActivated = false;
	public bool OnEnableActivated = false;
	public bool OnDisableActivated = false;
	public bool OnCollision2dEnterActivated = false;
	public bool OnCollision2dExitActivated = false;
	public bool OnTriggerEnter2dActivated = false;
	public bool OnTriggerExit2dActivated = false;
	public bool OnParticleCollisionActivated = false;
	#endregion
	
	#region activate/deactivate behavior
	void Awake()
	{
		if(audioSource == null)
			audioSource = gameObject.AddComponent<AudioSource>();
		if(!Application.isPlaying) return;
		if(audioIsValid && (clipType == ClipType.ClipFromSoundManager || clipType == ClipType.ClipFromGroup))
			Play();
		this.isVisible = false;
	}
	
	void OnDestroy()
	{
		if(Application.isPlaying) Unbind();
		if(audioSource != null)
		{
			audioSource.clip = null;
		}
	}
	#endregion
	
	#region binding
	/// <summary>
	/// Bind all events. Fires automatically on activate.
	/// </summary>
	public void Bind()
	{
		if(isBound)
			return;
		
		foreach(AudioSubscription sub in audioSubscriptions)
		{
			if(!sub.isStandardEvent)
				sub.Bind(this);
			else
				BindStandardEvent(sub.standardEvent, true);
		}
		
		isBound = true;
	}
	
	/// <summary>
	/// Unbind all events. Fires automatically on deactivate.
	/// </summary>
	public void Unbind()
	{
		if(!isBound)
			return;

		isBound = false;
		
		foreach(AudioSubscription sub in audioSubscriptions)
		{
			if(!sub.isStandardEvent)
				sub.Unbind();
			else
				BindStandardEvent(sub.standardEvent, false);
		}
	}
	
	/// <summary>
	/// Binds standard events.
	/// </summary>
	public void BindStandardEvent(AudioSourceStandardEvent evt, bool activated)
	{
		switch(evt)
		{
		case AudioSourceStandardEvent.OnStart:
			OnStartActivated = activated;
			break;
		case AudioSourceStandardEvent.OnVisible:
			OnVisibleActivated = activated;
			break;
		case AudioSourceStandardEvent.OnInvisible:
			OnInvisibleActivated = activated;
			break;
		case AudioSourceStandardEvent.OnCollisionEnter:
			OnCollisionEnterActivated = activated;
			break;
		case AudioSourceStandardEvent.OnCollisionExit:
			OnCollisionExitActivated = activated;
			break;
		case AudioSourceStandardEvent.OnTriggerEnter:
			OnTriggerEnterActivated = activated;
			break;
		case AudioSourceStandardEvent.OnTriggerExit:
			OnTriggerExitActivated = activated;
			break;
		case AudioSourceStandardEvent.OnMouseEnter:
			OnMouseEnterActivated = activated;
			break;
		case AudioSourceStandardEvent.OnMouseClick:
			OnMouseClickActivated = activated;
			break;
		case AudioSourceStandardEvent.OnEnable:
			OnEnableActivated = activated;
			break;
		case AudioSourceStandardEvent.OnDisable:
			OnDisableActivated = activated;
			break;
		case AudioSourceStandardEvent.OnCollisionEnter2D:
			OnCollision2dEnterActivated = activated;
			break;
		case AudioSourceStandardEvent.OnCollisionExit2D:
			OnCollision2dExitActivated = activated;
			break;
		case AudioSourceStandardEvent.OnTriggerEnter2D:
			OnTriggerEnter2dActivated = activated;
			break;
		case AudioSourceStandardEvent.OnTriggerExit2D:
			OnTriggerExit2dActivated = activated;
			break;
		case AudioSourceStandardEvent.OnParticleCollision:
			OnParticleCollisionActivated = activated;
			break;
		default:
			break;
		}
	}
	#endregion
	
	#region internal play
	
	/// <summary>
	/// Calls the correct play handler by standard event.
	/// </summary>
	void PlaySoundInternal(AudioSourceStandardEvent evt)
	{
		AudioSubscription sub = FindSubscriptionForEvent(evt);
		
		if(sub == null)
			return;
		
		switch(sub.actionType)
		{
		case AudioSourceAction.Play:
			PlayHandler();
			break;
		case AudioSourceAction.PlayLoop:
			PlayLoopHandler();
			break;
		case AudioSourceAction.PlayCapped:
			PlayCappedHandler(sub.cappedName);
			break;
		case AudioSourceAction.Stop:
			StopHandler();
			break;
		case AudioSourceAction.None:
		default:
			return;
		}
	}
	
	/// <summary>
	/// Finds the subscription for event.
	/// </summary>
	AudioSubscription FindSubscriptionForEvent(AudioSourceStandardEvent evt)
	{
		return audioSubscriptions.Find(delegate(AudioSubscription obj) {
			return (obj.isStandardEvent && obj.standardEvent == evt);
		});
	}
	
	/// <summary>
	/// Handler for AudioSourceAction.Play
	/// </summary>
	void PlayHandler()
	{
		Play();
	}
	
	/// <summary>
	/// Handler for AudioSourceAction.PlayCapped
	/// </summary>
	void PlayCappedHandler(string cappedID)
	{
		SoundManager.PlayCappedSFX(audioSource, clip, cappedID, volume, pitch);
	}
	
	/// <summary>
	/// Handler for AudioSourceAction.PlayLoop
	/// </summary>
	void PlayLoopHandler()
	{
		SoundManager.PlaySFX(audioSource, clip, true, volume, pitch);
	}
	
	/// <summary>
	/// Handler for AudioSourceAction.Stop
	/// </summary>
	void StopHandler()
	{
		Stop();
	}
	#endregion
	
	#region standard events
	void Start()
	{
		if(!Application.isPlaying) 
			return;
		
		if(!isBound && componentsAreValid && audioIsValid)
		{
			Bind();
		}
		
		if (OnStartActivated) 
		{
			PlaySoundInternal(AudioSourceStandardEvent.OnStart);
		}
	}
	
	void OnBecameVisible() 
	{
		if (OnVisibleActivated && !isVisible) 
		{
			isVisible = true;
			PlaySoundInternal(AudioSourceStandardEvent.OnVisible);
		}
	}
	
	void OnBecameInvisible() 
	{
		if (OnInvisibleActivated) 
		{
			isVisible = false;
			PlaySoundInternal(AudioSourceStandardEvent.OnInvisible);
		}
	}
	
	void OnEnable()
	{
		if(!Application.isPlaying) 
			return;
		
		if(!isBound && componentsAreValid && audioIsValid)
		{
			Bind();
		}
		
		if (OnEnableActivated) 
		{
			PlaySoundInternal(AudioSourceStandardEvent.OnEnable);
		}
	}
	
	void OnDisable() 
	{
		if(!Application.isPlaying)
			return;
		else
		{
			if (OnDisableActivated) 
			{
				PlaySoundInternal(AudioSourceStandardEvent.OnDisable);
			}
			Unbind();
		}		
	}
	
	#if !(UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2)
	void OnTriggerEnter2D(Collider2D other) 
	{
		if (!OnTriggerEnter2dActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnTriggerEnter2D);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << other.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(other.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(other.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnTriggerEnter2D);
	}

	void OnTriggerExit2D(Collider2D other) 
	{
		if (!OnTriggerExit2dActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnTriggerExit2D);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << other.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(other.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(other.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnTriggerExit2D);
	}

	void OnCollisionEnter2D(Collision2D collision) 
	{
		if (!OnCollision2dEnterActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnCollisionEnter2D);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << collision.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(collision.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(collision.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnCollisionEnter2D);
	}
	
	void OnCollisionExit2D(Collision2D collision) 
	{
		if (!OnCollision2dExitActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnCollisionExit2D);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << collision.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(collision.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(collision.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnCollisionExit2D);
	}
	#endif

	void OnCollisionEnter(Collision collision) 
	{
		if (!OnCollisionEnterActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnCollisionEnter);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << collision.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(collision.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(collision.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnCollisionEnter);
	}
	
	void OnCollisionExit(Collision collision) 
	{
		if (!OnCollisionExitActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnCollisionExit);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << collision.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(collision.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(collision.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnCollisionExit);
	}
	
	void OnTriggerEnter(Collider other) 
	{
		if (!OnTriggerEnterActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnTriggerEnter);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << other.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(other.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(other.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnTriggerEnter);
	}

	void OnTriggerExit(Collider other) 
	{
		if (!OnTriggerExitActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnTriggerExit);
		if(sub == null) 
			return;
	
		if(sub.filterLayers && (sub.layerMask & 1 << other.gameObject.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(other.gameObject.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(other.gameObject.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnTriggerExit);
	}
	
	void OnParticleCollision(GameObject other) 
	{
		if (!OnParticleCollisionActivated) 
			return;
		
		AudioSubscription sub = FindSubscriptionForEvent(AudioSourceStandardEvent.OnParticleCollision);
		if(sub == null) 
			return;
		
		if(sub.filterLayers && (sub.layerMask & 1 << other.layer) != 0)
			return;
		if(sub.filterTags && !sub.tags.Contains(other.tag))
			return;
		if(sub.filterNames && !sub.names.Contains(other.name))
			return;
		
		PlaySoundInternal(AudioSourceStandardEvent.OnParticleCollision);
	}
	
	void OnMouseEnter() 
	{
		if (OnMouseEnterActivated) 
		{
			PlaySoundInternal(AudioSourceStandardEvent.OnMouseEnter);
		}
	}
	
	void OnMouseDown() 
	{
		if (OnMouseClickActivated) 
		{
			PlaySoundInternal(AudioSourceStandardEvent.OnMouseClick);
		}
	}	
	#endregion
	
	#region valiidation
	public bool componentsAreValid
	{
		get {
			for(int i = 0; i < audioSubscriptions.Count; i++)
				if(!audioSubscriptions[i].isStandardEvent && !audioSubscriptions[i].componentIsValid)
					return false;
			return true;
		}
	}
	
	public bool audioIsValid
	{
		get {	
			bool valid = false;
			switch(clipType)
			{
			case ClipType.AudioClip:
				valid = (audioSource.clip != null);
				break;
			case ClipType.ClipFromSoundManager:
				valid = SoundManager.ClipNameIsValid(clipName);
				break;
			case ClipType.ClipFromGroup:
				valid = SoundManager.GroupNameIsValid(groupName);
				break;
			default:
				break;
			}
			if(!valid)
				Debug.LogError("AudioClip setup is not valid.");
			return valid;
		}
	}
	#endregion	
	
	#region audiosource variable access
	public bool bypassEffects {
		get {
			return audioSource.bypassEffects;
		} set {
			audioSource.bypassEffects = value;
		}
	}
	
	public AudioClip clip {
		get {
			switch(clipType)
			{
			case ClipType.ClipFromSoundManager:
				return SoundManager.Load(clipName);
			case ClipType.ClipFromGroup:
				return SoundManager.LoadFromGroup(groupName);
			case ClipType.AudioClip:
			default:
				return audioSource.clip;
			}
		} set {
			switch(clipType)
			{
			case ClipType.ClipFromSoundManager:
			case ClipType.ClipFromGroup:
				Debug.LogWarning("Assigning a clip only works for the AudioClip ClipType. It is automatically changed for you.");
				clipType = ClipType.AudioClip;
				audioSource.clip = value;
				break;
			case ClipType.AudioClip:
			default:
				audioSource.clip = value;
				break;
			}
		}
	}
	
	public float dopplerLevel {
		get {
			return audioSource.dopplerLevel;
		} set {
			audioSource.dopplerLevel = value;
		}
	}
	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1
	public bool ignoreListenerPause {
		get {
			return audioSource.ignoreListenerPause;
		} set {
			audioSource.ignoreListenerPause = value;
		}
	}
#endif
	
	public bool ignoreListenerVolume {
		get {
			return audioSource.ignoreListenerVolume;
		} set {
			audioSource.ignoreListenerVolume = value;
		}
	}
	
	public bool isPlaying {
		get {
			return audioSource.isPlaying;
		}
	}
	
	public bool loop {
		get {
			return audioSource.loop;
		} set {
			audioSource.loop = value;
		}
	}
	
	public float maxDistance {
		get {
			return audioSource.maxDistance;
		} set {
			audioSource.maxDistance = value;
		}
	}
	
	public float minDistance {
		get {
			return audioSource.minDistance;
		} set {
			audioSource.minDistance = value;
		}
	}
	
	public bool mute {
		get {
			return audioSource.mute;
		} set {
			audioSource.mute = value;
		}
	}
	
	public float pan {
		get {
			return audioSource.pan;
		} set {
			audioSource.pan = value;
		}
	}
	
	public float panLevel {
		get {
			return audioSource.panLevel;
		} set {
			audioSource.panLevel = value;
		}
	}
	
	public float pitch {
		get {
			return audioSource.pitch;
		} set {
			audioSource.pitch = value;
		}
	}
	
	public bool playOnAwake {
		get {
			return audioSource.playOnAwake;
		} set {
			audioSource.playOnAwake = value;
		}
	}
	
	public int priority {
		get {
			return audioSource.priority;
		} set {
			audioSource.priority = value;
		}
	}
	
	public AudioRolloffMode rolloffMode {
		get {
			return audioSource.rolloffMode;
		} set {
			audioSource.rolloffMode = value;
		}
	}
	
	public float spread {
		get {
			return audioSource.spread;
		} set {
			audioSource.spread = value;
		}
	}
	
	public float time {
		get {
			return audioSource.time;
		} set {
			audioSource.time = value;
		}
	}
	
	public int timeSamples {
		get {
			return audioSource.timeSamples;
		} set {
			audioSource.timeSamples = value;
		}
	}
	
	public AudioVelocityUpdateMode velocityUpdateMode {
		get {
			return audioSource.velocityUpdateMode;
		} set {
			audioSource.velocityUpdateMode = value;
		}
	}
	
	public float volume {
		get {
			return audioSource.volume;
		} set {
			audioSource.volume = value;
		}
	}
	#endregion
	
	#region audiosource function access
	public void GetOutputData(float[] samples, int channel)
	{
		audioSource.GetOutputData(samples, channel);
	}
	
	public void GetSpectrumData(float[] samples, int channel, FFTWindow window)
	{
		audioSource.GetSpectrumData(samples, channel, window);
	}
	
	public void Pause()
	{
		audioSource.Pause();
	}
	
	public void Play(ulong delay=0)
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
		case ClipType.ClipFromGroup:
			SoundManager.PlaySFX(audioSource, clip, loop, volume, pitch);
			break;
		case ClipType.AudioClip:
		default:
			audioSource.Play(delay);
			break;
		}
	}
	
#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1	
	public void PlayDelayed(float delay)
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
			StartCoroutine(PlayDelayedHelper(delay, clipName));
			break;
		case ClipType.ClipFromGroup:
			StartCoroutine(PlayDelayedHelper(delay, groupName));
			break;
		case ClipType.AudioClip:
		default:
			audioSource.PlayDelayed(delay);
			break;
		}
	}
	
	IEnumerator PlayDelayedHelper(float delay, string nameValue)
	{
		yield return new WaitForSeconds(delay);
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
			SoundManager.PlaySFX(audioSource, SoundManager.Load(nameValue), loop, volume, pitch);
			break;
		case ClipType.ClipFromGroup:
			SoundManager.PlaySFX(audioSource, SoundManager.LoadFromGroup(nameValue), loop, volume, pitch);
			break;
		default:
			break;
		}		
	}
#endif
	
	public void PlayOneShot(AudioClip clip, float volumeScale)
	{
		audioSource.PlayOneShot(clip, volumeScale);
	}

#if !UNITY_3_5 && !UNITY_4_0 && !UNITY_4_0_1	
	public void PlayScheduled(double time)
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
		case ClipType.ClipFromGroup:
			Debug.LogError("PlayScheduled does not work for this ClipType yet. Use ClipType.AudioClip");
			break;
		case ClipType.AudioClip:
		default:
			audioSource.PlayScheduled(time);
			break;
		}
	}
	
	public void SetScheduledEndTime(double time)
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
		case ClipType.ClipFromGroup:
			Debug.LogError("SetScheduledEndTime does not work for this ClipType yet. Use ClipType.AudioClip");
			break;
		case ClipType.AudioClip:
		default:
			audioSource.SetScheduledEndTime(time);
			break;
		}
	}
	
	public void SetScheduledStartTime(double time)
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
		case ClipType.ClipFromGroup:
			Debug.LogError("SetScheduledStartTime does not work for this ClipType yet. Use ClipType.AudioClip");
			break;
		case ClipType.AudioClip:
		default:
			audioSource.SetScheduledStartTime(time);
			break;
		}
	}
#endif
	
	public void Stop()
	{
		switch(clipType)
		{
		case ClipType.ClipFromSoundManager:
		case ClipType.ClipFromGroup:
			SoundManager.StopSFXObject(audioSource);
			break;
		case ClipType.AudioClip:
		default:
			audioSource.Stop();
			break;
		}
	}
	
	public static void PlayClipAtPoint(AudioClip clip, Vector3 position, float volume=1f)
	{
		SoundManager.PlaySFX(clip, volume, 1f, position);
	}
	#endregion
	
	#region editor related - DO NOT MODIFY
	public bool ShowEditor3D = false;
	public bool ShowEditor2D = false;
	public bool ShowEventTriggers = false;
	public int numSubscriptions = 0;
	public List<AudioSubscription> audioSubscriptions = new List<AudioSubscription>();
	#endregion
}

public enum ClipType
{
	AudioClip,
	ClipFromSoundManager,
	ClipFromGroup
}

public enum AudioSourceAction
{
	None,
	Play,
	PlayLoop,
	PlayCapped,
	Stop
}

public enum AudioSourceStandardEvent
{
	OnStart,
	OnVisible,
	OnInvisible,
	OnCollisionEnter,
	OnCollisionExit,
	OnTriggerEnter,
	OnTriggerExit,
	OnMouseEnter,
	OnMouseClick,
	OnEnable,
	OnDisable,
	OnCollisionEnter2D,
	OnCollisionExit2D,
	OnTriggerEnter2D,
	OnTriggerExit2D,
	OnParticleCollision
}
