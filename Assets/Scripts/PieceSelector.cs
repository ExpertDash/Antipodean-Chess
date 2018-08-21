using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PieceSelector : MonoBehaviour {
	public TurnSystem turnSystem;
	public GameBoard board;

	public bool singleplayer;
	public int faction;
	//public bool inCheckmate;
	public GameObject selectedSquare;

	public float rotationSpeed = 100f;
	public float zoomSpeed = 10f;

	public float minDistanceFromCenter = 5f;
	public float maxDistanceFromCenter = 20f;

	public int mouseButtonRotate = 1, mouseButtonSelect = 0, mouseButtonReturn = 2;

	public KeyCode flipButton = KeyCode.F, toggleNamesButton = KeyCode.N, showScoreButton = KeyCode.S, restartButton = KeyCode.B;

	public Color moveSquareColor = Color.green, attackSquareColor = Color.yellow, checkSquareColor = Color.red;

	public string[] possiblePaths;

	private GameObject heldPiece;
	private float heldDistance;
	private Vector3 lastMouse;

	void Start() {
		GameRules.board = board;
	}

	void SetPaths(bool state) {
		Square selectedSquare = GetSelectedSquare();

		if(selectedSquare != null && selectedSquare.GetSurfacePiece(board) != null) {
			GamePiece piece = selectedSquare.GetSurfacePiece(board).GetComponent<GamePiece>();

			string[] movementPaths = GameRules.GetPaths(piece, selectedSquare.name);
			string[] attackPaths = GameRules.GetAttackPaths(piece, selectedSquare.name);

			List<string> paths = new List<string>();

			foreach(string squareName in movementPaths) {
				Square square = board.GetSquare(squareName);

				if(square.GetSurfacePiece(board) == null) {
					paths.Add(squareName);
					square.tile.GetComponent<Renderer>().material.color = state ? moveSquareColor : square.tileColor;
				}
			}

			foreach(string squareName in attackPaths) {
				Square square = board.GetSquare(squareName);

				if(square.GetSurfacePiece(board) != null && square.GetSurfacePiece(board).GetComponent<GamePiece>().faction != faction) {
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

	///<summary>Determine if the given faction is in checkmate</summary>
	bool DetermineCheckmate(int faction) {
		string king = board.GetSquares(GamePiece.Type.KING, faction)[0];

		//Debug.Log("King of faction " + faction + " on " + king);
		
		foreach(Square square in board.GetSquares()) {
			square.flipside = king[0] == '-';
			GameObject piece = square.localPiece;

			if(piece && piece.GetComponent<GamePiece>().faction != faction && GameRules.GetAttackPaths(piece.GetComponent<GamePiece>(), square.localName).Any(s => (square.flipside ? "-" : "") + s == king)) {
				board.GetSquare(king).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = checkSquareColor;
				//turnSystem.GetPlayer(faction).inCheckmate = true;

				return true;
			}
		}

		board.GetSquare(king).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = board.GetSquare(king).tileColor;
		return false;
	}

	//determine checkmate, pawn trade-in when reaching omni-square, online game
	void UseTurn(string initial, string final, GamePiece piece = null) {
		GamePiece captured = null;
		if(piece) captured = board.Move(initial, final, piece);

		for(int i = 0; i < turnSystem.factions; i++) if(i != faction) DetermineCheckmate(i);
		selectedSquare = null;

		if(DetermineCheckmate(faction)) {
			Debug.Log("Invalid move");

			board.GetSquare(final).transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = checkSquareColor;
			
			if(piece) {
				board.Move("", initial, piece);
				board.jail.Remove(captured);
				if(captured) board.Move("", final, captured);
			} else {
				while(!board.finishedFlipAnimation) board.Swap(GetSelectedSquare().name);
			}
		} else {
			turnSystem.NextTurn();
		}
	}

	void OnSquareSelected(RaycastHit hit) {
		GameObject tile;

		if(hit.transform.GetComponent<GamePiece>() != null) {
			tile = board.GetSquare(hit.transform.GetComponent<GamePiece>().GetSquare()).tile;
		} else {
			tile = hit.transform.gameObject;
		}

		GameObject square = tile.transform.parent.gameObject;

		if(GetSelectedSquare() != null && GetSelectedSquare().GetSurfacePiece(board) != null && Array.Exists(possiblePaths, name => name == square.name)) {
			UseTurn((board.isFlipside ? "-" : "") + GetSelectedSquare().name, (board.isFlipside ? "-" : "") + square.name, GetSelectedSquare().GetSurfacePiece(board).GetComponent<GamePiece>());
		} else {
			Square actualSquare = board.GetSquare(square.name);

			if(GetSelectedSquare() != null && actualSquare.name == GetSelectedSquare().name) {
				SetPaths(false);
				selectedSquare.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = GetSelectedSquare().tileColor;
				selectedSquare = null;
			} else if(actualSquare.HasPiece() && actualSquare.GetFavorablePiece(board).GetComponent<GamePiece>().faction == faction) {
				tile.GetComponent<Renderer>().material.color = moveSquareColor;
				selectedSquare = square;
				SetPaths(true);
			} else {
				selectedSquare = null;
			}
		}
	}

	void OnSelectPress() {
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

	void OnReturnPress() {
		if(selectedSquare != null) {
			bool canReturn = false;

			if(
			    (GetSelectedSquare().piece == null) != (GetSelectedSquare().antiPiece == null) &&
			   !(GetSelectedSquare().piece && (GetSelectedSquare().piece.GetComponent<GamePiece>().type == GamePiece.Type.KING || GetSelectedSquare().piece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN)) &&
			   !(GetSelectedSquare().antiPiece && (GetSelectedSquare().antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.KING || GetSelectedSquare().antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN))
			) {
				/*
				Func<GamePiece.Type, Square> find = delegate(GamePiece.Type t) {
					return board.GetSquares().First(s => {
						bool match = false;
						GamePiece p;

						if(s.piece) {
							p = s.piece.GetComponent<GamePiece>();
							match = p.faction == faction && p.type == t;
						}
						
						if(!match && s.antiPiece) {
							p = s.antiPiece.GetComponent<GamePiece>();
							match = p.faction == faction && p.type == t;
						}

						return match;
					});
				};

				Square kingSquare = find(GamePiece.Type.KING), queenSquare = find(GamePiece.Type.QUEEN);
				*/

				string[] kingString = board.GetSquares(GamePiece.Type.KING, faction), queenString = board.GetSquares(GamePiece.Type.QUEEN, faction);
				Square kingSquare = kingString.Length > 0 ? board.GetSquare(kingString[0]) : null, queenSquare = queenString.Length > 0 ? board.GetSquare(queenString[0]) : null;

				//Debug.Log("King: " + kingSquare.piece + ", King-A: " + kingSquare.antiPiece + ", Square: " + GetSelectedSquare().piece + ", Square-A: " + GetSelectedSquare().antiPiece);

				if(kingSquare && kingSquare.piece && kingSquare.piece.GetComponent<GamePiece>().type == GamePiece.Type.KING && GetSelectedSquare().antiPiece) {
					//Debug.Log("T1");
					canReturn = true;	
				} else if(kingSquare && kingSquare.antiPiece && kingSquare.antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.KING && GetSelectedSquare().piece) {
					//Debug.Log("T2");
					canReturn = true;
				} else if(queenSquare && queenSquare.piece && queenSquare.piece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN && GetSelectedSquare().piece) {
					//Debug.Log("T3");
					canReturn = true;	
				} else if(queenSquare && queenSquare.antiPiece && queenSquare.antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN && GetSelectedSquare().antiPiece) {
					//Debug.Log("T4");
					canReturn = true;
				}
			} else if(GetSelectedSquare().piece && GetSelectedSquare().antiPiece &&
				   GetSelectedSquare().piece.GetComponent<GamePiece>().faction == GetSelectedSquare().antiPiece.GetComponent<GamePiece>().faction) {
				if(GetSelectedSquare().piece.GetComponent<GamePiece>().type == GamePiece.Type.KING && GetSelectedSquare().antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN) {
					//Debug.Log("T5");
					canReturn = true;
				} else if(GetSelectedSquare().piece.GetComponent<GamePiece>().type == GamePiece.Type.QUEEN && GetSelectedSquare().antiPiece.GetComponent<GamePiece>().type == GamePiece.Type.KING) {
					//Debug.Log("T6");
					canReturn = true;
				}
			}

			if(canReturn) {
				SetPaths(false);
				board.Swap(GetSelectedSquare().name);

				selectedSquare.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = GetSelectedSquare().tileColor;
				
				UseTurn(GetSelectedSquare().name, GetSelectedSquare().name);
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
		if(Input.GetMouseButtonDown(mouseButtonRotate)) ToggleRotation(true);
		if(Input.GetMouseButtonUp(mouseButtonRotate)) ToggleRotation(false);
		if(Input.GetMouseButton(mouseButtonRotate)) ExecuteRotation();

		if(singleplayer) faction = turnSystem.currentFaction;
		if(!Jail.mainJail.state && turnSystem.IsTurn(faction)) {
			if(Input.GetMouseButtonDown(mouseButtonSelect)) OnSelectPress();
			if(Input.GetMouseButtonDown(mouseButtonReturn)) OnReturnPress();
		}

		if(Input.GetKeyUp(flipButton)/* && GetSelectedSquare() == null*/) {
			bool ss = selectedSquare;

			if(ss) {
				SetPaths(false);
			}

			board.Flipside();

			if(ss) {
				SetPaths(true);
			}
		}

		if(Input.GetKeyDown(restartButton)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		if(Input.GetKeyDown(toggleNamesButton)) board.ToggleSquareNames(!board.showSquareNames);

		if(Input.GetKeyDown(showScoreButton)) {
			Jail.mainJail.Toggle();
		}

		if(Application.isFocused) {
			if(Input.mouseScrollDelta.y != 0) {
				if(Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, board.transform.position) <= minDistanceFromCenter) {
					return;
				} else if(Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, board.transform.position) >= maxDistanceFromCenter) {
					return;
				}

				transform.position = Vector3.MoveTowards(transform.position, board.transform.position, Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime);
			}
		}

		if(Jail.mainJail.state) {
			if(Input.GetMouseButtonDown(mouseButtonSelect)) {
				RaycastHit hit;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
					if(hit.transform.GetComponent<GamePiece>()) {
						heldPiece = hit.transform.gameObject;
						heldDistance = Vector3.Distance(Camera.main.transform.position, heldPiece.gameObject.transform.position);
					}
				}
			}

			if(Input.GetMouseButtonUp(mouseButtonSelect)) {
				heldPiece = null;
			}

			if(heldPiece) {
				heldPiece.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, heldDistance));
				heldPiece.transform.GetComponent<Rigidbody>().velocity = Input.mousePosition - lastMouse;
				lastMouse = Input.mousePosition;
			}
		}
	}
}
