using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class ListElementsController : MonoBehaviour {

	private List<Sprite> sprites = new List<Sprite>();
	private GameObject newPanel;
	public List<Vector2> selectedUV = new List<Vector2>();
	public float selectedTileTextureWidth;
	public float selectedTileTextureHeight;
	private Dropdown dropDown;

	private int heightElementCount = 4; 
	private int weightElementCount = 4;
	private int elementWidth = 32;
	private int elementHeigh = 32;
	float pixelWidthSize = 1f / 128f;
	float pixelHeightSize = 1f / 128f;

	GameObject panelPrefab;
	GameObject tilePrefab;

	public static string listType  = "tiles";
	public static string selectedInteractiveObject = "";

	// Use this for initialization
	void Start () {
		panelPrefab = (GameObject)Resources.Load("UI/TilesPanel");
		tilePrefab = (GameObject)Resources.Load("UI/TileGround");

		dropDown = GameObject.Find ("Dropdown").GetComponent<Dropdown>();
		LoadTexturesByName (dropDown.captionText.text);
	}

	void Update () {
	
	}

	public void LoadTexturesByName(string textureName){
		selectedUV.Clear ();
		foreach (GameObject panel in GameObject.FindGameObjectsWithTag("TilesPanel")) {
			Destroy (panel);
		}
		GameObject tileList = GameObject.Find ("GridTileSpritesList");

		Sprite[] sprites = Resources.LoadAll<Sprite> ("GameTextures/" + textureName);
		Texture mainTexture = sprites [0].texture;
		SetTextureToMeshes (mainTexture);


		if (listType == "tiles") {
			List<Sprite> tiles = new List<Sprite> (sprites).FindAll (s => IsRightString(s.name, "tile"));
			SetSpritesToList (tiles, tileList, textureName);
		}
		if (listType == "decors") {
			List<Sprite> decors = new List<Sprite> (sprites).FindAll (s => IsRightString(s.name, "decor"));
			SetSpritesToList (decors, tileList, textureName);
		}
	}


	List<Vector2> GetUvsByName(string textureName, string spriteName){
		List<Vector2> uvs = new List<Vector2> ();
		
		
		Sprite sprite = GetSprite(textureName, spriteName);
		Rect uvRect = sprite.rect;
		float textureWidthSize = sprite.texture.width;
		float textureHeightSize = sprite.texture.height;


		uvs.Add (new Vector2(uvRect.position.x*(1f/textureWidthSize), uvRect.position.y*(1f/textureHeightSize)));
		uvs.Add (new Vector2((uvRect.position.x + uvRect.width)*(1f/textureWidthSize), uvRect.position.y*(1f/textureHeightSize)));
		uvs.Add (new Vector2((uvRect.position.x + uvRect.width)*(1f/textureWidthSize), (uvRect.position.y + uvRect.height)*(1f/textureHeightSize)));
		uvs.Add (new Vector2(uvRect.position.x*(1f/textureWidthSize), (uvRect.position.y + uvRect.height)*(1f/textureHeightSize)));
		return uvs;	
	}

	Sprite GetSprite(string textureName, string spriteName){
		Sprite[] textures_tmp = Resources.LoadAll<Sprite> ("GameTextures/" + textureName.ToString());
		List<Sprite> textures = new List<Sprite> (textures_tmp);

		Sprite sprite = textures.Find (s => s.name == spriteName.ToString());
		return sprite;
	}

	public static void SetTextureToMeshes(Texture texture){
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag ("Mesh")){
			obj.GetComponent<MeshRenderer> ().material.mainTexture = texture; 
		}
	}

	bool IsRightString(string str, string pattern){
		bool right = false;
		Regex regEx = new Regex (pattern);
		MatchCollection matchs = regEx.Matches (str);
		if (matchs.Count >= 1) {
			right = true;
		}
		return right;
		//Debug.Log (matchs.Count);
	}

	void SetSpritesToList(List<Sprite> arr, GameObject list, string textureName){
		for (int i = 0; i < arr.Count ; i++) {
			//for (int j = 0; j < heightElementCount; j++) {

			Texture texture = arr[i].texture;
			//Sprite newSrite = Sprite.Create(texture, new Rect(i*elementWidth, j*elementHeigh, elementWidth, elementHeigh), new Vector2(0.5f, 0.5f));


			if ((i + 4) % 4 == 0) {
				newPanel = Instantiate (panelPrefab);
				newPanel.transform.parent = list.transform;
			}

			GameObject newTileGround = Instantiate (tilePrefab);
			newTileGround.transform.parent = newPanel.transform;
			GameObject newTile = newTileGround.transform.GetChild (0).gameObject;

			newTile.GetComponent<Image>().sprite = arr[i];
			TileScript tileScript = newTile.GetComponent<TileScript> ();

			List<Vector2> tileSpriteUvs = GetUvsByName (textureName, arr[i].name);
			Sprite sprite = GetSprite (textureName, arr [i].name);
			tileScript.textureHeight = sprite.texture.height;
			tileScript.textureWidth = sprite.texture.width;

			foreach(Vector2 uv in tileSpriteUvs){
				tileScript.uvVertices.Add (uv);
			}
			/*if (selectedUV.Count == 0) {
				foreach(Vector2 uv in tileSpriteUvs){
					tileScript.uvVertices.Add (uv);
				}
			}*/
		}
	}
}
