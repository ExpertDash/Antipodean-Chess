using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {
	private static GameManager instance;
	public static Chessboard board { get { return instance._board; } }

	public Chessboard _board;
	public bool ongoingGame = false;

	private Dictionary<Team, Piece> kings;
	public Team[] teams;
	public int teamIndex = 0;

	public Stack<string> record;

	void Start() {
		if(instance) {
			Debug.Log("Another GameManager was attempted to be instantiated");
			Destroy(this);
		} else {
			instance = this;
			record = new Stack<string>();
		}
	}

	private static void AddToRecord(string play) {
		instance.record.Push(play);
		Debug.Log(play);
	}

	public static void Undo() {
		//TODO: Undo moves that allow king to be checkmated
		if(instance.record.Count > 0) {
			string lastPlay = instance.record.Pop();

			string[] playType = lastPlay.Split(':');

			switch(playType[0]) {
				case "M": //Move
					{
						string[] data = playType[1].Split(',');

						Piece piece = board.GetPiece(data[1]);
						piece.SetTransformParent(board.GetPlane(data[0]).transform);
						piece.timesMoved--;
					}
					break;
				case "C": //Capture
					{
						string[] data = playType[1].Split(',');

						string oldLocation = data[0];
						string location = data[1];
						Piece.Type enemyType = (Piece.Type)Enum.Parse(typeof(Piece.Type), data[2]);
						Team enemyTeam = (Team)Enum.Parse(typeof(Team), data[3]);

						Piece piece = board.GetPiece(location);
						piece.SetTransformParent(board.GetPlane(oldLocation).transform);
						piece.timesMoved--;

						Place(enemyType, enemyTeam, location);
					}
					break;
				case "S": //Swap
					Swap(playType[1], false);
					break;
				case "P": //Promote
					{
						string[] data = playType[1].Split(',');

						string location = data[0];
						Piece piece = board.GetPiece(location);
						Team team = piece.team;
						int timesMoved = piece.timesMoved;

						Destroy(piece.gameObject);
						Place((Piece.Type)Enum.Parse(typeof(Piece.Type), data[1]), team, location);
						board.GetPiece(location).timesMoved = timesMoved;
					}
					break;
				default:
					break;
			}
		} else {
			Debug.Log("Unable to undo further");
		}
	}

	public static bool IsGameOngoing() {
		return instance.ongoingGame;
	}

	public static void SetGameOngoing(bool state) {
		instance.ongoingGame = state;
	}

	/// <summary>Places a piece on the board</summary>
	/// <param name="type">Type of piece to be placed</param>
	/// <param name="team">Team to assign the piece</param>
	/// <param name="location">Location to place the piece</param>
	/// <returns>Returns whether a piece could be placed<returns>
	public static bool Place(Piece.Type type, Team team, string location) {
		GameObject plane = null;

		//Only place if a tile exists and there is no piece on the location
		if((plane = board.GetPlane(location)) && !board.GetPiece(location)) {
			//Get appropriate prefab and create instance
			GameObject prefab = Piece.GetPrefab(type);
			GameObject pieceObject = Instantiate(prefab);
			pieceObject.name = prefab.name;

			//Add piece component
			Piece piece = pieceObject.AddComponent<Piece>();
			piece.type = type;
			piece.team = team;
			piece.heading = team == Team.WHITE ? Direction.Down : Direction.Up;
			piece.timesMoved = 0;
			piece.color = piece.color; //Getting the piece's color always returns color based on team while setting can make it any color
			piece.SetTransformParent(plane.transform);

			return true;
		}

		return false;
	}

	/// <summary>Moves a piece from its current location to the given</summary>
	/// <param name="piece">The piece to move</param>
	/// <param name="location">The location to move the piece to</param>
	/// <returns>Returns whether the piece could be moved</returns>
	public static bool Move(Piece piece, string location, bool record = true) {
		Piece existingPiece = board.GetPiece(location);
		GameObject plane = board.GetPlane(location);

		if(!existingPiece && Rules.GetMoveableLocations(piece).Contains(location)) {
			if(record) AddToRecord("M:" + piece.GetLocation() + "," + location);

			//Move
			piece.SetTransformParent(plane.transform);
			piece.timesMoved++;

			return true;
		} else if(existingPiece && existingPiece.team != piece.team && Rules.GetAttackableLocations(piece).Contains(location)) {
			if(record) AddToRecord("C:" + piece.GetLocation() + "," + location + "," + existingPiece.type + "," + existingPiece.team);

			//Capture
			plane.transform.DetachChildren();
			Destroy(existingPiece.gameObject);
			piece.SetTransformParent(plane.transform);
			piece.timesMoved++;

			return true;
		}

		return false;
	}

	/// <summary>Swaps the piece at the location and the inverse location</summary>
	/// <param name="location">Tile to swap</param>
	/// <remarks>Pieces on either side of the swap do not have their first move used<remarks>
	public static void Swap(string location, bool record = true) {
		Tile tile = board.GetTile(location);

		Piece piece = tile.GetPiece(), inversePiece = tile.GetInversePiece();
		GameObject plane = tile.GetPlane(), inversePlane = tile.GetInversePlane();

		if(piece) piece.SetTransformParent(inversePlane.transform);
		if(inversePiece) inversePiece.SetTransformParent(plane.transform);

		if(record) AddToRecord("S:" + location);
	}

	/// <summary>Promotes a piece into a different type</summary>
	/// <param name="piece">Piece to be promoted</param>
	/// <param name="type">Type to be promoted into</param>
	public static void Promote(Piece piece, Piece.Type type, bool record = true) {
		//Get the piece's location and team
		string location = piece.GetLocation();
		Team team = piece.team;

		if(record) AddToRecord("P:" + location + "," + piece.type + "," + type);

		//Destroy the piece
		GameObject plane = board.GetPlane(piece.GetLocation());
		plane.transform.DetachChildren();
		Destroy(piece.gameObject);

		//Place a new piece with the promoted type on the same team at the same location
		Place(type, team, location);
	}

	/// <summary>Go to the next turn</summary>
	public static void NextTurn() {
		if(instance.kings == null) {
			instance.kings = new Dictionary<Team, Piece>();;

			//Discover kings for each team and place them into dictionary
			foreach(Tile tile in board.GetTiles()) {
				Piece piece = tile.GetPiece();
				Piece inversePiece = tile.GetInversePiece();

				if(piece && piece.type == Piece.Type.KING) instance.kings[piece.team] = piece;
				if(inversePiece && inversePiece.type == Piece.Type.KING) instance.kings[inversePiece.team] = inversePiece;
			}
		}

		//Alternate between whose turn it is
		if(++instance.teamIndex >= instance.teams.Length) instance.teamIndex = 0;

		//Display checkmated kings as red
		foreach(Piece king in instance.kings.Values) {
			if(Rules.IsVunerable(king)) {
				Tile tile = king.GetTile();
				if(king.IsLocationInverse()) tile.SetInverseColor(BoardConfig.visuals.checkTileColor);
				else tile.SetColor(BoardConfig.visuals.checkTileColor);
			}
		}
	}

	/// <param name="team">Team to check</param>
	/// <returns>Returns whether it is the given team's turn</returns>
	public static bool IsTurn(Team team) {
		return (team == Team.ALL) || ((0 <= instance.teamIndex && instance.teamIndex < instance.teams.Length) ? instance.teams[instance.teamIndex] == team : false);
	}
}