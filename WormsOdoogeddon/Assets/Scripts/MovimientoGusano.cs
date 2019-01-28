using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;
    private Rigidbody2D miRigibody;
    public int velocidad;

	// Use this for initialization
	void Start () {
        miTransform = this.transform;
        miRigibody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
	}
    public void moverDerecha() {
        miTransform.Translate(new Vector3(1, 0, 0) * velocidad * Time.deltaTime);
    }
    public void moverIzquierda() {
        miTransform.Translate(new Vector3(-1, 0, 0) * velocidad * Time.deltaTime);
    }
    public void saltar() {
        miRigibody.AddForce(new Vector2(1, 1) * velocidad);
    }
}
