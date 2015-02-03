using UnityEngine;
using System.Collections;

public class SecondChestLogic : ChestLogic
{
	// The instructions script
	public InstructionsLogic instructionsLogic;
	
	new void OnEnable()
	{
		base.OnEnable();
		
		// Show the camera instructions
		instructionsLogic.StartCoroutine(
			instructionsLogic.ShowInstructions(InstructionsLogic.Instructions.Camera, 5.0f));
	}
}
