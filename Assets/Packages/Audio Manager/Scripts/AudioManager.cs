//----------------------------------------------
//            Audio Manager
// Copyright © 2014 Tek-Wise Studios
//----------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Audio Manager, manages all audio tracks.
/// With a centralized and organized list,
/// you can easily control your music and sound tracks!
/// </summary>
public sealed class AudioManager : MonoBehaviour {
	#region Singleton Instance
	private static AudioManager _instance = null;
	private GameObject _persistentParent;
	private GameObject _nonPersistentParent;
	
    /// <summary>
    /// Gets the current Audio Manager instance.
    /// </summary>
	public static AudioManager AudioInstance {
		get {
			if (AudioManager._instance == null) {
				AudioManager audioManager = Object.FindObjectOfType<AudioManager>();
				
				if (audioManager != null) {
					AudioManager._instance = audioManager;
				}
				
				if (AudioManager._instance == null) {
					Debug.LogError(typeof(AudioManager).ToString() + " - Instance: Could not find Audio Manager in scene!");
				}
			}
			
			return AudioManager._instance;
		}
	}
	#endregion

	[SerializeField]
	GameObject audioTrack; // Required
	[SerializeField]
	public float musicVolume = 1.0f;
	[SerializeField]
	public float soundVolume = 1.0f;
	[SerializeField]
	public AudioInfo[] musicList = new AudioInfo[0];
	[SerializeField]
	public AudioInfo[] soundList = new AudioInfo[0];
	[SerializeField]
	bool showDebug = false;
	
	private List<GameObject> musicPool = new List<GameObject>();
	private List<GameObject> soundPool = new List<GameObject>();
	private List<GameObject> forcedPool = new List<GameObject>();

	void Awake() {
		this.CreateMusicPool();
		this.CreateSoundPool();
		this.CheckIfChild();

		if(this.IsAudioManagerInScene() == true) {
			AudioManager.AudioInstance.ConnectMusicPool(this.musicPool, this.musicList);
			AudioManager.AudioInstance.ConnectSoundPool(this.soundPool, this.soundList);
			AudioManager.AudioInstance.TrimLists();
			this.DestoryParents();
			DestroyImmediate(this.gameObject);
		} else {
            DontDestroyOnLoad(this.gameObject);
		}
	}

	private bool IsAudioManagerInScene() {
		AudioManager[] possibleInstances = GameObject.FindObjectsOfType<AudioManager>();
		
		if(possibleInstances.Length > 1) {
			return true;
		} else {
			return false;
		}
	}

