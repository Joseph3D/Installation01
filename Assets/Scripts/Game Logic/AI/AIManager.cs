using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.AI
{
	/// <summary>
	/// This class will be attached to an empty GameObject
	/// This class is responsible for managing a list of every single active AI entity in the world
	/// As well as a central point for any other functions / utilities that AIEntities may need
	/// </summary>
	public sealed class AIManager :  MonoBehaviour
    {
		private GameObject[] AIEntityCollection;

		private List<AIMessage> AIMessageList;

		public int AIEntityCount
		{
			get
			{
				return AIEntityCollection.Length;
			}
		}

		public void Start()
		{
			InitializeInternals();
		}

		public void Update()
		{
		}

		private void UpdateAIEntityCollection()
		{
			AIEntityCollection = GameObject.FindGameObjectsWithTag("AI");
		}

		private void InitializeInternals()
		{
			UpdateAIEntityCollection();

			AIMessageList = new List<AIMessage>();
		}
		
		public AIEntity GetEntity(int Index)
		{
			return AIEntityCollection.Count > 0 ? AIEntityCollection[Index] : null;
		}
    }
}