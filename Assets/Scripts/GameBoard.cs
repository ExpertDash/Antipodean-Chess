using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(BoardGen))]
public class GameBoard : MonoBehaviour {
    public GameObject pawnPrefab, knightPrefab, bishopPrefab, rookPrefab, queenPrefab, kingPrefab;

    public BoardLayout boardLayout;
    public BoardGen boardGen {
        get {
            return GetComponent<BoardGen>();
        }
    }

    public bool isFlipside;
    [HideInInspector] public bool finishedFlipAnimation;
    public Color normalSkybox = Color.gray, flipsideSkybox = Color.black;

    public int factions;
    public Jail jail;//public List<GamePiece>[] factionJails;

    [HideInInspector]
    public GameObject nameContainer;
    public bool showSquareNames = true;
    public float nameHeightOffset = 1f;
    public Color squareNameColor = Color.blue;
    public Material squareMaterial;

    void SetupPieces() {
        if(boardLayout.useDefaultLayout) {
            boardLayout.EnactDefaultLayout(Place);
        } else {
            foreach(BoardLayout.Placement place in boardLayout.placements) {
                Place(place.square, place.faction, place.piece);
            }
        }
    }

    public void Create() {
        Camera.main.backgroundColor = normalSkybox;
        nameContainer = null;

        GameRules.maxRow = GameRules.minRow + boardGen.pitchSteps - 1;
        GameRules.maxColumn = (char)(GameRules.minColumn + boardGen.yawSteps - 1);
/*
        factionJails = new List<GamePiece>[factions];
        for(int i = 0; i < factions; i++) {
			factionJails[i] = new List<GamePiece>();
		}
*/
        nameContainer = new GameObject();
        nameContainer.name = "Names";
        nameContainer.transform.parent = null;

        for(int i = 0; i < boardGen.gameObject.transform.childCount; i++) {
            Square square = boardGen.transform.GetChild(i).gameObject.AddComponent<Square>();
            square.tile = square.gameObject.transform.GetChild(0).gameObject;
            square.tile.name = "Tile";
            square.tileColor = square.tile.GetComponent<Renderer>().material.color;
            square.tile.GetComponent<Renderer>().material = squareMaterial;
            square.tile.GetComponent<Renderer>().material.color = square.tileColor;

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

        nameContainer.transform.parent = transform;
        nameContainer.transform.localPosition = Vector3.zero;
        nameContainer.transform.localRotation = Quaternion.identity;

        ToggleSquareNames(showSquareNames);

        SetupPieces();

        finishedFlipAnimation = true;
    }

    public void Flipside() {
        if(finishedFlipAnimation) {
            finishedFlipAnimation = false;
            isFlipside = !isFlipside;
            StartCoroutine((AnimateFlipside(1f, 10f)));
        }
    }

    IEnumerator AnimateFlipside(float rowDelay, float speed) {
        StartCoroutine(AnimateColorChange(1f));

        for(float i = 0f; i < 180f / speed; i++) {
            foreach(Square square in GetSquares()) {
                square.transform.Rotate(0, speed, 0);
            }

            yield return null;
        }

        StartCoroutine(AnimateFlipsideSquare(speed, "Alpha", false));
        yield return new WaitForSeconds(rowDelay / speed);

        for(int x = GameRules.minRow; x <= GameRules.maxRow; x++) {
            StartCoroutine(AnimateFlipsideRow(speed, x));
            yield return new WaitForSeconds(rowDelay / speed);
        }

        StartCoroutine(AnimateFlipsideSquare(speed, "Omega", true));
        yield return new WaitForSeconds(rowDelay / speed);
    }

    IEnumerator AnimateColorChange(float duration) {
        Color initial = isFlipside ? normalSkybox : flipsideSkybox;
        Color final = isFlipside ? flipsideSkybox : normalSkybox;

        float dR = (final.r - initial.r) / duration;
        float dG = (final.g - initial.g) / duration;
        float dB = (final.b - initial.b) / duration;

        Camera.main.backgroundColor = initial;

        Color c = initial;

        for(float i = 0; i < duration; i += Time.deltaTime) {
            c = new Color(c.r + dR * Time.deltaTime, c.g + dG * Time.deltaTime, c.b + dB * Time.deltaTime);
            Camera.main.backgroundColor = c;

            yield return null;
        }

        Camera.main.backgroundColor = final;
    }

    IEnumerator AnimateFlipsideSquare(float speed, string location, bool end) {
        for(float i = 0f; i < 180f / speed; i++) {
            GetSquare(location).transform.Rotate(speed, 0f, 0f);
            yield return null;
        }

        if(end) {
            finishedFlipAnimation = true;
        }
    }

    IEnumerator AnimateFlipsideRow(float speed, int x) {
        for(float i = 0f; i < 180f / speed; i++) {
            for(char y = GameRules.minColumn; y <= GameRules.maxColumn; y++) {
                GetSquare("" + y + x).transform.Rotate(speed, 0f, 0f);
            }

            yield return null;
        }
    }

    IEnumerator SwapAnimate(float speed, string location) {
        finishedFlipAnimation = false;

        Square square =  GetSquare(location);
        Quaternion rot = square.transform.localRotation;

         for(float i = 0f; i < 180f / speed; i++) {
            square.transform.Rotate(0, speed, 0);
            yield return null;
        }

        for(float i = 0f; i < 180f / speed; i++) {
            square.transform.Rotate(speed, 0f, 0f);
            yield return null;
        }

        square.transform.localRotation = rot;

        GameObject p = GetSquare(location).piece;
        GameObject ap = GetSquare(location).antiPiece;
        
        Remove(location);
        Remove("-" + location);

        if(ap) Place(location, ap.GetComponent<GamePiece>().faction, ap.GetComponent<GamePiece>().type);
        if(p) Place("-" + location, p.GetComponent<GamePiece>().faction, p.GetComponent<GamePiece>().type);

        finishedFlipAnimation = true;
    }

    public void ToggleSquareNames(bool state) {
        showSquareNames = state;

        for(int i = 0; i < nameContainer.transform.childCount; i++) {
            nameContainer.transform.GetChild(i).gameObject.SetActive(showSquareNames);
        }
    }

    void Update() {
        if(showSquareNames && nameContainer) {
            for(int i = 0; i < nameContainer.transform.childCount; i++) {
                nameContainer.transform.GetChild(i).transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position, Camera.main.transform.up);
            }
        }
    }

