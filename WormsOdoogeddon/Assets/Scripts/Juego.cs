using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Juego : MonoBehaviour {

    public GameObject posJ1, posJ2;
    private Cliente ClienteScript;

	// Use this for initialization
	void Start () {
        ClienteScript = (Cliente)FindObjectOfType(typeof(Cliente));
        if (ClienteScript.getposJ1().transform.position.z == -300) {
            ClienteScript.setposJ1(posJ1);
        } else {
            ClienteScript.setposJ2(posJ2);
        }
        
    }

    private void Awake() {
        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
