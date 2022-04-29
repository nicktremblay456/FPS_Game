using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform m_Camera;
    
    private void Start() 
    {
        m_Camera = Camera.main.transform;
    }

    private void Update() 
    {
        if (m_Camera != null)
        {
            transform.LookAt(transform.position + m_Camera.transform.rotation * -Vector3.forward, m_Camera.rotation * Vector3.up);
        }
    }
}
