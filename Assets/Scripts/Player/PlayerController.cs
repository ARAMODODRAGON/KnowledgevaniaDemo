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
	private bool lastTeritaryState = false;
	private bool Teritary => m_input.Tertiary;

	// references

	[SerializeField] GameObject m_camera;
	private Rigidbody2D m_body = null;
	private BoxCollider2D m_box = null;
	private SpriteRenderer m_spr = null;
	[SerializeField] private ActivateOnEvent m_fadeOutEvent;

	[Header("Movement")]
	[SerializeField] private float m_hAcceleration;
	[SerializeField] private float m_hMaxSpeed;
	[SerializeField] private float m_vAcceleration;
	[SerializeField] private float m_vFastAcceleration;
	[SerializeField] private float m_vMaxFallSpeed;
	[SerializeField] private float m_vJumpSpeed;
	[SerializeField] private float m_dashSpeed;
	[SerializeField] private float m_dashAcceleration;
	[SerializeField] private float m_dashDistance;

	[Header("Layers")]
	[SerializeField] private LayerMask m_damageLayermask;

	[Header("Other")]
	[SerializeField] private float m_colorCyclesPerSecond;
	[SerializeField] private float m_timeToRestartAfterDeath;
	[SerializeField] private float m_fallTimeToAutoDie;

	// interactions
	private List<Interactable> m_interactables = new List<Interactable>();

	// collision

	//private List<Collision2D> m_collisions = new List<Collision2D>();
	[SerializeField] private float m_groundCheckAccuracy;
	[SerializeField] private ContactFilter2D m_groundFilter;

	// state

	public bool IsDead { get; private set; } = false;
	public bool IsGrounded { get; private set; } = false;
	public bool IsDashing { get; private set; } = false;
	private float m_timerToRestart = 0f;
	private float m_fallTimer = 0f;

	private int m_dashCount = 1;
	private float m_dashTimer = 0f;
	private bool m_canJumpDuringDash = false;

	private float m_colorTimer = 0f;

	private Vector2 m_lastPos = Vector2.zero;
	private float m_standingTimer = 0f;
	private bool m_timespeeding = false;

	private void Awake() {
		if (!KnowledgeInventory.Contains("To Remember")) KnowledgeInventory.Learn("To Remember");

		m_body = GetComponent<Rigidbody2D>();
		m_box = GetComponent<BoxCollider2D>();
		m_spr = GetComponent<SpriteRenderer>();
		if (!m_body || !m_box || !m_spr) {
			Debug.LogError("Missing components!");
			gameObject.SetActive(false);
			return;
		}

		// detach camera
		if (m_camera) {
			m_camera.transform.parent = null;
		}

		if (!GameManager.Player) GameManager.Player = this;

	}

	private void OnDestroy() {
		if (GameManager.Player == this) GameManager.Player = null;
		if (m_timespeeding) GameManager.TimeScale = 1f;
	}

	private void Update() {
		// check standing
		Vector2 newpos = new Vector2(transform.position.x, transform.position.y);
		if (m_lastPos == newpos) {
			m_standingTimer += GameManager.DeltaTime;
			if (m_standingTimer > 20f) {
				GameManager.TimeScale = 10f;
				m_timespeeding = true;
			}
		} else {
			if (m_timespeeding) {
				m_standingTimer = 0f;
				GameManager.TimeScale = 1f;
				m_timespeeding = false;
			}
		}
		m_lastPos = newpos;

		// flip sprite
		if (Right != Left && !IsDashing) {
			if (Right) m_spr.flipX = false;
			else if (Left) m_spr.flipX = true;
		}

		// color change if dashing
		if (IsDashing) {
			m_colorTimer += GameManager.DeltaTime;
			if (m_colorTimer > m_colorCyclesPerSecond) m_colorTimer -= m_colorCyclesPerSecond;
			m_spr.color = Color.HSVToRGB(m_colorTimer / m_colorCyclesPerSecond, 1f, 1f);
		}
		// no color change
		else {
			m_spr.color = Color.white;
		}

		// check for interaction
		if (SecondaryPressed && m_interactables.Count > 0) {
			Interactable i = m_interactables[m_interactables.Count - 1];
			i.OnInteract(this);
		}

		if (Input.GetKeyDown(KeyCode.I)) {
			KnowledgeInventory.Learn("Jump", KnowledgeType.Ability);
			KnowledgeInventory.Learn("Dash", KnowledgeType.Ability);
			KnowledgeInventory.Learn("Double Dash", KnowledgeType.Ability);
		}
		//if (m_input.SecondaryPressed) GameManager.TimeScale = 20f;
		//else if (m_input.SecondaryReleased) GameManager.TimeScale = 1f;
	}

	private void FixedUpdate() {
		if (GameManager.IsPaused) return;
		// is alive
		if (!IsDead) {
			CheckGrounded();
			DoMovement();

			// countdown
			if (!IsGrounded) {
				m_fallTimer += GameManager.FixedDeltaTime;
				if (m_fallTimer > m_fallTimeToAutoDie) {
					IsDead = true;
					m_timerToRestart = m_timeToRestartAfterDeath;
					m_spr.enabled = false;
				}
			}
			// reset timer
			else m_fallTimer = 0f;
		}
		// is dead
		else if (m_timerToRestart > 0f) {
			m_body.velocity = Vector2.zero;
			m_timerToRestart -= GameManager.FixedDeltaTime;
			if (m_timerToRestart <= 0f) {
				m_fadeOutEvent.Activate();
			}
		}

		lastPrimaryState = Primary;
		lastTeritaryState = Teritary;
	}

	private bool NearlyEqual(float a, float b, float perc = 0.01f) {
		return Mathf.Abs(a - b) < perc;
	}

	private bool NearlyZero(float a, float perc = 0.01f) {
		return Mathf.Abs(a) < perc;
	}

	private void DoMovement() {
		if (GameManager.IsPaused || Mathf.Abs(GameManager.TimeScale) <= 0.001f) {
			return;
		}

		float delta = GameManager.FixedDeltaTime;
		Vector2 vel = m_body.velocity;
		int maxdashes = (KnowledgeInventory.Contains("Double Dash") ? 2 : 1);

		float vacceleration = ((Primary || vel.y < 0f) ? m_vAcceleration : m_vFastAcceleration);

		// only when not dashing
		if (!IsDashing) {

			// jump stuff
			if (IsGrounded && (Primary && !lastPrimaryState) &&
				KnowledgeInventory.Contains("Jump")) {
				vel.y = m_vJumpSpeed;
			}

			// falling stuff
			if (vel.y < -m_vMaxFallSpeed) {
				vel.y = -m_vMaxFallSpeed;
			} else {
				vel.y -= vacceleration * delta;
			}

			if (KnowledgeInventory.Contains("Dash")) {
				// dash starting stuff
				if (IsGrounded && !IsDashing) m_dashCount = maxdashes;
				if ((m_dashCount > 0) && (Teritary && !lastTeritaryState)) {
					// start dash
					m_dashCount--;
					m_dashTimer = m_dashDistance / m_dashSpeed;
					IsDashing = true;
					m_canJumpDuringDash = IsGrounded;
					vel.y = 0f;
				}
			}

		}
		// when dashing
		else {
			m_dashTimer -= GameManager.FixedDeltaTime;

			// jump stuff
			if ((IsGrounded || m_canJumpDuringDash) && (Primary && !lastPrimaryState) &&
				KnowledgeInventory.Contains("Jump")) {
				vel.y = m_vJumpSpeed;
				m_dashTimer = 0f;
				m_dashCount = maxdashes;
				m_canJumpDuringDash = false;
			}

			// end dash
			if (m_dashTimer <= 0f) {
				m_dashTimer = 0f;
				IsDashing = false;
				m_canJumpDuringDash = false;
			}
		}

		float hacceldelta = (!IsDashing ? m_hAcceleration : m_dashAcceleration) * delta;
		float hmaxspeed = (!IsDashing ? m_hMaxSpeed : m_dashSpeed);
		bool left = (!IsDashing ? Left : m_spr.flipX);
		bool right = (!IsDashing ? Right : !m_spr.flipX);

		// move left or right
		if (left != right) {
			if (left) {
				if (NearlyEqual(vel.x, -hmaxspeed, hacceldelta)) vel.x = -hmaxspeed;
				else if (vel.x > -hmaxspeed) vel.x -= hacceldelta;
				else vel.x += hacceldelta;
			}
			if (right) {
				if (NearlyEqual(vel.x, hmaxspeed, hacceldelta)) vel.x = hmaxspeed;
				else if (vel.x < hmaxspeed) vel.x += hacceldelta;
				else vel.x -= hacceldelta;
			}
		}
		// slow down
		else {
			if (NearlyZero(vel.x, hacceldelta)) vel.x = 0f;
			else if (vel.x < 0f) vel.x += hacceldelta;
			else vel.x -= hacceldelta;
		}

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
		m_body.GetContacts(m_groundFilter, contacts);

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
			IsDead = true;
			m_timerToRestart = m_timeToRestartAfterDeath;
			m_spr.enabled = false;
		}
	}

	private void OnCollisionExit2D(Collision2D collision) {
		// remove collision
		//m_collisions.Remove(collision);
	}

}