    public object[] Remove(string location) {
        Square square = GetSquare(location);

        if(square != null && square.localPiece != null) {
            object[] data = {square.localPiece.GetComponent<GamePiece>().type, square.localPiece.GetComponent<GamePiece>().faction};

            Destroy(square.localPiece);
            square.localPiece = null;

            return data;
        }

        return new object[0];
    }

    public GamePiece Capture(string location, GamePiece attacker) {
        Square square = GetSquare(location);
        GameObject piece = square.localPiece;
        GamePiece gp = piece.GetComponent<GamePiece>();

        //factionJails[attacker.faction].Add(gp);
        square.localPiece = null;

        Debug.Log("Faction " + gp.faction + "'s " + gp.type + " was captured by faction " + attacker.faction + "'s " + attacker.type);

        /*
        piece.transform.parent = null;
        piece.transform.position = Vector3.up * 20f;
        piece.transform.localRotation = Quaternion.identity;
        */

        StartCoroutine(CaptureAnimation(gp));

        return gp;
    }

    IEnumerator CaptureAnimation(GamePiece piece) {
        float speed = 0.5f;
        float dist = 0f;
        float dest = 2f;

        Color c = piece.pieceColor;

        while(piece.pieceColor.a > 0) {
            piece.gameObject.transform.position += piece.gameObject.transform.up * speed;

            dist += speed;
            speed *= 0.85f;

            piece.pieceColor = new Color(piece.pieceColor.r, piece.pieceColor.g, piece.pieceColor.b, (dest - dist) / dest);
            
            yield return null;
        }

        piece.pieceColor = c;//Destroy(piece.gameObject);
        jail.Add(piece);
    }

    public void Swap(string location) {
        if(finishedFlipAnimation) {
            StartCoroutine(SwapAnimate(10f, location));
        }
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
            gp.direction = faction % 2 == 0 ? GamePiece.Direction.DOWN : GamePiece.Direction.UP;
            gp.SetColor(boardLayout);

            piece.transform.SetParent(square.gameObject.transform);
            piece.transform.localPosition = Vector3.zero;
            piece.transform.localRotation = gp.GetAppropriateRotation(square.flipside);
        }

        square.localPiece = piece;
    }

    public GamePiece Move(string initial, string final, GamePiece piece) {
        GamePiece captured = null;
        Square square = GetSquare(final);

        if(square != null) {
            if(initial.Length > 0) GetSquare(initial).localPiece = null;

            if(square.localPiece != null) {
                captured = Capture(final, piece);
            }

            piece.transform.SetParent(square.gameObject.transform);
            piece.transform.localRotation = piece.GetAppropriateRotation(isFlipside);
            piece.transform.localPosition = Vector3.zero;

            square.localPiece = piece.gameObject;

            //return true;
        }

        return captured; //return false;
    }

    public Square GetSquare(string name) {
        bool flipside = false;

        if(name[0] == '-') {
            name = name.Substring(1, name.Length - 1);
            flipside = true;
        }

        if(name == "Alpha" || name == "Omega") {
            Square earlySquare = transform.Find(name).gameObject.GetComponent<Square>();
            earlySquare.flipside = flipside;

            return earlySquare;
        }

        int startIndex = transform.Find("Alpha").GetSiblingIndex() + 1;
        int index = startIndex + (int.Parse("" + name[1]) - 1) * GameRules.maxRow + (name[0] - GameRules.minColumn);

        Square square = transform.GetChild(index).gameObject.GetComponent<Square>();
        square.flipside = flipside;

        return square;
    }

    public Square[] GetSquares() {
        List<Square> squares = new List<Square>();

        int startIndex = transform.Find("Alpha").GetSiblingIndex() + 1;
        int endIndex = startIndex + (int.Parse("" + GameRules.maxRow) - 1) * GameRules.maxRow + (GameRules.maxColumn - GameRules.minColumn);

        squares.Add(transform.GetChild(startIndex - 1).gameObject.GetComponent<Square>());
        for(int i = startIndex; i <= endIndex; i++) {
            squares.Add(transform.GetChild(i).gameObject.GetComponent<Square>());
        }
        squares.Add(transform.GetChild(endIndex + 1).gameObject.GetComponent<Square>());

        return squares.ToArray();
    }

    public string[] GetSquares(GamePiece.Type type) {
        List<string> squares = new List<string>();

        foreach(Square square in GetSquares()) {
            if(square.piece && square.piece.GetComponent<GamePiece>().type == type) squares.Add(square.name);
            if(square.antiPiece && square.antiPiece.GetComponent<GamePiece>().type == type) squares.Add("-" + square.name);
        }

        return squares.ToArray();
    }

    public string[] GetSquares(GamePiece.Type type, int faction) {
        return GetSquares(type).Where(s => GetSquare(s).localPiece.GetComponent<GamePiece>().faction == faction).ToArray();
    }
}
