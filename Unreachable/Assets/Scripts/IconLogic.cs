using UnityEngine;
using System.Collections;

public class IconLogic : StateMachine
{
	// Surprise icon texture
	public Texture surpriseSignTexture;
	
	// Surprise icon texture
	public Texture dropSignTexture;
	
	// The head position
	public Transform head;
	
	// The height offset from the head of the surprise sign
	public Vector3 surpriseSignHeadOffset;
	
	// The offset form the head of the drop sign
	public Vector3 dropSignHeadOffset;
	
	// The height the drop falls from relative to the dropSignHeadOffset
	public float dropSignFallingOffset;
	
	// The duration of the drop sign
	public float dropSignDelay;
	
	// Icon state
	public enum States
	{
		Hidden,
		SurpriseSignBlinking,
		DropSign,
	}

	// Use this for initialization
	void Start ()
	{
		currentState = States.Hidden;
	}
	
	// ----------------------------------- Hidden state ---------------------------------
	void Hidden_EnterState()
	{
		renderer.enabled = false;
	}
	
	// ----------------------------------- SurpriseSign state ---------------------------
	void SurpriseSign_SetPosition()
	{
		transform.position = head.position;
		transform.Translate(surpriseSignHeadOffset, Space.World); 
	}
	
	public IEnumerator SurpriseSignBlinking_EnterState()
	{
		// Select the surprise icon texture
		gameObject.renderer.material.mainTexture = surpriseSignTexture;
		
		// Set the sign position
		SurpriseSign_SetPosition();
		
		renderer.enabled = true;
		
		yield return new WaitForSeconds(0.1f);
		
		renderer.enabled = false;
		
		yield return new WaitForSeconds(0.1f);
		
		renderer.enabled = true;
		
		yield return new WaitForSeconds(0.1f);
		
		renderer.enabled = false;
		
		currentState = States.Hidden;
	}
	
	void SurpriseSignBlinking_Update()
	{
		// Update the sign position
		SurpriseSign_SetPosition();
	}
	
	// -------------------------------------- DropSign state ------------------------------
	Vector3 dropSign_offset;
	
	void DropSign_SetPosition()
	{
		transform.position = head.position;
		transform.Translate(dropSign_offset, Space.World); 
	}
	
	public IEnumerator DropSign_EnterState()
	{
		// Select the drop texture
		gameObject.renderer.material.mainTexture = dropSignTexture;
		
		// Set the sign position
		DropSign_SetPosition();
		
		dropSign_offset = dropSignHeadOffset;
		dropSign_offset.y += dropSignFallingOffset;
		
		renderer.enabled = true;
		
		yield return new WaitForSeconds(dropSignDelay);
		
		currentState = States.Hidden;
	}
	
	void DropSign_Update()
	{
		// Update the sign position
		dropSign_offset.y -= (dropSignFallingOffset / dropSignDelay) * Time.deltaTime;
		DropSign_SetPosition();
	}
}
