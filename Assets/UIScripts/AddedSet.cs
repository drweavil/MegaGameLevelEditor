using UnityEngine;
using System.Collections;

public class AddedSet : MonoBehaviour {
	public int setId;

	public void DrawSet(){
		BlockController.bc.DrawSquaresForSelectedObjects (BlockController.bc.level.FindSetById (setId).objects);
		//Debug.Log(BlockController.bc.level.FindSetById (setId).objects.Count);

		GameObject.Find ("RemoveSet").GetComponent<RemovedSet>().selectedSetId = setId;
	}
}
