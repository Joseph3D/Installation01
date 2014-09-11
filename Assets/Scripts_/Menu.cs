using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour 
{

	public static Menu Instance;

	public GUIStyle MenuBoxes;
	public GUIStyle LOGO;
	public GUIStyle MenuButtons;

	public Texture2D Splash;

	public Texture2D BackgroundTexture;

	public float SensitivityNorm;
	public float SensitivityAims;

	public string CurrentMenu;
	public string MatchName;

	public int Players;

	void Start () 
	{
		SensitivityNorm = 3;
		SensitivityAims = 0.5f;
		if(PlayerPrefs.GetString("name") == "")
			PlayerPrefs.SetString("name", "NewPlayer " + Random.Range(0,200));

		Screen.lockCursor = false;
		Instance = this; 
		CurrentMenu = "MainMenu";
		MatchName = "IO1 " + Random.Range(0,2000);
		Players = 8;
	}
	
	
	void ToMenu(string Menu)
	{
		CurrentMenu = Menu;
	}

	public void OnGUI()
	{
		GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), BackgroundTexture);
		if(CurrentMenu == "MainMenu")
			MainMenu();
		if(CurrentMenu == "Multiplayer")
			Multiplayer();
		if(CurrentMenu == "HostMatch")
			HostMatch();
		if(CurrentMenu == "ServerList")
			ServerList();
		if(CurrentMenu == "Lobby")
			Lobby();
		if(CurrentMenu == "Options")
			Options();
	}

	public void MainMenu()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(new Rect(50, Screen.height /2, 300, Screen.height/2), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1);

		GUI.Label(new Rect(Screen.width - 300, Screen.height - 150, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(Screen.width - 210, Screen.height - 110, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.6f);
		if(GUI.Button(new Rect(50, Screen.height /2, 300, 30), "Multiplayer", MenuButtons))
			ToMenu("Multiplayer");
		if(GUI.Button(new Rect(50, Screen.height /2 + 30, 300, 30), "Options", MenuButtons))
			ToMenu("Options");
		if(GUI.Button(new Rect(50, Screen.height /2 + 60, 300, 30), "Quit", MenuButtons))
			Application.Quit();
		GUI.color = new Color(1,1,1,1);

		GUI.color = new Color(1,1,1,0.6f);
		GameManager.Instance.PlayerName = GUI.TextField(new Rect(50, Screen.height /2 + 95, 300, 30), GameManager.Instance.PlayerName, MenuBoxes);
		PlayerPrefs.SetString("name", GameManager.Instance.PlayerName);
		GUI.color = new Color(1,1,1,1f);
	}

	public void Options()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(new Rect(50, Screen.height /2, 300, Screen.height/2), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1);

		GUI.Label(new Rect(Screen.width - 300, Screen.height - 150, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(Screen.width - 210, Screen.height - 110, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.6f);

		if(GUI.Button(new Rect(50, Screen.height/2, 300, 30), "Low", MenuButtons))
			QualitySettings.SetQualityLevel(0);

		if(GUI.Button(new Rect(50, Screen.height/2 + 30, 300, 30), "Medium", MenuButtons))
			QualitySettings.SetQualityLevel(1);

		if(GUI.Button(new Rect(50, Screen.height/2 + 60, 300, 30), "High", MenuButtons))
			QualitySettings.SetQualityLevel(2);

		if(GUI.Button(new Rect(50, Screen.height/2 + 90, 300, 30), "Very High", MenuButtons))
			QualitySettings.SetQualityLevel(3);

		if(GUI.Button(new Rect(50, Screen.height/2 + 120, 300, 30), "Ultra", MenuButtons))
			QualitySettings.SetQualityLevel(4);

		GUI.Box(new Rect(50, Screen.height/2 + 150, 300, 30), "Sensitivity: " + SensitivityNorm.ToString());

		if(GUI.Button(new Rect(50, Screen.height/2 + 180, 150, 30), "+", MenuButtons))
			SensitivityNorm = SensitivityNorm + 0.5f;
		if(GUI.Button(new Rect(200, Screen.height/2 + 180, 150, 30), "-", MenuButtons))
			SensitivityNorm = SensitivityNorm - 0.5f;

		GUI.Box(new Rect(50, Screen.height/2 + 210, 300, 30), "Sensitivity Zooming: " + SensitivityAims.ToString());

		if(GUI.Button(new Rect(50, Screen.height/2 + 240, 150, 30), "+", MenuButtons))
			SensitivityAims = SensitivityAims + 0.5f;
		if(GUI.Button(new Rect(200, Screen.height/2 + 240, 150, 30), "-", MenuButtons))
			SensitivityAims = SensitivityAims - 0.5f;
		
		if(GUI.Button(new Rect(50, Screen.height - 30, 300, 30), "Back", MenuButtons))
			ToMenu("MainMenu");
		
		GUI.color = new Color(1,1,1,1);
	}

	public void Multiplayer()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(new Rect(50, Screen.height /2, 300, Screen.height/2), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1);

		GUI.Label(new Rect(Screen.width - 300, Screen.height - 150, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(Screen.width - 210, Screen.height - 110, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.6f);
		if(GUI.Button(new Rect(50, Screen.height /2, 300, 30), "Host Match", MenuButtons))
			ToMenu("HostMatch");
		if(GUI.Button(new Rect(50, Screen.height /2 + 30, 300, 30), "Server List", MenuButtons))
			ToMenu("ServerList");
		if(GUI.Button(new Rect(50, Screen.height /2 + 60, 300, 30), "Back" , MenuButtons))
			ToMenu("MainMenu");
		GUI.color = new Color(1,1,1,1);
	}

	public void HostMatch()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(new Rect(50, Screen.height /2, 300, Screen.height/2), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1);

		GUI.Label(new Rect(Screen.width - 300, Screen.height - 150, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(Screen.width - 210, Screen.height - 110, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.6f);
		if(GUI.Button(new Rect(50, Screen.height /2, 300, 30), "Start Server", MenuButtons))
		{
			GameManager.Instance.StartServer(MatchName,Players);
			ToMenu("Lobby");
		}

		MatchName = GUI.TextField(new Rect(50, Screen.height /2 + 65, 300, 30),MatchName, MenuBoxes);

		if(GUI.Button(new Rect(50, Screen.height /2 + 30, 300, 30), "Back" , MenuButtons))
			ToMenu("Multiplayer");
		GUI.color = new Color(1,1,1,1);
		
	}

	public void ServerList()
	{
		GUI.Label(new Rect(Screen.width - 300, Screen.height - 150, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(Screen.width - 210, Screen.height - 110, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.6f);
		//GUI.Box(new Rect(10,10, Screen.width - 20, Screen.height - 85), "");

		if(GUI.Button(new Rect(10, Screen.height - 70, Screen.width - 20, 30), "Refresh", MenuButtons))
		{
			MasterServer.RequestHostList("IO1");
			Debug.Log("REFESRS");
		}

		if(GUI.Button(new Rect(10, Screen.height - 35, Screen.width - 20, 30), "Back", MenuButtons))
			ToMenu("Multiplayer");
		GUI.color = new Color(1,1,1,1f);

		GUI.color = new Color(1,1,1,0.1f);
		GUILayout.BeginArea(new Rect(10,10, Screen.width - 20, Screen.height - 85), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1f);
		foreach (HostData hd in MasterServer.PollHostList())
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(hd.gameName, MenuButtons);
            if (GUILayout.Button("Connect", MenuButtons))
            {
                Network.useNat = hd.useNat;

				if (Network.useNat)
					print("Using Nat punchthrough to connect to host");
				else
					print("Connecting directly to host");

				Network.Connect(hd);	
                ToMenu("Lobby");
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndArea();
        GUI.color = new Color(1,1,1,1f);
	}

	public void Lobby()
	{
		GUI.color = new Color(1,1,1,0.1f);
		GUI.Box(new Rect(Screen.width - (Screen.width/4 + 10),10,Screen.width/4,Screen.height - 20), "", MenuBoxes);
		GUI.color = new Color(1,1,1,1f);

		GUILayout.BeginArea(new Rect(Screen.width - (Screen.width/4 + 10),10,Screen.width/4,Screen.height - 20));
        foreach (Player pl in GameManager.Instance.PlayerList)
        {
        	if(pl.PlayerName == GameManager.Instance.MyPlayer.PlayerName)
        	GUI.color = Color.blue;
			GUILayout.Box(pl.PlayerName, MenuButtons);
			GUI.color = Color.white;
        }
        GUILayout.EndArea();
        GUI.color = new Color(1,1,1,1);

        GUI.color = new Color(1,1,1,0.1f);
        GUI.Box(new Rect(10,10, Screen.width - (Screen.width / 4 + 30), Screen.height - 20), "", MenuBoxes);
        GUI.color = new Color(1,1,1,1f);

        GUI.Label(new Rect(20, 20, 50, 50), "Installation 01:", LOGO);
		GUI.Label(new Rect(17, 55, 50, 50), "CE3 DEMO", LOGO);

		GUI.color = new Color(1,1,1,0.4f);
		GUI.Box(new Rect(20, Screen.height/2 + 15, Screen.width - (Screen.width / 4 + 50), 30), "VANTAGE | SLAYER", MenuBoxes);
		GUI.color = new Color(1,1,1,1f);

        GUI.DrawTexture(new Rect(20,Screen.height/2 + 50, Screen.width - (Screen.width / 4 + 50), Screen.height / 3), Splash);

        if(Network.isServer)
        {
	        if(GUI.Button(new Rect(10, Screen.height - 40, Screen.width - (Screen.width / 4 + 30), 30), "Start Match", MenuButtons))
	        {
	        	GameManager.Instance.networkView.RPC("LoadLevel", RPCMode.All);
	        }
   		}
	}
}
