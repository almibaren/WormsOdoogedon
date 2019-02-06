using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    public GameObject posJ1, posJ2;
    private Cliente clienteScript;
    public Text vidas,turno,vidasRival;
    public bool miTurno = false;

    // Use this for initialization
    void Start () {
        clienteScript = (Cliente)FindObjectOfType(typeof(Cliente));
        clienteScript.setpos(posJ1,posJ2);
        turno.enabled = false;

    }

    private void Awake() {
        
    }

    // Update is called once per frame
    void Update () {
	}

    public void derecha() {
        if (miTurno) {
            clienteScript.mover("derecha");
        }
    }
    public void izquierda() {
        if (miTurno) {
            clienteScript.mover("izquierda");
        }
    }
    public void arriba() {
        if (miTurno) {
            clienteScript.apuntar("arriba");
        }
    }
    public void abajo() {
        if (miTurno) {
            clienteScript.apuntar("abajo");
        }
    }
    public void salto() {
        if (miTurno) {
            clienteScript.saltar();
        }
    }
    public void parar() {
        if (miTurno) {
            clienteScript.parar();
        }
    }
    public void disparar() {
        if (miTurno) {
            clienteScript.disparar();
        }
    }
    public void cambiarVidasLocal(int vida) {
        vidas.text = vida + " ";
    }
    public void cambiarVidasRival(int vida) {
        vidasRival.text = vida + "";
    }
    public void jugadorPierde(string playerName) {
        turno.text = playerName + " ha perdido";
    }
}
