using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Boton : MonoBehaviour, IPointerDownHandler, IPointerClickHandler,
    IPointerUpHandler, IPointerExitHandler, IPointerEnterHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Juego juego;

    public void setJuego(Juego juego) {
        this.juego = juego;
    }
    public void OnBeginDrag(PointerEventData eventData) {
    }

    public void OnDrag(PointerEventData eventData) {
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    public void OnPointerClick(PointerEventData eventData) {
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (this.tag.Equals("Izquierda")) {
            juego.izquierda();
        } else if (this.tag.Equals("Derecha")) {
            juego.derecha();
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData) {
    }

    public void OnPointerExit(PointerEventData eventData) {
    }

    public void OnPointerUp(PointerEventData eventData) {
        juego.parar();
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
