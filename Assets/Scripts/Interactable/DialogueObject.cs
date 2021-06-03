using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueObject : Interactable {

	// public state
	public bool IsComplete { get; private set; } = false;

	// references
	[SerializeField] private GameObject m_canvasObject;
	[SerializeField] private Text m_canvasText;

	// reference to main dialogue tree
	[SerializeField] private DialogueTree m_dialogueTree;

	// private state
	private bool m_hideDialogueBox = false;
	private int m_dialoguePos = -1;

	private void Awake() {
		if (!m_dialogueTree) {
			enabled = false;
			Debug.LogError("No referenced dialogue tree!");
			return;
		}
		NextDialogue();
	}

	private void NextDialogue() {
		if (IsComplete) return;

		// has dialogue
		if (m_dialogueTree.Count != 0) {
			// show next
			m_dialoguePos++;
			while (m_dialoguePos < m_dialogueTree.Count
				&& m_dialogueTree[m_dialoguePos].text == null) { // make sure we are on a valid index
				m_dialoguePos++;
			}
		}
		// no dialogue
		else {
			m_dialoguePos = 0;
		}

		// end of dialogue
		if (m_dialoguePos == m_dialogueTree.Count) {
			m_dialoguePos = -1;
			DialogueTree d = m_dialogueTree.EndDialogue();

			// start next dialogue tree
			if (d != null) {
				m_dialogueTree = d;
				NextDialogue();
			}

			// end
			else {
				IsComplete = true;
				m_hideDialogueBox = m_dialogueTree.HideDialogueOnEnd;
				m_canvasObject.SetActive(false);
				m_dialogueTree[m_dialogueTree.Count - 1].optionalEvent?.Activate();
			}
		}
		// next dialogue
		else {
			m_canvasText.text = m_dialogueTree[m_dialoguePos].text;
			// activate previous event
			if (m_dialoguePos > 0) m_dialogueTree[m_dialoguePos - 1].optionalEvent?.Activate();
		}

	}

	public override void OnEnterRange(PlayerController player) {
		if (!m_hideDialogueBox) m_canvasObject.SetActive(true);
	}

	public override void OnExitRange(PlayerController player) {
		if (!m_hideDialogueBox) {
			m_canvasObject.SetActive(false);
			if (m_dialogueTree.EndDialogueOnLeave) {
				m_hideDialogueBox = true;
				IsComplete = true;
			}
		}
	}

	public override void OnInteract(PlayerController player) {
		if (!IsComplete) NextDialogue();
	}
}
