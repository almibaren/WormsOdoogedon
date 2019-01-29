using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;
    private Rigidbody2D miRigibody;
    public int velocidad;
    float moveHorizontal, moveVertical;

    // Use this for initialization
    void Start () {
        miTransform = this.transform;
        miRigibody = GetComponent<Rigidbody2D>();
        moveHorizontal = 0;
        moveVertical = 0;
    }
	
	// Update is called once per frame
	void Update () {
	}
    public void moverDerecha() {
        moveHorizontal = 10;
        velocidad = 10;
    }
    public void moverIzquierda() {
        moveHorizontal = -10;
        velocidad = 10;
    }
    public void saltar() {
        moveVertical = 100;
        velocidad = 10;
    }

    void FixedUpdate() {

        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        miRigibody.AddForce(movement * velocidad);
        velocidad = 0;
    }
}
