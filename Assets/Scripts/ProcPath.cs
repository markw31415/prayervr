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
	int numSub = 1; // number of subdivisions for the higher resolution side/edge of path 
	int numEdgeVerts = 5; // ....for each half of a hairpin 
	int currVert = 0; // current vertex index 
	int lId = 0; // left index 
	int rId = 0; // right index 
	int anchL; // left anchor.  the point of a triangle fan, where you would hold it (if it was a physical hand fan) 
	static float rad = 100f; // radius of entire labyrinth path 
	static float inset = 10f; // distance from the 0 point of each axis/dimension  
	static float goldenRatio = 1.61803398875f;
	static float smallerPercentage = 1f / (goldenRatio + 1f);
	static float biggerPercentage = 1f - smallerPercentage;
	// widths 
	static float layerWid = rad * 0.75f / 7; // repeatable ring layers of the labyrinth, imagining it as an onion. 
	// starting from the outside edge of entire labyrinth, each layer has these zones:
	// 		(1) grass dotted with trees/bamboo
	//		(2) outer hedge
	//		(3) path
	//		(4) inner hedge
	float hedgeWid = smallerPercentage * layerWid / 2;
	float pathWid = biggerPercentage * layerWid / 2; // for now, the path & the dotted grass rings will share the same width
	List <LabyrinthQuadrant> labQuads = new List<LabyrinthQuadrant>() { 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant(), 
		new LabyrinthQuadrant() 
	};

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

		setupPathGeometry();

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


	void setupPathGeometry() {
		// setup a palette of vertices to use later 
		// (we start at practically the southernmost point, in the center of the x axis, 
		// starting with outermost elbow, & moving counter-clockwise) 
		cacheConcentricElbowVertsFor(Quadrant.SouthEast, new Vector3(inset, 0, -rad)); 
		cacheConcentricElbowVertsFor(Quadrant.NorthEast, new Vector3(rad, 0, inset));
		cacheConcentricElbowVertsFor(Quadrant.NorthWest, new Vector3(-inset, 0, rad));
		cacheConcentricElbowVertsFor(Quadrant.SouthWest, new Vector3(-rad, 0, -inset));

		// make 1st 2 verts for the starting entrance archway of the labyrinth 
		addVertAndUvToGlobalPool(new Vector3(pathWid/2, 0, -rad));
		addVertAndUvToGlobalPool(new Vector3(-pathWid/2, 0, -rad));

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
		var lq = labQuads[(int)Quadrant.SouthWest]; // labyrinth quad 
		var ri = 4; // ring id/index 
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);

		// 2nd small elbow 
		makeNorthEdgeHairpinFromWestToEast(new Vector3(-rad+layerWid*ri, 0, -inset));

		// 2nd BIG elbow 
		ri = 5;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);

		// 3rd small elbow 
		//makeEastEdgeHairpinFromSouthToNorth(new Vector3(-inset, 0, -rad+layerWid*ri));

		// 3rd BIG elbow 
		ri = 6;
		lq.Rights[ri].Reverse();
		lq.Lefts[ri].Reverse();
		stitchLeftEdgesToRightEdges(numSub, lq.Rights[ri], lq.Lefts[ri]);

		// 1st quadrant bridge 
		//makeBridgeSectionBetweenQuadrants(0f, new Vector3(-rad+layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		// 4th BIG elbow 
		lq = labQuads[(int)Quadrant.NorthWest];
		//ri = 6;
		lq.Rights[ri].Reverse();
		lq.Lefts[ri].Reverse();
		stitchLeftEdgesToRightEdges(numSub, lq.Rights[ri], lq.Lefts[ri]);

		// 2nd quadrant bridge 
		//makeBridgeSectionBetweenQuadrants(90f, new Vector3(-inset, 0, rad-layerWid*ri), new Vector3(inset*2, 0, 0));

		// 5th BIG elbow 
		lq = labQuads[(int)Quadrant.NorthEast];
		//ri = 6;
		lq.Rights[ri].Reverse();
		lq.Lefts[ri].Reverse();
		stitchLeftEdgesToRightEdges(numSub, lq.Rights[ri], lq.Lefts[ri]);

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
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);

		// 3rd quadrant bridge 
		makeBridgeSectionBetweenQuadrants(-90f, new Vector3(inset, 0, rad-layerWid*ri), new Vector3(-inset*2, 0, 0));

		// innermost NW big elbow
		//ri = 5;
		lq = labQuads[(int)Quadrant.NorthWest];
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);

		makeSouthEdgeHairpinFromEastToWest(new Vector3(-rad+layerWid*ri, 0, inset));

		// BIG elbow (3rd from center) 
		ri = 4;
		lq.Rights[ri].Reverse();
		lq.Lefts[ri].Reverse();
		stitchLeftEdgesToRightEdges(numSub, lq.Rights[ri], lq.Lefts[ri]);

		makeEastEdgeHairpinFromSouthToNorth(new Vector3(-inset, 0, rad-layerWid*ri));

		// BIG elbow (4th from center)
		ri = 3;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);

		makeBridgeSectionBetweenQuadrants(-180f, new Vector3(-rad+layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		// BIG elbow (4th from center)
		lq = labQuads[(int)Quadrant.SouthWest];
		//ri = 3;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);

		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset-layerWid/2, 0, -rad+layerWid*ri));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeNorthEdgeHairpinFromEastToWest(new Vector3(-rad+layerWid*ri, 0, -inset));

		ri = 1;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset-layerWid/2, 0, -rad+layerWid));

		ri = 0;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);

		makeBridgeSectionBetweenQuadrants(0f, new Vector3(-rad+layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		lq = labQuads[(int)Quadrant.NorthWest];
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeEastEdgeHairpinFromNorthToSouth(new Vector3(-inset, 0, rad));

		ri = 1;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeSouthEdgeHairpinFromWestToEast(new Vector3(-rad+layerWid*ri, 0, inset));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeBridgeSectionBetweenQuadrants(90f, new Vector3(-inset, 0, rad-layerWid*ri), new Vector3(inset*2, 0, 0));

		lq = labQuads[(int)Quadrant.NorthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeSouthEdgeHairpinFromEastToWest(new Vector3(rad-layerWid*ri, 0, inset));

		ri = 3;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, rad-layerWid*ri));

		ri = 4;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeBridgeSectionBetweenQuadrants(180f, new Vector3(rad-layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		lq = labQuads[(int)Quadrant.SouthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, -rad+layerWid*ri));

		ri = 3;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeNorthEdgeHairpinFromWestToEast(new Vector3(rad-layerWid*ri, 0, -inset));

		ri = 2;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeWestEdgeHairpinFromNorthToSouth(new Vector3(inset, 0, -rad+layerWid*ri));

		ri = 1;
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeBridgeSectionBetweenQuadrants(0f, new Vector3(rad-layerWid*ri, 0, -inset), new Vector3(0, 0, inset*2));

		lq = labQuads[(int)Quadrant.NorthEast];
		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeWestEdgeHairpinFromSouthToNorth(new Vector3(inset, 0, rad-layerWid*ri));

		ri = 0;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);
		makeBridgeSectionBetweenQuadrants(180f, new Vector3(rad-layerWid*ri, 0, inset), new Vector3(0, 0, -inset*2));

		lq = labQuads[(int)Quadrant.SouthEast];
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);

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

		stitchLeftEdgesToRightEdges(numSub, lq.Lefts[ri], lq.Rights[ri]);
		makeNorthEdgeHairpinFromEastToWest(new Vector3(rad-layerWid*ri, 0, -inset));

		ri = 6;
		makeBigElbowWithEdgesSwappedAndBackwards(lq, ri);

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
		stitchLeftEdgesToRightEdges(numSub, lq.Rights[ringId], lq.Lefts[ringId]);
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
		var lq = labQuads[(int)q]; // labyrinth quadrants 

		switch (q) {
			case Quadrant.SouthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(currRadDist-inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathWid, 90f, 0f, pathStart, latestDelta, Vector3.zero, lq.Lefts[i], lq.Rights[i], false);
					pathStart.z += layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthEast:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, currRadDist-inset);
					makeArcedPathSection(num, pathWid, 0f, -90f, pathStart, latestDelta, Vector3.zero, lq.Lefts[i], lq.Rights[i], false);
					pathStart.x -= layerWid;
					currRadDist -= layerWid;
				}
				break;
			case Quadrant.NorthWest:
				for (int i = 0; i < 7; i++) {
					latestDelta = new Vector3(-currRadDist+inset, 0, -currRadDist+inset);
					makeArcedPathSection(num, pathWid, -90f, -180f, pathStart, latestDelta, Vector3.zero, lq.Lefts[i], lq.Rights[i], false);
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

					makeArcedPathSection(num, pathWid, -180f, -270f, pathStart, latestDelta, Vector3.zero, lq.Lefts[i], lq.Rights[i], false);
					pathStart.x += layerWid;
					currRadDist -= layerWid;
				}
				break;
		}
	}


	// TODO: connect to the first 2 orphan vertices (if currVertId <= 2) 
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
		//numVerts = getNumVertsForThisManyEdgeDoublings(numDoublings, numVerts);
		makeArc(rights,	startR, endR, pivotAnchor, numVerts);

		if (addPointsToFinalList)
			stitchLeftEdgesToRightEdges(numSub, lefts, rights);
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


	// an "edge" here is a straight segment (or side, if we were talking about closed polygons) between 2 vertices. 
	// to make things simple, every edge resolution increase will double the number of edges 
	int getNumVertsForThisManyEdgeDoublings(int numDoublings, int numVerts) {
		for (int i = 0; i < numDoublings; i++) {
			numVerts += (numVerts - 1);
		}

		return numVerts;
	}


	void stitchLeftEdgesToRightEdges(int numDoublings, List<Vector3> lefts, List<Vector3> rights) { // ....and fill intermediate lists 
		lId = 0; // left index 
		rId = 0; // right index 

		// let's try a different way










		return;

		// make 1st rung (2 points, which are outside of subsequent repeatable patterns) 
		addVertAndUvToGlobalPool(rights[rId++]);
		addVertAndUvToGlobalPool(lefts[lId++]);


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
			//Debug.Log("\nlId: " + lId);

			if (lefts.Count == rights.Count) {
				//Debug.Log("\n the stitch is a simple quad ladder");
				makeOneRLRTriangle(rights);
				makeOneLLRTriangle(lefts);
			}else{ 
				Debug.Log("\n it has more verts on the right, requiring 3 or more tris per chunk");
				//while (rId <= rights.Count/2) {
					makeRLRTriangleFan(2);
					makeOneLLRTriangleAfterFan(); // exact middle tri (of a rung to rung chunk) pointing STRAIGHT outwards 
				//}
			}
		}
	}


	void makeOneLLRTriangleAfterFan() {
		addVertAndUvToGlobalPool(lefts[lId]);

		triIndices.Add(anchL);
		triIndices.Add(currVert - 1);
		triIndices.Add(currVert - 2);
	}


	void makeOneRLRTriangle(List<Vector3> rights) {
		addVertAndUvToGlobalPool(rights[rId++]);

		triIndices.Add(currVert - 3);
		triIndices.Add(currVert - 2);
		triIndices.Add(currVert - 1);
	}


	void makeRLRTriangleFan(int numTris) {
		// notes:
		// the left tri remains the same
		// always use the latest 2 right tris (with same anchor point in the middle, as we travel down path) 

		// diff indices from makeOneRLRTriangle, because we're before the rId++ here 
		int prevR = currVert - 2;
		anchL = currVert - 1;
		int currR = currVert;

		for (int i = 0; i < numTris; i++) {
			addVertAndUvToGlobalPool(rights[rId++]);

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


	void makeOneLLRTriangle(List<Vector3> lefts) {
		addVertAndUvToGlobalPool(lefts[lId]);

		triIndices.Add(currVert - 3);
		triIndices.Add(currVert - 1);
		triIndices.Add(currVert - 2);
	}


	void addVertAndUvToGlobalPool(Vector3 v) {
		verts.Add(v);
		uvs.Add(new Vector2(v.x/4, v.z/4));
		currVert++;
	}
}