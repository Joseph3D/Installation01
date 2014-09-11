using UnityEngine;
using System.Collections;

public class TimedObjectDestroyer : MonoBehaviour 
{
	public float TimeOut;

	
	void Start () 
	{
		Invoke ("DestroyNow", TimeOut);
	}
	
	
	void DestroyNow () 
	{
		DestroyObject(gameObject);
	}
}
