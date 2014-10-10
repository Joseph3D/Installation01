using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.IO;
using UnityEngine;

using UnityRandom = UnityEngine.Random;
using UnityDebug  = UnityEngine.Debug;

namespace Assets.Scripts.Game_Logic.AI
{
    public sealed class AIEntity : MonoBehaviour
    {
		private CharacterController Controller;
        private NavMeshAgent NavigationAgent;
		
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

        private GameObject[] PatrolPoints;

        public AIState InitialState;
	

        public void Start()
        {
			InitializeInternals();

        }
		
        public void Update()
        {
			UpdateAIState();
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
                case AIState.Patrolling:
                {

                    break;
                }
			}
		}

        private void Update_Patrol()
        {
            
        }
		
		private void Update_SingleWaypoint()
		{
            if(NavigationAgent.hasPath == false && NavigationAgent.pathPending == false) // no current path, not in the process of making one
            {
                SetEntityDestination(CurrentWaypoint()); // set path and let pathfinding take it from there
            }
		}
		
		/// <summary>
		/// Update behavior for WaypointList state
		/// </summary>
		private void Update_WaypointList()
		{
            if(!InTransit())
            {
                SetEntityDestination(CurrentWaypoint());
            }
            if(InTransit())
            {
                if(AtDestination())
                {
                    RemoveTopWaypoint();
                    SetEntityDestination(CurrentWaypoint());
                }
            }
		}
		
		private void UpdateCommunications()
		{
			
		}
		
		private void CheckSeparation()
		{
			
		}

        #region Helper Methods
        /// <summary>
        /// Sets this Entitiy's destination in the pathfinding engine
        /// </summary>
        /// <param name="Destination">Destination Point</param>
        public void SetEntityDestination(Vector3 Destination)
        {
            NavigationAgent.SetDestination(Destination);
        }

        
        private bool AtDestination()
        {
            float DistanceRemaining = NavigationAgent.remainingDistance;

            if(DistanceRemaining != Mathf.Infinity && NavigationAgent.pathStatus == NavMeshPathStatus.PathComplete && NavigationAgent.remainingDistance == 0)
            {
                return true;
            }
            return false;
		}

        private bool InTransit()
        {
            if (NavigationAgent.pathStatus == NavMeshPathStatus.PathComplete &&
                NavigationAgent.remainingDistance != Mathf.Infinity &&
                NavigationAgent.remainingDistance > 0)
                return true;

            return false;
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

        private void UpdatePatrolPoints()
        {
            PatrolPoints = GameObject.FindGameObjectsWithTag("AI Patrol Point");
        }

        /// <summary>
        /// Gets a random patrol point GameObject
        /// </summary>
        /// <returns>A random GameObject in the world that is tagged "AI Patrol Point". Returns null if there are no patrol points in the world</returns>
        private GameObject GetRandomPatrolPoint()
        {
            if (PatrolPoints.Length > 0)
                return PatrolPoints[UnityRandom.Range(0, PatrolPoints.Length)];
            return null;
        }
        #endregion

        /// <summary>
		/// 1) Grabs CharacterController component for this entity
		/// 2) Gets a reference to the GameObject tagged as "AI Manager"
		/// 3) Gets a reference to the AIManager component of the GameObject tagged as "AI Manager"
		/// </summary>
		private void InitializeInternals()
		{
			Controller = GetComponent<CharacterController>() as CharacterController;

            NavigationAgent = GetComponent<NavMeshAgent>() as NavMeshAgent;
			
			Waypoints = new Stack<Vector3>();
			
			StateStack = new Stack<AIState>();

            StateStack.Push(InitialState);

            PatrolPoints = null;
			
			AcquireAIManager();

            UpdatePatrolPoints();
		}
		
		/// <summary>
		/// Acquires the AI manager.
		/// </summary>
		private void AcquireAIManager()
		{
			GameObject ManagerGameObject = GameObject.FindGameObjectWithTag("AI Manager");
			if(!ManagerGameObject)
			{
				UnityDebug.LogError(@"GameObject tagged as 'AI Manager' Not found in scene");
			}
			else
			{
				Manager = ManagerGameObject.GetComponent<AIManager>() as AIManager;
				if(!Manager)
				{
					UnityDebug.LogError(@"GameObject tagged as 'AI Manager' found. But it is missing the AIManager component");
				}
			}
		}
    }
}