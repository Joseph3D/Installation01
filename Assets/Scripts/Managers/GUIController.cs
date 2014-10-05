using UnityEngine;
using System.Collections;

public class GUIController : MonoBehaviour 
{
	public static GUIController Instance;

	public GUIStyle FireModeBox;
	public GUIStyle GunNameBox;
	public GUIStyle AmmoBox;
	public GUIStyle GUIButton;
	public GUIStyle GUIBox;

	public Texture2D HUDOverlay;
	public Texture2D ShieldBar;
	public Texture2D ShieldBarOutLine;
	public Texture2D AR;
	public Texture2D Magnum;
	public Texture2D Minimap;
	public Texture2D ZoomTexture;
	public Texture2D Crosshair;
	public Texture2D Health;
	public Texture2D Scoreboard;
	public Texture2D FragG;
	public Texture2D PlasmsaG;

	public bool MenuOpen;
	public bool ScoreboardOpen;

	public float MinimapTimer;
	public bool MapPulse;
	
	void Start () 
	{
		MenuOpen = false;
		Instance = this;
		MapPulse = true;  
		ScoreboardOpen = false;
	}
	
	
	void Update ()
	{
		if(MinimapTimer > 0.3f)
			MinimapTimer = 0.3f; 

		if(MapPulse == true)
		{
			MinimapTimer += Time.deltaTime;

			if(MinimapTimer >= 0.3f)
				MapPulse = false;
		}

		if(MapPulse == false)
		{
			StartCoroutine("PulseDown");
		}

		if(Input.GetKeyDown(KeyCode.Escape) )
		{
			if(MenuOpen == false)
			{
				Screen.lockCursor = false;
				MenuOpen = true;

                //PlayerManager.Instance.ML1.sensitivityX = 0;
                //PlayerManager.Instance.ML1.sensitivityY = 0;

                //PlayerManager.Instance.ML2.sensitivityX = 0;
                //PlayerManager.Instance.ML2.sensitivityY = 0;

			}
			else if(MenuOpen == true)
			{
				Screen.lockCursor = true;
				MenuOpen = false;

                //PlayerManager.Instance.ML1.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML1.sensitivityY = PlayerManager.Instance.NormalSens;

                //PlayerManager.Instance.ML2.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML2.sensitivityY = PlayerManager.Instance.NormalSens;
			}
		}

		if(Input.GetKeyDown(KeyCode.F1))
		{
			if(ScoreboardOpen == false)
			{
				ScoreboardOpen = true;
                //PlayerManager.Instance.ML1.sensitivityX = 0;
                //PlayerManager.Instance.ML1.sensitivityY = 0;

                //PlayerManager.Instance.ML2.sensitivityX = 0;
                //PlayerManager.Instance.ML2.sensitivityY = 0;
			}
			else if (ScoreboardOpen == true)
			{
				ScoreboardOpen = false;
                //PlayerManager.Instance.ML1.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML1.sensitivityY = PlayerManager.Instance.NormalSens;

                //PlayerManager.Instance.ML2.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML2.sensitivityY = PlayerManager.Instance.NormalSens;
			}
		}
	}


	IEnumerator PulseDown()
	{
		MinimapTimer -= Time.deltaTime;
		yield return new WaitForSeconds(1);

		if(MinimapTimer <= 0)
				MapPulse = true; 
	}

	public void OnGUI()
	{
		if(MenuOpen == true && GameManager.Instance.MatchStarted && GameManager.Instance.MyPlayer.isAlive)
		{
			GUI.Box(new Rect(10,10, Screen.width  - 20, Screen.height - 20), "", GUIBox);
	
			GUI.color = new Color(1,1,1,0.6f);
			if(GUI.Button(new Rect(10,10,Screen.width - 20, 30), "Resume Game", GUIButton))
			{
				MenuOpen = false;

                //PlayerManager.Instance.ML1.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML1.sensitivityY = PlayerManager.Instance.NormalSens;

                //PlayerManager.Instance.ML2.sensitivityX = PlayerManager.Instance.NormalSens;
                //PlayerManager.Instance.ML2.sensitivityY = PlayerManager.Instance.NormalSens;
			}

			if(GUI.Button(new Rect(10,40,Screen.width - 20, 30), "Suicide", GUIButton))
			{
				MenuOpen = false;
				GameManager.Instance.MyPlayer.Manager.networkView.RPC("Server_TakeDamage",RPCMode.All, 100.0f);
				GameManager.Instance.MyPlayer.Manager.networkView.RPC("Server_TakeDamage",RPCMode.All, 100.0f);
			}

			if(GUI.Button(new Rect(10,70,Screen.width - 20, 30), "Quit Installation 01", GUIButton))
			{
				Network.Disconnect();
				Application.Quit();
			}
			GUI.color = new Color(1,1,1,1f);

		}

		if(PlayerManager.Instance == null || GameManager.Instance == null || GameManager.Instance.MyPlayer == null) {
			return;
		}

		if(PlayerManager.Instance.Zooming == false && GameManager.Instance.MatchStarted && GameManager.Instance.MyPlayer.isAlive && MenuOpen == false)
		{
			if(ScoreboardOpen == true)
			{
				GUI.DrawTexture (new Rect(Screen.width / 4 - 50, Screen.height/8 - 50, (Screen.width) - (Screen.width /2) + 150, (Screen.height) - (Screen.height /2) + 200), Scoreboard);
				
				GUILayout.BeginArea(new Rect(Screen.width / 4, Screen.height/4 - 10, Screen.width /2, (Screen.height) - (Screen.height /2)), "");
					
				foreach(Player pl in GameManager.Instance.PlayerList)
				{
					if(pl.PlayerName == GameManager.Instance.MyPlayer.PlayerName)
        				GUI.color = Color.blue;
					GUILayout.BeginHorizontal();
					GUILayout.Box(pl.PlayerName, GUIButton);
					GUILayout.Box(pl.Score.ToString(), GUIButton);
					GUILayout.Box(pl.Kills.ToString(), GUIButton);
					GUILayout.Box(pl.Deaths.ToString(), GUIButton);
					GUILayout.EndHorizontal();
						GUI.color = Color.white;
						
				}
				GUILayout.EndArea();
			}
		}
		//GUI.Box(new Rect(10,Screen.height - 65,120,25), Weapon.Instance.WeaponName, GunNameBox);
		//GUI.Box(new Rect(10,Screen.height - 35,120,25), Weapon.Instance.GameMagAmmo.ToString() + " / " + Weapon.Instance.GameSpareAmmo.ToString(), AmmoBox);
		//GUI.Label(new Rect(10,10,200,200),GameManager.Instance.MyPlayer.Shields.ToString());
		if(PlayerManager.Instance.Zooming == false && GameManager.Instance.MatchStarted && GameManager.Instance.MyPlayer.isAlive && MenuOpen == false)
		{
			if(ChooseEquipment.instance.Curweapon.AimingPerson == false)
				GUI.color = new Color(1,1,1,0.5f);
			else if(ChooseEquipment.instance.Curweapon.AimingPerson == true)
				GUI.color = new Color(1,0,0,0.5f);

			GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height + 20), Crosshair);
			GUI.color = new Color(1,1,1,1f);

