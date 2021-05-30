using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class RestartIfPlayerDeadOnEvent : ActivateOnEvent {

	protected override void OnActivate() {
		if (!GameManager.Player) {
			Debug.LogError("No player was found in the game manager!");
			return;
		}
		if (GameManager.Player.IsDead) {
			GameManager.RestartGame();
		}
	}
}
