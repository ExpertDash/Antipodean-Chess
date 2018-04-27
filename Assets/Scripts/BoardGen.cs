using System.Collections;
﻿using UnityEngine;

public class BoardGen : MonoBehaviour {
	[RangeAttribute(1, 20)]
	public int yawSteps, pitchSteps;
	public float radius;
	public float delay;

	void Start() {
		StartCoroutine(SlowGenerate());
	}

	IEnumerator SlowGenerate() {
		int yStart = 0, pStart = 1;
		float yAngle = 2f * Mathf.PI, yDelta = yAngle / yawSteps;
		float pAngle = Mathf.PI, pDelta = pAngle / (pitchSteps + 1);

		for(int yStep = yStart; yStep < yawSteps; yStep++) {
			for(int pStep = pStart; pStep < (pitchSteps + 1); pStep++) {
				GenerateFace(transform.localPosition, GetCoords(yStep * yDelta, pStep * pDelta));
				yield return new WaitForSeconds(delay);
			}
		}

		yield return new WaitForSeconds(delay * 5);

		for(int i = 0; i < transform.childCount; i++) {
			Destroy(transform.GetChild(i).gameObject);
		}

		StartCoroutine(SlowGenerate());
	}


	GameObject GenerateFace(Vector3 center, Vector3 position) {
		GameObject face = GameObject.CreatePrimitive(PrimitiveType.Cube);
		face.name = "Face";

		face.transform.parent = transform;
		face.transform.localPosition = position;
		face.transform.localRotation = Quaternion.LookRotation(position - center);
		face.transform.localScale = new Vector3(1, 1, 0.001f);

		return face;
	}

	Vector3 GetCoords(float yaw, float pitch) {
		float x, y, z;

		x = radius * Mathf.Cos(yaw) * Mathf.Sin(pitch);
		z = radius * Mathf.Sin(yaw) * Mathf.Sin(pitch);
		y = radius * Mathf.Cos(pitch);

		return new Vector3(x, y, z);
	}
}
