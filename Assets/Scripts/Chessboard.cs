using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

/// <summary>Provides access to the tiles and pieces based on location</summary>
public class Chessboard : MonoBehaviour {
	public enum Style { ANTIPODEAN, SPHERE }

	private Dictionary<string, Tile> tiles;
	[HideInInspector] public bool isInverted = false;
	private GameObject tileNames;
	private Style _style;
	public Style style { get { return _style; } }

	/// <summary>Creates tiles and places pieces</summary>
	public void Create(Style style) {
		_style = style;
		tiles = new Dictionary<string, Tile>();

		tileNames = new GameObject();
		tileNames.SetActive(false);
		tileNames.name = "Names";
		tileNames.transform.parent = null;

		StartCoroutine(Generate());
	}

	/// <summary>Removes all tiles and pieces</summary>
	public void Clear() {
		Camera.main.backgroundColor = BoardConfig.visuals.normalSkyboxColor;

		for(int i = 0; i < transform.childCount; i++) Destroy(transform.GetChild(i).gameObject);
		tiles.Clear();

		isInverted = false;
	}

	/// <summary>Toggles the visibility of the names displayed above the tiles</summary>
	public void ToggleTileNames() {
		tileNames.SetActive(!tileNames.activeSelf);
	}

	/// <param name="location">Location to check</param>
	/// <returns>Returns whether the location is inverse</returns>
	public bool IsLocationInverse(string location) {
		return location[0] == '-';
	}

	/// <param name="location">Location to find a tile at</param>
	/// <returns>Returns the tile that is tied to the location</returns>
	public Tile GetTile(string location) {
		string key = IsLocationInverse(location) ? location.Substring(1) : location;
		if(tiles.ContainsKey(key)) return tiles[key];

		return null;
	}

	/// <param name="location">Location to find a plane at</param>
	/// <returns>Returns the GameObject that the location is tied</returns>
	/// <remarks>A tile is made of a top and bottom plane. This returns the bottom plane if the location is inverse and top otherwise</remarks>
	public GameObject GetPlane(string location) {
		string key = IsLocationInverse(location) ? location.Substring(1) : location;
		if(tiles.ContainsKey(key)) return IsLocationInverse(location) ? tiles[key].GetInversePlane() : tiles[key].GetPlane();

		return null;
	}

	/// <param name="location">Location to find a piece at</param>
	/// <returns>Returns the piece at the location</returns>
	public Piece GetPiece(string location) {
		Tile tile = GetTile(location);
		if(tile) return IsLocationInverse(location) ? tile.GetInversePiece() : tile.GetPiece();

		return null;
	}

	/// <returns>All the tiles of the board</returns>
	public ICollection<Tile> GetTiles() {
		return tiles.Values;
	}

	void Update() {
		//Ensures that tile names rotate to face the camera when the parent is active
		if(tileNames && tileNames.activeSelf) {
			for(int i = 0; i < tileNames.transform.childCount; i++) {
				tileNames.transform.GetChild(i).transform.rotation = Quaternion.LookRotation(-Camera.main.transform.position, Camera.main.transform.up);
			}
		}
	}

	/// <summary>Generates the tiles for the board as children of this GameObject</summary>
	IEnumerator Generate() {
		//Determine settings
		int rows = BoardConfig.rows,
			columns = BoardConfig.columns + 1;

		float rowDelta = (2f * Mathf.PI) / (float)rows,
			  columnDelta = Mathf.PI / (float)columns;

		float radius = BoardConfig.generation.radius;

		//Used to get position for tiles based on row, column, and deltas
		Func<float, float, Vector3> GetCoords = delegate(float yaw, float pitch) {
			float x, y, z;

			x = radius * Mathf.Cos(yaw) * Mathf.Sin(pitch);
			z = radius * Mathf.Sin(yaw) * Mathf.Sin(pitch);
			y = radius * Mathf.Cos(pitch);

			return new Vector3(x, y, z);
		};

		//Generate Alpha tile
		if(style == Style.ANTIPODEAN) {
			GenerateTile("Alpha", BoardConfig.visuals.axialTileColor, GetCoords(0, 0));

			//Delay Alpha tile
			if(BoardConfig.visuals.generateProcedurally) {
				yield return new WaitForSeconds(BoardConfig.visuals.generationDelay);
			}
		}

		//Generate tiles
		bool state = false;
		for(int column = 1; column < columns; column++, state = !state) for(int row = 0; row < rows; row++, state = !state) {
			GenerateTile(
				"" + (char)(BoardConfig.initialColumn + row) + (BoardConfig.initialRow + column - 1),
				state ? BoardConfig.visuals.primaryTileColor : BoardConfig.visuals.secondaryTileColor,
				GetCoords(row * rowDelta, column * columnDelta)
			);

			if(BoardConfig.visuals.generateProcedurally) {
				yield return new WaitForSeconds(BoardConfig.visuals.generationDelay);
			}
		}

		//Generate Omega tile
		if(style == Style.ANTIPODEAN) GenerateTile("Omega", BoardConfig.visuals.axialTileColor, GetCoords(0, Mathf.PI));

		AddTileNames();
		Layout.EnactDefaultLayout(style);
	}

