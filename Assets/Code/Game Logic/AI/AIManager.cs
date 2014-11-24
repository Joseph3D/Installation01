using UnityEngine;
using System;
using System.IO;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using GameLogic;
using Helpers;


namespace AI
{
	/// <summary>
	/// This class will be attached to an empty GameObject
	/// This class is responsible for managing a list of every single active AI entity in the world
	/// As well as a central point for any other functions / utilities that AIEntities may need
	/// </summary>
	public sealed class AIManager : MonoBehaviour
    {
		private List<GameObject> AIEntityCollection;

		private List<AIMessage> AIMessageList;

        private GameManager Manager;

        private float AIEntityCollectionUpdateInterval;

		public int EntityCount
		{
			get
			{
                return AIEntityCollection.Count;
			}
		}

		public void Start()
		{
			InitializeInternals();

            StartCoroutine(FillAIEntityCollection());
		}

		public void Update()
		{
		}

		private void UpdateAIEntityCollection()
		{
            AIEntityCollection = Manager.GetAllEntitesTagged(Tag.AIEntity);

            AIEntityCollectionUpdateInterval = 2;
		}

		private void InitializeInternals()
		{
			UpdateAIEntityCollection();

			AIMessageList = new List<AIMessage>();

            Manager = GameManagerLocator.Manager;
		}
		
		public AIEntity GetEntity(int Index)
		{
            if (AIEntityCollection.Count > 0)
            {
                AIEntity Entity = AIEntityCollection[Index].GetComponent<AIEntity>() as AIEntity;

                return Entity;
            }
            return null;
		}

        IEnumerator FillAIEntityCollection()
        {
            UpdateAIEntityCollection();
            yield return new WaitForSeconds(AIEntityCollectionUpdateInterval);
        }
    }
}