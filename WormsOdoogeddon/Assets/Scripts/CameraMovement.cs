using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    private Vector3 initPos;
    private Vector3 lastPos;
    private float dragDistance;

    void Start() {
        dragDistance = Screen.height * 15 / 100;
    }

    void Update() {

        if (Input.touchCount == 1) {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) {
                initPos = touch.position;
                lastPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) {
                lastPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) {
                lastPos = touch.position;


                if (Mathf.Abs(lastPos.x - initPos.x) > dragDistance || Mathf.Abs(lastPos.y - initPos.y) > dragDistance) {
                    //check if the drag is vertical or horizontal
                    if (Mathf.Abs(lastPos.x - initPos.x) > Mathf.Abs(lastPos.y - initPos.y)) {   
                        if ((lastPos.x > initPos.x))
                            Debug.Log("Right Swipe");
                    }
                    else {
                        Debug.Log("Left Swipe");
                    }
                }
            }
            else {   //It's a tap as the drag distance is less than 20% of the screen height
                Debug.Log("Tap");
            }
        }
    }
}

