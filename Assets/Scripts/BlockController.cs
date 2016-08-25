using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System;




public class BlockController : MonoBehaviour {
	private Vector3 worldMousePosition;
	private Hashtable blocks = new Hashtable();
	private int width = 70;
	private int height = 70;
	public static List<GameObject> meshes = new List<GameObject> ();
	public static List<LevelMesh> returnMeshes = new List<LevelMesh> ();
	private GameObject newMesh;
	private GameObject levelMesh; 
	private GameObject levelMeshPrefab;
	private Ray ray;
	private RaycastHit hit;
	//private int objCount = 0;
	private ListElementsController tileList;
	public LevelRudiment level;
	public static BlockController bc;
	private List<ObjectInfo> objectList = new List<ObjectInfo> ();

	private List<GameObject> gridMeshes = new List<GameObject>();
	GameObject gridMeshPrefab;
	GameObject gridMesh;
	SquareCoords squareCoords = new SquareCoords();

	List<SquareCoords> renderedSquares = new List<SquareCoords> ();

	public List<ObjectInfo> selectedObjects = new List<ObjectInfo> ();

	public string editorMode = "creating";
	public string creatingMode = "tile";

	public bool randomeModeActive = false;

	private Hashtable interactiveObjectPrefabs = new Hashtable ();
	InputField randomModePercentValue;

