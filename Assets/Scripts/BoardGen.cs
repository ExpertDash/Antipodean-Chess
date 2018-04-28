using System.Collections;
﻿using UnityEngine;
using UnityEngine.Events;

public class BoardGen : MonoBehaviour {
	public int yawSteps = 8, pitchSteps = 8;
	public float radius = 5;
	public float delay = 0.05f;
	public float thinness = 0.01f;

	public bool useProceduralGeneration = false;

	[HideInInspector]
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

		GenerateSquare("Alpha", 0, GetCoords(0, 0));

		if(useDelay) {
			yield return new WaitForSeconds(delay);
		}

		bool color = false;

		for(int pStep = pStart; pStep < (pitchSteps + 1); pStep++, color = !color) {
			for(int yStep = yStart; yStep < yawSteps; yStep++, color = !color) {
				GenerateSquare("" + (char)(65 + yStep) + (pStep), color ? 1 : 2, GetCoords(yStep * yDelta, pStep * pDelta));

				if(useDelay) {
					yield return new WaitForSeconds(delay);
				}
			}
		}

		GenerateSquare("Omega", 0, GetCoords(0, Mathf.PI));

		genCompleteEvent.Invoke();
	}


	GameObject GenerateSquare(string spatialPosition, int color, Vector3 position) {
		GameObject square = new GameObject(spatialPosition);
		square.transform.parent = transform;
		square.transform.localPosition = position;
		square.transform.localRotation = Quaternion.LookRotation(position - transform.localPosition);
		square.transform.Rotate(90f, 0f, 0f);

		GameObject face = GameObject.CreatePrimitive(PrimitiveType.Cube);
		face.transform.parent = square.transform;
		face.transform.localPosition = Vector3.zero;
		face.transform.localRotation = Quaternion.identity;
		face.transform.localScale = CalculateScale(square.transform.localPosition);

		Color col = Color.blue;

		switch(color) {
			case 0:
				col = Color.grey;
				break;
			case 1:
				col = Color.white;
				break;
			case 2:
				col = Color.black;
				break;
		}

		face.GetComponent<Renderer>().material.color = col;

		return square;
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

		return new Vector3(scale, thinness, scale);
	}
}
