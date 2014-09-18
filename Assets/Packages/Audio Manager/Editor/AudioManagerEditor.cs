//----------------------------------------------
//            Audio Manager
// Copyright © 2014 Tek-Wise Studios
//----------------------------------------------

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor {
	// Property Fields.
	SerializedProperty audioTrack;
	SerializedProperty musicList;
	SerializedProperty soundList;
    SerializedProperty musicVolume;
    SerializedProperty soundVolume;

	private List<bool> foldOutMusic = new List<bool>();
	private List<bool> foldOutSound = new List<bool>();
	private bool showMusic = true;
	private bool showSound = true;
    private enum SortType {
        None = 0,
        Ascending,
        Descending
    }
    private SortType musicSort = SortType.None;
    private SortType soundSort = SortType.None;

	void OnEnable() {
		this.audioTrack = serializedObject.FindProperty("audioTrack");
		this.musicList = serializedObject.FindProperty("musicList");
        this.musicVolume = serializedObject.FindProperty("musicVolume");
		this.soundList = serializedObject.FindProperty("soundList");
        this.soundVolume = serializedObject.FindProperty("soundVolume");
		this.UpdateFoldOut();
	}

	public override void OnInspectorGUI() {
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Audio Track Prefab (Required)");
		this.audioTrack.objectReferenceValue = EditorGUILayout.ObjectField (this.audioTrack.objectReferenceValue, typeof(GameObject), true, GUILayout.ExpandWidth (true));
		GUILayout.EndHorizontal();
		
		if(this.audioTrack.objectReferenceValue != null) {
			this.showMusic = EditorGUILayout.Foldout(this.showMusic, "Music");
			if(this.showMusic == true) {
				this.DrawFromList(this.musicList, "Music", this.foldOutMusic, ref this.musicVolume, ref this.musicSort);
			}
			this.showSound = EditorGUILayout.Foldout(this.showSound, "Sound");
			if(this.showSound == true) {
				this.DrawFromList(this.soundList, "Sound", this.foldOutSound, ref this.soundVolume, ref this.soundSort);
			}
		} else {
			EditorGUILayout.LabelField("Audio track object must be assigned");
		}

		serializedObject.ApplyModifiedProperties();
	}

	void DrawFromList(SerializedProperty audioList, string listName, List<bool> foldOut, ref SerializedProperty masterVolume, ref SortType sortType) {
		EditorGUI.indentLevel++;
        GUILayout.BeginHorizontal();
        sortType = (AudioManagerEditor.SortType) EditorGUILayout.EnumPopup("Sort Order: ", sortType);
        if (GUILayout.Button("Sort", GUILayout.MaxWidth(125.0f), GUILayout.ExpandWidth(false))) {
            this.Sort(audioList, sortType);
        }
        GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(listName + " List Size: " + audioList.arraySize, GUILayout.ExpandWidth(true));
		GUI.color = Color.green;
		if(GUILayout.Button("+ " + listName, GUILayout.MaxWidth(125.0f))) {
			this.AddItem(audioList, 0);
			this.UpdateFoldOut();
			return;
		}
		GUI.color = Color.white;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		EditorGUILayout.LabelField(listName + " Volume", GUILayout.MaxWidth(95.0f));
		masterVolume.floatValue *= 100.0f;
		masterVolume.floatValue = EditorGUILayout.Slider(masterVolume.floatValue, 0, 100);
		
		if (masterVolume.floatValue < 0.0f) {
			masterVolume.floatValue = 0.0f;
		} else if (masterVolume.floatValue > 100.0f) {
			masterVolume.floatValue = 1.0f;
		} else {
			masterVolume.floatValue /= 100.0f;
		}
		
		EditorGUILayout.LabelField("%", GUILayout.Width(15.0f));
		GUILayout.EndHorizontal();
		
		for(int i = 0; i < audioList.arraySize; i++) {
			SerializedProperty audioItem = audioList.GetArrayElementAtIndex(i);
            SerializedProperty trackName = audioItem.FindPropertyRelative("trackName");
            SerializedProperty trackClip = audioItem.FindPropertyRelative("trackClip");
            SerializedProperty customVolume = audioItem.FindPropertyRelative("customVolume");
            SerializedProperty startDelay = audioItem.FindPropertyRelative("startDelay");
            SerializedProperty fadeIn = audioItem.FindPropertyRelative("fadeIn");
            SerializedProperty fadeOut = audioItem.FindPropertyRelative("fadeOut");
            SerializedProperty pitch = audioItem.FindPropertyRelative("pitch");
            SerializedProperty overrideAudioTrack = audioItem.FindPropertyRelative("overrideAudioTrack");
            SerializedProperty enableLoop = audioItem.FindPropertyRelative("enableLoop");
            SerializedProperty persist = audioItem.FindPropertyRelative("persist");
           //  SerializedProperty forcePlayThrough = audioItem.FindPropertyRelative("forcePlayThrough");
            SerializedProperty maxInstances = audioItem.FindPropertyRelative("maxInstances");
			
			GUILayout.BeginHorizontal();
			foldOut[i] = EditorGUILayout.Foldout(foldOut[i], trackName.stringValue);
			GUI.color = Color.red;
			if(GUILayout.Button("- " + listName + " Track", GUILayout.MaxWidth(125.0f), GUILayout.ExpandWidth(false))) {
				audioList.DeleteArrayElementAtIndex(i);
				this.UpdateFoldOut();
				return;
			}
			GUI.color = Color.white;
			GUILayout.EndHorizontal();
			
			if(foldOut[i] == true) {
				EditorGUI.indentLevel++;
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Track Name", GUILayout.Width (100.0f));
				trackName.stringValue = EditorGUILayout.TextField(trackName.stringValue, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Track Clip", GUILayout.Width (100.0f));
				trackClip.objectReferenceValue = EditorGUILayout.ObjectField(trackClip.objectReferenceValue, typeof(AudioClip), false);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				float trackLength = 0.0f;
				if(trackClip.objectReferenceValue != null) {
					trackLength = (trackClip.objectReferenceValue as System.Object as AudioClip).length;
					EditorGUILayout.LabelField(this.GetTrackTime(trackLength) + " runtime");
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Volume", GUILayout.Width (90.0f));
				customVolume.floatValue *= 100.0f;
				customVolume.floatValue = EditorGUILayout.Slider(customVolume.floatValue, 0, 100);
				
				if(customVolume.floatValue < 0) {
					customVolume.floatValue = 0;
				} else if(customVolume.floatValue > 100.0f) {
					customVolume.floatValue = 1;
				} else {
					customVolume.floatValue /= 100.0f;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Start Delay", GUILayout.ExpandWidth(true));
				startDelay.floatValue = EditorGUILayout.FloatField(startDelay.floatValue, GUILayout.Width(50.0f));
				
				if(startDelay.floatValue < 0) {
					startDelay.floatValue = 0;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Fade In", GUILayout.Width (90.0f));
				fadeIn.floatValue = EditorGUILayout.Slider(fadeIn.floatValue, 0, trackLength, GUILayout.ExpandWidth(true));
				
				if(fadeIn.floatValue < 0) {
					fadeIn.floatValue = 0;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Fade Out", GUILayout.Width (90.0f));
				fadeOut.floatValue = EditorGUILayout.Slider(fadeOut.floatValue, 0, trackLength, GUILayout.ExpandWidth(true));
				
				if(fadeOut.floatValue < 0) {
					fadeOut.floatValue = 0;
				}
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Pitch", GUILayout.Width (90.0f));
				pitch.floatValue = EditorGUILayout.Slider(pitch.floatValue, -3, 3, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Override", GUILayout.Width (90.0f));
				overrideAudioTrack.objectReferenceValue = EditorGUILayout.ObjectField(overrideAudioTrack.objectReferenceValue, typeof(GameObject), false, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Loop", GUILayout.Width (90.0f));
				enableLoop.boolValue = EditorGUILayout.Toggle(enableLoop.boolValue, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Persist", GUILayout.Width (90.0f));
				persist.boolValue = EditorGUILayout.Toggle(persist.boolValue, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();

				// WIP
				/*GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Force", GUILayout.Width (90.0f));
				forcePlayThrough.boolValue = EditorGUILayout.Toggle(forcePlayThrough.boolValue, GUILayout.ExpandWidth(true));
				GUILayout.EndHorizontal();*/
				
				GUILayout.BeginHorizontal();
				EditorGUILayout.LabelField("Max Clones", GUILayout.ExpandWidth(true));
				maxInstances.intValue = EditorGUILayout.IntField(maxInstances.intValue, GUILayout.ExpandWidth(true));
				if(maxInstances.intValue < -1) {
					maxInstances.intValue = -1;
				} else if(maxInstances.intValue == 0) {
					maxInstances.intValue = 1;
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				if(maxInstances.intValue == -1) {
					EditorGUILayout.TextArea("When set to -1, Audio Manager will create as\nmany audio clips as needed when it gets called.");
				}
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
			}
		}

		this.DropAreaGUI (listName, audioList);

		EditorGUI.indentLevel--;
	}

	void AddItem(SerializedProperty list, int index, AudioClip audioClip = null) {
		list.InsertArrayElementAtIndex(index);
        SerializedProperty audioItem = list.GetArrayElementAtIndex(index-1);
        SerializedProperty trackName = audioItem.FindPropertyRelative("trackName");
        SerializedProperty trackClip = audioItem.FindPropertyRelative("trackClip");
        SerializedProperty customVolume = audioItem.FindPropertyRelative("customVolume");
        SerializedProperty startDelay = audioItem.FindPropertyRelative("startDelay");
        SerializedProperty fadeIn = audioItem.FindPropertyRelative("fadeIn");
        SerializedProperty fadeOut = audioItem.FindPropertyRelative("fadeOut");
        SerializedProperty pitch = audioItem.FindPropertyRelative("pitch");
        SerializedProperty overrideAudioTrack = audioItem.FindPropertyRelative("overrideAudioTrack");
        SerializedProperty enableLoop = audioItem.FindPropertyRelative("enableLoop");
        SerializedProperty persist = audioItem.FindPropertyRelative("persist");
        // SerializedProperty forcePlayThrough = audioItem.FindPropertyRelative("forcePlayThrough");
        SerializedProperty maxInstances = audioItem.FindPropertyRelative("maxInstances");

		if(audioClip != null) {
			trackName.stringValue = audioClip.name;
			trackClip.objectReferenceValue = audioClip;
			customVolume.floatValue = 1.0f;
			startDelay.floatValue = 0.0f;
			fadeIn.floatValue = 0.0f;
			fadeOut.floatValue = 0.0f;
			pitch.floatValue = 1.0f;
			overrideAudioTrack.objectReferenceValue = null;
			enableLoop.boolValue = false;
			persist.boolValue = false;
			// forcePlayThrough.boolValue = false;
			maxInstances.intValue = 1;
		} else {
			trackName.stringValue = "New Track";
			trackClip.objectReferenceValue = null;
			customVolume.floatValue = 1.0f;
			startDelay.floatValue = 0.0f;
			fadeIn.floatValue = 0.0f;
			fadeOut.floatValue = 0.0f;
			pitch.floatValue = 1.0f;
			overrideAudioTrack.objectReferenceValue = null;
			enableLoop.boolValue = false;
			persist.boolValue = false;
			// forcePlayThrough.boolValue = true;
			maxInstances.intValue = 1;
		}
	}

	string GetTrackTime(float length) {
		string trackTime = "";

		int minutes = (int)(length / 60.0f);
		int seconds = (int)(length % 60.0f);
		int millisecond = (int)(((length / 60.0f - (float)minutes) * 60.0f - (float)seconds) * 1000.0f);

		if(minutes < 10) {
			trackTime = "0" + minutes + ":";
		} else {
			trackTime = minutes + ":";
		}

		if(seconds < 10) {
			trackTime += "0" + seconds;
		} else {
			trackTime += seconds;
		}

		if(millisecond < 100) {
			trackTime += ".0" + millisecond;
		} else {
			trackTime += "." + millisecond;
		}

		return trackTime;
	}

	void UpdateFoldOut() {
		this.foldOutMusic = null;
		this.foldOutMusic = new List<bool>();

		for(int i = 0; i < this.musicList.arraySize; i++) {
			this.foldOutMusic.Add(false);
		}

		this.foldOutSound = null;
		this.foldOutSound = new List<bool>();
		
		for(int i = 0; i < this.soundList.arraySize; i++) {
			this.foldOutSound.Add(false);
		}
	}

    void Sort(SerializedProperty list, SortType sortType) {
        if (sortType == SortType.Ascending) {
            for (int i = 0; i < list.arraySize; i++) {
                if ((i + 1) < list.arraySize) {
                    for (int j = i + 1; j < list.arraySize; j++) {
                        SerializedProperty firstTrack = list.GetArrayElementAtIndex(i);
                        SerializedProperty secondTrack = list.GetArrayElementAtIndex(j);
                        string firstTrackName = firstTrack.FindPropertyRelative("trackName").stringValue;
                        string secondTrackName = secondTrack.FindPropertyRelative("trackName").stringValue;

                        if (firstTrackName.CompareTo(secondTrackName) > 0) {
                            this.SwapValues(firstTrack, secondTrack);
                        }
                    }
                }
            }
        } else if (sortType == SortType.Descending) {
            for (int i = 0; i < list.arraySize; i++) {
                if ((i + 1) < list.arraySize) {
                    for (int j = i + 1; j < list.arraySize; j++) {
                        SerializedProperty firstTrack = list.GetArrayElementAtIndex(i);
                        SerializedProperty secondTrack = list.GetArrayElementAtIndex(j);
                        string firstTrackName = firstTrack.FindPropertyRelative("trackName").stringValue;
                        string secondTrackName = secondTrack.FindPropertyRelative("trackName").stringValue;

                        if (firstTrackName.CompareTo(secondTrackName) < 0) {
                            this.SwapValues(secondTrack, firstTrack);
                        }
                    }
                }
            }
        } else {
            return;
        }
    }

    void SwapValues(SerializedProperty from, SerializedProperty to) {
        string trackName = from.FindPropertyRelative("trackName").stringValue;
        AudioClip trackClip = from.FindPropertyRelative("trackClip").objectReferenceValue as AudioClip;
        float customVolume = from.FindPropertyRelative("customVolume").floatValue;
        float startDelay = from.FindPropertyRelative("startDelay").floatValue;
        float fadeIn = from.FindPropertyRelative("fadeIn").floatValue;
        float fadeOut = from.FindPropertyRelative("fadeOut").floatValue;
        float pitch = from.FindPropertyRelative("pitch").floatValue;
        GameObject overrideAudioTrack = from.FindPropertyRelative("overrideAudioTrack").objectReferenceValue as GameObject;
        bool enableLoop = from.FindPropertyRelative("enableLoop").boolValue;
        bool persist = from.FindPropertyRelative("persist").boolValue;
        // bool forcePlayThrough = from.FindPropertyRelative("forcePlayThrough").boolValue;
        int maxInstances = from.FindPropertyRelative("maxInstances").intValue;

        from.FindPropertyRelative("trackName").stringValue = to.FindPropertyRelative("trackName").stringValue;
        from.FindPropertyRelative("trackClip").objectReferenceValue = to.FindPropertyRelative("trackClip").objectReferenceValue;
        from.FindPropertyRelative("customVolume").floatValue = to.FindPropertyRelative("customVolume").floatValue;
        from.FindPropertyRelative("startDelay").floatValue = to.FindPropertyRelative("startDelay").floatValue;
        from.FindPropertyRelative("fadeIn").floatValue = to.FindPropertyRelative("fadeIn").floatValue;
        from.FindPropertyRelative("fadeOut").floatValue = to.FindPropertyRelative("fadeOut").floatValue;
        from.FindPropertyRelative("pitch").floatValue = to.FindPropertyRelative("pitch").floatValue;
        from.FindPropertyRelative("overrideAudioTrack").objectReferenceValue = to.FindPropertyRelative("overrideAudioTrack").objectReferenceValue;
        from.FindPropertyRelative("enableLoop").boolValue = to.FindPropertyRelative("enableLoop").boolValue;
        from.FindPropertyRelative("persist").boolValue = to.FindPropertyRelative("persist").boolValue;
        // from.FindPropertyRelative("forcePlayThrough").boolValue = to.FindPropertyRelative("forcePlayThrough").boolValue;
        from.FindPropertyRelative("maxInstances").intValue = to.FindPropertyRelative("maxInstances").intValue;

        to.FindPropertyRelative("trackName").stringValue = trackName;
        to.FindPropertyRelative("trackClip").objectReferenceValue = trackClip;
        to.FindPropertyRelative("customVolume").floatValue = customVolume;
        to.FindPropertyRelative("startDelay").floatValue = startDelay;
        to.FindPropertyRelative("fadeIn").floatValue = fadeIn;
        to.FindPropertyRelative("fadeOut").floatValue = fadeOut;
        to.FindPropertyRelative("pitch").floatValue = pitch;
        to.FindPropertyRelative("overrideAudioTrack").objectReferenceValue = overrideAudioTrack;
        to.FindPropertyRelative("enableLoop").boolValue = enableLoop;
        to.FindPropertyRelative("persist").boolValue = persist;
        // to.FindPropertyRelative("forcePlayThrough").boolValue = forcePlayThrough;
        to.FindPropertyRelative("maxInstances").intValue = maxInstances;
    }

	void DropAreaGUI(string listName, SerializedProperty audioList) {
		var evt = Event.current;

		EditorGUILayout.BeginHorizontal ();
		var dropArea = GUILayoutUtility.GetRect (0.0f, 60.0f, GUILayout.ExpandWidth(true));

		GUI.Box (dropArea, "<----Drop " + listName + " Clips Here---->\nLock the Inspector so you can\n select multiple Audio Clips\n from your project folder!");

		switch(evt.type) {
			case EventType.DragUpdated:
			case EventType.DragPerform:
			if(!dropArea.Contains(evt.mousePosition)) {
				break;
			}
			DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

			if(evt.type == EventType.DragPerform) {
				DragAndDrop.AcceptDrag();

				foreach(var draggedObject in DragAndDrop.objectReferences) {
					AudioClip audioClip = draggedObject as AudioClip;
					
					if(!audioClip) continue;
					this.AddItem(audioList, 0, audioClip);
					this.UpdateFoldOut();
				}
			}
			
			Event.current.Use();
			break;
		}

		EditorGUILayout.EndHorizontal ();
	}
}
