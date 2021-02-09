// This code contains NVIDIA Confidential Information and is disclosed to you
// under a form of NVIDIA software license agreement provided separately to you.
//
// Notice
// NVIDIA Corporation and its licensors retain all intellectual property and
// proprietary rights in and to this software and related documentation and
// any modifications thereto. Any use, reproduction, disclosure, or
// distribution of this software and related documentation without an express
// license agreement from NVIDIA Corporation is strictly prohibited.
//
// ALL NVIDIA DESIGN SPECIFICATIONS, CODE ARE PROVIDED "AS IS.". NVIDIA MAKES
// NO WARRANTIES, EXPRESSED, IMPLIED, STATUTORY, OR OTHERWISE WITH RESPECT TO
// THE MATERIALS, AND EXPRESSLY DISCLAIMS ALL IMPLIED WARRANTIES OF NONINFRINGEMENT,
// MERCHANTABILITY, AND FITNESS FOR A PARTICULAR PURPOSE.
//
// Information and code furnished is believed to be accurate and reliable.
// However, NVIDIA Corporation assumes no responsibility for the consequences of use of such
// information or for any infringement of patents or other rights of third parties that may
// result from its use. No license is granted by implication or otherwise under any patent
// or patent rights of NVIDIA Corporation. Details are subject to change without notice.
// This code supersedes and replaces all information previously supplied.
// NVIDIA Corporation products are not authorized for use as critical
// components in life support devices or systems without express written approval of
// NVIDIA Corporation.
//
// Copyright (c) 2019 NVIDIA Corporation. All rights reserved.

using UnityEngine;

public class SampleList : MonoBehaviour
{
    #region Messages

    private void Start()
    {
        Application.targetFrameRate = -1;

        m_sampleList = GetComponentsInChildren<SampleBase>(true);

        foreach (var s in m_sampleList)
            s.gameObject.SetActive(false);

        if (m_sampleList.Length > 0)
            SetActiveSample(0);
    }

    private void Update()
    {
        m_prevFrameTime = m_currFrameTime;
        m_currFrameTime = Time.realtimeSinceStartup;

        if (Input.GetKeyDown(KeyCode.M))
            m_showMenu = !m_showMenu;

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            int nextSample = m_activeSample;
            if (Input.GetKey(KeyCode.LeftShift)) nextSample = (m_activeSample + m_sampleList.Length - 1) % m_sampleList.Length;
            else nextSample = (m_activeSample + 1) % m_sampleList.Length;
            SetActiveSample(nextSample);
        }

        if (Input.GetKeyDown(KeyCode.R))
            SetActiveSample(m_activeSample);
    }

    GUIStyle guiStyle = new GUIStyle();

    private void OnGUI()
    {
        var oldColor = GUI.contentColor;

        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        float k = 0.9f;
        m_smoothFrameTime = m_smoothFrameTime * k + (m_currFrameTime - m_prevFrameTime) * (1 - k);
        GUI.contentColor = Color.Lerp(Color.white, Color.red, Mathf.Clamp01((m_smoothFrameTime - Time.fixedDeltaTime) / Time.fixedDeltaTime));
        GUILayout.Label(string.Format(" {0:0}", 1.0f / m_smoothFrameTime), guiStyle, GUILayout.Width(100));

        if (m_showMenu)
        {
            for (int i = 0; i < m_sampleList.Length; ++i)
            {
                GUI.contentColor = i == m_activeSample ? Color.yellow : Color.white;
                if (GUILayout.Button(m_sampleList[i].name.Replace("Sample ", ""), GUILayout.Width(140))) SetActiveSample(i);
            }
        }

        GUI.contentColor = oldColor;
    }

    #endregion

    #region Private

    void SetActiveSample(int index)
    {
        if (m_activeSample >= 0)
            m_sampleList[m_activeSample].gameObject.SetActive(false);

        m_activeSample = index;

        if (m_activeSample >= 0)
            m_sampleList[m_activeSample].gameObject.SetActive(true);
    }

    SampleBase[] m_sampleList;
    int m_activeSample = -1;
    bool m_showMenu = true;
    float m_currFrameTime, m_prevFrameTime, m_smoothFrameTime;

    #endregion
}
