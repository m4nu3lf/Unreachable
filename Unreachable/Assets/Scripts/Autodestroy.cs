using UnityEngine;
using System.Collections;

public class Autodestroy : MonoBehaviour
{
	
	public float delay;
	float _livingTime;
	
	// Use this for initialization
	void Start ()
	{
		_livingTime = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		_livingTime += Time.deltaTime;
		if (_livingTime > delay)
		{
			Destroy(gameObject);
		}
	}
	
	void OnDisable()
	{
		Destroy(gameObject);
	}
}
