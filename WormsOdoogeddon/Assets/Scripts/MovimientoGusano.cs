using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;
    private Rigidbody2D miRigibody;
    public int velocidad;
    float moveHorizontal, moveVertical;
    private SpriteRenderer sprite;
    private Animator anim;

    // Use this for initialization
    void Start () {
        miTransform = this.transform;
        miRigibody = GetComponent<Rigidbody2D>();
        moveHorizontal = 0;
        moveVertical = 0;
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
	}
    public void moverDerecha() {
        moveHorizontal = 10;
        velocidad = 100;
        sprite.flipX = true;
        anim.SetBool("moviendo", true);
    }
    public void pararDeMover() {
        velocidad = 0;
        anim.SetBool("moviendo", false);
    }
    public void moverIzquierda() {
        moveHorizontal= -10;
        velocidad = 100;
        sprite.flipX = false;
        anim.SetBool("moviendo", true);
    }
    public void saltar() {
        moveVertical = 10;
        velocidad = 650;
    }

    void FixedUpdate() {

        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        miRigibody.AddForce(movement * velocidad);
    }
}
