using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventsTracker : MonoBehaviour {

	[Header("References")]
	[SerializeField] private UnityEngine.UI.Text m_text;


	struct EventCounter {
		public int time;
		public string name;
	}

	private List<EventCounter> m_eventcounters = new List<EventCounter>();
	//private IList<int> m_eventkeys = null;
	//private bool isDirty = true;

	private void Awake() {
		KnowledgeInventory.OnLearnedKnowledge += OnLearned;
	}

	private void Start() {
		foreach (Knowledge kn in KnowledgeInventory.Values) {
			Knowledge k = kn; // workaround to pass as ref
			OnLearned(ref k);
		}
	}

	private void OnDestroy() {
		KnowledgeInventory.OnLearnedKnowledge -= OnLearned;
	}

	private void Update() {
		if (GameManager.TimeScale <= 0.001f) return;

		string s = "";
		//foreach (KeyValuePair<int, EventCounter> item in m_eventcounters) {
		//	EventCounter ec = item.Value;
		//	ec.timer += GameManager.DeltaTime;
		//	s += $"{Mathf.CeilToInt(ec.timer)}s until '{ec.name}'\n";
		//	m_eventcounters[item.Key] = ec;
		//}
		for (int i = 0; i < m_eventcounters.Count; ++i) {
			EventCounter ec = m_eventcounters[i];
			float timeremaining = Schedule.Timer - (ec.time * 60);
			if (timeremaining > 0f) s += $"{Mathf.CeilToInt(timeremaining)}s until '{ec.name}'\n";
		}

		m_text.text = s;
	}

	private void OnLearned(ref Knowledge kn) {
		if (!kn.IsEvent) return;
		
		// check if already exists
		string name = kn.name;
		int index = m_eventcounters.FindIndex((EventCounter ec) => ec.name == name);
		if (index != -1) {
			return;
		}
		

		try {
			EventCounter counter;
			int time = kn.EventTime();
			counter.time = time;
			counter.name = kn.name;
			m_eventcounters.Add(counter);
		} catch (System.NullReferenceException) { }
	}

}
