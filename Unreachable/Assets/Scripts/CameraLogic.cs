using UnityEngine;
using System.Collections;

public class CameraLogic : StateMachine
{
	// The target of the camera
	public Transform character;
	
	// An alterative target for temporary sight
	public Transform alternativeTarget;
	
	// Time for target transition
	public float targetTransitionTime;
	
	// Maximum distance form the target
	public float maxDistance;
	
	// Minimum distance form the target
	public float minDistance;
	
	// Character height offset where to look at
	public float characterHeightOffset;
	
	// Speed factors for camera movements
	public float speedFactor;
	public float angularSpeedFactor;
	
	// Analog joystick speed and acceleration parameters
	public float analogMinSpeedFactor;
	public float analogMaxSpeedFactor;
	public float analogAccelerationFactor;
	float _analogSpeedFactorX;
	float _analogSpeedFactorY;
	
	// Time for idle mouse before switching to Auto mode
	public float inputToAutoDelay;
	
	// Mouse deltas
	float _deltaX;
	float _deltaY;
	
	bool UpdateUserInput()
	{
		// restore the input delta values
		_deltaX = _deltaY = 0.0f;
		
		// retrieve the gamepad right analog input
		float RightH = Input.GetAxis("Right H");
		float RightV = Input.GetAxis("Right V");
		
		// check for the horiziontal axe to be non zero
		if (RightH != 0.0f)
		{
			// compute deltaX
			_analogSpeedFactorX += analogAccelerationFactor * Time.deltaTime;
			_analogSpeedFactorX = Mathf.Min(_analogSpeedFactorX, analogMaxSpeedFactor);
			_deltaX += RightH * Time.deltaTime * _analogSpeedFactorX;
		}
		else
		{
			// restore the analogSpeedFactor to the minimum value
			_analogSpeedFactorX = analogMinSpeedFactor;
		}
		
		// check for the vertical axe to be non zero
		if (RightH != 0.0f)
		{
			// compute deltaY
			_analogSpeedFactorY += analogAccelerationFactor * Time.deltaTime;
			_analogSpeedFactorY = Mathf.Min(_analogSpeedFactorY, analogMaxSpeedFactor);
			_deltaY += -RightV * Time.deltaTime * _analogSpeedFactorY;
		}
		else
		{
			// restore the analogSpeedFactor to the maximum value
			_analogSpeedFactorY = analogMinSpeedFactor;
		}
		

		// check for mouse movement
		_deltaX += Input.GetAxis("Mouse X");
		_deltaY += Input.GetAxis("Mouse Y");
		
		return !Mathf.Approximately(_deltaX, 0.0f)
			|| !Mathf.Approximately(_deltaY, 0.0f);
	}
	
	// Position of the target
	Vector3 _targetPosition;
	
	void UpdateTargetPosition()
	{ 
		_targetPosition = character.position + character.up * characterHeightOffset;
	}
	
	// The offset of the target form the camera
	Vector3 _targetOffset;
	
	void UpdateTargetOffset()
	{
		_targetOffset = _targetPosition - transform.position;
	}
	
	void KeepTargetOffset()
	{
		transform.position = _targetPosition - _targetOffset;
	}
	
	// The camera distance when exting the auto state
	float _cameraTargetDistance;
	
	// Calculate the vertical angle
	float ComputeVerticalAngle()
	{
		Vector3 groundForwardProjection = transform.forward - Vector3.Project(transform.forward, character.up);
		groundForwardProjection.Normalize();
		
		float x = Vector3.Dot(- transform.forward, - groundForwardProjection); 
		float y = Vector3.Dot(- transform.forward, character.up); 

		return Mathf.Rad2Deg * Mathf.Atan2(y, x);
	}
	
	
	// Smoothly damp the look at between character and the alternative target
	void SmoothDampLookAt(float t)
	{
		Quaternion lookAtRotA = new Quaternion();
		Quaternion lookAtRotB = new Quaternion();
		
		lookAtRotA.SetLookRotation(_targetOffset);
		lookAtRotB.SetLookRotation(alternativeTarget.position
			- transform.position);
		
		transform.rotation = QuaternionHelper.SmoothSlerp(lookAtRotA, lookAtRotB, t);
	}
	
	// Camera states
	public enum States
	{
		// Follow automatically the character
		Auto,
		
		// Camera is moved by the user
		UserInput,
		
		// Camera is fixed in relation to the character
		Fixed,
		
		// Wait for input from the user or go to Follow mode
		WaitForInput,
		
		// Temporarly change the target with a smooth transition
		TargetTransition,
		
