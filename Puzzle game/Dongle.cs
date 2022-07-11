using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public bool isDrag;
    Rigidbody2D rigid;

    void Awake() 
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if(isDrag) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // X축 경계 설정
            float leftBorder = -4.2f + transform.localScale.x / 2;
            float rightBorder = 4.2f - transform.localScale.x / 2;

            if(mousePos.x < leftBorder){
                mousePos.x = leftBorder;
         }
            else if (mousePos.x > rightBorder){
            mousePos.x = rightBorder;
        }

            mousePos.z = 0;
            mousePos.y = 8;
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f);
        }    
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
}
