using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float m_Smooth;
    [SerializeField] private float m_SwayMultiplier;

    private void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * m_SwayMultiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * m_SwayMultiplier;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, m_Smooth * Time.deltaTime);
    }
}