	// Use this for initialization
	void Start () {
		bc = gameObject.GetComponent<BlockController> ();
		tileList = GameObject.Find ("ObjectsLists").GetComponent<ListElementsController> ();
		levelMeshPrefab = (GameObject)Resources.Load("LevelMesh");
		levelMesh = Instantiate(levelMeshPrefab);
		meshes.Add (levelMesh);
		for(int i = 0; i< width; i++){
			for(int j = 0; j< width; j++){
				blocks[width.ToString()+";"+height.ToString()] = null;
			}
		}

		level = new LevelRudiment ();

		gridMeshPrefab = (GameObject)Resources.Load ("GridMesh");
		randomModePercentValue = GameObject.Find ("RandomPercentField").GetComponent<InputField>();

		foreach(GameObject obj in Resources.LoadAll<GameObject>("GameObjects")){
			interactiveObjectPrefabs.Add (obj.name, obj.gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.I)){
			Debug.Log (selectedObjects.Count);
		}
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint (Input.mousePosition);

		/*********************Random****************/
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
		/*********************************************/

		if(!EventSystem.current.IsPointerOverGameObject()){
			if (editorMode == "creating") {
				if (randomeModeActive == true && (GameObject.Find ("RandomPercentField").GetComponent<InputField> ().text == "" || GameObject.Find ("RandomPercentField").GetComponent<InputField> ().text == null)) {
					UIController.uic.dialogWindow.SetActive (true);
					GameObject.Find ("DialogWindowText").GetComponent<Text> ().text = "Значение рандома нету!";
				} else {
					ray = Camera.main.ScreenPointToRay (Input.mousePosition);
					Physics.Raycast (ray, out hit);
					if (creatingMode == "tile") {
						if (Input.GetMouseButton (0)) {				
							if (mousePosition.x > 0 && mousePosition.y < 0 && mousePosition.x < width && mousePosition.y > -height && tileList.selectedUV.Count != 0) {
								int mouseBlockX = (int)mousePosition.x;
								int mouseBlockY = (int)mousePosition.y;
								if (IsFreePlace (new Vector2 (mousePosition.x, mousePosition.y), "tile")/*blocks[mouseBlockX.ToString()+";"+ mouseBlockY.ToString()] == null*/) {
									if (meshes [meshes.Count - 1].GetComponent<LevelMesh> ().newVertices.Count >= 12000) {
										newMesh = Instantiate (levelMeshPrefab);
										meshes.Add (newMesh);

									} 
									meshes [meshes.Count - 1].GetComponent<LevelMesh> ().GenTile (mouseBlockX, mouseBlockY, 0, tileList.selectedUV);

									List<SerializableVector2> uvs = new List<SerializableVector2> ();
									foreach (Vector2 uv in tileList.selectedUV) {
										uvs.Add (new SerializableVector2 (uv.x, uv.y));
									}
									level.AddBlock (new SerializableVector3 (mouseBlockX, mouseBlockY, 0), uvs, randomeModeActive, rndPercent);
								}
							}
						}

						if (Input.GetMouseButton (1)) {
							if (mousePosition.x > 0 && mousePosition.y < 0) {
								int mouseBlockX = (int)mousePosition.x;
								int mouseBlockY = (int)mousePosition.y;
								if (hit.collider != null) {
									if (hit.collider.name == "LevelMesh(Clone)") {
										hit.collider.GetComponent<LevelMesh> ().RemoveTile ((float)mouseBlockX, (float)mouseBlockY, 0f);
										if (hit.collider.GetComponent<LevelMesh> ().newVertices.Count == 0) {
											if (meshes.Count != 1) {
												meshes.RemoveAt (meshes.FindIndex (meshPred => meshPred.GetComponent<LevelMesh> ().newVertices.Count == 0));
												Destroy (hit.collider.gameObject);
											}
										}
									}							
								}
								List<ObjectInfo> objects = FindObjectByMousePosition (new Vector2 (mousePosition.x, mousePosition.y));
								objects = objects.FindAll (o => o.objectType == "tile");
								foreach (ObjectInfo obj in objects) {
									level.RemoveObjectById (obj.objId);
								}
							}
						}
					}

					if (creatingMode == "decor") {
						if (Input.GetMouseButton (0)) {				
							if (mousePosition.x > 0 && mousePosition.y < 0 && mousePosition.x < width && mousePosition.y > -height && tileList.selectedUV.Count != 0) {
								bool isFreePosition = IsFreePlace (new Vector2 (mousePosition.x, mousePosition.y), "decor");
								if (Input.GetKey (KeyCode.G)) {
									Rect decorRectPosition = GetRectBySelectedTile (mousePosition);
									decorRectPosition = DownStickingByRect (decorRectPosition);
									isFreePosition = IsFreePlace (decorRectPosition, "decor");
								}
								if (Input.GetKey (KeyCode.T)) {
									Rect decorRectPosition = GetRectBySelectedTile (mousePosition);
									decorRectPosition = UpStickingByRect (decorRectPosition);
									isFreePosition = IsFreePlace (decorRectPosition, "decor");
								}
								if (Input.GetKey (KeyCode.F)) {
									Rect decorRectPosition = GetRectBySelectedTile (mousePosition);
									decorRectPosition = LeftStickingByRect (decorRectPosition);
									isFreePosition = IsFreePlace (decorRectPosition, "decor");
								}
								if (Input.GetKey (KeyCode.H)) {
									Rect decorRectPosition = GetRectBySelectedTile (mousePosition);
									decorRectPosition = RightStickingByRect (decorRectPosition);
									isFreePosition = IsFreePlace (decorRectPosition, "decor");
								}
								if (isFreePosition) {
									if (meshes [meshes.Count - 1].GetComponent<LevelMesh> ().newVertices.Count >= 12000) {
										newMesh = Instantiate (levelMeshPrefab);
										meshes.Add (newMesh);

									} 

									ObjectInfo newObj = new ObjectInfo ();
									Rect decorRect = GetRectBySelectedTile (mousePosition);
									if (Input.GetKey (KeyCode.G)) {
										decorRect = DownStickingByRect (decorRect);
									}
									if (Input.GetKey (KeyCode.T)) {
										decorRect = UpStickingByRect (decorRect);
									}
									if (Input.GetKey (KeyCode.F)) {
										decorRect = LeftStickingByRect (decorRect);
									}
									if (Input.GetKey (KeyCode.H)) {
										decorRect = RightStickingByRect (decorRect);
									}
									List<Vector3> decorVerticesCoords = GetVerticesCoordsByRect (decorRect);
									newObj.objRectCoord = new SerializableVector2 (decorRect.x, decorRect.y);
									newObj.objRectHeight = decorRect.height;
									newObj.objRectWidth = decorRect.width;

									List<SerializableVector3> serDecorVerticesCoords = new List<SerializableVector3> ();
									foreach (Vector3 coord in decorVerticesCoords) {
										serDecorVerticesCoords.Add (new SerializableVector3 (coord.x, coord.y, coord.z));	
									}

									List<SerializableVector2> serDecorUv = new List<SerializableVector2> ();
									foreach (Vector2 uv in tileList.selectedUV) {
										serDecorUv.Add (new SerializableVector2 (uv.x, uv.y));	
									}


									level.AddDecorBlock (serDecorVerticesCoords, serDecorUv, newObj, randomeModeActive, rndPercent);


									meshes [meshes.Count - 1].GetComponent<LevelMesh> ().GenSquareByCoords (decorVerticesCoords, tileList.selectedUV);

								}
							}
						}

						if (Input.GetMouseButton (1)) {
							if (mousePosition.x > 0 && mousePosition.y < 0) {
								List<ObjectInfo> objects = FindObjectByMousePosition (new Vector2 (mousePosition.x, mousePosition.y));
								objects = objects.FindAll (o => o.objectType == "decor");
								foreach (ObjectInfo obj in objects) {
									Rect objRect = new Rect ();
									objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
									objRect.width = obj.objRectWidth;
									objRect.height = obj.objRectHeight;

									foreach (GameObject mesh in meshes) {
										mesh.GetComponent<LevelMesh> ().RemoveSquareByCoords (GetVerticesCoordsByRect (objRect));
									}
									level.RemoveObjectById (obj.objId);
								}
							}							
						}
					}

					if (creatingMode == "interactiveObject") {
						if (Input.GetMouseButton (0)) {				
							if (mousePosition.x > 0 && mousePosition.y < 0 && mousePosition.x < width && mousePosition.y > -height && ListElementsController.selectedInteractiveObject != "") {
								bool isFreePosition = IsFreePlace (new Vector2 (mousePosition.x, mousePosition.y), "interactiveObject");
								if (Input.GetKey (KeyCode.G)) {
									Rect objectRectPosition = GetRectBySelectedObject (mousePosition);
									objectRectPosition = DownStickingByRect (objectRectPosition);
									isFreePosition = IsFreePlace (objectRectPosition, "interactiveObject");
								}
								if (Input.GetKey (KeyCode.T)) {
									Rect objectRectPosition = GetRectBySelectedObject (mousePosition);
									objectRectPosition = UpStickingByRect (objectRectPosition);
									isFreePosition = IsFreePlace (objectRectPosition, "interactiveObject");
								}
								if (Input.GetKey (KeyCode.F)) {
									Rect objectRectPosition = GetRectBySelectedObject (mousePosition);
									objectRectPosition = LeftStickingByRect (objectRectPosition);
									isFreePosition = IsFreePlace (objectRectPosition, "interactiveObject");
								}
								if (Input.GetKey (KeyCode.H)) {
									Rect objectRectPosition = GetRectBySelectedObject (mousePosition);
									objectRectPosition = RightStickingByRect (objectRectPosition);
									isFreePosition = IsFreePlace (objectRectPosition, "interactiveObject");
								}
								if (isFreePosition) {
									ObjectInfo newObj = new ObjectInfo ();
									Rect interactiveObjectRect = GetRectBySelectedObject (mousePosition);
									if (Input.GetKey (KeyCode.G)) {
										interactiveObjectRect = DownStickingByRect (interactiveObjectRect);
									}
									if (Input.GetKey (KeyCode.T)) {
										interactiveObjectRect = UpStickingByRect (interactiveObjectRect);
									}
									if (Input.GetKey (KeyCode.F)) {
										interactiveObjectRect = LeftStickingByRect (interactiveObjectRect);
									}
									if (Input.GetKey (KeyCode.H)) {
										interactiveObjectRect = RightStickingByRect (interactiveObjectRect);
									}

									newObj.objRectCoord = new SerializableVector2 (interactiveObjectRect.x, interactiveObjectRect.y);
									newObj.objRectHeight = interactiveObjectRect.height;
									newObj.objRectWidth = interactiveObjectRect.width;

									InteractiveObject newIntObj = level.AddInteractiveObject (new SerializableVector3 (interactiveObjectRect.center.x, interactiveObjectRect.center.y, 0), ListElementsController.selectedInteractiveObject, newObj, randomeModeActive, rndPercent);
									GameObject newSceneIntObj = Instantiate ((GameObject)interactiveObjectPrefabs [ListElementsController.selectedInteractiveObject]);
									newSceneIntObj.transform.position = interactiveObjectRect.center;
									newSceneIntObj.GetComponent<InteractiveObjectScene> ().objId = newIntObj.objId;
								}
							}
						}

						if (Input.GetMouseButton (1)) {
							if (mousePosition.x > 0 && mousePosition.y < 0) {
								List<ObjectInfo> objects = FindObjectByMousePosition (new Vector2 (mousePosition.x, mousePosition.y));
								objects = objects.FindAll (o => o.objectType == "interactiveObject");
								foreach (ObjectInfo obj in objects) {
									GameObject[] tmp = GameObject.FindGameObjectsWithTag ("InteractiveObject");
									List<GameObject> interactiveObjects = new List<GameObject> (tmp);
									Destroy (interactiveObjects.Find (o => o.GetComponent<InteractiveObjectScene> ().objId == obj.objId));
									level.RemoveObjectById (obj.objId);
								}
							}							
						}							
					}
				}
			}




			if (editorMode == "allocation") {
				if (Input.GetKeyDown (KeyCode.Delete)) {
					List<ObjectInfo> removedObjects = new List<ObjectInfo> ();
					foreach (ObjectInfo obj in selectedObjects) {
						if (obj.objectType == "tile") {

							foreach (GameObject mesh in meshes) {
								mesh.GetComponent<LevelMesh> ().RemoveTile ((float)obj.objRectCoord.x, (float)obj.objRectCoord.y + 1, 0f);
							}
							level.RemoveObjectById (obj.objId);
						}

						if(obj.objectType == "decor"){
							Rect objRect = new Rect ();
							objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
							objRect.width = obj.objRectWidth;
							objRect.height = obj.objRectHeight;

							foreach(GameObject mesh in meshes){
								mesh.GetComponent<LevelMesh> ().RemoveSquareByCoords(GetVerticesCoordsByRect (objRect));
							}
							level.RemoveObjectById (obj.objId);
						}

						if (obj.objectType == "interactiveObject") {
							GameObject[] tmp = GameObject.FindGameObjectsWithTag("InteractiveObject");
							List<GameObject> interactiveObjects = new List<GameObject> (tmp);
							Destroy(interactiveObjects.Find (o => o.GetComponent<InteractiveObjectScene>().objId == obj.objId));
							level.RemoveObjectById (obj.objId);
						}

						int index = selectedObjects.FindIndex (o => o.objId == obj.objId);
						if(index != -1){
							//selectedObjects.RemoveAt (index);
							removedObjects.Add(selectedObjects[index]);
						}

						//
					}
					foreach(ObjectInfo obj in removedObjects){
						int index = selectedObjects.FindIndex (o => o.objId == obj.objId);
						if (index != -1) {
							selectedObjects.RemoveAt (index);
						}
					}
					DrawSquaresForSelectedObjects (selectedObjects);
				}


				if (Input.GetMouseButton (0)) {
					if (gridMeshes.Count == 0) {
						gridMesh = Instantiate (gridMeshPrefab);
						gridMeshes.Add (gridMesh);
					}
					if (gridMeshes.Count > 0) {
						if (gridMeshes [gridMeshes.Count - 1].GetComponent<GridMeshController> ().lineVertices.Count >= 12000) {
							gridMesh = Instantiate (gridMeshPrefab);
							gridMeshes.Add (gridMesh);
						}
					}

					if (squareCoords.startDefined) {
						mousePosition.z = 0;
						if (!(squareCoords.end.x == mousePosition.x && squareCoords.end.y == mousePosition.y && squareCoords.end.z == mousePosition.z)) {
							foreach (GameObject gmc in gridMeshes) {
								gmc.GetComponent<GridMeshController> ().RemoveSquare (squareCoords.start, squareCoords.end);
							}
							gridMeshes [gridMeshes.Count - 1].GetComponent<GridMeshController> ().DrawSquare (squareCoords.start, mousePosition, "Green");
							squareCoords.end = mousePosition;
						}

						Rect squareRect = new Rect ();
						squareRect.position = new Vector2 (squareCoords.start.x, squareCoords.end.y);
						Vector2 widthPointCoords = new Vector2 (squareCoords.end.x, squareCoords.start.y);
						Vector2 heightPointCoords = new Vector2 (squareCoords.start.x, squareCoords.end.y);
						float widthDistance = Vector2.Distance (widthPointCoords, new Vector2 (squareCoords.start.x, squareCoords.start.y));
						float heightDistance = Vector2.Distance (heightPointCoords, new Vector2 (squareCoords.start.x, squareCoords.start.y));

						squareRect.width = widthDistance;				
						squareRect.height = heightDistance;//heightVector.magnitude;

						int widthCoeff = -1;
						int heightCoeff = -1;
						if(squareCoords.start.x >= widthPointCoords.x){
							widthCoeff = 1;
						}

						if (squareCoords.start.y >= heightPointCoords.y) {
							heightCoeff = 1;
						}


						Vector2 squareCenter = new Vector2 (widthPointCoords.x + (widthDistance/2)*widthCoeff, heightPointCoords.y + (heightDistance/2)*heightCoeff);
						squareRect.center = squareCenter;
						if (!Input.GetKey(KeyCode.LeftControl)) {
							selectedObjects.Clear ();
						}
						foreach(ObjectInfo obj in FindObjectsByRect(squareRect)){
							selectedObjects.Add (obj);
						}
						selectedObjects = selectedObjects.Distinct ().ToList();

						DrawSquaresForSelectedObjects (selectedObjects);
					}

					if (!squareCoords.startDefined) {
						mousePosition.z = 0;
						squareCoords.start = mousePosition;
						squareCoords.startDefined = true;
					}
				} else {
					if(Input.GetMouseButton(1)){
						List<ObjectInfo> removeFromSelectedObjects = FindObjectByMousePosition (mousePosition);
						foreach (ObjectInfo obj in removeFromSelectedObjects) {
							int index = selectedObjects.FindIndex (o => o.objId == obj.objId);
							if(index != -1){
								selectedObjects.RemoveAt (index);
							}
						}
						DrawSquaresForSelectedObjects (selectedObjects);
					}
					if (squareCoords.startDefined == true) {
						squareCoords.startDefined = false;
						if (gridMeshes.Count != 0) {
							foreach (GameObject gmc in gridMeshes) {
								gmc.GetComponent<GridMeshController>().RemoveSquare (squareCoords.start, squareCoords.end);
							}
						}
					}
				}
			}
		}
	}


