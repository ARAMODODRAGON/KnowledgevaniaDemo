using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

class LoadSceneOnEvent : ActivateOnEvent {

	[Header("Event Specifics")]
	[SerializeField] private int m_sceneToload = -1;


	protected override void OnActivate() {
		// check if valid scene
		if (m_sceneToload >= 0 && m_sceneToload < SceneManager.sceneCountInBuildSettings) {
			SceneManager.LoadScene(m_sceneToload);
		}
		// invalid scene
		else {
			Debug.LogError($"Invalid scene index \"{m_sceneToload}\"");
		}
	}
}
