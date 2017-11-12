using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

[System.Serializable]
public class LevelMesh : MonoBehaviour {

	public List<Vector3> newVertices = new List<Vector3>();
	private List<int> newTriangles = new List<int>();

	public List<Vector3> colVertices = new List<Vector3>();
	public List<int> colTriangles = new List<int>();

	public List<Vector2> newUV = new List<Vector2> ();
	private Mesh mesh;
	private Mesh colMesh;
	private MeshCollider col;

	private int squareCount;
	private int colCount;

	//private int squareCoordsIndex;
	private int colliderIndex;
	private GameObject meshGameObject;

	private LevelMeshDataSerializable serializableData = new LevelMeshDataSerializable();


	public void Awake(){
		mesh = GetComponent<MeshFilter> ().mesh;
		colMesh = new Mesh ();
		col = GetComponent<MeshCollider> ();
	}

	public void GenTile(float x, float y, float z, List<Vector2> uvVertices){
		GenSquare (x, y, z, uvVertices);
		GenCollider (x, y, z, 1);
		UpdateMesh ();
	}

	public void RemoveTile(float x, float y, float z){
		RemoveSquare (x, y, z);
		RemoveCollider (x, y, z);
		UpdateMesh ();
	}


	void UpdateMesh(){
		mesh.Clear ();
		mesh.vertices = newVertices.ToArray();
		mesh.triangles = newTriangles.ToArray();
		mesh.uv = newUV.ToArray ();
		;
		mesh.RecalculateNormals ();

		colMesh.Clear ();
		colMesh.vertices = colVertices.ToArray();
		colMesh.triangles = colTriangles.ToArray();
		;
		colMesh.RecalculateNormals ();


		col.sharedMesh = colMesh;
		if (colCount == 5) {
			col.convex = true;
			col.convex = false;
		}
	}

	public void GenSquareByCoords(List<Vector3> squareVerticesCoords, List<Vector2> uvVertices){
		foreach(Vector3 vertice in squareVerticesCoords){
			newVertices.Add (vertice);
		}

		newTriangles.Add (squareCount * 4);
		newTriangles.Add ((squareCount * 4)+1);
		newTriangles.Add ((squareCount * 4)+3);
		newTriangles.Add ((squareCount * 4)+1);
		newTriangles.Add ((squareCount * 4)+2);
		newTriangles.Add ((squareCount * 4)+3);





		newUV.Add (uvVertices[0]);
		newUV.Add (uvVertices[1]);
		newUV.Add (uvVertices[2]);
		newUV.Add (uvVertices[3]);

		squareCount++;
		UpdateMesh ();
	}

	public void RemoveSquareByCoords(List<Vector3> coords){
		int vertex1 = GetSquareIndexByVertices (coords);
		//Debug.Log (vertex1);
		if (vertex1 != -1) {
			newVertices.RemoveAt (vertex1);
			newVertices.RemoveAt (vertex1);
			newVertices.RemoveAt (vertex1);
			newVertices.RemoveAt (vertex1);

			newUV.RemoveAt (vertex1);
			newUV.RemoveAt (vertex1);
			newUV.RemoveAt (vertex1);
			newUV.RemoveAt (vertex1);

			squareCount--;

			newTriangles.Clear ();
			for (int i = 0; i < squareCount; i++) {
				newTriangles.Add (i * 4);
				newTriangles.Add ((i * 4) + 1);
				newTriangles.Add ((i * 4) + 3);
				newTriangles.Add ((i * 4) + 1);
				newTriangles.Add ((i * 4) + 2);
				newTriangles.Add ((i * 4) + 3);
			}
			UpdateMesh ();
		}
	}

	int GetSquareIndexByVertices(List<Vector3> coords){
		int squareCoordsIndex = -1;
		for(int i =0;i< newVertices.Count;i++){
			if(newVertices.Count - 3 > i){
				if ((newVertices [i].x == coords[0].x && newVertices [i].y == coords[0].y && newVertices [i].z == coords[0].z) &&
					(newVertices [i + 1].x == coords[1].x && newVertices [i+1].y == coords[1].y && newVertices [i+1].z == coords[1].z) &&
					(newVertices [i + 2].x == coords[2].x  && newVertices [i+2].y == coords[2].y  && newVertices [i+2].z == coords[2].z) &&
					(newVertices [i + 3].x == coords[3].x && newVertices [i+3].y == coords[3].y && newVertices [i+3].z == coords[3].z)) {
					squareCoordsIndex = i;
				}
			}
		}
		return squareCoordsIndex;	
	}

