using System.Collections.Generic;
using UnityEngine;

public class Jail : MonoBehaviour {
    public static Jail mainJail;

    public GameBoard board;
    public GameObject spawnpoint;
    public float randomness = 5f;
    public float pieceScale = 0.25f;

    [SerializeField] private List<GamePiece> jailedPieces;
    [HideInInspector] public bool state;

    void Start() {
        mainJail = this;
        gameObject.SetActive(false);
    }

    public void Add(GamePiece piece) {
        jailedPieces.Add(piece);
        piece.gameObject.SetActive(false);
        piece.gameObject.transform.parent = spawnpoint.transform;
        piece.gameObject.transform.localPosition = Vector3.zero;
        piece.gameObject.transform.localRotation = Quaternion.identity;
        piece.gameObject.transform.localScale = Vector3.one * pieceScale;
        piece.gameObject.AddComponent<Rigidbody>();
        piece.GetComponent<MeshCollider>().convex = true;
    }

    public void Remove(GamePiece piece) {
        //Destroy(piece.gameObject);
        if(piece) {
            piece.gameObject.SetActive(true);
            piece.gameObject.transform.parent = null;
            piece.gameObject.transform.localPosition = Vector3.zero;
            piece.gameObject.transform.localRotation = Quaternion.identity;
            piece.gameObject.transform.localScale = board.pawnPrefab.transform.localScale;
            Destroy(piece.gameObject.GetComponent<Rigidbody>());
            piece.GetComponent<MeshCollider>().convex = true;
        }

        jailedPieces.Remove(piece);
    }

    public void Toggle() {
        Toggle(!state);
    }

    public void Toggle(bool state) {
        this.state = state;

        gameObject.SetActive(state);
        board.gameObject.SetActive(!state);

        UnityEngine.Random.InitState((int)Time.time);

        foreach(GamePiece p in jailedPieces) {
            if(state) {
                p.gameObject.transform.localPosition = spawnpoint.transform.localPosition + new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
                p.gameObject.transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
                p.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
                p.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
            }

            p.gameObject.SetActive(state);
        }
    }
}