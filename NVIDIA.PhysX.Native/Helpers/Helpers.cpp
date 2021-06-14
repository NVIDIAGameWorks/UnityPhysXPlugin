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

#include <header.h>

physx::PxCudaContextManager* PxFoundation_createCudaContextManager(physx::PxFoundation& foundation, const char* dllPath)
{
    struct GpuLoadHook : PxGpuLoadHook
    {
        std::string dllPath;
        virtual const char* getPhysXGpuDllName() const { return dllPath.c_str(); }
    };
    static GpuLoadHook sl_gpuLoadHook;
    sl_gpuLoadHook.dllPath = dllPath;
    PxSetPhysXGpuLoadHook(&sl_gpuLoadHook);
    physx::PxCudaContextManagerDesc cudaDesc;
    return PxCreateCudaContextManager(foundation, cudaDesc);
}

//void physx::PxErrorToExceptionCallback::reportError(physx::PxErrorCode::Enum code, const char* message, const char* file, int line)
//{
//    switch (code)
//    {
//    case physx::PxErrorCode::Enum::eNO_ERROR: break;
//    case physx::PxErrorCode::Enum::eDEBUG_INFO: break;
//    case physx::PxErrorCode::Enum::eDEBUG_WARNING: break;
//    case physx::PxErrorCode::Enum::eINVALID_PARAMETER: throw std::exception(message); break;
//    case physx::PxErrorCode::Enum::eINVALID_OPERATION: throw std::exception(message); break;
//    case physx::PxErrorCode::Enum::eOUT_OF_MEMORY: throw std::exception(message); break;
//    case physx::PxErrorCode::Enum::eINTERNAL_ERROR: throw std::exception(message); break;
//    case physx::PxErrorCode::Enum::eABORT: throw std::exception(message); break;
//    case physx::PxErrorCode::Enum::ePERF_WARNING: break;
//    }
//}

void physx::PxRigidActorList::addRigidActor(physx::PxRigidActor* actor)
{
    m_actors.push_back(actor);
}

physx::PxU32 physx::PxRigidActorList::getNbRigidActors() const
{
    return (PxU32)m_actors.size();
}

physx::PxRigidActor* physx::PxRigidActorList::getRigidActor(PxU32 index)
{
    return m_actors[index];
}

void physx::PxRigidActorList::getRigidActorMatrices(PxF32* matrices, PxU32 start, PxU32 count)
{
    for (PxU32 i = 0; i < count; ++i)
    {
        ((PxMat44*)matrices)[i] = PxMat44(m_actors[i + start]->getGlobalPose());
    }
}

void physx::PxRigidActorList::releaseRigidActors()
{
    for (auto a : m_actors)
        a->release();
}

physx::PxFilterFlags physx::PxCustomSimulationFilterShader(physx::PxFilterObjectAttributes attributes0, physx::PxFilterData filterData0, physx::PxFilterObjectAttributes attributes1, physx::PxFilterData filterData1, physx::PxPairFlags& pairFlags, const void* constantBlock, physx::PxU32 constantBlockSize)
{
    auto flags = PxDefaultSimulationFilterShader(attributes0, filterData0, attributes1, filterData1, pairFlags, constantBlock, constantBlockSize);
    if ((uint32_t)flags == 0 && pairFlags == physx::PxPairFlag::eCONTACT_DEFAULT && ((filterData0.word1 & 1) || (filterData1.word1 & 1)))
        pairFlags |= physx::PxPairFlag::eNOTIFY_TOUCH_FOUND | physx::PxPairFlag::eNOTIFY_TOUCH_PERSISTS | physx::PxPairFlag::eNOTIFY_TOUCH_LOST | physx::PxPairFlag::eNOTIFY_CONTACT_POINTS;
    if ((pairFlags & physx::PxPairFlag::eDETECT_DISCRETE_CONTACT) && !PxFilterObjectIsTrigger(attributes0) && !PxFilterObjectIsTrigger(attributes1))
        pairFlags |= physx::PxPairFlag::eDETECT_CCD_CONTACT; // @@@
    return flags;
}

void physx::PxSetCallbacksEnabled(physx::PxActor& actor, bool yes)
{
    physx::PxActorType::Enum aType = actor.getType();
    switch (aType)
    {
    case physx::PxActorType::eRIGID_DYNAMIC:
    case physx::PxActorType::eRIGID_STATIC:
    case physx::PxActorType::eARTICULATION_LINK:
    {
        const physx::PxRigidActor& rActor = static_cast<const physx::PxRigidActor&>(actor);

        physx::PxShape* shape;
        for (physx::PxU32 i=0; i < rActor.getNbShapes(); i++)
        {
            rActor.getShapes(&shape, 1, i);

            physx::PxFilterData resultFd = shape->getSimulationFilterData();

            if (yes) resultFd.word1 |= 1;
            else resultFd.word1 &= ~1;

            shape->setSimulationFilterData(resultFd);
        }
    }
    break;
    case physx::PxActorType::eACTOR_COUNT:
    case physx::PxActorType::eACTOR_FORCE_DWORD:
        break;
    }
}

bool physx::PxGetCallbacksEnabled(const physx::PxActor& actor)
{
    PxActorType::Enum aType = actor.getType();
    switch (aType)
    {
    case PxActorType::eRIGID_DYNAMIC:
    case PxActorType::eRIGID_STATIC:
    case PxActorType::eARTICULATION_LINK:
    {
        const PxRigidActor& rActor = static_cast<const PxRigidActor&>(actor);
        if(rActor.getNbShapes() < 1) return false;

        PxShape* shape = NULL;
        rActor.getShapes(&shape, 1);

        physx::PxFilterData resultFd = shape->getSimulationFilterData();
        return (resultFd.word1 & 1) != 0;
    }
    break;

    case PxActorType::eACTOR_COUNT:
    case PxActorType::eACTOR_FORCE_DWORD:
        break;
    }

    return false;
}

