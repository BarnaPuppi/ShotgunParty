using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class RGBEffect : MonoBehaviour {
    private SpriteRenderer colorRing;
    private Color color;
    private float t = 0;
    private float colorSpeed = 1f;

    private Color red = new Color(1, 0, 0);
    private Color yellow = new Color(1, 1, 0);
    private Color green = new Color(0, 1, 0);
    private Color cyan = new Color(0, 1, 1);
    private Color blue = new Color(0, 0, 1);
    private Color magenta = new Color(1, 0, 1);  
    
    void Start() {
        color = red;
        colorRing = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update() {
        t += Time.deltaTime * colorSpeed;
        if (t < 1) 
            color = Color.Lerp(red, yellow, t);
        else if (t < 2)
            color = Color.Lerp(yellow, green, t - 1);
        else if (t < 3)
            color = Color.Lerp(green, cyan, t - 2);
        else if (t < 4)
            color = Color.Lerp(cyan, blue, t - 3);
        else if (t < 5)
            color = Color.Lerp(blue, magenta, t - 4);
        else if (t < 6)
            color = Color.Lerp(magenta, red, t - 5);
        else t = 0;

        colorRing.color = color;
        
    }
}
