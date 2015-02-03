using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class CharacterLogic : StateMachine
{
	// The camera the input is relative to
	public Camera lookingCamera;
	
	// The light pointing the character
	public Light topLight;
	
	// The shadow plane
	public GameObject shadowPlane;
	
	// The texture for the closed eyes
	public Texture eyeClosedTexture;
	
	// The bed to drop into
	public GameObject bed;
	
	// The offset respect to the bed
	public Vector3 bedOffset;
	
	// Wheter the input is enable or not
	public bool inputEnabled;
	
	// maximum delay between two tap to be considered a running command
	public float doublePressDelay = 0.3f;
	
	// Input delay before the character is supposed not to be running
	public float runningStopDelay = 0.2f;
	
	// Movements velocities
	public float walkVelocity;
	public float runVelocity;
	public float turnVelocity;
	
	// Store the directional input as a Vector2
	Vector2 _directionalInput = Vector2.zero;
	
	// The gravity acceleration
	public float gravityAcceleration;
	
	// Update the commands direction
	void UpdateDirectionalInput()
	{
		if (!inputEnabled)
		{
			_directionalInput = new Vector2();
			return;
		}
		
		_directionalInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		
		// Clamp magnitude to one
		if (_directionalInput.sqrMagnitude > 1)
		{
			_directionalInput.Normalize();
		}
	}
	
	// Compute the direction relative to the camera
	void ComputeCameraRelativeDirection()
	{
		if (_directionalInput.x != 0.0f || _directionalInput.y != 0.0f)
		{
			// Compute the forward vector relative to the camera
			Vector3 forward = (transform.position - lookingCamera.transform.position);
			forward -= Vector3.Project(forward, transform.up); // remove the up component from the forward vector
			forward.Normalize();
			
			// Compute the angle of the directional input
			float dirAngle = Mathf.Atan2(_directionalInput.y, _directionalInput.x);
			dirAngle -= Mathf.PI/2;
			
			// Rotate the forward vector around the up vector by the direction angle
			// (I don't know why but seems that Unity uses the non standard convention for rotations, so i need the minus)
			forward = Quaternion.AngleAxis(- Mathf.Rad2Deg * dirAngle, transform.up) * forward;
							
			// Calculate the quaternion for the new forward and set the new orientation
			transform.rotation = Quaternion.LookRotation(forward);
		}
	}
	
	// The animator, for animations control
	Animator _animator;
	
	// The game object holding the mesh
	GameObject _model;
	
	// The character controller
	CharacterController _controller;
	
	void Start()
	{		
		// Cache the animator
		_animator = gameObject.GetComponent<Animator>();
		
		// Cache the mesh object
		_model = GameObject.Find("Character");
		
		// Chache the character controller
		_controller = gameObject.GetComponent<CharacterController>();
		
		// Starting state
		currentState = States.LookAround;
		
		// Character should not be destroyed when loading the new scene
		Object.DontDestroyOnLoad(gameObject);
	}
	
	// Wait for the given animator state
	IEnumerator WaitForAnimatorState(int layer, string stateName)
	{
		int nameHash = Animator.StringToHash(stateName);
		while(_animator.GetCurrentAnimatorStateInfo(layer).nameHash != nameHash)
		{
			yield return null;
		}
	}
	
	// Make the eyes closed
	public void CloseEyes()
	{
		_model.renderer.materials[5].mainTexture = eyeClosedTexture;
	}
	
	// Turn the player to a target
	public IEnumerator SmoothLookAt(Vector3 target)
	{
		// Keep the time
		float time = 0.0f;
		
		// Keep the start rotation
		Quaternion startRotation = transform.rotation;
		
		// Remove the up component from the target vector
		target -= Vector3.Project(target, transform.up);
		
		// Get the look at rotation for the target
		Quaternion lookAtRotation = new Quaternion();
		lookAtRotation.SetLookRotation(target - transform.position);
		
		// Get the angle between the two quaternion
		float rotationAngle = Quaternion.Angle(startRotation, lookAtRotation);
		
		// Get the rotation time
		float rotationTime = rotationAngle  / turnVelocity;
		
		// While rotation time is not elapsed
		while (time < rotationTime)
		{
			time += Time.deltaTime;
			transform.rotation =
				QuaternionHelper.SmoothSlerp(startRotation, lookAtRotation, time / rotationTime);
			
			yield return null;
		}
	}
	
	
	// Make the player reach a given point by walking
	public IEnumerator SmoothTranslateTo(Vector3 target, float velocity)
	{
		// Keep the time
		float time = 0.0f;
		
		// Keep the start position
		Vector3 startPosition = transform.position;
		
		// Remove the up component from the target vector
		target -= Vector3.Project(target, transform.up);
		
		// Get the angle between the two quaternion
		float distance = Vector3.Distance(transform.position, target);
		
		// Get the walk time
		float walkTime = distance / velocity;
		
		// While walk time is not elapsed
		while (time < walkTime)
		{
			time += Time.deltaTime;
			
			float smoothT = Mathf.SmoothStep(0.0f, 1.0f, time / velocity);
			
			transform.position = Vector3.Lerp(startPosition, target, smoothT);
			
			yield return null;
		}
	}
	
	// Character states
	public enum States
	{
		Idle,
		Walking,
		Running,
		LookAround,
		Falling,
		GettingUpFromGround,
		GoingToBed,
		InBed
	}
	
	//------------------------------------Idle state------------------------
	void Idle_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.Idle);
	}
	
	void Idle_Update()
	{
		UpdateDirectionalInput();
		
		// check if the directional buttons points somewhere
		if (!_directionalInput.Equals(Vector2.zero))
		{
			currentState = States.Walking;
		}
	}
	
	
	//--------------------------------- Walking state ---------------------------
	void Walking_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.Walking);
	}
	
	void Walking_Update()
	{
		UpdateDirectionalInput();
		ComputeCameraRelativeDirection();
		
		_controller.Move(transform.forward * walkVelocity * Time.deltaTime
			* _directionalInput.magnitude);
		
		if (_directionalInput.Equals(Vector2.zero))
		{
			currentState = States.Idle;
		}
		else if (_directionalInput.magnitude > 0.5f)
		{
			currentState = States.Running;
		}
	}
	
	
	//--------------------------------- Running state ---------------------------
	void Running_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.Running);
	}
	
	void Running_Update()
	{
		UpdateDirectionalInput();
		ComputeCameraRelativeDirection();
		
		_controller.Move(transform.forward * runVelocity * Time.deltaTime
			* _directionalInput.magnitude);
		
		if (_directionalInput.Equals(Vector2.zero))
		{
			currentState = States.Idle;
		}
		else if (_directionalInput.magnitude < 0.5)
		{
			currentState = States.Walking;
		}
	}
	
	//--------------------------------- LookAround state ---------------------------	
	void LookAround_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.LookAround);
	}
	
	//--------------------------------- LookAround state ---------------------------
	float _fallingVelocity;
	
	void Falling_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.Falling);
	}
	
	void Falling_FixedUpdate()
	{
		// Update the fallingVelocity
		_fallingVelocity += gravityAcceleration * Time.deltaTime;
		
		_controller.Move(- transform.up * _fallingVelocity * Time.deltaTime
			* _directionalInput.magnitude);
	}
	
	// -------------------------------- GettingUpFromGround state --------------------
	IEnumerator GettingUpFromGround_EnterState()
	{
		_animator.SetInteger("StateId", (int)States.GettingUpFromGround);
		
		// Wait for the state of the animator to be Idle
		yield return StartCoroutine(WaitForAnimatorState(0, "Base Layer.Idle"));
		
		currentState = States.Idle;
	}
	
	// --------------------------------- GoingToBed state -----------------------------
	IEnumerator GoingToBed_EnterState()
	{	
		// Play the walking animation
		_animator.SetInteger("StateId", (int)States.Walking);
		
		// Reach the bed
		yield return StartCoroutine(SmoothLookAt(bed.transform.position));
		
		yield return StartCoroutine(SmoothTranslateTo(bed.transform.position, walkVelocity));
		
		yield return StartCoroutine(SmoothLookAt(transform.position - bed.transform.right));
		
		// Start the drop animation
		_animator.SetInteger("StateId", (int)States.GoingToBed);
		
		// Wait for the state of the animator to be InBed
		yield return StartCoroutine(WaitForAnimatorState(0, "Base Layer.InBed"));
		
		// Change the state
		currentState = States.InBed;
	}
}
