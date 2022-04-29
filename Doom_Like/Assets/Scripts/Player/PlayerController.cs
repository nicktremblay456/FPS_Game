using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private static PlayerController m_Instance;
    public static PlayerController Instance { get => m_Instance; }

    [Space, Header("Player Settings")]
    [SerializeField] private float m_MoveSpeed = 5.0f;
    [SerializeField] private float m_MouseSensitivity = 1.0f;

    [SerializeField] private float m_MinRotation = 40f;
    [SerializeField] private float m_MaxRotation = 140f;

    private PlayerInput m_Input;
    private Rigidbody m_RigidBody;
    private Camera m_Camera;

    private void Awake() 
    {
        if (m_Instance == null)
            m_Instance = this;
        m_RigidBody = GetComponent<Rigidbody>();
        m_Input = GetComponent<PlayerInput>();
    }

    private void Start() 
    {
        m_Camera = Camera.main;
        GameManager.playerController = this;

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (GameManager.Instance.savedPositions.ContainsKey(sceneIndex))
        {
            transform.position = GameManager.Instance.savedPositions[sceneIndex];
        }
    }

    private void Update() 
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.eulerAngles.y, transform.rotation.eulerAngles.z - m_Input.CameraInput.x);

        Vector3 camRot = new Vector3(m_Camera.transform.localRotation.eulerAngles.x, Mathf.Clamp(m_Camera.transform.localRotation.eulerAngles.y, m_MinRotation, m_MaxRotation), m_Camera.transform.localRotation.eulerAngles.z);
        m_Camera.transform.localRotation = Quaternion.Euler(camRot + new Vector3(0f, m_Input.CameraInput.y, 0f));
    }

    private void FixedUpdate() 
    {
        Vector3 moveHorizontal = transform.up * -m_Input.MoveInput.x;
        Vector3 moveVertical = transform.right * m_Input.MoveInput.y;

        m_RigidBody.velocity = (moveHorizontal + moveVertical) * m_MoveSpeed * Time.fixedDeltaTime;
    }

    private void OnDestroy()
    {
        // This Try/Catch is only use to remove to error log when exiting play mode.
        try
        {
            int sceneIndex = SceneManager.GetActiveScene().buildIndex;
            GameManager.Instance.savedPositions[sceneIndex] = transform.position;
        }
        catch
        {
            return;
        }
    }
}