using UnityEngine;

//PAWN MUST BECOME SOMETHING ELSE AT ALPHA/OMEGA DUE TO NATURE OF MOVEMENT LIMITING TO ONLY FORWARDS (UP/DOWN)
public class GamePiece : MonoBehaviour {
    public enum Type {
        PAWN,
        KNIGHT,
        BISHOP,
        ROOK,
        QUEEN,
        KING
    }

    public enum Direction {
        UP, DOWN, LEFT, RIGHT,
        UP_LEFT, UP_RIGHT, DOWN_LEFT, DOWN_RIGHT
    }

    public Type type = Type.PAWN;
    public Direction direction = Direction.DOWN;

    //0 none, 1 white, 2 black
    public int faction = 0;

    public void SetColor(BoardLayout layout = null) {
        Color color = Color.magenta;

        if(layout) {
            color = layout.GetFactionColor(faction);
        } else {
    		switch(faction) {
    			case 1:
    				color = Color.white;
    				break;
    			case 2:
    				color = Color.black;
    				break;
    		}
        }

		pieceColor = color;
    }

    public Color pieceColor {
        set {
            gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material.color = value;
        }

        get {
            return gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material.color;
        }
    }

    public string GetSquare() {
        return transform.parent.gameObject.name;
    }

    public bool RequiresDirectPath() {
        return type == Type.KNIGHT ? false : true;
    }

    public Quaternion GetAppropriateRotation(bool flipside) {
        return Quaternion.Euler(flipside ? new Vector3(180f, faction % 2 == 0 ? 180f : 0f, 0f) : new Vector3(0, faction % 2 == 0 ? 0 : 180f, 0f));
    }
}
