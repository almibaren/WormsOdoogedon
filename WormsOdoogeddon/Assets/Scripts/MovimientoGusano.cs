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
    private GameObject posLejana,rotador,posCercana;
    private float rotation, actualRotation, rotationPerSec;
    public bool golpeado;
    public int vida;

    // Use this for initialization
    void Start () {
        miTransform = this.transform;
        miRigibody = GetComponent<Rigidbody2D>();
        moveHorizontal = 0;
        moveVertical = 0;
        sprite = GetComponent<SpriteRenderer>();
        spritePuntoDeMira = GameObject.FindGameObjectWithTag("posLejana").GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rotador = this.transform.Find("rotador").gameObject;
        rotation = 0;
        actualRotation = 0;
        rotationPerSec = 0;
        golpeado = false;
        vida = 3;
        
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag.Equals("Bala")) {
            Debug.Log("Golpeado");
            golpeado = true;
        }
    }

    // Update is called once per frame
    void Update () {
        rotation = rotationPerSec * Time.deltaTime;
        actualRotation = rotador.transform.localRotation.eulerAngles.z;
        rotador.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, actualRotation + rotation));
    }
    public void moverDerecha() {
        miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled=false;
        moveHorizontal = 10;
        velocidad = 30;
        sprite.flipX = true;
        anim.SetBool("moviendo", true);
    }
    public void pararDeMover() {
        miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = true;
        velocidad = 0;
        anim.SetBool("moviendo", false);
        rotationPerSec = 0;
    }
    public void moverIzquierda() {
        miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = false;
        moveHorizontal = -10;
        velocidad = 30;
        sprite.flipX = false;
        anim.SetBool("moviendo", true);
    }
    public void saltar() {
        miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = false;
        moveVertical = 10;
        velocidad = 500;
    }
    public void apuntarArriba() {
        rotationPerSec = -20;
    }
    public void apuntarAbajo() {
        rotationPerSec = 20;
    }

    public Vector3 getDireccionDisparo() {
        return miTransform.FindChild("rotador").FindChild("posLejana").transform.position - miTransform.FindChild("rotador").FindChild("posCercana").transform.position;
    }

    void FixedUpdate() {

        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        miRigibody.AddForce(movement * velocidad);
    }
}
