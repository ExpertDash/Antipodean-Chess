using UnityEngine;

public class Piece : MonoBehaviour {
	public enum Type { PAWN, KNIGHT, BISHOP, ROOK, QUEEN, KING }

	public Team team;
	public Type type;

	//Used for pawns as to whether they go up or down the sphere
	public Direction heading;
	//Used for pawns two allow double space first move
	public int timesMoved;

	/// <summary>Intermediate property to get/set the material color of the object</summary>
	public Color color {
		set {
			gameObject.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material.color = value;
		}

		get {
			switch(team) {
				case Team.WHITE:
					return BoardConfig.visuals.teamWhiteColor;
				case Team.BLACK:
					return BoardConfig.visuals.teamBlackColor;
				default:
					return Color.magenta;
			}
		}
	}

	/// <returns>Returns the piece's location on the board as a string</returns>
	public string GetLocation() {
		return transform.parent.gameObject.name;
	}

	/// <returns>Returns whether this piece's location is inverse/whether its plane is inverse</returns>
	public bool IsLocationInverse() {
		return GameManager.board.IsLocationInverse(GetLocation());
	}

	/// <returns>Returns the tile at this piece's location</returns>
	public Tile GetTile() {
		return GameManager.board.GetTile(GetLocation());
	}

	private Quaternion CalculateRotation() {
		return Quaternion.Euler(
			IsLocationInverse() ?
			new Vector3(180f, team == Team.WHITE ? 180f : 0f, 0f) :
			new Vector3(0, team == Team.WHITE ? 0 : 180f, 0f)
		);
	}

	private Vector3 CalculateScale() {
		float scale = 0.1f;

		return new Vector3(
			scale / transform.parent.lossyScale.x,
			scale / transform.parent.lossyScale.y,
			scale / transform.parent.lossyScale.z
		);
	}

	/// <summary>Set's this piece's parent and adjusts it rotation and scale to fit properly</summary>
	/// <param name="parent">Transform to make parent</param>
	public void SetTransformParent(Transform parent) {
		transform.SetParent(parent);
		transform.localPosition = Vector3.zero;
		transform.localRotation = CalculateRotation();
		transform.localScale = CalculateScale();
	}

	/// <param name="type">Type of the piece</param>
	/// <returns>Returns the prefab for the piece type</returns>
	public static GameObject GetPrefab(Type type) {
		switch(type) {
			case Type.PAWN:
				return BoardConfig.prefabs.pawn;
			case Type.KNIGHT:
				return BoardConfig.prefabs.knight;
			case Type.BISHOP:
				return BoardConfig.prefabs.bishop;
			case Type.ROOK:
				return BoardConfig.prefabs.rook;
			case Type.QUEEN:
				return BoardConfig.prefabs.queen;
			case Type.KING:
				return BoardConfig.prefabs.king;
			default:
				return null;
		}
	}

	/// <returns>Returns whether this piece requires line of sight to attack</returns>
	public bool RequiresLOS() {
		return type != Type.KNIGHT;
	}
}