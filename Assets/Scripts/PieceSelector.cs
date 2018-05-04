using System;
using System.Collections.Generic;
﻿using UnityEngine;

public class PieceSelector : MonoBehaviour {
	public string user;
	public int faction;

	public TurnSystem turnSystem;

	public GameBoard board;
	public GameObject selectedSquare;

	public float rotationSpeed = 100f;
	public float zoomSpeed = 10f;

	public float minDistanceFromCenter = 5f;
	public float maxDistanceFromCenter = 20f;

	public int mouseButtonRotate = 1;
	public int mouseButtonSelect = 0;

	public Color moveSquareColor = Color.green;
	public Color attackSquareColor = Color.yellow;
	public Color wrongSquareColor = Color.red;

	public string[] possiblePaths;

	void Start() {
		GameRules.board = board;
	}

	void SetPaths(bool state) {
		Square selectedSquare = GetSelectedSquare();

		if(selectedSquare != null && selectedSquare.piece != null) {
			GamePiece piece = selectedSquare.piece.GetComponent<GamePiece>();

			string[] movementPaths = GameRules.GetPaths(piece, selectedSquare.name);
			string[] attackPaths = GameRules.GetAttackPaths(piece, selectedSquare.name);

			List<string> paths = new List<string>();

			foreach(string squareName in movementPaths) {
				Square square = board.GetSquare(squareName);

				if(square.piece == null) {
					paths.Add(squareName);
					square.tile.GetComponent<Renderer>().material.color = state ? moveSquareColor : square.tileColor;
				}
			}

			foreach(string squareName in attackPaths) {
				Square square = board.GetSquare(squareName);

				if(square.piece != null && square.piece.GetComponent<GamePiece>().faction != faction) {
					paths.Add(squareName);
					square.tile.GetComponent<Renderer>().material.color = state ? attackSquareColor : square.tileColor;
				}
			}

			possiblePaths = paths.ToArray();
		}
	}

	Square GetSelectedSquare() {
		return selectedSquare != null ? board.GetSquare(selectedSquare.name) : null;
	}

	void OnSquareSelected(RaycastHit hit) {
		GameObject tile;

		if(hit.transform.GetComponent<GamePiece>() != null) {
			tile = board.GetSquare(hit.transform.GetComponent<GamePiece>().GetSquare()).tile;
		} else {
			tile = hit.transform.gameObject;
		}

		GameObject square = tile.transform.parent.gameObject;

		if(GetSelectedSquare() != null && GetSelectedSquare().piece != null && Array.Exists(possiblePaths, name => name == square.name)) {
			board.Move(GetSelectedSquare().name, square.name, GetSelectedSquare().piece.GetComponent<GamePiece>());
			selectedSquare = null;

			turnSystem.NextTurn();
		} else {
			Square actualSquare = board.GetSquare(square.name);

			if(GetSelectedSquare() != null && actualSquare.name == GetSelectedSquare().name) {
				SetPaths(false);
				selectedSquare.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = GetSelectedSquare().tileColor;
				selectedSquare = null;
			} else if(actualSquare.piece != null && actualSquare.piece.GetComponent<GamePiece>().faction == faction) {
				tile.GetComponent<Renderer>().material.color = moveSquareColor;
				selectedSquare = square;
				SetPaths(true);
			} else {
				selectedSquare = null;
			}
		}
	}

	void OnSelectPress() {
		if(turnSystem.IsTurn(user)) {
			if(selectedSquare != null) {
				SetPaths(false);
				selectedSquare.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = GetSelectedSquare().tileColor;
			}

			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				OnSquareSelected(hit);
			} else {
				selectedSquare = null;
			}
		}
	}

	void ToggleRotation(bool state) {
		Cursor.lockState = state ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !state;
	}

	void ExecuteRotation() {
		Vector2 move = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * rotationSpeed * Time.deltaTime;

		transform.RotateAround(board.transform.position, transform.up, move.y);
		transform.RotateAround(board.transform.position, -transform.right, move.x);
	}

	void Update() {
		if(Input.GetMouseButtonDown(mouseButtonSelect)) OnSelectPress();
		if(Input.GetMouseButtonDown(mouseButtonRotate)) ToggleRotation(true);
		if(Input.GetMouseButtonUp(mouseButtonRotate)) ToggleRotation(false);
		if(Input.GetMouseButton(mouseButtonRotate)) ExecuteRotation();

		if(Input.mouseScrollDelta.y != 0) {
			if(Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, board.transform.position) <= minDistanceFromCenter) {
				return;
			} else if(Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, board.transform.position) >= maxDistanceFromCenter) {
				return;
			}

			transform.position = Vector3.MoveTowards(transform.position, board.transform.position, Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime);
		}
	}
}
