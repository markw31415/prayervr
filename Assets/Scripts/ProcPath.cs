using UnityEngine;
using System.Collections.Generic;



public class ProcPath : MonoBehaviour {
	public List<Vector3> newVertices = new List<Vector3>();
	public List<Vector2> newUV = new List<Vector2>();
	public List<int> newTriangles = new List<int>();

	// private 
	float rad = 2048f; // radius of entire maze 
	float wid = 3f; // path width 
	float minLen = 0.5f; // minimum line length (resolution of curves) 
	//			perhaps an angular resolution would make a lot more sense and make calculations more predictable 
	//Vector3 currPos = new Vector3(385f, 1.5f, 447f);
	//Quaternion currRot;

	Transform tr;
	MeshFilter mf;
	MeshRenderer mr;
	Mesh mesh = new Mesh();



	void Start() {
		tr = transform;
		mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();

		mf.mesh = mesh;
		mesh.vertices = newVertices.ToArray();
		mesh.uv = newUV.ToArray();
		mesh.triangles = newTriangles.ToArray();

		makeElbow(-45f, new Vector3(-2f, 0f, 2f));
	}


	void makeElbow(float degreeDelta, Vector3 posDelta) { // the difference in degrees and position at the end 
		
	}
}