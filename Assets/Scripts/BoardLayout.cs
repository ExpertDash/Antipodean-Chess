using UnityEngine;
using System;

public class BoardLayout : MonoBehaviour {
    [System.SerializableAttribute]
    public struct Placement {
        public string square;
        public int faction;
        public GamePiece.Type piece;
    }

    [System.SerializableAttribute]
    public struct FactionColor {
        public int faction;
        public Color color;
    }

    public bool useDefaultLayout;

    public FactionColor[] factionColors;

    public Placement[] placements;

    public Color GetFactionColor(int faction) {
        foreach(FactionColor fc in factionColors) {
            if(fc.faction == faction) {
                return fc.color;
            }
        }

        return Color.magenta;
    }

    public void EnactDefaultLayout(Action<string, int, GamePiece.Type> Place) {
        Place("Alpha", 0, GamePiece.Type.KING);
        Place("A1", 0, GamePiece.Type.ROOK);
        Place("B1", 0, GamePiece.Type.KNIGHT);
        Place("C1", 0, GamePiece.Type.KNIGHT);
        Place("D1", 0, GamePiece.Type.ROOK);
        Place("E1", 0, GamePiece.Type.BISHOP);
        Place("F1", 0, GamePiece.Type.KNIGHT);
        Place("G1", 0, GamePiece.Type.KNIGHT);
        Place("H1", 0, GamePiece.Type.BISHOP);

        Place("A2", 0, GamePiece.Type.PAWN);
        Place("B2", 0, GamePiece.Type.PAWN);
        Place("C2", 0, GamePiece.Type.PAWN);
        Place("D2", 0, GamePiece.Type.PAWN);
        Place("E2", 0, GamePiece.Type.PAWN);
        Place("F2", 0, GamePiece.Type.PAWN);
        Place("G2", 0, GamePiece.Type.PAWN);
        Place("H2", 0, GamePiece.Type.PAWN);

        Place("-Alpha", 0, GamePiece.Type.QUEEN);
        Place("-A1", 0, GamePiece.Type.ROOK);
        Place("-B1", 0, GamePiece.Type.KNIGHT);
        Place("-C1", 0, GamePiece.Type.KNIGHT);
        Place("-D1", 0, GamePiece.Type.ROOK);
        Place("-E1", 0, GamePiece.Type.BISHOP);
        Place("-F1", 0, GamePiece.Type.KNIGHT);
        Place("-G1", 0, GamePiece.Type.KNIGHT);
        Place("-H1", 0, GamePiece.Type.BISHOP);

        Place("-A2", 0, GamePiece.Type.PAWN);
        Place("-B2", 0, GamePiece.Type.PAWN);
        Place("-C2", 0, GamePiece.Type.PAWN);
        Place("-D2", 0, GamePiece.Type.PAWN);
        Place("-E2", 0, GamePiece.Type.PAWN);
        Place("-F2", 0, GamePiece.Type.PAWN);
        Place("-G2", 0, GamePiece.Type.PAWN);
        Place("-H2", 0, GamePiece.Type.PAWN);

        Place("Omega", 1, GamePiece.Type.KING);
        Place("A8", 1, GamePiece.Type.ROOK);
        Place("B8", 1, GamePiece.Type.KNIGHT);
        Place("C8", 1, GamePiece.Type.KNIGHT);
        Place("D8", 1, GamePiece.Type.ROOK);
        Place("E8", 1, GamePiece.Type.BISHOP);
        Place("F8", 1, GamePiece.Type.KNIGHT);
        Place("G8", 1, GamePiece.Type.KNIGHT);
        Place("H8", 1, GamePiece.Type.BISHOP);

        Place("A7", 1, GamePiece.Type.PAWN);
        Place("B7", 1, GamePiece.Type.PAWN);
        Place("C7", 1, GamePiece.Type.PAWN);
        Place("D7", 1, GamePiece.Type.PAWN);
        Place("E7", 1, GamePiece.Type.PAWN);
        Place("F7", 1, GamePiece.Type.PAWN);
        Place("G7", 1, GamePiece.Type.PAWN);
        Place("H7", 1, GamePiece.Type.PAWN);

        Place("-Omega", 1, GamePiece.Type.QUEEN);
        Place("-A8", 1, GamePiece.Type.ROOK);
        Place("-B8", 1, GamePiece.Type.KNIGHT);
        Place("-C8", 1, GamePiece.Type.KNIGHT);
        Place("-D8", 1, GamePiece.Type.ROOK);
        Place("-E8", 1, GamePiece.Type.BISHOP);
        Place("-F8", 1, GamePiece.Type.KNIGHT);
        Place("-G8", 1, GamePiece.Type.KNIGHT);
        Place("-H8", 1, GamePiece.Type.BISHOP);

        Place("-A7", 1, GamePiece.Type.PAWN);
        Place("-B7", 1, GamePiece.Type.PAWN);
        Place("-C7", 1, GamePiece.Type.PAWN);
        Place("-D7", 1, GamePiece.Type.PAWN);
        Place("-E7", 1, GamePiece.Type.PAWN);
        Place("-F7", 1, GamePiece.Type.PAWN);
        Place("-G7", 1, GamePiece.Type.PAWN);
        Place("-H7", 1, GamePiece.Type.PAWN);
    }
}
