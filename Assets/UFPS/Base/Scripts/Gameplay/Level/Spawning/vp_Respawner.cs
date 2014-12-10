/////////////////////////////////////////////////////////////////////////////////
//
//	vp_Respawner.cs
//	© VisionPunk. All Rights Reserved.
//	https://twitter.com/VisionPunk
//	http://www.visionpunk.com
//
//	description:	this script allows a gameobject to respawn in the same position,
//					or at random, tagged vp_SpawnPoints after its 'Die' method has
//					been called
//
/////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[System.Serializable]
public class vp_Respawner : MonoBehaviour
{

	public enum SpawnMode
	{
		SamePosition,
		SpawnPoint
	}

	// describes what to do when the spawnpoint is obstructed
	public enum ObstructionSolver
	{
		Wait,				// wait for 'RespawnTime' seconds and try again (good for items and powerups)
		AdjustPlacement		// try to find the closest valid position. if no position is found, wait. (good for players and AI)
	}

	public SpawnMode m_SpawnMode = SpawnMode.SamePosition;
	public string SpawnPointTag = "";

	public ObstructionSolver m_ObstructionSolver = ObstructionSolver.Wait;
	public float ObstructionRadius = 1.0f;	// area around object which must be clear of other objects before respawn
		
	public float MinRespawnTime = 3.0f;		// random timespan in seconds to delay respawn
	public float MaxRespawnTime = 3.0f;
	public bool SpawnOnAwake = false;

	public AudioClip SpawnSound = null;		// sound to play upon respawn
	public GameObject [] SpawnFXPrefabs = null;	// e.g. a particle effect to be played upon respawn

	#if UNITY_EDITOR
	[vp_HelpBox(typeof(vp_Respawner), UnityEditor.MessageType.None, typeof(vp_Respawner), null, true)]
	public float helpbox;
	#endif

	protected Vector3 m_InitialPosition = Vector3.zero;		// initial position detected and used for respawn
	protected Quaternion m_InitialRotation;		// initial rotation detected and used for respawn
	protected vp_Placement Placement = new vp_Placement();
	protected Transform m_Transform = null;
	protected AudioSource m_Audio = null;

	protected bool m_IsInitialSpawnOnAwake = false;

	protected vp_Timer.Handle m_RespawnTimer = new vp_Timer.Handle();


