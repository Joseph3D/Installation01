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

namespace AI
{
    public sealed class AIEntity : MonoBehaviour
    {
		private CharacterController Controller;
        private NavMeshAgent NavigationAgent;
		
		private Stack<AIState> StateStack;

		private Stack<Vector3> Waypoints;

		private AIManager    Manager;

		public  float Speed;
		public  float SprintSpeed;
		private bool   Sprinting;
        public  AIState InitialState;
        public int CommunicationsChannel;
	
        private GameObject[] PatrolPoints;

        private AITraits Traits;

        public void Start()
        {
			InitializeInternals();
        }
		
        public void Update()
        {
			UpdateState();

            UpdateSpeed();
		}


        /// <summary>
        /// Updates state of unit
        /// </summary>
		private void UpdateState()
		{
			switch(CurrentState())
			{
				case AIState.Idle:
				{
                    // eventually interesting idle behavior will go here
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
                    if (PatrolPoints.Length == 0) break;

                    Update_Patrol();

                    break;
                }
			}
		}

        /// <summary>
        /// Updates current speed of this unit
        /// </summary>
        private void UpdateSpeed()
        {
            if (Sprinting)
                NavigationAgent.speed = SprintSpeed;
            else
                NavigationAgent.speed = Speed;
        }

        /// <summary>
        /// Update method for patrol functionality
        /// </summary>
        private void Update_Patrol()
        {
            if(!InTransit())
            {
                Vector3 NextPatrolPoint = GetRandomPatrolPoint().transform.position;

                SetEntityDestination(NextPatrolPoint);
            }
        }
		
        /// <summary>
        /// Update method for single waypoint navigation
        /// </summary>
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

            //if(InTransit())
            //{
            //    if(AtDestination())
            //    {
            //        RemoveTopWaypoint();
            //        SetEntityDestination(CurrentWaypoint());
            //    }
            //}     

            if(InTransit() && AtDestination()) // New cleaner implementation, old is commented out above. UNTESTED 10-10-14 10:24am pacific time
            {
                RemoveTopWaypoint();
                SetEntityDestination(CurrentWaypoint());
            }
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

        /// <summary>
        /// AtDestination()
        /// </summary>
        /// <returns>True if the unit is at it's navigation destination</returns>
        private bool AtDestination()
        {
            float DistanceRemaining = NavigationAgent.remainingDistance;

            if(DistanceRemaining != Mathf.Infinity && NavigationAgent.pathStatus == NavMeshPathStatus.PathComplete && NavigationAgent.remainingDistance == 0)
            {
                return true;
            }

            return false;
		}

        /// <summary>
        /// Determines if this AI unit is currently in transit
        /// </summary>
        /// <returns>True if the unit is currently in transit</returns>
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
		/// Moves the entity. using the character controller attached to it.
        /// this DOES NOT involve pathfinding / navigation in any way
		/// </summary>
		/// <param name="Direction">Direction to move entity in</param>
		/// <param name="Speed">Speed (meters per second)</param>
		private void MoveEntity(MovementDirection Direction)
		{
			switch(Direction)
			{
				case MovementDirection.Left:
				{
                    if (Sprinting)
                    {
                        Controller.SimpleMove(Directions.Left * SprintSpeed);
                    }
                    else
                    {
                        Controller.SimpleMove(Directions.Left * Speed);
                    }

					break;
				}
				case MovementDirection.Right:
				{
                    if (Sprinting)
                    {
                        Controller.SimpleMove(Directions.Right * SprintSpeed);
                    }
                    else
                    {
                        Controller.SimpleMove(Directions.Right * Speed);
                    }

					break;
				}
				case MovementDirection.Foreward:
				{
                    if (Sprinting)
                    {
                        Controller.SimpleMove(Directions.Foreward * SprintSpeed);
                    }
                    else
                    {
                        Controller.SimpleMove(Directions.Foreward * Speed);
                    }

					break;
				}
				case MovementDirection.Backward:
				{
                    if (Sprinting)
                    {
                        Controller.SimpleMove(Directions.Backward * SprintSpeed);
                    }
                    else
                    {
                        Controller.SimpleMove(Directions.Backward * Speed);
                    }
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

			if(Sprinting)
            {
                Controller.SimpleMove(DirectionToPoint * SprintSpeed);
            }
            else
            {
                Controller.SimpleMove(DirectionToPoint * Speed);
            }
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

        /// <summary>
        /// HasWaypoints()
        /// </summary>
        /// <returns>True if this unit has waypoints</returns>
		public bool HasWaypoints()
		{
			return Waypoints.Count > 0 ? true : false;
		}

        /// <summary>
        /// WaypointCount()
        /// </summary>
        /// <returns>The number of waypoints that this unit has</returns>
		public int WaypointCount()
		{
			return Waypoints.Count;
		}

        /// <summary>
		/// CurrentState()
		/// </summary>
		/// <returns>The top of this Unit's State stack</returns>
		public AIState CurrentState()
		{
			if(StateStack.Count > 0)
			{
				return StateStack.Peek();
			}
			return AIState.Idle;
		}
		
        /// <summary>
        /// Removes the current state
        /// </summary>
		public void RemoveCurrentState()
		{
			StateStack.Pop();
		}
		
        /// <summary>
        /// Pushes new waypoint state
        /// </summary>
        /// <param name="State"></param>
		public void PushState(AIState State)
		{
			StateStack.Push(State);
		}

        /// <summary>
        /// Gathers a list of every GameObject in the world tagged "AI Patrol Point"
        /// </summary>
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

            InitializeNavigation();
			
			Waypoints = new Stack<Vector3>();
			
			StateStack = new Stack<AIState>();

            StateStack.Push(InitialState);

            PatrolPoints = null;
			
			AcquireAIManager();

            UpdatePatrolPoints();
		}

        /// <summary>
        /// Sets up this AI unit to use the Navigation System.
        /// </summary>
        private void InitializeNavigation()
        {
            NavigationAgent = GetComponent<NavMeshAgent>() as NavMeshAgent;

            NavigationAgent.speed = Speed;
        }
		
        /// <summary>
        /// Loads AITraits from XML file
        /// </summary>
        /// <param name="TraitsFilePath"></param>
        private void LoadTraits(string TraitsFilePath)
        {
            if(File.Exists(TraitsFilePath))
            {
                Traits = AITraits.LoadFromFile(TraitsFilePath);
            }
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