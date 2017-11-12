using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AutoTileButton : MonoBehaviour {
	public string buttonKey;
	public Image image;

	public void UseKey(){
		foreach (AutoTileButton button in ListElementsController.listElementsController.autoTileButtons) {
			button.image.color = Color.white;
		}

		image.color = Color.gray;
		BlockController.bc.autoTileMode = buttonKey;
	}
}
