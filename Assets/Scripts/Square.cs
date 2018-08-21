using UnityEngine;

public class Square : MonoBehaviour {
    public GameObject tile;
    public Color tileColor;

    public GameObject piece;
    public GameObject antiPiece;

    public bool flipside;

    //Piece based on whether the location is negative (flipside) or not according to the referenced name (e.g. -A1 vs. A1)
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

    public string localName {
        get {
            return (flipside ? "-" : "") + name;
        }
    }

    ///<summary>Returns piece based on whether the global board is flipped or not</summary>
    public GameObject GetSurfacePiece(GameBoard board) {
        return board.isFlipside ? antiPiece : piece;
    }

    public bool HasPiece() {
        return piece || antiPiece;
    }

    ///<summary>Returns global surface piece if it exists, otherwise the other side</summary>
    public GameObject GetFavorablePiece(GameBoard board) {
        if(board.isFlipside) {
            return antiPiece ? antiPiece : piece;
        } else {
            return piece ? piece : antiPiece;
        }
    }

    public bool HasPieceFromFaction(int faction) {
        return (piece && piece.GetComponent<GamePiece>().faction == faction) || (antiPiece && antiPiece.GetComponent<GamePiece>().faction == faction);
    }
}
