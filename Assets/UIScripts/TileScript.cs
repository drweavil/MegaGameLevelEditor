using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TileScript : MonoBehaviour {
	private ListElementsController tileList;
	public List<Vector2> uvVertices = new List<Vector2>();
	public float textureWidth;
	public float textureHeight;

	// Use this for initialization
	void Start () {
		tileList = GameObject.Find ("ObjectsLists").GetComponent<ListElementsController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SetTile (){
		tileList.selectedUV = uvVertices;
		tileList.selectedTileTextureHeight = textureHeight;
		tileList.selectedTileTextureWidth = textureWidth;
	}
}