	/// <summary>
	/// 
	/// </summary>
	protected virtual void Awake()
	{

		m_Transform = transform;
		m_Audio = audio;

		Placement.Position = m_InitialPosition = m_Transform.position;
		Placement.Rotation = m_InitialRotation = m_Transform.rotation;

		if (m_SpawnMode == SpawnMode.SamePosition)
			SpawnPointTag = "";

		if (SpawnOnAwake)
		{
			m_IsInitialSpawnOnAwake = true;
			vp_Utility.Activate(gameObject, false);
			PickSpawnPoint();
		}

	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnEnable()
	{
	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void OnDisable()
	{
	}


	/// <summary>
	/// 
	/// </summary>
	protected virtual void SpawnFX()
	{

		if (!m_IsInitialSpawnOnAwake)
		{

			if (m_Audio != null)
			{
				m_Audio.pitch = Time.timeScale;
				m_Audio.PlayOneShot(SpawnSound);
			}

			// spawn effects gameobjects
			if (SpawnFXPrefabs != null && SpawnFXPrefabs.Length > 0)
			{
				foreach (GameObject fx in SpawnFXPrefabs)
				{
					if (fx != null)
						vp_Utility.Instantiate(fx, m_Transform.position, m_Transform.rotation);
				}
			}
		}

		m_IsInitialSpawnOnAwake = false;

	}
	

	/// <summary>
	/// event target, typically sent by vp_DamageHandler or vp_ItemPickup
	/// </summary>
	protected virtual void Die()
	{
		vp_Timer.In(UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime), PickSpawnPoint, m_RespawnTimer);
	}


	/// <summary>
	/// respawns the object if no other object is occupying the
	/// respawn area. otherwise reschedules respawning. NOTE:
	/// this method can only run in singleplayer and by a master
	/// in multiplayer. multiplayer _clients_ will instead use
	/// the version of 'GetSpawnPoint' that takes a position and
	/// rotation (called directly by the master)
	/// </summary>
	public virtual void PickSpawnPoint()
	{

		// return if the object has been destroyed (for example
		// as a result of loading a new level while it was gone)
		if (this == null)
			return;

		// if mode is 'SamePosition' or the level has no spawnpoints, go to initial position
		if ((m_SpawnMode == SpawnMode.SamePosition) || (vp_SpawnPoint.SpawnPoints.Count < 1))
		{

			Placement.Position = m_InitialPosition;
			Placement.Rotation = m_InitialRotation;
			// if an object the size of 'RespawnCheckRadius' can't fit at
			// 'm_InitialPosition' ...
			if (Placement.IsObstructed(ObstructionRadius))
			{
				switch (m_ObstructionSolver)
				{
					case ObstructionSolver.Wait:
						// ... just try again later!
						vp_Timer.In(UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime), PickSpawnPoint, m_RespawnTimer);
						return;
					case ObstructionSolver.AdjustPlacement:
						// try to adjust the position ...
						if (!vp_Placement.AdjustPosition(Placement, ObstructionRadius))
						{
							// ... and only if we failed to adjust the position, try again later
							vp_Timer.In(UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime), PickSpawnPoint, m_RespawnTimer);
							return;
						}
						break;
				}
			}
		}
		else
		{

			// placement will be calculated by the spawnpoint system.
			// NOTE: the obstruction solution logic becomes slightly
			// different with spawnpoints
			switch (m_ObstructionSolver)
			{
				case ObstructionSolver.Wait:
					// if an object the size of 'RespawnCheckRadius' can't fit at
					// this random spawnpoint ...
					Placement = vp_SpawnPoint.GetRandomPlacement(0.0f, SpawnPointTag);
					if (Placement == null)
					{
						Placement = new vp_Placement();
						m_SpawnMode = SpawnMode.SamePosition;
						PickSpawnPoint();
					}
					// NOTE: no 'snap to ground' in this mode since the snap logic
					// of 'GetRandomPlacement' is dependent on its input value
					if (Placement.IsObstructed(ObstructionRadius))
					{
						// ... skip trying to adjust the position and try again later
						vp_Timer.In(UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime), PickSpawnPoint, m_RespawnTimer);
						return;
					}
					break;
				case ObstructionSolver.AdjustPlacement:
					// if an object the size of 'RespawnCheckRadius' can't fit at
					// this random spawnpoint and we fail to adjust the position ...
					Placement = vp_SpawnPoint.GetRandomPlacement(ObstructionRadius, SpawnPointTag);
					if (Placement == null)
					{
						// ... try again later
						vp_Timer.In(UnityEngine.Random.Range(MinRespawnTime, MaxRespawnTime), PickSpawnPoint, m_RespawnTimer);
						return;
					}
					break;
			}

		}

		Respawn();

	}


	/// <summary>
	/// forces an object's 'Placement' to 'position', 'rotation' and
	/// respawns it at that point
	/// </summary>
	public virtual void PickSpawnPoint(Vector3 position, Quaternion rotation)
	{

		Placement.Position = position;
		Placement.Rotation = rotation;

		Respawn();

	}


	/// <summary>
	/// 
	/// </summary>
	public virtual void Respawn()
	{

		// reactivate and reset
		vp_Utility.Activate(gameObject);
		SpawnFX();

		// in multiplayer, this should only be true if we're the master / host
		if (vp_Gameplay.isMaster)
		{
			vp_GlobalEvent<Transform, vp_Placement>.Send("Respawn", transform.root, Placement);
		}

		SendMessage("Reset");		// will trigger on vp_Respawners + vp_DamageHandlers

		// reset placement to start position for next respawn since it may
		// have been adjusted by obstruction logic during respawn
		Placement.Position = m_InitialPosition;
		Placement.Rotation = m_InitialRotation;
		// NOTE: this should end up affecting mainly items and powerups since
		// players typically use vp_SpawnPoints to fetch a new Placement
		// every time

	}
	
	
	/// <summary>
	/// event target. resets position, angle and motion
	/// </summary>
	public virtual void Reset()
	{

		if (!Application.isPlaying)
			return;

		m_Transform.position = Placement.Position;

		if (rigidbody != null && !rigidbody.isKinematic)
		{
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
		}

	}


}

