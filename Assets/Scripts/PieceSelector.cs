using UnityEngine;

public class PieceSelector : MonoBehaviour {
	public GameBoard board;
	public GameObject selectedSquare;

	public float rotationSpeed = 100f;
	public float zoomSpeed = 10f;

	public float minDistanceFromCenter = 5f;
	public float maxDistanceFromCenter = 20f;

	public int mouseButtonRotate = 1;
	public int mouseButtonSelect = 0;

	private Color selectedSquareColor;

	void Update() {
		if(Input.GetMouseButtonDown(mouseButtonSelect)) {
			RaycastHit hit;
			if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit)) {
				GameObject tile;

				if(hit.transform.GetComponent<GamePiece>() != null) {
					tile = board.GetSquare(hit.transform.GetComponent<GamePiece>().GetSquare()).tile;
				} else {
					tile = hit.transform.gameObject;
				}

				if(selectedSquare != null) {
					selectedSquare.transform.GetChild(0).gameObject.GetComponent<Renderer>().material.color = selectedSquareColor;
				}

				selectedSquareColor = tile.GetComponent<Renderer>().material.color;
				tile.GetComponent<Renderer>().material.color = Color.green;
				selectedSquare = tile.transform.parent.gameObject;
		   }
	   }

		if(Input.GetMouseButtonDown(mouseButtonRotate)) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		if(Input.GetMouseButtonUp(mouseButtonRotate)) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}

		if(Input.GetMouseButton(mouseButtonRotate)) {
			Vector2 move = new Vector2(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X")) * rotationSpeed * Time.deltaTime;

			transform.RotateAround(board.transform.position, transform.up, move.y);
			transform.RotateAround(board.transform.position, -transform.right, move.x);
		}

		if(Input.mouseScrollDelta.y != 0) {
			if(Input.mouseScrollDelta.y > 0 && Vector3.Distance(transform.position, board.transform.position) <= minDistanceFromCenter) {
				return;
			} else if(Input.mouseScrollDelta.y < 0 && Vector3.Distance(transform.position, board.transform.position) >= maxDistanceFromCenter) {
				return;
			}

			transform.position = Vector3.MoveTowards(transform.position, board.transform.position, Input.mouseScrollDelta.y * zoomSpeed * Time.deltaTime);
		}
	}
}
