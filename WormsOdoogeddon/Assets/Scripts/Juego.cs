using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    public GameObject posJ1, posJ2;
    private Cliente clienteScript;
    public Text vidas;

	// Use this for initialization
	void Start () {
        clienteScript = (Cliente)FindObjectOfType(typeof(Cliente));
        clienteScript.juegoEmpezado();
        clienteScript.setpos(posJ1,posJ2);

    }

    private void Awake() {
        
    }

    // Update is called once per frame
    void Update () {
	}

    public void derecha() {
        clienteScript.mover("derecha");
    }
    public void izquierda() {
        clienteScript.mover("izquierda");
    }
    public void arriba() {
        clienteScript.apuntar("arriba");
    }
    public void abajo() {
        clienteScript.apuntar("abajo");
    }
    public void salto() {
        clienteScript.saltar();
    }
    public void parar() {
        clienteScript.parar();
    }
    public void disparar() {
        clienteScript.disparar();
    }
    public void cambiarVidas(int vida) {
        vidas.text = vida + " ";
    }
    public void jugadorPierde(string playerName) {
        vidas.text = playerName;
    }
}
