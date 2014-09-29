using UnityEngine;
using System.Collections;

public sealed class IgnoreTimeScale : MonoBehaviour {
	private static float total_time =  0.0f;
	private static float delta_time = 0.0f;
	private static float now_time = 0.0f;
	private static float last_time = 0.0f;

	public static float TotalTime {
		get {
			return total_time;
		}
	}

	public static float DeltaTime {
		get {
			return delta_time;
		}
	}

	void Awake() {
		IgnoreTimeScale[] pInstances = Object.FindObjectsOfType<IgnoreTimeScale>() as IgnoreTimeScale[];

		if(pInstances != null) {
			if(pInstances.Length > 1) {
				Destroy(this);
			}
		}
	}

	void Update() {
		now_time = Time.realtimeSinceStartup;
		if(last_time != 0.0f)
			delta_time = now_time - last_time;
		last_time = now_time;
	}
}
