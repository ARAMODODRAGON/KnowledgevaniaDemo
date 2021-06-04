using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum KnowledgeType : byte {
	Other, Ability, Event, Skill, Hidden, None = byte.MaxValue
}

[Serializable]
struct Knowledge : IEquatable<Knowledge> {
	public string name;
	public KnowledgeType type;
	public object data;

	public Knowledge(string name_, KnowledgeType type_ = KnowledgeType.None, object data_ = null) {
		name = name_;
		type = type_;
		data = data_;
	}

	public static Knowledge Null {
		get {
			Knowledge knowledge;
			knowledge.name = null;
			knowledge.type = KnowledgeType.None;
			knowledge.data = null;
			return knowledge;
		}
	}

	public bool IsNull => Equals(Null);
	public bool IsOther => type == KnowledgeType.Other;
	public bool IsAbility => type == KnowledgeType.Ability;
	public bool IsEvent => type == KnowledgeType.Event;
	public bool IsSkill => type == KnowledgeType.Skill;
	public bool IsHidden => type == KnowledgeType.Hidden;
	public bool IsNone => type == KnowledgeType.None;

	// returns the time if this is a event knowledge type
	// returns -1 if this is not an event
	// throws NullReferenceException if there was no attached time info found
	public int EventTime() {
		if (type == KnowledgeType.Event) {
			if (data is int a) return a;
			throw new NullReferenceException();
		}
		return -1;
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
	public static bool Learn(string name, KnowledgeType type = KnowledgeType.Other, object data = null) {
		return Learn(new Knowledge(name, type, data));
	}

	// adds knowledge to the inventory
	// returns false on failure
	public static bool Learn(Knowledge knowledge) {
		string lowername = knowledge.name.ToLower();
		// check if null
		if (knowledge == Knowledge.Null)
			Debug.LogError("Knowledge was null");
		// add it
		if (!s_knowledge.ContainsKey(lowername)) {
			OnLearnedKnowledge?.Invoke(ref knowledge);
			s_knowledge.Add(lowername, knowledge);
			return true;
		}
		// error
		else Debug.LogError($"KnowledgeInventory already had knowledge of name {lowername}");

		// failure
		return false;
	}

	// removes knowledge
	// returns true on success
	public static bool Forget(string name) {
		string lowername = name.ToLower();
		if (s_knowledge.ContainsKey(lowername)) {
			Knowledge knowledge = s_knowledge[lowername];
			s_knowledge.Remove(lowername);
			OnForgottenKnowledge?.Invoke(ref knowledge);
			return true;
		}
		return false;
	}

	// checks if the knowledge exists
	public static bool Contains(string name) {
		return s_knowledge.ContainsKey(name.ToLower());
	}

	// finds and returns a piece of knowledge by name
	public static Knowledge Get(string name) {
		try {
			return s_knowledge[name.ToLower()];
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