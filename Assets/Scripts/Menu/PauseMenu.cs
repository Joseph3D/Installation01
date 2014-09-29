using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour 
{
	public static PauseMenu Instance;
	public bool MenuOpen;

	void Start () 
	{
		MenuOpen = false;
	}
	
	
	void Update () 
	{
		if(Input.GetKey(KeyCode.K) && MenuOpen == false)
			MenuOpen = true;
		if(Input.GetKey(KeyCode.K) && MenuOpen == true)
			MenuOpen = false;
	}

	public void OnGUI()
	{
		if(MenuOpen == true)
		{
			GUI.Box(new Rect(10,10, Screen.width / 4, Screen.height - 20), "Menu");
		}
	}
}
