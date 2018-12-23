using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

///<summary>Contains the set of movement and attack rules that pieces must follow along with methods required to navigate the board</summary>
public abstract class Rules {
	///<summary>Finds a column at a certain distance away from another</summary>
	///<param name="startingColumn">The column to start at</param>
	///<param name="distance">The distance away from the starting column</param>
	///<returns>Returns the column at the destination</returns>
	///<remarks>
	///<para>Examples for Default BoardConfig:</para>
	///<para>	FindColumn('A', 1) returns 'B'</para>
	///<para>	FindColumn('C', 2) returns 'E'</para>
	///<para>	FindColumn('H', 1) returns 'A'</para>
	///</remarks>
	public static char FindColumn(char startingColumn, int distance) {
		char endingColumn = (char)(startingColumn + distance);

		//Loop around recursively if outside bounds
		if(endingColumn > BoardConfig.finalColumn) return FindColumn(BoardConfig.initialColumn, endingColumn - BoardConfig.finalColumn - 1);
		else if(endingColumn < BoardConfig.initialColumn) return FindColumn(BoardConfig.finalColumn, 1 - (BoardConfig.initialColumn - endingColumn));

		return endingColumn;
	}

	///<summary>Finds a row at a certain distance away from another</summary>
	///<param name="startingRow">The row to start at</param>
	///<param name="distance">The distance away from the starting row</param>
	///<returns>Returns the row at the destination</returns>
	public static int FindRow(int startingRow, int distance) {
		int endingRow = startingRow + distance;

		//Stop at initial or final row
		if(endingRow < BoardConfig.initialRow - 1) return BoardConfig.initialRow - 1;
		else if(endingRow > BoardConfig.finalRow + 1) return BoardConfig.finalRow + 1;

		return endingRow;
	}

	///<summary>Finds the tile(s) at a given distance away from the location in the direction</summary>
	///<param name="location">The location to search from</param>
	///<param name="direction">The direction to search in</param>
	///<param name="distance">How far to search from the location</param>
	///<returns>Returns the tile(s) in the direction from the location at the distance</returns>
	///<remarks>
	///<para>Most tiles return just one result</para>
	///<para>Axial tiles lack direction and therefore return all surrounding it regardless of direction</para>
	///</remarks>
	public static HashSet<string> GetTileTowards(string location, Direction direction, int distance) {
		HashSet<string> tiles = new HashSet<string>();
		string prefix = GameManager.board.IsLocationInverse(location) ? "-" : "";
		Tile tile = GameManager.board.GetTile(location);

		if(tile) {
			if(tile.IsAxial()) {
				for(char i = BoardConfig.initialColumn; i <= BoardConfig.finalColumn; i++) {
					tiles.UnionWith(GetTileTowards(prefix + i + (tile.location == "Alpha" ? BoardConfig.initialRow : BoardConfig.finalRow), direction, distance - 1));
				}
			} else {
				int columnDistance = 0, rowDistance = 0;

				if(direction.IsRight()) columnDistance += distance;
				if(direction.IsLeft()) columnDistance -= distance;
				if(direction.IsUp()) rowDistance -= distance;
				if(direction.IsDown()) rowDistance += distance;				

				char column = FindColumn(tile.location[0], columnDistance);
				int row = FindRow(int.Parse("" + tile.location[1]), rowDistance);

				if(row == BoardConfig.initialRow - 1) tiles.Add(prefix + "Alpha");
				else if(row == BoardConfig.finalRow + 1) tiles.Add(prefix + "Omega");
				else tiles.Add(prefix + column + row);
			}
		}

		return tiles;
	}

	///<param name="location">The location to search from</param>
	///<param name="direction">The direction to search in</param>
	///<param name="distance">How far to search from the location</param>
	///<returns>Returns all the tiles in the direction from the location to the distance</returns>
	public static HashSet<string> GetTilesBetween(string location, Direction direction, int distance) {
		HashSet<string> tiles = new HashSet<string>();

		if(Tile.IsAxial(location)) {
			foreach(string s in GetTileTowards(location, direction, 1)) {
				if(!GameManager.board.GetPiece(s) && !GetTilesBetween(s, direction, 1).Contains(location)) {
					tiles.Add(s);
					tiles.UnionWith(GetTilesBetween(s, direction, distance - 1));
				}
			}
		} else {
			for(int i = 0; i < distance; i++) {
				HashSet<string> next = GetTileTowards(location, direction, 1 + i);
				tiles.UnionWith(next);
				if(next.Count == 1 && GameManager.board.GetPiece(next.First())) break;
			}
		}

		return tiles;
	}

