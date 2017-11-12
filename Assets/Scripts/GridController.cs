using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GridController : MonoBehaviour {
	public static GridController gridController;
	private LineRenderer line;
	//private int lineIndex=0;
	public int gridWidth=70;
	public int gridHeight=70;

	public Text widthField;
	public Text heightField;

	public InputField widthInputField;
	public InputField heightInputField;


	private Mesh mesh;
	private List<Vector3> lineVertices = new List<Vector3>();
	private List<int> lineTriangles = new List<int>();
	private List<Vector2> lineUV = new List<Vector2>();
	private int squareCount;
	// Use this for initialization


	void Awake(){
		gridController = this;
		widthInputField.text = gridWidth.ToString ();
		heightInputField.text = gridHeight.ToString ();
	}

	void Start () {
		mesh = GetComponent<MeshFilter> ().mesh;

		RedrawGrid ();

	}

	public void SetWidth(){
		int width = 0; 
		//Debug.Log (widthField.text);
		System.Int32.TryParse(widthField.text, out width);
		if (width == 0) {
			width = 1;
		}
		gridWidth = width;
		RedrawGrid ();
		BlockController.bc.width = width;
		BlockController.bc.level.levelWidth = width;
	}

	public void SetHeight(){
		int height = 0;
		System.Int32.TryParse(heightField.text, out height);
		if (height == 0) {
			height = 1;
		}
		gridHeight =  height;
		RedrawGrid ();

		BlockController.bc.height = height;
		BlockController.bc.level.levelHeight = height;
	}

	public void RedrawGrid(){
		ClearMesh ();

		for(int i=0; i<gridWidth+1; i++){
			DrawLine (new Vector3 (i , 0, 0.01f), new Vector3(i,-gridHeight, 0.01f), 0.1f);
		}

		for(int i=0; i<gridHeight+1; i++){
			DrawLine (new Vector3 (0, i * -1, 0.01f), new Vector3 (gridWidth, i * -1, 0.01f), 0.1f);
		}
		UpdateMesh ();
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


	void ClearMesh(){
		mesh.Clear ();
		lineVertices.Clear ();
		lineTriangles.Clear ();
		lineUV.Clear ();
		squareCount = 0;
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
