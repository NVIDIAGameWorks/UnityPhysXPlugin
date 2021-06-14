using System;
using System.Collections.Generic;
using UnityEngine;
using PX = NVIDIA.PhysX;

namespace NVIDIA.PhysX.Unity
{
    public class PxCompoundShape : PxShape
    {
        #region Properties

        public override bool valid { get { return Array.Exists(m_shapes, x => x && x.valid); } }

        public override PX.PxShape[] apiShapes
        {
            get
            {
                var shapeList = new List<PX.PxShape>();
                foreach (var s in m_shapes)
                    if (s.valid) shapeList.AddRange(s.apiShapes);
                return shapeList.ToArray();
            }
        }

        public override float[] densities
        {
            get
            {
                var densityList = new List<float>();
                foreach (var s in m_shapes)
                    if (s.valid) densityList.AddRange(s.densities);
                return densityList.ToArray();
            }
        }

        public PxShape[] shapes { get { return m_shapes; } set { m_shapes = value; ValidateShape(); } }

        #endregion

        #region Protected

        protected override void ValidateShape()
        {
            CheckLoops();
            base.ValidateShape();
        }

        #endregion

        #region Private

        void CheckLoops()
        {
            for (int i = 0; i < m_shapes.Length; ++i)
                if (LoopDependency(m_shapes[i]))
                    m_shapes[i] = null;
        }

        bool LoopDependency(PxShape shape)
        {
            var queue = new Queue<PxShape>();
            queue.Enqueue(shape);
            while (queue.Count > 0)
            {
                var item = queue.Dequeue() as PxCompoundShape;

                if (item == this) return true;

                if (item != null)
                    foreach (var s in item.shapes)
                        queue.Enqueue(s);

            }
            return false;
        }

        [SerializeField]
        PxShape[] m_shapes = new PxShape[0];

        #endregion
    }
}
