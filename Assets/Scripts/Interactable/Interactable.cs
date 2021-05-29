using UnityEngine;

public abstract class Interactable : MonoBehaviour {
	public abstract void OnEnterRange(PlayerController player);
	public abstract void OnExitRange(PlayerController player);
	public abstract void OnInteract(PlayerController player);
}
