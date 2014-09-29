using UnityEngine;
using System.Collections;

public class ThrowGrenade : MonoBehaviour 
{
	public static ThrowGrenade Instance;
	public Rigidbody GrenadeObjF;
	public Rigidbody GrenadeObjP;

	public Transform Spawn;

	public Rigidbody grenF;
	public Rigidbody grenP;

	public AudioClip FragPin;
	public AudioClip PlasmaPin;

	public int GrenadeCountStartingFrag;
	public int GrenadeCountStartingPlasma;

	public bool Switching;

	public float Timer;

    [HideInInspector]
	public int GrenadeCountActualFrag;
	[HideInInspector]
	public int GrenadeCountActualPlasma;
	[HideInInspector]
	public Vector3 GrenadePos;
	[HideInInspector]
	public bool IsThrowing;
	[HideInInspector]
	public bool isFrag;
	[HideInInspector]
	public bool CanFire;
	[HideInInspector]
	public string Throw;

	void Start () 
	{
		Switching = false;
		Instance = this;
		Timer = 0f;

		if(Timer < 0)
			Timer = 0;

		GrenadeCountActualFrag = GrenadeCountStartingFrag;
		GrenadeCountActualPlasma = GrenadeCountStartingPlasma;

		CanFire = false;
		isFrag = true;
		IsThrowing = false;
	}


	IEnumerator Toss()
	{
		yield return new WaitForSeconds(0.2f);
		CanFire = false;

		if(isFrag)
		{
			grenF = Instantiate(GrenadeObjF,Spawn.position,Spawn.rotation) as Rigidbody;
			grenF.rigidbody.AddRelativeForce(Vector3.forward * 1000);
			GrenadeCountActualFrag--;
		}
		
		if(!isFrag)
		{
			grenP = Instantiate(GrenadeObjP,Spawn.position,Spawn.rotation) as Rigidbody;
			grenP.rigidbody.AddRelativeForce(Vector3.forward * 1000);
			GrenadeCountActualPlasma--;
		}
		//gameObject.SetActive(false);
	}
	
	
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.G) && isFrag)
			isFrag = false;
		else if(Input.GetKeyDown(KeyCode.G) && !isFrag)
			isFrag = true;

		Timer -= Time.deltaTime;

		if(Input.GetMouseButtonDown(1) && CanFire == false && PlayerManager.Instance.Zooming == false && Timer <= 0)
		{
			if(isFrag && GrenadeCountActualFrag > 0)
			{
				audio.PlayOneShot(FragPin);
				Timer = 3;

				IsThrowing = true;
				CanFire = true;
				StartCoroutine(Toss());
				
				ChooseEquipment.instance.Curweapon.Gun.animation.Play("ThrowNew2");
			}
			
			if(!isFrag && GrenadeCountActualPlasma > 0)
			{
				audio.PlayOneShot(PlasmaPin);
				Timer = 3;

				IsThrowing = true;
				CanFire = true;
				StartCoroutine(Toss());
				
				ChooseEquipment.instance.Curweapon.Gun.animation.Play("ThrowNew2");
			}
		}
	}
}
