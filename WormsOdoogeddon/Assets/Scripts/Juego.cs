﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
