using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ClimbController : MonoBehaviour
{
    EnvironmentScanner envScanner;

    public void Awake()
    {
        envScanner = GetComponent<EnvironmentScanner>();        
    }

    private void Update()
    {
        if(Input.GetButton("Jump"))
        {
            if(envScanner.ClimbLedgeCheck(transform.forward, out RaycastHit ledgeHit))
            {
                Debug.Log("Ledge Hit : " + ledgeHit.transform.name);
            }
        }
    }
}
