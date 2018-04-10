using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
[] - Brackets
{} - Braces
() - Parenthesis

Hot keys:
- Clean Code:   CTRL + K + D
- Fold Code:    CTRL + M + O
- UnFold Code:  CTRL + M + P
*/

public class Player : MonoBehaviour
{
    public string message = "Hello World!";
    float speed = 5;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F3))
        {
            print(message);
        }

    }
}