namespace physx
{
    PxUnityCollisionFiltering PxUnityCollisionFiltering::instance;

    void PxUnityCollisionFiltering::setCollision(PxShape* shape, PxShapeCollision* collision)
    {
        shape->userData = collision;
    }

    PxShapeCollision* PxUnityCollisionFiltering::getCollision(const PxShape* shape)
    {
        return (shape && shape->userData && *(PxU32*)shape->userData == PxShapeCollision::SIGNATURE) ? (PxShapeCollision*)shape->userData : nullptr;
    }

    void PxUnityCollisionFiltering::setCollision(PxActor* actor, PxActorCollision* collision)
    {
        actor->userData = collision;
    }

    PxActorCollision* PxUnityCollisionFiltering::getCollision(const PxActor* actor)
    {
        return (actor && actor->userData && *(PxU32*)actor->userData == PxActorCollision::SIGNATURE) ? (PxActorCollision*)actor->userData : nullptr;
    }

    void PxUnityCollisionFiltering::setCollision(PxScene* scene, PxSceneCollision* collision)
    {
        scene->userData = collision;
    }

    PxSceneCollision* PxUnityCollisionFiltering::getCollision(const PxScene* scene)
    {
        return (scene && scene->userData && *(PxU32*)scene->userData == PxSceneCollision::SIGNATURE) ? (PxSceneCollision*)scene->userData : nullptr;
    }

    PxFilterFlags PxUnityCollisionFiltering::shader(PxFilterObjectAttributes attributes0, PxFilterData filterData0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, PxPairFlags& pairFlags, const void* constantBlock, PxU32 constantBlockSize)
    {
        //if (PxFilterObjectIsTrigger(attributes0) || PxFilterObjectIsTrigger(attributes1))
        //{
        //    pairFlags = PxPairFlag::eTRIGGER_DEFAULT;
        //    return PxFilterFlags();
        //}

        if (PxFilterObjectIsTrigger(attributes0) || PxFilterObjectIsTrigger(attributes1))
            return PxFilterFlag::eKILL;

        return physx::PxFilterFlag::eCALLBACK;
    }

    PxFilterFlags PxUnityCollisionFiltering::pairFound(PxU32 pairID, PxFilterObjectAttributes attributes0, PxFilterData filterData0, const PxActor* a0, const PxShape* s0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, const PxActor* a1, const PxShape* s1, PxPairFlags& pairFlags)
    {
        //if (pairFlags == physx::PxPairFlag::eTRIGGER_DEFAULT)
        //{
        //    return PxFilterFlags();
        //}

        auto sceneCollision = getCollision(a0->getScene());
        auto actor0Collision = getCollision(a0);
        auto actor1Collision = getCollision(a1);
        auto shape0Collision = getCollision(s0);
        auto shape1Collision = getCollision(s1);

        if (!sceneCollision)
            return PxFilterFlag::eKILL;

        if (!actor0Collision || !actor1Collision)
        {
            pairFlags = PxPairFlag::eDETECT_DISCRETE_CONTACT | PxPairFlag::eDETECT_CCD_CONTACT;
        }
        else if (actor0Collision != actor1Collision)
        {
            if (sceneCollision->canCollide(actor0Collision->index, actor1Collision->index))
                pairFlags = PxPairFlag::eDETECT_DISCRETE_CONTACT | PxPairFlag::eDETECT_CCD_CONTACT;
            else
                return PxFilterFlag::eKILL;
        }
        else
        {
            if (!shape0Collision || !shape1Collision || actor0Collision->canCollide(shape0Collision->index, shape1Collision->index))
                pairFlags = PxPairFlag::eDETECT_DISCRETE_CONTACT | PxPairFlag::eDETECT_CCD_CONTACT;
            else
                return PxFilterFlag::eKILL;
        }

        if (pairFlags | physx::PxPairFlag::eDETECT_DISCRETE_CONTACT)
        {
            if (PxFilterObjectIsTrigger(attributes0) || PxFilterObjectIsTrigger(attributes1))
            {
                pairFlags |= physx::PxPairFlag::eNOTIFY_TOUCH_FOUND |
                             physx::PxPairFlag::eNOTIFY_TOUCH_LOST;
            }
            else
            {
                if ((actor0Collision && actor0Collision->collisionEvents) || (actor1Collision && actor1Collision->collisionEvents))
                {
                    pairFlags |= physx::PxPairFlag::eNOTIFY_TOUCH_FOUND |
                                 physx::PxPairFlag::eNOTIFY_TOUCH_PERSISTS |
                                 physx::PxPairFlag::eNOTIFY_TOUCH_LOST |
                                 physx::PxPairFlag::eNOTIFY_CONTACT_POINTS;
                }
                if ((!shape0Collision || shape0Collision->solveContacts) && (!shape1Collision || shape1Collision->solveContacts))
                {
                    pairFlags |= physx::PxPairFlag::eSOLVE_CONTACT;
                }
            }
        }

        if (!(pairFlags & (physx::PxPairFlag::eNOTIFY_TOUCH_FOUND | physx::PxPairFlag::eSOLVE_CONTACT)))
            return PxFilterFlag::eKILL;

        return PxFilterFlags();
    }
}
