
using UnityEngine;

public class PointInTime {

	public Vector3 position;
	public Quaternion rotation;
	public Vector2 Velocity;
	public float angularVelocity;

	public PointInTime (Transform t, Vector2 v, float av)
	{
		position = t.position;
		rotation = t.rotation;
		Velocity = v;
		angularVelocity = av;
	}

}
