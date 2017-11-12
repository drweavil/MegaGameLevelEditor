using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GridMeshController : MonoBehaviour {


	private Mesh mesh;
	public List<Vector3> lineVertices = new List<Vector3> ();
	public List<Vector2> lineUV = new List<Vector2>();
	public List<int> lineTriangles = new List<int>();
	private int lineCount = 0;


	private Hashtable colorsUvCoords = new Hashtable ();


	Vector3 start;
	Vector3 end;

	// Use this for initialization
	void Awake () {
		mesh = GetComponent<MeshFilter> ().mesh;



		int elementWidth = 32;
		int elementHeigh = 32;
		float pixelWidthSize = 1f / 128f;
		float pixelHeightSize = 1f / 128f;

		List<Vector2> uvs = new List<Vector2> ();
		int i = 2;
		int j = 2;
		uvs.Clear();
		uvs.Add (new Vector2((float)i*elementWidth*pixelWidthSize, (float)j*elementHeigh*pixelHeightSize));
		uvs.Add (new Vector2((float)(i*elementWidth+elementWidth)*pixelWidthSize, (float)j*elementHeigh*pixelHeightSize));
		uvs.Add (new Vector2((float)(i*elementWidth+elementWidth)*pixelWidthSize, (float)(j*elementHeigh+elementHeigh)*pixelHeightSize));
		uvs.Add (new Vector2((float)i*elementWidth*pixelWidthSize, (float)(j*elementHeigh+elementHeigh)*pixelHeightSize));

		colorsUvCoords.Add("Yellow", uvs);

		i = 2;
		j = 1;
		uvs.Clear();
		uvs.Add (new Vector2((float)i*elementWidth*pixelWidthSize, (float)j*elementHeigh*pixelHeightSize));
		uvs.Add (new Vector2((float)(i*elementWidth+elementWidth)*pixelWidthSize, (float)j*elementHeigh*pixelHeightSize));
		uvs.Add (new Vector2((float)(i*elementWidth+elementWidth)*pixelWidthSize, (float)(j*elementHeigh+elementHeigh)*pixelHeightSize));
		uvs.Add (new Vector2((float)i*elementWidth*pixelWidthSize, (float)(j*elementHeigh+elementHeigh)*pixelHeightSize));

		colorsUvCoords.Add("Green", uvs);



		/*DrawLine (new Vector3 (0, 0, 0), new Vector3 (1, 1, 0), 0.1f, "Green");*/
		/*start = new Vector3 (0, 1, 0);
		end = new Vector3 (1, 0, 0);
		DrawSquare (start, end, "Green");
		DrawSquare (start.x, end, "Green");*/
	
	}
	
	// Update is called once per frame
	void Update () {
		/*if (Input.GetKeyDown (KeyCode.Z)) {
			//RemoveSquare (start, end);
		}*/
	}

	public void DrawSquare(Vector3 start, Vector3 end, string color){
		DrawLine (start, new Vector3 (end.x, start.y, end.z), 0.05f, color);
		DrawLine (new Vector3(end.x, start.y, end.z), end, 0.05f, color);
		DrawLine (end, new Vector3 (start.x, end.y, end.z), 0.05f, color);
		DrawLine (new Vector3 (start.x, end.y, end.z), start, 0.05f, color);
	}
		

	public void RemoveSquare(Vector3 start, Vector3 end){
		if (lineVertices.Count != 0) {
			List<Vector3> vertices = new List<Vector3> ();

			foreach (Vector3 vert in DrawLine (start, new Vector3 (end.x, start.y, end.z), 0.05f, "Green", true)) {
				vertices.Add (vert);
			}

			foreach (Vector3 vert in DrawLine (new Vector3(end.x, start.y, end.z), end, 0.05f, "Green", true)) {
				vertices.Add (vert);
			}

			foreach (Vector3 vert in DrawLine (end, new Vector3 (start.x, end.y, end.z), 0.05f, "Green", true)) {
				vertices.Add (vert);
			}

			foreach (Vector3 vert in DrawLine (new Vector3 (start.x, end.y, end.z), start, 0.05f, "Green", true)) {
				vertices.Add (vert);
			}

			int index = GetSquareIndex (vertices);
			//Debug.Log (index);
			if (index != -1) {
				for (int i = 0; i < 16; i++) {
					lineVertices.RemoveAt (index);
					lineUV.RemoveAt (index);
				}
				lineCount -= 4;

				lineTriangles.Clear ();
				for (int i = 0; i < lineCount; i++) {
					lineTriangles.Add (i * 4);
					lineTriangles.Add ((i * 4) + 1);
					lineTriangles.Add ((i * 4) + 3);
					lineTriangles.Add ((i * 4) + 1);
					lineTriangles.Add ((i * 4) + 2);
					lineTriangles.Add ((i * 4) + 3);
				}
				UpdateMesh ();
			}
		}
	}


	int GetSquareIndex(List<Vector3> vertices){
		int squareCoordsIndex = -1;
		//Debug.Log (vertices.Count);
		for(int i =0;i< lineVertices.Count;i++){
			//Debug.Log (i);
			if(i%16 == 0){
				int iv = i%16;
				if (/*(Math.Round(lineVertices [i].x, 1) == Math.Round(vertices[i].x, 1) && Math.Round(lineVertices [i].y, 1) == Math.Round(vertices[i].y, 1) && Math.Round(lineVertices [i].z, 1) == Math.Round(vertices[i].z, 1)) &&
					(Math.Round(lineVertices [i+1].x, 1) == Math.Round(vertices[i+1].x, 1) && Math.Round(lineVertices [i+1].y, 1) == Math.Round(vertices[i+1].y, 1) && Math.Round(lineVertices [i+1].z, 1) == Math.Round(vertices[i+1].z, 1)) &&
					(Math.Round(lineVertices [i+2].x, 1) == Math.Round(vertices[i+2].x, 1) && Math.Round(lineVertices [i+2].y, 1) == Math.Round(vertices[i+2].y, 1) && Math.Round(lineVertices [i+2].z, 1) == Math.Round(vertices[i+2].z, 1)) &&
					(Math.Round(lineVertices [i+3].x, 1) == Math.Round(vertices[i+3].x, 1) && Math.Round(lineVertices [i+3].y, 1) == Math.Round(vertices[i+3].y, 1) && Math.Round(lineVertices [i+3].z, 1) == Math.Round(vertices[i+3].z, 1)) &&
					(Math.Round(lineVertices [i+4].x, 1) == Math.Round(vertices[i+4].x, 1) && Math.Round(lineVertices [i+4].y, 1) == Math.Round(vertices[i+4].y, 1) && Math.Round(lineVertices [i+4].z, 1) == Math.Round(vertices[i+4].z, 1)) &&
					(Math.Round(lineVertices [i+5].x, 1) == Math.Round(vertices[i+5].x, 1) && Math.Round(lineVertices [i+5].y, 1) == Math.Round(vertices[i+5].y, 1) && Math.Round(lineVertices [i+5].z, 1) == Math.Round(vertices[i+5].z, 1)) &&
					(Math.Round(lineVertices [i+6].x, 1) == Math.Round(vertices[i+6].x, 1) && Math.Round(lineVertices [i+6].y, 1) == Math.Round(vertices[i+6].y, 1) && Math.Round(lineVertices [i+6].z, 1) == Math.Round(vertices[i+6].z, 1)) &&
					(Math.Round(lineVertices [i+7].x, 1) == Math.Round(vertices[i+7].x, 1) && Math.Round(lineVertices [i+7].y, 1) == Math.Round(vertices[i+7].y, 1) && Math.Round(lineVertices [i+7].z, 1) == Math.Round(vertices[i+7].z, 1)) &&
					(Math.Round(lineVertices [i+8].x, 1) == Math.Round(vertices[i+8].x, 1) && Math.Round(lineVertices [i+8].y, 1) == Math.Round(vertices[i+8].y, 1) && Math.Round(lineVertices [i+8].z, 1) == Math.Round(vertices[i+8].z, 1)) &&
					(Math.Round(lineVertices [i+9].x, 1) == Math.Round(vertices[i+9].x, 1) && Math.Round(lineVertices [i+9].y, 1) == Math.Round(vertices[i+9].y, 1) && Math.Round(lineVertices [i+9].z, 1) == Math.Round(vertices[i+9].z, 1)) &&
					(Math.Round(lineVertices [i+10].x, 1) == Math.Round(vertices[i+10].x, 1) && Math.Round(lineVertices [i+10].y, 1) == Math.Round(vertices[i+10].y, 1) && Math.Round(lineVertices [i+10].z, 1) == Math.Round(vertices[i+10].z, 1)) &&
					(Math.Round(lineVertices [i+11].x, 1) == Math.Round(vertices[i+11].x, 1) && Math.Round(lineVertices [i+11].y, 1) == Math.Round(vertices[i+11].y, 1) && Math.Round(lineVertices [i+11].z, 1) == Math.Round(vertices[i+11].z, 1)) &&
					(Math.Round(lineVertices [i+12].x, 1) == Math.Round(vertices[i+12].x, 1) && Math.Round(lineVertices [i+12].y, 1) == Math.Round(vertices[i+12].y, 1) && Math.Round(lineVertices [i+12].z, 1) == Math.Round(vertices[i+12].z, 1)) &&
					(Math.Round(lineVertices [i+13].x, 1) == Math.Round(vertices[i+13].x, 1) && Math.Round(lineVertices [i+13].y, 1) == Math.Round(vertices[i+13].y, 1) && Math.Round(lineVertices [i+13].z, 1) == Math.Round(vertices[i+13].z, 1)) &&
					(Math.Round(lineVertices [i+14].x, 1) == Math.Round(vertices[i+14].x, 1) && Math.Round(lineVertices [i+14].y, 1) == Math.Round(vertices[i+14].y, 1) && Math.Round(lineVertices [i+14].z, 1) == Math.Round(vertices[i+14].z, 1)) &&
					(Math.Round(lineVertices [i+15].x, 1) == Math.Round(vertices[i+15].x, 1) && Math.Round(lineVertices [i+15].y, 1) == Math.Round(vertices[i+15].y, 1) && Math.Round(lineVertices [i+15].z, 1) == Math.Round(vertices[i+15].z, 1))*/


					(lineVertices [i].x == vertices[iv].x && lineVertices [i].y == vertices[iv].y && lineVertices [i].z == vertices[iv].z) &&
					(lineVertices [i+1].x == vertices[iv+1].x && lineVertices [i+1].y == vertices[iv+1].y && lineVertices [i+1].z == vertices[iv+1].z) &&
					(lineVertices [i+2].x == vertices[iv+2].x && lineVertices [i+2].y == vertices[iv+2].y && lineVertices [i+2].z == vertices[iv+2].z) &&
					(lineVertices [i+3].x == vertices[iv+3].x && lineVertices [i+3].y == vertices[iv+3].y && lineVertices [i+3].z == vertices[iv+3].z) &&
					(lineVertices [i+4].x == vertices[iv+4].x && lineVertices [i+4].y == vertices[iv+4].y && lineVertices [i+4].z == vertices[iv+4].z) &&
					(lineVertices [i+5].x == vertices[iv+5].x && lineVertices [i+5].y == vertices[iv+5].y && lineVertices [i+5].z == vertices[iv+5].z) &&
					(lineVertices [i+6].x == vertices[iv+6].x && lineVertices [i+6].y == vertices[iv+6].y && lineVertices [i+6].z == vertices[iv+6].z) &&
					(lineVertices [i+7].x == vertices[iv+7].x && lineVertices [i+7].y == vertices[iv+7].y && lineVertices [i+7].z == vertices[iv+7].z) &&
					(lineVertices [i+8].x == vertices[iv+8].x && lineVertices [i+8].y == vertices[iv+8].y && lineVertices [i+8].z == vertices[iv+8].z) &&
					(lineVertices [i+9].x == vertices[iv+9].x && lineVertices [i+9].y == vertices[iv+9].y && lineVertices [i+9].z == vertices[iv+9].z) &&
					(lineVertices [i+10].x == vertices[iv+10].x && lineVertices [i+10].y == vertices[iv+10].y && lineVertices [i+10].z == vertices[iv+10].z) &&
					(lineVertices [i+11].x == vertices[iv+11].x && lineVertices [i+11].y == vertices[iv+11].y && lineVertices [i+11].z == vertices[iv+11].z) &&
					(lineVertices [i+12].x == vertices[iv+12].x && lineVertices [i+12].y == vertices[iv+12].y && lineVertices [i+12].z == vertices[iv+12].z) &&
					(lineVertices [i+13].x == vertices[iv+13].x && lineVertices [i+13].y == vertices[iv+13].y && lineVertices [i+13].z == vertices[iv+13].z) &&
					(lineVertices [i+14].x == vertices[iv+14].x && lineVertices [i+14].y == vertices[iv+14].y && lineVertices [i+14].z == vertices[iv+14].z) &&
					(lineVertices [i+15].x == vertices[iv+15].x && lineVertices [i+15].y == vertices[iv+15].y && lineVertices [i+15].z == vertices[iv+15].z)
				) {
					squareCoordsIndex = i;
				}
			}
		}
		return squareCoordsIndex;
	}


	List<Vector3> DrawLine(Vector3 start, Vector3 end, float width, string color, bool withoutDraw = false){
		List<Vector3> vertices = new List<Vector3> ();
		width = width / 2;
		Vector3 upNormal = Vector3.Cross (start - new Vector3 (start.x, start.y, start.z - 1), end - start);
		upNormal.Normalize ();

		Vector3 downNormal = Vector3.Cross (end - start, start - new Vector3 (start.x, start.y, start.z - 1));
		downNormal.Normalize ();

		if (!withoutDraw) {
			lineVertices.Add (upNormal * width + start);
			lineVertices.Add (upNormal * width + end);
			lineVertices.Add (downNormal * width + end);
			lineVertices.Add (downNormal * width + start);

			lineTriangles.Add (lineCount * 4);
			lineTriangles.Add ((lineCount * 4) + 1);
			lineTriangles.Add ((lineCount * 4) + 3);
			lineTriangles.Add ((lineCount * 4) + 1);
			lineTriangles.Add ((lineCount * 4) + 2);
			lineTriangles.Add ((lineCount * 4) + 3);


			List<Vector2> colorUvs = (List<Vector2>)colorsUvCoords [color];
			lineUV.Add (colorUvs [0]);
			lineUV.Add (colorUvs [1]);
			lineUV.Add (colorUvs [2]);
			lineUV.Add (colorUvs [3]);

			//lineUV.Add ((Vector2)colorsUvCoords[color]));
			UpdateMesh ();
			/*Debug.Log (lol[0]);*/

			lineCount++;
		}

		vertices.Add (upNormal * width + start);
		vertices.Add (upNormal * width + end);
		vertices.Add (downNormal * width + end);
		vertices.Add (downNormal * width + start);


		return vertices;
	}

	void UpdateMesh(){
		mesh.Clear ();
		mesh.vertices = lineVertices.ToArray();
		mesh.triangles = lineTriangles.ToArray();
		mesh.uv = lineUV.ToArray ();
		;
		mesh.RecalculateNormals ();
	}
}