	/*public List<LevelMesh> GetMeshes (){
		returnMeshes.Clear ();
		for (int i = 0; i < meshes.Count; i++) {
			returnMeshes.Add (meshes [i].GetComponent<LevelMesh> ());
		}
		return returnMeshes;
	}*/

	public void DrawLevel(){
		DestroyMeshes ();


		if (meshes.Count == 0) {
			meshes.Add (Instantiate(levelMeshPrefab));
		}

		List<Block> blocks = level.fixBlocks;
		blocks.AddRange (level.randomBlocks);
		foreach (Block block in blocks) {
			if (meshes [meshes.Count - 1].GetComponent<LevelMesh> ().newVertices.Count >= 12000) {
				meshes.Add (Instantiate(levelMeshPrefab));
			}
			List<Vector2> uvs = new List<Vector2> ();
			foreach (SerializableVector2 sUv in block.squareUV) {
				uvs.Add (new Vector2 (sUv.x, sUv.y));
			}
			meshes [meshes.Count - 1].GetComponent<LevelMesh> ().GenTile (block.blockCoords.x, block.blockCoords.y, block.blockCoords.z, uvs);
		}


		List<DecorBlock> decorBlocks = level.fixDecorBlocks;
		decorBlocks.AddRange (level.randomDecorBlocks);
		foreach (DecorBlock decorBlock in decorBlocks) {
			if (meshes [meshes.Count - 1].GetComponent<LevelMesh> ().newVertices.Count >= 12000) {
				meshes.Add (Instantiate(levelMeshPrefab));
			}

			List<Vector3> vertices = new List<Vector3> ();
			foreach (SerializableVector3 vert in decorBlock.blockCoords) {
				vertices.Add (new Vector3 (vert.x, vert.y, vert.z));
			}

			List<Vector2> uvs = new List<Vector2> ();
			foreach (SerializableVector2 sUv in decorBlock.squareUV) {
				uvs.Add (new Vector2 (sUv.x, sUv.y));
			}
			meshes [meshes.Count - 1].GetComponent<LevelMesh> ().GenSquareByCoords (vertices, uvs);
		}

		List<InteractiveObject> objects = level.fixInteractiveObjects;
		objects.AddRange (level.randomInteractiveObjects);
		foreach(InteractiveObject obj in objects){
			GameObject newObj = Instantiate ((GameObject)Resources.Load("GameObjects/"+obj.type));
			newObj.GetComponent<InteractiveObjectScene> ().objId = obj.objId;
			newObj.transform.position = new Vector3 (obj.objCoord.x, obj.objCoord.y, obj.objCoord.z);
		}

		GameObject setListGround = GameObject.Find ("GridSetsList");
		foreach (Set newSet in level.sets) {
			GameObject newSetGround = Instantiate ((GameObject)Resources.Load ("UI/SetGround"));

			newSetGround.transform.parent = setListGround.transform;
			newSetGround.transform.GetChild (0).gameObject.GetComponent<AddedSet> ().setId = newSet.objId;
			newSetGround.transform.GetChild (0).GetChild (0).gameObject.GetComponent<Text> ().text = "set " + newSet.objId.ToString ();
		}

		Sprite[] sprites = Resources.LoadAll<Sprite> ("GameTextures/" + level.textureName);
		Texture mainTexture = sprites [0].texture;
		ListElementsController.SetTextureToMeshes (mainTexture);
	}

