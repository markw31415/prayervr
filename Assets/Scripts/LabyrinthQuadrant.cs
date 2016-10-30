using UnityEngine;
using System.Collections.Generic;



public class LabyrinthQuadrant {
	// Lists of lists, for building the edge lines of all concentric elbow sections 
	public List<List <Vector3>> Lefts = new List<List <Vector3>>();
	public List<List <Vector3>> Rights = new List<List <Vector3>>();



	public LabyrinthQuadrant() {
		for (int i = 0; i < 7; i++) {
			Lefts.Add(new List<Vector3>()); 
			Rights.Add(new List<Vector3>());
		}
	}
}
