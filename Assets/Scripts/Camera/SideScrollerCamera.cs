using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideScrollerCamera : MonoBehaviour {

	[SerializeField] private GameObject m_target = null;

	[Header("Follow Movement")]
	[SerializeField] private float m_verticalFollowDistance;
	[SerializeField] private float m_horizontalFollowDistance;

	private void Awake() {
		if (m_target) {
			Vector3 pos = transform.position;
			pos.y = m_target.transform.position.y + m_verticalFollowDistance;
			transform.position = pos;
		}
	}

	private void LateUpdate() {
		UpdateFollow();
	}

	private void UpdateFollow() {
		if (!m_target) return;

		// bounds surrounding player
		Bounds b = new Bounds(m_target.transform.position, new Vector3(m_horizontalFollowDistance, m_verticalFollowDistance, 0f));

		Vector3 newpos = b.ClosestPoint(transform.position);
		newpos.z = transform.position.z;
		transform.position = newpos;

	}
}
