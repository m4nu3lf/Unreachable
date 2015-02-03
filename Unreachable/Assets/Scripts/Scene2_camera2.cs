using UnityEngine;
using System.Collections;

public class Scene2_camera2 : StateMachine
{
	// Time in the Waiting state
	public float waitTime;
	
	// Time in the Movement state
	public float movementTime;
	
	// The velocity of the camera
	public float movingVelocity;
	
	// The zooming speed factor
	public float zoomingSpeedFactor;

	// Use this for initialization
	void Start ()
	{
		currentState = States.Waiting;
	}
	
	public enum States
	{
		Waiting,
		Moving,
		Stop
	}
	
	// ------------------------- Still state ---------------------
	IEnumerator Waiting_EnterState()
	{
		yield return new WaitForSeconds(waitTime);
		
		currentState = States.Moving;
	}
	
	
	// -------------------------- Moving state -------------------
	void Moving_Update()
	{
		if (timeInCurrentState > movementTime)
		{
			currentState = States.Stop;
		}
		else
		{
			transform.position += transform.forward * movingVelocity * Time.deltaTime;
			camera.fieldOfView -= zoomingSpeedFactor * Time.deltaTime;
			camera.fieldOfView = Mathf.Max(10, camera.fieldOfView);
		}
	}
	
	// ------------------------- Stop state -----------------------
	void Stop_EnterState()
	{
		// Show the end game menu
		GetComponent<EndLevelMenu>().enabled = true;
	}
}
