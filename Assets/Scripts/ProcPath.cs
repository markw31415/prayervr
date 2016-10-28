using UnityEngine;
using System.Collections.Generic;

/* 			NOTES: 
 *  
 * elbow = a shape like an elbow macaroni.  or a corner curve track piece that would end up rotating slot cars -/+ 90 degrees
 * 
 * LLR == triangles that are defined by points in this order: left, left, right
 * RLR == triangles that are defined by points in this order: right, left, right
 * (from the separate "lefts"/"rights" vertex lists) 
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
 * 2nd, we could plot a ray from the points exactly between the inside points, and the tip of the LLR triangles 
 * (which also points straight out) 
*/

public class ProcPath : MonoBehaviour {
	public Material mat;

	// private 
	int currVert = 0; // current vertex index 
	int lId = 0; // left index 
	int rId = 0; // right index 
	static float rad = 100f; // radius of entire labyrinth path 
	static float inset = 10f; // distance from the 0 point of each axis/dimension  
	static float goldenRatio = 1.61803398875f;
	static float smallerPercentage = 1f / goldenRatio;
	static float biggerPercentage = 1f - smallerPercentage;
	// widths 
	static float layerWid = rad * 0.75f / 7; // repeatable ring layers of the labyrinth, imagining it as an onion. 
	// starting from the outside edge of entire labyrinth, each layer has these zones:
	// 		(1) grass dotted with trees/bamboo
	//		(2) outer hedge
	//		(3) path
	//		(4) inner hedge
	float hedgeWid = smallerPercentage * layerWid / 2;
	float pathWid = biggerPercentage * layerWid / 2;
	// for now, the path & the dotted grass rings will share the same width

	//Vector3 currPos = new Vector3(385f, 1.5f, 447f);
	Transform tr;
	Transform rtt; // run-time transform 
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

	enum Quadrant {
		NorthEast,
		NorthWest,
		SouthEast,
		SouthWest
	}

	void Start() {
		//RenderSettings.ambientIntensity = 3f;
		tr = transform;
		rtt = transform;
		//rtt = new Transform();
		mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();
		mr.material = mat;

		m = new Mesh();
		mf.mesh = m;

		setupPathGeometry();

		// build final mesh
		m.vertices = verts.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = triIndices.ToArray();
		m.RecalculateNormals();
	}


	Vector3 latestDelta;
	void setupPathGeometry() {
		var currPos = new Vector3(0, 0, 12f);
		var currAng = 0f;
		var wid = 4f;

		// 1st elbow 
		// (any iterations above 1 are for a debug visual, to make sure all angles look good.  
		// it lays out a column of elbows, each next 1 is rotated a bit more) 
		for (int i = 0; i < 1/*9*/; i++) {
			var endDelta = Quaternion.Euler(0, currAng, 0) * new Vector3(-6f, 0, 6f);
			var pivotAnch = Quaternion.Euler(0, currAng, 0) * new Vector3(-6f, 0, 0f);
			makeArcedPathSection(5, wid, currAng, currAng-90f,
				currPos, 
				endDelta, 
				currPos + pivotAnch);

			currPos.z += 17f;
			currAng -= 45f;
		}

		currPos = new Vector3(inset, 0, -rad);
		var endD = new Vector3(-inset, 0, 7f); // delta from start to end point 
		makeArcedPathSection(7, wid, -90f, 0f, 
			currPos, 
			endD, 
			currPos + new Vector3(0, 0, 6));

		// the 2nd straight piece 
		currPos += endD;
		makeArcedPathSection(2, wid, 0f, 0f, 
			currPos, 
			new Vector3(0, 0, layerWid*4), 
			currPos + new Vector3(Mathf.Infinity, 0, 0));

		currPos = new Vector3(inset, 0, -rad+5*layerWid);
		endD = new Vector3(-inset, 0, -layerWid/2);
		makeArcedPathSection(7, wid, -90f, -180f, 
			currPos, 
			endD, 
			currPos + new Vector3(layerWid/2, 0, -layerWid/2));
		endD += currPos;

		// last elbow going into center of labyrinth 
		currPos = new Vector3(inset, 0, -rad+6*layerWid);
		makeArcedPathSection(7, wid, -90f, 0f, 
			currPos, 
			new Vector3(-inset, 0, 7f), 
			currPos + new Vector3(0, 0, 6));


		makeConcentricElbowsFor(Quadrant.SouthEast, new Vector3(inset, 0, -rad)); 
		makeConcentricElbowsFor(Quadrant.NorthEast, new Vector3(rad, 0, inset));
		makeConcentricElbowsFor(Quadrant.NorthWest, new Vector3(-inset, 0, rad));
		makeConcentricElbowsFor(Quadrant.SouthWest, new Vector3(-rad, 0, -inset));
	}


