using UnityEngine;
using System;
using System.Collections;

public class BoardAnimation {
	private static bool active = false;
	private static Action _onComplete = null;
	private static MonoBehaviour animationTarget;

	public static bool Active() {
		return active;
	}

	/// <summary>Sets an action to be executed after the next animation is completed</summary>
	/// <value>Action to be executed</value>
	public static Action onComplete { set { _onComplete = value; } }

	private static void Complete() {
		if(_onComplete != null) _onComplete();
		_onComplete = null;
		active = false;
	}

	/// <summary>Sets the script to which animation coroutines are bound</summary>
	/// <param name="target">Script to bind animations to</param>
	public static void SetAnimationTarget(MonoBehaviour target) {
		animationTarget = target;
	}

	public static void Move(Piece piece, Vector3 initialPos, Quaternion initialRot) {
		animationTarget.StartCoroutine(AnimateMove(piece, initialPos, initialRot));
	}

	private static IEnumerator AnimateMove(Piece piece, Vector3 initialPos, Quaternion initialRot) {
		float speed = 10f, threshold = 0.05f;

		//Make the piece invisible by giving alpha 0
		piece.color *= 0f;

		//Create a sphere for rotating the display piece
		GameObject sphere = new GameObject("MoveAnimation");
		sphere.transform.position = GameManager.board.transform.position;

		//Get the initaial and final rotations for the sphere
		Quaternion initial = Quaternion.LookRotation(initialPos - sphere.transform.position),
				   final = Quaternion.LookRotation(piece.transform.position - sphere.transform.position);

		//Set the sphere to the initial rotation
		sphere.transform.rotation = initial;

		//Place a display piece on the sphere with the position, rotation, and color of the piece before it moved
		GameObject display = GameObject.Instantiate(piece.gameObject);
		GameObject.Destroy(display.GetComponent<MeshCollider>());
		display.transform.SetParent(sphere.transform);
		display.transform.position = initialPos;
		display.transform.rotation = initialRot;
		display.transform.localScale = piece.transform.lossyScale;
		display.GetComponent<Piece>().color = piece.color;

		//Rotate the sphere from initial rotation to final rotation until the difference between current and final angle is less than or equal to 1
		while(Quaternion.Angle(sphere.transform.rotation = Quaternion.Slerp(sphere.transform.rotation, final, Time.deltaTime * speed), final) > threshold) {
			yield return null;
		}

		//Make the original piece visible again and destroy the displayed piece and sphere
		piece.color = piece.color;
		GameObject.Destroy(display);
		GameObject.Destroy(sphere);
	}

	/// <summary>Animates the pieces of a tile being pushed/pulled</summary>
	/// <param name="location">Location of the tile</param>
	public static void PushOrPull(string location) {
		//Push or pull animation probably to be discarded over swap
		Swap(location);

		/*
		active = true;
		animationTarget.StartCoroutine(AnimatePOP(location, 2.75f));
		*/
	}

	private static IEnumerator AnimatePOP(string location, float speed = 1f) {
		Tile tile = GameManager.board.GetTile(location);

		Piece piece = tile.GetPiece(),
			  inversePiece = tile.GetInversePiece();

		//Get original scale
		Vector3 scale = piece ? piece.transform.localScale : inversePiece ? inversePiece.transform.localScale : Vector3.one;

		float start = 1f, end = -1f, i = 0;
		while(i < 1f) {
			i += Time.deltaTime * speed;

			//Linearly interpolate scale from positive to negative value
		 	Vector3 s = new Vector3(scale.x, scale.y * Mathf.Lerp(start, end, i), scale.z);
			if(piece) piece.transform.localScale = s;
			if(inversePiece) inversePiece.transform.localScale = s;

			yield return null;
		}

		//Reset scale
		if(piece) piece.transform.localScale = scale;
		if(inversePiece) inversePiece.transform.localScale = scale;

		Complete();
	}

	/// <summary>Animates a piece bing captured</summary>
	/// <param name="piece">Piece to animate capture for</param>
	public static void Capture(Piece piece) {
		active = true;
		animationTarget.StartCoroutine(AnimateCapture(piece));
	}

