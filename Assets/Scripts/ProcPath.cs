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
 * & if we do (2), maybe the outside edges could have lower-edge/side resolution, and it could be easy to do it
 * at twice the resolution of the inside edge of the path?
 * 
 * 1st, we could use the chunk (a repeating piece/pattern of the path section) dividing lines because they
 * point straight outwards.
 * 2nd, we could plot a ray from the points exactly between the inside points, and the tip of the LLR triangles 
 * (which also points straight out) 
*/

public class ProcPath : MonoBehaviour {
	public Material Mat;

	// private 
	int numEdgeVerts = 5; // ....for each half of a hairpin 
	int lId = 0; // left index 
	int rId = 0; // right index 
	static float rad = 100f; // radius of entire labyrinth path 
	static float inset = 10f; // distance from the 0 point of each axis/dimension  
	static float goldenRatio = 1.61803398875f;
	static float smallerFrac = 1f / (goldenRatio + 1f); // fraction 
	static float biggerFrac = 1f - smallerFrac; //         ^ 
	// widths 
	static float layerWid = rad * 0.75f / 7; // repeatable ring layers of the labyrinth, imagining it as an onion. 
	// starting from the outside edge of entire labyrinth, each layer has these zones:
	// 		(1) grass dotted with trees/bamboo
	//		(2) outer hedge
	//		(3) path
	//		(4) inner hedge
	float hedgeWid = smallerFrac * layerWid / 2;
	float pathWid = biggerFrac * layerWid / 2; // for now, the path & the tree/prop-dotted grass spaces between the concentric elbows, will share the same width 
	List <LabyrinthQuadrant> pathQs = new List<LabyrinthQuadrant>() { // path quadrants (huge, concentric-elbows) 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant() 
	};

	Mesh m;
	MeshFilter mf;
	MeshRenderer mr;

    // for building seperate L/R edges individually for sections of the path (usually elbow arcs). 
    List<Vector3> lefts = new List<Vector3>();
	List<Vector3> rights = new List<Vector3>();

	// final form to put into mesh object
	List<Vector3> verts = new List<Vector3>();
	List<Vector2> uvs = new List<Vector2>();
	List<int> triIndices = new List<int>();

	enum Quadrant { // don't change this order, as they are also used as indices 
		SouthEast,
		NorthEast,
		NorthWest,
		SouthWest
	}

	void Start() {
		mf = gameObject.AddComponent<MeshFilter>();
		mr = gameObject.AddComponent<MeshRenderer>();
		mr.material = Mat;

		m = new Mesh();
		mf.mesh = m;

        makeVertexEdgesForEntirePath();
        makeTrisForEntirePath();

        // build final mesh
        m.vertices = verts.ToArray();
		m.uv = uvs.ToArray();
		m.triangles = triIndices.ToArray();
		setAllNormalsPointingUp();
	}


	void setAllNormalsPointingUp() {
		// usually you would just do "m.RecalculateNormals();".....
		// ....however it works VERY slowly for this path (which doesn't need any fancy math) 

		var norms = new Vector3[verts.Count];
		var up = Vector3.up; // this actually takes a very little time if not cached 

		for (int i = 0; i < norms.Length; i++)
			norms[i] = up;

		m.normals = norms;
	}


