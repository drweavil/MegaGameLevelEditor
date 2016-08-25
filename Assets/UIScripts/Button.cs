using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Button : MonoBehaviour {
	private InputField saveLoadDiskField;
	private InputField saveLoadTypeField;
	private InputField saveLoadNumberField;
	private InputField saveLoadSizeField;
	private InputField randomPercentField;

	// Use this for initialization
	void Start () {
		saveLoadDiskField = GameObject.Find ("SaveLoadDiskField").GetComponent<InputField> ();
		saveLoadTypeField = GameObject.Find ("SaveLoadTypeField").GetComponent<InputField> ();
		saveLoadNumberField = GameObject.Find ("SaveLoadNumberField").GetComponent<InputField> ();
		saveLoadSizeField = GameObject.Find ("SaveLoadSizeField").GetComponent<InputField> ();
		randomPercentField = GameObject.Find ("RandomPercentField").GetComponent<InputField> ();



		GameObject.Find ("CreatingModeButton").GetComponent<Image> ().color = Color.gray;
		GameObject.Find ("FixModeButton").GetComponent<Image> ().color = Color.gray;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Save(bool rewrite = false){
		SaveLoadManager.levelSerialize = BlockController.bc.level;
		List<string> errors = SaveLoadManager.Save (saveLoadDiskField.text, saveLoadSizeField.text, GameObject.Find ("Dropdown").GetComponent<Dropdown> ().captionText.text, saveLoadTypeField.text, saveLoadNumberField.text, rewrite);
		if (errors.Count != 0) {
			if (errors.FindIndex (s => s == "rewriteRequest") == -1) {
				UIController.uic.dialogWindow.SetActive (true);
				string text = "";
				foreach (string error in errors) {
					text = text + error + "\n";
				}
				GameObject.Find ("DialogWindowText").GetComponent<Text> ().text = text;
			} else {
				UIController.uic.rewriteRequesDialog.SetActive (true);
			}
		}
	}

	public void Load(){
		List<string> errors = SaveLoadManager.Load (saveLoadDiskField.text, saveLoadSizeField.text, GameObject.Find ("Dropdown").GetComponent<Dropdown> ().captionText.text, saveLoadTypeField.text, saveLoadNumberField.text);
	
	
		if (errors.Count != 0) {
			UIController.uic.dialogWindow.SetActive (true);
			string text = "";
			foreach (string error in errors) {
				text = text + error + "\n";
			}
			GameObject.Find ("DialogWindowText").GetComponent<Text> ().text = text;
		}
	}

	public void SetAllocationMode(){
		GameObject.Find ("AllocationModeButton").GetComponent<Image> ().color = Color.gray;
		GameObject.Find ("CreatingModeButton").GetComponent<Image> ().color = Color.white;
		BlockController.bc.editorMode = "allocation";
	}

	public void SetCreatingMode(){
		GameObject.Find ("AllocationModeButton").GetComponent<Image> ().color = Color.white;
		GameObject.Find ("CreatingModeButton").GetComponent<Image> ().color = Color.gray;
		BlockController.bc.editorMode = "creating";
	}

	public void SetDecorsActive(){
		ListElementsController.listType = "decors";
		BlockController.bc.creatingMode = "decor";
		GameObject.Find("ObjectsLists").GetComponent<ListElementsController>().LoadTexturesByName(GameObject.Find ("Dropdown").GetComponent<Dropdown> ().captionText.text);
	}

	public void SetTilesActive(){
		ListElementsController.listType = "tiles";
		BlockController.bc.creatingMode = "tile";
		GameObject.Find("ObjectsLists").GetComponent<ListElementsController>().LoadTexturesByName(GameObject.Find ("Dropdown").GetComponent<Dropdown> ().captionText.text);
	}

	public void SetInteractiveObjectsActive(){
		BlockController.bc.creatingMode = "interactiveObject";
		GameObject list = GameObject.Find("GridTileSpritesList");
		GameObject[] removedObjects = GameObject.FindGameObjectsWithTag ("TilesPanel");
		foreach (GameObject obj in removedObjects) {
			Destroy (obj);
		}
		GameObject[] menuObjects = Resources.LoadAll<GameObject> ("UI/InteractiveObjects");
		foreach(GameObject obj in menuObjects){
			GameObject newObject = Instantiate (obj);
			newObject.transform.parent = list.transform;
		}
	}

	public void AddSet(){
		if (randomPercentField.text == "" || randomPercentField.text == null) {
			UIController.uic.dialogWindow.SetActive (true);
			GameObject.Find ("DialogWindowText").GetComponent<Text> ().text = "Значение рандома нету!";
		} else {
			InputField randomModePercentValue = GameObject.Find ("RandomPercentField").GetComponent<InputField>();
			int rndPercent = 50;
			try{
				rndPercent = int.Parse(randomModePercentValue.text);
				if(rndPercent > 100){
					rndPercent = 100;
				}
				if(rndPercent < 0){
					rndPercent = 0;
				}
			}catch(FormatException){
			}

			Set newSet = BlockController.bc.level.AddSet(BlockController.bc.selectedObjects, rndPercent);
			GameObject setListGround = GameObject.Find ("GridSetsList");
			GameObject newSetGround = Instantiate((GameObject)Resources.Load ("UI/SetGround"));

			newSetGround.transform.parent = setListGround.transform;
			newSetGround.transform.GetChild(0).gameObject.GetComponent<AddedSet> ().setId = newSet.objId;
			newSetGround.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Text> ().text = "set " + newSet.objId.ToString ();
			BlockController.bc.level.setsCount++;
		}

	}

	public void RemoveSet(){
		int setId = GameObject.Find ("RemoveSet").GetComponent<RemovedSet> ().selectedSetId;
		BlockController.bc.level.RemoveSet(setId);

		GameObject[] sets = GameObject.FindGameObjectsWithTag ("UiSet");
		List<GameObject> lSets = new List<GameObject> (sets);
		int setIndex = lSets.FindIndex (s => s.GetComponent<AddedSet>().setId == setId);
		if(setIndex != -1){
			Destroy(lSets[setIndex].transform.parent.gameObject);
		}
	}

	public void FixModeActive(){
		GameObject.Find ("FixModeButton").GetComponent<Image> ().color = Color.gray;
		GameObject.Find ("RandomModeButton").GetComponent<Image> ().color = Color.white;
		BlockController.bc.randomeModeActive = false;
	}

	public void RandomeModeActive(){
		GameObject.Find ("FixModeButton").GetComponent<Image> ().color = Color.white;
		GameObject.Find ("RandomModeButton").GetComponent<Image> ().color = Color.gray;
		BlockController.bc.randomeModeActive = true;		
	}

	public void DialogOkButton(){
		UIController.uic.dialogWindow.SetActive (false);
	}

	public void RewriteDialogYesButton(){
		UIController.uic.rewriteRequesDialog.SetActive (false);
		Save (true);
	}

	public void RewriteDialogNoButton(){
		UIController.uic.rewriteRequesDialog.SetActive (false);
	}
}
