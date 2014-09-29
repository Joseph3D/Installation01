using UnityEngine;
using System.Collections;

public class SpawnPointManager : MonoBehaviour 
{
	public static SpawnPointManager Instance;

	public GameObject [] SpawnPoints;
	
	void Start () 
	{
		Instance = this;
		SpawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints");
	}
	
	
	void Update () 
	{
	
	}
}
