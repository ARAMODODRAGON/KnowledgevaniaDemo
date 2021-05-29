using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameManager {

	public static PlayerController Player { get; set; } = null;

	public static SideScrollerCamera SSCamera { get; set; } = null;

	// the pause state of the game
	public static bool IsPaused { get; set; } = false;

	// returns 0 or 1 based off IsPaused
	public static float PausedScale => (IsPaused ? 0f : 1f);

	// the time scale that affects the internal timer and the deltatime
	public static float TimeScale { get; set; } = 1.0f;

	// built in deltatime thats scaled by the GameTimeScale
	public static float DeltaTime => Time.deltaTime * TimeScale * PausedScale;

	// built in fixeddeltatime thats scaled by the GameTimeScale
	public static float FixedDeltaTime => Time.fixedDeltaTime * TimeScale * PausedScale;

}
