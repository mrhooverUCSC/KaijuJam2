using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LineDrawer : MonoBehaviour {
    private LineRenderer lineRend;
    private Canvas screenCanvas;
    private bool isDragging = false;
    private Vector3 mousePos;
    private Vector3 startMousePos;

    // [SerializeField] private Text distanceText;

    // private float distance;

    // Start is called before the first frame update
    void Start() {
        // lineRend.SetPosition(0, new Vector3(0f, 0f, -60f));
        // lineRend.SetPosition(1, new Vector3(0f, 0f, -60f));
        lineRend = GetComponent<LineRenderer>();
        // screenCanvas = GetComponent<Canvas>(); 
        lineRend.positionCount = 2;
    }

    // Update is called once per frame
    void Update() {
        // if(isDragging) {
        //     Vector2 movePos;
        //     RectTransformUtility.ScreenPointToLocalPointInRectangle(
        //         screenCanvas.transform as RectTransform,
        //         Input.mousePosition,
        //         screenCanvas.worldCamera,
        //         out movePos
        //     );

        //     lineRend.SetPosition(0, transform.position);
        //     lineRend.SetPosition(1, screenCanvas.transform.TransformPoint(movePos));
        // }

        if(Input.GetMouseButtonDown(0)) {
            startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        if(Input.GetMouseButton(0)) {
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            lineRend.SetPosition(0, new Vector3(startMousePos.x, startMousePos.y, startMousePos.z));
            lineRend.SetPosition(1, new Vector3(mousePos.x, mousePos.y, mousePos.z));
            // distance = (mousePos - startMousePos).magnitude;
            // distanceText.text = distance.ToString("F2") + "meters";
        }
    }

    void SetStartPosition(Transform pos) {
        lineRend.SetPosition(0, new Vector3(pos.position.x, pos.position.y, -60f));
    }

    void SetEndPosition(Transform pos) {
        lineRend.SetPosition(1, new Vector3(pos.position.x, pos.position.y, -60f));
    }

    // public void OnBeginDrag(PointerEventData eventData) {
    //     isDragging = true;
    // }

    // public void OnEndDrag(PointerEventData eventData) {
    //     isDragging = false;
    // }

    // public void OnDrag(PointerEventData eventData) {
    // }
}
