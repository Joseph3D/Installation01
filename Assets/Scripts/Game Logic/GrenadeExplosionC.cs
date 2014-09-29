using UnityEngine;
using System.Collections;

public class GrenadeExplosionC : MonoBehaviour 
{
	public float WaitTime = 2.0f;
	public float ExplosionRadius = 5.0f;
	public float ExplosionPower = 10.0f;
	public float ExplosionDamage= 100.0f;

	public Transform Explosion;

	public bool isFragGrenade;

	public GameObject GrenadeSound;
	
	void Start () 
	{
		StartCoroutine(HasThrown());
	}
	
	IEnumerator HasThrown () 
	{
		yield return new WaitForSeconds(WaitTime);
		ThrowGrenade.Instance.IsThrowing = false;
		Network.Instantiate(Explosion, transform.position, Quaternion.identity, 0);
		Network.Instantiate(GrenadeSound, transform.position, Quaternion.identity, 0);
		GrenadeDamage();
	}

	public void GrenadeDamage()
	{
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5.0f);
		foreach (Collider Co in hitColliders)
		{
			if(Co.gameObject.tag == "Player")
			{
				PlayerManager hitter = Co.gameObject.transform.root.GetComponent<PlayerManager>();

				if(hitter != null)
				{
					hitter.networkView.RPC("Server_TakeDamage", RPCMode.All, 200.0f);
					hitter.networkView.RPC("FindHitter1", RPCMode.All, GameManager.Instance.MyPlayer.PlayerName);
				}
			}
		}	
	}
}
