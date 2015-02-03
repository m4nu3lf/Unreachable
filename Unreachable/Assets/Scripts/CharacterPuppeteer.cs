using UnityEngine;
using System.Collections;
using System;

public class CharacterPuppeteer : MonoBehaviour {
	
	// The sound of the character hitting the ground
	public AudioClip thudClip;
	
	// The character spwan object
	public Transform characterSpawnObject;
	
	// The character script
	CharacterLogic _character;
	
	// The main camera
	public GameObject mainCamera;
		
	// The secondary camera
	public GameObject secondaryCamera;
	
	// The delay before the character goes to bed
	public float delayBeforeGoingToBed;
	
	// The delay between laying in the bed and closing the eyes
	public float eyeCloseDelay;
	
	// Use this for initialization
	IEnumerator Start ()
	{
		// Cache the character script
		_character = GameObject.Find("character").GetComponent<CharacterLogic>();
		
		// Put the character near to the bed
		_character.transform.position = characterSpawnObject.position;
		_character.transform.rotation = characterSpawnObject.rotation;
			
		// Play the thud noise
		_character.audio.PlayOneShot(thudClip);
			
		// Set the looking camera to the current camera
		_character.lookingCamera = GameObject.Find("Main Camera").GetComponent<Camera>();
		
		// Give the character the bed game object
		_character.bed = GameObject.Find ("Bed");
			
		// Disable the top light
		_character.topLight.enabled = false;
		
		// Change the character state
		_character.currentState = CharacterLogic.States.GettingUpFromGround;
		
		// Disable character input
		_character.inputEnabled = false;
		
		// Wait for the getting up animation to end
		yield return StartCoroutine(WaitForState(_character, CharacterLogic.States.Idle));
		
		// Wait before going to bed
		yield return new WaitForSeconds(delayBeforeGoingToBed);
		
		// Make the character going to bed
		_character.currentState = CharacterLogic.States.GoingToBed;
		
		// Wait for the going to bed animation to end
		yield return StartCoroutine(WaitForState(_character, CharacterLogic.States.InBed));
		
		// Switch the the secondary camera
		mainCamera.SetActive(false);
		secondaryCamera.SetActive(true);
		
		// Wait some time before closing eyes
		yield return new WaitForSeconds(eyeCloseDelay);
		
		// Make the character eyes closed
		_character.CloseEyes();
	}
	
	static IEnumerator WaitForState(StateMachine fsm, Enum state)
	{
		while(!fsm.currentState.Equals(state))
		{
			yield return null;
		}
	}
	
	
}
