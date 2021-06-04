using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Schedule {

	// callback events for the gametimer
	public delegate void TimerEvent();

	// when the timer runs out this event will be called
	public static TimerEvent onTimerEnd = null;

	// when the timer gets reset
	public static TimerEvent onTimerReset = null;

	public static bool PauseTimer { get; set; } = false;

	// a built in timer used for the level
	// counts down
	public static float Timer { get; private set; } = 0f;

	// returns the current seconds remaining
	public static int Seconds => System.TimeSpan.FromSeconds(Timer).Seconds;

	// returns the current minutes remaining
	public static int Minutes => System.TimeSpan.FromSeconds(Timer).Minutes;

	// returns the total minutes remaining
	public static int TotalMinutes => System.TimeSpan.FromSeconds(Timer).Minutes + System.TimeSpan.FromSeconds(Timer).Hours * 60;

	// return the current hours remaining
	public static int Hours => System.TimeSpan.FromSeconds(Timer).Hours;

	// returns if the timer is still counting down
	// does not check if PauseTimer is true
	public static bool IsTimerActive { get; private set; } = false;

	// reset the GameTimer
	public static void ResetTimer(float time) {
		if (time >= 0f) {
			if (time != 0f) IsTimerActive = true;
			else IsTimerActive = false;
			Timer = time;
			s_lastMinutes = TotalMinutes;
			onTimerReset?.Invoke();
		}
	}

	// update the GameTimer with GameManager.DeltaTime
	public static void UpdateTimer() {
		if (PauseTimer) return;
		Timer -= GameManager.DeltaTime;
		CheckTimer();
	}

	// update the GameTimer with GameManager.FixedDeltaTime
	public static void FixedUpdateTimer() {
		if (PauseTimer) return;
		Timer -= GameManager.FixedDeltaTime;
		CheckTimer();
	}

	public static void AddToSchedule(TimerEvent te, int minute) {
		try {
			s_schedule[minute] += te;
		} catch (KeyNotFoundException) {
			s_schedule.Add(minute, te);
		}
	}

	public static void RemoveFromSchedule(TimerEvent te, int minute) {
		try {
			s_schedule[minute] -= te;
			if (s_schedule[minute] == null) s_schedule.Remove(minute);
		} catch (KeyNotFoundException) { }
	}

	private static SortedList<int, TimerEvent> s_schedule = new SortedList<int, TimerEvent>();
	private static int s_lastMinutes = TotalMinutes;

	private static void CheckTimer() {
		// end the timer
		if (Timer < 0f) {
			Timer = 0f;
			if (IsTimerActive) {
				IsTimerActive = false;
				onTimerEnd?.Invoke();
			}
		}

		// check whats on schedule and invoke that
		if (IsTimerActive) {
			int totalmin = TotalMinutes;
			if (totalmin != s_lastMinutes) {
				for (int i = s_lastMinutes; i > totalmin; i--) {
					try {
						s_schedule[i]?.Invoke();
					} catch (KeyNotFoundException) { }
				}
				s_lastMinutes = totalmin;
			}
		}
	}

}
