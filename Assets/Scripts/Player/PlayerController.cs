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

	// references

	private Rigidbody2D m_body = null;
	private SpriteRenderer m_spr = null;

	[Header("Movement")]
	[SerializeField] private float m_hAcceleration;
	[SerializeField] private float m_hMaxSpeed;
	[SerializeField] private float m_vAcceleration;
	[SerializeField] private float m_vMaxFallSpeed;
	[SerializeField] private float m_vJumpSpeed;

	// collision

	//private List<Collision2D> m_collisions = new List<Collision2D>();
	public bool IsGrounded { get; private set; } = false;
	[SerializeField] private float m_groundCheckAccuracy = 0.9f;

	private void Awake() {
		m_body = GetComponent<Rigidbody2D>();
		m_spr = GetComponent<SpriteRenderer>();
		if (!m_body || !m_spr) {
			Debug.LogError("Missing components!");
			gameObject.SetActive(false);
			return;
		}
	}

	private void Update() {
		if (Right != Left) {
			if (Right) m_spr.flipX = false;
			else if (Left) m_spr.flipX = true;
		}
	}

	private void FixedUpdate() {
		CheckGrounded();
		DoMovement();

		lastPrimaryState = Primary;
	}

	private bool NearlyEqual(float a, float b, float perc = 0.01f) {
		return Mathf.Abs(a - b) < perc;
	}

	private bool NearlyZero(float a, float perc = 0.01f) {
		return Mathf.Abs(a) < perc;
	}

	private void DoMovement() {
		float delta = Time.fixedDeltaTime;
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

		if (IsGrounded && (Primary && !lastPrimaryState)) {
			vel.y = m_vJumpSpeed;
		}

		if (vel.y < -m_vMaxFallSpeed) vel.y = -m_vMaxFallSpeed;
		else vel.y -= m_vAcceleration * delta;

		// set the new velocity
		m_body.velocity = vel;

	}

	private void CheckGrounded() {
		IsGrounded = true;
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
	}

	//private void OnCollisionEnter2D(Collision2D collision) {
	//	m_collisions.Add(collision);
	//}
	
	//private void OnCollisionExit2D(Collision2D collision) {
	//	m_collisions.Remove(collision);
	//}
}
