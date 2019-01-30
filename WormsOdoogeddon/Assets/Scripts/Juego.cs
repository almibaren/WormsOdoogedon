using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Juego : MonoBehaviour {

    public GameObject posJ1, posJ2;
    private Cliente clienteScript;

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
    public void salto() {
        clienteScript.saltar();
    }
    public void parar() {
        clienteScript.parar();
    }
}
