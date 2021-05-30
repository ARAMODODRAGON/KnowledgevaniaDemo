using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class FadeOnEvent : ActivateOnEvent {

	enum FadeType : byte {
		FadeIn, FadeOut
	}

	[Header("Event Specifics")]
	[SerializeField] private Image m_imageToFade; 
	[SerializeField] private FadeType m_fadeType;
	[Tooltip("Should use GameManager.UnscaledDeltaTime")]
	[SerializeField] private bool m_fadeUnscaled;

	[Header("Optional End Event")]
	[Tooltip("Called after the fade ends")]
	[SerializeField] private ActivateOnEvent m_onFadeEndEvent = null;

	// state
	private bool m_isFading = false;
	private float m_fadeVal = 0f;

	private void Awake() {
		if (m_fadeType == FadeType.FadeIn)
			m_fadeVal = 1f;
		else
			m_fadeVal = 0f;
	}

	protected override void OnActivate() {
		m_isFading = true;
	}

	private void LateUpdate() {
		if (m_isFading) {
			float delta = (m_fadeUnscaled ? GameManager.UnscaledDeltaTime : GameManager.DeltaTime);

			// fade in (black to color)
			if (m_fadeType == FadeType.FadeIn) {
				m_fadeVal -= delta;
				if (m_fadeVal <= 0f) {
					m_fadeVal = 0f;
					m_onFadeEndEvent?.Activate();
					// deactivate and swap type
					m_isFading = false; 
					m_fadeType = FadeType.FadeOut;
				}
			}

			// fade out (color to black)
			else {
				m_fadeVal += delta;
				if (m_fadeVal >= 1f) {
					m_fadeVal = 1f;
					m_onFadeEndEvent?.Activate();
					// deactivate and swap type
					m_isFading = false;
					m_fadeType = FadeType.FadeIn;
				}
			}

			// update alpha
			Color color = m_imageToFade.color;
			color.a = m_fadeVal;
			m_imageToFade.color = color;
		}
	}
}
