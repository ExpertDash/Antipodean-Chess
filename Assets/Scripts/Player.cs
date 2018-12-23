using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using System.Collections.Generic;

public class Player : MonoBehaviour {
	[System.SerializableAttribute]
	public class Controls {
		public int mouseButtonRotate = 1,
				   mouseButtonSelect = 0;

	   public KeyCode toggleMenu = KeyCode.Escape;
	}

	[System.SerializableAttribute]
	public class Interaction {
		public Vector3 center = Vector3.zero;

		public float rotationSpeed = 100f,
					 zoomSpeed = 10f,
					 minZoom = 5f,
					 maxZoom = 20f;
	}

	[System.SerializableAttribute]
	public class Selection {
		public Piece selectedPiece;
		public GameObject heldPiece;
		public float heldDistance;
		public Vector3 lastMouse;
		public Vector3 heldOffset;
	}

	[System.SerializableAttribute]
	public class Display {
		public GameObject menuUI, ingameUI, promotionUI, rulesUI;
		public Button buttonInvert, buttonPush, buttonPull, buttonShowScore;
		public Dropdown dropdownStyle;
	}

	public Team team;
	public Jail jail;

	//Categories
	public Controls controls;
	public Interaction interaction;
	public Selection selection;
	public Display display;

	void Start() {
		display.ingameUI.SetActive(false);
		display.promotionUI.SetActive(false);
		display.rulesUI.SetActive(false);
		jail.SetVisible(false);
		BoardAnimation.SetAnimationTarget(this);
	}

	void UpdateButtonStatus() {
		display.buttonInvert.interactable = GameManager.board.gameObject.activeSelf && !BoardAnimation.Active();
		display.buttonPush.interactable = GameManager.board.gameObject.activeSelf && !BoardAnimation.Active() && selection.selectedPiece;
		display.buttonPull.interactable = GameManager.board.gameObject.activeSelf && !BoardAnimation.Active() && selection.selectedPiece;
		display.buttonShowScore.interactable = !BoardAnimation.Active();
	}

	void Update() {
		//Button interactability
		UpdateButtonStatus();

		//Controls for menu
		if(Input.GetKeyUp(controls.toggleMenu) && GameManager.IsGameOngoing() && !display.promotionUI.activeSelf) {
			if(display.rulesUI.activeSelf) {
				display.rulesUI.SetActive(false);
			} else if(display.menuUI.activeSelf) {
				display.menuUI.SetActive(false);
				display.ingameUI.SetActive(true);
			} else {
				display.menuUI.SetActive(true);
				display.ingameUI.SetActive(false);
			}
		}

		if(display.ingameUI.activeSelf) {
			//Controls for rotating
			if(Input.GetMouseButtonDown(controls.mouseButtonRotate)) {
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			} else if(Input.GetMouseButtonUp(controls.mouseButtonRotate)) {
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			} else if(Input.GetMouseButton(controls.mouseButtonRotate)) {
				Vector2 move = new Vector2(
					Input.GetAxis("Mouse Y"),
					Input.GetAxis("Mouse X")
				) * interaction.rotationSpeed * Time.deltaTime;

				transform.RotateAround(interaction.center, transform.up, move.y);
				transform.RotateAround(interaction.center, -transform.right, move.x);
			}

			//Controls for zooming
			if(Application.isFocused) {
				if(Input.mouseScrollDelta.y != 0) {
					if(Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, interaction.center) <= interaction.minZoom) {
						return;
					} else if(Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, interaction.center) >= interaction.maxZoom) {
						return;
					}

					transform.position = Vector3.MoveTowards(transform.position, interaction.center, Input.mouseScrollDelta.y * interaction.zoomSpeed * Time.deltaTime);
				}
			}

