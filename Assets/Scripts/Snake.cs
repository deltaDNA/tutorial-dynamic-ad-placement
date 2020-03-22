using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    private GameManager gm; 

    private void Start()
    {
        gm = GameObject.FindObjectOfType<GameManager>();
    }
    void OnTriggerEnter(Collider c)
    {
        Debug.Log("Collider " + c.name);
        if(c.name.StartsWith("Food"))
        {
            gm.EatFood(c.gameObject);
        }

    }

}
