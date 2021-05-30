using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ChangePausedStateOnEvent : ActivateOnEvent {

	enum PauseType : byte {
		Pause,
		Unpause,
		TogglePause
	}

	[Header("Event Specifics")]
	[SerializeField] private PauseType m_pauseType;

	protected override void OnActivate() {
		switch (m_pauseType) {
			case PauseType.Pause:
				GameManager.IsPaused = true;
				break;
			case PauseType.Unpause:
				GameManager.IsPaused = false;
				break;
			case PauseType.TogglePause:
				GameManager.IsPaused = !GameManager.IsPaused;
				break;
			default: Debug.LogError("Unsupported pausetype"); break;
		}
	}
}