			GUI.color = new Color(1,1,1,0.1f);
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), HUDOverlay);
			GUI.color = new Color(1,1,1,1);

			GUI.color = new Color(1,1,1,0.8f);
			GUI.DrawTexture(new Rect(Screen.width/3 + 50, -110 , Screen.width/3 - 100, 300), ShieldBarOutLine);
			//GUI.DrawTexture(new Rect(Screen.width/3 + 58, -112 , Screen.width/3 - 115, 300), ShieldBar);

			float adjust = GameManager.Instance.MyPlayer.Shields*(Screen.height/140);

			GUI.BeginGroup(new Rect(Screen.width/3 + 50, 10, adjust,100));
			GUI.DrawTexture(new Rect(8,6,Screen.width/3 - 115,30), ShieldBar);
			GUI.EndGroup();

			float adjust1 = GameManager.Instance.MyPlayer.Health*(Screen.height/140);

			GUI.BeginGroup(new Rect(Screen.width/3 + 50, 10, adjust1,100));
			GUI.DrawTexture(new Rect(8,6,Screen.width/3 - 115,30), Health);
			GUI.EndGroup();

			GUI.Label(new Rect(Screen.width - 100,0,100,50), ChooseEquipment.instance.Curweapon.GameMagAmmo.ToString() + " / " + ChooseEquipment.instance.Curweapon.GameSpareAmmo.ToString(), AmmoBox);
			GUI.Label(new Rect(Screen.width - 100,25, Screen.width/8, 100), ChooseEquipment.instance.Curweapon.HUDTexture);
			GUI.color = new Color(1,1,1,1);

			GUI.color = new Color(1,1,1,0.6f);
			GUI.DrawTexture(new Rect(-5,Screen.height - (Screen.height / 4 + 5),Screen.width/8,Screen.width/8), Minimap); 
			GUI.color = new Color(1,1,1,1);

			GUI.color = new Color(1,1,1,MinimapTimer);
			GUI.DrawTexture(new Rect(-5,Screen.height - (Screen.height / 4 + 5),Screen.width/8,Screen.width/8), Minimap); 
			GUI.color = new Color(1,1,1,1);

			if(ThrowGrenade.Instance.isFrag)
				GUI.color = new Color(1,1,1,0.5f);
			else
				GUI.color = new Color(1,1,1,0.2f);

			GUI.DrawTexture(new Rect(5,10, 50, 50), FragG);
			GUI.Label(new Rect(8,5,10,10), ThrowGrenade.Instance.GrenadeCountActualFrag.ToString(), AmmoBox);
			GUI.color = new Color(1,1,1,1);

			if(!ThrowGrenade.Instance.isFrag)
				GUI.color = new Color(1,1,1,0.5f);
			else
				GUI.color = new Color(1,1,1,0.2f);

			GUI.Label(new Rect(60,10, 50, 50), PlasmsaG);
			GUI.Label(new Rect(63,5,10,10), ThrowGrenade.Instance.GrenadeCountActualPlasma.ToString(), AmmoBox);
			GUI.color = new Color(1,1,1,1);
		}
		else if(GameManager.Instance.MatchStarted && PlayerManager.Instance.Zooming == true && GameManager.Instance.MyPlayer.isAlive && ChooseEquipment.instance.Curweapon.CanZoom == false)
		{
			GUI.color = new Color(1,1,1,0.7f);
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), ZoomTexture);
			GUI.color = new Color(1,1,1,1);
		}
		else if(GameManager.Instance.MatchStarted && PlayerManager.Instance.Zooming == true && GameManager.Instance.MyPlayer.isAlive && ChooseEquipment.instance.Curweapon.CanZoom == true)
		{
			GUI.color = new Color(1,1,1,1f);
			GUI.DrawTexture(new Rect(0,0, Screen.width, Screen.height), ChooseEquipment.instance.Curweapon.ZoomTexture);
			GUI.color = new Color(1,1,1,1);
		}
	}
}
