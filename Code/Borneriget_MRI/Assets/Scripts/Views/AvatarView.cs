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
			BYE,
			DIPLOMA
		}

		[SerializeField]
		private AudioClip[] speaks_dk = null;
		[SerializeField]
		private AudioClip wakeUp_dk = null;
		[SerializeField]
		private AudioClip[] speaks_uk = null;
		[SerializeField]
		private AudioClip wakeUp_uk = null;

		private State currentState;
		private List<SpriteRenderer> theaObjects = new List<SpriteRenderer>();
		private List<SpriteRenderer> theoObjects = new List<SpriteRenderer>();
		private AudioSource audioSource;
		private Animator animator;
		private bool danishSpeaks;

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

		public void Show(PreferencesProxy.Avatars avatar, bool danishSpeaks)
        {
			this.danishSpeaks = danishSpeaks;
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
            if (state == State.DIPLOMA)
            {
				animator.SetTrigger("diploma");
            }
		}

		public void BackToIdle()
        {
			SetState(State.NEUTRAL);
			animator.SetTrigger("idle");
		}

		public State GetState() {
			return currentState;
		}

		public void WakeUpSpeak()
        {
			audioSource.PlayOneShot((danishSpeaks) ? wakeUp_dk : wakeUp_uk);
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
			var clips = (danishSpeaks) ? speaks_dk : speaks_uk;
			StartCoroutine(SpeakCo(clips.SafeGet(progress), speakDone, progress < 5));
		}

		public void StopSpeak()
		{
			StopAllCoroutines();
			animator.SetBool("talking", false);
			audioSource.Stop();
		}

		private IEnumerator SpeakCo(AudioClip clip, Action speakDone, bool activateTalk)
		{			
			audioSource.PlayOneShot(clip);
            if (activateTalk)
            {
				animator.SetBool("talking", true);
			}
			yield return new WaitForSeconds(clip.length);
            if (activateTalk)
            {
				animator.SetBool("talking", false);
            }
			audioSource.Stop();
			speakDone();
		}
    }
}