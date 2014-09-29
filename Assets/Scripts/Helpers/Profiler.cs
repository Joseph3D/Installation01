using UnityEngine;
using System.Collections;
using System.Diagnostics;
using System;

public class Profiler
{
	private static Stopwatch Timer;

	static Profiler()
	{
		Timer = new Stopwatch();
	}

	public static void BeginPerformanceTimer()
	{
		if(!Timer.IsRunning)
		{
			Timer.Start();
		}
	}

	public static void EndPerformanceTimer()
	{
		if(Timer.IsRunning)
		{
			Timer.Stop();
		}
	}
	
	public static long GetElapsedMilliseconds(bool ResetTimer)
	{
		long Milliseconds = Timer.ElapsedMilliseconds;

		if(ResetTimer)
			Timer.Reset();

		return Milliseconds;
	}

	public static long GetElapsedTicks(bool ResetTimer)
	{
		long Ticks = Timer.ElapsedTicks;

		if(ResetTimer)
			Timer.Reset();

		return Ticks;
	}

	public static TimeSpan GetElapsedTimeSpan(bool ResetTimer)
	{
		TimeSpan Span = Timer.Elapsed;

		if(ResetTimer)
			Timer.Reset();

		return Span;
	}
}