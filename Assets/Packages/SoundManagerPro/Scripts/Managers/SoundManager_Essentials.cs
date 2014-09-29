using UnityEngine;
using System.Collections;
using antilunchbox;

public partial class SoundManager : Singleton<SoundManager> {
	
	// Singleton required initialization
	public static SoundManager Instance {
		get {
			return ((SoundManager)mInstance);
		} set {
			mInstance = value;
		}
	}

	// Enum representing what method to play songs
	public enum PlayMethod {
		ContinuousPlayThrough,
		ContinuousPlayThroughWithDelay,
		ContinuousPlayThroughWithRandomDelayInRange,
		OncePlayThrough,
		OncePlayThroughWithDelay,
		OncePlayThroughWithRandomDelayInRange,
		ShufflePlayThrough,
		ShufflePlayThroughWithDelay,
		ShufflePlayThroughWithRandomDelayInRange,
	}
}
