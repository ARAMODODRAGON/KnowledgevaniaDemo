using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DisableTimerOnEvent : ActivateOnEvent {

	protected override void OnActivate() {
		Schedule.PauseTimer = true;
	}

}
