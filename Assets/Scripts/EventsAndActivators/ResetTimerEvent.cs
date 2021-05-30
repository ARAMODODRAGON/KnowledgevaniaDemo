using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ResetTimerEvent : ActivateOnEvent {

	[Header("Event Specifics")]
	[SerializeField] private int m_timeToSet;

	protected override void OnActivate() {
		Schedule.ResetTimer(m_timeToSet);
	}
}
