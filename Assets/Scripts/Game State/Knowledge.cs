using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Knowledge : IEquatable<Knowledge> {
	public string name;
	public object data;

	public Knowledge(string name_, object data_) {
		name = name_;
		data = data_;
	}

	public static Knowledge Null {
		get {
			Knowledge knowledge;
			knowledge.name = null;
			knowledge.data = null;
			return knowledge;
		}
	}

	#region Inherited Functions

	public override int GetHashCode() {
		return name.GetHashCode() % data.GetHashCode();
	}

	public override string ToString() {
		return $"{name} (data: {data})";
	}

	public static bool operator ==(Knowledge lhs, Knowledge rhs) {
		return lhs.Equals(rhs);
	}

	public static bool operator !=(Knowledge lhs, Knowledge rhs) {
		return !lhs.Equals(rhs);
	}

	public override bool Equals(object obj) {
		if (obj is Knowledge other)
			return Equals(other);
		else
			return false;
	}

	public bool Equals(Knowledge other) {
		return name.Equals(other.name) && data.Equals(other.data);
	}

	#endregion

}

static class KnowledgeInventory {

	public delegate void KnowledgeChangeEvent(ref Knowledge knowledge);

	// called when something is learned
	// invoked before the knowledge is added to the inventory
	public static KnowledgeChangeEvent OnLearnedKnowledge = null;

	// called when something is forgotten
	// invoked after the knowledge is removed from the inventory
	public static KnowledgeChangeEvent OnForgottenKnowledge = null;

	// returns a ValueCollection to enumerate through the values
	public static Dictionary<string, Knowledge>.ValueCollection Values => s_knowledge.Values;

	// returns a KeyCollection to enumerate through the keys
	public static Dictionary<string, Knowledge>.KeyCollection Keys => s_knowledge.Keys;

	// adds knowledge to the inventory
	// returns false on failure
	public static void Learn(string name, object obj = null) {
		Knowledge knowledge = new Knowledge(name, obj);
		// check if null
		if (knowledge == Knowledge.Null) {
			Debug.LogError("Knowledge was null");
		}
		// add it
		if (!s_knowledge.ContainsKey(knowledge.name)) {
			OnLearnedKnowledge?.Invoke(ref knowledge);
			s_knowledge.Add(knowledge.name, knowledge);
		}
		// error
		else {
			Debug.LogError($"KnowledgeInventory already had knowledge of name {knowledge.name}");
		}
	}

	// removes knowledge
	// returns true on success
	public static bool Forget(string name) {
		if (s_knowledge.ContainsKey(name)) {
			Knowledge knowledge = s_knowledge[name];
			s_knowledge.Remove(name);
			OnForgottenKnowledge?.Invoke(ref knowledge);
			return true;
		}
		return false;
	}

	// checks if the knowledge exists
	public static bool Contains(string name) {
		return s_knowledge.ContainsKey(name);
	}

	// finds and returns a piece of knowledge by name
	public static Knowledge Get(string name) {
		try {
			return s_knowledge[name];
		}
		// failed
		catch (KeyNotFoundException) {
			return Knowledge.Null;
		}
	}

	// invoke this when its time to reset the contained knowledge
	public static void Reset() {
		s_knowledge.Clear();
	}

	private static Dictionary<string, Knowledge> s_knowledge = new Dictionary<string, Knowledge>();
}