	void DestroyMeshes(){
		GameObject[] meshesR = GameObject.FindGameObjectsWithTag ("Mesh");
		for (int i = 0; i < meshesR.Length; i++) {
			Destroy(meshes[i].gameObject);
		}
		meshes.Clear ();
	}



	bool IsFreePlace(Vector3 mousePosition, string objType){
		bool isFree = true;
		foreach (ObjectInfo obj in level.objects) {
			Rect objRect = new Rect ();
			objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
			objRect.height = obj.objRectHeight;
			objRect.width = obj.objRectWidth;
			if (objRect.Contains (mousePosition) && obj.objectType == objType) {
				isFree = false;
			}
		}
		return isFree;
	}

	bool IsFreePlace(Rect rect, string objType){
		bool isFree = true;
		foreach (ObjectInfo obj in level.objects) {
			Rect objRect = new Rect ();
			objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
			objRect.height = obj.objRectHeight;
			objRect.width = obj.objRectWidth;
			if (objRect.Contains (rect.center) && obj.objectType == objType) {
				isFree = false;
			}
		}
		return isFree;
	}

	List<ObjectInfo> FindObjectByMousePosition(Vector2 mousePosition){
		List<ObjectInfo> findedObjects = new List<ObjectInfo>();
		foreach(ObjectInfo obj in level.objects){
			Rect rect = new Rect ();
			rect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
			rect.width = obj.objRectWidth;
			rect.height = obj.objRectHeight;

			if (rect.Contains (mousePosition)) {
				findedObjects.Add (obj);
			}
		}
		return findedObjects;
	}

