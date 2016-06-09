using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour {

	private LineRenderer line;
	//private int lineIndex=0;
	private int gridWidth=70;
	private int gridHeight=70;





	private Mesh mesh;
	private List<Vector3> lineVertices = new List<Vector3>();
	private List<int> lineTriangles = new List<int>();
	private List<Vector2> lineUV = new List<Vector2>();
	private int squareCount;
	// Use this for initialization
	void Start () {
		mesh = GetComponent<MeshFilter> ().mesh;

		for(int i=0; i<gridWidth+1; i++){
			DrawLine (new Vector3 (i , 0, 0.01f), new Vector3(i,-gridHeight, 0.01f), 0.1f);
		}

		for(int i=0; i<gridHeight+1; i++){
			DrawLine (new Vector3 (0, i * -1, 0.01f), new Vector3 (gridWidth, i * -1, 0.01f), 0.1f);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void DrawLine(Vector3 start, Vector3 end, float width){
		width = width / 2;
		Vector3 upNormal = Vector3.Cross (start - new Vector3 (start.x, start.y, start.z - 1), end - start);
		upNormal.Normalize ();

		Vector3 downNormal = Vector3.Cross (end - start, start - new Vector3 (start.x, start.y, start.z - 1));
		downNormal.Normalize ();

		lineVertices.Add (upNormal * width + start);
		lineVertices.Add (upNormal * width + end);
		lineVertices.Add (downNormal * width + end);
		lineVertices.Add (downNormal * width + start);

		lineTriangles.Add (squareCount * 4);
		lineTriangles.Add ((squareCount * 4)+1);
		lineTriangles.Add ((squareCount * 4)+3);
		lineTriangles.Add ((squareCount * 4)+1);
		lineTriangles.Add ((squareCount * 4)+2);
		lineTriangles.Add ((squareCount * 4)+3);

		lineUV.Add (new Vector2 (0,0));
		lineUV.Add (new Vector2 (0,0));
		lineUV.Add (new Vector2 (0,0));
		lineUV.Add (new Vector2 (0,0));
		UpdateMesh ();

		squareCount++;
	}


	void UpdateMesh(){
		mesh.Clear ();
		mesh.vertices = lineVertices.ToArray();
		mesh.triangles = lineTriangles.ToArray();
		mesh.uv = lineUV.ToArray ();
		mesh.Optimize ();
		mesh.RecalculateNormals ();
	}


}
