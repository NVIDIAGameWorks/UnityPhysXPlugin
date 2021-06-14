using NVIDIA.PhysX.Unity;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxShapeEditor : PxAssetEditor
    {
        protected override void CreatePreviewRender()
        {
            base.CreatePreviewRender();
        }

        protected override void DestroyPreviewRender()
        {
            base.DestroyPreviewRender();
        }

        public override bool HasPreviewGUI()
        {
            return false;// targets.Length == 1;
        }
    }
}