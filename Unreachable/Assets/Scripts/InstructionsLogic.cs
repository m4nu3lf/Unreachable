using UnityEngine;
using System.Collections;

public class InstructionsLogic : MonoBehaviour
{
	// The instructions textures
	public Texture2D movementInstructionsTexture;
	public Texture2D cameraInstructionsTexture;
	
	// Time the instruction texture fades in/out
	public float fadeTime;
	
	// The character script
	public CharacterLogic character;
	
	// The camera script
	public CameraLogic cameraLogic;
	
	public enum Instructions
	{
		Movement,
		Camera
	}
	
	// Show instructions
	public IEnumerator ShowInstructions(Instructions instructions, float delay)
	{
		Texture texture = null;
		
		// choose the right texture
		switch (instructions)
		{
		case Instructions.Movement:
			texture = movementInstructionsTexture;
			break;
			
		case Instructions.Camera:
			texture = cameraInstructionsTexture;
			break;
		}
		
		// Wait for the delay amount
		while (delay > 0 && WaitingForUser(instructions))
		{
			delay -= Time.deltaTime;
			yield return null;
		}
		
		// Texture fade in
		IEnumerator fadeEnum = FadeInTexture(texture, fadeTime);
		while(fadeEnum.MoveNext() && WaitingForUser(instructions))
		{
			yield return null;
		}
		
		// Wait till the user gets the hint
		while(WaitingForUser(instructions))
		{
			yield return null;
			Debug.Log(guiTexture.pixelInset);
		}
		
		// Texture fede out
		yield return StartCoroutine(FadeOutTexture(fadeTime));
	}
	
	// Check if the user did't get the hint
	bool WaitingForUser(Instructions instructions)
	{
		switch (instructions)
		{
		case Instructions.Movement:
			return character.currentState.Equals(CharacterLogic.States.Idle);
			
		case Instructions.Camera:
			return cameraLogic.currentState.Equals(CameraLogic.States.Auto);
			
		default:
			return false;
		}
	}
	
	// Fade out and remove the current texture
	IEnumerator FadeOutTexture(float time)
	{
		while(true)
		{
			Color color = guiTexture.color;
			color.a -= Time.deltaTime / time;
			guiTexture.color = color;
			
			if (guiTexture.color.a <= 0.0f)
			{
				guiTexture.texture = null;
				yield break;
			}
			else
			{
				yield return null;
			}
		}
	}
	
	
	// Assing and fade in a texture
	IEnumerator FadeInTexture(Texture texture, float time)
	{
		guiTexture.texture = texture;
		
		Color color = guiTexture.color;
		color.a = 0.0f;
		guiTexture.color = color;
		
		while(true)
		{
			color = guiTexture.color;
			color.a += Time.deltaTime / time;
			guiTexture.color = color;
			
			if (guiTexture.color.a >= 1.0f)
			{
				yield break;
			}
			else
			{
				yield return null;
			}
		}
	}
	
}
