using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIController : MonoBehaviour {

	public GameObject dialogWindow;
	public GameObject rewriteRequesDialog;
	public static UIController uic;

	// Use this for initialization
	void Start () {
		uic = gameObject.GetComponent<UIController>();
		dialogWindow.SetActive (false);
		rewriteRequesDialog.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