		// return to the current target with a smooth transition
		TargetBack
		
	}
	
	// Use this for initialization
	void Start ()
	{
		// Start in the auto state
		currentState = States.Auto;
	}
	
	//---------------------------------- Auto state ----------------------------------------
	public float Auto_maxVerticalAngle;
	public float Auto_minVerticalAngle;
	
	void Auto_Update()
	{	
		UpdateTargetPosition();
		UpdateTargetOffset();
		
		// Looks at the character
		transform.LookAt(_targetPosition);
		
		// Smooth camera distance correction
		float distance = _targetOffset.magnitude;
		
		if (distance > maxDistance)
		{
			transform.position +=
				transform.forward * (distance - maxDistance) * speedFactor * Time.deltaTime;
		}
		else if (distance < minDistance)
		{
			transform.position +=
				transform.forward * (distance - minDistance) * speedFactor * Time.deltaTime;
		}
		
		// Smooth camera vertical angle correction
		float verticalAngle = ComputeVerticalAngle();
		
		if (verticalAngle > Auto_maxVerticalAngle)
		{
			transform.RotateAround(_targetPosition, transform.right,
				(Auto_maxVerticalAngle - verticalAngle) * angularSpeedFactor * Time.deltaTime);
		}
		else if (verticalAngle < Auto_minVerticalAngle)
		{
			transform.RotateAround(_targetPosition, transform.right,
				(Auto_minVerticalAngle - verticalAngle) * angularSpeedFactor * Time.deltaTime);
		}
		
		if (UpdateUserInput())
		{
			currentState = States.UserInput;
		}
	}
	
	
	//------------------------------------- UserInput state ------------------------
	public float UserInput_maxVerticalAngle;
	public float UserInput_minVerticalAngle;
	
	void UserInput_Update()
	{	
		UpdateTargetPosition();
		KeepTargetOffset();
		
		transform.RotateAround(_targetPosition, character.up, _deltaX);
		transform.RotateAround(_targetPosition, transform.right, -_deltaY);
		
		// Camera vertical angle limits
		float verticalAngle = ComputeVerticalAngle();
		
		if (verticalAngle > UserInput_maxVerticalAngle)
		{
			transform.RotateAround(_targetPosition, transform.right,
				(UserInput_maxVerticalAngle - verticalAngle));
		}
		else if (verticalAngle < UserInput_minVerticalAngle)
		{
			transform.RotateAround(_targetPosition, transform.right,
				(UserInput_minVerticalAngle - verticalAngle));
		}
		
		if (UpdateUserInput())
		{
			currentState = States.UserInput;
		}
		
		UpdateTargetOffset();
		
		if (!UpdateUserInput())
		{
			currentState = States.WaitForInput;
		}
	}
	
	
	//------------------------------------- WaitForInput state ----------------------------
	void WaitForInput_EnterState()
	{
		// Target offset should remain the same, so we update it here
		UpdateTargetOffset();
	}
	
	void WaitForInput_Update()
	{
		// Make the camera stay fixed in relation to the target position
		UpdateTargetPosition();
		KeepTargetOffset();
		
		// Wait some amount of time for user input
		if (timeInCurrentState > inputToAutoDelay)
		{
			currentState = States.Auto;
		}
		else if (UpdateUserInput())
		{
			currentState = States.UserInput;
		}
	}
	
	// -------------------------------------- Fixed state ------------------------------------
	void Fixed_EnterState()
	{
		// Target offset should remain the same, so we update it here
		UpdateTargetOffset();
	}
	
	void Fixed_Update()
	{
		// Make the camera stay fixed in relation to the target position
		UpdateTargetPosition();
		KeepTargetOffset();
	}
	
	// -------------------------------------- TargetTransition state -------------------------
	void TargetTransition_EnterState()
	{	
		UpdateTargetOffset();
		if (alternativeTarget == null)
		{
			currentState = States.Auto;
		}
	}
	
	void TargetTransition_Update()
	{
		if (timeInCurrentState < targetTransitionTime)
		{
			SmoothDampLookAt(timeInCurrentState / targetTransitionTime);
		}
	}
	
	
	// ------------------------------------- TargetBack state --------------------------------
	void TargetBack_EnterState()
	{
		UpdateTargetOffset();
		if (alternativeTarget == null)
		{
			currentState = States.Auto;
		}
	}
	
	void TargetBack_Update()
	{
		if (timeInCurrentState < targetTransitionTime)
		{
			SmoothDampLookAt(1.0f - timeInCurrentState / targetTransitionTime);
		}
		else
		{
			currentState = States.Auto;
		}
	}
	
}
