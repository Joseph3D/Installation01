using UnityEngine;
using System.Collections;

public class WeaponMovement : MonoBehaviour 
{
	public float amount = 0.02f;
	public float maxAmount = 0.03f;
	public float Smooth = 3;

	[HideInInspector]
	public Vector3 def;
	[HideInInspector]
	public Vector3 Final;
	[HideInInspector]
	public bool isMovementActive;
	[HideInInspector]
	public float factorX;
	[HideInInspector]
	public float factorY;
	
	void Start () 
	{
		def = transform.localPosition;
	}
	
	
	void Update () 
	{
    	if(isMovementActive == false)
   		{
        	factorX = -Input.GetAxis("Mouse X") * amount;
       		factorY = -Input.GetAxis("Mouse Y") * amount;
       
        	if (factorX > maxAmount)
        		factorX = maxAmount;
       
        	if (factorX < -maxAmount)
        		factorX = -maxAmount;
       
        	if (factorY > maxAmount)
        		factorY = maxAmount;
       
        	if (factorY < -maxAmount)
        		factorY = -maxAmount;
 
 
        	Final = new Vector3(def.x+factorX, def.y+factorY, def.z);
        	transform.localPosition = Vector3.Lerp(transform.localPosition, Final, Time.deltaTime * Smooth);
    	} 
	}
}
