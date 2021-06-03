using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct DialogueLeaf {
	[Multiline]
	public string text;
	public ActivateOnEvent optionalEvent;
}

//[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue")]
public class DialogueTree : MonoBehaviour {

	// returns the number of dialogue leafs
	public int Count => m_dialogue.Count;

	// indexes the dialogue
	public DialogueLeaf this[int index] {
		get => m_dialogue[index];
		set => m_dialogue[index] = value;
	}

	public bool HideDialogueOnEnd => m_hideDialogueOnEnd;
	public bool EndDialogueOnLeave => m_endDialogueOnLeave;

	enum NextAction : byte {
		None, CheckKnowledge
	}

	[Header("Dialogue")]
	[SerializeField] private List<DialogueLeaf> m_dialogue = new List<DialogueLeaf>();

	[Header("End of dialogue action")]
	[SerializeField] private bool m_hideDialogueOnEnd = false;
	[SerializeField] private bool m_endDialogueOnLeave = false;
	[SerializeField] private NextAction m_nextAction;

	[Header("Check Knowledge")]
	[SerializeField] private Knowledge m_knowledgeCheck;
	[SerializeField] private ActivateOnEvent m_successOptionalEvent;
	[SerializeField] private DialogueTree m_successOptionalDialogue;
	[SerializeField] private ActivateOnEvent m_failedOptionalEvent;
	[SerializeField] private DialogueTree m_failedOptionalDialogue;

	public DialogueTree EndDialogue() {
		switch (m_nextAction) {
			case NextAction.None: break;
			case NextAction.CheckKnowledge:
				Knowledge k = KnowledgeInventory.Get(m_knowledgeCheck.name);

				// failed
				if (k.name != m_knowledgeCheck.name || k.type != m_knowledgeCheck.type) {
					m_failedOptionalEvent?.Activate();
					return m_failedOptionalDialogue;
				}
				// success
				else {
					m_successOptionalEvent?.Activate();
					return m_successOptionalDialogue;
				}
			default: Debug.LogError($"End of dialogue action '{m_nextAction}' not supported"); break;
		}
		return null;
	}

}