	void makeVertexEdgesForEntirePath() {
		// setup a palette of vertices to use later 
		// (we start at practically the southernmost point, in the center of the x axis, 
		// starting with outermost elbow, & moving counter-clockwise) 
		cacheConcentricElbowVertsFor(Quadrant.SouthEast, new Vector3(inset, 0, -rad)); 
		cacheConcentricElbowVertsFor(Quadrant.NorthEast, new Vector3(rad, 0, inset));
		cacheConcentricElbowVertsFor(Quadrant.NorthWest, new Vector3(-inset, 0, rad));
		cacheConcentricElbowVertsFor(Quadrant.SouthWest, new Vector3(-rad, 0, -inset));

        // make 1st 2 verts for the starting entrance archway of the labyrinth 
        addVertAndUvToFinalList(new Vector3(-pathWid / 2, 0, -rad));
        addVertAndUvToFinalList(new Vector3( pathWid / 2, 0, -rad));

        // 1st small elbow 
        var currPos = new Vector3(-inset/2, 0, -rad+layerWid*3);
		var endDelta = new Vector3(-inset/2, 0, layerWid);
		var pivotAnch = currPos + new Vector3(-layerWid, 0, 0);
		makeArcedPathSection(5, pathWid, 0f, -90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

        // 1st of the big concentric elbows 
        var pq = pathQs[(int)Quadrant.SouthWest]; // path quadrant 
		var ri = 4; // ring id/index 
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);

        // 2nd small elbow 
        makeNorthEdgeHairpinFromWestToEast(new Vector3(-rad+layerWid*ri, 0, -inset));

        // 2nd BIG elbow 
        ri = 5;
		addSection(pq.Lefts[ri], pq.Rights[ri]);

		// 3rd small elbow 
		//makeEastEdgeHairpinFromSouthToNorth(new Vector3(-inset, 0, -rad+layerWid*ri));

		// 3rd BIG elbow 
		ri = 6;
		pq.Rights[ri].Reverse();
		pq.Lefts[ri].Reverse();
		addSection(pq.Rights[ri], pq.Lefts[ri]);

		// 1st quadrant bridge 
		//makeBridgeSectionBetweenQuadrants(0f, new Vector3(-rad+layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		// 4th BIG elbow 
		pq = pathQs[(int)Quadrant.NorthWest];
		//ri = 6;
		pq.Rights[ri].Reverse();
		pq.Lefts[ri].Reverse();
		addSection(pq.Rights[ri], pq.Lefts[ri]);

		// 2nd quadrant bridge 
		//makeBridgeSectionBetweenQuadrants(90f, new Vector3(-inset, 0, rad-layerWid*ri), new Vector3(inset*2, 0, 0));

		// 5th BIG elbow 
		pq = pathQs[(int)Quadrant.NorthEast];
		//ri = 6;
		pq.Rights[ri].Reverse();
		pq.Lefts[ri].Reverse();
		addSection(pq.Rights[ri], pq.Lefts[ri]);

		// 6th small elbow 
		//ri = 6;
		currPos = new Vector3(rad-layerWid*ri, 0, inset);
		endDelta = new Vector3(layerWid/2, 0, -layerWid/2);
		pivotAnch = currPos;
		pivotAnch.x += layerWid/2;

		makeArcedPathSection(5, pathWid, 180f, 90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(layerWid/2, 0, layerWid/2);

		makeArcedPathSection(5, pathWid, 90f, 0f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		// 6th BIG elbow 
		ri = 5;
		addSection(pq.Lefts[ri], pq.Rights[ri]);

		// 3rd quadrant bridge 
		makeBridgeSectionBetweenQuadrants(-90f, new Vector3(inset, 0, rad-layerWid*ri), new Vector3(-inset*2, 0, 0));

		// innermost NW big elbow
		//ri = 5;
		pq = pathQs[(int)Quadrant.NorthWest];
		addSection(pq.Lefts[ri], pq.Rights[ri]);

		makeSouthEdgeHairpinFromEastToWest(new Vector3(-rad+layerWid*ri, 0, inset));

		// BIG elbow (3rd from center) 
		ri = 4;
		pq.Rights[ri].Reverse();
		pq.Lefts[ri].Reverse();
		addSection(pq.Rights[ri], pq.Lefts[ri]);

		makeEastEdgeHairpinFromSouthToNorth(new Vector3(-inset, 0, rad-layerWid*ri));

		// BIG elbow (4th from center)
		ri = 3;
		addSection(pq.Lefts[ri], pq.Rights[ri]);

		makeBridgeSectionBetweenQuadrants(-180f, new Vector3(-rad+layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		// BIG elbow (4th from center)
		pq = pathQs[(int)Quadrant.SouthWest];
		//ri = 3;
		addSection(pq.Lefts[ri], pq.Rights[ri]);

		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset-layerWid/2, 0, -rad+layerWid*ri));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeNorthEdgeHairpinFromEastToWest(new Vector3(-rad+layerWid*ri, 0, -inset));

		ri = 1;
		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset-layerWid/2, 0, -rad+layerWid));

		ri = 0;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);

		makeBridgeSectionBetweenQuadrants(0f, new Vector3(-rad+layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		pq = pathQs[(int)Quadrant.NorthWest];
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset, 0, rad));

		ri = 1;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeSouthEdgeHairpinFromWestToEast(new Vector3(-rad+layerWid*ri, 0, inset));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeBridgeSectionBetweenQuadrants(90f, new Vector3(-inset, 0, rad-layerWid*ri), new Vector3(inset*2, 0, 0));

