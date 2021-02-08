using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Theo : TheoBase {
	public enum State {
		NEUTRAL,
		HAPPY,
		STRESSED,
		SLEEP,
		SLEEPING,
		BYE
	}

	private State currentState;
	private new CompositeCollider2D collider;

	protected override void OnAwake() {
		SetState(State.SLEEPING);

		collider = gameObject.AddComponent<CompositeCollider2D>();
		collider.isTrigger = true;
		collider.geometryType = CompositeCollider2D.GeometryType.Polygons;
		var rigidBody = GetComponent<Rigidbody2D>();
		rigidBody.simulated = false;
		foreach (var child in gameObject.GetComponentsInChildren<SpriteRenderer>()) {
			var childCollider = child.gameObject.AddComponent<BoxCollider2D>();
			childCollider.usedByComposite = true;
		}
	}

	private void Update() {
		if (Debug.isDebugBuild) {
			if (Input.GetKeyUp(KeyCode.Alpha1)) SetState(State.NEUTRAL);
			if (Input.GetKeyUp(KeyCode.Alpha2)) SetState(State.HAPPY);
			if (Input.GetKeyUp(KeyCode.Alpha3)) SetState(State.STRESSED);
			if (Input.GetKeyUp(KeyCode.Alpha4)) SetState(State.SLEEP);
			if (Input.GetKeyUp(KeyCode.Alpha5)) SetState(State.SLEEPING);
			if (Input.GetKeyUp(KeyCode.Alpha6)) SetState(State.BYE);
		}
	}

	public void SetState(State state) {
		currentState = state;
		animator.SetBool("happy", state == State.HAPPY);
		animator.SetBool("stress", state == State.STRESSED);
		animator.SetBool("sleep", state == State.SLEEP);
		animator.SetBool("sleeping", state == State.SLEEPING);
		if (state == State.BYE) {
			animator.SetTrigger("bye");
		}
	}

	public bool IsPointWithinBear(Vector2 point) {
		return collider.OverlapPoint(Camera.main.ScreenToWorldPoint(point));
	}

	public State GetState() {
		return currentState;
	}

	public void Sleep() {
		SetState(State.SLEEPING);
	}
}