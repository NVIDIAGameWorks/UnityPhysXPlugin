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

using System;

namespace NVIDIA.PhysX.Unity
{
    public delegate void DependencyAction(IPxDependency dependency);

    public interface IPxDependency
    {
        #region Properties

        bool created { get; }

        #endregion

        #region Events

        event DependencyAction onAfterCreate;
        event DependencyAction onBeforeDestroy;
        event DependencyAction onBeforeRecreate;
        event DependencyAction onAfterRecreate;

        #endregion
    }

    public static class PxDependencyArrayExtension
    {
        public static IPxDependency[] Append(this IPxDependency[] a, IPxDependency[] b)
        {
            var c = new IPxDependency[a.Length + b.Length];
            Array.Copy(a, c, a.Length);
            Array.Copy(b, 0, c, a.Length, b.Length);
            return c;
        }
    }

}