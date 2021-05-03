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

		[Serializable]
		public struct SpeakInfo
		{
			public float AnimationDelay;
			public float AnimationStopOffset;
			public AudioClip Clip;
		}

		[SerializeField]
		private SpeakInfo[] speaksDk = null;
		[SerializeField]
		private SpeakInfo[] speaksUk = null;
		[SerializeField]
		private AudioClip wakeTheBear_dk = null;
		[SerializeField]
		private AudioClip wakeTheBear_uk = null;
		[SerializeField]
		private AudioClip snore = null;
		[SerializeField]
		private AudioClip waking = null;
		[SerializeField]
		private AudioSource snoreAudioSource = null;
		[SerializeField]
		private Vector2 scannerStopSpeakTiming_DK = Vector2.zero;
		[SerializeField]
		private Vector2 scannerStopSpeakTiming_UK = Vector2.zero;

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
			snoreAudioSource.clip = snore;
			snoreAudioSource.loop = true;
			snoreAudioSource.Play();
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
				animator.ResetTrigger("idle");
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
			audioSource.PlayOneShot((danishSpeaks) ? wakeTheBear_dk : wakeTheBear_uk);
		}

		public void WakeUp(Action awake)
        {
			StartCoroutine(WakeUpCo(awake));
        }

        private IEnumerator WakeUpCo(Action awake)
        {
			snoreAudioSource.Stop();
			snoreAudioSource.clip = waking;
			snoreAudioSource.loop = false;
			snoreAudioSource.Play();
			yield return new WaitForSeconds(2);
			SetState(State.NEUTRAL);
			yield return new WaitForSeconds(3);
			awake();
        }

		public void ShowDiploma()
        {
			StartCoroutine(ShowDiplomaCo());
		}

		private IEnumerator ShowDiplomaCo()
        {
			animator.SetBool("talking", true);
			yield return new WaitForSeconds(6);
			animator.SetBool("talking", false);
			SetState(State.DIPLOMA);
		}

		public void Sleep() {
			SetState(State.SLEEPING);
		}

		public void Speak(int progress, Action speakDone)
		{
			var clips = (danishSpeaks) ? speaksDk : speaksUk;
            if (progress == 3)
            {
				var timing = (danishSpeaks) ? scannerStopSpeakTiming_DK : scannerStopSpeakTiming_UK;
				StartCoroutine(SpeakWithPauseCo(clips.SafeGet(progress), speakDone, timing.x, timing.y));
			}
			else
            {
				StartCoroutine(SpeakCo(clips.SafeGet(progress), speakDone, progress < 5));
			}
		}

		public void StopSpeak()
		{
			StopAllCoroutines();
			animator.SetBool("talking", false);
			audioSource.Stop();
		}

		private IEnumerator SpeakCo(SpeakInfo speak, Action speakDone, bool activateTalk)
		{			
			audioSource.PlayOneShot(speak.Clip);
            if (activateTalk)
            {
				yield return new WaitForSeconds(speak.AnimationDelay);
				animator.SetBool("talking", true);
			}
			yield return new WaitForSeconds(speak.Clip.length - speak.AnimationStopOffset - speak.AnimationDelay);
            if (activateTalk)
            {
				animator.SetBool("talking", false);
            }
			audioSource.Stop();
			speakDone();
		}

		private IEnumerator SpeakWithPauseCo(SpeakInfo speak, Action speakDone, float startPause, float endPause)
		{
			audioSource.PlayOneShot(speak.Clip);			
			yield return new WaitForSeconds(speak.AnimationDelay);
			animator.SetBool("talking", true);
			yield return new WaitForSeconds(startPause);
			animator.SetBool("talking", false);
			yield return new WaitForSeconds(endPause - startPause);
			animator.SetBool("talking", true);
			yield return new WaitForSeconds(speak.Clip.length - endPause - speak.AnimationStopOffset - speak.AnimationDelay);
			animator.SetBool("talking", false);
			audioSource.Stop();
			speakDone();
		}

	}
}