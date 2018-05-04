using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(BoardGen))]
public class GameBoard : MonoBehaviour {
    public GameObject pawnPrefab, knightPrefab, bishopPrefab, rookPrefab, queenPrefab, kingPrefab;

    public BoardLayout boardLayout;

    public int factions;
    public List<GamePiece>[] factionJails;

    [HideInInspector]
    public GameObject nameContainer;
    public bool showSquareNames = true;
    public float nameHeightOffset = 1f;
    public Color squareNameColor = Color.blue;
    public Material squareMaterial;

    void SetupPieces() {
        foreach(BoardLayout.Placement place in boardLayout.placements) {
            Place(place.square, place.faction, place.piece);
        }
    }

    public void Create() {
        BoardGen gen = GetComponent<BoardGen>();
        nameContainer = null;

        GameRules.maxRow = GameRules.minRow + gen.pitchSteps - 1;
        GameRules.maxColumn = (char)(GameRules.minColumn + gen.yawSteps - 1);

        factionJails = new List<GamePiece>[factions];
        for(int i = 0; i < factions; i++) {
			factionJails[i] = new List<GamePiece>();
		}

        if(showSquareNames) {
            nameContainer = new GameObject();
            nameContainer.name = "Names";
            nameContainer.transform.parent = null;
        }

        for(int i = 0; i < gen.gameObject.transform.childCount; i++) {
            Square square = gen.transform.GetChild(i).gameObject.AddComponent<Square>();
            square.tile = square.gameObject.transform.GetChild(0).gameObject;
            square.tile.name = "Tile";
            square.tileColor = square.tile.GetComponent<Renderer>().material.color;
            square.tile.GetComponent<Renderer>().material = squareMaterial;
            square.tile.GetComponent<Renderer>().material.color = square.tileColor;

            if(showSquareNames) {
                GameObject nameRender = new GameObject();
                nameRender.transform.localScale = Vector3.one;
                nameRender.transform.position = square.gameObject.transform.position + square.gameObject.transform.up * nameHeightOffset;
                nameRender.transform.localRotation = Quaternion.identity;

                TextMesh textMesh = nameRender.AddComponent<TextMesh>();
                textMesh.text = square.gameObject.name;
                textMesh.characterSize = 0.01f;
                textMesh.fontSize = 350;
                textMesh.anchor = TextAnchor.MiddleCenter;

                nameRender.GetComponent<Renderer>().material.shader = Shader.Find("GUI/3D Text");
                nameRender.GetComponent<Renderer>().material.color = squareNameColor;

                nameRender.name = "Name for " + square.gameObject.name;
                nameRender.transform.parent = nameContainer.transform;
            }
        }

        if(showSquareNames) {
            nameContainer.transform.parent = transform;
            nameContainer.transform.localPosition = Vector3.zero;
            nameContainer.transform.localRotation = Quaternion.identity;
        }

        SetupPieces();
    }

    void Update() {
        if(showSquareNames) {
            for(int i = 0; i < nameContainer.transform.childCount; i++) {
                nameContainer.transform.GetChild(i).transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position, Camera.main.transform.up);
            }
        }
    }

    public object[] Remove(string location) {
        Square square = GetSquare(location);

        if(square != null && square.piece != null) {
            object[] data = {square.piece.GetComponent<GamePiece>().type, square.piece.GetComponent<GamePiece>().faction};

            Destroy(square.piece);
            square.piece = null;

            return data;
        }

        return new object[0];
    }

    public void Place(string location, GamePiece piece) {
        Place(location, piece.faction, piece.type);
    }

    public void Place(string location, int faction, GamePiece.Type type) {
        GameObject prefab = null, piece = null;

        switch(type) {
            case GamePiece.Type.PAWN:
                prefab = pawnPrefab;
                break;
            case GamePiece.Type.KNIGHT:
                prefab = knightPrefab;
                break;
            case GamePiece.Type.BISHOP:
                prefab = bishopPrefab;
                break;
            case GamePiece.Type.ROOK:
                prefab = rookPrefab;
                break;
            case GamePiece.Type.QUEEN:
                prefab = queenPrefab;
                break;
            case GamePiece.Type.KING:
                prefab = kingPrefab;
                break;
        }

        piece = Instantiate(prefab);
        piece.name = prefab.name;

        Square square = GetSquare(location);

        if(square != null) {
            GamePiece gp = piece.AddComponent<GamePiece>();
            gp.type = type;
            gp.faction = faction;
            gp.direction = faction == 1 ? GamePiece.Direction.DOWN : GamePiece.Direction.UP;
            gp.SetColor();

            piece.transform.SetParent(square.gameObject.transform);
            piece.transform.localRotation = faction == 1 ? Quaternion.identity : Quaternion.Euler(Vector3.up * 180f);
            piece.transform.localPosition = Vector3.zero;

            square.piece = piece;
        }
    }

    public bool Move(string initial, string final, GamePiece piece) {
        Square square = GetSquare(final);

        if(square != null) {
            GetSquare(initial).piece = null;

            if(square.piece != null) {
                object[] data = Remove(final);
                Debug.Log("Faction " + data[1] + "'s " + data[0] + " was captured by faction " + piece.faction + "'s " + piece.type);
            }

            piece.transform.SetParent(square.gameObject.transform);
            piece.transform.localRotation = piece.faction == 1 ? Quaternion.identity : Quaternion.Euler(Vector3.up * 180f);
            piece.transform.localPosition = Vector3.zero;

            square.piece = piece.gameObject;

            return true;
        }

        return false;
    }

    public Square GetSquare(string name) {
        if(name == "Alpha" || name == "Omega") {
            return transform.Find(name).gameObject.GetComponent<Square>();
        }

        int startIndex = transform.Find("Alpha").GetSiblingIndex() + 1;
        int index = startIndex + (int.Parse("" + name[1]) - 1) * GameRules.maxRow + (name[0] - GameRules.minColumn);

        return transform.GetChild(index).gameObject.GetComponent<Square>();
    }
}
