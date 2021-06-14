using NVIDIA.PhysX.Unity;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxAssetEditor : Editor
    {
        protected virtual void InitProperties()
        {
        }

        protected void BeginRecreateCheck()
        {
            EditorGUI.BeginChangeCheck();
        }

        protected void EndRecreateCheck()
        {
            if (EditorGUI.EndChangeCheck())
                serializedObject.FindProperty("m_forceRecreate").boolValue = true;
        }

        protected PreviewRenderUtility m_previewRender = null;
        protected Vector3 m_cameraTarget = Vector3.zero;
        protected Material m_previewLinesMaterial = null;
        protected Mesh m_previewLines = null;

        protected virtual void CreatePreviewRender()
        {
            m_previewRender = new PreviewRenderUtility();
            m_previewRender.cameraFieldOfView = 30.0f;
            m_previewLinesMaterial = new Material(Shader.Find("PhysX/DrawPreviewLines"));
        }

        protected virtual void DestroyPreviewRender()
        {
            if (m_previewLines) DestroyImmediate(m_previewLines);
            if (m_previewLinesMaterial) DestroyImmediate(m_previewLinesMaterial);
            m_previewRender.Cleanup();
            m_previewRender = null;
        }

        protected virtual void GetPreviewLines(List<Vector3> positions, List<Color> colors)
        {
            positions.Add(Vector3.right); colors.Add(Color.red);
            positions.Add(Vector3.zero); colors.Add(Color.black);
            positions.Add(Vector3.up); colors.Add(Color.green);
            positions.Add(Vector3.zero); colors.Add(Color.black);
            positions.Add(Vector3.forward); colors.Add(Color.blue);
            positions.Add(Vector3.zero); colors.Add(Color.black);
        }

        protected void ResetPreview()
        {
            if (m_previewLines) DestroyImmediate(m_previewLines);
        }

        public override bool HasPreviewGUI()
        {
            return false;
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (Event.current.type == EventType.Repaint)
            {
                EditorGUIUtility.AddCursorRect(r, MouseCursor.Orbit);

                m_previewRender.BeginPreview(r, background);
                var commandBuffer = new CommandBuffer();
                if (!m_previewLines)
                {
                    var positions = new List<Vector3>();
                    var colors = new List<Color>();
                    GetPreviewLines(positions, colors);
                    var mesh = new Mesh();
                    mesh.vertices = positions.ToArray();
                    mesh.colors = colors.ToArray();
                    var indices = new int[positions.Count];
                    for (int i = 0; i < positions.Count; ++i) indices[i] = i;
                    mesh.indexFormat = IndexFormat.UInt32;
                    mesh.SetIndices(indices, MeshTopology.Lines, 0);
                    mesh.RecalculateBounds();
                    m_previewLines = mesh;
                    //
                    Bounds bounds = mesh.bounds;
                    float radius = bounds.center.magnitude + bounds.size.magnitude * 0.5f;
                    float angle = m_previewRender.camera.fieldOfView * Mathf.Deg2Rad * 0.5f;
                    float distance = radius / Mathf.Sin(angle);
                    m_cameraTarget = Vector3.zero;// bounds.center;
                    Transform cameraTransform = m_previewRender.camera.transform;
                    cameraTransform.position = m_cameraTarget - cameraTransform.forward * distance;
                    m_previewRender.camera.nearClipPlane = 0.001f;// distance - radius;
                    m_previewRender.camera.farClipPlane = 1000.0f;// distance + radius;
                }
                commandBuffer.DrawMesh(m_previewLines, Matrix4x4.identity, m_previewLinesMaterial);
                //PreviewCommands(commandBuffer);
                m_previewRender.camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
                m_previewRender.camera.cullingMask = 0;
                m_previewRender.camera.Render();
                m_previewRender.camera.RemoveCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
                m_previewRender.EndAndDrawPreview(r);
            }
            else if (Event.current.type == EventType.MouseDrag && r.Contains(Event.current.mousePosition))
            {
                Vector2 mouseDelta = Event.current.delta;
                Transform cameraTransform = m_previewRender.camera.transform;
                cameraTransform.RotateAround(m_cameraTarget, Vector3.up, mouseDelta.x);
                cameraTransform.RotateAround(m_cameraTarget, cameraTransform.right, mouseDelta.y);
                Event.current.Use();
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.ScrollWheel && r.Contains(Event.current.mousePosition))
            {
                Vector2 scrollDelta = Event.current.delta;
                Transform cameraTransform = m_previewRender.camera.transform;
                float distance = Vector3.Distance(cameraTransform.position, m_cameraTarget);
                distance *= (1.0f + scrollDelta.y * 0.03f);
                cameraTransform.position = m_cameraTarget - cameraTransform.forward * distance;
                Event.current.Use();
                GUI.changed = true;
            }
        }

        protected static void AddLine(List<Vector3> positions, List<Color> colors, Vector3 from, Vector3 to, Color color)
        {
            positions.Add(from); colors.Add(color);
            positions.Add(to); colors.Add(color);
        }

        protected static void AddBox(List<Vector3> positions, List<Color> colors, Vector3 halfExtents, Color color)
        {
            var hs = halfExtents;
            AddLine(positions, colors, new Vector3(hs.x, hs.y, hs.z), new Vector3(hs.x, hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, hs.y, hs.z), new Vector3(-hs.x, hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, -hs.y, hs.z), new Vector3(-hs.x, -hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(hs.x, -hs.y, hs.z), new Vector3(hs.x, -hs.y, -hs.z), color);

            AddLine(positions, colors, new Vector3(hs.x, hs.y, hs.z), new Vector3(-hs.x, hs.y, hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, hs.y, hs.z), new Vector3(-hs.x, -hs.y, hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, -hs.y, hs.z), new Vector3(hs.x, -hs.y, hs.z), color);
            AddLine(positions, colors, new Vector3(hs.x, -hs.y, hs.z), new Vector3(hs.x, hs.y, hs.z), color);

            AddLine(positions, colors, new Vector3(hs.x, hs.y, -hs.z), new Vector3(-hs.x, hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, hs.y, -hs.z), new Vector3(-hs.x, -hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(-hs.x, -hs.y, -hs.z), new Vector3(hs.x, -hs.y, -hs.z), color);
            AddLine(positions, colors, new Vector3(hs.x, -hs.y, -hs.z), new Vector3(hs.x, hs.y, -hs.z), color);
        }

        protected static void AddArc(List<Vector3> positions, List<Color> colors, Vector3 _centre, Vector3 _axis, Vector3 _start, float _radius, float _angle, Color color, float _error = 0.01f)
        {
            float maxStep = 2.0f * Mathf.Acos((_radius - _error) / _radius);
            int steps = (int)(_angle / maxStep);
            float step = _angle / steps;
            for (int i = 0; i < steps; ++i)
                AddLine(positions, colors, _centre + Quaternion.AngleAxis(step * i, _axis) * _start * _radius,
                        _centre + Quaternion.AngleAxis(step * (i + 1), _axis) * _start * _radius, color);
        }

        protected static void AddCapsule(List<Vector3> positions, List<Color> colors, float radius, float halfHeight, Color color, float error = 0.01f)
        {
            Vector3 p0 = Vector3.right * halfHeight;
            Vector3 p1 = Vector3.left * halfHeight;

            AddArc(positions, colors, p0, Vector3.right, Vector3.up, radius, 360.0f, color, error);
            AddArc(positions, colors, p0, Vector3.up, Vector3.forward, radius, 180.0f, color, error);
            AddArc(positions, colors, p0, Vector3.back, Vector3.up, radius, 180.0f, color, error);

            AddArc(positions, colors, p1, Vector3.left, Vector3.up, radius, 360.0f, color, error);
            AddArc(positions, colors, p1, Vector3.down, Vector3.forward, radius, 180.0f, color, error);
            AddArc(positions, colors, p1, Vector3.forward, Vector3.up, radius, 180.0f, color, error);

            AddLine(positions, colors, p0 + Vector3.up * radius, p1 + Vector3.up * radius, color);
            AddLine(positions, colors, p0 + Vector3.forward * radius, p1 + Vector3.forward * radius, color);
            AddLine(positions, colors, p0 + Vector3.down * radius, p1 + Vector3.down * radius, color);
            AddLine(positions, colors, p0 + Vector3.back * radius, p1 + Vector3.back * radius, color);
        }

        protected static void AddSphere(List<Vector3> positions, List<Color> colors, float radius, Color color, float error = 0.01f)
        {
            Vector3 c = Vector3.zero;

            AddArc(positions, colors, c, Vector3.right, Vector3.up, radius, 360.0f, color, error);
            AddArc(positions, colors, c, Vector3.up, Vector3.forward, radius, 180.0f, color, error);
            AddArc(positions, colors, c, Vector3.back, Vector3.up, radius, 180.0f, color, error);

            AddArc(positions, colors, c, Vector3.left, Vector3.up, radius, 360.0f, color, error);
            AddArc(positions, colors, c, Vector3.down, Vector3.forward, radius, 180.0f, color, error);
            AddArc(positions, colors, c, Vector3.forward, Vector3.up, radius, 180.0f, color, error);
        }

        //protected virtual void PreviewCommands(CommandBuffer commandBuffer)
        //{
        //}
    }
}