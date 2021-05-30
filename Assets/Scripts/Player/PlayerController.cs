using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	// input

	[SerializeField] private InputHandler m_input = null;
	private bool Left => (m_input ? m_input.Left : false);
	private bool Right => (m_input ? m_input.Right : false);
	private bool Up => (m_input ? m_input.Up : false);
	private bool Down => (m_input ? m_input.Down : false);
	private bool lastPrimaryState = false;
	private bool Primary => (m_input ? m_input.Primary : false);
	private bool SecondaryPressed => m_input.SecondaryPressed;

	// references

	private Rigidbody2D m_body = null;
	private BoxCollider2D m_box = null;
	private SpriteRenderer m_spr = null;

	[Header("Movement")]
	[SerializeField] private float m_hAcceleration;
	[SerializeField] private float m_hMaxSpeed;
	[SerializeField] private float m_vAcceleration;
	[SerializeField] private float m_vMaxFallSpeed;
	[SerializeField] private float m_vJumpSpeed;

	[Header("Layers")]
	[SerializeField] private LayerMask m_damageLayermask;

	[Header("Other")]
	[SerializeField] private float m_timeToRestartAfterDeath;

	// interactions
	private List<Interactable> m_interactables = new List<Interactable>();

	// collision

	//private List<Collision2D> m_collisions = new List<Collision2D>();
	[SerializeField] private float m_groundCheckAccuracy = 0.9f;
	[SerializeField] private ContactFilter2D m_groundMask;

	// state

	public enum PlayerState : byte {
		Alive,
		Dead
	}

	public bool IsGrounded { get; private set; } = false;
	public PlayerState State { get; private set; } = PlayerState.Alive;
	private float m_timerToRestart = 0f;

	private void Awake() {
		m_body = GetComponent<Rigidbody2D>();
		m_box = GetComponent<BoxCollider2D>();
		m_spr = GetComponent<SpriteRenderer>();
		if (!m_body || !m_box || !m_spr) {
			Debug.LogError("Missing components!");
			gameObject.SetActive(false);
			return;
		}

		if (!GameManager.Player) GameManager.Player = this;

		Schedule.onTimerEnd += OnGameTimerEnd;
		Schedule.AddToSchedule(OnPlayerStruggle, 3);
	}

	private void OnDestroy() {
		if (GameManager.Player == this) GameManager.Player = null;

		Schedule.onTimerEnd -= OnGameTimerEnd;
		Schedule.RemoveFromSchedule(OnPlayerStruggle, 3);
	}

	private void OnGameTimerEnd() {
		Debug.Log("Player is dead");
	}

	private void OnPlayerStruggle() {
		Debug.Log("Player is struggling");
	}

	private void Update() {
		if (Right != Left) {
			if (Right) m_spr.flipX = false;
			else if (Left) m_spr.flipX = true;
		}

		// check for interaction
		if (SecondaryPressed && m_interactables.Count > 0) {
			Interactable i = m_interactables[m_interactables.Count - 1];
			i.OnInteract(this);
		}

		//if (m_input.SecondaryPressed) GameManager.TimeScale = 20f;
		//else if (m_input.SecondaryReleased) GameManager.TimeScale = 1f;
	}

	private void FixedUpdate() {
		// is alive
		if (State == PlayerState.Alive) {
			CheckGrounded();
			DoMovement();
		}
		// is dead
		else if (State == PlayerState.Dead) {
			m_body.velocity = Vector2.zero;
			m_timerToRestart -= GameManager.FixedDeltaTime;
			if (m_timerToRestart <= 0f) {
				GameManager.RestartGame();
			}
		}

		lastPrimaryState = Primary;
	}

	private bool NearlyEqual(float a, float b, float perc = 0.01f) {
		return Mathf.Abs(a - b) < perc;
	}

	private bool NearlyZero(float a, float perc = 0.01f) {
		return Mathf.Abs(a) < perc;
	}

	private void DoMovement() {
		float delta = GameManager.FixedDeltaTime;
		Vector2 vel = m_body.velocity;
		float hacceldelta = m_hAcceleration * delta;

		// move left or right
		if (Left != Right) {
			if (Left) {
				if (NearlyEqual(vel.x, -m_hMaxSpeed, hacceldelta)) vel.x = -m_hMaxSpeed;
				else if (vel.x > -m_hMaxSpeed) vel.x -= hacceldelta;
				else vel.x += hacceldelta;
			}
			if (Right) {
				if (NearlyEqual(vel.x, m_hMaxSpeed, hacceldelta)) vel.x = m_hMaxSpeed;
				else if (vel.x < m_hMaxSpeed) vel.x += hacceldelta;
				else vel.x -= hacceldelta;
			}
		}
		// slow down
		else {
			if (NearlyZero(vel.x, hacceldelta)) vel.x = 0f;
			else if (vel.x < 0f) vel.x += hacceldelta;
			else vel.x -= hacceldelta;
		}

		if (IsGrounded && (Primary && !lastPrimaryState) && KnowledgeInventory.Contains("Jump")) {
			vel.y = m_vJumpSpeed;
		}

		if (vel.y < -m_vMaxFallSpeed) vel.y = -m_vMaxFallSpeed;
		else vel.y -= m_vAcceleration * delta;

		// set the new velocity
		m_body.velocity = vel;

	}

	private void CheckGrounded() {
		//IsGrounded = true;
		//IsGrounded = false;
		//foreach (Collision2D coll in m_collisions) {
		//	for (int i = 0; i < coll.contactCount; i++) {
		//		ContactPoint2D co = coll.GetContact(i);
		//		if (Vector2.Dot(co.normal, Vector2.up) > m_groundCheckAccuracy) {
		//			IsGrounded = true;
		//			return;
		//		}
		//	}
		//}

		IsGrounded = false;
		List<ContactPoint2D> contacts = new List<ContactPoint2D>();
		m_body.GetContacts(m_groundMask, contacts);

		for (int i = 0; i < contacts.Count; i++) {
			ContactPoint2D co = contacts[i];
			if (Vector2.Dot(co.normal, Vector2.up) > m_groundCheckAccuracy) {
				IsGrounded = true;
				return;
			}
		}
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		Interactable inter = collision.GetComponent<Interactable>();
		if (inter != null) {
			m_interactables.Add(inter);
			inter.OnEnterRange(this);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		Interactable inter = collision.GetComponent<Interactable>();
		if (inter != null) {
			m_interactables.Remove(inter);
			inter.OnExitRange(this);
		}
	}

	private void OnCollisionEnter2D(Collision2D collision) {
		// add collision
		//m_collisions.Add(collision);

		// check for damage
		if ((m_damageLayermask.value & (1 << collision.gameObject.layer)) != 0) {
			State = PlayerState.Dead;
			m_timerToRestart = m_timeToRestartAfterDeath;
			m_spr.enabled = false;
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		// remove collision
		//m_collisions.Remove(collision);
	}

}
