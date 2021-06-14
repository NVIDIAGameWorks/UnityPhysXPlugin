using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PXU = NVIDIA.PhysX.Unity;

public class SamplePCS : SampleBase
{
    #region Properties

    public GameObject m_samplePrefab;

    #endregion

    #region Protected

    protected override void CreateSample()
    {
        base.CreateSample();

        if (m_samplePrefab)
            m_sampleInstance = GameObject.Instantiate<GameObject>(m_samplePrefab);

        m_pxScene = PXU.PxScene.main;
        if (m_pxScene)
            m_pxScene.onBeforeSimulation += OnBeforeSimulation;
    }

    protected override void DestroySample()
    {
        if (m_pxScene)
            m_pxScene.onBeforeSimulation -= OnBeforeSimulation;

        m_pxScene = null;

        if (m_sampleInstance)
            DestroyImmediate(m_sampleInstance);

        base.DestroySample();
    }

    #endregion

    #region Messages

    private void Update()
    {
        UpdateCamera();
        UpdateInput();

        if (m_pxScene)
            UpdatePicker(m_pxScene.apiScene);
    }

    #endregion

    #region Private

    void OnBeforeSimulation()
    {
        if (m_throwBall) ThrowBall();
    }

    void ThrowBall()
    {
        var pos = Camera.main.transform.position + Camera.main.transform.forward * 2 + Camera.main.transform.up * -2;
        var rot = Camera.main.transform.rotation;

        var throwBallPrefab = Resources.Load<GameObject>("ThrowBall");
        var throwBall = GameObject.Instantiate(throwBallPrefab, pos, rot, m_sampleInstance.transform);
        var dynamicActor = throwBall.GetComponent<PXU.PxDynamicActor>();
        if (dynamicActor)
            dynamicActor.linearVelocity = Camera.main.transform.forward * 80;
    }

    void UpdateInput()
    {
        m_throwBall = Input.GetKey(KeyCode.Space);
    }

    void UpdateCamera()
    {
        const float MOTION_SPEED = 20.0f;
        const float ROTATION_SPEED = 0.3f;
        float forward = 0, right = 0;
        if (Input.GetKey(KeyCode.W)) forward += 1;
        if (Input.GetKey(KeyCode.S)) forward -= 1;
        if (Input.GetKey(KeyCode.D)) right += 1;
        if (Input.GetKey(KeyCode.A)) right -= 1;
        float dt = Time.deltaTime;
        var cameraTransform = Camera.main.transform;
        cameraTransform.Translate(Vector3.forward * forward * MOTION_SPEED * dt + Vector3.right * right * MOTION_SPEED * dt, Space.Self);

        if (Input.GetMouseButtonDown(1))
        {
            m_mousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            var mouseDelta = Input.mousePosition - m_mousePosition;
            cameraTransform.Rotate(Vector3.up, mouseDelta.x * ROTATION_SPEED, Space.World);
            cameraTransform.Rotate(Vector3.left, mouseDelta.y * ROTATION_SPEED, Space.Self);
            m_mousePosition = Input.mousePosition;
        }
    }

    GameObject m_sampleInstance;
    Vector3 m_mousePosition;
    bool m_throwBall = false;
    PXU.PxScene m_pxScene = null;

    #endregion
}
