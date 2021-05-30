using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class LearnOnEvent : ActivateOnEvent {

	[Header("Event Specifics")]
	[SerializeField] private string m_knowledgeToLearn;

	protected override void OnActivate() {
		KnowledgeInventory.Learn(m_knowledgeToLearn);
	}
}
