using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

using UnityRandom = UnityEngine.Random;

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
        }
       
		
        public void Update()
        {
			UpdateAIState();
		}

        int GhettoCohesionCheckCounter = 0;
		private void UpdateAIState()
		{
			switch(CurrentState())
			{
				case AIState.Idle:
				{
                    GhettoCohesionCheckCounter++;
                    if (GhettoCohesionCheckCounter >= 120)
                    {
                        CheckSeparation();

                        GhettoCohesionCheckCounter = 0;
                    }

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
		
		private void UpdateCommunications()
		{
			
		}
		
        /// <summary>
        /// Communicates with the AIManager to determine if this unit is too close to to other units.
        /// If it determines that it is too close, it will pick a random vector and determine if there are other units in the way
        /// If there are no other units along this vector ( determined via raycast ) it will set the waypoint for the end of the vector and push AIState.SingleWaypoint
        /// </summary>
		private void CheckSeparation()
		{
			int EntityCount = Manager.EntityCount;
			
			Vector3 EntityGroupAveragePosition = Vector3.zero;
			
			for(int i = 0; i < EntityCount; ++i)
			{
				AIEntity Entity = Manager.GetEntity(i);

                if (Entity.GetHashCode() == this.GetHashCode()) // do not count our own position
                    continue;

				EntityGroupAveragePosition += Entity.transform.position;
			}
			
			EntityGroupAveragePosition /= EntityCount;
			
			RaycastHit HitInformation;
            int RandomAngle;
            Vector3 AngleVector;
			
			if(Vector3.Distance(transform.position,EntityGroupAveragePosition) < 15) // our position is TOO CLOSE to the group's average position
			{
				Vector3 CurrentPosition = transform.position;

                do
                {
                    RandomAngle = UnityEngine.Random.Range(0, 359); // Pick an angle

                    AngleVector = new Vector3((float)Math.Cos(RandomAngle), (float)Math.Sin(RandomAngle));
                    AngleVector.Normalize();

                    Ray DirectionalTestRay = new Ray(CurrentPosition,(AngleVector * 100)); // Make a ray that points at the angle for 100 meters

                    Physics.Raycast(DirectionalTestRay, out HitInformation); // check to see if there are AIEntities along this ray
                } while (HitInformation.collider == null || HitInformation.collider.tag != "AI");

                // At this point AngleVector is a Vector that points in a direction where there are no AIEntities

                int DistanceToSeparateBy = UnityRandom.Range(5, 15);

                AngleVector *= DistanceToSeparateBy; // extend to X meters from normalized direction vector

                AddWaypoint((transform.position + AngleVector)); // set waypoint to the end of the AngleVector
                PushState(AIState.SingleWaypoint); // push new state
			}
		}

        #region Helper Methods
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
        #endregion

        /// <summary>
		/// 1) Grabs CharacterController component for this entity
		/// 2) Gets a reference to the GameObject tagged as "AI Manager"
		/// 3) Gets a reference to the AIManager component of the GameObject tagged as "AI Manager"
		/// </summary>
		private void InitializeInternals()
		{
			Controller = GetComponent<CharacterController>() as CharacterController;
			
			Waypoints = new Stack<Vector3>();
			
			StateStack = new Stack<AIState>();
			
			StateStack.Push(AIState.Idle); // put idle at the bottom
			
			AcquireAIManager();
		}
		
		/// <summary>
		/// Acquires the AI manager.
		/// </summary>
		private void AcquireAIManager()
		{
			GameObject ManagerGameObject = GameObject.FindGameObjectWithTag("AI Manager");
			if(!ManagerGameObject)
			{
				Debug.LogError(@"GameObject tagged as 'AI Manager' Not found in scene");
			}
			else
			{
				Manager = ManagerGameObject.GetComponent<AIManager>() as AIManager;
				if(!Manager)
				{
					Debug.LogError(@"GameObject tagged as 'AI Manager' found. But it is missing the AIManager component");
				}
			}
		}
    }
}