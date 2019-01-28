using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;

	// Use this for initialization
	void Start () {
        miTransform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void mover() {
        miTransform.Translate(new Vector3(1, 0, 0) * 1 * Time.deltaTime);
    }
}
