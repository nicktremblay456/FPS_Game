using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppLauncher : MonoBehaviour
{
    [SerializeField] private string m_SceneNameToLoad = "";
    [SerializeField] private float m_Timer = 5f;
    private WaitForSeconds m_WaitForLoading;

    private void Awake() 
    {
        m_WaitForLoading = new WaitForSeconds(m_Timer);
    }

    private void Start() 
    {
        StartCoroutine(WaitForInit());
    }

    private IEnumerator WaitForInit()
    {
        yield return m_WaitForLoading;
        GameManager.Instance.TransitionToScene(m_SceneNameToLoad);
    }
}