	List<ObjectInfo> FindObjectsByRect(Rect squareRect){
		List<ObjectInfo> foundObjects = new List<ObjectInfo> ();
		foreach (ObjectInfo obj in level.objects) {
			Rect objRect = new Rect ();
			objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
			objRect.width = obj.objRectWidth;
			objRect.height = obj.objRectHeight;
			if(squareRect.Overlaps (objRect)){
				foundObjects.Add(obj);
			}
		}
		return foundObjects;
	}

	public void DrawSquaresForSelectedObjects(List<ObjectInfo> objs){
		if(gridMeshes.Count == 0){
			gridMeshes.Add (Instantiate(gridMeshPrefab));
		}
		foreach (SquareCoords sqr in renderedSquares) {
			gridMeshes [gridMeshes.Count - 1].GetComponent<GridMeshController> ().RemoveSquare (sqr.start, sqr.end);
		}
		renderedSquares.Clear ();
		foreach (ObjectInfo obj in objs) {
			Rect objRect = new Rect ();
			objRect.position = new Vector2 (obj.objRectCoord.x, obj.objRectCoord.y);
			Vector3 start = new Vector3 (obj.objRectCoord.x, obj.objRectCoord.y + obj.objRectHeight, -1); 
			Vector3 end = new Vector3 (obj.objRectCoord.x + obj.objRectWidth, obj.objRectCoord.y, -1);
			SquareCoords foundSquare = renderedSquares.Find (o => o.start == start && o.end == end);
			if(foundSquare == null){
				gridMeshes [gridMeshes.Count - 1].GetComponent<GridMeshController> ().DrawSquare (start, end, "Yellow");
				renderedSquares.Add (new SquareCoords ().SetData (start, end));
			}
		}
	}
		

