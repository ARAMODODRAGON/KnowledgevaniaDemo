using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateOnEvent : MonoBehaviour {

	enum EventType : byte {
		None,
		OnAwake,
		OnTime,
		OnTriggerEnter,
		OnTriggerExit
	}

	[Header("Optional Chain Event")]
	[Tooltip("Called immediately after this event")]
	[SerializeField] private ActivateOnEvent m_chainEvent;

	[Header("Event Activation")]
	[SerializeField] private bool m_activateOnce = false;
	[SerializeField] private EventType m_eventType = EventType.None;
	[SerializeField] private int m_timeToActivate;

	// call this to manually activate the event
	public void Activate() {
		if (m_activateOnce && m_wasActivated) return;
		else m_wasActivated = true;
		OnActivate();
		m_chainEvent?.Activate();
	}

	// virtual function called when activated
	protected virtual void OnActivate() { }

	// save the scheduled time here so we can use it later 
	private int m_scheduleTime;
	private bool m_wasActivated;

	private void Awake() {
		switch (m_eventType) {
			case EventType.None: break;
			case EventType.OnAwake:
				Activate();
				break;
			case EventType.OnTime:
				m_scheduleTime = m_timeToActivate;
				Schedule.AddToSchedule(Activate, m_scheduleTime);
				break;
			case EventType.OnTriggerEnter: break;
			case EventType.OnTriggerExit: break;
			default:
				Debug.LogError($"Unsupported event \"{m_eventType}\"");
				break;
		}
	}

	private void OnDestroy() {
		if (m_eventType == EventType.OnTime) {
			Schedule.RemoveFromSchedule(Activate, m_scheduleTime);
		}
	}

	private void OnTriggerEnter2D(Collider2D collider) {
		if (m_eventType == EventType.OnTriggerEnter) {
			Activate();
		}
	}

	private void OnTriggerExit2D(Collider2D collidercollider) {
		if (m_eventType == EventType.OnTriggerExit) {
			Activate();
		}
	}

}
