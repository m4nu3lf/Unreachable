using UnityEngine;
using System.Collections;

public class EndLevelMenu : StateMachine
{	
	// Time for the game to auto restart
	public float restartDelay;
	
	// Buttons parameters
	public float buttonHeight;
	public float buttonWidth;
	public float buttonBorder;
	public Vector2 backButtonNormalizedPos;
	
	// Holds the screen center coordinate
	Vector2 _screenCenter;
	
	// Credits game object and text
	public GameObject credits;
	public TextAsset creditsText;
	
	// States
	public enum States
	{
		Main,
		Credits
	}
	
	// Get a rect in a grid
	Rect GetGridButtonRect(int i, int j, int rows, int columns)
	{
		float buttonFullWidth = buttonWidth + buttonBorder * 2;
		float buttonFullHeight = buttonHeight + buttonBorder * 2;
		float gridLeft = _screenCenter.x - buttonFullWidth * columns / 2;
		float gridTop = _screenCenter.y - buttonFullHeight * rows / 2;
		float buttonLeft = gridLeft + buttonFullWidth * j + buttonBorder;
		float buttonTop = gridTop + buttonFullHeight * i + buttonBorder;
		
		return new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight);
	}
	
	// Get the rect of the back button
	Rect GetBackButtonRect()
	{
		Vector2 backButtonPos = backButtonNormalizedPos;
		backButtonPos.x *= Screen.width;
		backButtonPos.y *= Screen.height;
		backButtonPos.x -= (buttonWidth + buttonBorder) / 2.0f;
		backButtonPos.y -= (buttonHeight + buttonBorder) / 2.0f;
		return new Rect(backButtonPos.x, backButtonPos.y, buttonWidth, buttonHeight);
	}
	
	// Use this for initialization
	void Start ()
	{		
		// Start from the main menu state
		currentState = States.Main;
		
		_screenCenter = new Vector2(Screen.width, Screen.height);
		_screenCenter /= 2;
		
	}
	
	// ------------------------------- Main State --------------------------------
	void Main_OnGUI()
	{
		Rect againButtonRect = GetGridButtonRect(0, 0, 3, 1); 
		Rect quitButtonRect = GetGridButtonRect(1, 0, 3, 1); ;
		Rect creditsButtonRect = GetGridButtonRect(2, 0, 3, 1); ;
		
		if (timeInCurrentState > restartDelay || GUI.Button(againButtonRect, "Again"))
		{
			// Ensure the character is destroyed before loading the first level
			GameObject.Destroy(GameObject.Find("character"));
			
			// Restart the game
			Application.LoadLevel(0);
		}
		
		if (GUI.Button(creditsButtonRect, "Credits"))
		{
			// Show credits
			currentState = States.Credits;
		}
		
		if (GUI.Button(quitButtonRect, "Quit"))
		{
			// Quit the game
			Application.Quit();
		}
	}
	
	
	// ------------------------------- Credits State --------------------------------
	void Credits_OnGUI()
	{	
		credits.SetActive(true);
		credits.guiText.text = creditsText.ToString();
			
		
		if (GUI.Button(GetBackButtonRect(), "Back"))
		{
			// Back to main menu
			currentState = States.Main;
		}
	}
	
	void Credits_ExitState()
	{
		credits.SetActive(false);
	}
	
}
