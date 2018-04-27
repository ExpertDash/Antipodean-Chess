using UnityEngine;

public class GamePiece : MonoBehaviour {
    public enum Type {
        PAWN,
        KNIGHT,
        BISHOP,
        ROOK,
        QUEEN,
        KING
    }

    public Type type = Type.PAWN;
    public int faction = 0;
}
