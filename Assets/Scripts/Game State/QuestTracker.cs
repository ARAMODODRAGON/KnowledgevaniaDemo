


// following is unused


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//struct Quest : IEquatable<Quest> {
//	public string name;
//	public object data;
//
//	public Quest(string name_, object data_) {
//		name = name_;
//		data = data_;
//	}
//
//	public static Quest Null {
//		get {
//			Quest quest;
//			quest.name = null;
//			quest.data = null;
//			return quest;
//		}
//	}
//
//	#region Inherited Functions
//
//	public override int GetHashCode() {
//		return name.GetHashCode() % data.GetHashCode();
//	}
//
//	public override string ToString() {
//		return $"{name} (data: {data})";
//	}
//
//	public static bool operator ==(Quest lhs, Quest rhs) {
//		return lhs.Equals(rhs);
//	}
//
//	public static bool operator !=(Quest lhs, Quest rhs) {
//		return !lhs.Equals(rhs);
//	}
//
//	public override bool Equals(object obj) {
//		if (obj is Quest other)
//			return Equals(other);
//		else
//			return false;
//	}
//
//	public bool Equals(Quest other) {
//		return name.Equals(other.name) && data.Equals(other.data);
//	}
//
//	#endregion
//
//}
//
//static class QuestTracker {
//
//	// adds a quest
//	public static void Add(string name, object obj = null) {
//		Quest quest = new Quest(name, obj);
//		// null check
//		if (quest == Quest.Null) {
//			Debug.LogError("Quest was null");
//		}
//		// add quest
//		else if (!s_quests.ContainsKey(quest.name)) {
//			s_quests.Add(quest.name, quest);
//		}
//		// failed, contains key
//		else {
//			Debug.LogError($"Cannot add quest with name {quest.name}");
//		}
//	}
//
//	// returns a quest with the given name
//	public static Quest Get(string name) {
//		try {
//			return s_quests[name];
//		}
//		// failed
//		catch (KeyNotFoundException) {
//			return Quest.Null;
//		}
//	}
//
//	// invoke this when its time to reset the contained quests
//	public static void Reset() {
//		s_quests.Clear();
//	}
//
//	private static Dictionary<string, Quest> s_quests = new Dictionary<string, Quest>();
//}
//