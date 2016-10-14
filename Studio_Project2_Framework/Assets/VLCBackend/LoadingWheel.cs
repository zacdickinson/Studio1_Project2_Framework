using UnityEngine;
using System.Collections;

public class LoadingWheel : MonoBehaviour {

	public float timeOut = 8;
	float stoptime;

	void OnEnable () {
		stoptime = Time.realtimeSinceStartup + timeOut;
	}
	
	void FixedUpdate () {
		transform.Rotate (Vector3.back * 230f * Time.fixedDeltaTime);
		if (Time.realtimeSinceStartup > stoptime && stoptime != 0) {
			gameObject.SetActive (false);
			stoptime = 0;
		}
	}
}
