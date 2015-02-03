using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameObjectSwitch : MonoBehaviour
{
	[System.Serializable]
	public class ObjectInfo
	{
		public float enableDelay;
		public float disableDelay;
		public GameObject obj;
		
		public IEnumerator delayedSetActive(bool active)
		{
			if(active)
			{
				yield return new WaitForSeconds(enableDelay);
			}
			else
			{
				yield return new WaitForSeconds(disableDelay);
			}
			
			obj.SetActive(active);
		}
	}
	
	void Start()
	{
		// Starts from index zero
		activeIndex = 0;
	}
	
	// List of object to switch from
	public List<ObjectInfo> objects = new List<ObjectInfo>();
		
	// The current active object
	int _activeIndex;
	public int activeIndex
	{
		get
		{
			return _activeIndex;
		}
		
		set
		{
			StartCoroutine(objects[_activeIndex].delayedSetActive(false));
			_activeIndex = value;
			StartCoroutine(objects[_activeIndex].delayedSetActive(true));
		}
	}
				

}
