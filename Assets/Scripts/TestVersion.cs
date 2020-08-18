using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestVersion : MonoBehaviour
{
    private void Start()
    {
        
    }

    private void Update()
    {
        if(Input.GetButton("Fire1"))
        {
            Debug.Log("Hello");
        }
    }
}
