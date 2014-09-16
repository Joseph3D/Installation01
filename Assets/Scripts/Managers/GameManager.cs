using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
 	public static GameManager Instance; 
 	public Player MyPlayer; 

 	public List<Player> PlayerList = new List<Player>();

 	public string PlayerName;

 	public GameObject SpawnPlayer;
    public float RespawnTime;
    public float ScreenBlackValue;
    public string RespawnString;

    public bool MatchStarted;
    public GUIStyle SpawnStyle;

    public AudioClip RespawnSound1;
    public AudioClip RespawnSound2;
    public AudioClip RespawnSound3;
    public AudioClip RespawnSound4;
    public AudioClip Slayer;

    public bool hasBeen1;
    public bool hasBeen2;
    public bool hasBeen3;
    public bool hasBeen4;

    public Texture2D ScreenBlack;
	
	void Start () 
	{
        ScreenBlackValue = 0.5f;
        hasBeen1 = false;
        hasBeen2 = false;
        hasBeen3 = false;
        hasBeen4 = false;

		Instance = this;
        MatchStarted = false; 
		PlayerName = PlayerPrefs.GetString("name");

        RespawnTime = 3; 

        DontDestroyOnLoad(gameObject);
	}
	
	
	void Update () 
	{
        RespawnString = RespawnTime.ToString("F0");

        if(MyPlayer.isAlive)
            ScreenBlackValue -= Time.deltaTime;

        if(ScreenBlackValue < 0)
            ScreenBlackValue = 0; 

	    if(RespawnTime > 3)
            RespawnTime = 3;

        if(MatchStarted && !MyPlayer.isAlive)
            RespawnTime -= Time.deltaTime;

        if(RespawnString == "3" &&  MyPlayer.isAlive == false && MatchStarted && !hasBeen1 && !Application.isLoadingLevel)
        {
            audio.PlayOneShot(RespawnSound1);
            hasBeen1 = true;
        }

        if(RespawnString == "2" &&  MyPlayer.isAlive == false && MatchStarted && !hasBeen2)
        {
            audio.PlayOneShot(RespawnSound2);
            hasBeen2 = true;
        }

        if(RespawnString == "1" &&  MyPlayer.isAlive == false && MatchStarted && !hasBeen3)
        {
            audio.PlayOneShot(RespawnSound3);
            hasBeen3 = true;
        }

        if(RespawnString == "0" &&  MyPlayer.isAlive == false && MatchStarted && !hasBeen4)
        {
            audio.PlayOneShot(RespawnSound4);
            hasBeen4 = true;
        }
    }

    /*IEnumerator Respawn()
    {
        yield return new WaitForSeconds(1);
        RespawnTime -= Time.deltaTime;
    }*/

	public void StartServer(string ServerName, int MaxPlayers)
    {
        Network.InitializeSecurity();
        Network.InitializeServer(MaxPlayers, 25565, true);
        MasterServer.RegisterHost("IO1", ServerName, "");

        Network.useNat = !Network.HavePublicAddress();

        //Debug.Log("Server Started Successfully");
    }

    void OnPlayerConnected(NetworkPlayer id)
    {
      	foreach (Player pl in PlayerList)
     	{
            networkView.RPC("Client_PlayerJoined", id, pl.PlayerName, pl.OnlinePlayer);
        }
    }

    void OnServerInitialized()
    {
        Server_PlayerJoined(PlayerName, Network.player);
    }

    void OnConnectedToServer()
    {
    	Debug.Log("gesgesg");
        networkView.RPC("Server_PlayerJoined", RPCMode.Server, PlayerName, Network.player);
    }

    void OnPlayerDisconnected(NetworkPlayer id)
    {
        Debug.Log("A player is leaving");
        networkView.RPC("RemovePlayer", RPCMode.All, id);
        Network.Destroy(GetPlayer(id).Manager.gameObject);
        Network.RemoveRPCs(id);
        //Network.Disconnect(
        Debug.Log("A player left");
    }

    void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Debug.Log("you are leaving");
        foreach (Player pl in PlayerList)
        {
            Network.Destroy(pl.Manager.gameObject);
        }
        PlayerList.Clear();
        Application.LoadLevel(0);
        Debug.Log("you left");
    }

    [RPC]
    public void Server_PlayerJoined(string Username, NetworkPlayer id)
    {
        networkView.RPC("Client_PlayerJoined", RPCMode.All,Username, id);
    }

    [RPC]
    public void Client_PlayerJoined(string Username,  NetworkPlayer id)
    {
        Player temp = new Player();
        temp.PlayerName = Username;
        temp.OnlinePlayer = id;
        
        PlayerList.Add(temp);
        if (Network.player == id)
        {
            MyPlayer = temp;
            GameObject LastPlayer = Network.Instantiate(SpawnPlayer, Vector3.zero, Quaternion.identity, 0) as GameObject;
            LastPlayer.networkView.RPC("RequestPlayer", RPCMode.AllBuffered, Username);
            LastPlayer.GetComponent<PlayerManager>().OwnerName = Username;
            //temp.Manager = LastPlayer.GetComponent<UserPlayer>();
        }
    }

    [RPC]
    public void LoadLevel()
    {
        Application.LoadLevel("Test Level");
        StartCoroutine("Slaye2");
        MatchStarted = true; 
    }

    IEnumerator Slaye2()
    {
        yield return new WaitForSeconds(5);
        audio.PlayOneShot(Slayer);
    }

    public static Player GetPlayer(NetworkPlayer id)
    {
        foreach (Player pl in Instance.PlayerList)
        {
            if (pl.OnlinePlayer == id)
                return pl;
        }
        return null;
    }

   	public static Player GetPlayer(string id)
    {
        foreach (Player pl in Instance.PlayerList)
        {
            if (pl.PlayerName == id)
                return pl;
        }
        return null;
    }

    public static bool HasPlayer(string n)
    {
        foreach (Player pl in Instance.PlayerList)
        {
            if (pl.PlayerName == n)
                return true;
        }
        return false;
    }

    public void OnGUI()
    {
        if(MatchStarted && MyPlayer.isAlive)
        {
            GUI.color = new Color(1,1,1, ScreenBlackValue);
            GUI.DrawTexture(new Rect(0,0,Screen.width, Screen.height), ScreenBlack);
        }

        if(MatchStarted == true && !MyPlayer.isAlive && !Application.isLoadingLevel)
        {
            GUI.Label(new Rect(Screen.width/2 - 75, 50 , Screen.width/3 - 100, 300), "Respawn in: " + RespawnTime.ToString("F0"), SpawnStyle);
            if(RespawnTime <= 0)
            {
                int SpawnIndex = Random.Range(0,SpawnPointManager.Instance.SpawnPoints.Length -1);
                MyPlayer.Manager.FirstPerson.localPosition = SpawnPointManager.Instance.SpawnPoints[SpawnIndex].transform.position;
                MyPlayer.Manager.FirstPerson.localRotation = SpawnPointManager.Instance.SpawnPoints[SpawnIndex].transform.rotation;

                MyPlayer.Manager.networkView.RPC("Spawn", RPCMode.AllBuffered);
                ScreenBlackValue = 0.5f;
                RespawnTime = 3;
                Screen.lockCursor = true;

                hasBeen1 = false;
                hasBeen2 = false;
                hasBeen3 = false;
                hasBeen4 = false;

               // PlayerManager.Instance.Zooming = false;
                
                ChooseEquipment.instance.Curweapon.animation.Play("Draw");
                ChooseEquipment.instance.Primary.GameSpareAmmo = ChooseEquipment.instance.Primary.SpareAmmo;
                ChooseEquipment.instance.Primary.GameMagAmmo = ChooseEquipment.instance.Primary.MagAmmo;

                ChooseEquipment.instance.Secondary.GameSpareAmmo = ChooseEquipment.instance.Secondary.SpareAmmo;
                ChooseEquipment.instance.Secondary.GameMagAmmo = ChooseEquipment.instance.Secondary.MagAmmo;

                ThrowGrenade.Instance.GrenadeCountActualFrag = ThrowGrenade.Instance.GrenadeCountStartingFrag;
                ThrowGrenade.Instance.GrenadeCountActualPlasma = ThrowGrenade.Instance.GrenadeCountStartingPlasma;

                ChooseEquipment.instance.Spawn();

                NetworkAnimController.Instance.isMeleeing = false;

                PlayerManager.Instance.ML1.sensitivityX = PlayerManager.Instance.NormalSens;
                PlayerManager.Instance.ML1.sensitivityY = PlayerManager.Instance.NormalSens;

                PlayerManager.Instance.ML2.sensitivityX = PlayerManager.Instance.NormalSens;
                PlayerManager.Instance.ML2.sensitivityY = PlayerManager.Instance.NormalSens;

                Weapon.Instance.ReloadingBool = false;
            }
        }

    }
}

[System.Serializable]
public class Player
{
	public string PlayerName;

	public float Health;
    public float Shields; 

	public int EXP;
    public int Score;
    public int Kills;
    public int Deaths;
	public int Rank;

	public NetworkPlayer OnlinePlayer;
	public PlayerManager Manager;
    
	public bool isAlive; 
}