			//Controls for jail
			if(jail.gameObject.activeSelf && !BoardAnimation.Active()) {
				if(Input.GetMouseButtonDown(controls.mouseButtonSelect)) {
					//Find the object the mouse is over
					RaycastHit hit;
					if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
						//Check if its a piece and hold it at the same distance if so
						if(hit.transform.GetComponent<Piece>()) {
							selection.heldPiece = hit.transform.gameObject;
							selection.heldDistance = Vector3.Distance(Camera.main.transform.position, selection.heldPiece.gameObject.transform.position);
							selection.heldOffset = selection.heldPiece.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selection.heldDistance));
						}
					}
				} else if(Input.GetMouseButtonUp(controls.mouseButtonSelect)) {
					//Stop holding piece
					selection.heldPiece = null;
				}

				//Update the pieces position so it follows the cursor and maintains its velocity
				if(selection.heldPiece) {
					selection.heldPiece.transform.position = selection.heldOffset + Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, selection.heldDistance));
					selection.heldPiece.transform.GetComponent<Rigidbody>().velocity = Input.mousePosition - selection.lastMouse;
					selection.lastMouse = Input.mousePosition;
				}
			}

			//Controls for selecting on board (only possible when no animation is being undergone)
			if(GameManager.IsTurn(team) && !jail.gameObject.activeSelf && Input.GetMouseButtonDown(controls.mouseButtonSelect) && !BoardAnimation.Active()) {
				RaycastHit hit;
				if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
					string location = null;

					if(hit.transform.GetComponent<Piece>()) location = hit.transform.GetComponent<Piece>().GetLocation();
					else if(hit.transform.parent && hit.transform.parent.GetComponent<Tile>()) location = hit.transform.name;

					if(!string.IsNullOrEmpty(location)) {
						Piece existingPiece = GameManager.board.GetPiece(location);

						if(selection.selectedPiece) {
							//Decolor paths
							SetMovementPathsColor(selection.selectedPiece, null);
							SetAttackPathsColor(selection.selectedPiece, null);

							if(existingPiece == selection.selectedPiece) {
								//Deselect if already selected
								selection.selectedPiece = null;
							} else if(existingPiece && existingPiece.team == selection.selectedPiece.team) {
								//Select new piece and color paths if same team as previously selected
								selection.selectedPiece = existingPiece;
								SetMovementPathsColor(existingPiece, BoardConfig.visuals.moveTileColor);
								SetAttackPathsColor(existingPiece, BoardConfig.visuals.attackTileColor);
							} else {
								//Create an instance of the piece to be captured if the move is an attack
								Piece capturedPiece = null;
								if(existingPiece) {
									capturedPiece = Instantiate(existingPiece);
									capturedPiece.SetTransformParent(GameManager.board.GetPlane(location).transform);
								}

								Vector3 initialPos = selection.selectedPiece.transform.position;
								Quaternion initialRot = selection.selectedPiece.transform.rotation;

								if(GameManager.Move(selection.selectedPiece, location)) {
									//Animate movement
									BoardAnimation.Move(selection.selectedPiece, initialPos, initialRot);

									//Animate piece being captured if there was one
									if(capturedPiece) {
										BoardAnimation.onComplete = () => {
											jail.Add(capturedPiece);
										};

										BoardAnimation.Capture(capturedPiece);
									}

									bool shouldPromote = false;

									//Check if the piece was a pawn and should be promoted
									if(selection.selectedPiece.type == Piece.Type.PAWN) {
										switch(GameManager.board.style) {
											case Chessboard.Style.ANTIPODEAN: //Promote if the pawn reached an axial tile
												shouldPromote = Tile.IsAxial(location);
												break;
											case Chessboard.Style.SPHERE: //Promote if the pawn reached the the first or last row
												int row = int.Parse("" + location[1]);
												shouldPromote = row == BoardConfig.initialRow || row == BoardConfig.finalRow;
												break;
										}
									}

									if(shouldPromote) {
										//Show promotion UI if conditions were met
										display.ingameUI.SetActive(false);
										display.promotionUI.SetActive(true);
									} else {
										//Deselect if piece moved
										selection.selectedPiece = null;

										//Use turn
										GameManager.NextTurn();
									}
								} else {
									//Don't animate since a piece wasn't captured
									if(capturedPiece) Destroy(capturedPiece.gameObject);

									//Recolor paths if piece wasn't moved
									SetMovementPathsColor(selection.selectedPiece, BoardConfig.visuals.moveTileColor);
									SetAttackPathsColor(selection.selectedPiece, BoardConfig.visuals.attackTileColor);
								}
							}
						} else if(existingPiece && (existingPiece.team == team || team == Team.ALL)) {
							//Select and color paths
							selection.selectedPiece = existingPiece;
							SetMovementPathsColor(existingPiece, BoardConfig.visuals.moveTileColor);
							SetAttackPathsColor(existingPiece, BoardConfig.visuals.attackTileColor);
						}
					}
				}
			}
		}
	}

	/// <summary>Sets the color for all tiles that the piece can move to</summary>
	/// <param name="piece">Piece to color paths for</param>
	/// <param name="color">Color to set tiles in the path. Uses the tiles default color if null</param>
	void SetMovementPathsColor(Piece piece, Color? color) {
		Tile tile = null;

		Action<string> applyColor = delegate(string location) {
			//Get the tile at the location
			if(tile = GameManager.board.GetTile(location)) {
				//Get the given color or tile's default color
				Color c = color != null ? color.Value : tile.color;

				//Set the tile's planes' colors
				if(GameManager.board.IsLocationInverse(location)) tile.SetInverseColor(c);
				else tile.SetColor(c);
			}
		};

		//Apply color to selected piece and those in its path
		applyColor(piece.GetLocation());
		foreach(string location in Rules.GetMoveableLocations(piece)) {
			if(!GameManager.board.GetPiece(location)) applyColor(location);
		}
	}

	/// <summary>Sets the color for all tiles that the piece can attack</summary>
	/// <param name="piece">Piece to color paths for</param>
	/// <param name="color">Color to set tiles in the path. Uses the tiles default color if null</param>
	void SetAttackPathsColor(Piece piece, Color? color) {
		Piece target = null;
		Tile tile = null;

		foreach(string location in Rules.GetAttackableLocations(piece)) {
			//Get the tile at the location and the piece in the attack path
			if((tile = GameManager.board.GetTile(location)) && (target = GameManager.board.GetPiece(location)) && target.team != piece.team) {
				//Get the given color or tile's default color
				Color c = color != null ? color.Value : tile.color;

				//Set the tile's planes' colors
				if(GameManager.board.IsLocationInverse(location)) tile.SetInverseColor(c);
				else tile.SetColor(c);
			}
		}
	}

	/// <summary>Uses the Event System to determine when a button is pressed and carry out the appropriate action</summary>
	/// <param name="id">ID of the button being pressed</param>
	public void OnClick(int id) {
		//Ensure buttons are at latest state
		UpdateButtonStatus();

		switch(id) {
			case 0: //Play
				if(!GameManager.IsGameOngoing()) {
					Chessboard.Style style = Chessboard.Style.ANTIPODEAN;

					//Get the approiate mode
					switch(display.dropdownStyle.value) {
						case 0:
							display.buttonInvert.gameObject.SetActive(true);
							display.buttonPush.gameObject.SetActive(true);
							display.buttonPull.gameObject.SetActive(true);
							style = Chessboard.Style.ANTIPODEAN;
							break;
						case 1:
							display.buttonInvert.gameObject.SetActive(false);
							display.buttonPush.gameObject.SetActive(false);
							display.buttonPull.gameObject.SetActive(false);
							style = Chessboard.Style.SPHERE;
							break;
						default:
							break;
					}

					GameManager.SetGameOngoing(true);
					GameManager.board.Create(style);
					jail.Create();
				}

				display.menuUI.SetActive(false);
				display.ingameUI.SetActive(true);
				break;
			case 1: //Options
				break;
			case 2: //Quit
				if(Application.isEditor) {
					GameManager.SetGameOngoing(false);
					GameManager.board.Clear();
					jail.Clear();
				} else {
					Application.Quit();
				}
				break;
			case 3: //Invert
				if(display.buttonInvert.interactable) {
					GameManager.board.isInverted = !GameManager.board.isInverted;
					BoardAnimation.Inversion();
				}
				break;
			case 4: //Toggle Names
				GameManager.board.ToggleTileNames();
				break;
			case 5: //Show Score
				if(display.buttonShowScore.interactable) {
					bool jailVisibility = !jail.gameObject.activeSelf;
					GameManager.board.gameObject.SetActive(!jailVisibility);
					jail.SetVisible(jailVisibility);
				}
				break;
			case 6: //Push
				if(display.buttonPush.interactable && Rules.CanPush(selection.selectedPiece)) {
					//Decolor
					SetMovementPathsColor(selection.selectedPiece, null);
					SetAttackPathsColor(selection.selectedPiece, null);

					//Swap and deselect after animation completed
					BoardAnimation.onComplete = () => {
						GameManager.Swap(selection.selectedPiece.GetLocation());
						selection.selectedPiece = null;

						//Use turn
						GameManager.NextTurn();
					};

					//Animate push
					BoardAnimation.PushOrPull(selection.selectedPiece.GetLocation());
				}
				break;
			case 7: //Pull
				if(display.buttonPull.interactable && Rules.CanPull(selection.selectedPiece)) {
					//Decolor
					SetMovementPathsColor(selection.selectedPiece, null);
					SetAttackPathsColor(selection.selectedPiece, null);

					//Swap and deselect after animation completed
					BoardAnimation.onComplete = () => {
						GameManager.Swap(selection.selectedPiece.GetLocation());
						selection.selectedPiece = null;

						//Use turn
						GameManager.NextTurn();
					};

					//Animate pull or swap
					Tile tile = selection.selectedPiece.GetTile();
					if(tile.GetPiece() && tile.GetInversePiece() &&
						(
							(tile.GetPiece().type == Piece.Type.KING && tile.GetInversePiece().type == Piece.Type.QUEEN) ||
							(tile.GetInversePiece().type == Piece.Type.KING && tile.GetPiece().type == Piece.Type.QUEEN))
						) {
						BoardAnimation.Swap(selection.selectedPiece.GetLocation());
					} else {
						BoardAnimation.PushOrPull(selection.selectedPiece.GetLocation());
					}
				}
				break;
			case 8: case 9: case 10: case 11: //Promotion
				Piece.Type type = Piece.Type.PAWN;

				//Get type based on button press
				if(id == 8) type = Piece.Type.QUEEN;
				else if(id == 9) type = Piece.Type.KNIGHT;
				else if(id == 10) type = Piece.Type.ROOK;
				else if(id == 11) type = Piece.Type.BISHOP;

				//Promopte the piece
				GameManager.Promote(selection.selectedPiece, type);
				
				//Change UI
				display.promotionUI.SetActive(false);
				display.ingameUI.SetActive(true);

				//Deselect and end the turn
				selection.selectedPiece = null;
				GameManager.NextTurn();
				break;
			case 12: //Rules
				display.rulesUI.SetActive(true);
				break;
			case 13: //Close Rules
				display.rulesUI.SetActive(false);
				break;
			case 14: //Undo
				GameManager.Undo();
				break;
			default:
				Debug.Log("Undefined interaction: " + id);
				break;
		}
	}
}