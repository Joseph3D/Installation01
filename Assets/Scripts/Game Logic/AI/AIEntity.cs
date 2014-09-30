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
		
		private Stack<AIState> StateStack;

		private Stack<Vector3> Waypoints;
		public int WaypointTolerance;

		private AIManager    Manager;
		private Ray          VisionRay;
		private Vector3      LookVector;
		private LineRenderer DebugVisionRayRenderer;

		public float Speed;
		public float SprintSpeed;
		private bool Sprinting;
	

        public void Start()
        {
			InitializeInternals();
			
			WaypointTest();
        }
        
        public void WaypointTest()
        {
        	Vector3 Position = transform.position;
        	Position.x += 20;
        	Position.z += 20;
        	
        	AddWaypoint(Position);
        	PushState(AIState.SingleWaypoint);
        }
		
        public void Update()
        {
			UpdateAIState();
			
			DEBUG_Update();
		}
		
		private void DEBUG_Update()
		{
			Debug.DrawRay(this.transform.position,transform.position + (Vector3.forward * 5),Color.green);
		}


		private void UpdateAIState()
		{
			switch(CurrentState())
			{
				case AIState.Idle:
				{
					break;
				}
				case AIState.SingleWaypoint:
				{
					Update_SingleWaypoint();
					break;
				}
				case AIState.WaypointList:
				{
					Update_WaypointList();
					break;
				}
			}
		}
		
		private void Update_SingleWaypoint()
		{
			if(AtWaypoint(CurrentWaypoint(),WaypointTolerance))
			{
				RemoveCurrentState();
			}
			else
			{
				MoveEntityTowardsPoint(CurrentWaypoint());
			}
		}
		
		/// <summary>
		/// Update behavior for WaypointList state
		/// </summary>
		private void Update_WaypointList()
		{
			if(AtWaypoint(CurrentWaypoint(),WaypointTolerance))
			{
				RemoveTopWaypoint();
				return;
			}
			else
			{
				MoveEntityTowardsPoint(CurrentWaypoint());
			}
		}

		private bool AtWaypoint(Vector3 Waypoint, int Tolerance)
		{
			return Physics.CheckSphere(Waypoint,Tolerance,1);
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
		/// Adds the waypoint
		/// </summary>
		/// <param name="NewWaypoint">New waypoint</param>
		private void AddWaypoint(Vector3 NewWaypoint)
		{
			Waypoints.Push(NewWaypoint);
		}

		/// <summary>
		/// Removes the top waypoint.
		/// </summary>
		private void RemoveTopWaypoint()
		{
			Waypoints.Pop();
		}

		/// <summary>
		/// Returns the next waypoint Stack.Peek()
		/// </summary>
		/// <returns>The waypoint.</returns>
		private Vector3 CurrentWaypoint()
		{
			return Waypoints.Peek();
		}

		public bool HasWaypoints()
		{
			return Waypoints.Count > 0 ? true : false;
		}

		public int WaypointCount()
		{
			return Waypoints.Count;
		}
		
		/// <summary>
		/// returns the current AIState via Stack.Peek
		/// </summary>
		/// <returns>The state.</returns>
		public AIState CurrentState()
		{
			if(StateStack.Count > 0)
			{
				return StateStack.Peek();
			}
			return AIState.Idle;
		}
		
		public void RemoveCurrentState()
		{
			StateStack.Pop();
		}
		
		public void PushState(AIState State)
		{
			StateStack.Push(State);
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
			
			Waypoints = new Stack<Vector3>();
			
			StateStack = new Stack<AIState>();
			
			StateStack.Push(AIState.Idle);
		}	
    }
}