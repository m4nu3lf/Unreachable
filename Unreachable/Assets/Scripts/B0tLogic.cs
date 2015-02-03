using UnityEngine;
using System.Collections;
using System;

public class B0tLogic : StateMachine
{
	// Movement parameters
	// NOTE: Steps movement is approximated with a cosine function
	public float maxVelocity;
	public float timeScale;
	
	// How long the character will push before transit to the next state
	public float pushingTime;
	
	// Time has to pass in HeadTurning state before the suprise sign appear
	public float surpriseDelay;
	
	// Pushed object
	public Transform pushed;
	
	// The icon script
	public IconLogic icon;
	
	// The character script
	public CharacterLogic characterLogic;
		
	// The camera script
	public CameraLogic cameraLogic;
	
	// The game object switch
	GameObjectSwitch _objectSwitch;
	
	// The current velocity
	float _velocity;
	
	// Animator cache
	Animator animator;
	
	public enum States
	{
		Pushing,
		HeadTurning
	}
	
	// Use this for initialization
	void Start ()
	{
		// Cache the animator
		animator = gameObject.GetComponent<Animator>();
		
		// Cache the game object switch
		_objectSwitch = GameObject.Find("Object Switch").GetComponent<GameObjectSwitch>();
		
		// Start by pushing
		currentState = States.Pushing;

	}
	
	void OnEnable()
	{
		// Make the camera look at the bot character
		cameraLogic.alternativeTarget = transform;
		cameraLogic.currentState = CameraLogic.States.TargetTransition;
		
		// Disable main character input and put the character in the Idle state
		characterLogic.inputEnabled = false;
		characterLogic.currentState = CharacterLogic.States.Idle;
		
		// Turn the main character toward the bot character
		characterLogic.StartCoroutine(characterLogic.SmoothLookAt(transform.position));
	}
	
	// ------------------------------- Pushing state ---------------------------
	void Pushing_EnterState()
	{
		animator.SetInteger("state", 0);
	}
	
	void Pushing_Update ()
	{
		// Steps movement is approximated with a cosine function
		_velocity = Mathf.Abs(Mathf.Cos(timeInCurrentState * timeScale) * maxVelocity);
		
		transform.Translate(transform.forward * _velocity * Time.deltaTime, Space.World);
		pushed.Translate(transform.forward * _velocity * Time.deltaTime, Space.World);
		
		if (timeInCurrentState > pushingTime)
		{
			currentState = States.HeadTurning;
		}
	}
	
	// --------------------------------- HeadTurning state ---------------------
	IEnumerator HeadTurning_EnterState()
	{
		animator.SetInteger("state", 1);
		
		yield return new WaitForSeconds(surpriseDelay);
		
		icon.currentState = IconLogic.States.SurpriseSignBlinking;
		
		while(icon.currentState.Equals(IconLogic.States.Hidden))
		{
			yield return null;
		}
		
		yield return new WaitForSeconds(0.5f);
		
		_objectSwitch.activeIndex ++;
	}
}
