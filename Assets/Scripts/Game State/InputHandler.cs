using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Input Handler", menuName = "Assets/Input Handler")]
public class InputHandler : ScriptableObject {

	// properties

	public bool Left => Input.GetKey(m_leftKey);
	public bool Right => Input.GetKey(m_rightKey);
	public bool Down => Input.GetKey(m_downKey);
	public bool Up => Input.GetKey(m_upKey);

	public Vector2 Axis {
		get {
			Vector2 v = Vector2.zero;
			if (Left != Right) {
				if (Left) v.x = -1f;
				if (Right) v.x = 1f;
			}
			if (Down != Up) {
				if (Down) v.y = -1f;
				if (Up) v.y = 1f;
			}
			return v;
		}
	}

	public bool LeftPressed => Input.GetKeyDown(m_leftKey);
	public bool RightPressed => Input.GetKeyDown(m_rightKey);
	public bool DownPressed => Input.GetKeyDown(m_downKey);
	public bool UpPressed => Input.GetKeyDown(m_upKey);
	public bool PausedPressed => Input.GetKeyDown(m_pausedKey);

	public bool ConfirmPressed => Input.GetKeyDown(m_confirmKey);
	public bool CancelPressed => Input.GetKeyDown(m_cancelKey);

	public bool Primary => Input.GetKey(m_primaryKey);
	public bool Secondary => Input.GetKey(m_secondaryKey);
	public bool Tertiary => Input.GetKey(m_tertiaryKey);
	public bool PrimaryPressed => Input.GetKeyDown(m_primaryKey);
	public bool SecondaryPressed => Input.GetKeyDown(m_secondaryKey);
	public bool TertiaryPressed => Input.GetKeyDown(m_tertiaryKey);
	public bool PrimaryReleased => Input.GetKeyUp(m_primaryKey);
	public bool SecondaryReleased => Input.GetKeyUp(m_secondaryKey);
	public bool TertiaryReleased => Input.GetKeyUp(m_tertiaryKey);

	// inspector stuff

	[Header("General")]
	[SerializeField] private KeyCode m_leftKey = KeyCode.None;
	[SerializeField] private KeyCode m_rightKey = KeyCode.None;
	[SerializeField] private KeyCode m_upKey = KeyCode.None;
	[SerializeField] private KeyCode m_downKey = KeyCode.None;
	[SerializeField] private KeyCode m_pausedKey = KeyCode.None;

	[Header("UI")]
	[SerializeField] private KeyCode m_confirmKey = KeyCode.None;
	[SerializeField] private KeyCode m_cancelKey = KeyCode.None;

	[Header("Gameplay")]
	[SerializeField] private KeyCode m_primaryKey = KeyCode.None;
	[SerializeField] private KeyCode m_secondaryKey = KeyCode.None;
	[SerializeField] private KeyCode m_tertiaryKey = KeyCode.None;

}
