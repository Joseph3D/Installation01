using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerManager : MonoBehaviour 
{
	//Important Vars
    public CharacterController CharCont;
    public CharacterMotor CharMotor;

    public GUIStyle KilledPersons;

    public Camera MainCam;
	
	public Transform WalkAnimationHolder;
	public Transform FirstPerson;
    public Transform ThirdPerson;

    public AudioClip ShieldsDown;
    public AudioClip ShieldsUp;
    public bool ShieldsDownBool;

    public GameObject PlayerDead;
    public float PlayerDeadChance;

    public Texture2D PlayerDamage;
    public float PlayerDamageVar;

    public GameObject Ragdoll;
    //public Transform ThirdPerson;	

    public float WalkSpeed;
    public float RunSpeed;

    public float HealthRegen;
    public bool Regen;
    public bool GotKill;

    public int FOVHolder;
    public int ZoomFOV;
    public int NormFOV;

    public AudioClip ZoomIn;
    public AudioClip ZoomOut;
    public bool HasRegen;

    public Player LastShootBy;
    public string LastShootByName;
    public string LastShootByGun;

//    public MouseLook ML1;
//    public MouseLook ML2;

    public float NormalSens;
    public float ZoomingSens;

    public string OwnerName;

    public static PlayerManager Instance;
	 public Player MyPlayer;

	public Animation CamAnims; 

	//HiddenVars
	[HideInInspector]
	public WalkingState walkingstate = WalkingState.Idle;
	[HideInInspector]
    public float VelocityMagnitude;
    [HideInInspector]
    public bool Zooming;
    [HideInInspector]
    public Vector3 CurPos;
    [HideInInspector]
    public Quaternion CurRot;
	
	
	
	void Start () 
	{
        NormalSens = Menu.Instance.SensitivityNorm;
        ZoomingSens = Menu.Instance.SensitivityAims;

        HasRegen = true;
        HealthRegen = 0.0000001f;
        Regen = false;
        Instance = this;
        
        ShieldsDownBool = false;
        PlayerDamageVar = 0; 
		FOVHolder = NormFOV;
		FirstPerson.gameObject.SetActive(false);
		ThirdPerson.gameObject.SetActive(false);
		DontDestroyOnLoad(gameObject);

        //ML1.sensitivityX = NormalSens;
        //ML1.sensitivityY = NormalSens;

        //ML2.sensitivityX = NormalSens;
        //ML2.sensitivityY = NormalSens;

		if(networkView.isMine)
		{
			MyPlayer = GameManager.GetPlayer(OwnerName);
			MyPlayer.Manager = this;
		}

		Instance = this;
		
	}

	void Update () 
	{
        HealthRegen += Time.deltaTime;

        if(MyPlayer.Shields > 100)
            MyPlayer.Shields = 100;

        if(PlayerDamageVar < 0)
            PlayerDamageVar = 0;

        if(PlayerDamageVar > 0.2f)
            PlayerDamageVar = 0.2f;

        PlayerDamageVar -= Time.deltaTime;

	   	AnimationController();
	    SpeedController();
        
        VelocityMagnitude = CharCont.velocity.magnitude;

        MainCam.fieldOfView = Mathf.Lerp(MainCam.fieldOfView, FOVHolder, Time.deltaTime * 20);

        if(Input.GetKeyDown(KeyCode.Z) || Input.GetMouseButtonDown(2))
	    {
		    if(Zooming == false && ChooseEquipment.instance.Curweapon.CanZoom == false)
		    {
				Zooming = true;
				FOVHolder = ZoomFOV;
			    ChooseEquipment.instance.Curweapon.gameObject.SetActive(false);
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;
			    audio.PlayOneShot(ZoomIn);

                //ML1.sensitivityX = ZoomingSens;
                //ML1.sensitivityY = ZoomingSens;

                //ML2.sensitivityX = ZoomingSens;
                //ML2.sensitivityY = ZoomingSens;
                Weapon.Instance.ReloadingBool = false;
			}
			else if(Zooming == true && ChooseEquipment.instance.Curweapon.CanZoom == false)
			{
			   	Zooming = false;
			   	FOVHolder = NormFOV;
				ChooseEquipment.instance.Curweapon.gameObject.SetActive(true);
				audio.PlayOneShot(ZoomOut);
                Weapon.Instance.ReloadingBool = false;
                ChooseEquipment.instance.Curweapon.animation.Play("Draw");
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;

                //ML1.sensitivityX = NormalSens;
                //ML1.sensitivityY = NormalSens;

                //ML2.sensitivityX = NormalSens;
                //ML2.sensitivityY = NormalSens;
			}

            if(Zooming == false && ChooseEquipment.instance.Curweapon.CanZoom == true)
            {
                Zooming = true;
                FOVHolder = ZoomFOV;
                
                audio.PlayOneShot(ZoomIn);

                //ML1.sensitivityX = ZoomingSens;
                //ML1.sensitivityY = ZoomingSens;

                //ML2.sensitivityX = ZoomingSens;
                //ML2.sensitivityY = ZoomingSens;
                Weapon.Instance.ReloadingBool = false;
                ChooseEquipment.instance.Curweapon.Gun.gameObject.SetActive(false);
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;
            }
            else if(Zooming == true && ChooseEquipment.instance.Curweapon.CanZoom == true)
            {
                Zooming = false;
                FOVHolder = NormFOV;
                
                audio.PlayOneShot(ZoomOut);
                Weapon.Instance.ReloadingBool = false;
                ChooseEquipment.instance.Curweapon.Gun.animation.Play("Draw");

                //ML1.sensitivityX = NormalSens;
                //ML1.sensitivityY = NormalSens;

                //ML2.sensitivityX = NormalSens;
                //ML2.sensitivityY = NormalSens;
                ChooseEquipment.instance.Curweapon.Gun.gameObject.SetActive(true);
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;
            }

            if(Input.GetKeyDown(KeyCode.Tab))
            {
                Zooming = false;
                FOVHolder = NormFOV;
                
                audio.PlayOneShot(ZoomOut);
                Weapon.Instance.ReloadingBool = false;
                ChooseEquipment.instance.Curweapon.Gun.animation.Play("Draw");

                //ML1.sensitivityX = NormalSens;
                //ML1.sensitivityY = NormalSens;

                //ML2.sensitivityX = NormalSens;
                //ML2.sensitivityY = NormalSens;
                ChooseEquipment.instance.Curweapon.Gun.gameObject.SetActive(true);
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;

                Zooming = false;
                FOVHolder = NormFOV;
                ChooseEquipment.instance.Curweapon.gameObject.SetActive(true);
                audio.PlayOneShot(ZoomOut);
                Weapon.Instance.ReloadingBool = false;
                ChooseEquipment.instance.Curweapon.animation.Play("Draw");
                ChooseEquipment.instance.Curweapon.ReloadingBool = false;

                //ML1.sensitivityX = NormalSens;
                //ML1.sensitivityY = NormalSens;

                //ML2.sensitivityX = NormalSens;
                //ML2.sensitivityY = NormalSens;
            }
		}

        if(MyPlayer.Shields < 100)
        {
            StartCoroutine("HealthRegenB");
        }

        if(MyPlayer.Shields >= 100)
            HasRegen = true; 
    }
	
	public void SpeedController()
    {
		if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && VelocityMagnitude > 0)
        {
            walkingstate = WalkingState.Walking;
            CharMotor.movement.maxForwardSpeed = WalkSpeed;
            CharMotor.movement.maxSidewaysSpeed = WalkSpeed;
		}
        else
        {
            walkingstate = WalkingState.Idle;
        }
	}
	
	public void AnimationController()
	{
        if (walkingstate == WalkingState.Walking && CharCont.isGrounded)
        {
            WalkAnimationHolder.animation["RunRifle"].speed = VelocityMagnitude / WalkSpeed * 1.2f;
            WalkAnimationHolder.animation["TestWalk"].speed = VelocityMagnitude / WalkSpeed * 0.9f;
            WalkAnimationHolder.animation.CrossFade("TestWalk",0.1f);
        }
        else
        {
		
            WalkAnimationHolder.animation.CrossFade("TestIdle",0.2f);
        }
	}

	[RPC]
    public void RequestPlayer(string Nameeee)
    {
        networkView.RPC("GiveMyPlayer", RPCMode.OthersBuffered, Nameeee);
    }

    [RPC]
    public void GiveMyPlayer(string n)
    {
        StartCoroutine(GivePlayer(n));
    }

    IEnumerator GivePlayer(string nn)
    {
        while (!GameManager.HasPlayer(nn))
        {
            yield return new WaitForEndOfFrame();
        }
        MyPlayer = GameManager.GetPlayer(nn);
        MyPlayer.Manager = this;
    }

    [RPC]
    public void FindHitter(string name, string gun)
    {
        LastShootBy = GameManager.GetPlayer(name);
        LastShootByName = name;
        LastShootByGun = gun;
    }

    [RPC]
    public void FindHitter1(string name)
    {
        LastShootBy = GameManager.GetPlayer(name);
        LastShootByName = name;
    }

    [RPC]
    public void Spawn()
    {
        //GameManager.Instance.RespawnTime = 3;
        MyPlayer.Shields = 100; 
    	MyPlayer.Health = 100;
		MyPlayer.isAlive = true;

		    if(networkView.isMine)
		    {
			    FirstPerson.gameObject.SetActive(true);
			    ThirdPerson.gameObject.SetActive(false);
                ShieldsDownBool = false;
		    }
		    else
		    {
			     FirstPerson.gameObject.SetActive(false);
			     ThirdPerson.gameObject.SetActive(true);
		    }
    }

    [RPC]
    public void Server_TakeDamage(float Damage)
    {
        Debug.Log("Recieved");
        Client_TakeDamage(Damage);
    }

    public void Client_TakeDamage(float Damage)
    {
        StopCoroutine("HealthRegenB");
        audio.Stop();
        PlayerDamageVar = 0;

        if(HasRegen == true)
            HasRegen = false;

        if(MyPlayer.Shields > 0)
           MyPlayer.Shields -= Damage;
        else if (MyPlayer.Shields <= 0)
            MyPlayer.Health -= (Damage * 4);

        if(networkView.isMine)
           PlayerDamageVar += 0.2f;

        if(MyPlayer.Shields <= 0 && ShieldsDownBool == false && networkView.isMine)
        {
            ShieldsDownBool = true; 
            audio.PlayOneShot(ShieldsDown);
        }

        if(MyPlayer.Health <= 0)
        {
            MyPlayer.Health = 0;
            Die();
        }
    }

    [RPC]
    public void Server_TakeDamage1(float Damage)
    {
        Debug.Log("Recieved");
        Client_TakeDamage1(Damage);
    }

    public void Client_TakeDamage1(float Damage)
    {
        StopCoroutine("HealthRegenB");
        audio.Stop();
        PlayerDamageVar = 0;

        if(HasRegen == true)
            HasRegen = false;

        MyPlayer.Health -= (Damage * 4);

        if(networkView.isMine)
           PlayerDamageVar += 0.2f;

        if(MyPlayer.Shields <= 0 && ShieldsDownBool == false && networkView.isMine)
        {
            ShieldsDownBool = true; 
            audio.PlayOneShot(ShieldsDown);
        }

        if(MyPlayer.Health <= 0)
        {
            MyPlayer.Health = 0;
            Die();
        }
    }

    IEnumerator HealthRegenB()
    {
        yield return new WaitForSeconds(6);

        if(HasRegen == false)
        {
             audio.PlayOneShot(ShieldsUp);
             HasRegen = true;
        }

        Regen = true;

        //if(MyPlayer.Shields < 100)
            Server_UpdateHealth(0.1f);

        if(MyPlayer.Shields >= 100)
            StopCoroutine("HealthRegenB");
    }

    public void Server_UpdateHealth(float health)
    {
        if(GameManager.Instance.MatchStarted)
            networkView.RPC("Client_UpdateHealth", RPCMode.All, health);
    }

    [RPC]
    public void Client_UpdateHealth(float health)
    {
        MyPlayer.Shields += (health * 4) ;
    }



    public void Die()
    {
        if(networkView.isMine)
        {
            Zooming = false;
            FOVHolder = NormFOV;
                
            audio.PlayOneShot(ZoomOut);
            Weapon.Instance.ReloadingBool = false;
            ChooseEquipment.instance.Curweapon.Gun.animation.Play("Draw");

            //ML1.sensitivityX = NormalSens;
            //ML1.sensitivityY = NormalSens;

            //ML2.sensitivityX = NormalSens;
            //ML2.sensitivityY = NormalSens;

            ChooseEquipment.instance.Curweapon.Gun.gameObject.SetActive(true);
            ChooseEquipment.instance.Curweapon.ReloadingBool = false;

            ChooseEquipment.instance.Curweapon = ChooseEquipment.instance.Primary;
        }

        if(PlayerDeadChance <= 4)
            Network.Instantiate(PlayerDead, FirstPerson.position, FirstPerson.rotation, 0);

        if(networkView.isMine)
            Network.Instantiate(Ragdoll, FirstPerson.position + new Vector3(0,-2,0), FirstPerson.rotation, 0);

        FirstPerson.gameObject.SetActive(false);
        ThirdPerson.gameObject.SetActive(false);
        PlayerDeadChance = Random.Range(0,8);
        MyPlayer.isAlive = false;

        MyPlayer.Deaths++;

        if (LastShootBy != null)
        {
            LastShootBy.Manager.GetKill();
        }
    }

    public void GetKill()
    {
        ShowMyKill(MyPlayer.PlayerName);
        StartCoroutine("Killed");
        if(networkView.isMine)
            GotKill = true;
    }

    IEnumerator Killed()
    {
        yield return new WaitForSeconds(2);
        GotKill = false; 
    }

    public void ShowMyKill(string MyName)
    {
        GameManager.GetPlayer(MyName).Score += 100;
        GameManager.GetPlayer(MyName).Kills++;
    }

    void FixedUpdate()
    {
		//GotKillTime();
  		if(networkView.isMine)
  		{
   			CurPos = FirstPerson.position;
   			CurRot = FirstPerson.rotation;
  		}
  		else
  		{
  			ThirdPerson.position = CurPos + new Vector3(0,-2,0);
   			ThirdPerson.rotation = CurRot;
  		}
 	}
 
 	void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
 	{
  		if(stream.isWriting)
 		{
   			stream.Serialize(ref CurPos);
   			stream.Serialize(ref CurRot);

            char Ani = (char)GetComponent<NetworkAnimStates>().CurrentAnim;
            stream.Serialize(ref Ani);
  		}
 		else
  		{
   			stream.Serialize(ref CurPos);
   			stream.Serialize(ref CurRot);

            char Ani = (char)0;
            stream.Serialize(ref Ani);
            GetComponent<NetworkAnimStates>().CurrentAnim = (Animations)Ani;
  		}
 	}

    public void OnGUI()
    {
        GUI.color = new Color(1,1,1, PlayerDamageVar);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), PlayerDamage);
        GUI.color = new Color(1,1,1,1);

        if(GotKill == true)
            GUI.Label(new Rect(10, Screen.height / 2 + (Screen.height / 6), 200, 30), "Player Killed", KilledPersons);
    }
}

public enum WalkingState
{
    Idle,
    Walking,
    Running
}


