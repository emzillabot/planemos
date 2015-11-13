﻿using UnityEngine;
using System.Collections;

public class Perturber : MonoBehaviour {

	public MapContstraints mapConstraints;

	private Vector3 eulerAngleVelocity;
	private float speed;
	private Vector3 dir;
	private ObjectRangeOfMotion motionField;
	private Rigidbody rb;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		motionField = mapConstraints.objectMotionField;
		eulerAngleVelocity = new Vector3(
			Random.Range (-200, 200), 
			Random.Range (-200, 200), 
			Random.Range (-200, 200)
		);

		float x = Random.Range (-1.0f, 1.0f);
		float y = Random.Range (-1.0f, 1.0f);
		float z = Random.Range (-1.0f, 1.0f);

		switch (motionField) {
		case ObjectRangeOfMotion.PLANAR:
			dir = new Vector3(x, 0, z);
			break;
		case ObjectRangeOfMotion.FULL_3D:
			dir = new Vector3(x, y, z);
			break;
		}

		speed = Random.Range ( -20, 20 );

		rb.AddForce(dir.normalized * speed, ForceMode.Impulse);
		rb.AddRelativeTorque (eulerAngleVelocity, ForceMode.Impulse);
	
	}
}