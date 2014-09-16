using UnityEngine;
using System.Collections;

public class ChooseEquipment : MonoBehaviour {
	
	//public GameObject Primary;
	public Weapon Secondary;
	public Weapon Primary;
	public Weapon Curweapon;
	
	public static ChooseEquipment instance;
	
	public bool SecondaryActive;
	public bool PrimaryActive;

	public bool Switching;
	
	// Use this for initialization
	public void Start () 
	{	
		Switching = false;
		instance = this;
		Curweapon = Primary; 
		
		Secondary.gameObject.SetActive(false);
		Secondary.active = false;
	}

	public void Spawn()
	{
		Debug.Log("spawning fuck ");
		Curweapon = Primary;

		PlayerManager.Instance.Zooming = false;
        PlayerManager.Instance.FOVHolder = PlayerManager.Instance.NormFOV;
        
		Primary.gameObject.SetActive(true);
		Primary.Gun.gameObject.SetActive(true);
		Switching = false;
		Primary.active = true;
		Curweapon.WeaponSwitch();
		//Gun.Instance.UnAim();
			
		Secondary.gameObject.SetActive(false);
		Secondary.Gun.gameObject.SetActive(false);
		Secondary.active = false;

		Curweapon.Gun.gameObject.SetActive(true);
        Curweapon.ReloadingBool = false;

        Curweapon.gameObject.SetActive(true);
        Switching = false;
	}
	
	// Update is called once per frame
	public void Update () 
	{	
		if(Input.GetKeyDown(KeyCode.Tab))
		{
			if(Switching == false)
			{
				Switching = true;
			}
			else if (Switching == true)
			{
				Switching = false;
			}
		}

	    if(Switching == false && Secondary.active == true && PlayerManager.Instance.Zooming == false)
		{
			Curweapon = Primary;
			Primary.gameObject.SetActive(true);
			Primary.Gun.gameObject.SetActive(true);
			Primary.active = true;
			Curweapon.WeaponSwitch();
			//Gun.Instance.UnAim();
			
			Secondary.gameObject.SetActive(false);
			Secondary.active = false;
		}
		
		if(Switching == true && Primary.active == true && PlayerManager.Instance.Zooming == false)
		{
			Curweapon = Secondary;
			Secondary.gameObject.SetActive(true);
			Secondary.Gun.gameObject.SetActive(true);
			Secondary.active = true;
			Weapon.Instance.Gun.animation.Play("Draw");
			Curweapon.WeaponSwitch();

			Primary.active = false;
			Primary.gameObject.SetActive(false);
		}
	}
}