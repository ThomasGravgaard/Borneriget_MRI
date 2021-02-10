using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borneriget.MRI
{
	public class AvatarView : MonoBehaviour {
		public enum State {
			NEUTRAL,
			HAPPY,
			STRESSED,
			SLEEP,
			SLEEPING,
			BYE
		}

		private State currentState;
		private List<SpriteRenderer> theaObjects = new List<SpriteRenderer>();
		private List<SpriteRenderer> theoObjects = new List<SpriteRenderer>();
		private AudioSource audioSource;
		private Animator animator;

		private void Awake()
		{
			//Find the gender specific body parts
			foreach (var child in GetComponentsInChildren<SpriteRenderer>(includeInactive: true))
			{
				if (child != this.gameObject)
				{
					if (child.CompareTag("Female"))
					{
						theaObjects.Add(child);
					}
					else if (child.CompareTag("Male"))
					{
						theoObjects.Add(child);
					}
				}
			}
			audioSource = GetComponent<AudioSource>();
			animator = GetComponent<Animator>();

			SetState(State.NEUTRAL);
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

		public void Show(PreferencesProxy.Avatars avatar)
        {
			gameObject.SetActive(true);
			foreach (var theoObject in theoObjects)
			{
				theoObject.enabled = (avatar == PreferencesProxy.Avatars.Theo);
			}
			foreach (var theaObject in theaObjects)
			{
				theaObject.enabled = (avatar == PreferencesProxy.Avatars.Thea);
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

		public State GetState() {
			return currentState;
		}

		public void Sleep() {
			SetState(State.SLEEPING);
		}

		public void Speak(Action speakDone)
		{
			StartCoroutine(SpeakCoroutine(speakDone));
		}

		public void StopSpeak()
		{
			StopAllCoroutines();
			animator.SetBool("talking", false);
			audioSource.Stop();
		}

		private IEnumerator SpeakCoroutine(Action speakDone)
		{
			audioSource.PlayOneShot(audioSource.clip);
			animator.SetBool("talking", true);
			yield return new WaitForSeconds(13);
			animator.SetBool("talking", false);
			Debug.Log("Speak done");
			audioSource.Stop();
			speakDone();
		}
	}
}