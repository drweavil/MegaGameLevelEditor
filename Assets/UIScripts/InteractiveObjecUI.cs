using UnityEngine;
using System.Collections;

public class InteractiveObjecUI : MonoBehaviour {

	public string type;

	public void SelectInteractiveObject(){
		ListElementsController.selectedInteractiveObject = type;
	}
}
