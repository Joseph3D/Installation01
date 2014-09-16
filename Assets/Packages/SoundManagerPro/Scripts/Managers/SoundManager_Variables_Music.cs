using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {

	// List of soundconnections
	public List<SoundConnection> soundConnections = new List<SoundConnection>();

	[SerializeField]
	public AudioSource[] audios;

	public string currentLevel;
	public SoundConnection currentSoundConnection;
	private AudioSource currentSource;
	public float crossDuration = 5f;
	public bool showDebug = true;
	public bool offTheBGM = false;

	private int currentPlaying = 0;
	private int lastPlaying = -1;

	private bool silentLevel = false;

	public bool isPaused = false;
	private bool skipSongs = false;
	private int skipAmount = 0;

	private bool[] inCrossing = new bool[] { false, false };
	private bool[] outCrossing = new bool[] { false, false };
	public bool movingOnFromSong = false;

	float lastLevelLoad = 0f;

	public const int SOUNDMANAGER_FALSE = -1;

	public delegate void SongCallBack();
	public SongCallBack OnSongEnd;
	public SongCallBack OnSongBegin;
	public SongCallBack OnCrossOutBegin;
	public SongCallBack OnCrossInBegin;	
	private SongCallBack InternalCallback;
	
	private int currentSongIndex = -1;

	private bool ignoreFromLosingFocus = false;
	private bool ignoreLevelLoad = false;

	public float volume1 {
		get {
			return audios[0].volume;
		} set {
			audios[0].volume = value;
		}
	}
	public float volume2 {
		get{
			return audios[1].volume;
		} set {
			audios[1].volume = value;
		}
	}

	public float maxMusicVolume {
		get{
			return _maxMusicVolume;
		} set {
			_maxMusicVolume = value;
		}
	}
	private float _maxMusicVolume = 1f;

	public float maxVolume {
		get{
			return _maxVolume;
		} set {
			_maxVolume = value;
		}
	}
	private float _maxVolume = 1f;

	public bool mutedMusic {
		get {
			return _mutedMusic;
		} set {
			audios[0].mute = audios[1].mute = value;
			_mutedMusic = value;
		}
	}
	private bool _mutedMusic = false;

	public bool muted {
		get {
			return (mutedMusic || mutedSFX);
		} set {
			mutedMusic = mutedSFX = value;
		}
	}

	private bool crossingIn {
		get {
			return (inCrossing[0] || inCrossing[1]);
		}
	}

	private bool crossingOut {
		get {
			return (outCrossing[0] || outCrossing[1]);
		}
	}
}
