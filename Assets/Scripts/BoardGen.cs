using System.Collections;
﻿using UnityEngine;
using UnityEngine.Events;

public class BoardGen : MonoBehaviour {
	public int yawSteps = 8, pitchSteps = 8;
	public float radius = 5;
	public float delay = 0.05f;
	public float thinness = 0.001f;

	public bool useProceduralGeneration = false;

	public UnityEvent genCompleteEvent;

	void Start() {
		StartCoroutine(Generate(useProceduralGeneration));
	}

	IEnumerator Generate(bool useDelay) {
		int yStart = 0, pStart = 1;
		float yAngle = 2f * Mathf.PI, yDelta = yAngle / yawSteps;
		float pAngle = Mathf.PI, pDelta = pAngle / (pitchSteps + 1);

		if(useDelay) {
			yield return new WaitForSeconds(delay);
		}

		GenerateFace("Alpha", GetCoords(0, 0));

		if(useDelay) {
			yield return new WaitForSeconds(delay);
		}

		for(int pStep = pStart; pStep < (pitchSteps + 1); pStep++) {
			for(int yStep = yStart; yStep < yawSteps; yStep++) {
				GenerateFace("" + (char)(65 + yStep) + (pStep), GetCoords(yStep * yDelta, pStep * pDelta));

				if(useDelay) {
					yield return new WaitForSeconds(delay);
				}
			}
		}

		GenerateFace("Omega", GetCoords(0, Mathf.PI));

		genCompleteEvent.Invoke();
	}


	GameObject GenerateFace(string spacialPosition, Vector3 position) {
		GameObject face = GameObject.CreatePrimitive(PrimitiveType.Cube);
		face.name = spacialPosition;

		face.transform.parent = transform;
		face.transform.localPosition = position;
		face.transform.localRotation = Quaternion.LookRotation(position - transform.localPosition);
		face.transform.localScale = CalculateScale(position);

		return face;
	}

	Vector3 GetCoords(float yaw, float pitch) {
		float x, y, z;

		x = radius * Mathf.Cos(yaw) * Mathf.Sin(pitch);
		z = radius * Mathf.Sin(yaw) * Mathf.Sin(pitch);
		y = radius * Mathf.Cos(pitch);

		return new Vector3(x, y, z);
	}

	Vector3 CalculateScale(Vector3 position) {
		float scale = 1;
		float baseline = 6f;

		float distance = radius - Mathf.Abs(position.y);

		if(distance != 0) {
			float distanceUnit = distance / radius * baseline;

			scale = Mathf.Log(4 + distanceUnit + Mathf.Pow(distanceUnit, 2f), 10f);
		}

		return new Vector3(scale, scale, thinness);
	}
}
