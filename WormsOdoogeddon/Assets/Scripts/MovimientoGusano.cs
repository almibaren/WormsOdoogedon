using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MovimientoGusano : MonoBehaviour {

    private Transform miTransform;
    private Rigidbody2D miRigibody;
    public int velocidad;
    float moveHorizontal, moveVertical;
    private SpriteRenderer sprite,spritePuntoDeMira;
    private Animator anim;
    private GameObject posLejana,rotador,posCercana;
    private float rotation, actualRotation, rotationPerSec;
    public Text vidaTexto;
    public bool golpeado,tocandoSuelo=false;
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
        vidaTexto.text = 3 + "";
    }
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag.Equals("Bala")) {
            Debug.Log("Golpeado");
            golpeado = true;
            vidaTexto.text = int.Parse(vidaTexto.text) - 1 +"";
        }
    }
    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.transform.tag.Equals("Mapa") || collision.transform.tag.Equals("Player")) {
            Debug.Log("TocandoSuelo");
            tocandoSuelo = true;
            moveVertical = 0;
        }
    }
    private void OnCollisionExit2D(Collision2D collision) {
        if (collision.transform.tag.Equals("Mapa")) {
            Debug.Log("NoTocandoSuelo");
            tocandoSuelo = false;
        }
    }

    // Update is called once per frame
    void Update () {
        rotation = rotationPerSec * Time.deltaTime;
        actualRotation = rotador.transform.localRotation.eulerAngles.z;
        rotador.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, actualRotation + rotation));

    }
    public void moverDerecha() {
        if (tocandoSuelo) {
            miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = false;
            moveHorizontal = 10;
            velocidad = 30;
            sprite.flipX = true;
            miTransform.Find("Gorreador").transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = true;
            miTransform.Find("rotador").Find("posLejana").localPosition = miTransform.Find("rotador").Find("posLejana").localPosition * new Vector2(-1, 1);
            anim.SetBool("moviendo", true);
        }
    }
    public void pararDeMover() {
            miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = true;
            velocidad = 0;
            anim.SetBool("moviendo", false);
            rotationPerSec = 0;
    }
    public void moverIzquierda() {
        if (tocandoSuelo) {
            miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = false;
            moveHorizontal = -10;
            velocidad = 30;
            sprite.flipX = false;
            miTransform.Find("Gorreador").transform.GetChild(0).GetComponent<SpriteRenderer>().flipX = false;
            miTransform.Find("rotador").Find("posLejana").localPosition = miTransform.Find("rotador").Find("posLejana").localPosition * new Vector2(-1, 1);
            anim.SetBool("moviendo", true);
        }
    }
    public void saltar() {
        if (tocandoSuelo) {
            miTransform.Find("rotador").Find("posLejana").GetComponent<SpriteRenderer>().enabled = false;
            moveVertical = 100;
            velocidad = 50;
        }
    }
    public void apuntarArriba() {
        rotationPerSec = -20;
    }
    public void apuntarAbajo() {
        rotationPerSec = 20;
    }

    public Vector3 getDireccionDisparo() {
        return miTransform.Find("rotador").Find("posLejana").transform.position - miTransform.Find("rotador").Find("posCercana").transform.position;
    }

    void FixedUpdate() {

        Vector3 movement = new Vector2(moveHorizontal, moveVertical);

        miRigibody.AddForce(movement * velocidad);
    }
}
