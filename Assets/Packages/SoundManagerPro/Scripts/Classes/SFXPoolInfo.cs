using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SFXPoolInfo {
	public int currentIndexInPool = 0;
	public int prepoolAmount = 0;
	public List<float> timesOfDeath = new List<float>();
	public List<GameObject> ownedAudioClipPool = new List<GameObject>();
	
	public SFXPoolInfo(int index, int minAmount, List<float> times, List<GameObject> pool)
	{
		currentIndexInPool = index;
		prepoolAmount = minAmount;
		timesOfDeath = times;
		ownedAudioClipPool = pool;
	}
}
