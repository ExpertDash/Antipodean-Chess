using System.Collections.Generic;
using UnityEngine;

public class Jail : MonoBehaviour {
	[TooltipAttribute("Where to spawn the pieces")] [SerializeField] private Vector3 spawnpoint;
	[TooltipAttribute("Max random offset when placing pieces at the spawnpoint")] [SerializeField] private float randomness = 5f;
	[TooltipAttribute("Scale to set the pieces to")] [SerializeField] private float pieceScale = 0.25f;
	[TooltipAttribute("A list of all the pieces in Jail from any team")] [SerializeField] private List<Piece> jail;
	private GameObject spawnpointObject = null;

	public void Create() {
		spawnpointObject = new GameObject("Spawnpoint");
		spawnpointObject.transform.SetParent(transform);
		spawnpointObject.transform.localPosition = spawnpoint;
	}

	public void Clear() {
		jail.Clear();
		Destroy(spawnpointObject);
	}

	/// <summary>Adds a piece to jail</summary>
	/// <param name="piece">Piece to place in jail</param>
	public void Add(Piece piece) {
		jail.Add(piece);
		piece.gameObject.SetActive(false);
		piece.transform.parent = spawnpointObject.transform;
		piece.transform.localPosition = Vector3.zero;
		piece.transform.localRotation = Quaternion.identity;
		piece.transform.localScale = Vector3.one * pieceScale;
		piece.gameObject.AddComponent<Rigidbody>();
		piece.GetComponent<MeshCollider>().convex = true;
	}

	/// <summary>Places the jailed pieces at the spawnpoint or hides the jail</summary>
	/// <param name="state">Whether the jail should be visibile</param>
	public void SetVisible(bool state) {
		UnityEngine.Random.InitState((int)Time.time);

		gameObject.SetActive(state);

		foreach(Piece p in jail) {
			if(state) {
				p.gameObject.transform.localPosition = spawnpointObject.transform.localPosition + new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
				p.gameObject.transform.localEulerAngles = new Vector3(UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f), UnityEngine.Random.Range(0f, 360f));
				p.GetComponent<Rigidbody>().velocity = new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
				p.GetComponent<Rigidbody>().angularVelocity = new Vector3(UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness), UnityEngine.Random.Range(-randomness, randomness));
			}

			p.gameObject.SetActive(state);
		}
	}
}