using UnityEngine;
using System.Collections;

public static class QuaternionHelper
{
	public static Quaternion SmoothSlerp(Quaternion from, Quaternion to, float t)
	{
		float smoothT = Mathf.SmoothStep(0.0f, 1.0f, t);
		return Quaternion.Slerp(from, to, smoothT);
	}
}
