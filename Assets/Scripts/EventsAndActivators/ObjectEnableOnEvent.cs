using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class ObjectEnableOnEvent : ActivateOnEvent {

	enum EnableType : byte {
		Enable, Disable, Toggle
	}

	[Header("Event Specifics")]
	[SerializeField] private GameObject m_targetObject = null;
	[SerializeField] private EnableType type;

	protected override void OnActivate() {
		switch (type) {
			case EnableType.Enable:
				m_targetObject.SetActive(true);
				break;
			case EnableType.Disable:
				m_targetObject.SetActive(false);
				break;
			case EnableType.Toggle:
				m_targetObject.SetActive(!m_targetObject.activeSelf);
				break;
			default:
				break;
		}
	}

}
