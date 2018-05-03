using UnityEngine;

public class Targeter : MonoBehaviour {
    public GameObject target;

    void FixedUpdate() {
        transform.localRotation = Quaternion.LookRotation(target.transform.position);
    }
}
