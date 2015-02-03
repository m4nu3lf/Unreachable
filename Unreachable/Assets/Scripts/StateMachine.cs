using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class StateMachine : MonoBehaviour
{
	private Enum _currentState;
	
	// Access the current state
	public Enum currentState
	{
		get
		{
			return _currentState;
		}
		set
		{
			_currentState = value;
			_timeEnteredState = Time.time;
			ConfigureCurrentState();
		}
	}
	
	// The time the current state was entered
	public float _timeEnteredState;
	
	// The time elapsed since the machine entered this state
	public float timeInCurrentState
	{
		get
		{
			return Time.time - _timeEnteredState;
		}
	}
	
	// Delegates declaration
	public Action DoUpdate = DoNothing;
	public Action DoLateUpdate = DoNothing;
	public Action DoFixedUpdate = DoNothing;
	public Action<Collider> DoOnTriggerEnter = DoNothingCollider;
	public Action<Collider> DoOnTriggerStay = DoNothingCollider;
	public Action<Collider> DoOnTriggerExit = DoNothingCollider;
	public Action<Collision> DoOnCollisionEnter = DoNothingCollision;
	public Action<Collision> DoOnCollisionStay = DoNothingCollision;
	public Action<Collision> DoOnCollisionExit = DoNothingCollision;
	public Action DoOnMouseEnter = DoNothing;
	public Action DoOnMouseUp = DoNothing;
	public Action DoOnMouseDown = DoNothing;
	public Action DoOnMouseOver = DoNothing;
	public Action DoOnMouseExit = DoNothing;
	public Action DoOnMouseDrag = DoNothing;
	public Action DoOnGUI = DoNothing;
	public Func<IEnumerator> ExitState = DoNothingCoroutine;
	
	
	// DoNothing methods
	static void DoNothing(){}
	static void DoNothingCollider(Collider collider){}
	static void DoNothingCollision(Collision collision){}
	static IEnumerator DoNothingCoroutine(){ yield return null; }
	
	
	// Delegate wrappers
	void Update()
	{
		DoUpdate();
	}
	
	void LateUpdate()
	{
		DoLateUpdate();
	}
	
	void FixedUpdate()
	{
		DoFixedUpdate();
	}
	
	void OnTriggerEnter(Collider collider)
	{
		DoOnTriggerEnter(collider);
	}
	
	void OnTriggerStay(Collider collider)
	{
		DoOnTriggerStay(collider);
	}
	
	void OnTriggerExit(Collider collider)
	{
		DoOnTriggerStay(collider);
	}
	
	void OnCollisionEnter(Collision collision)
	{
		DoOnCollisionEnter(collision);
	}
	
	void OnCollisionStay(Collision collision)
	{
		DoOnCollisionStay(collision);
	}
	
	void OnCollisionExit(Collision collision)
	{
		DoOnCollisionExit(collision);
	}
	
	void OnMouseEnter()
	{
		DoOnMouseEnter();
	}
	
	void OnMouseUp()
	{
		DoOnMouseUp();
	}
	
	void OnMouseDown()
	{
		DoOnMouseUp();
	}
	
    void OnMouseOver()
	{
		DoOnMouseUp();
	}
	
	void OnMouseExit()
	{
		DoOnMouseUp();
	}
	
    void OnMouseDrag()
	{
		DoOnMouseUp();
	}
	
	void OnGUI()
	{
		DoOnGUI();
	}
	
	// Configure the current state
	void ConfigureCurrentState()
	{
		if(ExitState != null)
		{
			StartCoroutine(ExitState());
		}
		
		// Every delegate is configured for the current state
		DoUpdate = ConfigureDelegate<Action>("Update", DoNothing);
		DoLateUpdate = ConfigureDelegate<Action>("LateUpdate", DoNothing);
		DoFixedUpdate = ConfigureDelegate<Action>("FixedUpdate", DoNothing);
	    DoOnTriggerEnter = ConfigureDelegate<Action<Collider>>("OnTriggerEnter", DoNothingCollider);
	    DoOnTriggerStay = ConfigureDelegate<Action<Collider>>("OnTriggerStay", DoNothingCollider);
	    DoOnTriggerExit = ConfigureDelegate<Action<Collider>>("OnTriggerExit", DoNothingCollider);
	    DoOnCollisionEnter = ConfigureDelegate<Action<Collision>>("OnCollisionEnter", DoNothingCollision);
	    DoOnCollisionStay = ConfigureDelegate<Action<Collision>>("OnCollisionStay", DoNothingCollision);
	    DoOnCollisionExit = ConfigureDelegate<Action<Collision>>("OnCollisionExit", DoNothingCollision);
	    DoOnMouseEnter = ConfigureDelegate<Action>("OnMouseEnter", DoNothing);
	    DoOnMouseUp = ConfigureDelegate<Action>("OnMouseUp", DoNothing);
	    DoOnMouseDown = ConfigureDelegate<Action>("OnMouseDown", DoNothing);
	    DoOnMouseOver = ConfigureDelegate<Action>("OnMouseOver", DoNothing);
	    DoOnMouseExit = ConfigureDelegate<Action>("OnMouseExit", DoNothing);
	    DoOnMouseDrag = ConfigureDelegate<Action>("OnMouseDrag", DoNothing);
	    DoOnGUI = ConfigureDelegate<Action>("OnGUI", DoNothing);
		Func<IEnumerator> enterState = ConfigureDelegate<Func<IEnumerator>>("EnterState", DoNothingCoroutine);
	    ExitState = ConfigureDelegate<Func<IEnumerator>>("ExitState", DoNothingCoroutine);
		
		// Enable/Disable the GUI according whether we have an OnGUI method or not
		EnableGUI();
		
		// Start the current state
		StartCoroutine(enterState());
		
	}
	
	// States delegates cache
	Dictionary<object, Dictionary<string, Delegate>> _cache = new Dictionary<object, Dictionary<string, Delegate>>();
	
	T ConfigureDelegate<T>(string methodRoot, T Default) where T : class
	{		
		Dictionary<string, Delegate> lookup;
		
		if (!_cache.TryGetValue(_currentState, out lookup))
		{
			// No lookup in cache for this state, so build new one
			lookup = _cache[_currentState] = new Dictionary<string, Delegate>();
		}
		
		Delegate deleg;
		
		if (!lookup.TryGetValue(methodRoot, out deleg))
		{
			// No delegate in the lookup, so create new one
			var method = GetType().GetMethod(_currentState.ToString() + "_" + methodRoot,
				System.Reflection.BindingFlags.Instance |
				System.Reflection.BindingFlags.Public |
				System.Reflection.BindingFlags.NonPublic |
				System.Reflection.BindingFlags.InvokeMethod);
			if (method != null)
			{
				if (typeof(T) == typeof(Func<IEnumerator>) && method.ReturnType != typeof(IEnumerator))
				{
					// The method is not a coroutine but we need one, so create a wrapper
					Action action = Delegate.CreateDelegate(typeof(Action), this, method) as Action;
					
					// Since returning a null pointer causes an exception to be raised, return a dummy coroutine
					Func<IEnumerator> func = () => {action(); return DoNothingCoroutine(); };
					deleg = func;	
				}
				else
				{
					deleg = Delegate.CreateDelegate(typeof(T), this, method); 
				}
			}
			else
			{
				deleg = Default as Delegate;
			}
			
			// Add the delegate to the lookup
			lookup[methodRoot] = deleg;
		}
		
		return deleg as T;
	}
	
	void EnableGUI()
	{
		useGUILayout = DoOnGUI != DoNothing;					
	}
	
	
	
	// Use this for initialization
	void Start ()
	{
	
	}
	
}
