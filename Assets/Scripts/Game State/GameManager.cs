using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : ScriptableObject {
	public static GameManager Instance {
		get {
			if (!s_instance) {
				s_instance = FindObjectOfType<GameManager>();
				if (!s_instance) s_instance = CreateInstance<GameManager>();
			}
			return s_instance;
		}
	}
	private static GameManager s_instance = null;

	

}
