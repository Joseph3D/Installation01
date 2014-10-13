using UnityEngine;
using System.IO;
using System.Threading;
using System.Collections;
using Assets.Scripts.Data;

public class GameManager : MonoBehaviour
{
    #region Members
    #endregion

    void Start()
    {

    }

    void Update()
    {

    }

    private void InitializeInternals()
    {
        GlobalObjectPool.instance.AddObject("GameManager", this);
    }
}