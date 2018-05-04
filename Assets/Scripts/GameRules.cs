using System.Collections.Generic;
using System.Linq;

public class GameRules {
	public static readonly int MAX_DISTANCE = 10;
	public int currentFaction;
	public int currentTurn;

	public static char minColumn = 'A', maxColumn = 'H';
	public static int minRow = 1, maxRow = 8;

	public static GameBoard board {get; set;}

	public static char GetColumn(char letter, int distance) {
		char place = (char)(letter + distance);

		if(place > maxColumn) {
			return GetColumn(minColumn, place - maxColumn - 1);
		} else if(place < minColumn) {
			return GetColumn(maxColumn, minColumn - place - 1);
		} else {
			return place;
		}
	}

	public static int GetRow(int number, int distance) {
		int place = number + distance;

		if(place <= minRow - 1) {
			return minRow - 1;
		} else if(place >= maxRow + 1) {
			return maxRow + 1;
		} else {
			return place;
		}
	}

	public static string[] GetSquareAt(string square, GamePiece.Direction direction, int distance) {
		int xDist = 0, yDist = 0;

		if(square == "Alpha") {
			List<string> squares = new List<string>();

			for(char i = (char)(minColumn - 1); i < maxColumn; i++) {
				squares.AddRange(GetSquareAt("" + i + minRow, direction, distance - 1));
			}

			return squares.ToArray();
		} else if(square == "Omega") {
			List<string> squares = new List<string>();

			for(char i = (char)(minColumn - 1); i < maxColumn; i++) {
				squares.AddRange(GetSquareAt("" + i + maxRow, direction, distance - 1));
			}

			return squares.ToArray();
		}

		if(direction == GamePiece.Direction.RIGHT ||
		   direction == GamePiece.Direction.UP_RIGHT ||
		   direction == GamePiece.Direction.DOWN_RIGHT) {
			xDist += distance;
		} else if(direction == GamePiece.Direction.LEFT ||
				  direction == GamePiece.Direction.UP_LEFT ||
				  direction == GamePiece.Direction.DOWN_LEFT) {
			xDist -= distance;
		}

		if(direction == GamePiece.Direction.UP ||
		   direction == GamePiece.Direction.UP_LEFT ||
		   direction == GamePiece.Direction.UP_RIGHT) {
			yDist -= distance;
		} else if(direction == GamePiece.Direction.DOWN ||
				  direction == GamePiece.Direction.DOWN_LEFT ||
				  direction == GamePiece.Direction.DOWN_RIGHT) {
			yDist += distance;
		}

		char column = GetColumn(square[0], xDist);
		int row = GetRow(int.Parse("" + square[1]), yDist);

		if(row == 0) {
			return new string[]{"Alpha"};
		} else if(row == 9) {
			return new string[]{"Omega"};
		} else {
			return new string[]{ "" + column + row};
		}
	}

	public static string[] GetSquaresBetween(string square, GamePiece.Direction direction, int distance) {
		List<string> squares = new List<string>();

		for(int i = 0; i < distance; i++) {
			string[] next = GetSquareAt(square, direction, 1 + i);

			squares.AddRange(next);

			if(board != null && next.Length == 1 && board.GetSquare(next[0]).piece != null) {
				break;
			}
		}

		return squares.ToArray();
	}

	private static void Merge(ref List<string> paths, string path) {
		paths = paths.Union(new string[]{path}).ToList();
	}

	private static void Merge(ref List<string> paths, string[] newPaths) {
		paths = paths.Union(newPaths).ToList();
	}

	public static string[] GetPaths(GamePiece piece, string square) {
		List<string> paths = new List<string>();

		switch(piece.type) {
            case GamePiece.Type.PAWN:
				Merge(ref paths, GetSquareAt(piece.GetSquare(), piece.direction, 1));
                break;
            case GamePiece.Type.KNIGHT:
				//top t
				foreach(string p in GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP, 2)) {
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.LEFT, 1));
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.RIGHT, 1));
				}

				//bot t
				foreach(string p in GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN, 2)) {
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.LEFT, 1));
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.RIGHT, 1));
				}

				//left t
				foreach(string p in GetSquareAt(piece.GetSquare(), GamePiece.Direction.LEFT, 2)) {
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.UP, 1));
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.DOWN, 1));
				}

				//right t
				foreach(string p in GetSquareAt(piece.GetSquare(), GamePiece.Direction.RIGHT, 2)) {
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.UP, 1));
					Merge(ref paths, GetSquareAt(p, GamePiece.Direction.DOWN, 1));
				}


                break;
            case GamePiece.Type.BISHOP:
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP_LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP_RIGHT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN_LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN_RIGHT, MAX_DISTANCE));
                break;
            case GamePiece.Type.ROOK:
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.RIGHT, MAX_DISTANCE));
                break;
            case GamePiece.Type.QUEEN:
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP_LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP_RIGHT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN_LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN_RIGHT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.UP, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.DOWN, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.LEFT, MAX_DISTANCE));
				Merge(ref paths, GetSquaresBetween(piece.GetSquare(), GamePiece.Direction.RIGHT, MAX_DISTANCE));
                break;
            case GamePiece.Type.KING:
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_LEFT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_RIGHT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_LEFT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_RIGHT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_LEFT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_RIGHT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_LEFT, 1));
				Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_RIGHT, 1));
                break;
            default:
                break;
		}

		return paths.ToArray();
	}

	public static string[] GetAttackPaths(GamePiece piece, string square) {
		List<string> paths = new List<string>();

		switch(piece.type) {
            case GamePiece.Type.PAWN:
				if(piece.direction == GamePiece.Direction.DOWN) {
					Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_LEFT, 1));
					Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.DOWN_RIGHT, 1));
				} else if(piece.direction == GamePiece.Direction.UP) {
					Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_LEFT, 1));
					Merge(ref paths, GetSquareAt(piece.GetSquare(), GamePiece.Direction.UP_RIGHT, 1));
				}
                break;
            default:
				return GetPaths(piece, square);
		}

		return paths.ToArray();
	}
}