	void CreateMusicPool() {
		foreach (AudioInfo audioItem in this.musicList) {
			if (audioItem.trackClip != null) {
				GameObject newTrack;
				
				if (audioItem.overrideAudioTrack != null) {
					newTrack = Instantiate(audioItem.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
				}
				else {
					newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
				}
				
				if (newTrack != null) {
					if (newTrack.GetComponent<AudioTrack>() != null) {
						if(audioItem.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
						newTrack.GetComponent<AudioTrack>().SetTrack(audioItem);
                        newTrack.GetComponent<AudioTrack>().UpdateVolume(this.musicVolume);
						newTrack.SetActive(false);
						this.musicPool.Add(newTrack);
					}
					else {
						if(this.showDebug) {
							Debug.LogWarning(typeof(AudioManager).ToString() + " - Music Pool: Overriden Track Prefab does not contain component AudioTrack for " + audioItem.trackName + "!");
						}
					}
				}
			}
		}
	}

	void AddToMusicPool(string trackName, Vector3 position, GameObject parent) {
		foreach(AudioInfo a in this.musicList) {
			if(string.Equals(a.trackName, trackName) == true) {
				if(a.maxInstances == -1) {
					if(a.trackClip != null) {
						GameObject newTrack;
						
						if (a.overrideAudioTrack != null) {
							newTrack = Instantiate(a.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
						}
						else {
							newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
						}
						
						if (newTrack != null) {
							if (newTrack.GetComponent<AudioTrack>() != null) {
								if(a.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
								newTrack.GetComponent<AudioTrack>().SetTrack(a);
								newTrack.GetComponent<AudioTrack>().UpdateVolume(this.soundVolume);
								newTrack.SetActive(true);
								newTrack.GetComponent<AudioTrack>().Play();
								this.musicPool.Add(newTrack);
							} else {
								if(this.showDebug == true) {
									Debug.LogWarning(typeof(AudioManager).ToString() + " - Music Pool: Audio Track Prefab does not contain component AudioTrack for " + a.trackName + "!");
								}
							}
						}
					}
				} else if(this.GetMusicCount(trackName) < a.maxInstances) {
					if(a.trackClip != null) {
						GameObject newTrack;
						
						if (a.overrideAudioTrack != null) {
							newTrack = Instantiate(a.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
						}
						else {
							newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
						}

						if (newTrack != null) {
							if (newTrack.GetComponent<AudioTrack>() != null) {
								if(a.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
								newTrack.GetComponent<AudioTrack>().SetTrack(a);
                                newTrack.GetComponent<AudioTrack>().UpdateVolume(this.soundVolume);
								newTrack.SetActive(true);
								newTrack.GetComponent<AudioTrack>().Play();
								this.musicPool.Add(newTrack);
							} else {
								if(this.showDebug == true) {
									Debug.LogWarning(typeof(AudioManager).ToString() + " - Music Pool: Audio Track Prefab does not contain component AudioTrack for " + a.trackName + "!");
								}
							}
						}
					}
				}
			}
		}
	}
	
    /// <summary>
    /// Plays a specific music track.
    /// </summary>
    /// <param name="trackName"></param>
	public void PlayMusic(string trackName) {
		this.PlayMusic(trackName, default(Vector3));
	}
	
    /// <summary>
    /// Plays a specific music track
    /// with a specific location (3D sound).
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="position"></param>
	public void PlayMusic(string trackName, Vector3 position) {
		this.PlayMusic(trackName, position, null);
	}
	
    /// <summary>
    /// Plays a specific music track
    /// with a specific location (3D sound) and parent.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
	public void PlayMusic(string trackName, Vector3 position, GameObject parent) {
		foreach (GameObject pooledTrack in this.musicPool) {
			if(pooledTrack == null) return;
			if (string.Equals(pooledTrack.GetComponent<AudioTrack>().TrackName, trackName) == true) {
				if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == false) {
					if(parent != null) {
						pooledTrack.transform.parent = parent.transform;
					}
					
					pooledTrack.transform.position = position;
					pooledTrack.SetActive(true);
					pooledTrack.GetComponent<AudioTrack>().Play();
					
					if(pooledTrack.GetComponent<AudioTrack>().ForcePlayThrough == true && parent == null) {
						this.forcedPool.Add(pooledTrack);
						this.ConnectToPersistantParent(pooledTrack);
					}
					
					return;
				}
			}
		}

		if(this.GetMusicCount(trackName) > 0) {
			this.AddToMusicPool(trackName, position, parent);
			return;
		}

		if(this.showDebug == true) {
			Debug.LogWarning(typeof(AudioManager).ToString() + " - PlayMusic: Could not find music track '" + trackName + "'!");
		}
	}

    /// <summary>
    /// Pauses a specific music track.
    /// </summary>
    /// <param name="trackName"></param>
	public void PauseMusic(string trackName) {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().Pause();
				return;
			}
		}
	}

    /// <summary>
    /// Unpauses a specific music track.
    /// </summary>
    /// <param name="trackName"></param>
	public void UnPauseMusic(string trackName) {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().UnPause();
				return;
			}
		}
	}

    /// <summary>
    /// Pauses all currently playing music tracks.
    /// </summary>
	public void PauseAllMusic() {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				pooledTrack.GetComponent<AudioTrack>().Pause();
			}
		}
	}

    /// <summary>
    /// Unpauses all currently paused music.
    /// </summary>
	public void UnPauseAllMusic() {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPaused == true) {
				pooledTrack.GetComponent<AudioTrack>().UnPause();
			}
		}
	}

    /// <summary>
    /// Stops a specific music track.
    /// </summary>
    /// <param name="trackName"></param>
	public void StopMusic(string trackName) {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().Stop();
				return;
			}
		}
	}

    /// <summary>
    /// Stops all music that is currently playing.
    /// </summary>
	public void StopAllMusic() {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				pooledTrack.GetComponent<AudioTrack>().Stop();
			}
		}
	}

	/// <summary>
	/// Stops the music with fade.
	/// </summary>
	/// <param name="trackName">Track name.</param>
	public void StopMusicWithFade(string trackName, float fadeTime) {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				if(string.Equals(pooledTrack.GetComponent<AudioTrack>().TrackName, trackName) == true) {
					pooledTrack.GetComponent<AudioTrack>().StopWithFade(fadeTime);
					return;
				}
			}
		}
	}

	/// <summary>
	/// Stops all music with fade.
	/// </summary>
	/// <param name="fadeTime">Fade time.</param>
	public void StopAllMusicWithFade(float fadeTime) {
		foreach(GameObject pooledTrack in this.musicPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				pooledTrack.GetComponent<AudioTrack>().StopWithFade(fadeTime);
			}
		}
	}

    /// <summary>
    /// Mutes all music tracks.
    /// </summary>
	public void MuteMusic() {
		foreach(GameObject pooledMusic in this.musicPool) {
			pooledMusic.GetComponent<AudioTrack>().Mute = true;
		}
	}

    /// <summary>
    /// Unmutes all music tracks.
    /// </summary>
	public void UnMuteMusic() {
		foreach(GameObject pooledMusic in this.musicPool) {
			pooledMusic.GetComponent<AudioTrack>().Mute = false;
		}
	}

    /// <summary>
    /// This allows you to change the colume of the music
    /// clips. the value passed must be between 0 and 1.
    /// </summary>
    /// <param name="volume"></param>
    public void MusicVolume(float volume) {
        foreach (GameObject pooledTrack in this.musicPool) {
            pooledTrack.GetComponent<AudioTrack>().UpdateVolume(volume);
        }
    }

    /// <summary>
    /// Returns an array of string for the current music list.
    /// </summary>
    /// <returns></returns>
    public string[] GetMusicList() {
        List<string> music = new List<string>();

        foreach (AudioInfo trackInfo in this.musicList) {
            music.Add(trackInfo.trackName);
        }

        return music.ToArray();
    }

    int GetMusicCount(string trackName) {
        int count = 0;

        foreach (GameObject m in this.musicPool) {
            if (m != null) {
                if (string.Equals(m.GetComponent<AudioTrack>().TrackName, trackName) == true) {
                    count++;
                }
            }
        }

        return count;
    }

    void CreateSoundPool() {
        foreach (AudioInfo audioItem in this.soundList) {
            if (audioItem.trackClip != null) {
                GameObject newTrack;

                if (audioItem.overrideAudioTrack != null) {
                    newTrack = Instantiate(audioItem.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
                } else {
                    newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
                }

                if (newTrack != null) {

                    if (newTrack.GetComponent<AudioTrack>() != null) {
                        newTrack.GetComponent<AudioTrack>().SetTrack(audioItem);
                        newTrack.GetComponent<AudioTrack>().UpdateVolume(this.soundVolume);
                        if (audioItem.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
                        newTrack.SetActive(false);
                        this.soundPool.Add(newTrack);
                    } else {
                        if(this.showDebug == true) {
							Debug.LogWarning(typeof(AudioManager).ToString() + " - Sound Effects Pool: Audio Track Prefab does not contain component AudioTrack for " + audioItem.trackName + "!");
						}
                    }
                }
            }
        }
    }

    void AddToSoundPool(string trackName, Vector3 position, GameObject parent) {
        foreach (AudioInfo a in this.soundList) {
            if (string.Equals(a.trackName, trackName) == true) {
				if(a.maxInstances == -1) {
					if (a.trackClip != null) {
						GameObject newTrack;
						
						if (a.overrideAudioTrack != null) {
							newTrack = Instantiate(a.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
						} else {
							newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
						}
						
						if (newTrack != null) {
							
							if (newTrack.GetComponent<AudioTrack>() != null) {
								newTrack.GetComponent<AudioTrack>().SetTrack(a);
								newTrack.GetComponent<AudioTrack>().UpdateVolume(this.soundVolume);
								if (a.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
								newTrack.SetActive(true);
								newTrack.GetComponent<AudioTrack>().Play();
								this.soundPool.Add(newTrack);
							} else {
								if(this.showDebug == true) {
									Debug.LogWarning(typeof(AudioManager).ToString() + " - Sound Effects Pool: Overriden Track Prefab does not contain component AudioTrack for " + a.trackName + "!");
								}
							}
						}
					}
				} else if (this.GetSoundCount(trackName) < a.maxInstances) {
                    if (a.trackClip != null) {
                        GameObject newTrack;

                        if (a.overrideAudioTrack != null) {
                            newTrack = Instantiate(a.overrideAudioTrack, default(Vector3), Quaternion.identity) as GameObject;
                        } else {
                            newTrack = Instantiate(this.audioTrack, default(Vector3), Quaternion.identity) as GameObject;
                        }

                        if (newTrack != null) {

                            if (newTrack.GetComponent<AudioTrack>() != null) {
                                newTrack.GetComponent<AudioTrack>().SetTrack(a);
                                newTrack.GetComponent<AudioTrack>().UpdateVolume(this.soundVolume);
                                if (a.persist == true) this.ConnectToPersistantParent(newTrack); else this.ConnectToNonPersistantParent(newTrack);
                                newTrack.SetActive(true);
                                newTrack.GetComponent<AudioTrack>().Play();
                                this.soundPool.Add(newTrack);
                            } else {
                                if(this.showDebug == true) {
									Debug.LogWarning(typeof(AudioManager).ToString() + " - Sound Effects Pool: Overriden Track Prefab does not contain component AudioTrack for " + a.trackName + "!");
								}
                            }
                        }
                    }
                }
            }
        }
    }
	
    /// <summary>
    /// Plays a specific sound track.
    /// </summary>
    /// <param name="trackName"></param>
	public void PlaySound(string trackName) {
		this.PlaySound(trackName, default(Vector3));
	}
	
    /// <summary>
    /// Plays a specific sound track.
    /// with a specific location (3D sound)
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="position"></param>
	public void PlaySound(string trackName, Vector3 position) {
		this.PlaySound(trackName, position, null);
	}
	
    /// <summary>
    /// Plays a specific sound track
    /// with a specific location (3D sound) and parent.
    /// </summary>
    /// <param name="trackName"></param>
    /// <param name="position"></param>
    /// <param name="parent"></param>
	public void PlaySound(string trackName, Vector3 position, GameObject parent) {
		foreach (GameObject pooledTrack in this.soundPool) {
			if(pooledTrack == null) return;
			if (pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == false) {
					if(parent != null) {
						pooledTrack.transform.parent = parent.transform;
					}

					pooledTrack.transform.position = position;
					pooledTrack.SetActive(true);
					pooledTrack.GetComponent<AudioTrack>().Play();

					if(pooledTrack.GetComponent<AudioTrack>().ForcePlayThrough == true && parent == null) {
						this.forcedPool.Add(pooledTrack);
						this.ConnectToPersistantParent(pooledTrack);
					}

					return;
				}
			}
		}

		if(this.GetSoundCount(trackName) > 0) {
			this.AddToSoundPool(trackName, position, parent);
			return;
		} else {
			if(this.showDebug == true) {
				Debug.LogWarning(typeof(AudioManager).ToString() + " - PlaySound: Could not find sound effect '" + trackName + "'!");
			}
			return;
		}
	}

    /// <summary>
    /// Pauses a certain sound track.
    /// </summary>
    /// <param name="trackName"></param>
	public void PauseSound(string trackName) {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().Pause();
				return;
			}
		}
	}
	
    /// <summary>
    /// Unpauses a certain paused sound track.
    /// </summary>
    /// <param name="trackName"></param>
	public void UnPauseSound(string trackName) {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().UnPause();
				return;
			}
		}
	}

    /// <summary>
    /// Pauses all currently playing sound tracks.
    /// </summary>
	public void PauseAllSound() {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				pooledTrack.GetComponent<AudioTrack>().Pause();
			}
		}
	}

    /// <summary>
    /// Unpauses all sound tracks that where
    /// previously paused.
    /// </summary>
	public void UnPauseAllSound() {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPaused == true) {
				pooledTrack.GetComponent<AudioTrack>().UnPause();
			}
		}
	}
	
    /// <summary>
    /// Stops a specific sound track.
    /// </summary>
    /// <param name="trackName"></param>
	public void StopSound(string trackName) {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().TrackName == trackName) {
				pooledTrack.GetComponent<AudioTrack>().Stop();
				return;
			}
		}
	}

    /// <summary>
    /// Stops all sound tracks that are currently
    /// playing.
    /// </summary>
	public void StopAllSound() {
		foreach(GameObject pooledTrack in this.soundPool) {
			if(pooledTrack.GetComponent<AudioTrack>().IsPlaying == true) {
				pooledTrack.GetComponent<AudioTrack>().Stop();
			}
		}
	}

    /// <summary>
    /// Stops both music and sounds tracks that are
    /// currently playing.
    /// </summary>
	public void StopAll() {
		this.StopAllSound();
		this.StopAllMusic();
	}

    /// <summary>
    /// Pauses both music and sound tracks.
    /// </summary>
	public void PauseAll() {
		this.PauseAllMusic();
		this.PauseAllSound();
	}

    /// <summary>
    /// Unpauses both music and sound tracks.
    /// </summary>
	public void UnPauseAll() {
		this.UnPauseAllMusic();
		this.UnPauseAllSound();
	}

    /// <summary>
    /// Mutes all sound tracks.
    /// </summary>
	public void MuteSound() {
		foreach(GameObject pooledMusic in this.soundPool) {
			pooledMusic.GetComponent<AudioTrack>().Mute = true;
		}
	}
	
    /// <summary>
    /// Unmutes all sound tracks.
    /// </summary>
	public void UnMuteSound() {
		foreach(GameObject pooledMusic in this.soundPool) {
			pooledMusic.GetComponent<AudioTrack>().Mute = false;
		}
	}

    /// <summary>
    /// Mutes both music and sound tracks.
    /// </summary>
	public void MuteAll() {
		this.MuteMusic();
		this.MuteSound();
	}

    /// <summary>
    /// Unmutes both music and sound tracks.
    /// </summary>
	public void UnMuteAll() {
		this.UnMuteMusic();
		this.UnMuteSound();
	}

    /// <summary>
    /// This allows you to change the volume of the sound
    /// clips. The value passed must be between 0 and 1.
    /// </summary>
    /// <param name="volume">0->1</param>
    public void SoundVolume(float volume) {
        foreach (GameObject pooledTrack in this.soundPool) {
            pooledTrack.GetComponent<AudioTrack>().UpdateVolume(volume);
        }
    }
	
    /// <summary>
    /// Returns an array of strings for the current sound list.
    /// </summary>
    /// <returns></returns>
	public string[] GetSoundList() {
		List<string> soundList = new List<string>();
		
		foreach (AudioInfo trackInfo in this.soundList) {
			soundList.Add(trackInfo.trackName);
		}
		
		return soundList.ToArray();
	}

	public void ConnectMusicPool(List<GameObject> mPool, AudioInfo[] mList) {
		List<GameObject> addPool = new List<GameObject>();
		bool addToPool = true;
		
		foreach(GameObject m in mPool) {
			for(int i = 0; i < this.musicPool.Count; i++) {
				if(this.musicPool[i] != null) {
					if(string.Equals(this.musicPool[i].GetComponent<AudioTrack>().TrackName, m.GetComponent<AudioTrack>().TrackName) == false) {
						addToPool = true;
					} else {
						addToPool = false;
						break;
					}
				}
			}
			
			if(addToPool == true) {
				if(m.GetComponent<AudioTrack>().Persist == true) this.ConnectToPersistantParent(m); else this.ConnectToNonPersistantParent(m);
				addPool.Add(m);
			}
		}

		foreach(GameObject a in addPool) {
			this.musicPool.Add(a);
		}

		List<AudioInfo> addList = new List<AudioInfo>();
		bool addToList = true;
		
		foreach(AudioInfo s in mList) {
			for(int i = 0; i < this.musicList.Length; i++) {
				if(this.musicList[i] != null) {
					if(string.Equals(this.musicList[i].trackName, s.trackName) == false) {
						addToList = true;
					} else {
						addToList = false;
						break;
					}
				}
			}
			
			if(addToList == true) {
				addList.Add(s);
			}
		}
		
		foreach(AudioInfo sItem in this.musicList) {
			addList.Add(sItem);
		}
		
		AudioInfo[] newSoundList = new AudioInfo[addList.Count];
		addList.CopyTo(newSoundList);
		this.musicList = newSoundList;
	}

	public void ConnectSoundPool(List<GameObject> sPool, AudioInfo[] sList) {
		List<GameObject> addPool = new List<GameObject>();
		bool addToPool = true;

		foreach(GameObject s in sPool) {
			for(int i = 0; i < this.soundPool.Count; i++) {
				if(this.soundPool[i] != null) {
					if(string.Equals(this.soundPool[i].GetComponent<AudioTrack>().TrackName, s.GetComponent<AudioTrack>().TrackName) == false) {
						addToPool = true;
					} else {
						addToPool = false;
						break;
					}
				}
			}

			if(addToPool == true) {
				if(s.GetComponent<AudioTrack>().Persist == true) this.ConnectToPersistantParent(s); else this.ConnectToNonPersistantParent(s);
				addPool.Add(s);
			}
		}
		
		foreach(GameObject a in addPool) {
			this.soundPool.Add(a);
		}

		List<AudioInfo> addList = new List<AudioInfo>();
		bool addToList = true;

		foreach(AudioInfo s in sList) {
			for(int i = 0; i < this.soundList.Length; i++) {
				if(this.soundList[i] != null) {
					if(string.Equals(this.soundList[i].trackName, s.trackName) == false) {
						addToList = true;
					} else {
						addToList = false;
						break;
					}
				}
			}

			if(addToList == true) {
				addList.Add(s);
			}
		}

		foreach(AudioInfo sItem in this.soundList) {
			addList.Add(sItem);
		}

		AudioInfo[] newSoundList = new AudioInfo[addList.Count];
		addList.CopyTo(newSoundList);
		this.soundList = newSoundList;
	}

	void Connect(GameObject track) {
		if(!this.forcedPool.Contains(track)) {
			track.transform.parent = this._persistentParent.transform;
			this.forcedPool.Add (track);
		}
	}

	void Disconnect(GameObject track) {
		if(this.forcedPool.Contains(track)) {
			this.forcedPool.Remove(track);

		}
	}

	void TrimLists() {
		List<GameObject> newMusicPool = new List<GameObject>();

		for(int i = 0; i < this.musicPool.Count; i++) {
			if(this.musicPool[i] != null) {
				newMusicPool.Add(this.musicPool[i]);
			}
		}

		this.musicPool = newMusicPool;

		List<GameObject> newSoundPool = new List<GameObject>();

		for(int i = 0; i < this.soundPool.Count; i++) {
			if(this.soundPool[i] != null) {
				newSoundPool.Add(this.soundPool[i]);
			}
		}

		this.soundPool = newSoundPool;

		List<AudioInfo> mListToKeep = new List<AudioInfo>();
		bool addToMList = false;

		foreach(AudioInfo m in this.musicList) {
			for(int i = 0; i < this.musicPool.Count; i++) {
				if(string.Equals(m.trackName, this.musicPool[i].GetComponent<AudioTrack>().TrackName) == true) {
					addToMList = true;
					break;
				} else {
					addToMList = false;
				}
			}

			if(addToMList == true) {
				mListToKeep.Add(m);
			}
		}

		AudioInfo[] newMusicList = new AudioInfo[mListToKeep.Count];
		mListToKeep.CopyTo(newMusicList);
		this.musicList = newMusicList;

		List<AudioInfo> sListToKeep = new List<AudioInfo>();
		bool addToSList = false;
		
		foreach(AudioInfo s in this.soundList) {
			for(int i = 0; i < this.soundPool.Count; i++) {
				if(string.Equals(s.trackName, this.soundPool[i].GetComponent<AudioTrack>().TrackName) == true) {
					addToSList = true;
					break;
				} else {
					addToSList = false;
				}
			}
			
			if(addToSList == true) {
				sListToKeep.Add(s);
			}
		}
		
		AudioInfo[] newSoundList = new AudioInfo[sListToKeep.Count];
		sListToKeep.CopyTo(newSoundList);
		this.soundList = newSoundList;
		List<GameObject> mark = new List<GameObject>();

		foreach(GameObject f in this.forcedPool) {
			if(f.GetComponent<AudioTrack>().IsPlaying == false) {
				mark.Add(f);
			}
		}

		foreach(GameObject m in mark) {
			if(this.forcedPool.Contains(m)) {

				this.forcedPool.Remove(m);
			}
		}

		for(int i = 0; i < mark.Count; i++) {
			DestroyImmediate(mark[0]);
		}
	}

	int GetSoundCount(string trackName) {
		int count = 0;

		foreach(GameObject s in this.soundPool) {
			if(s != null) {
				if(string.Equals(s.GetComponent<AudioTrack>().TrackName, trackName) == true) {
					count++;
				}
			}
		}

		return count;
	}

	void CreateNonPersistantParent() {
		if(this._nonPersistentParent == null) {
			this._nonPersistentParent = new GameObject("_Non Persistant Parent");
			this._nonPersistentParent.transform.position = default(Vector3);
		}
	}

	void CreatePersistantParent() {
		if(this._persistentParent == null) {
			this._persistentParent = new GameObject("_Persistant Parent");
			this._persistentParent.transform.position = default(Vector3);
			DontDestroyOnLoad(this._persistentParent);
		}
	}

	void ConnectToNonPersistantParent(GameObject o) {
		if(this._nonPersistentParent == null) {
			this.CreateNonPersistantParent();
		}

		o.transform.parent = this._nonPersistentParent.transform;
	}


	void ConnectToPersistantParent(GameObject o) {
		if(this._persistentParent == null) {
			this.CreatePersistantParent();
		}

		o.transform.parent = this._persistentParent.transform;
	}

	void DestoryParents() {
		DestroyImmediate(this._persistentParent);
		DestroyImmediate(this._nonPersistentParent);
	}

	void CheckIfChild() {
		if(this.transform.root != this.transform) {
			if(this.showDebug == true) {
				Debug.LogWarning(typeof(AudioManager).ToString() + " - CheckIfChild(): " + typeof(AudioManager).ToString() + " should not be parented!\nUnparenting...");
			}
			this.transform.parent = null;
		}
	}
}

/// <summary>
/// Audio Info, describes key
/// properties an audio track has.
/// </summary>
[System.Serializable]
public class AudioInfo {
	public string trackName;
	public AudioClip trackClip;
	public float customVolume;
	public float startDelay;
	public float fadeIn;
	public float fadeOut;
	public float pitch;
	public GameObject overrideAudioTrack;
	public bool enableLoop;
	public bool persist;
	public bool forcePlayThrough;
	public int maxInstances;
}
