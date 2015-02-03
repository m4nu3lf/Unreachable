using UnityEngine;
using System.Collections;

public class ChestLogic : MonoBehaviour
{
	// The character object reference
	public GameObject character;
	
	// The sound of the switching light
	public AudioClip spotlightSwitchingClip;
	
	// The game object playing the turn off sound when this object is disabled
	public GameObject spotlightTurnOffClipPlayer;
	
	// The object switch
	GameObjectSwitch objectSwitch;
	
	// True if the application is quitting
	protected bool spawnTurnOffClipPlayer = true;
	
	// Use this for initialization
	protected void Start ()
	{
		// Caches the chest manager
		objectSwitch = GameObject.Find("Object Switch").GetComponent<GameObjectSwitch>();
		
		character = GameObject.Find("character");
	}
	
	public void OnEnable()
	{
		// Spotlight sound
		audio.PlayOneShot(spotlightSwitchingClip);
	}
	
	public void OnDisable()
	{
		if (spawnTurnOffClipPlayer)
		{
			// Spotlight sound cannot be played by this object because has been disabled,
			// so we intantiate an object in the same position that play the sound and
			// then destroys itself.
			GameObject obj = Instantiate(spotlightTurnOffClipPlayer) as GameObject;
			obj.transform.position = transform.position;
			obj.audio.PlayOneShot(spotlightSwitchingClip);
		}
	}
	
	public void OnApplicationQuit()
	{
		spawnTurnOffClipPlayer = false;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject == character)
		{
			objectSwitch.activeIndex ++;
		}
	}
}
