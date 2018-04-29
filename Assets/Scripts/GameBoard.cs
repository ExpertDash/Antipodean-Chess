using UnityEngine;

[RequireComponent(typeof(BoardGen))]
public class GameBoard : MonoBehaviour {
    [System.Serializable]
    public class Square {
        public string name;
        public GameObject obj;
        public GameObject tile;
        public GameObject piece;
        public GameObject antiPiece;

        public Square(string name, GameObject obj, GameObject tile) {
            this.name = name;
            this.obj = obj;
            this.tile = tile;
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
                squares[i] = new Square(square.name, square, square.transform.GetChild(0).gameObject);
            }

            SetupPieces();
        });
    }

    void SetupPieces() {
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


        /*
        object[] data = Remove("Alpha");
        Debug.Log(data[0] + "_" + data[1]);
        */
    }

    object[] Remove(string location) {
        Square square = GetSquare(location);

        if(square != null && square.piece != null) {
            object[] data = {square.piece.GetComponent<GamePiece>().type, square.piece.GetComponent<GamePiece>().faction};

            Destroy(square.piece);
            square.piece = null;

            return data;
        }

        return new object[0];
    }

    void Place(string location, int faction, GamePiece.Type type) {
        GameObject piece = null;

        switch(type) {
            case GamePiece.Type.PAWN:
                piece = Instantiate(pawnPrefab);
                piece.name = pawnPrefab.name;
                break;
            case GamePiece.Type.KNIGHT:
                piece = Instantiate(knightPrefab);
                piece.name = knightPrefab.name;
                break;
            case GamePiece.Type.BISHOP:
                piece = Instantiate(bishopPrefab);
                piece.name = bishopPrefab.name;
                break;
            case GamePiece.Type.ROOK:
                piece = Instantiate(rookPrefab);
                piece.name = rookPrefab.name;
                break;
            case GamePiece.Type.QUEEN:
                piece = Instantiate(queenPrefab);
                piece.name = queenPrefab.name;
                break;
            case GamePiece.Type.KING:
                piece = Instantiate(kingPrefab);
                piece.name = kingPrefab.name;
                break;
            default:
                break;
        }

        Square square = GetSquare(location);

        if(square != null) {
            piece.AddComponent<GamePiece>();
            piece.GetComponent<GamePiece>().type = type;
            piece.GetComponent<GamePiece>().faction = faction;
            piece.GetComponent<GamePiece>().SetColor();

            piece.transform.SetParent(square.obj.transform);
            piece.transform.localRotation = faction == 1 ? Quaternion.identity : Quaternion.Euler(Vector3.up * 180f);
            piece.transform.localPosition = Vector3.zero;

            square.piece = piece;
        }
    }

    public Square GetSquare(string name) {
        foreach(Square square in squares) {
            if(square.name == name) {
                return square;
            }
        }

        return null;
    }
}
