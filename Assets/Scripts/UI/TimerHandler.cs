using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerHandler : MonoBehaviour {

	// references
	[SerializeField] private Text m_timerText = null;

	// timer variables
	[SerializeField] private float m_initialTime = 0f;

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

		Schedule.ResetTimer(m_initialTime);
	}

	private void OnDestroy() {
		Schedule.onTimerReset -= OnTimerReset;
	}

	private void OnTimerReset() {
		m_lastTime = Schedule.Timer;
	}

	private void Update() {
		Schedule.UpdateTimer();

		// reset the timer
		if (Input.GetKeyDown(KeyCode.G)) Schedule.ResetTimer(m_initialTime);
	}

	private void LateUpdate() {
		// update the text
		if (Mathf.Abs(m_lastTime - Schedule.Timer) > 0.001f) {
			m_lastTime = Schedule.Timer;
			m_timerText.text = $"Time: {Schedule.TotalMinutes}:{Schedule.Seconds}";
		}
	}
}
