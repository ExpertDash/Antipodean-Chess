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

    public void SetColor() {
        Color color = Color.magenta;

		switch(faction) {
			case 1:
				color = Color.white;
				break;
			case 2:
				color = Color.black;
				break;
		}

		gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material.color = color;
    }
}
