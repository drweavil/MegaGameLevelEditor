using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.IO;

public class Tools : MonoBehaviour {
	public static Tools tools;

	void Awake(){
		tools = this;
	}

	void Update(){
		if(Input.GetKeyDown(KeyCode.I)){
			SetTileSet (0);
			SetTileSet (1);
			SetTileSet (2);
			SetTileSet (3);
			SetTileSet (4);
			SetTileSet (5);
			SetTileSet (6);
			SetTileSet (7);
			SetTileSet (8);
			SetTileSet (9);
		}
	}

	public void SetTileSet(int id){
		FileStream file;
		//file = File.Open (Application.dataPath + "/Resources/TileSets/EditorNameIDtileSet_" + id.ToString(), FileMode.Open);
		string jsonString = File.ReadAllText (Application.dataPath + "/Resources/TileSets/EditorNameIDtileSet_" + id.ToString() + ".txt");
		//Debug.Log(Application.dataPath);
		JSONNode jsonFile = JSON.Parse(jsonString); 
		//Debug.Log (jsonFile ["maxID"]);

		int maxID = System.Int32.Parse (jsonFile ["maxID"]);
		//JSONArray array = jsonFile ["names"].AsArray;



		JSONNode idUVs = JSONNode.Parse("{}");



		//Dictionary<string, >

		Sprite[] sprites = Resources.LoadAll<Sprite> ("TileSets/" + "set_" + id.ToString());
		foreach (Sprite spr in sprites) {

			//JSONArray uvsArray;
			if (jsonFile ["names"] [spr.name] == null) {
				JSONArray uvsArray = JSONNode.Parse("{a:[]}")["a"].AsArray;

				foreach (Vector2 vector in ListElementsController.listElementsController.GetUvsByName (id.ToString(), spr.name.ToString())) {
					JSONNode vectorNode = JSONNode.Parse ("{}");
					vectorNode ["x"] = vector.x;
					vectorNode ["y"] = vector.y;

					uvsArray.Add (vectorNode);
					
				}


				idUVs[maxID.ToString()] = uvsArray;
			} else {
				JSONArray uvsArray = JSONNode.Parse("{a:[]}")["a"].AsArray;

				foreach (Vector2 vector in ListElementsController.listElementsController.GetUvsByName (id.ToString(), spr.name.ToString())) {
					JSONNode vectorNode = JSONNode.Parse ("{}");
					vectorNode ["x"] = vector.x;
					vectorNode ["y"] = vector.y;

					uvsArray.Add (vectorNode);

				}


				idUVs[jsonFile ["names"] [spr.name].Value] = uvsArray;


				/*idUVs [jsonFile ["names"] [spr.name].Value] ["x"] = vector.x;
				idUVs [jsonFile ["names"] [spr.name].Value] ["y"] = vector.y;*/
			}




			//Debug.Log(jsonFile ["names"] [spr.name]);
			if (jsonFile ["names"] [spr.name] == null) {
				jsonFile ["names"] [spr.name] = maxID;


				//ListElementsController.listElementsController.Get

				//Debug.Log (spr.name);


				maxID += 1;
				//idUVs[maxID] = maxID;

			}
		}

		jsonFile ["maxID"] = maxID;


		Debug.Log (idUVs.ToString());
		File.WriteAllText (Application.dataPath + "/Resources/TileSets/EditorNameIDtileSet_" + id.ToString() + ".txt", jsonFile.ToString());
		File.WriteAllText (Application.dataPath + "/Resources/TileSets/idUVsSet_" + id.ToString() + ".txt", idUVs.ToString());


		/*foreach (JSONNode node in jsonFile ["names"].AsArray) {
			Debug.Log (node.AsObject);
		}*/
	}

	/*public bool ContainName(JSONNode jsFl, string name){
		bool contain = false;
		if(jsFl[])
		jsFl.
	

		return contain;
	}*/
}