	void makeConcentricElbowsFor(Quadrant q, Vector3 pathStart) {
		int num = 8; // number of vertices per edge 
		var currRadDist = rad; // current radial distance (from center of labyrinth) 

		switch (q) {
			case Quadrant.SouthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(currRadDist-inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathStart, latestDelta, 90f, 0f, 4f, Vector3.zero);
					pathStart.z += layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathStart, latestDelta, 0f, -90f, 4f, Vector3.zero);
					pathStart.x -= layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthWest:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, -currRadDist+inset);
					makeArcedPathSection(num, pathStart, latestDelta, -90f, -180f, 4f, Vector3.zero);
					pathStart.z -= layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.SouthWest:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(currRadDist-inset, 0, -currRadDist+inset);
					makeArcedPathSection(num, pathStart, latestDelta, -180f, -270f, 4f, Vector3.zero);
					pathStart.x += layerWid;
					currRadDist -= layerWid;
				}
				break;
		}
	}


	void makeArcedPathSection(
		int numVerts,
		float width,  // ang = euler angle in Y 
		float startAng, 
		float endAng, 
		Vector3 startPos, 
		Vector3 endDelta, 
		Vector3 pivotAnchor
	) {
		int numDoublings = 1;

		//var pivotAnchor = Quaternion.Euler(0, startAng, 0) * (Vector3.left*lateralDist);//(endPos-startPos)/2 + rtt.right*lateralDist;
		//		makePoint(pivotAnchor, Color.red);		// show center point 

		var startL = Quaternion.Euler(0, startAng, 0) * (Vector3.left * width/2);
		var startR = Quaternion.Euler(0, startAng, 0) * (Vector3.right * width/2);
		startL += startPos;
		startR += startPos;
		Debug.Log("startL: " + startL);
		Debug.Log("startR: " + startR);
		var endL = Quaternion.Euler(0, endAng, 0) * (Vector3.left * width/2);
		var endR = Quaternion.Euler(0, endAng, 0) * (Vector3.right * width/2);
		endL += (startPos + endDelta);
		endR += (startPos + endDelta);
		Debug.Log("endL: " + endL);
		Debug.Log("endR: " + endR);

		makeArc(lefts, startL, endL, pivotAnchor, numVerts);
		//numVerts = getNumVertsForThisManyEdgeDoublings(numDoublings, numVerts);
		makeArc(rights,	startR, endR, pivotAnchor, numVerts);

		stitchLeftEdgesToDoubledRightEdges(numDoublings);
	}


	void makeArc(List<Vector3> l, Vector3 startPos, Vector3 endPos, Vector3 pivotAnchor, int numPoints) {
		float curr = 0f; // current point in 0f - 1f spectrum 
		float inc = 1f / (numPoints-1); // increment interval through spectrum

		var beg = startPos - pivotAnchor; // beginning direction  
		var end = endPos - pivotAnchor; // end direction 

		for (int i = 0; i < numPoints; i++) {
			makePoint(pivotAnchor + Vector3.Slerp(beg, end, curr), Color.black, l);
			curr += inc;
		}
	}


	void makePoint(Vector3 v, Color c, List<Vector3> l) {
		/*var f = 0.2f;
		var o = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		o.transform.position = v;
		o.transform.localScale = new Vector3(f, f, f);
		o.transform.SetParent(rtt, false);
		o.GetComponent<Renderer>().material.color = c;*/
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


	void stitchLeftEdgesToDoubledRightEdges(int numDoublings) { // ....and fill intermediate lists 
		lId = 0; // left index 
		rId = 0; // right index 
		// make 1st rung (2 points, which are outside of subsequent repeatable patterns) 
		addVertAndUv(rights[rId++]);
		addVertAndUv(lefts[lId++]);


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

		for (; lId < lefts.Count; lId++) {
			// do one chunk 
			// (between all the points of the lower resolution edge) 
			Debug.Log("\nlId: " + lId);

			if // it's a simple quad ladder 
				(lefts.Count == rights.Count) 
			{
				makeOneRLRTriangle();
				makeLLRTriangle();
			}else{ // it has more verts on the right, requiring 3 or more tris per chunk 
				//while (rId <= rights.Count/2) {
					makeRLRTriangleFan(2);
					makeAnAFTERFANLLRTriangle(); // exact middle tri (of a rung to rung chunk) pointing outwards 
				//}
			}
		}
	}


	void makeOneRLRTriangle() {
		addVertAndUv(rights[rId++]);

		triIndices.Add(currVert - 3);
		triIndices.Add(currVert - 2);
		triIndices.Add(currVert - 1);
	}


	int anchL; // left anchor.  the point of a triangle fan, where you would hold it (if it was a physical hand fan) 
	void makeRLRTriangleFan(int numTris) {
		// notes:
		// the left tri remains the same
		// always use the latest 2 right tris

		// diff indices from makeOneRLRTriangle, because we're before the rId++ here 
		int prevR = currVert - 2;
		anchL = currVert - 1;
		int currR = currVert;

		for (int i = 0; i < numTris; i++) {
			addVertAndUv(rights[rId++]);

			if (i == 0) {
				prevR = currVert - 3;
			}else{
				prevR = currR;
			}

			currR = currVert - 1;

			triIndices.Add(prevR);
			triIndices.Add(anchL);
			triIndices.Add(currR);
		}
	}


	void makeAnAFTERFANLLRTriangle() {
		addVertAndUv(lefts[lId]);

		triIndices.Add(anchL);
		triIndices.Add(currVert - 1);
		triIndices.Add(currVert - 2);
	}


	void makeLLRTriangle() {
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