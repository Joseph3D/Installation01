using UnityEngine;
using System.Collections;

public class KillVolume : MonoBehaviour
{
    void OnTriggerEnter(Collider Player)
    {
         if (Player)
         {
              Destroy(Player.gameObject);
         }
    }
}