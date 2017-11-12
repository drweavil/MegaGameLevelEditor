using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoadManager {
	// Use this for initialization
	//private static Level meshData = Level;
	public static LevelRudiment levelSerialize;
	private static List<LevelMesh> meshes = new List<LevelMesh> ();


	public static List<string> Save(string disk, string texture, string type, string number, bool rewrite = false){
		List<string> errors = new List<string> ();
		if (disk == "" || disk == null) {
			errors.Add ("Введи диск, слепошарый!");
		}
		if (type == "" || type == null) {
			errors.Add ("Забыл тип, бестолочь!");
		}
		if (number == "" || number == null) {
			errors.Add ("А номер кто вводить будет?!");
		}
		if (type != "" && type != null) {
			if (!levelSerialize.PortalsTypeCheck (int.Parse (type))) {
				errors.Add ("Мини-порталы не соответствуют типу");
			}
		}

		if (!levelSerialize.WallWithoutHolesCheck ()) {
			errors.Add ("Дыры в стенах залатай!");
		}
		/*if (size == "" || size == null) {
			errors.Add ("Размер где? А?А?А?");
		}*/

		if (errors.Count == 0) {
			levelSerialize.SetEnemiesCount ();
			string directory = disk + ":/MegaGameLevels/" + texture +  "/" + type;
			string path = directory + "/" + number + ".level";
			if (!Directory.Exists (directory)) {
				Directory.CreateDirectory (directory);
			}
			FileStream file;


			if (File.Exists (path)) {
				if (rewrite) {
					File.Delete (path);
					file = File.Create (path);
					BinaryFormatter bf = new BinaryFormatter ();
					levelSerialize.textureName = texture;
					bf.Serialize (file, levelSerialize);
					file.Close ();
				} else {
					errors.Add ("rewriteRequest");
				}
			} else {
				file = File.Create (path);
				BinaryFormatter bf = new BinaryFormatter ();
				levelSerialize.textureName = texture;
				bf.Serialize (file, levelSerialize);
				file.Close ();
			}
		}
		return errors;
	}

	public static List<string> Load(string disk, string texture, string type, string number){
		string directory = disk + ":/MegaGameLevels/" + texture +  "/" + type;
		string path = directory + "/" + number + ".level";

		List<string> errors = new List<string> ();
		if (disk == "" || disk == null) {
			errors.Add ("Введи диск, слепошарый!");
		}
		if (type == "" || type == null) {
			errors.Add ("Забыл тип, бестолочь!");
		}
		if (number == "" || number == null) {
			errors.Add ("А номер кто вводить будет?!");
		}
		/*if (size == "" || size == null) {
			errors.Add ("Размер где? А?А?А?");
		}*/

		if (errors.Count == 0) {
			LevelRudiment level;
			FileStream file;
			BinaryFormatter bf = new BinaryFormatter ();
			if (File.Exists (path)) {
				//level.levelBlocks.Clear ();
				file = File.Open (path, FileMode.Open);
				BlockController.bc.level = (LevelRudiment)bf.Deserialize (file);
				BlockController.bc.DrawLevel ();
				file.Close ();
			} else {
				errors.Add ("Такого файла нема =(");
			}
		}

		return errors;

	}
}
