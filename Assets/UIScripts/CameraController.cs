using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class CameraController : MonoBehaviour {

	private  float cameraDistance =5f;

	private Vector3 dragOrigin;
	private float dragSpeed = 0.1f;

	// Use this for initialization
	void Start () {
		Camera.main.orthographicSize = cameraDistance;
		//camera = GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if ((Input.GetAxis ("Mouse ScrollWheel") < 0 || Input.GetAxis ("Mouse ScrollWheel") > 0) && !EventSystem.current.IsPointerOverGameObject()) {
			cameraDistance -= Input.GetAxis ("Mouse ScrollWheel")*10;
			Camera.main.orthographicSize = cameraDistance;
		}


		if(Input.GetAxis ("Horizontal") < 0 || Input.GetAxis ("Horizontal") > 0 ){
			float newX = (float)(transform.position.x + ((float)Input.GetAxis ("Horizontal") * 0.5));
			Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z);
			transform.position = newPosition;	
		}

		if(Input.GetAxis ("Vertical") < 0 || Input.GetAxis ("Vertical") > 0 ){
			transform.position = new Vector3(transform.position.x, (float)(transform.position.y + (float)Input.GetAxis ("Vertical") * 0.5), transform.position.z);	
		}
	}
}
