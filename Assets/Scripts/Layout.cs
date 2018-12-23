using UnityEngine;
using System;

/// <summary>Loads layouts or creates them programatically</summary>
public class Layout {
	public static void EnactDefaultLayout(Chessboard.Style style) {
		switch(style) {
			case Chessboard.Style.ANTIPODEAN:
				DefaultAntipodean();
				break;
			case Chessboard.Style.SPHERE:
				DefaultSphere();
				break;
		}
	}

	private static void DefaultAntipodean() {
		GameManager.Place(Piece.Type.KING, Team.WHITE, "Alpha");
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "A1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "B1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "C1");
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "D1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "E1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "F1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "G1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "H1");

		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "A2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "B2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "C2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "D2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "E2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "F2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "G2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "H2");

		GameManager.Place(Piece.Type.QUEEN, Team.WHITE, "-Alpha");
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "-A1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "-B1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "-C1");
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "-D1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "-E1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "-F1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "-G1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "-H1");

		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-A2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-B2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-C2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-D2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-E2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-F2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-G2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "-H2");

		GameManager.Place(Piece.Type.KING, Team.BLACK, "Omega");
		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "A8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "B8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "C8");
		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "D8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "E8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "F8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "G8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "H8");

		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "A7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "B7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "C7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "D7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "E7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "F7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "G7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "H7");

		GameManager.Place(Piece.Type.QUEEN, Team.BLACK, "-Omega");
		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "-A8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "-B8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "-C8");
		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "-D8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "-E8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "-F8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "-G8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "-H8");

		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-A7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-B7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-C7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-D7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-E7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-F7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-G7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "-H7");
	}

	private static void DefaultSphere() {
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "A1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "B1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "C1");
		GameManager.Place(Piece.Type.QUEEN, Team.WHITE, "D1");
		GameManager.Place(Piece.Type.KING, Team.WHITE, "E1");
		GameManager.Place(Piece.Type.BISHOP, Team.WHITE, "F1");
		GameManager.Place(Piece.Type.KNIGHT, Team.WHITE, "G1");
		GameManager.Place(Piece.Type.ROOK, Team.WHITE, "H1");

		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "A2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "B2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "C2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "D2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "E2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "F2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "G2");
		GameManager.Place(Piece.Type.PAWN, Team.WHITE, "H2");

		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "A8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "B8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "C8");
		GameManager.Place(Piece.Type.QUEEN, Team.BLACK, "D8");
		GameManager.Place(Piece.Type.KING, Team.BLACK, "E8");
		GameManager.Place(Piece.Type.BISHOP, Team.BLACK, "F8");
		GameManager.Place(Piece.Type.KNIGHT, Team.BLACK, "G8");
		GameManager.Place(Piece.Type.ROOK, Team.BLACK, "H8");

		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "A7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "B7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "C7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "D7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "E7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "F7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "G7");
		GameManager.Place(Piece.Type.PAWN, Team.BLACK, "H7");
	}
}