	///<param name="piece">Piece to find movement path for</param>
	///<returns>Returns the movement path for a piece</returns>
	///<remarks>Does not check if the locations are already taken by other pieces<remarks>
	public static HashSet<string> GetMoveableLocations(Piece piece) {
		HashSet<string> paths = new HashSet<string>();

		switch(piece.type) {
			case Piece.Type.PAWN:
				paths = GetTilesBetween(piece.GetLocation(), piece.heading, piece.timesMoved == 0 ? 2 : 1);
				break;
			case Piece.Type.KNIGHT:
				Action<Direction, Direction> addTSection = delegate(Direction forward, Direction sides) {
					foreach(string p in GetTileTowards(piece.GetLocation(), forward, 2)) {
						paths.UnionWith(GetTileTowards(p, sides, 1));
						paths.UnionWith(GetTileTowards(p, ~sides, 1));
					}
				};

				//Knights have a T shaped movement hop
				addTSection(Direction.Up, Direction.Left);
				addTSection(Direction.Down, Direction.Left);
				addTSection(Direction.Left, Direction.Up);
				addTSection(Direction.Right, Direction.Up);
				break;
			case Piece.Type.BISHOP:
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up.ToLeft, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up.ToRight, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down.ToLeft, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down.ToRight, BoardConfig.maxTravelDistance));
				break;
			case Piece.Type.ROOK:
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Left, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Right, BoardConfig.maxTravelDistance));
				break;
			case Piece.Type.QUEEN:
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up.ToLeft, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up.ToRight, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down.ToLeft, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down.ToRight, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Up, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Down, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Left, BoardConfig.maxTravelDistance));
				paths.UnionWith(GetTilesBetween(piece.GetLocation(), Direction.Right, BoardConfig.maxTravelDistance));
			break;
			case Piece.Type.KING:
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Up, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Down, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Left, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Right, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Up.ToLeft, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Up.ToRight, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Down.ToLeft, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), Direction.Down.ToRight, 1));
				break;
			default:
				break;
		}

		return paths;
	}

	///<param name="piece">Piece to find attack path for</param>
	///<returns>Returns the attack path for a piece</returns>
	public static HashSet<string> GetAttackableLocations(Piece piece) {
		HashSet<string> paths = new HashSet<string>();

		switch(piece.type) {
			case Piece.Type.PAWN:
				paths.UnionWith(GetTileTowards(piece.GetLocation(), piece.heading.ToLeft, 1));
				paths.UnionWith(GetTileTowards(piece.GetLocation(), piece.heading.ToRight, 1));
				break;
			default:
				return GetMoveableLocations(piece);
		}

		return paths;
	}

	/// <param name="piece">Piece to check</param>
	/// <returns>Returns whether the piece can be pushed to the other side</returns>
	public static bool CanPush(Piece piece) {
		bool isLocInverse = GameManager.board.IsLocationInverse(piece.GetLocation());
		Tile tile = piece.GetTile();

		//Only allow pushing if the other side is empty
		if(!(isLocInverse ? tile.GetPiece() : tile.GetInversePiece())) {
			//Check if a queen exists to push
			return GameManager.board.GetTiles().Any(t => {
				//Searches through pieces on the same side as the selected
				Piece found = isLocInverse ? t.GetInversePiece() : t.GetPiece();

				//Returns true only if the found piece is a queen on the same team and if it's not the same as the selected piece (which would imply a queen pushing itself since only another queen can push a queen)
				return found && found.type == Piece.Type.QUEEN && found.team == piece.team && found != piece;
			});
		}

		return false;
	}

	/// <param name="piece">Piece to check</param>
	/// <returns>Returns whether the piece can be pulled to the other side</returns>
	public static bool CanPull(Piece piece) {
		bool isLocInverse = GameManager.board.IsLocationInverse(piece.GetLocation());
		Tile tile = piece.GetTile();
		Piece inversePiece = isLocInverse ? tile.GetPiece() : tile.GetInversePiece();

		//Only allow pushing if the other side is empty or a queen is selected and the team's king is on the other side
		if(!inversePiece) {
			return GameManager.board.GetTiles().Any(t => {
				//Searches through pieces on the inverse side of the selected
				Piece found = isLocInverse ? t.GetPiece() : t.GetInversePiece();

				//Returns true only if the piece is the king of the same team
				return found && found.type == Piece.Type.KING && found.team == piece.team;
			});
		} else if(piece.type == Piece.Type.QUEEN && inversePiece.type == Piece.Type.KING) {
			return true;
		}

		return false;
	}

	/// <summary>Stands for Is Location Susceptible to attack by the enemy team</summary>
	/// <param name="location">Location to check susceptibility</param>
	/// <param name="team">Team to consider friendly</param>
	/// <returns>Returns whether an enemy piece can attack the given location</returns>
	public static bool IsLocationSusceptible(string location, Team team) {
		//Locations that have already been checked
		HashSet<string> marked = new HashSet<string>();

		//Gets all locations adjacent to the given
		Func<string, HashSet<string>> getSurrounding = delegate(string loc) {
			HashSet<string> surrounding = new HashSet<string>();

			surrounding.UnionWith(GetTileTowards(loc, Direction.Up, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Down, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Left, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Right, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Up.ToLeft, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Up.ToRight, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Down.ToLeft, 1));
			surrounding.UnionWith(GetTileTowards(loc, Direction.Down.ToRight, 1));

			return surrounding;
		};

		//If the candidate piece can attack the given location
		Func<Piece, bool> isAggressor = delegate(Piece candidate) {
			return candidate.team != team && GetAttackableLocations(candidate).Contains(location);
		};

		//Declare to allow recursive lambda
		Func<string, bool> checkSurrounding = null;

		checkSurrounding = delegate(string loc) {
			//Get surrounding tiles and check if they can attack
			foreach(string l in getSurrounding(loc)) {
				if(marked.Add(l)) {
					Piece nearby = GameManager.board.GetPiece(l);

					if(nearby) {
						if(isAggressor(nearby)) return true;
					} else if(checkSurrounding(l)) {
						return true;
					}
				}
			}

			return false;
		};

		//Get piece on the same side that don't require LOS to attack and check them
		foreach(Tile tile in GameManager.board.GetTiles()) {
			Piece p = GameManager.board.IsLocationInverse(location) ? tile.GetInversePiece() : tile.GetPiece();

			if(p && !p.RequiresLOS()) {
				marked.Add(p.GetLocation());
				if(isAggressor(p)) return true;
			}
		}

		//Mark initial location since no checking needs to occur here
		marked.Add(location);
		return checkSurrounding(location);
	}

	/// <param name="piece">Piece to check vunerability for</param>
	/// <returns>Returns true if any enemy piece has the ability to capture the specified one</returns>
	public static bool IsVunerable(Piece piece) {
		return IsLocationSusceptible(piece.GetLocation(), piece.team);
	}
}