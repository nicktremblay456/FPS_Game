using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    private float m_MaxRecoil_x = -20.0f;
    private float m_MaxRecoil_y = -10.0f;
    private float m_MaxTrans_x = 1.0f;
    private float m_MaxTrans_z = -1.0f;
    private float m_RecoilSpeed = 10.0f;
    public float Recoil = 0.0f;

    private void Update()
    {
        if (Recoil > 0)
        {
            Quaternion maxRecoil = Quaternion.Euler(
                Random.Range(transform.localRotation.x, m_MaxRecoil_x),
                Random.Range(transform.localRotation.y, m_MaxRecoil_y),
                transform.localRotation.z);

            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, maxRecoil, Time.deltaTime * m_RecoilSpeed);

            Vector3 maxTranslation = new Vector3(
                Random.Range(transform.localPosition.x, m_MaxTrans_x),
                transform.localPosition.y,
                Random.Range(transform.localPosition.z, m_MaxTrans_z));

            transform.localPosition = Vector3.Slerp(transform.localPosition, maxTranslation, Time.deltaTime * m_RecoilSpeed);

            Recoil -= Time.deltaTime;
        }
        else
        {
            Recoil = 0;
            Quaternion minRecoil = Quaternion.Euler(
                Random.Range(0, transform.localRotation.x),
                Random.Range(0, transform.localRotation.y),
                transform.localRotation.z);

            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, minRecoil, Time.deltaTime * m_RecoilSpeed / 2);

            Vector3 minTranslation = new Vector3(
                Random.Range(0, transform.localPosition.x),
                transform.localPosition.y,
                Random.Range(0, transform.localPosition.z));

            transform.localPosition = Vector3.Slerp(transform.localPosition, minTranslation, Time.deltaTime * m_RecoilSpeed);
        }
    }
}