	Rect GetRectBySelectedTile(Vector3 mousePosition){
		Rect rect = new Rect ();
		float decorWidth = Vector2.Distance (tileList.selectedUV [0], tileList.selectedUV [1]);
		float decorHeight = Vector2.Distance (tileList.selectedUV [1], tileList.selectedUV [2]);

		float tileWidth = 128 * (1 / tileList.selectedTileTextureWidth);
		float tileHeight = 128 * (1 / tileList.selectedTileTextureHeight);

		rect.width = decorWidth / tileWidth;
		rect.height = decorHeight / tileHeight;
		rect.center = new Vector2 (mousePosition.x, mousePosition.y);
		return rect;
	}

	List<Vector3> GetVerticesCoordsByRect(Rect squareRect){
		List<Vector3> vertices = new List<Vector3> ();
		vertices.Add (new Vector3 (squareRect.center.x - (squareRect.width / 2), squareRect.center.y + (squareRect.height / 2)));
		vertices.Add (new Vector3 (squareRect.center.x + (squareRect.width / 2), squareRect.center.y + (squareRect.height / 2)));
		vertices.Add (new Vector3 (squareRect.center.x + (squareRect.width / 2), squareRect.center.y - (squareRect.height / 2)));
		vertices.Add (new Vector3 (squareRect.center.x - (squareRect.width / 2), squareRect.center.y - (squareRect.height / 2)));
		return vertices;
	}

