using UnityEngine;
using System.Collections;

public class BulletManager : MonoBehaviour 
{
	public float DestroyTimer = 3.0f;
	
	void Start () 
	{
		Destroy(gameObject, DestroyTimer);
	}
	
	void OnCollisionEnter(Collision col)
	{
		//Debug.Log("aegtsg");
		Destroy(gameObject);
	}
	
	void Update ()
	{
	
	}
}
