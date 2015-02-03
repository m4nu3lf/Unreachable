using UnityEngine;
using System.Collections;

public class LastChestLogic : ChestLogic
{
	// The camera script
	public CameraLogic cameraLogic;
	
	// The terrain
	public GameObject terrain;
	
	// The character icon script
	public IconLogic characterIcon;
	
	// The character script
	public CharacterLogic characterLogic;
	
	// The time between the chest is triggerd and the new scene is loaded
	public float newSceneDelay;
	
	new void OnEnable()
	{
		base.OnEnable();
		
		// Make the camera look back to the main character
		cameraLogic.currentState = CameraLogic.States.TargetBack;
		
		// Reenable the main character input
		characterLogic.inputEnabled = true;
		
		// Make the character show the drop 
		characterIcon.currentState = IconLogic.States.DropSign;
		
		// Don't spawn any object beacuse this object will be disabled
		// when unloading the level
		spawnTurnOffClipPlayer = false;
	}
	
	IEnumerator OnTriggerEnter(Collider other)
	{
		if (other.gameObject == character)
		{
			// Disable the terrain
			terrain.SetActive(false);
			
			// Make the character fall
			characterLogic.currentState = CharacterLogic.States.Falling;
			
			// Keep the camera fixed
			cameraLogic.currentState = CameraLogic.States.Fixed;
			
			// Wait for some time
			yield return new WaitForSeconds(newSceneDelay);
			
			// To the next scene
			Application.LoadLevel("Scene2");
		}
	}
}
