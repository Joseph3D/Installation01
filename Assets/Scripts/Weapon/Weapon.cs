using UnityEngine;
using System.Collections;

public class Weapon : MonoBehaviour 
{
	//Variables to be seen For Weapon
	public static Weapon Instance;

	public string WeaponName;
	public string WeaponDescription; 

	public int MagAmmo;
	public int SpareAmmo;

	public FireMode firemode;

	public float MaxDamage;
	public float MinDamage;

	public float AutoTimer;
	public float MeleeTimer;

	public Texture2D ZoomTexture;
	public Texture2D HUDTexture;

	public TextMesh AmmoCounter;
	public bool HasAmmoCounter;

	public bool CanChangeFireMode;
	public bool CanZoom;
	public bool CanHeadshot;

	public AudioClip ShootSound;
	public GameObject ShootSoundObject;
	public AudioClip ReloadMagOut; 
	public AudioClip ReloadMagIn;
	public AudioClip SwitchFireModeSound;
	public AudioClip MeleeSoundObject;
	public AudioClip MeleeSoundNothing;
	public AudioClip DrawSound;
	public AudioClip MeleeSoundPlayer;
	public AudioClip HitMark;

	public Vector3 KickRotation;
    public Vector3 KickKickback;

	public Transform MuzzleFlashHolder;
	public Transform SpawnPoint;
	public Transform MeleeSpawnPoint;
	public Transform CamRecoilHolder;
	public Transform CamMeleeHolder;

	public GameObject Gun;
	public GameObject MuzzleFlash;
	public GameObject Bullet;

	public GameObject Sparks;
	public GameObject DirtImpact;
	public GameObject Wood;
	public GameObject Concrete; 

	public GameObject ShieldHit;
	public GameObject Playerhit; 
	public bool isMelee;

	//Not important and so hidden
	[HideInInspector]
	public int GameMagAmmo;
	[HideInInspector]
	public bool AimingPerson;
	[HideInInspector]
	public int GameSpareAmmo;
	[HideInInspector]
	public bool ReloadingBool;
	[HideInInspector]
	public float AutoCooler;
	[HideInInspector]
	public int BulletMagReload;
	[HideInInspector]
	public bool FullReload;
	[HideInInspector]
	public Camera WeaponCam;
	[HideInInspector]
	public Vector3 KickHolder;
	[HideInInspector]
	public Vector3 KickHolder1;
	[HideInInspector]
	public float MeleeCooler;
	[HideInInspector]
	public Vector3 CamMelee;
	[HideInInspector]
	public Vector3 CamMelee1;
	[HideInInspector]
	public int WeaponAnimValue;
	[HideInInspector]
	public float ActualDamage;
	[HideInInspector]
	public bool AutoReload;
	

	void Start () 
	{
		AutoReload = false;
		AimingPerson = false;
		Instance = this;
		FullReload = false;
		isMelee = false;
		//Zooming = false;

		AutoCooler = AutoTimer;
		MeleeCooler = MeleeTimer;

		GameMagAmmo = MagAmmo;
		GameSpareAmmo = SpareAmmo;

		ReloadingBool = false;

		ChooseEquipment.instance.Curweapon.Gun.animation.Play("Draw");
		audio.PlayOneShot(DrawSound);
	}
	
	
	void Update () 
	{
		CrosshairChangeColor();

		if(HasAmmoCounter)
			AmmoCounter.text = GameMagAmmo.ToString();
		else
			AmmoCounter.text = "";

		if(AutoCooler >= AutoTimer)
			AutoCooler = AutoTimer;

		if(MeleeCooler >= MeleeTimer)
			MeleeCooler = MeleeTimer;

		AutoCooler += Time.deltaTime;
		MeleeCooler += Time.deltaTime;

		if (GameSpareAmmo < 0)
		{
			GameSpareAmmo = 0;
		}

		//Weapon stuff
		if(GameMagAmmo != 0 && ReloadingBool == false && MeleeCooler >= MeleeTimer && ThrowGrenade.Instance.CanFire == false)
		{
			if(firemode == FireMode.Auto)
			{
				if(Input.GetMouseButton(0)  && MeleeCooler >= MeleeTimer)
				{
					if(AutoCooler >= AutoTimer)
						Fire();
				}
			}

			if(firemode == FireMode.Single || firemode == FireMode.Sniper)
			{
				if(Input.GetMouseButtonDown(0)  && MeleeCooler >= MeleeTimer)
				{
					if(AutoCooler >= AutoTimer)
						Fire();
				}
			}
		}

		if(Input.GetKeyDown(KeyCode.R) && GameMagAmmo != 0 && ReloadingBool == false && GameMagAmmo != MagAmmo && GameSpareAmmo != 0 && ThrowGrenade.Instance.CanFire == false)
		{
			StartCoroutine("Reload");
			BulletMagReload = GameMagAmmo;
			FullReload = false;
			ReloadingBool = true;
			audio.PlayOneShot(ReloadMagIn);
		}

		if(Input.GetKeyDown(KeyCode.F) && ReloadingBool == false && ThrowGrenade.Instance.CanFire == false)
		{
			if(MeleeCooler >= MeleeTimer)
			{
				Melee();
				isMelee = false; 
			}
		}
	}