	Rect DownStickingByRect(Rect rect){
		int positionStartY = (int)rect.center.y;
		int positionEndY = positionStartY - 1;


		float downPosition = rect.center.y - rect.height / 2;
		float upPosition = downPosition + rect.height;
		float distanceY = Math.Abs(positionEndY - downPosition);

		int coeff = -1;
		if(downPosition < positionEndY && upPosition > positionEndY){
			coeff = 1;
		}
		if(downPosition < positionStartY && upPosition > positionStartY){
			coeff = -1;
		}
		if(downPosition < positionEndY && upPosition > positionStartY){
			coeff = 1;
		}

		Rect newRect = rect;
		newRect.center = new Vector2 (rect.center.x, rect.center.y + coeff*distanceY);
		return newRect;

	}

	Rect UpStickingByRect(Rect rect){
		int positionStartY = (int)rect.center.y;
		int positionEndY = positionStartY - 1;

		float upPosition = (rect.y + rect.height);
		float downPosition = rect.position.y;
		float distanceY = Math.Abs(upPosition - positionStartY);

		int coeff = 1;
		if(upPosition > positionEndY && downPosition < positionEndY){
			coeff = 1;
		}
		if(upPosition > positionStartY && downPosition < positionStartY){
			coeff = -1;
		}

		Rect newRect = rect;
		newRect.center = new Vector2 (rect.center.x, (rect.center.y + coeff*distanceY));
		return newRect;

	}

