using UnityEngine;

public class Square : MonoBehaviour {
    public GameObject tile;
    public Color tileColor;

    public GameObject piece;
    public GameObject antiPiece;

    public bool flipside;

    //Gets or Sets piece based on whether the location is negative (flipside) or not
    public GameObject localPiece {
        get {
            return flipside ? antiPiece : piece;
        }

        set {
            if(flipside) {
                antiPiece = value;
            } else {
                piece = value;
            }
        }
    }

    //Returns piece based on whether the board is flipped or not
    public GameObject GetSurfacePiece(GameBoard board) {
        return board.isFlipside ? antiPiece : piece;
    }
}
