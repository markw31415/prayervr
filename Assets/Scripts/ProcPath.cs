using UnityEngine;
using System.Collections.Generic;



public class ProcPath : MonoBehaviour {
	public List<Vector3> verts = new List<Vector3>();
	public List<Vector2> uvs = new List<Vector2>();
	public List<int> tris = new List<int>();

	// private 
	float rad = 2048f; // radius of entire maze 
	float wid = 3f; // path width 
	//Vector3 currPos = new Vector3(385f, 1.5f, 447f);
	Transform tr;
	Mesh m;
	MeshFilter mf;
	MeshRenderer mr;



	void Start() {
		tr = transform;
		mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();

		m = new Mesh();
		mf.mesh = m;

		m.vertices = verts.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = tris.ToArray();

		makeArcTo(new Vector3(11f, 0f, -4f), 10, -1.5f);
	}


	// lateralDist to the center of a circle.  which is the distance away (laterally, considering
	// the "movement" direction) from the center point (which lies between our current pos and the endpoint target pos). 
	// in other words, it is a single float value that gives our distance to the left (neg) or right (pos), where we place
	// the center of the circle (a segment of the perimeter of this circle forms the arc) 
	void makeArcTo(Vector3 posDelta, int numPoints, float lateralDist) {
		float curr = 0f; // current point in 0f - 1f spectrum 
		float inc = 1f / numPoints; // increment interval through spectrum

		// show center point 
		tr.forward = posDelta;
		var pivotAnchor = tr.position + posDelta/2 + tr.right*lateralDist;
		makePoint(pivotAnchor, Color.red);
		var beg = tr.position - pivotAnchor; // beginning direction  
		var end = (tr.position + posDelta) - pivotAnchor; // end direction 

		for (int i = 0; i <= numPoints; i++) {
			makePoint(pivotAnchor + Vector3.Slerp(beg, end, curr), Color.black);
			curr += inc;
		}
	}


	void makePoint(Vector3 v, Color c) {
		var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		o.transform.position = v;
		o.GetComponent<Renderer>().material.color = c;
	}
}