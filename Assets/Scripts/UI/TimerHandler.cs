using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler : ActivateOnEvent {

	// references
	[SerializeField] private Text m_timerText = null;

	// check last time to update timer
	private float m_lastTime = 0f;

	private void Awake() {
		if (!m_timerText) {
			m_timerText = GetComponent<Text>();
			if (!m_timerText) {
				gameObject.SetActive(false);
				return;
			}
		}
		Schedule.onTimerReset += OnTimerReset;

		// decide if the timer should show up
		if (!KnowledgeInventory.Contains("Timer")) {
			m_timerText.enabled = false;
		}
	}

	private void OnDestroy() {
		Schedule.onTimerReset -= OnTimerReset;
	}

	private void OnTimerReset() {
		m_lastTime = Schedule.Timer;
	}

	private void Update() {
		Schedule.UpdateTimer();

		// debug stuff
		if (Input.GetKeyDown(KeyCode.G)) Schedule.ResetTimer(180);
		if (Input.GetKey(KeyCode.H)) GameManager.TimeScale = 10f;
		else GameManager.TimeScale = 1f;
	}

	private void LateUpdate() {
		// update the text
		if (m_timerText.enabled && Mathf.Abs(m_lastTime - Schedule.Timer) > 0.001f) {
			m_lastTime = Schedule.Timer;
			m_timerText.text = $"Time: {Schedule.TotalMinutes}:{Schedule.Seconds}";
		}
	}

	protected override void OnActivate() {
		// decide if the timer should show up
		if (!KnowledgeInventory.Contains("Timer")) {
			m_timerText.enabled = false;
		}
	}
}