	private static IEnumerator AnimateCapture(Piece piece) {
		float speed = 0.5f;
		float dist = 0f;
		float dest = 2f;

		Color c = piece.color;

		while(c.a > 0) {
			//Increase y-position, increase distance, and decrease speed
			piece.gameObject.transform.position += piece.gameObject.transform.up * speed;
			dist += speed;
			speed *= 0.85f;

			//Set alpha
			piece.color = (c = new Color(c.r, c.g, c.b, (dest - dist) / dest));

			yield return null;
		}

		//Reset color
		piece.color = piece.color;

		Complete();
	}

	/// <summary>Animates a tile swapping</summary>
	/// <param name="location">Location of the tile</param>
	public static void Swap(string location) {
		active = true;
		animationTarget.StartCoroutine(AnimateSwap(10f, location));
	}

	private static IEnumerator AnimateSwap(float speed, string location) {
		Tile tile = GameManager.board.GetTile(location);
		Quaternion rot = tile.transform.localRotation;

		//Rotate around y-axis (spin)
		 for(float i = 0f; i < 180f / speed; i++) {
			tile.transform.Rotate(0, speed, 0);
			yield return null;
		}

		//Rotate around x-axis (flip)
		for(float i = 0f; i < 180f / speed; i++) {
			tile.transform.Rotate(speed, 0f, 0f);
			yield return null;
		}

		//Reset rotation
		tile.transform.localRotation = rot;

		Complete();
	}

	/// <summary>Animates the board inverting</summary>
	public static void Inversion() {
		active = true;
		animationTarget.StartCoroutine((BoardAnimation.AnimateInversion(1f, 10f)));
	}

	private static IEnumerator AnimateInversion(float rowDelay, float speed) {
		animationTarget.StartCoroutine(AnimateColorChange(1f));

		//Spin all tiles simultaneously
		for(float i = 0f; i < 180f / speed; i++) {
			foreach(Tile tile in GameManager.board.GetTiles()) {
				tile.transform.Rotate(0, speed, 0);
			}

			yield return null;
		}

		//Flip alpha tile and wait
		animationTarget.StartCoroutine(AnimateInversionTile(speed, "Alpha", false));
		yield return new WaitForSeconds(rowDelay / speed);

		//Flip other tiles and wait
		for(int row = BoardConfig.initialRow; row <= BoardConfig.finalRow; row++) {
			animationTarget.StartCoroutine(AnimateInversionRow(speed, row));
			yield return new WaitForSeconds(rowDelay / speed);
		}

		//Flip omega tile
		animationTarget.StartCoroutine(AnimateInversionTile(speed, "Omega", true));
	}

	private static IEnumerator AnimateColorChange(float duration) {
		Color initial = GameManager.board.isInverted ? BoardConfig.visuals.normalSkyboxColor : BoardConfig.visuals.inverseSkyboxColor,
			  final = GameManager.board.isInverted ? BoardConfig.visuals.inverseSkyboxColor : BoardConfig.visuals.normalSkyboxColor,
			  c = initial;

		//Color deltas
		float dR = (final.r - initial.r) / duration,
			  dG = (final.g - initial.g) / duration,
			  dB = (final.b - initial.b) / duration;

		//Ensure skybox starts with initial color
		Camera.main.backgroundColor = initial;

		//Interpolate between colors over time
		for(float i = 0; i < duration; i += Time.deltaTime) {
			c = new Color(c.r + dR * Time.deltaTime, c.g + dG * Time.deltaTime, c.b + dB * Time.deltaTime);
			Camera.main.backgroundColor = c;

			yield return null;
		}

		//Ensure skybox ends with final color
		Camera.main.backgroundColor = final;
	}

	private static IEnumerator AnimateInversionTile(float speed, string location, bool end) {
		//Flip
		for(float i = 0f; i < 180f / speed; i++) {
			GameManager.board.GetTile(location).transform.Rotate(speed, 0f, 0f);
			yield return null;
		}

		//Finish animation (should be Omega location)
		if(end) Complete();
	}

	private static IEnumerator AnimateInversionRow(float speed, int row) {
		//Flip all tiles in row
		for(float i = 0f; i < 180f / speed; i++) {
			for(char column = BoardConfig.initialColumn; column <= BoardConfig.finalColumn; column++) {
				GameManager.board.GetTile("" + column + row).transform.Rotate(speed, 0f, 0f);
			}

			yield return null;
		}
	}
}