	/// <summary>Generates a single tile at a position in space with the specified color and given board position</summary>
	void GenerateTile(string boardPosition, Color color, Vector3 spatialPosition) {
		//Creat the tile
		Tile tile = Tile.Create(new GameObject(boardPosition), boardPosition, color);

		//Set position and rotation
		tile.transform.parent = transform;
		tile.transform.localPosition = spatialPosition;
		tile.transform.localRotation = Quaternion.LookRotation(spatialPosition);
		tile.transform.Rotate(90f, 0f, 0f);

		//Determine the scale at which tiles should be shown
		float scaleFactor = 1f,
			  baseline = 6f,
			  distance = BoardConfig.generation.radius - Mathf.Abs(tile.transform.localPosition.y),
			  halfThickness = BoardConfig.generation.thickness * 0.5f;

		if(distance != 0) {
			float distanceUnit = distance / BoardConfig.generation.radius * baseline;
			scaleFactor = Mathf.Log(4 + distanceUnit + Mathf.Pow(distanceUnit, 2f), 10f);
		}

		Vector3 scale = new Vector3(scaleFactor, halfThickness, scaleFactor);

		//Create the visible top tile
		GameObject normalTile = Instantiate(BoardConfig.prefabs.tile);
		normalTile.name = tile.name;
		normalTile.transform.parent = tile.transform;
		normalTile.transform.localPosition = Vector3.up * halfThickness * 0.5f;
		normalTile.transform.localRotation = Quaternion.identity;
		normalTile.transform.localScale = scale;

		//Create the visible bottom tile
		GameObject inverseTile = Instantiate(BoardConfig.prefabs.tile);
		inverseTile.name = "-" + tile.name;
		inverseTile.transform.parent = tile.transform;
		inverseTile.transform.localPosition = Vector3.down * halfThickness * 0.5f;
		inverseTile.transform.localRotation = Quaternion.identity;
		inverseTile.transform.localScale = scale;

		//Set the tile colors
		tile.SetColor(color);
		tile.SetInverseColor(color);

		//Add to dictionary
		tiles.Add(boardPosition, tile);
	}

	/// <summary>Creates the GameObjects for names for each tiles</summary>
	void AddTileNames() {
		foreach(KeyValuePair<string, Tile> tile in tiles) {
			GameObject tileName = new GameObject();
			tileName.transform.localScale = Vector3.one;
			tileName.transform.position = tile.Value.gameObject.transform.position + tile.Value.gameObject.transform.up * BoardConfig.visuals.tileNameHeightOffset;
			tileName.transform.localRotation = Quaternion.identity;

			TextMesh textMesh = tileName.AddComponent<TextMesh>();
			textMesh.text = tile.Value.gameObject.name;
			textMesh.characterSize = 0.01f;
			textMesh.fontSize = 350;
			textMesh.anchor = TextAnchor.MiddleCenter;

			tileName.GetComponent<Renderer>().material.shader = Shader.Find("GUI/3D Text");
			tileName.GetComponent<Renderer>().material.color = BoardConfig.visuals.tileNameColor;

			tileName.name = "N:" + tile.Value.gameObject.name;
			tileName.transform.parent = tileNames.transform;
		}

		tileNames.transform.parent = transform;
		tileNames.transform.localPosition = Vector3.zero;
		tileNames.transform.localRotation = Quaternion.identity;
	}
}