		pq = pathQs[(int)Quadrant.NorthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeSouthEdgeHairpinFromEastToWest(new Vector3(rad-layerWid*ri, 0, inset));

		ri = 3;
		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, rad-layerWid*ri));

		ri = 4;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeBridgeSectionBetweenQuadrants(180f, new Vector3(rad-layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		pq = pathQs[(int)Quadrant.SouthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, -rad+layerWid*ri));

		ri = 3;
		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeNorthEdgeHairpinFromWestToEast(new Vector3(rad-layerWid*ri, 0, -inset));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, -rad+layerWid*ri));

		ri = 1;
		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeBridgeSectionBetweenQuadrants(0f, new Vector3(rad-layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		pq = pathQs[(int)Quadrant.NorthEast];
		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeWestEdgeHairpinFromSouthToNorth(new Vector3(inset, 0, rad-layerWid*ri));

		ri = 0;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);
		makeBridgeSectionBetweenQuadrants(180f, new Vector3(rad-layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		pq = pathQs[(int)Quadrant.SouthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);

		// simple elbow (single 90'ish degree arc, forming quarter of a torus), which precedes the 2nd of 3 straight sections 
		currPos = new Vector3(inset, 0, -rad);
		var endD = new Vector3(-inset, 0, 7f); // delta from start to end point 
		makeArcedPathSection(7, pathWid, -90f, 0f, 
			currPos, 
			endD, 
			currPos + new Vector3(0, 0, 6),
			lefts, rights);

		// next to last simple elbow 
		ri = 5;
		var z = -rad+layerWid*ri;
		currPos = new Vector3(0, 0, z-7f);
		endD = new Vector3(inset, 0, 7f);
		makeArcedPathSection(7, pathWid, 0f, 90f, 
			currPos, 
			endD, 
			currPos + new Vector3(inset, 0, 0),
			lefts, rights);
		endD += currPos;

		addSection(pq.Lefts[ri], pq.Rights[ri]);
		makeNorthEdgeHairpinFromEastToWest(new Vector3(rad-layerWid*ri, 0, -inset));

		ri = 6;
		makeBigElbowWithEdgesSwappedAndBackwards(pq, ri);

		// last simple elbow going into center of labyrinth 
		currPos = new Vector3(inset, 0, -rad+6*layerWid);
		makeArcedPathSection(7, pathWid, -90f, 0f, 
			currPos, 
			new Vector3(-inset, 0, 7f), 
			currPos + new Vector3(0, 0, 6),
			lefts, rights);
	}


	void makeBigElbowWithEdgesSwappedAndBackwards(LabyrinthQuadrant lq, int ringId) {
		lq.Rights[ringId].Reverse();
		lq.Lefts[ringId].Reverse();
		addSection(lq.Rights[ringId], lq.Lefts[ringId]);
	}


	void makeBridgeSectionBetweenQuadrants(float angle, Vector3 currPos, Vector3 endDelta) {
		makeArcedPathSection(4, pathWid, angle, angle,
			currPos, 
			endDelta, 
			Vector3.zero,
			lefts, rights);
	}


	void makeWestEdgeHairpinFromSouthToNorth(Vector3 currPos) {
		var endDelta = new Vector3(-layerWid/2, 0, layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.z += layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, -90f, 0f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(layerWid/2, 0, layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 0f, 90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeWestEdgeHairpinFromNorthToSouth(Vector3 currPos) {
		var endDelta = new Vector3(-layerWid/2, 0, -layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.z -= layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, -90f, 180f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(layerWid/2, 0, -layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 180f, 90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeEastEdgeHairpinFromNorthToSouth(Vector3 currPos) {
		var endDelta = new Vector3(layerWid/2, 0, -layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.z -= layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, 90f, 180f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(-layerWid/2, 0, -layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 180f, -90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeEastEdgeHairpinFromSouthToNorth(Vector3 currPos) {
		var endDelta = new Vector3(layerWid/2, 0, layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.z += layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, 90f, 0f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(-layerWid/2, 0, layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 0f, -90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeSouthEdgeHairpinFromWestToEast(Vector3 currPos) {
		var endDelta = new Vector3(layerWid/2, 0, -layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.x += layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, 180f, 90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(layerWid/2, 0, layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 90f, 0f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeSouthEdgeHairpinFromEastToWest(Vector3 currPos) {
		var endDelta = new Vector3(-layerWid/2, 0, -layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.x -= layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, -180f, -90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(-layerWid/2, 0, layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, -90f, 0f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeNorthEdgeHairpinFromEastToWest(Vector3 currPos) {
		var endDelta = new Vector3(-layerWid/2, 0, layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.x -= layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, 0f, -90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(-layerWid/2, 0, -layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, -90f, 180f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	void makeNorthEdgeHairpinFromWestToEast(Vector3 currPos) {
		var endDelta = new Vector3(layerWid/2, 0, layerWid/2);
		var pivotAnch = currPos;
		pivotAnch.x += layerWid/2;

		makeArcedPathSection(numEdgeVerts, pathWid, 0f, 90f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);

		currPos += endDelta;
		endDelta = new Vector3(layerWid/2, 0, -layerWid/2);

		makeArcedPathSection(numEdgeVerts, pathWid, 90f, 180f,
			currPos, 
			endDelta, 
			pivotAnch,
			lefts, rights);
	}


	Vector3 latestDelta;
	void cacheConcentricElbowVertsFor(Quadrant q, Vector3 pathStart) {
		int num = 8; // number of vertices per edge 
		var currRadDist = rad; // current radial distance (from center of labyrinth) 
		var pq = pathQs[(int)q]; // path quadrant 

		switch (q) {
			case Quadrant.SouthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(currRadDist-inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathWid, 90f, 0f, pathStart, latestDelta, Vector3.zero, pq.Lefts[i], pq.Rights[i], false);
					pathStart.z += layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathWid, 0f, -90f, pathStart, latestDelta, Vector3.zero, pq.Lefts[i], pq.Rights[i], false);
					pathStart.x -= layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthWest:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, -currRadDist+inset);
					makeArcedPathSection(num, pathWid, -90f, -180f, pathStart, latestDelta, Vector3.zero, pq.Lefts[i], pq.Rights[i], false);
					pathStart.z -= layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.SouthWest:
				for (int i = 0; i < 7; i++) {
					if (i < 4)
						latestDelta = new Vector3(currRadDist-inset-layerWid/2, 0, -currRadDist+inset);
					else
						latestDelta = new Vector3(currRadDist-inset, /********/ 0, -currRadDist+inset);

					makeArcedPathSection(num, pathWid, -180f, -270f, pathStart, latestDelta, Vector3.zero, pq.Lefts[i], pq.Rights[i], false);
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
		Vector3 pivotAnchor,
		List<Vector3> lefts,
		List<Vector3> rights,
		bool addPointsToFinalList = true
	) {
		//Debug.Log("\n makeArcedPathSection()");
		var startL = Quaternion.Euler(0, startAng, 0) * (Vector3.left * width/2);
		var startR = Quaternion.Euler(0, startAng, 0) * (Vector3.right * width/2);
		startL += startPos;
		startR += startPos;
		//Debug.Log("startL: " + startL);
		//Debug.Log("startR: " + startR);
		var endL = Quaternion.Euler(0, endAng, 0) * (Vector3.left * width/2);
		var endR = Quaternion.Euler(0, endAng, 0) * (Vector3.right * width/2);
		endL += (startPos + endDelta);
		endR += (startPos + endDelta);
        //Debug.Log("endL: " + endL);
        //Debug.Log("endR: " + endR);

        makeArc(lefts, startL, endL, pivotAnchor, numVerts);
		makeArc(rights,	startR, endR, pivotAnchor, numVerts);

		if (addPointsToFinalList)
			addSection(lefts, rights);
	}


	void makeArc(List<Vector3> l, Vector3 startPos, Vector3 endPos, Vector3 pivotAnchor, int numPoints) {
		float curr = 0f; // current point in 0f - 1f spectrum 
		float inc = 1f / (numPoints-1); // increment interval through spectrum

		var beg = startPos - pivotAnchor; // beginning direction  
		var end = endPos - pivotAnchor; // end direction 

		for (int i = 0; i < numPoints; i++) {
			makePoint(pivotAnchor + Vector3.Slerp(beg, end, curr), Color.red, l);
			curr += inc;
		}
	}


	void makePoint(Vector3 v, Color c, List<Vector3> l) {
		var f = 0.2f;
		var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
		o.transform.position = v;
		o.transform.localScale = new Vector3(f, f, f);
		o.transform.SetParent(transform, false);
		o.GetComponent<Renderer>().material.color = c;
		l.Add(v);
	}


    void addSection(List<Vector3> lefts, List<Vector3> rights) {
        // iterate L/R pairs 
        for (int i = 0; i < lefts.Count; i++) {
            addVertAndUvToFinalList(lefts[i]);
            addVertAndUvToFinalList(rights[i]);
        }

        lefts.Clear();
        rights.Clear();
    }


    void makeTrisForEntirePath() {
        // triangulate a quad ladder, where every chunk of 4 vertices makes 2 tris (3 + 3 indices) 
        for (int i = 0; i < verts.Count-3; i+=2) {
            triIndices.Add(i + 0);
            triIndices.Add(i + 2);
            triIndices.Add(i + 3);

            triIndices.Add(i + 0);
            triIndices.Add(i + 3);
            triIndices.Add(i + 1);
        }
    }


    void addVertAndUvToFinalList(Vector3 v) {
		verts.Add(v);
		uvs.Add(new Vector2(v.x/4, v.z/4));
	}
}