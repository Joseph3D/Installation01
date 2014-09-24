using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Assets.Scripts.Game_Logic.AI
{
    public sealed class AIEntity : MonoBehaviour
    {
		private CharacterController Controller;

        public void Start()
        {
			Controller = GetComponent<CharacterController>() as CharacterController;
        }
		
        public void Update()
        {
			Test_MoveForeward();
		}

		private void Test_MoveForeward()
		{
			Vector3 MoveVelocity = new Vector3(1,0,0);

			Controller.SimpleMove(MoveVelocity);
		}
    }
}