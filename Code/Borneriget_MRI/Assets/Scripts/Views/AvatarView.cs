using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Borneriget.MRI
{
    public class AvatarView : MonoBehaviour
	{
		public enum State {
			NEUTRAL,
			HAPPY,
			STRESSED,
			SLEEP,
			SLEEPING,
			BYE
		}

		[SerializeField]
		private AudioClip[] speaks = null;

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

			foreach (var child in gameObject.GetComponentsInChildren<SpriteRenderer>())
			{
				var childCollider = child.gameObject.AddComponent<BoxCollider2D>();
				childCollider.usedByComposite = true;
			}

			SetState(State.SLEEPING);
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

		public void WakeUp(Action awake)
        {
			StartCoroutine(WakeUpCo(awake));
        }

        private IEnumerator WakeUpCo(Action awake)
        {
			SetState(State.NEUTRAL);
			yield return new WaitForSeconds(3);
			awake();
        }

        public void Sleep() {
			SetState(State.SLEEPING);
		}

		public void Speak(int progress, Action speakDone)
		{
			StartCoroutine(SpeakCo(speaks.SafeGet(progress), speakDone));
		}

		public void StopSpeak()
		{
			StopAllCoroutines();
			animator.SetBool("talking", false);
			audioSource.Stop();
		}

		private IEnumerator SpeakCo(AudioClip clip, Action speakDone)
		{			
			audioSource.PlayOneShot(clip);
			animator.SetBool("talking", true);
			yield return new WaitForSeconds(clip.length);
			animator.SetBool("talking", false);
			audioSource.Stop();
			speakDone();
		}
    }
}