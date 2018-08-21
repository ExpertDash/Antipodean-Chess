using System.Linq;
using UnityEngine;

public class TurnSystem : MonoBehaviour {
	public PieceSelector[] players;
	public int factions, currentFaction, currentTurn;

	public bool IsTurn(int faction) {
		return currentFaction == faction;
	}

	public void NextTurn() {
		currentTurn++;

		if(++currentFaction > factions - 1) {
			currentFaction = 0;
		}
	}

	public PieceSelector GetPlayer(int faction) {
		return players.First(p => p.faction == faction);
	}
}
