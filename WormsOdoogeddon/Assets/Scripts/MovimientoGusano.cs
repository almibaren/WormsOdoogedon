using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;
    private Rigidbody2D miRigibody;
    public int velocidad;
    float moveHorizontal, moveVertical;
    private SpriteRenderer sprite,spritePuntoDeMira;
    private Animator anim;
    private GameObject posLejana;

    // Use this for initialization
    void Start () {
        miTransform = this.transform;
        miRigibody = GetComponent<Rigidbody2D>();
        moveHorizontal = 0;
        moveVertical = 0;
        sprite = GetComponent<SpriteRenderer>();
        spritePuntoDeMira = GameObject.FindGameObjectWithTag("posLejana").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        
    }
	
	// Update is called once per frame
	void Update () {

	}
    public void moverDerecha() {
        miTransform.FindChild("rotador").FindChild("posLejana").GetComponent<SpriteRenderer>().enabled=false;
        moveHorizontal = 10;
        velocidad = 30;
        sprite.flipX = true;
        anim.SetBool("moviendo", true);
    }
    public void pararDeMover() {
        miTransform.FindChild("rotador").FindChild("posLejana").GetComponent<SpriteRenderer>().enabled = true;
        velocidad = 0;
        anim.SetBool("moviendo", false);
    }
    public void moverIzquierda() {
        miTransform.FindChild("rotador").FindChild("posLejana").GetComponent<SpriteRenderer>().enabled = false;
        moveHorizontal = -10;
        velocidad = 30;
        sprite.flipX = false;
        anim.SetBool("moviendo", true);
    }
    public void saltar() {
        miTransform.FindChild("rotador").FindChild("posLejana").GetComponent<SpriteRenderer>().enabled = false;
        moveVertical = 10;
        velocidad = 500;
    }

    void FixedUpdate() {

        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        miRigibody.AddForce(movement * velocidad);
    }
}
