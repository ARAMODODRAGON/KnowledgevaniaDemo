using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeacherPerson : Interactable {

	// reference to canvas
	[SerializeField] private GameObject m_canvasObject;
	[SerializeField] private Text m_canvasText;

	// text & knowledge to teach
	[SerializeField] [Multiline] private string m_preTeachDialogueText;
	[SerializeField] private string m_knowledgeToTeach;
	[SerializeField] [Multiline] private string m_postTeachDialogueText;

	// state
	private bool m_hasTaught = false;
	private bool m_finishedDialogue = false;

	private void Awake() {
		m_canvasText.text = m_preTeachDialogueText;
	}

	// inherited functions
	public override void OnEnterRange(PlayerController player) {
		if (!m_finishedDialogue) m_canvasObject.SetActive(true);
	}

	public override void OnExitRange(PlayerController player) {
		if (!m_finishedDialogue) m_canvasObject.SetActive(false);
	}

	public override void OnInteract(PlayerController player) {
		if (!m_hasTaught) {
			m_hasTaught = true;
			KnowledgeInventory.Learn(m_knowledgeToTeach);
			if (m_postTeachDialogueText.Length != 0) m_canvasText.text = m_postTeachDialogueText;
			else {
				m_canvasObject.SetActive(false);
				m_finishedDialogue = true;
			}
		}
	}
}
