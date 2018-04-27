using UnityEngine;

[RequireComponent(typeof(BoardGen))]
public class GameBoard : MonoBehaviour {
    [System.Serializable]
    public class Square {
        public string name;
        public GameObject obj;
        public GameObject piece;

        public Square(string name, GameObject obj) {
            this.name = name;
            this.obj = obj;
        }
    }

    public GameObject pawnPrefab,
                     knightPrefab,
                     bishopPrefab,
                     rookPrefab,
                     queenPrefab,
                     kingPrefab;

    public Square[] squares;

    void Start() {
        BoardGen gen = GetComponent<BoardGen>();

        gen.genCompleteEvent.AddListener(() => {
            squares = new Square[gen.gameObject.transform.childCount];

            for(int i = 0; i < gen.gameObject.transform.childCount; i++) {
                GameObject square = gen.transform.GetChild(i).gameObject;
                squares[i] = new Square(square.name, square);
            }

            SetupPieces();
        });
    }

    void SetupPieces() {
        SetupPiece("A1", GamePiece.Type.ROOK);
    }

    void SetupPiece(string location, GamePiece.Type type) {
        GameObject piece = null;

        switch(type) {
            case GamePiece.Type.PAWN:
                piece = Instantiate(pawnPrefab);
                break;
            case GamePiece.Type.KNIGHT:
                piece = Instantiate(knightPrefab);
                break;
            case GamePiece.Type.BISHOP:
                piece = Instantiate(bishopPrefab);
                break;
            case GamePiece.Type.ROOK:
                piece = Instantiate(rookPrefab);
                break;
            case GamePiece.Type.QUEEN:
                piece = Instantiate(queenPrefab);
                break;
            case GamePiece.Type.KING:
                piece = Instantiate(kingPrefab);
                break;
            default:
                break;
        }

        Square square = GetSquare(location);

        if(square != null) {
            piece.AddComponent<GamePiece>();
            piece.GetComponent<GamePiece>().type = type;

            Vector3 scale = piece.transform.localScale;

            piece.transform.SetParent(square.obj.transform);
            piece.transform.localPosition = Vector3.zero;
            piece.transform.localRotation = Quaternion.identity;

            piece.transform.parent = gameObject.transform;
            piece.transform.localScale = scale;
            piece.transform.localPosition += piece.transform.forward * scale.x * 0.5f;

            square.piece = piece;
        }
    }

    Square GetSquare(string name) {
        foreach(Square square in squares) {
            if(square.name == name) {
                return square;
            }
        }

        return null;
    }
}
