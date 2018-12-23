using UnityEngine;

public class Tile : MonoBehaviour {
	[SerializeField] private string _location;
	[SerializeField] private Color _color;

	public string location { get { return _location; } }
	public Color color { get { return _color; } }

	/// <summary>Creates a new tile with a given color and location</summary>
	/// <param name="obj">Object to assign the Tile component to</param>
	/// <param name="location">Location that the tile is at</param>
	/// <param name="color">Default color for the tile</param>
	/// <returns>Returns the tile that has just been made</returns>
	/// <remarks>Should only be called once during its lifetime</remarks>
	public static Tile Create(GameObject obj, string location, Color color) {
		if(obj.GetComponent<Tile>()) {
			Debug.Log("Tile already created for " + location);

			return obj.GetComponent<Tile>();
		}

		Tile tile = obj.AddComponent<Tile>();
		tile._location = location;
		tile._color = color;

		return tile;
	}

	///<summary>Returns the normal tile object</summary>
	public GameObject GetPlane() {
		return transform.GetChild(0).gameObject;
	}

	///<summary>Returns the inverse tile object</summary>
	public GameObject GetInversePlane() {
		return transform.GetChild(1).gameObject;
	}

	///<summary>Set the normal tile color</summary>
	public void SetColor(Color c) {
		GetPlane().GetComponent<Renderer>().material.color = c;
	}

	///<summary>Sets the inverse tile color</summary>
	public void SetInverseColor(Color c) {
		GetInversePlane().GetComponent<Renderer>().material.color = c;
	}

	///<summary>Returns if the tile is on the axis of the sphere</summary>
	public bool IsAxial() {
		return IsAxial(_location);
	}

	///<summary>Returns if the tile location is on the axis of the sphere</summary>
	public static bool IsAxial(string location) {
		return location.Length > 3;
	}

	///<summary>Gets the piece on the normal side of the tile</summary>
	public Piece GetPiece() {
		if(GetPlane().transform.childCount > 0) {
			return GetPlane().transform.GetChild(0).gameObject.GetComponent<Piece>();
		}

		return null;
	}

	///<summary>Gets the piece on the inverse side of the tile</summary>
	public Piece GetInversePiece() {
		if(GetInversePlane().transform.childCount > 0) {
			return GetInversePlane().transform.GetChild(0).gameObject.GetComponent<Piece>();
		}

		return null;
	}
}