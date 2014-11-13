using UnityEngine;
using System.Collections;

public class UIToggleOnHover : MonoBehaviour {

	public GameObject ObjectToToggle;

	void OnHover(bool isOver){

		if(isOver){
		ObjectToToggle.gameObject.SetActive(true);
		}
		else
		{
			ObjectToToggle.gameObject.SetActive(false);
		}

	}
}