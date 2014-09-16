using UnityEngine;
using System.Collections;

public class NetworkAnimController : MonoBehaviour 
{
	public static NetworkAnimController Instance;
	
	public float v;
	public float h;
	public NetworkAnimStates States;

	public bool isMeleeing;

	
	void Start() 
	{
		Instance = this;
	    isMeleeing = false;
	}
	
	
	void Update() 
	{
		v = Input.GetAxis("Vertical");
		h = Input.GetAxis("Horizontal");
		
		if(Input.GetKeyDown(KeyCode.F) && isMeleeing == false)
		{
			isMeleeing = true;
			StartCoroutine("melee");
			States.SyncAnimation("thirdpersonmelee", 1);
			Debug.Log("MELEESGNI");
		}
		else if(v < 0 && isMeleeing == false)
		{
			States.SyncAnimation("WalkBack", 1);
		}
		else if (v > 0 && isMeleeing == false)
		{
			States.SyncAnimation("WalkNew", -1);
		}
		else if (h < 0 && isMeleeing == false)
		{
			States.SyncAnimation("WalkLeft", 1);
		}
		else if (h > 0 && isMeleeing == false)
		{
			States.SyncAnimation("WalkRight", -1);
		}
		else if(isMeleeing == false)
		{
			States.SyncAnimation("IdleNew", 1);
		}
		
	
	}

	IEnumerator melee()
	{
		yield return new WaitForSeconds(1);
		isMeleeing = false;
	}
	
}
