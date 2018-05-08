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

    public FactionColor[] factionColors = {
        new FactionColor() {
            faction = 1,
            color = Color.white
        },

        new FactionColor() {
            faction = 2,
            color = Color.black
        }
    };

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
        Place("Alpha", 1, GamePiece.Type.KING);
        Place("A1", 1, GamePiece.Type.ROOK);
        Place("B1", 1, GamePiece.Type.KNIGHT);
        Place("C1", 1, GamePiece.Type.KNIGHT);
        Place("D1", 1, GamePiece.Type.ROOK);
        Place("E1", 1, GamePiece.Type.BISHOP);
        Place("F1", 1, GamePiece.Type.KNIGHT);
        Place("G1", 1, GamePiece.Type.KNIGHT);
        Place("H1", 1, GamePiece.Type.BISHOP);

        Place("A2", 1, GamePiece.Type.PAWN);
        Place("B2", 1, GamePiece.Type.PAWN);
        Place("C2", 1, GamePiece.Type.PAWN);
        Place("D2", 1, GamePiece.Type.PAWN);
        Place("E2", 1, GamePiece.Type.PAWN);
        Place("F2", 1, GamePiece.Type.PAWN);
        Place("G2", 1, GamePiece.Type.PAWN);
        Place("H2", 1, GamePiece.Type.PAWN);

        Place("-Alpha", 1, GamePiece.Type.QUEEN);
        Place("-A1", 1, GamePiece.Type.ROOK);
        Place("-B1", 1, GamePiece.Type.KNIGHT);
        Place("-C1", 1, GamePiece.Type.KNIGHT);
        Place("-D1", 1, GamePiece.Type.ROOK);
        Place("-E1", 1, GamePiece.Type.BISHOP);
        Place("-F1", 1, GamePiece.Type.KNIGHT);
        Place("-G1", 1, GamePiece.Type.KNIGHT);
        Place("-H1", 1, GamePiece.Type.BISHOP);

        Place("-A2", 1, GamePiece.Type.PAWN);
        Place("-B2", 1, GamePiece.Type.PAWN);
        Place("-C2", 1, GamePiece.Type.PAWN);
        Place("-D2", 1, GamePiece.Type.PAWN);
        Place("-E2", 1, GamePiece.Type.PAWN);
        Place("-F2", 1, GamePiece.Type.PAWN);
        Place("-G2", 1, GamePiece.Type.PAWN);
        Place("-H2", 1, GamePiece.Type.PAWN);

        Place("Omega", 2, GamePiece.Type.KING);
        Place("A8", 2, GamePiece.Type.ROOK);
        Place("B8", 2, GamePiece.Type.KNIGHT);
        Place("C8", 2, GamePiece.Type.KNIGHT);
        Place("D8", 2, GamePiece.Type.ROOK);
        Place("E8", 2, GamePiece.Type.BISHOP);
        Place("F8", 2, GamePiece.Type.KNIGHT);
        Place("G8", 2, GamePiece.Type.KNIGHT);
        Place("H8", 2, GamePiece.Type.BISHOP);

        Place("A7", 2, GamePiece.Type.PAWN);
        Place("B7", 2, GamePiece.Type.PAWN);
        Place("C7", 2, GamePiece.Type.PAWN);
        Place("D7", 2, GamePiece.Type.PAWN);
        Place("E7", 2, GamePiece.Type.PAWN);
        Place("F7", 2, GamePiece.Type.PAWN);
        Place("G7", 2, GamePiece.Type.PAWN);
        Place("H7", 2, GamePiece.Type.PAWN);

        Place("-Omega", 2, GamePiece.Type.QUEEN);
        Place("-A8", 2, GamePiece.Type.ROOK);
        Place("-B8", 2, GamePiece.Type.KNIGHT);
        Place("-C8", 2, GamePiece.Type.KNIGHT);
        Place("-D8", 2, GamePiece.Type.ROOK);
        Place("-E8", 2, GamePiece.Type.BISHOP);
        Place("-F8", 2, GamePiece.Type.KNIGHT);
        Place("-G8", 2, GamePiece.Type.KNIGHT);
        Place("-H8", 2, GamePiece.Type.BISHOP);

        Place("-A7", 2, GamePiece.Type.PAWN);
        Place("-B7", 2, GamePiece.Type.PAWN);
        Place("-C7", 2, GamePiece.Type.PAWN);
        Place("-D7", 2, GamePiece.Type.PAWN);
        Place("-E7", 2, GamePiece.Type.PAWN);
        Place("-F7", 2, GamePiece.Type.PAWN);
        Place("-G7", 2, GamePiece.Type.PAWN);
        Place("-H7", 2, GamePiece.Type.PAWN);
    }
}
