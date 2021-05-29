using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class LearnedTextUpdater : MonoBehaviour {

	// references
	[SerializeField] private Text m_learnedText;

	// timer
	[SerializeField] private float m_textTime;

	// state
	bool isDirty = true;

	// learned list
	struct Learned {
		public string text;
		public float timer;
	}
	private List<Learned> m_learnedList = new List<Learned>();

	private void Awake() {
		KnowledgeInventory.OnLearnedKnowledge += OnLearnedKnowledge;
		KnowledgeInventory.OnForgottenKnowledge += OnForgottenKnowledge;
	}

	private void OnDestroy() {
		KnowledgeInventory.OnLearnedKnowledge -= OnLearnedKnowledge;
		KnowledgeInventory.OnForgottenKnowledge -= OnForgottenKnowledge;
	}

	private void LateUpdate() {
		float delta = GameManager.DeltaTime;

		for (int i = 0; i < m_learnedList.Count; i++) {
			Learned l = m_learnedList[i];
			l.timer -= delta;

			// remove
			if (l.timer <= 0f) {
				m_learnedList.RemoveAt(i);
				i--;
				isDirty = true;
			}
			// continue 
			else {
				m_learnedList[i] = l;
			}
		}

		// update text
		if (isDirty) {
			isDirty = false;

			string text = "";

			foreach (Learned l in m_learnedList) {
				if (text.Length > 0) text += "\n";
				text += l.text;
			}

			m_learnedText.text = text;
		}
	}

	private void OnLearnedKnowledge(ref Knowledge knowledge) {
		Learned learned;
		learned.text = $"<color=green>+ Knowleadge learned: \"{knowledge.name}\"</color>";
		learned.timer = m_textTime;
		m_learnedList.Add(learned);
		isDirty = true;
	}

	private void OnForgottenKnowledge(ref Knowledge knowledge) {
		Learned learned;
		learned.text = $"<color=red>- Knowleadge forgotten: \"{knowledge.name}\"</color>";
		learned.timer = m_textTime;
		m_learnedList.Add(learned);
		isDirty = true;
	}

}
