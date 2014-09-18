//----------------------------------------------
//            Audio Manager
// Copyright © 2014 Tek-Wise Studios
//----------------------------------------------

using UnityEngine;
using System.Diagnostics;
using System.Threading;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class AudioTrack : MonoBehaviour {
	private float delay;
	private bool isPlaying;
	private float fadeIn;
	private float fadeOut;
	private bool forcePlayThrough;
	private bool loop;
	private bool mute;
	private bool persist;
	private float pitch;
	private string trackName;
	private float trackLength;
	private float trackTime;
	private float volume;
	private float fadeOutTime;

	// private
	private bool isFadingOut;
	private bool isMuted = false;
	private bool isPaused;
    private float trackVolume;
	private bool isBeingStopped = false;
	
	public float Delay {
		get {
			return this.delay;
		} private set {
			this.delay = value;
		}
	}
	
	public bool IsPlaying {
		get {
			return this.isPlaying;
		} private set {
			this.isPlaying = value;
		}
	}

	public bool IsPaused {
		get {
			return this.isPaused;
		} private set {
			this.isPaused = value;
		}
	}
	
	public float FadeIn {
		get {
			return this.fadeIn;
		} private set {
			this.fadeIn = value;
		}
	}
	
	public float FadeOut {
		get {
			return this.fadeOut;
		} private set {
			this.fadeOut = value;
		}
	}

	public bool ForcePlayThrough {
		get {
			return this.forcePlayThrough;
		} private set {
			this.forcePlayThrough = value;
		}
	}
	
	public bool Loop {
		get {
			return this.loop;
		} private set {
			this.loop = value;
		}
	}

	public bool Mute {
		get {
			return this.mute;
		} set {
			this.mute = value;
		}
	}
	
	public bool Persist {
		get {
			return this.persist;
		} private set {
			this.persist = value;
		}
	}
	
	public float Pitch {
		get {
			return this.pitch;
		} private set {
			this.pitch = value;
		}
	}
	
	public float TrackLength {
		get {
			return this.trackLength;
		} private set {
			this.trackLength = value;
		}
	}
	
	public string TrackName {
		get {
			return this.trackName;
		} private set {
			this.trackName = value;
		}
	}
	
	public float TrackTime {
		get {
			return this.trackTime;
		} private set {
			this.trackTime = value;
		}
	}
	
	public float Volume {
		get {
			return this.volume;
		} private set {
			this.volume = value;
		}
	}
	
	void CheckLoop() {
		if (this.Loop == true) {
			this.StartCoroutine("PlayTrack");
		}
		else {
			this.transform.position = default(Vector3);
			this.gameObject.SetActive(false);
		}
	}
	
	public void Play() {
		this.StartCoroutine ("PlayTrack");
		this.isBeingStopped = false;
		if(this.IsPaused == true) this.IsPaused = false;
	}

	public void Pause() {
		this.StopCoroutine("PlayTrack");
		this.StopCoroutine("StartFadeIn");
		this.StopCoroutine("StartFadeOut");
		this.GetComponent<AudioSource>().Pause();
		this.IsPlaying = false;
		this.IsPaused = true;
	}

	public void UnPause() {
		this.StartCoroutine("PlayTrack");
		this.isBeingStopped = false;
		this.IsPaused = false;
	}

	public void Stop() {
		this.StopCoroutine("PlayTrack");
		this.StopCoroutine("StartFadeIn");
		this.StopCoroutine("StartFadeOut");
		this.IsPlaying = false;
	}

	public void StopWithFade(float fadeTime) {
		this.StopCoroutine("StartFadeIn");
		this.StartCoroutine("StartFadeOut", fadeTime);
		this.isBeingStopped = true;
	}

	IEnumerator PlayTrack() {
		if(this.isPlaying == true) yield break;
		
		#region Delay
		float time_elapsed_delay = 0.0f;
		
		while(time_elapsed_delay < this.Delay) {
			time_elapsed_delay += IgnoreTimeScale.DeltaTime;
			yield return null;
		}
		#endregion
		
		this.GetComponent<AudioSource>().Play();
		this.StartCoroutine("StartFadeIn");
		this.IsPlaying = this.GetComponent<AudioSource>().isPlaying;
		
		// Keep track of playing the song.
		while(this.isPlaying == true) {
			this.IsPlaying = this.GetComponent<AudioSource>().isPlaying;
			this.TrackTime = this.GetComponent<AudioSource>().time;
			
			if(this.Mute == true && this.isMuted == false) {
				this.GetComponent<AudioSource>().mute = true;
				this.isMuted = true;
			} else if(this.Mute == false && this.isMuted == true) {
				this.GetComponent<AudioSource>().mute = false;
				this.isMuted = false;
			}

			if(this.TrackTime >= this.fadeOutTime && this.isFadingOut == false) {
				this.StartCoroutine("StartFadeOut", 0.0f);
			}
			
			yield return null;
		}
	}
	
	IEnumerator StartFadeIn() {
		#region Fade in
		float time_elapsed_fade_in = 0.0f;
		while (time_elapsed_fade_in < this.FadeIn) {
			
			if (this.FadeIn != 0.0f && this.Mute == false) {
				this.GetComponent<AudioSource>().volume = Mathf.Lerp(0.0f, this.Volume, time_elapsed_fade_in / this.FadeIn);
			}

			time_elapsed_fade_in += IgnoreTimeScale.DeltaTime;
			yield return null;
		}
		
		this.GetComponent<AudioSource>().volume = this.Volume;
		#endregion
	}
	
	IEnumerator StartFadeOut(float fadeTime) {
		if(this.isFadingOut == true) yield break;
		#region Fade Out
		this.isFadingOut = true;
		
		float time_elapsed_fade_out = 0.0f;
		
		while(this.TrackTime < this.TrackLength && this.isBeingStopped == false) {
			
			if (this.FadeOut != 0.0f) {
				this.GetComponent<AudioSource>().volume = Mathf.Lerp(this.Volume, 0.0f, time_elapsed_fade_out / this.FadeOut);
			}
			
			time_elapsed_fade_out += IgnoreTimeScale.DeltaTime;
			this.TrackTime += IgnoreTimeScale.DeltaTime;
			yield return null;
		}
		
		while(time_elapsed_fade_out < fadeTime && this.isBeingStopped == true) {
			this.GetComponent<AudioSource>().volume = Mathf.Lerp(this.GetComponent<AudioSource>().volume, 0.0f, time_elapsed_fade_out / fadeTime);
			time_elapsed_fade_out += IgnoreTimeScale.DeltaTime;
			this.TrackTime += IgnoreTimeScale.DeltaTime;
			yield return null;
		}
		
		#endregion
		
		this.isFadingOut = false;
		this.GetComponent<AudioSource>().volume = 0.0f;
		this.IsPlaying = false;
		this.TrackTime = 0.0f;
		if(this.isBeingStopped == false) this.CheckLoop(); else this.gameObject.SetActive(false);
	}
	
	public void SetTrack(AudioInfo audioItem) {
		this.GetComponent<AudioSource>().audio.clip = audioItem.trackClip;
		this.GetComponent<AudioSource>().volume = 0.0f;
		this.Delay = audioItem.startDelay;
		this.FadeIn = audioItem.fadeIn;
		this.FadeOut = audioItem.fadeOut;
		this.ForcePlayThrough = audioItem.forcePlayThrough;
		this.Loop = audioItem.enableLoop;
		this.Persist = audioItem.persist;
		this.Pitch = audioItem.pitch;
		this.GetComponent<AudioSource>().pitch = this.Pitch;
		this.TrackLength = audioItem.trackClip.length;
		this.TrackName = audioItem.trackName;
		this.Volume = audioItem.customVolume;
        this.trackVolume = this.Volume;
		
		this.name = "Audio Track - " + this.TrackName;
		this.fadeOutTime = this.TrackLength - this.FadeOut;
	}

	public void UpdateVolume(float volume) {
		this.Volume = this.trackVolume * volume;
	}
}