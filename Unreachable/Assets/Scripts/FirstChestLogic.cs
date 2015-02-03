using UnityEngine;
using System.Collections;

public class FirstChestLogic : ChestLogic 
{
	// The character icon script
	public IconLogic characterIcon;
	
	// The character script
	public CharacterLogic characterLogic;
	
	// The instructions script
	public InstructionsLogic instructionsLogic;
	
	new void OnEnable()
	{
		base.OnEnable();
		
		// Make the character show a surprise blinking sign up his head
		characterIcon.currentState = IconLogic.States.SurpriseSignBlinking;
		
		// Put the character script in the Idle state
		characterLogic.currentState = CharacterLogic.States.Idle;
		
		// Start the instruction script for movements
		instructionsLogic.StartCoroutine(
			instructionsLogic.ShowInstructions(InstructionsLogic.Instructions.Movement, 5.0f));
	}
}
