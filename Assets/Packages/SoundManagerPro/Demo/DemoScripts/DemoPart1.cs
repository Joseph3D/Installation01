using UnityEngine;
using System.Collections;

public class DemoPart1 : MonoBehaviour {
	// Particle effect to go with explosion sound ;)
	public GameObject explosionPrefab;
	public Texture2D AntiLunchBoxLogo;
	
	// Sample AudioClips
	public AudioClip sample1;
	public AudioClip sample2;
	public AudioClip sample3;
	public AudioClip sample4;
	
	int thecolor = 0;
	Color buttonColor = Color.yellow;
	
	float unitX;
	float unitY;

	int page = 1;
	
	// A SoundConnection to use later, initialization is shown in Start()
	SoundConnection sc;
	
	// Just so we don't have duplicates while moving through scenes
	void Awake () {
		if(GameObject.FindObjectsOfType(typeof(DemoPart1)).Length > 1)
		{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
	}
	
	void Start()
	{
		// How to initialize a SoundConnection
		sc = SoundManager.CreateSoundConnection("TemporarySoundConnection", SoundManager.PlayMethod.ContinuousPlayThrough, sample4, sample2, sample3, sample1);
		
		unitX = Screen.width / 48f;
		unitY = Screen.height / 30f;
	}
	
	void ChangeButtonColor()
	{
		switch(thecolor)
		{
		case 0:
			buttonColor = Color.blue;
			thecolor = 1;
			break;
		case 1:
			buttonColor = Color.red;
			thecolor = 2;
			break;
		case 2:
			buttonColor = Color.green;
			thecolor = 3;
			break;
		case 3:
		default:
			buttonColor = Color.yellow;
			thecolor = 0;
			break;
		}
	}
	
	
	GUIStyle boxStyle;
	GUIStyle buttonSTyle;
	GUIStyle labelStyle;
	void OnGUI()
	{
		boxStyle = GUI.skin.box;
		boxStyle.fontStyle = FontStyle.Bold;
		boxStyle.fontSize = 14;
		
		buttonSTyle = GUI.skin.button;
		buttonSTyle.fontStyle = FontStyle.Bold;
		
		labelStyle = GUI.skin.label;
		labelStyle.alignment = TextAnchor.MiddleCenter;
		labelStyle.fontStyle = FontStyle.Bold;
		labelStyle.normal.textColor = Color.black;
		
		/* TITLE */
		GUI.Box(new Rect(16f*unitX, 0f, 16f*unitX, 2f*unitY), "AntiLunchBox\nSoundManagerPro 2.0", boxStyle);
		
		/* LOAD SCENES */
		GUI.color = buttonColor;
		if(GUI.Button(new Rect(6f*unitX, 3f*unitY, 8f*unitX, 4f*unitY), "Load Level:\nMusicScene1" ) )
		{
			Application.LoadLevel("MusicScene1"); //LoadLevelAsync also works for UNITY PRO users
		}
		
		
		if(GUI.Button(new Rect(20f*unitX, 3f*unitY, 8f*unitX, 4f*unitY), "Load Level:\nMusicScene2" ) )
		{
			Application.LoadLevel("MusicScene2");
		}
		
		
		if(GUI.Button(new Rect(34f*unitX, 3f*unitY, 8f*unitX, 4f*unitY), "Load Level:\nMusicScene3" ) )
		{
			Application.LoadLevel("MusicScene3");
		}
		GUI.color = Color.white;
		
		/* COLUMN 1 */
		float yPos = unitY * 8f;
		float xPos = unitX * 5f;
		float height = unitY * 3f;
		float width = unitX * 10f;
		
		
		
		
		
		// Will play sample1, interrupting the current SoundConnection.  You can resume a SoundConnection when it's done using another overload below
		if(page == 1 && GUI.Button(new Rect(xPos, yPos, width, height), "Play Sample1"))
		{
			// You can use SoundManager.Load(clipname, customPath) -- Or set SoundManager.resourcesPath and forget the custom path
			SoundManager.Play(sample1);

		} 
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos, width, height), "Next Track"))
		{
			SoundManager.Next();
		}
		
		
		
		
		
		
		
		// Plays sample2 immediately, no crossfade
		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Sample2\nImmediately"))
		{
			SoundManager.PlayImmediately(sample2);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Prev Track"))
		{
			SoundManager.Prev();
		}
		
		
		
		
		
		

		// Plays sample3, and will call 'ChangeButtonColor' when the song ends.
		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Sample3 Then\nChange Button Colors\nOn Song End"))
		{
			SoundManager.Play(sample3, false, ChangeButtonColor);
		} 
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Sample1 as\na Looping Track"))
		{
			SoundManager.Play(sample1, true);
		}
		
		
		
		
		

		// Will play a SoundConnection made in code.  Does not save unless you use (AddSoundConnection/ReplaceSoundConnection)
		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Temporary\nSoundConnection"))
		{
			SoundManager.PlayConnection(sc);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Pause"))
		{
			SoundManager.Pause();
		}
		
		
		
		
		

		// Remove a SoundConnection from a scene making it a silent scene next time you enter it.
		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Custom\nSoundConnection\n(\"MyCustom\")"))
		{
			SoundManager.PlayConnection("MyCustom");
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "UnPause"))
		{
			SoundManager.UnPause();
		}
		
		
		
		
		

		/* COLUMN 2 */
		yPos = unitY * 8f;
		xPos = unitX * 19f;
		
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos, width, height), "Set All Volume 50%"))
		{
			SoundManager.SetVolume(.5f);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos, width, height), "Set SoundManager to\nIgnore AI"))
		{
			SoundManager.SetIgnoreLevelLoad(true);
		}
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Set All Volume 100%"))
		{
			SoundManager.SetVolume(1f);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Capped SFX\nBUTTON SMASH\nTHIS BUTTON!"))
		{
			SoundManager.PlayCappedSFX(SoundManager.Load("Explosion1"), "Explosion");
		}
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Set All Pitch 75%"))
		{
			SoundManager.SetPitch(.75f);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Set Pitch of SFX\n125%"))
		{
			SoundManager.SetPitchSFX(1.25f);
		}
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Set All Pitch 100%"))
		{
			SoundManager.SetPitch(1f);
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Set Pitch of SFX\n100%"))
		{
			SoundManager.SetPitchSFX(1f);
		}
		
		
		
		
		

		/* COLUMN 3 */
		yPos = unitY * 8f;
		xPos = unitX * 33f;
		
		
		
		
		
		

		// Crossfade out all music
		if(page == 1 && GUI.Button(new Rect(xPos, yPos, width, height), "Stop Music"))
		{
			SoundManager.StopMusic();
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos, width, height), "Remove SoundConnection\nin MusicScene1"))
		{
			SoundManager.RemoveSoundConnectionForLevel("MusicScene1");
		}
		
		
		
		
		

		// Stop music immediately, no crossfade
		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Stop Music\nImmediately"))
		{
			SoundManager.StopMusicImmediately();
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Add/Replace\nSoundConnection\nin MusicScene1"))
		{
			SoundConnection replacement = SoundManager.CreateSoundConnection("MusicScene1", SoundManager.PlayMethod.ContinuousPlayThrough, sample4, sample2, sample3, sample1);
			SoundManager.ReplaceSoundConnection(replacement);
		}
		
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Toggle All Mute"))
		{
			SoundManager.Mute();
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Add/Replace\nCustom SoundConnection\n(\"NewCustom\")"))
		{
			SoundConnection newCustom = SoundManager.CreateSoundConnection("NewCustom", SoundManager.PlayMethod.ContinuousPlayThrough, sample4, sample2, sample3, sample1);
			SoundManager.ReplaceSoundConnection(newCustom);
		}
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Toggle SFX Mute"))
		{
			SoundManager.MuteSFX();
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Play Random SFX\nfrom MyGroup"))
		{
			SoundManager.PlaySFX(SoundManager.LoadFromGroup("MyGroup"));
		}
		
		
		
		
		

		if(page == 1 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Toggle Music Mute"))
		{
			SoundManager.MuteMusic();
		}
		else if(page == 2 && GUI.Button(new Rect(xPos, yPos+=(unitY*4f), width, height), "Change the Crossfade\nDuration to 1s"))
		{
			SoundManager.SetCrossDuration(1f);
		}
		
		
		
		
		

		/* FOOTER */
		// Spawn the explosion particle effect and play the sound effect in 2 different ways (there are 6 more overloads)
		if(GUI.Button(new Rect(20f*unitX, 24f*unitY + Mathf.PingPong(Time.time*24f, unitY), 8f*unitX, 5f*unitY), AntiLunchBoxLogo, labelStyle))
		{
			GameObject newExplosion = GameObject.Instantiate(explosionPrefab, Camera.main.transform.position + (5f * Camera.main.transform.forward), Quaternion.identity) as GameObject;
			if(Random.Range(0,2) == 1)
				// This will play the SFX from the Stored SFXs on SoundManager, in the pooling system.
				SoundManager.PlaySFX(SoundManager.Load("Explosion1"));
			else
				// This will play the SFX on the gameobject--if it doesn't have an audiosource, it'll add it for you.
				SoundManager.PlaySFX(newExplosion, SoundManager.Load("Explosion1"));
			
			//If you want to make sure that audiosource is 2D, use this:
			//SoundManagerTools.make2D( ref SoundManager.PlaySFX(newExplosion, SoundManager.Load("Explosion1")));
			//OR 3d
			//SoundManagerTools.make3D( ref SoundManager.PlaySFX(newExplosion, SoundManager.Load("Explosion1")));
		}
		GUI.Label(new Rect(20f*unitX, 24f*unitY + Mathf.PingPong(Time.time*24f, unitY), 8f*unitX, 5f*unitY), "Click Me!", labelStyle);

		if(!CanGoNext()) GUI.enabled = false;
		if(GUI.Button(new Rect(Screen.width - 75f, 0f, 75f, 50f), "Next\nPage"))
			page = 2;
		GUI.enabled = true;

		if(!CanGoPrev()) GUI.enabled = false;
		if(GUI.Button(new Rect(0f, 0f, 75f, 50f), "Prev\nPage"))
			page = 1;
		GUI.enabled = true;
	}

	bool CanGoNext()
	{
		switch(page)
		{
		case 1:
			return true;
		case 2:
			return false;
		default:
			return false;
		}
	}

	bool CanGoPrev()
	{
		switch(page)
		{
		case 1:
			return false;
		case 2:
			return true;
		default:
			return false;
		}
	}
	
	void OnLevelWasLoaded(int level)
	{
		switch(Application.loadedLevelName)
		{
		case "MusicScene1":
			Camera.main.backgroundColor = Color.gray;
			break;
		case "MusicScene2":
			Camera.main.backgroundColor = Color.magenta;
			break;
		case "MusicScene3":
			Camera.main.backgroundColor = Color.blue;
			break;
		default:
			Camera.main.backgroundColor = new Color(49f/255f, 77f/255f, 121f/255f, 5f/255f);
			break;
		}
	}
}
