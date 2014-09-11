using UnityEngine;
using System.Collections;

public class RankManager : MonoBehaviour {

	
	void Start () 
	{
	
	}
	
	
	void Update () 
	{
		if(GameManager.Instance.MyPlayer.EXP > 25)
		{
			GameManager.Instance.MyPlayer.EXP = 0;
			GameManager.Instance.MyPlayer.Rank += 1; 
		}
	}
}
