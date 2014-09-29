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

		private Stack<Vector3> Waypoints;

		private AIManager Manager;
		private Ray VisionRay;
		private Vector3 LookVector;
		private LineRenderer DebugVisionRayRenderer;

		public float Speed;
		public float SprintSpeed;
		private bool Sprinting;

        public void Start()
        {
			InitializeInternals();
        }
		
        public void Update()
        {
			MoveEntity(MovementDirection.Left);
		}

		private void UpdateMovement()
		{
		}

		/// <summary>
		/// Moves the entity.
		/// </summary>
		/// <param name="Direction">Direction to move entity in</param>
		/// <param name="Speed">Speed (meters per second)</param>
		private void MoveEntity(Vector3 Direction)
		{
			Direction.Normalize();

			Controller.SimpleMove(Direction * Speed);
		}

		/// <summary>
		/// Moves the entity.
		/// </summary>
		/// <param name="Direction">Direction to move entity in</param>
		/// <param name="Speed">Speed (meters per second)</param>
		private void MoveEntity(MovementDirection Direction)
		{
			switch(Direction)
			{
				case MovementDirection.Left:
				{
					Controller.SimpleMove(Directions.Left * Speed);
					break;
				}
				case MovementDirection.Right:
				{
					Controller.SimpleMove(Directions.Right * Speed);
					break;
				}
				case MovementDirection.Foreward:
				{
					Controller.SimpleMove(Directions.Foreward * Speed);
					break;
				}
				case MovementDirection.Backward:
				{
					Controller.SimpleMove(Directions.Backward * Speed);
					break;
				}
			}
		}

		/// <summary>
		/// Moves the entity towards a point.
		/// </summary>
		/// <param name="Point">Point to move towards</param>
		private void MoveEntityTowardsPoint(Vector3 Point)
		{
			Vector3 DirectionToPoint = Point - this.transform.position;

			DirectionToPoint.Normalize();

			Controller.SimpleMove(DirectionToPoint*Speed);
		}

		/// <summary>
		/// 1) Grabs CharacterController component for this entity
		/// 2) Gets a reference to the GameObject tagged as "AI Manager"
		/// 3) Gets a reference to the AIManager component of the GameObject tagged as "AI Manager"
		/// 4) Sets up vision Ray and debug RayRenderer
		/// </summary>
		private void InitializeInternals()
		{
			Controller = GetComponent<CharacterController>() as CharacterController;

			GameObject AIManagerGameObject = GameObject.FindGameObjectWithTag("AI Manager");

			Waypoints = new Stack<Vector3>();

			if(!AIManagerGameObject)
			{
				throw new Exception("AI Manager GameObject not found in game world.");
			}
			else
			{
				Manager = AIManagerGameObject.GetComponent(typeof(AIManager)) as AIManager;
				if(!Manager)
				{
					throw new Exception(@"AIManager component not found in 'AI Manager' GameObject");
				}
			}

			Vector3 Position = transform.position;
		}	
    }
}