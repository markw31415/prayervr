using UnityEngine;
using System.Collections.Generic;

/* 			NOTES: 
 *  
 * LLR == triangles that are defined by these points: left, next left, right
 * RLR == triangles that are defined by these points: right, left, right
 * (from the "lefts"/"rights" vertex lists)
 * 
 * thoughts on either:
 * (1) automating existing hedge model PLACEMENT (and rotation) around the edges of the fully generated path
 * (2) fully generating hedge meshes to follow the form of the path perfectly, with no cracks or overlapping
 *
 * (surely there is a way to do the latter, where it's
 * easier than automating proper placing of existing "square" (right angled) rectangular/box models at appropriate
 * intervals?)
 * 
 * & if we do (2), maybe the outside edges could have lower edge/side resolution, and it could be easy to do it
 * at twice the resolution of the inside edge of the path?
 * 
 * 1st, we could use the chunk (a repeating piece/pattern of the path section) dividing lines because they
 * point straight outwards.
 * 2nd, we could plot a ray from the points exactly between the inside points, and the tip of the LLR triangles,
 * which also points straight out
*/

public class ProcPath : MonoBehaviour {
	public Material mat;

	// private 
	int currVert = 0; // current vertex index 
	int lId = 0; // left index 
	int rId = 0; // right index 
	float rad = 2048f; // radius of entire labyrinth path 
	float wid = 3f; // path width 
	float goldenRatio = 1.61803398875f;

	//Vector3 currPos = new Vector3(385f, 1.5f, 447f);
	Transform tr;
	Mesh m;
	MeshFilter mf;
	MeshRenderer mr;

	// for building the edge lines  
	List<Vector3> lefts = new List<Vector3>();
	List<Vector3> rights = new List<Vector3>();

	// final form to put into mesh object
	List<Vector3> verts = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<int> triIndices = new List<int>();



	void Start() {
		//RenderSettings.ambientIntensity = 3f;
		tr = transform;
		mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();
		mr.material = mat;

		m = new Mesh();
		mf.mesh = m;

		setupIntermediateStructures();

		// build final mesh
		m.vertices = verts.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = triIndices.ToArray();
		m.RecalculateNormals();
	}


	void setupIntermediateStructures() {
		var numVerts = 5;
		makeArc(lefts,
			new Vector3(-2f, 0f, 0f), 
			new Vector3(-4f, 0f, 4f), 
			-2.7f, numVerts);

		numVerts = getNumVertsForThisManyEdgeDoublings(1, numVerts);
		makeArc(rights,
			new Vector3(2f, 0f, 0f), 
			new Vector3(-8f, 0f, 8f), 
			-4f, numVerts);

		stitchLeftEdgesToDoubledRightEdges();
	}


	// lateralDist: ....to the center of a circle.  which is the LATERAL distance away (considering
	// the "movement" direction) from the center point (which lies between our current pos and the endpoint target pos). 
	// in other words, it is a single float value that gives our distance to the left (neg) or right (pos), where we place
	// the center of the circle (a select segment of the perimeter of the circle forms the arc) 
	void makeArc(List<Vector3> l, Vector3 startPos, Vector3 endDelta, float lateralDist, int numPoints) {
		float curr = 0f; // current point in 0f - 1f spectrum 
		float inc = 1f / (numPoints-1); // increment interval through spectrum

		tr.forward = endDelta;
		var pivotAnchor = startPos + endDelta/2 + tr.right*lateralDist;
		//		makePoint(pivotAnchor, Color.red);		// show center point 
		var beg = startPos - pivotAnchor; // beginning direction  
		var end = (startPos + endDelta) - pivotAnchor; // end direction 

		for (int i = 0; i < numPoints; i++) {
			makePoint(pivotAnchor + Vector3.Slerp(beg, end, curr), Color.black, l);
			curr += inc;
		}
	}


	void makePoint(Vector3 v, Color c, List<Vector3> l) {
		var f = 0.2f;
		var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		o.transform.position = tr.position + v;
		o.transform.localScale = new Vector3(f, f, f);
		o.transform.SetParent(tr, false);
		o.GetComponent<Renderer>().material.color = c;
		l.Add(v);
	}


	// an "edge" here is a straight segment (or side, if we were talking about closed polygons) between 2 vertices. 
	// to make things simple, every edge resolution increase will double the number of edges 
	int getNumVertsForThisManyEdgeDoublings(int numDoublings, int numVerts) {
		for (int i = 0; i < numDoublings; i++) {
			numVerts += (numVerts - 1);
		}

		return numVerts;
	}


	void stitchLeftEdgesToDoubledRightEdges() { // ....and fill intermediate lists 
		// starting triangle (to be more generic) should go 1st right to 1st left, then 2nd right 
		lId = 0; // left index 
		rId = 0; // right index 

		// LRL/LLR: refers to the sequence of lefts and rights
		//
		// a "ladder" here, refers to the edges, and the "straight out" lines
		// (from the center of the circle we're taking a segment from) are rungs.  which are formed
		// by matching points.  if both sides have the same number of points, then we have a simple
		// quad ladder, with each (rung to rung) chunk being 2 triangles.
		// with 1 edge doubled (in number of straight lines), each chunk will be 3 triangles.  

		// the middle triangle
		// (a LRL) will ALWAYS have the same shape, no matter how many extra resolution doublings 
		// occur.  the 2 remaining triangular areas can be made into fans, where the extra doublings
		// make the round edge (opposite of the sharp point) more and more round as more points are added.

		// "rungs" here, refers to the "straight out" (radially from the centerpoint arc is plotted from)
		// lines, one for each inside edge point 
		// a rung-to-rung run is a repeatable chunk pattern of the section we are building


		// TODO when expanding functionality to allow for more than 1 doubling:
		// each RLRTriangle, could be expanded to be a fan with multiple tris.  if there is more than 1 doubling. 


		// 1st rung (2 points, which are outside of subsequent repeatable patterns) 
		addVertAndUv(rights[rId++]);
		addVertAndUv(lefts[lId]);

		if // it's a simple quad ladder 
			(lefts.Count == rights.Count) 
		{
			//repeatForEntireSection {
				//makeRLRTriangles();
				//makeLLRTriangle();
			//}
		}else{
			for (; lId < lefts.Count ;) {
				lId++;

				// do one chunk between all the points of the lower resolution edge 
				bool newPointNeeded = true; //verts.Count < 5; // FIXME needed? for different resolution pairings?

				//repeatUntilNextLLRTri {
					makeRLRTriangles(newPointNeeded);
				//}

				makeLLRTriangle(newPointNeeded); // exact middle tri (of a rung to rung chunk) 

				//repeatUntilUntilNextRung { 
					makeRLRTriangles(newPointNeeded);
				//}
			}
		}

		/*
		// RLR tri 
		addVertAndUv(rights[rId++]);
		triIndices.Add(2);
		triIndices.Add(3);
		triIndices.Add(4);
		*/
	}


	void makeRLRTriangles(bool newPointNeeded) {
		if (newPointNeeded)
			addVertAndUv(rights[rId++]);
		
		triIndices.Add(currVert - 3);
		triIndices.Add(currVert - 2);
		triIndices.Add(currVert - 1);
	}


	void makeLLRTriangle(bool newPointNeeded) {
		if (newPointNeeded)
			addVertAndUv(lefts[lId]);
		
		triIndices.Add(currVert - 3);
		triIndices.Add(currVert - 1);
		triIndices.Add(currVert - 2);
	}


	void addVertAndUv(Vector3 v) {
		verts.Add(v);
		uvs.Add(new Vector2(v.x/4, v.z/4));
		currVert++;
	}
}