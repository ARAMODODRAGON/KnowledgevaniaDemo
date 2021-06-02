using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LearnOnEvent : ActivateOnEvent {

	[Header("Event Specifics")]
	[SerializeField] private Knowledge m_knowledgeToLearn;
	[SerializeField] private int m_timeOfEvent = -1;

	protected override void OnActivate() {
		Knowledge k = m_knowledgeToLearn;
		if (k.IsEvent) {
			if (m_timeOfEvent == -1) k.data = Schedule.TotalMinutes + 1;
			else k.data = m_timeOfEvent;
		}
		KnowledgeInventory.Learn(k);
	}
}
