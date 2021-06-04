using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRotation : MonoBehaviour {

	[SerializeField] private SpriteRenderer m_spr;

	[SerializeField] private float m_secondsPerCycle;

	private float m_timer = 0f;

	private void LateUpdate() {
		if (m_spr) {
			// timer update
			m_timer += Time.deltaTime;
			while (m_timer > m_secondsPerCycle) m_timer -= m_secondsPerCycle;

			// color update

			m_spr.color = Color.HSVToRGB(m_timer / m_secondsPerCycle, 1f, 1f);

		}
	}

}
