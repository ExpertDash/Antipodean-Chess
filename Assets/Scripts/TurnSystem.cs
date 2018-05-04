using UnityEngine;

public class TurnSystem : MonoBehaviour {
	public string[] users;
	public string userThisTurn;
	public int currentTurn = 0;
	private int id = 0;

	public bool IsTurn(string user) {
		return userThisTurn == user;
	}

	void Start() {
		userThisTurn = users[id];
	}

	public void NextTurn() {
		currentTurn++;
		id++;

		if(id > users.Length - 1) {
			id = 0;
		}

		userThisTurn = users[id];
	}
}
