using UnityEngine;

public class BoardLayout : MonoBehaviour {
    [System.SerializableAttribute]
    public struct Placement {
        public string square;
        public int faction;
        public GamePiece.Type piece;
    }

    public Placement[] placements;
}
