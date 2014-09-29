using UnityEngine;
using System.Collections;

[RequireComponent(typeof(IgnoreTimeScale))]
public sealed class GameStopWatch : MonoBehaviour {
	// Public access
	public float totalTime = 0.0f; // Holds the total time.

	// Internal
	private bool _startTimer = false;
	private bool _ignoreTimeScale = true;
	private bool _ignorePauseState;
	private bool pause = false;

	// Const values
	private static readonly int SECONDS_IN_DAY = 86400;
	private static readonly int HOURS_IN_DAY = 24;
	private static readonly int MINUTES_IN_HOUR = 60;
	private static readonly int SECONDS_IN_MINUTE = 60;

	public enum TimeFormat {
		NONE = 0,
		Dynamic,
		DD_HH_MM_SS,
		HH_MM_SS,
		MM_SS,
		M_SS
	}

	/// <summary>
	/// Creates the stop watch.
	/// </summary>
	/// <returns>The stop watch.</returns>
	/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
	/// <param name="ignorePauseState">If set to <c>true</c> ignore pause state.</param>
	/// <param name="name">Name.</param>
	public static GameStopWatch CreateStopWatch(bool ignoreTimeScale = false, bool ignorePauseState = false, string name = "Default") {
		GameObject o = new GameObject(typeof(GameStopWatch).ToString() + " - " + name);
		o.AddComponent<GameStopWatch>();
		o.GetComponent<GameStopWatch>().IgnoreUnityTimeScale = ignoreTimeScale;
		o.GetComponent<GameStopWatch>().IgnorePauseState = ignorePauseState;

		o.gameObject.SetActive(false);

		return o.GetComponent<GameStopWatch>();
	}

	public bool IgnoreUnityTimeScale {
		get {
			return this._ignoreTimeScale;
		} set {
			this._ignoreTimeScale = value;
		}
	}

	public bool IgnorePauseState {
		get {
			return this._ignorePauseState;
		} set {
			this._ignorePauseState = value;
		}
	}

	void Update() {
		if(this._startTimer == true) {
			if(this.pause == false) {
				if(this._ignoreTimeScale == true) {
					this.totalTime = IgnoreTimeScale.DeltaTime;
				} else {
					this.totalTime += Time.deltaTime;
				}
			}
		}
	}

	/// <summary>
	/// Starts the timer.
	/// </summary>
	public void StartTimer() {
		this._startTimer = true;
		this.gameObject.SetActive(true);
	}

	/// <summary>
	/// Stops the timer.
	/// </summary>
	/// <param name="reset_timer">If set to <c>true</c> resets timer.</param>
	public void StopTimer(bool reset_timer = false) {
		this._startTimer = false;
		if(reset_timer) this.ResetTimer();
		this.gameObject.SetActive(false);
	}

	/// <summary>
	/// Resets the timer.
	/// </summary>
	public void ResetTimer() {
		this.totalTime = 0.0f;
	}

	/// <summary>
	/// Formats the time.
	/// </summary>
	/// <returns>The time.</returns>
	/// <param name="format">Enum param for types of formats.</param>
	public string FormatTime(TimeFormat format) {
		string result = "";
		float days = this.totalTime / (float) SECONDS_IN_DAY;
		float hours = (days - (int) days) * (float) HOURS_IN_DAY;
		float minutes = (hours - (int) hours) * (float) MINUTES_IN_HOUR;
		float seconds = (minutes - (int) minutes) * (float) SECONDS_IN_MINUTE;
		
		if(totalTime < 60.0f) {
			days = 0.0f;
			hours = 0.0f;
			minutes = 0.0f;
			seconds = totalTime;
		}

		if(format == TimeFormat.Dynamic) {
			if(minutes < 60) {
				format = TimeFormat.M_SS;
			} else if(minutes > 60) {
				format = TimeFormat.HH_MM_SS;

				if(hours > 24) {
					format = TimeFormat.DD_HH_MM_SS;
				}
			}
		}

		if(format == TimeFormat.DD_HH_MM_SS) {

			if((int)days < 10) {
				result += "0" + ((int) days).ToString() + ":";
			} else {
				result += ((int) days).ToString() + ":";
			}

			if((int)hours < 10) {
				result += "0" + ((int)hours).ToString() + ":";
			} else {
				result += ((int)hours).ToString() + ":";
			}

			if((int) minutes < 10) {
				result += "0" + ((int)minutes).ToString() + ":";
			} else {
				result += ((int)minutes).ToString() + ":";
			}

			if((int) seconds < 10) {
				result += "0" + ((int) seconds).ToString();
			} else {
				result += ((int) seconds).ToString();
			}
		} else if(format == TimeFormat.HH_MM_SS) {
			if((int)hours < 10) {
				result += "0" + ((int)hours).ToString() + ":";
			} else {
				result += ((int)hours).ToString() + ":";
			}
			
			if((int) minutes < 10) {
				result += "0" + ((int)minutes).ToString() + ":";
			} else {
				result += ((int)minutes).ToString() + ":";
			}
			
			if((int) seconds < 10) {
				result += "0" + ((int) seconds).ToString();
			} else {
				result += ((int) seconds).ToString();
			}
		} else if(format == TimeFormat.MM_SS) {
			if((int) minutes < 10) {
				result += "0" + ((int)minutes).ToString() + ":";
			} else {
				result += ((int)minutes).ToString() + ":";
			}
			
			if((int) seconds < 10) {
				result += "0" + ((int) seconds).ToString();
			} else {
				result += ((int) seconds).ToString();
			}
		} else if(format == TimeFormat.M_SS) {
			result += ((int)minutes).ToString() + ":";
			
			if((int) seconds < 10) {
				result += "0" + ((int) seconds).ToString();
			} else {
				result += ((int) seconds).ToString();
			}
		} else {
			result = totalTime.ToString();
		}

		return result;
	}

	/// <summary>
	/// Raises the application pause event.
	/// </summary>
	/// <param name="pauseStatus">If set to <c>true</c> checks if ignore pause state is to be paused.</param>
	void OnApplicationPause(bool pauseStatus) {
		if(pauseStatus == true) {
			if(this.IgnorePauseState == false) {
				this.pause = true;
			}
		} else {
			this.pause = false;
		}
	}
}