	void FixedUpdate()
	{
		KickController();
	}

	public void KickController()
	{
		KickHolder = Vector3.Lerp(KickHolder, Vector3.zero, 0.4f);
        KickHolder1 = Vector3.Lerp(KickHolder1, KickHolder, 0.4f);

        CamMelee = Vector3.Lerp(CamMelee,Vector3.zero,0.04f);
		CamMelee1 = Vector3.Lerp(CamMelee1,CamMelee,0.5f);

		CamRecoilHolder.localEulerAngles = KickHolder1 * 40.2f;	
        CamMeleeHolder.localEulerAngles = CamMelee1 * 8;
    }

    public void CrosshairChangeColor()
    {
    	RaycastHit hit;

    	if(Physics.Raycast(SpawnPoint.position, SpawnPoint.forward, out hit, 100))
		{
			PlayerManager hitter = hit.transform.root.GetComponent<PlayerManager>();

			if(hitter != null)
			{
				AimingPerson = true;
			}
			else
				AimingPerson = false;
		}
    }

	public void Fire()
	{
		if(GUIController.Instance.MenuOpen == false)
			Screen.lockCursor = true;
		AutoCooler = 0;
		GameMagAmmo--;

		//Debug.Log("egsg");

		WeaponAnimValue = Random.Range(1,10);

		if(WeaponAnimValue >= 5)
		{
			ChooseEquipment.instance.Curweapon.Gun.animation.Rewind("Fire1New");
			ChooseEquipment.instance.Curweapon.Gun.animation.Play("Fire1New");
		}
		else
		{
			ChooseEquipment.instance.Curweapon.Gun.animation.Rewind("Fire2New");
			ChooseEquipment.instance.Curweapon.Gun.animation.Play("Fire2New");
		}
		//audio.PlayOneShot(ShootSound);

		KickHolder += new Vector3(KickRotation.x, Random.Range(-KickRotation.y, KickRotation.y));

		Network.Instantiate(MuzzleFlash, MuzzleFlashHolder.transform.position, MuzzleFlashHolder.transform.rotation, 0);
		Network.Instantiate(ShootSoundObject, MuzzleFlashHolder.transform.position, MuzzleFlashHolder.transform.rotation, 0);

		GameObject InsantiateBullet = Instantiate(Bullet, MuzzleFlashHolder.transform.position, MuzzleFlashHolder.transform.rotation) as GameObject;
        InsantiateBullet.rigidbody.AddRelativeForce(Vector3.forward, ForceMode.Impulse);

        RaycastHit hit;

        if(Physics.Raycast(SpawnPoint.position, SpawnPoint.forward, out hit, 800))
		{
			if(hit.collider.tag == "Dirt")
			{
				Network.Instantiate(DirtImpact,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
			}

			if(hit.collider.tag == "Metal")
			{
				Network.Instantiate(Sparks,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
			}

			if(hit.collider.tag == "Player")
			{
				Network.Instantiate(Sparks,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
			}

			if(hit.collider.tag == "concrete")
			{
				Network.Instantiate(Concrete,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
			}

			if(hit.collider.tag == "wood")
			{
				Network.Instantiate(Wood,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
			}


			PlayerManager hitter = hit.transform.root.GetComponent<PlayerManager>();

			if(hitter != null)
			{
				audio.PlayOneShot(HitMark);
				ActualDamage = Random.Range(MinDamage, MaxDamage);
				if(hit.collider.tag == "Head")
				{
					if(firemode == FireMode.Sniper)
					{
						hitter.networkView.RPC("Server_TakeDamage1", RPCMode.All, 100.0f);
						hitter.networkView.RPC("FindHitter", RPCMode.All, GameManager.Instance.MyPlayer.PlayerName, WeaponName);
						Debug.Log("PLOXPLOXPLOXPLOX");
					}
					else
					{
						hitter.networkView.RPC("Server_TakeDamage", RPCMode.All, Random.Range(MinDamage, MaxDamage) * 2);
						hitter.networkView.RPC("FindHitter", RPCMode.All, GameManager.Instance.MyPlayer.PlayerName, WeaponName);
						Debug.Log("lelelelelelelel");
					}
				}
				else if(hit.collider.tag == "Player")
				{
					hitter.networkView.RPC("Server_TakeDamage", RPCMode.All, Random.Range(MinDamage, MaxDamage));
					hitter.networkView.RPC("FindHitter", RPCMode.All, GameManager.Instance.MyPlayer.PlayerName, WeaponName);
				}
				Debug.Log("Sent");
			}
			
		}

		if(GameMagAmmo <= 0 && GameSpareAmmo != 0)
		{
			FullReload = true;
			GameMagAmmo = 0;
			StartCoroutine("Reload");
			ReloadingBool = true;
			audio.PlayOneShot(ReloadMagOut); 
		}
	}

	public void Melee()
	{
		MeleeCooler = 0;
		isMelee = true;
		RaycastHit hit;

		ChooseEquipment.instance.Curweapon.Gun.animation.Rewind("MeleeNew2");
		ChooseEquipment.instance.Curweapon.Gun.animation.Play("MeleeNew2");

		CamMelee += new Vector3(-0.5f,0.5f,0.0f);

        if(Physics.Raycast(MeleeSpawnPoint.position, MeleeSpawnPoint.forward, out hit, 2))
		{
			PlayerManager hitter = hit.transform.root.GetComponent<PlayerManager>();

			if(hitter != null)
			{
				ActualDamage = Random.Range(MinDamage, MaxDamage);
				hitter.networkView.RPC("Server_TakeDamage", RPCMode.All, 100.0f);
				hitter.networkView.RPC("FindHitter", RPCMode.All, GameManager.Instance.MyPlayer.PlayerName, WeaponName);
				Debug.Log("Sent");
				audio.PlayOneShot(MeleeSoundPlayer);
			}
			else
			{
				Network.Instantiate(Sparks,hit.point,Quaternion.FromToRotation(Vector3.up, hit.normal), 0);
				audio.PlayOneShot(MeleeSoundObject);
			}
		}
		else
			audio.PlayOneShot(MeleeSoundNothing);
	}

	[RPC]
	public void BulletTracers()
	{
		GameObject InsantiateBullet = Instantiate(Bullet, MuzzleFlashHolder.transform.position, MuzzleFlashHolder.transform.rotation) as GameObject;
        InsantiateBullet.rigidbody.AddRelativeForce(Vector3.forward, ForceMode.Impulse);
	}

	IEnumerator Reload()
	{
		if(firemode == FireMode.Sniper && FullReload == true)
			yield return new WaitForSeconds(1);

		ChooseEquipment.instance.Curweapon.Gun.animation.Rewind("ReloadNew2");
		ChooseEquipment.instance.Curweapon.Gun.animation.Play("ReloadNew2");

		if(firemode == FireMode.Sniper)
			yield return new WaitForSeconds(2f);
		else
			yield return new WaitForSeconds(1.5f);
			
		audio.PlayOneShot(ReloadMagOut); 
		ReloadingBool = false; 

		if(GameSpareAmmo != 0 && FullReload == false && GameSpareAmmo > (MagAmmo - BulletMagReload))
		{
			Debug.Log("segseg");
			GameMagAmmo = MagAmmo;
			GameSpareAmmo -= (MagAmmo - BulletMagReload);
		}
		else if (GameSpareAmmo != 0 && FullReload == true && GameSpareAmmo >= (MagAmmo - BulletMagReload))
		{
			Debug.Log("awfawf");
			GameMagAmmo = MagAmmo;
			GameSpareAmmo -= MagAmmo;
		}
		else if(GameSpareAmmo < (MagAmmo - BulletMagReload))
		{
			Debug.Log("fesgsegsegseg");
			GameMagAmmo += GameSpareAmmo;
			GameSpareAmmo = 0;
		}
	}

	public void WeaponSwitch()
	{
		ReloadingBool = true;
		StartCoroutine("WeaponSwitchE");
		ChooseEquipment.instance.Curweapon.Gun.animation.Play("Draw");
		audio.PlayOneShot(DrawSound);
	}

	IEnumerator WeaponSwitchE()
	{
		yield return new WaitForSeconds(1);
		ReloadingBool = false;
	}
}

public enum FireMode
{
	Single, 
	Auto,
	Sniper
}