	void GenSquare(float x, float y, float z, List<Vector2> uvVertices){

		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x+1, y, z));
		newVertices.Add (new Vector3 (x+1, y-1,z));
		newVertices.Add (new Vector3 (x, y-1, z));

		/*newVertices.Add (new Vector3 (x+1, y, z));
		newVertices.Add (new Vector3 (x, y, z));
		newVertices.Add (new Vector3 (x, y-1, z));
		newVertices.Add (new Vector3 (x+1, y-1,z));*/

		newTriangles.Add (squareCount * 4);
		newTriangles.Add ((squareCount * 4)+1);
		newTriangles.Add ((squareCount * 4)+3);
		newTriangles.Add ((squareCount * 4)+1);
		newTriangles.Add ((squareCount * 4)+2);
		newTriangles.Add ((squareCount * 4)+3);

		newUV.Add (uvVertices[0]);
		newUV.Add (uvVertices[1]);
		newUV.Add (uvVertices[2]);
		newUV.Add (uvVertices[3]);

		squareCount++;
	}

	void GenCollider(float x, float y, float z, float size){
		//Top
		colVertices.Add (new Vector3 (x, y, z));
		colVertices.Add (new Vector3 (x+size, y, z));
		colVertices.Add (new Vector3 (x+size, y,z-size));
		colVertices.Add (new Vector3 (x, y, z-size));

		colTriangles.Add (colCount * 4);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+3);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+2);
		colTriangles.Add ((colCount * 4)+3);

		colCount++;


		//Left
		colVertices.Add (new Vector3 (x, y, z));
		colVertices.Add (new Vector3 (x, y, z-size));
		colVertices.Add (new Vector3 (x, y-size,z-size));
		colVertices.Add (new Vector3 (x, y-size, z));

		colTriangles.Add (colCount * 4);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+3);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+2);
		colTriangles.Add ((colCount * 4)+3);

		colCount++;

		//Right
		colVertices.Add (new Vector3 (x+size, y, z));
		colVertices.Add (new Vector3 (x+size, y, z-size));
		colVertices.Add (new Vector3 (x+size, y-size,z-size));
		colVertices.Add (new Vector3 (x+size, y-size, z));

		colTriangles.Add (colCount * 4);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+3);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+2);
		colTriangles.Add ((colCount * 4)+3);

		colCount++;

		//Bottom
		colVertices.Add (new Vector3 (x, y-size, z));
		colVertices.Add (new Vector3 (x+size, y-size, z));
		colVertices.Add (new Vector3 (x+size, y-size,z-size));
		colVertices.Add (new Vector3 (x, y-size, z-size));

		colTriangles.Add (colCount * 4);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+3);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+2);
		colTriangles.Add ((colCount * 4)+3);

		colCount++;



		//SquareCollider
		colVertices.Add (new Vector3 (x, y, z));
		colVertices.Add (new Vector3 (x+size, y, z));
		colVertices.Add (new Vector3 (x+size, y-size,z));
		colVertices.Add (new Vector3 (x, y-size, z));

		colTriangles.Add (colCount * 4);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+3);
		colTriangles.Add ((colCount * 4)+1);
		colTriangles.Add ((colCount * 4)+2);
		colTriangles.Add ((colCount * 4)+3);

		colCount++;
	}


	void RemoveSquare(float x, float y, float z){
		int vertex1 = GetSquareIndex(x, y, z);

		newVertices.RemoveAt (vertex1);
		newVertices.RemoveAt (vertex1);
		newVertices.RemoveAt (vertex1);
		newVertices.RemoveAt (vertex1);

		newUV.RemoveAt (vertex1);
		newUV.RemoveAt (vertex1);
		newUV.RemoveAt (vertex1);
		newUV.RemoveAt (vertex1);

		squareCount--;

		newTriangles.Clear ();
		for (int i = 0; i < squareCount; i++) {
			newTriangles.Add (i * 4);
			newTriangles.Add ((i * 4)+1);
			newTriangles.Add ((i * 4)+3);
			newTriangles.Add ((i * 4)+1);
			newTriangles.Add ((i * 4)+2);
			newTriangles.Add ((i * 4)+3);
		}
	}

	void RemoveCollider(float x, float y, float z){
		GetColliderCoordIndex (x,y,z);
		//Remove vertices
		for (int i = 0; i < 4 * 5; i++) {
			colVertices.RemoveAt (colliderIndex);	
		}
		colCount -= 5;

		//Remove triangles
		colTriangles.Clear ();
		for (int i = 0; i < colCount; i++) {
			colTriangles.Add (i * 4);
			colTriangles.Add ((i * 4)+1);
			colTriangles.Add ((i * 4)+3);
			colTriangles.Add ((i * 4)+1);
			colTriangles.Add ((i * 4)+2);
			colTriangles.Add ((i * 4)+3);
		}
	}


	int GetSquareIndex(float x, float y, float z){
		int squareCoordsIndex = -1;
		for(int i =0;i< newVertices.Count;i++){
			if(newVertices.Count - 3 > i){
				if ((newVertices [i].x == x && newVertices [i].y == y && newVertices [i].z == z) &&
					(newVertices [i + 1].x == x + 1 && newVertices [i+1].y == y && newVertices [i+1].z == z) &&
					(newVertices [i + 2].x == x + 1 && newVertices [i+2].y == y - 1 && newVertices [i+2].z == z) &&
					(newVertices [i + 3].x == x && newVertices [i+3].y == y - 1 && newVertices [i+3].z == z)) {
					squareCoordsIndex = i;
				}
			}
		}
		return squareCoordsIndex;
	}


	void GetColliderCoordIndex(float x, float y, float z){
		//top
		for (int i = 0; i < colVertices.Count; i++) {
			if (colVertices.Count - 19 > i) {
				//top
				if ((colVertices [i].x == x && colVertices [i].y == y && colVertices [i].z == z) &&
					(colVertices [i + 1].x == x + 1 && colVertices [i + 1].y == y && colVertices [i + 1].z == z) &&
					(colVertices [i + 2].x == x + 1 && colVertices [i + 2].y == y && colVertices [i + 2].z == z - 1) &&
					(colVertices [i + 3].x == x && colVertices [i + 3].y == y && colVertices [i + 3].z == z - 1) &&
					//left
					(colVertices [i + 4].x == x && colVertices [i + 4].y == y && colVertices [i + 4].z == z) &&
					(colVertices [i + 5].x == x && colVertices [i + 5].y == y && colVertices [i + 5].z == z - 1) &&
					(colVertices [i + 6].x == x && colVertices [i + 6].y == y - 1 && colVertices [i + 6].z == z - 1) &&
					(colVertices [i + 7].x == x && colVertices [i + 7].y == y - 1 && colVertices [i + 7].z == z) &&
					//Right
					(colVertices [i + 8].x == x + 1 && colVertices [i + 8].y == y && colVertices [i + 8].z == z) &&
					(colVertices [i + 9].x == x + 1 && colVertices [i + 9].y == y && colVertices [i + 9].z == z - 1) &&
					(colVertices [i + 10].x == x + 1 && colVertices [i + 10].y == y - 1 && colVertices [i + 10].z == z - 1) &&
					(colVertices [i + 11].x == x + 1 && colVertices [i + 11].y == y - 1 && colVertices [i + 11].z == z) &&
					//Bottom
					(colVertices [i + 12].x == x && colVertices [i + 12].y == y - 1 && colVertices [i + 12].z == z) &&
					(colVertices [i + 13].x == x + 1 && colVertices [i + 13].y == y - 1 && colVertices [i + 13].z == z) &&
					(colVertices [i + 14].x == x + 1 && colVertices [i + 14].y == y - 1 && colVertices [i + 14].z == z - 1) &&
					(colVertices [i + 15].x == x && colVertices [i + 15].y == y - 1 && colVertices [i + 15].z == z - 1) &&
					//Square
					(colVertices [i + 16].x == x && colVertices [i + 16].y == y && colVertices [i + 16].z == z) &&
					(colVertices [i + 17].x == x + 1 && colVertices [i + 17].y == y && colVertices [i + 17].z == z) &&
					(colVertices [i + 18].x == x + 1 && colVertices [i + 18].y == y - 1 && colVertices [i + 18].z == z) &&
					(colVertices [i + 19].x == x && colVertices [i + 19].y == y - 1 && colVertices [i + 19].z == z)) {

					colliderIndex = i;
				}
			}
		}
	}
		

	public LevelMeshDataSerializable ToSerialize(){
		serializableData.squareVerticesSerializeble.Clear ();
		serializableData.squareUVSerializeble.Clear ();
		serializableData.colliderVerticesSerializeble.Clear ();
		for (int i = 0; i < newVertices.Count; i++) {
			serializableData.squareVerticesSerializeble.Add (new SerializableVector3 (newVertices [i].x, newVertices [i].y, newVertices [i].z));	
		}

		for (int i = 0; i < newUV.Count; i++) {
			serializableData.squareUVSerializeble.Add (new SerializableVector2 (newUV [i].x, newUV [i].y));	
		}

		for (int i = 0; i < colVertices.Count; i++) {
			serializableData.colliderVerticesSerializeble.Add (new SerializableVector3 (colVertices [i].x, colVertices [i].y, colVertices [i].z));	
		}

		//serializableData.squareVerticesSerializeble.Add (new SerializableVector3(10f, 3f,7f));
		//Debug.Log (serializableData.squareVerticesSerializeble[0].x + ";"+serializableData.squareVerticesSerializeble[0].y + ";"+serializableData.squareVerticesSerializeble[0].z);
		/*SerializableVector3 lol = new SerializableVector3(10f, 3f,7f);
		Debug.Log (lol.x);*/
		return serializableData;

	}

	public void SetData(LevelMeshDataSerializable data){
		newVertices.Clear ();
		newUV.Clear ();
		colVertices.Clear ();
		newTriangles.Clear ();
		colTriangles.Clear ();
		squareCount = 0;

		for (int i = 0; i < data.squareVerticesSerializeble.Count; i++) {
			newVertices.Add (new Vector3 (data.squareVerticesSerializeble [i].x, data.squareVerticesSerializeble [i].y, data.squareVerticesSerializeble [i].z));
			if ((i + 4) % 4 == 0) {
				newTriangles.Add (squareCount * 4);
				newTriangles.Add ((squareCount * 4)+1);
				newTriangles.Add ((squareCount * 4)+3);
				newTriangles.Add ((squareCount * 4)+1);
				newTriangles.Add ((squareCount * 4)+2);
				newTriangles.Add ((squareCount * 4)+3);
				squareCount++;
			}
		}

		for (int i = 0; i < data.squareUVSerializeble.Count; i++) {
			newUV.Add (new Vector2 (data.squareUVSerializeble [i].x, data.squareUVSerializeble [i].y));
		}

		for (int i = 0; i < data.colliderVerticesSerializeble.Count; i++) {
			colVertices.Add (new Vector3 (data.colliderVerticesSerializeble [i].x, data.colliderVerticesSerializeble [i].y, data.colliderVerticesSerializeble [i].z));
			if ((i + 4) % 4 == 0) {
				colTriangles.Add (colCount * 4);
				colTriangles.Add ((colCount * 4)+1);
				colTriangles.Add ((colCount * 4)+3);
				colTriangles.Add ((colCount * 4)+1);
				colTriangles.Add ((colCount * 4)+2);
				colTriangles.Add ((colCount * 4)+3);
				colCount++;
			}
		}

		UpdateMesh ();

	}

	[System.Serializable]
	public class LevelMeshDataSerializable {
		public List<SerializableVector3> squareVerticesSerializeble  = new List<SerializableVector3>();
		public List<SerializableVector2> squareUVSerializeble  = new List<SerializableVector2>();
		public List<SerializableVector3> colliderVerticesSerializeble  = new List<SerializableVector3>();	
	}
}