	Rect LeftStickingByRect(Rect rect){
		int positionStartX = (int)rect.center.x;
		int positionEndX = positionStartX + 1;

		float leftPosition = rect.position.x;
		float rightPosition = rect.position.x + rect.width;
		float distanceX = Math.Abs(leftPosition - positionStartX);

		int coeff = -1;
		if(leftPosition < positionEndX && rightPosition > positionEndX){
			coeff = -1;
		}
		if(leftPosition < positionStartX && rightPosition > positionStartX){
			coeff = 1;
		}

		Rect newRect = rect;
		newRect.center = new Vector2 (rect.center.x + coeff*distanceX, rect.center.y);
		return newRect;
	}

	Rect RightStickingByRect(Rect rect){
		int positionStartX = (int)rect.center.x;
		int positionEndX = positionStartX + 1;

		float leftPosition = rect.position.x;
		float rightPosition = rect.position.x + rect.width;
		float distanceX = Math.Abs(positionEndX - rightPosition);

		int coeff = 1;
		if(leftPosition < positionEndX && rightPosition > positionEndX){
			coeff = -1;
		}
		if(leftPosition < positionStartX && rightPosition > positionStartX){
			coeff = 1;
		}

		Rect newRect = rect;
		newRect.center = new Vector2 (rect.center.x + coeff*distanceX, rect.center.y);
		return newRect;
	}

	public Rect GetRectBySelectedObject(Vector3 mousePosition){
		Rect intObjRect = new Rect ();
		GameObject selectedInveractiveObject = (GameObject)interactiveObjectPrefabs [ListElementsController.selectedInteractiveObject];
		intObjRect.width = selectedInveractiveObject.GetComponent<BoxCollider> ().size.x * selectedInveractiveObject.transform.localScale.x;
		intObjRect.height = selectedInveractiveObject.GetComponent<BoxCollider> ().size.y * selectedInveractiveObject.transform.localScale.y;
		intObjRect.center = new Vector2 (mousePosition.x, mousePosition.y);
		return intObjRect;
	}


	private class SquareCoords{
		public Vector3 start = new Vector3();
		public Vector3 end = new Vector3();
		public bool startDefined = false;

		public SquareCoords SetData(Vector3 startS, Vector3 endS, bool startDefinedS = false){
			start = startS;
			end = endS;
			startDefined = startDefinedS;
			return this;
		}
	}


}
