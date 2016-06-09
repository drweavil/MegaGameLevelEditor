using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DropDownScript : MonoBehaviour {
	public Dropdown levelTypeDropDown;
	public GameObject objectLists;

	// Use this for initialization
	void Start () {
		objectLists = GameObject.Find ("ObjectsLists");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnChangeTypeValue(){
		objectLists.GetComponent<ListElementsController> ().LoadTexturesByName (levelTypeDropDown.captionText.text.ToString());
	}
}
