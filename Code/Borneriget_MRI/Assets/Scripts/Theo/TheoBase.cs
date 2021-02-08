using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TheoBase : MonoBehaviour {
	public enum Gender {
		MALE,
		FEMALE
	}
	
	private List<SpriteRenderer> theaObjects = new List<SpriteRenderer>();
	private List<SpriteRenderer> theoObjects = new List<SpriteRenderer>();
	
	protected AudioSource audioSource;
	protected Animator animator;

	public void Awake() {
		//Find the gender specific body parts
		foreach (var child in GetComponentsInChildren<SpriteRenderer>(includeInactive: true)) {
			if (child != this.gameObject) {
				if (child.CompareTag("Female")) {
					theaObjects.Add(child);
				}
				else if (child.CompareTag("Male")) {
					theoObjects.Add(child);
				}
			}
		}
		SetGender(Gender.MALE);

		audioSource = GetComponent<AudioSource>();
		animator = GetComponent<Animator>();
		
		Assert.IsNotNull(audioSource);
		Assert.IsNotNull(animator);
		
		OnAwake();
	}

	protected virtual void OnAwake() { }
	
	public void SetGender(Gender sex) {
		foreach (var theoObject in theoObjects) {
			theoObject.enabled = (sex == Gender.MALE);
		}
		foreach (var theaObject in theaObjects) {
			theaObject.enabled = (sex == Gender.FEMALE);
		}
	}
	
	public void Speak(AudioClip audioClip) {
		StartCoroutine(SpeakCoroutine(audioClip));
	}

	public void StopSpeak() {
		StopAllCoroutines();
		animator.SetBool("talking", false);
		audioSource.Stop();
	}
	
	private IEnumerator SpeakCoroutine(AudioClip audioClip) {
		audioSource.PlayOneShot(audioClip);
		animator.SetBool("talking", true);
		yield return new WaitForSeconds(audioClip.length);
		animator.SetBool("talking", false);
	}
	
	[ContextMenu("MALE")]
	protected void DoMale() {
		SetGender(Gender.MALE);
	}
	
	[ContextMenu("FEMALE")]
	protected void DoFemale() {
		SetGender(Gender.FEMALE);
	}
}
