using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelRotator : MonoBehaviour
{
    [SerializeField] private GameObject m_BarrelToRotate;
    [SerializeField] private float m_RotateSpeed;

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            RotateObject();
        }
    }

    public void RotateObject()
    {
        m_BarrelToRotate.transform.Rotate(0f, 0f, 1f * m_RotateSpeed * Time.deltaTime, Space.Self);
    }
}
