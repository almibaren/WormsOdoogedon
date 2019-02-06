using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Gorrete : MonoBehaviour {
    private Cliente clienteScript;
    private GameObject miGameObject;


    // Use this for initialization
    void Start() {
        clienteScript = (Cliente)FindObjectOfType(typeof(Cliente));
        miGameObject = this.gameObject;
    }

    // Update is called once per frame
    void Update() {

    }
    public void click() {
        Debug.Log(this.gameObject.name);
        clienteScript.seleccionarGorro(miGameObject);
    }

}