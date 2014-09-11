using UnityEngine;
using System.Collections;
using System;

public class NetworkAnimStates : MonoBehaviour 
{

	public Animations CurrentAnim = Animations.IdleNew;
	public GameObject ThirdPersonPlayer;
		
	public CharacterController CharCont;
    public CharacterMotor CharMotor;
	public float VelocityMag;
	
	void Start() 
	{
	    VelocityMag = CharCont.velocity.magnitude;
		//ThirdPersonPlayer = GetComponent<UserPlayer>().ThirdPerson.gameObject;
	}
	
	
	
	void Update() 
	{
		if(ThirdPersonPlayer != null)
	    	ThirdPersonPlayer.animation.CrossFade(Enum.GetName(typeof(Animations), CurrentAnim));
	}
	
	public void SyncAnimation(string AnimName, float Speed)
	{
		CurrentAnim = (Animations)Enum.Parse(typeof(Animations),AnimName);
		ThirdPersonPlayer.animation[CurrentAnim.ToString()].speed = VelocityMag;
	}
	
}

public enum Animations
{
	//You need to replace these with the actual names of the animations
	WalkNew,
	WalkBack,
	WalkLeft,
	WalkRight,
	IdleNew,
	thirdpersonmelee
}