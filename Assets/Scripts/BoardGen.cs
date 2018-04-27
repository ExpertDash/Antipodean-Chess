using System.Collections;
﻿using UnityEngine;

public class BoardGen : MonoBehaviour {
	[RangeAttribute(0, Mathf.PI * 2)]
	public float yStep, pStep;
	public float radius;
	public float delay;

	void Start() {
		StartCoroutine(SlowGenerate());
	}

	IEnumerator SlowGenerate() {

		for(float yaw = 0; yaw < 11f * Mathf.PI / 6f; yaw += yStep) {
			for(float pitch = Mathf.PI / 6; pitch < Mathf.PI; pitch += pStep) {
				GenerateFace(transform.localPosition, GetCoords(yaw, pitch));
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

		Debug.Log(position);

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
