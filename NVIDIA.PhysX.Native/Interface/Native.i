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

%module(directors="1") Native
%{
#include "header.h"
%}

%include PxPhysicsVersion.h

%include stdint.i
%include exception.i
%include arrays_csharp.i
%include typemaps.i

%exception {
    try {
        $action
    } catch(std::exception e) {
        SWIG_exception(SWIG_RuntimeError,e.what());
    } catch(...) {
        SWIG_exception(SWIG_RuntimeError,"Unknown exception");
    }
}

%include BasicTypemaps.h

// Rename operators as C++ operators don't map directly to C# operators
%rename(__assign) *::operator=;
%rename(__equal) *::operator==;
%rename(__not_equal) *::operator!=;
%rename(__negate) *::operator-();
%rename(__add) *::operator+;
%rename(__subtract) *::operator-;
%rename(__multiply) *::operator*;
%rename(__divide) *::operator/;
%rename(__add_assign) *::operator+=;
%rename(__subtract_assign) *::operator-=;
%rename(__multiply_assign) *::operator*=;
%rename(__divide_assign) *::operator/=;

// Remove enum prefixes
%rename("%(regex:/e([A-Z][A-Z0-9_]*)/\\1/)s", %$isenumitem) "";

namespace physx {

    // PhysX basic typedefs
    typedef int64_t PxI64;
    typedef uint64_t PxU64;
    typedef int32_t PxI32;
    typedef uint32_t PxU32;
    typedef int16_t PxI16;
    typedef uint16_t PxU16;
    typedef int8_t PxI8;
    typedef uint8_t PxU8;
    typedef float PxF32;
    typedef double PxF64;
    typedef float PxReal;
    typedef PxU16 PxType;
    typedef PxU32 PxTriangleID;
    typedef PxU16 PxMaterialTableIndex;
    typedef PxU8 PxDominanceGroup;
    typedef PxU8 PxClientID;
    typedef bool PxAgain;
    typedef unsigned int PxTaskID;
    typedef PxU16 PxMaterialTableIndex;
    typedef PxU32 PxFilterObjectAttributes;

    // Forward declarations
    class PxVec3;
    class PxVec4;
    class PxQuat;
    class PxPlane;
    class PxTransform;
    class PxMat33;
    class PxMat44;
    class PxTolerancesScale;
    class PxPhysics;
    class PxSceneDesc;
    class PxScene;
    class PxAggregate;
    class PxInputStream;
    class PxBase;
    class PxActor;
    class PxRigidActor;
    class PxRigidStatic;
    class PxRigidBody;
    class PxShape;
    class PxRigidDynamic;
    class PxGeometry;
    class PxBoxGeometry;
    class PxSphereGeometry;
    class PxCapsuleGeometry;
    class PxPlaneGeometry;
    class PxConvexMeshGeometry;
    class PxConvexMesh;
    class PxTriangleMesh;
    class PxHeightField;
    class PxMaterial;
    class PxBVHStructure;
    class PxPruningStructure;
    struct Px1DConstraint;
    struct PxConstraintInvMassScale;
    class PxConstraint;
    struct PxConstraintFlag;
    class PxArticulationBase;
    class PxArticulation;
    class PxArticulationReducedCoordinate;
    class PxArticulationJointBase;
    class PxArticulationJoint;
    class PxArticulationJointReducedCoordinate;
    class PxArticulationLink;
    class PxArticulationDriveCache;
    class PxCudaContextManager;
    class PxJoint;
    class PxFixedJoint;
    class PxDistanceJoint;
    class PxSphericalJoint;
    class PxRevoluteJoint;
    class PxPrismaticJoint;
    class PxD6Joint;
    class PxRigidActorList;
    class PxPvd;
    class PxBaseTask;
    class PxTask;
    class PxLightCpuTask;
    class PxTaskManager;
    class PxCooking;
    class PxMassProperties;
    class PxCpuDispatcher;
    class PxDefaultCpuDispatcher;

    // @@@ Just placeholders for function pointer types. Not sure if they should be exposed.
    struct PxConstraintSolverPrep;
    struct PxConstraintProject;
    struct PxConstraintVisualize;

    CSHARP_BYREF_ARRAY(float, float)
    CSHARP_OBJECT_ARRAY(physx::PxActor, PxActor)
    CSHARP_OBJECT_ARRAY(physx::PxShape, PxShape)
    CSHARP_OBJECT_ARRAY(physx::PxMaterial, PxMaterial)
    CSHARP_OBJECT_ARRAY(physx::PxRigidActor, PxRigidActor)

    DECLARE_BASIC_TYPEMAPS

    SIMPLIFY_ENUM(PxFrictionType,
        ePATCH,
        eONE_DIRECTIONAL,
        eTWO_DIRECTIONAL,
    );

    SIMPLIFY_ENUM(PxSolverType,
        ePGS,
        eTGS
    );

    SIMPLIFY_ENUM(PxPairFilteringMode,
        eKEEP,
        eSUPPRESS,
        eKILL,
        eDEFAULT = 1 //eSUPPRESS
    );

    SIMPLIFY_ENUM(PxBroadPhaseType,
        eSAP,
        eMBP,
        eABP,
        eGPU,
    );

    SIMPLIFY_FLAGS_ENUM(PxSceneFlag,
        eENABLE_ACTIVE_ACTORS = (1<<0),
        eENABLE_CCD = (1<<1),
        eDISABLE_CCD_RESWEEP = (1<<2),
        eADAPTIVE_FORCE = (1<<3),
        eENABLE_PCM = (1 << 6),
        eDISABLE_CONTACT_REPORT_BUFFER_RESIZE = (1 << 7),
        eDISABLE_CONTACT_CACHE = (1 << 8),
        eREQUIRE_RW_LOCK = (1 << 9),
        eENABLE_STABILIZATION = (1 << 10),
        eENABLE_AVERAGE_POINT = (1 << 11),
        eEXCLUDE_KINEMATICS_FROM_ACTIVE_ACTORS = (1 << 12),
        eENABLE_GPU_DYNAMICS = (1 << 13),
        eENABLE_ENHANCED_DETERMINISM = (1<<14),
        eENABLE_FRICTION_EVERY_ITERATION = (1 << 15),
        eMUTABLE_FLAGS = (1<<0)|(1<<12) /*eENABLE_ACTIVE_ACTORS|eEXCLUDE_KINEMATICS_FROM_ACTIVE_ACTORS*/
    );

    SIMPLIFY_ENUM(PxPruningStructureType,
        eNONE,
        eDYNAMIC_AABB_TREE,
        eSTATIC_AABB_TREE,
    );

    SIMPLIFY_ENUM(PxSceneQueryUpdateMode,
        eBUILD_ENABLED_COMMIT_ENABLED,
        eBUILD_ENABLED_COMMIT_DISABLED,
        eBUILD_DISABLED_COMMIT_DISABLED
    );

    SIMPLIFY_FLAGS_ENUM(PxBaseFlag,
        eOWNS_MEMORY = (1<<0),
        eIS_RELEASABLE = (1<<1)
    );

    SIMPLIFY_FLAGS_ENUM(PxActorFlag,
        eVISUALIZATION = (1<<0),
        eDISABLE_GRAVITY = (1<<1),
        eSEND_SLEEP_NOTIFIES = (1<<2),
        eDISABLE_SIMULATION = (1<<3)
    );

    SIMPLIFY_ENUM(PxForceMode,
        eFORCE,
        eIMPULSE,
        eVELOCITY_CHANGE,
        eACCELERATION
    );

    SIMPLIFY_FLAGS_ENUM(PxRigidBodyFlag,
        eKINEMATIC = (1<<0),
        eENABLE_CCD = (1<<2),
        eENABLE_CCD_FRICTION = (1<<3),
        eENABLE_POSE_INTEGRATION_PREVIEW = (1 << 4),
        eENABLE_SPECULATIVE_CCD = (1 << 5),
        eENABLE_CCD_MAX_CONTACT_IMPULSE = (1 << 6),
        eRETAIN_ACCELERATIONS = (1<<7)
    );

    SIMPLIFY_FLAGS_ENUM(PxRigidDynamicLockFlag,
        eLOCK_LINEAR_X = (1 << 0),
        eLOCK_LINEAR_Y = (1 << 1),
        eLOCK_LINEAR_Z = (1 << 2),
        eLOCK_ANGULAR_X = (1 << 3),
        eLOCK_ANGULAR_Y = (1 << 4),
        eLOCK_ANGULAR_Z = (1 << 5)
    );

    SIMPLIFY_ENUM(PxGeometryType,
        eSPHERE,
        ePLANE,
        eCAPSULE,
        eBOX,
        eCONVEXMESH,
        eTRIANGLEMESH,
        eHEIGHTFIELD,
        eINVALID = -1
    );

    SIMPLIFY_FLAGS_ENUM(PxShapeFlag,
        eSIMULATION_SHAPE = (1<<0),
        eSCENE_QUERY_SHAPE = (1<<1),
        eTRIGGER_SHAPE = (1<<2),
        eVISUALIZATION = (1<<3)
    );

    SIMPLIFY_FLAGS_ENUM(PxConvexMeshGeometryFlag,
        eTIGHT_BOUNDS = (1<<0)
    );

    SIMPLIFY_FLAGS_ENUM(PxMeshGeometryFlag,
        eDOUBLE_SIDED = (1<<1)
    );

    SIMPLIFY_FLAGS_ENUM(PxHeightFieldFormat,
        eS16_TM = (1 << 0)
    );

    SIMPLIFY_FLAGS_ENUM(PxHeightFieldFlag,
        eNO_BOUNDARY_EDGES = (1 << 0)
    );

    SIMPLIFY_FLAGS_ENUM(PxTriangleMeshFlag,
        e16_BIT_INDICES = (1<<1),
        eADJACENCY_INFO = (1<<2)
    );

    SIMPLIFY_FLAGS_ENUM(PxMaterialFlag,
        eDISABLE_FRICTION = 1 << 0,
        eDISABLE_STRONG_FRICTION = 1 << 1,
        eIMPROVED_PATCH_FRICTION = 1 << 2
    );

    SIMPLIFY_ENUM(PxCombineMode,
        eAVERAGE = 0,
        eMIN = 1,
        eMULTIPLY = 2,
        eMAX = 3,
        //eN_VALUES = 4,
    );

    SIMPLIFY_FLAGS_ENUM(PxActorTypeFlag,
        eRIGID_STATIC = (1 << 0),
        eRIGID_DYNAMIC = (1 << 1)
    );

    SIMPLIFY_ENUM(PxPvdUpdateType,
        CREATE_INSTANCE,
        RELEASE_INSTANCE,
        UPDATE_ALL_PROPERTIES,
        UPDATE_SIM_PROPERTIES
    );

    SIMPLIFY_FLAGS_ENUM(PxConstraintFlag,
        eBROKEN = 1<<0,
        ePROJECT_TO_ACTOR0 = 1<<1,
        ePROJECT_TO_ACTOR1 = 1<<2,
        ePROJECTION = (1<<1)|(1<<2), /*ePROJECT_TO_ACTOR0 | ePROJECT_TO_ACTOR1,*/
        eCOLLISION_ENABLED = 1<<3,
        eVISUALIZATION = 1<<4,
        eDRIVE_LIMITS_ARE_FORCES = 1<<5,
        eIMPROVED_SLERP = 1<<7,
        eDISABLE_PREPROCESSING = 1<<8,
        eENABLE_EXTENDED_LIMITS = 1<<9,
        eGPU_COMPATIBLE = 1<<10
    );

    SIMPLIFY_ENUM(PxArticulationJointDriveType,
        eTARGET = 0,
        eERROR = 1
    );

    SIMPLIFY_ENUM(PxJointActorIndex,
        eACTOR0,
        eACTOR1,
        COUNT
    );

    SIMPLIFY_ENUM(PxD6Axis,
        eX = 0,
        eY = 1,
        eZ = 2,
        eTWIST = 3,
        eSWING1 = 4,
        eSWING2 = 5,
        eCOUNT = 6
    );

    SIMPLIFY_ENUM(PxD6Motion,
        eLOCKED,
        eLIMITED,
        eFREE
    );

    SIMPLIFY_ENUM(PxD6Drive,
        eX = 0,
        eY = 1,
        eZ = 2,
        eSWING = 3,
        eTWIST = 4,
        eSLERP = 5,
        eCOUNT = 6
    );

    SIMPLIFY_ENUM(PxD6JointDriveFlag,
        eACCELERATION = 1
    );

    SIMPLIFY_ENUM(PxErrorCode,
        eNO_ERROR          = 0,
        eDEBUG_INFO        = 1,
        eDEBUG_WARNING     = 2,
        eINVALID_PARAMETER = 4,
        eINVALID_OPERATION = 8,
        eOUT_OF_MEMORY     = 16,
        eINTERNAL_ERROR    = 32,
        eABORT             = 64,
        ePERF_WARNING      = 128,
        eMASK_ALL          = -1
    );

    SIMPLIFY_FLAGS_ENUM(PxPvdInstrumentationFlag,
        eDEBUG   = 1 << 0,
        ePROFILE = 1 << 1,
        eMEMORY  = 1 << 2,
        //eALL     = ((1<<0)|(1<<1)|(1<<2)) /*(eDEBUG | ePROFILE | eMEMORY)*/
    );

    SIMPLIFY_FLAGS_ENUM(PxHitFlag,
        ePOSITION                    = (1<<0),
        eNORMAL                        = (1<<1),
        eUV                            = (1<<3),
        eASSUME_NO_INITIAL_OVERLAP    = (1<<4),
        eMESH_MULTIPLE                = (1<<5),
        eMESH_ANY                    = (1<<6),
        eMESH_BOTH_SIDES            = (1<<7),
        ePRECISE_SWEEP                = (1<<8),
        eMTD                        = (1<<9),
        eFACE_INDEX                    = (1<<10),
        eDEFAULT                    = (1<<0)|(1<<1)|(1<<10),/*ePOSITION|eNORMAL|eFACE_INDEX,*/
        eMODIFIABLE_FLAGS            = (1<<5)|(1<<7)|(1<<4)|(1<<8),/*eMESH_MULTIPLE|eMESH_BOTH_SIDES|eASSUME_NO_INITIAL_OVERLAP|ePRECISE_SWEEP*/
    );

    SIMPLIFY_FLAGS_ENUM(PxQueryFlag,
        eSTATIC = (1<<0),
        eDYNAMIC = (1<<1),
        ePREFILTER = (1<<2),
        ePOSTFILTER = (1<<3),
        eANY_HIT = (1<<4),
        eNO_BLOCK = (1<<5),
        eRESERVED = (1<<15)
    );

    SIMPLIFY_ENUM(PxTaskType,
        TT_CPU,
        TT_NOT_PRESENT,
        TT_COMPLETED
    );

    SIMPLIFY_FLAGS_ENUM(PxArticulationFlag,
        eFIX_BASE = (1 << 0),
        eDRIVE_LIMITS_ARE_FORCES = (1<<1)
    );

    SIMPLIFY_ENUM(PxConvexMeshCookingType,
        eQUICKHULL
    );

    SIMPLIFY_ENUM(PxTriangleMeshCookingResult,
        eSUCCESS = 0,
        eLARGE_TRIANGLE,
        eFAILURE
    );

    SIMPLIFY_FLAGS_ENUM(PxMeshPreprocessingFlag,
        eWELD_VERTICES = 1 << 0, 
        eDISABLE_CLEAN_MESH = 1 << 1, 
        eDISABLE_ACTIVE_EDGES_PRECOMPUTE = 1 << 2,
        eFORCE_32BIT_INDICES = 1 << 3
    );

    SIMPLIFY_ENUM(PxMeshMidPhase,
        eBVH33 = 0,
        eBVH34 = 1,
        eLAST
    );

    SIMPLIFY_ENUM(PxMeshCookingHint,
        eSIM_PERFORMANCE = 0,
        eCOOKING_PERFORMANCE = 1
    );

    SIMPLIFY_FLAGS_ENUM(PxMeshFlag,
        eFLIPNORMALS = (1<<0),
        e16_BIT_INDICES = (1<<1)
    );

    SIMPLIFY_ENUM(PxConcreteType,
        eUNDEFINED,
        eHEIGHTFIELD,
        eCONVEX_MESH,
        eTRIANGLE_MESH_BVH33,
        eTRIANGLE_MESH_BVH34,
        eRIGID_DYNAMIC,
        eRIGID_STATIC,
        eSHAPE,
        eMATERIAL,
        eCONSTRAINT,
        eAGGREGATE,
        eARTICULATION,
        eARTICULATION_REDUCED_COORDINATE,
        eARTICULATION_LINK,
        eARTICULATION_JOINT,
        eARTICULATION_JOINT_REDUCED_COORDINATE,
        ePRUNING_STRUCTURE,
        eBVH_STRUCTURE,
        ePHYSX_CORE_COUNT,
        eFIRST_PHYSX_EXTENSION = 256,
        eFIRST_VEHICLE_EXTENSION = 512,
        eFIRST_USER_EXTENSION = 1024
    );

    SIMPLIFY_FLAGS_ENUM(PxConvexFlag,
        e16_BIT_INDICES = (1<<0),
        eCOMPUTE_CONVEX = (1<<1),
        eCHECK_ZERO_AREA_TRIANGLES = (1<<2),
        eQUANTIZE_INPUT = (1 << 3),
        eDISABLE_MESH_VALIDATION = (1 << 4),
        ePLANE_SHIFTING = (1 << 5),
        eFAST_INERTIA_COMPUTATION = (1 << 6),
        eGPU_COMPATIBLE = (1 << 7),
        eSHIFT_VERTICES = (1 << 8)
    );

    SIMPLIFY_ENUM(PxConvexMeshCookingResult,
        eSUCCESS,
        eZERO_AREA_TEST_FAILED,
        ePOLYGONS_LIMIT_REACHED,
        eFAILURE
    );

    SIMPLIFY_FLAGS_ENUM(PxPairFlag,
        eSOLVE_CONTACT = (1<<0),
        eMODIFY_CONTACTS = (1<<1),
        eNOTIFY_TOUCH_FOUND = (1<<2),
        eNOTIFY_TOUCH_PERSISTS = (1<<3),
        eNOTIFY_TOUCH_LOST = (1<<4),
        eNOTIFY_TOUCH_CCD = (1<<5),
        eNOTIFY_THRESHOLD_FORCE_FOUND = (1<<6),
        eNOTIFY_THRESHOLD_FORCE_PERSISTS = (1<<7),
        eNOTIFY_THRESHOLD_FORCE_LOST = (1<<8),
        eNOTIFY_CONTACT_POINTS = (1<<9),
        eDETECT_DISCRETE_CONTACT = (1<<10),
        eDETECT_CCD_CONTACT = (1<<11),
        ePRE_SOLVER_VELOCITY = (1<<12),
        ePOST_SOLVER_VELOCITY = (1<<13),
        eCONTACT_EVENT_POSE = (1<<14),
        /*eNEXT_FREE = (1<<15),*/
        eCONTACT_DEFAULT = (1<<0)|(1<<10), /*eSOLVE_CONTACT | eDETECT_DISCRETE_CONTACT,*/
        eTRIGGER_DEFAULT = (1<<2)|(1<<4)|(1<<10) /*eNOTIFY_TOUCH_FOUND | eNOTIFY_TOUCH_LOST | eDETECT_DISCRETE_CONTACT*/
    );

    SIMPLIFY_ENUM(PxFilterOp,
        PX_FILTEROP_AND,
        PX_FILTEROP_OR,
        PX_FILTEROP_XOR,
        PX_FILTEROP_NAND,
        PX_FILTEROP_NOR,
        PX_FILTEROP_NXOR,
        PX_FILTEROP_SWAP_AND
    );

    SIMPLIFY_FLAGS_ENUM(PxContactPairHeaderFlag,
        eREMOVED_ACTOR_0 = (1<<0),
        eREMOVED_ACTOR_1 = (1<<1)
    );

    SIMPLIFY_FLAGS_ENUM(PxContactPairFlag,
        eREMOVED_SHAPE_0 = (1<<0),
        eREMOVED_SHAPE_1 = (1<<1),
        eACTOR_PAIR_HAS_FIRST_TOUCH = (1<<2),
        eACTOR_PAIR_LOST_TOUCH = (1<<3),
        eINTERNAL_HAS_IMPULSES = (1<<4),
        eINTERNAL_CONTACTS_ARE_FLIPPED = (1<<5)
    );

    SIMPLIFY_FLAGS_ENUM(PxFilterFlag,
        eKILL = (1<<0),
        eSUPPRESS = (1<<1),
        eCALLBACK = (1<<2),
        eNOTIFY = (1<<3) | (1<<2),/*eCALLBACK,*/
        eDEFAULT = 0
    );

    SIMPLIFY_FLAGS_ENUM(PxPvdSceneFlag,
        eTRANSMIT_CONTACTS = (1 << 0),
        eTRANSMIT_SCENEQUERIES = (1 << 1),
        eTRANSMIT_CONSTRAINTS = (1 << 2)
    );

    SIMPLIFY_FLAGS_ENUM(PxRevoluteJointFlag,
        eLIMIT_ENABLED = 1<<0,
        eDRIVE_ENABLED = 1<<1,
        eDRIVE_FREESPIN = 1<<2
    );

    SIMPLIFY_FLAGS_ENUM(PxDistanceJointFlag,
        eMAX_DISTANCE_ENABLED = 1<<1,
        eMIN_DISTANCE_ENABLED = 1<<2,
        eSPRING_ENABLED = 1<<3
    );

    SIMPLIFY_FLAGS_ENUM(PxSphericalJointFlag,
        eLIMIT_ENABLED    = 1<<1
    );

    SIMPLIFY_FLAGS_ENUM(PxPrismaticJointFlag,
        eLIMIT_ENABLED    = 1<<1
    );

    SIMPLIFY_ENUM(PxArticulationDriveType,
        eFORCE = 0,
        eACCELERATION = 1
    );

    SIMPLIFY_ENUM(PxArticulationJointType,
        eFIX = 0,
        ePRISMATIC = 1,
        eREVOLUTE = 2,
        eSPHERICAL = 3,
        eUNDEFINED = 4
        //ePRISMATIC = 0,
        //eREVOLUTE = 1,
        //eSPHERICAL = 2,
        //eFIX = 3,
        //eUNDEFINED = 4
    );

    SIMPLIFY_ENUM(PxArticulationAxis,
        eTWIST = 0,
        eSWING1 = 1,
        eSWING2 = 2,
        eX = 3,
        eY = 4,
        eZ = 5,
        //eCOUNT = 6
    );

    SIMPLIFY_ENUM(PxArticulationMotion,
        eLOCKED = 0,
        eLIMITED = 1,
        eFREE = 2
    );

    enum PxEMPTY {
        PxEmpty
    };

    enum PxZERO {
        PxZero
    };

    enum PxIDENTITY {
        PxIdentity
    };

    FLAT_STRUCT(physx::PxVec3, PxVec3, public float x, y, z;)
    FLAT_STRUCT(physx::PxVec4, PxVec4, public float x, y, z, w;)
    FLAT_STRUCT(physx::PxQuat, PxQuat, public float x, y, z, w;)
    FLAT_STRUCT(physx::PxPlane, PxPlane, public PxVec3 n; public float d;)
    FLAT_STRUCT(physx::PxTransform, PxTransform, public PxQuat q; public PxVec3 p;)
    FLAT_STRUCT(physx::PxMat33, PxMat33, public PxVec3 column0, column1, column2;)
    FLAT_STRUCT(physx::PxMat44, PxMat44, public PxVec4 column0, column1, column2, column3;)
    FLAT_STRUCT(physx::PxBounds3, PxBounds3, public PxVec3 minimum, maximum;)
    FLAT_STRUCT(physx::PxMassProperties, PxMassProperties, public PxMat33 inertiaTensor; public PxVec3 centerOfMass; public float mass;)
    FLAT_STRUCT(physx::PxHeightFieldSample, PxHeightFieldSample, public short height; public byte materialIndex0, materialIndex1;)
    FLAT_STRUCT(physx::PxContactPairPoint, PxContactPairPoint, public PxVec3 position; public float separation; public PxVec3 normal; public uint internalFaceIndex0; public PxVec3 impulse; public uint internalFaceIndex1;)
    FLAT_STRUCT(physx::PxSimulationFilterShader, PxSimulationFilterShader, global::System.IntPtr fnPtr;)
    FLAT_STRUCT(physx::PxGroupsMask, PxGroupsMask, public ushort bits0, bits1, bits2, bits3;)
    FLAT_STRUCT(physx::PxDefaultSimulationFilterShader, PxDefaultSimulationFilterShader, )
    FLAT_STRUCT(physx::PxFilterData, PxFilterData, public uint word0, word1, word2, word3;)
    FLAT_STRUCT(physx::PxHullPolygon, PxHullPolygon, public PxPlane mPlane; public short mNbVerts, mIndexBase;)

    WRAPPER_STRUCT(physx::PxContactPairHeader)
    WRAPPER_STRUCT(physx::PxContactPair)
    WRAPPER_STRUCT(physx::PxTriggerPair)
    WRAPPER_STRUCT(physx::PxList)
    WRAPPER_STRUCT(physx::PxPhysicsInsertionCallback)
    WRAPPER_STRUCT(physx::PxConstraintInfo)

    WRAPPER_CLASS(PxFoundation)
    WRAPPER_CLASS(PxPhysics)
    WRAPPER_CLASS(PxScene)
    WRAPPER_CLASS(PxBase)
    WRAPPER_CLASS(PxActor)
    WRAPPER_CLASS(PxRigidActor)
    WRAPPER_CLASS(PxRigidStatic)
    WRAPPER_CLASS(PxRigidBody)
    WRAPPER_CLASS(PxRigidDynamic)
    WRAPPER_CLASS(PxShape)
    WRAPPER_CLASS(PxConvexMesh)
    WRAPPER_CLASS(PxTriangleMesh)
    WRAPPER_CLASS(PxHeightField)
    WRAPPER_CLASS(PxMaterial)
    WRAPPER_CLASS(PxBVHStructure)
    WRAPPER_CLASS(PxPruningStructure)
    WRAPPER_CLASS(PxConstraint)
    WRAPPER_CLASS(PxArticulationJointBase)
    WRAPPER_CLASS(PxArticulationJoint)
    WRAPPER_CLASS(PxArticulationJointReducedCoordinate)
    WRAPPER_CLASS(PxArticulationLink)
    WRAPPER_CLASS(PxArticulationBase)
    WRAPPER_CLASS(PxArticulation)
    WRAPPER_CLASS(PxArticulationReducedCoordinate)
    WRAPPER_CLASS(PxAggregate)
    WRAPPER_CLASS(PxCudaContextManager)
    WRAPPER_CLASS(PxJoint)
    WRAPPER_CLASS(PxFixedJoint)
    WRAPPER_CLASS(PxRevoluteJoint);
    WRAPPER_CLASS(PxDistanceJoint);
    WRAPPER_CLASS(PxSphericalJoint);
    WRAPPER_CLASS(PxPrismaticJoint);
    WRAPPER_CLASS(PxD6Joint)
    WRAPPER_CLASS(PxPvdTransport)
    WRAPPER_CLASS(PxProfilerCallback)
    WRAPPER_CLASS(PxPvd)
    WRAPPER_CLASS(PxCpuDispatcher)
    WRAPPER_CLASS(PxDefaultCpuDispatcher)
    WRAPPER_CLASS(PxBaseTask)
    WRAPPER_CLASS(PxTask)
    WRAPPER_CLASS(PxTaskManager)
    WRAPPER_CLASS(PxLightCpuTask)
    WRAPPER_CLASS(PxCooking)

    class PxVec3 {
    public:
        //float x, y, z;
        //PxVec3();
        //PxVec3(float s);
        %extend { PxVec3(float s) { thread_local physx::PxVec3 v; v = physx::PxVec3(s); return &v; }}
        //PxVec3(float x, float y, float z);
        %extend { PxVec3(float x, float y, float z) { thread_local physx::PxVec3 v; v = physx::PxVec3(x, y, z); return &v; }}
        //PxVec3& operator=(const PxVec3& p);
        //float& operator[](unsigned int index);
        //const float& operator[](unsigned int index) const;
        //bool operator==(const PxVec3& v) const;
        //bool operator!=(const PxVec3& v) const;
        bool isFinite() const;
        bool isNormalized() const;
        float magnitudeSquared() const;
        float magnitude() const;
        //PxVec3 operator-() const;
        //PxVec3 operator+(const PxVec3& v) const;
        //PxVec3 operator-(const PxVec3& v) const;
        //PxVec3 operator*(float f) const;
        //PxVec3 operator/(float f) const;
        //PxVec3& operator+=(const PxVec3& v);
        //PxVec3& operator-=(const PxVec3& v);
        //PxVec3& operator*=(float f);
        //PxVec3& operator/=(float f);
        float dot(const PxVec3& v) const;
        PxVec3 cross(const PxVec3& v) const;
        PxVec3 getNormalized() const;
        float normalize();
        float normalizeSafe();
        float normalizeFast();
        PxVec3 multiply(const PxVec3& a) const;
        PxVec3 minimum(const PxVec3& v) const;
        float minElement() const;
        PxVec3 maximum(const PxVec3& v) const;
        float maxElement() const;
        PxVec3 abs() const;
    };

    class PxVec4 {
    public:
        //PxVec4(PxZERO r);
        //explicit PxVec4(float a);
        %extend { PxVec4(float s) { thread_local physx::PxVec4 v; v = physx::PxVec4(s); return &v; }}
        //PxVec4(float nx, float ny, float nz, float nw);
        %extend { PxVec4(float x, float y, float z, float w) { thread_local physx::PxVec4 v; v = physx::PxVec4(x, y, z, w); return &v; }}
        //PxVec4(const PxVec3& v, float nw);
        %extend { PxVec4(const PxVec3& xyz, float w) { thread_local physx::PxVec4 v; v = physx::PxVec4(xyz, w); return &v; }}
        //explicit PxVec4(const float v[]);
        PxVec4(const PxVec4& v);
        bool isZero() const;
        bool isFinite() const;
        bool isNormalized() const;
        float magnitudeSquared() const;
        float magnitude() const;
        float dot(const PxVec4& v) const;
        PxVec4 getNormalized() const;
        float normalize();
        PxVec4 multiply(const PxVec4& a) const;
        PxVec4 minimum(const PxVec4& v) const;
        PxVec4 maximum(const PxVec4& v) const;
        PxVec3 getXYZ() const;
        void setZero();
        //float x, y, z, w;
    };

    class PxQuat {
    public:
        //PxQuat(PxIDENTITY r);
        //explicit PxQuat(float r);
        %extend { PxQuat(float r) { thread_local physx::PxQuat q; q = physx::PxQuat(r); return &q; }}
        //PxQuat(float nx, float ny, float nz, float nw);
        %extend { PxQuat(float x, float y, float z, float w) { thread_local physx::PxQuat q; q = physx::PxQuat(x, y, z, w); return &q; }}
        //PxQuat(float angleRadians, const PxVec3& unitAxis);
        %extend { PxQuat(float angleRadians, const PxVec3& unitAxis) { thread_local physx::PxQuat q; q = physx::PxQuat(angleRadians, unitAxis); return &q; }}
        //PxQuat(const PxQuat& v);
        //explicit PxQuat(const PxMat33& m);
        %extend { PxQuat(const PxMat33& m) { thread_local physx::PxQuat q; q = physx::PxQuat(m); return &q; }}
        bool isIdentity() const;
        bool isFinite() const;
        bool isUnit() const;
        bool isSane() const;
        //void toRadiansAndUnitAxis(float& angle, PxVec3& axis) const;
        float getAngle() const;
        float getAngle(const PxQuat& q) const;
        float magnitudeSquared() const;
        float dot(const PxQuat& v) const;
        PxQuat getNormalized() const;
        float magnitude() const;
        float normalize();
        PxQuat getConjugate() const;
        PxVec3 getImaginaryPart() const;
        PxVec3 getBasisVector0() const;
        PxVec3 getBasisVector1() const;
        PxVec3 getBasisVector2() const;
        const PxVec3 rotate(const PxVec3& v) const;
        const PxVec3 rotateInv(const PxVec3& v) const;
        //float x, y, z, w;
    };

    class PxPlane {
    public:
        //PxPlane(float nx, float ny, float nz, float distance);
        %extend { PxPlane(float nx, float ny, float nz, float distance) { thread_local physx::PxPlane p; p = physx::PxPlane(nx, ny, nz, distance); return &p; }}
        //PxPlane(const PxVec3& normal, float distance);
        %extend { PxPlane(const PxVec3& normal, float distance) { thread_local physx::PxPlane p; p = physx::PxPlane(normal, distance); return &p; }}
        //PxPlane(const PxVec3& point, const PxVec3& normal);
        %extend { PxPlane(const PxVec3& point, const PxVec3& normal) { thread_local physx::PxPlane p; p = physx::PxPlane(point, normal); return &p; }}
        //PxPlane(const PxVec3& p0, const PxVec3& p1, const PxVec3& p2);
        %extend { PxPlane(const PxVec3& p0, const PxVec3& p1, const PxVec3& p2) { thread_local physx::PxPlane p; p = physx::PxPlane(p0, p1, p2); return &p; }}
        float distance(const PxVec3& p) const;
        bool contains(const PxVec3& p) const;
        PxVec3 project(const PxVec3& p) const;
        PxVec3 pointInPlane() const;
        void normalize();
        //PxVec3 n;
        //float d;
    };

    class PxTransform {
    public:
        //PxQuat q;
        //PxVec3 p;
        //explicit PxTransform(const PxVec3& position);
        %extend { PxTransform(const PxVec3& position) { thread_local physx::PxTransform t; t = physx::PxTransform(position); return &t; }}
        //explicit PxTransform(PxIDENTITY r);
        %extend { PxTransform(PxIDENTITY r) { thread_local physx::PxTransform t; t = physx::PxTransform(r); return &t; }}
        //explicit PxTransform(const PxQuat& orientation);
        %extend { PxTransform(const PxQuat& orientation) { thread_local physx::PxTransform t; t = physx::PxTransform(orientation); return &t; }}
        //PxTransform(float x, float y, float z, PxQuat aQ = PxQuat(PxIdentity));
        %extend { PxTransform(float x, float y, float z, PxQuat aQ = PxQuat(physx::PxIdentity)) { thread_local physx::PxTransform t; t = physx::PxTransform(x, y, z, aQ); return &t; }}
        //PxTransform(const PxVec3& p0, const PxQuat& q0);
        %extend { PxTransform(const PxVec3& p0, const PxQuat& q0) { thread_local physx::PxTransform t; t = physx::PxTransform(p0, q0); return &t; }}
        //explicit PxTransform(const PxMat44& m);
        %extend { PxTransform(const PxMat44& m) { thread_local physx::PxTransform t; t = physx::PxTransform(m); return &t; }}
        PxTransform getInverse() const;
        PxVec3 transform(const PxVec3& input) const;
        PxVec3 transformInv(const PxVec3& input) const;
        PxVec3 rotate(const PxVec3& input) const;
        PxVec3 rotateInv(const PxVec3& input) const;
        PxTransform transform(const PxTransform& src) const;
        bool isValid() const;
        bool isSane() const;
        bool isFinite() const;
        PxTransform transformInv(const PxTransform& src) const;
        PxPlane transform(const PxPlane& plane) const;
        PxPlane inverseTransform(const PxPlane& plane) const;
        PxTransform getNormalized() const;
    };

    class PxMat33 {
    public:
        //PxMat33(PxIDENTITY r);
        %extend { PxMat33(PxIDENTITY r) { thread_local physx::PxMat33 m; m = physx::PxMat33(r); return &m; }}
        //PxMat33(PxZERO r);
        %extend { PxMat33(PxZERO r) { thread_local physx::PxMat33 m; m = physx::PxMat33(r); return &m; }}
        //PxMat33(const PxVec3& col0, const PxVec3& col1, const PxVec3& col2);
        %extend { PxMat33(const PxVec3& col0, const PxVec3& col1, const PxVec3& col2) { thread_local physx::PxMat33 m; m = physx::PxMat33(col0, col1, col2); return &m; }}
        //explicit PxMat33(float r);
        %extend { PxMat33(float r) { thread_local physx::PxMat33 m; m = physx::PxMat33(r); return &m; }}
        //explicit PxMat33(float values[]);
        //explicit PxMat33(const PxQuat& q);
        %extend { PxMat33(const PxQuat& q) { thread_local physx::PxMat33 m; m = physx::PxMat33(q); return &m; }}
        //PxMat33(const PxMat33& other);
        static const PxMat33 createDiagonal(const PxVec3& d);
        const PxMat33 getTranspose() const;
        const PxMat33 getInverse() const;
        float getDeterminant() const;
        const PxVec3 transform(const PxVec3& other) const;
        const PxVec3 transformTranspose(const PxVec3& other) const;
        //const float* front() const;
        //PxVec3 column0, column1, column2;
    };

    class PxMat44 {
    public:
        //PxMat44(PxIDENTITY r);
        %extend { PxMat44(PxIDENTITY r) { thread_local physx::PxMat44 m; m = physx::PxMat44(r); return &m; }}
        //PxMat44(PxZERO r);
        %extend { PxMat44(PxZERO r) { thread_local physx::PxMat44 m; m = physx::PxMat44(r); return &m; }}
        //PxMat44(const PxVec4& col0, const PxVec4& col1, const PxVec4& col2, const PxVec4& col3);
        %extend { PxMat44(const PxVec4& col0, const PxVec4& col1, const PxVec4& col2, const PxVec4& col3) { thread_local physx::PxMat44 m; m = physx::PxMat44(col0, col1, col2, col3); return &m; }}
        //explicit PxMat44(float r);
        %extend { PxMat44(float r) { thread_local physx::PxMat44 m; m = physx::PxMat44(r); return &m; }}
        //PxMat44(const PxVec3& col0, const PxVec3& col1, const PxVec3& col2, const PxVec3& col3);
        %extend { PxMat44(const PxVec3& col0, const PxVec3& col1, const PxVec3& col2, const PxVec3& col3) { thread_local physx::PxMat44 m; m = physx::PxMat44(col0, col1, col2, col3); return &m; }}
        //explicit PxMat44(float values[]);
        //explicit PxMat44(const PxQuat& q);
        %extend { PxMat44(const PxQuat& q) { thread_local physx::PxMat44 m; m = physx::PxMat44(q); return &m; }}
        //explicit PxMat44(const PxVec4& diagonal);
        %extend { PxMat44(const PxVec4& diagonal) { thread_local physx::PxMat44 m; m = physx::PxMat44(diagonal); return &m; }}
        //PxMat44(const PxMat33& axes, const PxVec3& position);
        %extend { PxMat44(const PxMat33& axes, const PxVec3& position) { thread_local physx::PxMat44 m; m = physx::PxMat44(axes, position); return &m; }}
        //PxMat44(const PxTransform& t);
        %extend { PxMat44(const PxTransform& t) { thread_local physx::PxMat44 m; m = physx::PxMat44(t); return &m; }}
        //PxMat44(const PxMat44& other);
        const PxMat44 getTranspose() const;
        const PxVec4 transform(const PxVec4& other) const;
        const PxVec3 transform(const PxVec3& other) const;
        const PxVec4 rotate(const PxVec4& other) const;
        const PxVec3 rotate(const PxVec3& other) const;
        const PxVec3 getBasis(int num) const;
        const PxVec3 getPosition() const;
        void setPosition(const PxVec3& position);
        //const float* front() const;
        void scale(const PxVec4& p);
        const PxMat44 inverseRT(void) const;
        bool isFinite() const;
        //PxVec4 column0, column1, column2, column3;
    };

    class PxBounds3 {
    public:
        //PxBounds3(const PxVec3& minimum, const PxVec3& maximum);
        %extend { PxBounds3(const PxVec3& minimum, const PxVec3& maximum) { thread_local physx::PxBounds3 b; b = physx::PxBounds3(minimum, maximum); return &b; }}
        static PxBounds3 empty();
        static PxBounds3 boundsOfPoints(const PxVec3& v0, const PxVec3& v1);
        static PxBounds3 centerExtents(const PxVec3& center, const PxVec3& extent);
        static PxBounds3 basisExtent(const PxVec3& center, const PxMat33& basis, const PxVec3& extent);
        static PxBounds3 poseExtent(const PxTransform& pose, const PxVec3& extent);
        static PxBounds3 transformSafe(const PxMat33& matrix, const PxBounds3& bounds);
        static  PxBounds3 transformFast(const PxMat33& matrix, const PxBounds3& bounds);
        static  PxBounds3 transformSafe(const PxTransform& transform, const PxBounds3& bounds);
        static  PxBounds3 transformFast(const PxTransform& transform, const PxBounds3& bounds);
        void setMaximal();
        void include(const PxVec3& v);
        void include(const PxBounds3& b);
        bool isEmpty() const;
        bool intersects(const PxBounds3& b) const;
        bool intersects1D(const PxBounds3& a, uint32_t axis) const;
        bool contains(const PxVec3& v) const;
        bool isInside(const PxBounds3& box) const;
        PxVec3 getCenter() const;
        float getCenter(uint32_t axis) const;
        float getExtents(uint32_t axis) const;
        PxVec3 getDimensions() const;
        PxVec3 getExtents() const;
        void scaleSafe(float scale);
        void scaleFast(float scale);
        void fattenSafe(float distance);
        void fattenFast(float distance);
        bool isFinite() const;
        bool isValid() const;
        //PxVec3 minimum, maximum;
    };

    class PxMassProperties {
    public:
        //PxMassProperties();
        //PxMassProperties(const PxReal m, const PxMat33& inertiaT, const PxVec3& com);
        %extend { PxMassProperties(const PxReal m, const PxMat33& inertiaT, const PxVec3& com) { thread_local physx::PxMassProperties mp; mp = physx::PxMassProperties(m, inertiaT, com); return &mp; }}
        //PxMassProperties(const PxGeometry& geometry);
        %extend { PxMassProperties(const PxGeometry& geometry) { thread_local physx::PxMassProperties mp; mp = physx::PxMassProperties(geometry); return &mp; }}
        PxMassProperties operator*(const PxReal scale) const;
        void translate(const PxVec3& t);
        static PxVec3 getMassSpaceInertia(const PxMat33& inertia, PxQuat& massFrame);
        static PxMat33 translateInertia(const PxMat33& inertia, const PxReal mass, const PxVec3& t);
        static PxMat33 rotateInertia(const PxMat33& inertia, const PxQuat& q);
        static PxMat33 scaleInertia(const PxMat33& inertia, const PxQuat& scaleRotation, const PxVec3& scale);
        static PxMassProperties sum(const PxMassProperties props[], const PxTransform transforms[], const PxU32 count);
        //PxMat33 inertiaTensor;
        //PxVec3 centerOfMass;
        //PxReal mass;
    };

    class PxTolerancesScale {
    public:
        PxReal length;
        PxReal speed;
        PxTolerancesScale();
        bool isValid() const;
    };

    class PxSceneLimits {
    public:
        PxU32 maxNbActors;
        PxU32 maxNbBodies;
        PxU32 maxNbStaticShapes;
        PxU32 maxNbDynamicShapes;
        PxU32 maxNbAggregates;
        PxU32 maxNbConstraints;
        PxU32 maxNbRegions;
        PxU32 maxNbBroadPhaseOverlaps;
        void setToDefault();
        bool isValid() const;
    };

    struct PxConstraintInfo {
        //PxConstraintInfo();
        //PxConstraintInfo(PxConstraint* c, void* extRef, PxU32 t);
        PxConstraint* const constraint;
        //%apply void* VOID_INT_PTR { void* externalReference }
        //void* const externalReference;
        PxU32 const type;
        %extend { physx::PxJoint* const joint; %{ physx::PxJoint* physx_PxConstraintInfo_joint_get(const physx::PxConstraintInfo* info) { return (physx::PxJoint*)info->externalReference; }%}}
    };

    struct PxContactPairHeader {
    public:
        //PxContactPairHeader() {}
        //PxRigidActor* actors[2];
        %extend { physx::PxRigidActor* const actor0; %{ physx::PxRigidActor* physx_PxContactPairHeader_actor0_get(const physx::PxContactPairHeader* header) { return header->actors[0]; }%} }
        %extend { physx::PxRigidActor* const actor1; %{ physx::PxRigidActor* physx_PxContactPairHeader_actor1_get(const physx::PxContactPairHeader* header) { return header->actors[1]; }%} }
        //const PxU8* extraDataStream;
        //PxU16 extraDataStreamSize;
        //PxContactPairHeaderFlags flags;
        %extend { const physx::PxContactPairHeaderFlag::Enum flags; %{ physx::PxContactPairHeaderFlag::Enum physx_PxContactPairHeader_flags_get(const physx::PxContactPairHeader* header) { return (physx::PxContactPairHeaderFlag::Enum)(uint32_t)header->flags; }%} }
        //const struct PxContactPair* pairs;
        //PxU32 nbPairs;
    };

    struct PxContactPairPoint {
        //PxVec3 position;
        //PxReal separation;
        //PxVec3 normal;
        //PxU32 internalFaceIndex0;
        //PxVec3 impulse;
        //PxU32 internalFaceIndex1;
    };

    CSHARP_BYREF_ARRAY(physx::PxContactPairPoint, PxContactPairPoint)
    struct PxContactPair {
    public:
        //PxContactPair() {}
        //PxShape* shapes[2];
        %extend { physx::PxShape* const shape0; %{ physx::PxShape* physx_PxContactPair_shape0_get(const physx::PxContactPair* pair) { return pair->shapes[0]; }%} }
        %extend { physx::PxShape* const shape1; %{ physx::PxShape* physx_PxContactPair_shape1_get(const physx::PxContactPair* pair) { return pair->shapes[1]; }%} }
        //const PxU8* contactPatches;
        //const PxU8* contactPoints;
        //const PxReal* contactImpulses;
        //PxU32 requiredBufferSize;
        const PxU8 contactCount;
        //PxU8 patchCount;
        //PxU16 contactStreamSize;
        //PxContactPairFlags flags;
        %extend { const physx::PxContactPairFlag::Enum flags; %{ physx::PxContactPairFlag::Enum physx_PxContactPair_flags_get(const physx::PxContactPair* pair) { return (physx::PxContactPairFlag::Enum)(uint32_t)pair->flags; }%} }
        //PxPairFlags events;
        %extend { const physx::PxPairFlag::Enum events; %{ physx::PxPairFlag::Enum physx_PxContactPair_events_get(const physx::PxContactPair* pair) { return (physx::PxPairFlag::Enum)(uint32_t)pair->events; }%} }
        ////PxU32 internalData[2];    // For internal use only
        %apply PxContactPairPoint BYREF[] { PxContactPairPoint userBuffer[] }
        PxU32 extractContacts(PxContactPairPoint userBuffer[], PxU32 bufferSize) const;
        //void bufferContacts(PxContactPair* newPair, PxU8* bufferMemory) const;
        //const PxU32* getInternalFaceIndices() const;
    };

    struct PxTriggerPair {
        //PxTriggerPair() {}
        PxShape* const triggerShape;
        PxRigidActor* const triggerActor;
        PxShape* const otherShape;
        PxRigidActor* const otherActor;
        PxPairFlag::Enum const status;
        //PxTriggerPairFlags flags;
    };

    class PxSimulationFilterShader {};

    class PxGroupsMask {
    public:
        //PxGroupsMask();
        //~PxGroupsMask();
        //PxU16 bits0, bits1, bits2, bits3;
    };

    OUTPUT_TYPEMAP(physx::PxFilterOp::Enum, int, PxFilterOp, INT32_PTR)
    class PxDefaultSimulationFilterShader {
    public:
        %extend { static physx::PxSimulationFilterShader* const function; %{
            physx::PxSimulationFilterShader* physx_PxDefaultSimulationFilterShader_function_get() { thread_local physx::PxSimulationFilterShader s; s = physx::PxCustomSimulationFilterShader; return &s; }
        %}}
        %extend { static bool getGroupCollisionFlag(const PxU16 group1, const PxU16 group2) { return physx::PxGetGroupCollisionFlag(group1, group2); }}
        %extend { static void setGroupCollisionFlag(const PxU16 group1, const PxU16 group2, const bool enable) { physx::PxSetGroupCollisionFlag(group1, group2, enable); }}
        %extend { static PxU16 getGroup(const PxActor& actor) { return physx::PxGetGroup(actor); }}
        %extend { static void setGroup(PxActor& actor, const PxU16 collisionGroup) { physx::PxSetGroup(actor, collisionGroup); }}
        %apply PxFilterOp::Enum& OUTPUT { physx::PxFilterOp::Enum& op0, physx::PxFilterOp::Enum& op1, physx::PxFilterOp::Enum& op2 }
        %extend { static void getFilterOps(physx::PxFilterOp::Enum& op0, physx::PxFilterOp::Enum& op1, physx::PxFilterOp::Enum& op2) { physx::PxGetFilterOps(op0, op1, op2); }}
        %clear physx::PxFilterOp::Enum& op0, physx::PxFilterOp::Enum& op1, physx::PxFilterOp::Enum& op2;
        %extend { static void setFilterOps(PxFilterOp::Enum op0, PxFilterOp::Enum op1, PxFilterOp::Enum op2) { physx::PxSetFilterOps(op0, op1, op2); }}
        %extend { static bool getFilterBool() { return physx::PxGetFilterBool(); }}
        %extend { static void setFilterBool(const bool enable) { physx::PxSetFilterBool(enable); }}
        %apply physx::PxGroupsMask& OUTPUT { PxGroupsMask& c0, PxGroupsMask& c1 }
        %extend { static void getFilterConstants(PxGroupsMask& c0, PxGroupsMask& c1) { physx::PxGetFilterConstants(c0, c1); }}
        %clear PxGroupsMask& c0, PxGroupsMask& c1;
        %extend { static void setFilterConstants(const PxGroupsMask& c0, const PxGroupsMask& c1) { physx::PxSetFilterConstants(c0, c1); }}
        %extend { static PxGroupsMask getGroupsMask(const PxActor& actor) { return physx::PxGetGroupsMask(actor); }}
        %extend { static void setGroupsMask(PxActor& actor, const PxGroupsMask& mask) { physx::PxSetGroupsMask(actor, mask); }}
        %extend { static void setCallbacksEnabled(PxActor& actor, bool yes) { physx::PxSetCallbacksEnabled(actor, yes); }}
        %extend { static bool getCallbacksEnabled(PxActor& actor) { return physx::PxGetCallbacksEnabled(actor); }}
    private:
        ~PxDefaultSimulationFilterShader();
    };

    template<typename _Type> struct PxList {
        //const _Type* array;
        const PxU32 count;
        //PxList(const _Type* array, PxU32 count);
        const _Type& get(PxU32 index) const;
    };

    %template(PxConstraintInfoList) PxList<PxConstraintInfo>;
    using PxConstraintInfoList = PxList<PxConstraintInfo>;

    %template(PxActorList) PxList<PxActor*>;
    using PxActorList = PxList<PxActor*>;

    %template(PxContactPairList) PxList<PxContactPair>;
    using PxContactPairList = PxList<PxContactPair>;

    %template(PxTriggerPairList) PxList<PxTriggerPair>;
    using PxTriggerPairList = PxList<PxTriggerPair>;

    %template(PxRigidBodyList) PxList<const PxRigidBody*>;
    using PxRigidBodyList = PxList<const PxRigidBody*>;

    %template(PxTransformList) PxList<PxTransform>;
    using PxTransformList = PxList<PxTransform>;

    struct PxFilterData {
        PxFilterData(const PxEMPTY);
        //PxFilterData();
        PxFilterData(const PxFilterData& fd);
        PxFilterData(PxU32 w0, PxU32 w1, PxU32 w2, PxU32 w3);
        void setToDefault();
        //void operator = (const PxFilterData& fd);
        //bool operator == (const PxFilterData& a) const;
        //bool operator != (const PxFilterData& a) const;
        //PxU32 word0;
        //PxU32 word1;
        //PxU32 word2;
        //PxU32 word3;
    };

    namespace wrap {
        %feature("director") PxSimulationEventCallback;
        class PxSimulationEventCallback {
        public:
            PxSimulationEventCallback();
            virtual void onConstraintBreak(const PxConstraintInfoList& constraints) {}
            virtual void onWake(const PxActorList& actors) {}
            virtual void onSleep(const PxActorList& actors) {}
            virtual void onContact(const PxContactPairHeader& pairHeader, const PxContactPairList& pairs) {}
            virtual void onTrigger(const PxTriggerPairList& pairs) {}
            virtual void onAdvance(const PxRigidBodyList& bodyBuffer, const PxTransformList& poseBuffer) {}
            virtual ~PxSimulationEventCallback() {}
        };

        // @@@ Fix this
        %feature("director") PxSimulationFilterCallback;
        class PxSimulationFilterCallback {
        public:
            PxSimulationFilterCallback();
            //virtual PxFilterFlag::Enum pairFound(PxU32 pairID, const PxFilterObjectAttributes& attributes0, const PxFilterData& filterData0, const PxActor* a0, const PxShape* s0, const PxFilterObjectAttributes& attributes1, const PxFilterData& filterData1, const PxActor* a1, const PxShape* s1, PxPairFlag::Enum& pairFlags) {}
            virtual void pairLost(PxU32 pairID, bool objectRemoved, PxFilterObjectAttributes attributes0, const PxFilterData& filterData0, PxFilterObjectAttributes attributes1, const PxFilterData& filterData1) {}
            //virtual bool statusChange(PxU32& pairID, PxPairFlag::Enum& pairFlags, PxFilterFlag::Enum& filterFlags) {}
            virtual ~PxSimulationFilterCallback();
        };
    }

    %feature("director") PxBroadPhaseCallback;
    class PxBroadPhaseCallback {
    public:
        virtual ~PxBroadPhaseCallback() {}
        virtual void onObjectOutOfBounds(PxShape& shape, PxActor& actor) = 0;
        virtual void onObjectOutOfBounds(PxAggregate& aggregate) = 0;
    };

    class PxSceneDesc {
    public:
        PxVec3 gravity;
        wrap::PxSimulationEventCallback* simulationEventCallback;
        //PxContactModifyCallback* contactModifyCallback;
        //PxCCDContactModifyCallback* ccdContactModifyCallback;
        %apply void* VOID_INT_PTR { const void* filterShaderData }
        const void* filterShaderData;
        PxU32 filterShaderDataSize;
        PxSimulationFilterShader filterShader;
        wrap::PxSimulationFilterCallback* filterCallback;
        PxPairFilteringMode::Enum kineKineFilteringMode;
        PxPairFilteringMode::Enum staticKineFilteringMode;
        PxBroadPhaseType::Enum broadPhaseType;
        PxBroadPhaseCallback* broadPhaseCallback;
        PxSceneLimits limits;
        PxFrictionType::Enum frictionType;
        PxSolverType::Enum solverType;
        PxReal bounceThresholdVelocity; 
        PxReal frictionOffsetThreshold;
        PxReal ccdMaxSeparation;
        PxReal solverOffsetSlop;
        //PxSceneFlags flags;
        %extend { PxSceneFlag::Enum flags; %{
            physx::PxSceneFlag::Enum physx_PxSceneDesc_flags_get(const physx::PxSceneDesc* desc) { return (physx::PxSceneFlag::Enum)(uint32_t)desc->flags; }
            void physx_PxSceneDesc_flags_set(physx::PxSceneDesc* desc, physx::PxSceneFlag::Enum flags) { desc->flags.set(flags); }
        %}}
        PxCpuDispatcher* cpuDispatcher;
        PxCudaContextManager* cudaContextManager;
        PxPruningStructureType::Enum staticStructure;
        PxPruningStructureType::Enum dynamicStructure;
        PxU32 dynamicTreeRebuildRateHint;
        PxSceneQueryUpdateMode::Enum sceneQueryUpdateMode;
        //%apply void* VOID_INT_PTR { void* userData }
        //void* userData;
        PxU32 solverBatchSize;
        PxU32 solverArticulationBatchSize;
        PxU32 nbContactDataBlocks;
        PxU32 maxNbContactDataBlocks;
        PxReal maxBiasCoefficient;
        PxU32 contactReportStreamBufferSize;
        PxU32 ccdMaxPasses;
        PxReal ccdThreshold;
        PxReal wakeCounterResetValue;
        PxBounds3 sanityBounds;
        //PxgDynamicsMemoryConfig gpuDynamicsConfig;
        PxU32 gpuMaxNumPartitions;
        PxU32 gpuComputeVersion;
        PxSceneDesc(const PxTolerancesScale& scale);
        void setToDefault(const PxTolerancesScale& scale);
        bool isValid() const;
    };

    class PxInputStream {
    public:
        %apply void *VOID_INT_PTR { void* dest }
        virtual uint32_t read(void* dest, uint32_t count) = 0;
        virtual ~PxInputStream() { }
    };

    class PxInputData : public PxInputStream {
    public:
        virtual uint32_t getLength() const = 0;
        virtual void seek(uint32_t offset) = 0;
        virtual uint32_t tell() const = 0;
        virtual ~PxInputData() { }
    };

    class PxDefaultMemoryInputData : public PxInputData {
    public:
        %apply void *VOID_INT_PTR { void* data }
        %extend { PxDefaultMemoryInputData(void* data, PxU32 length) { return new physx::PxDefaultMemoryInputData((physx::PxU8*)data, length); }}
        virtual ~PxDefaultMemoryInputData();
        %apply void *VOID_INT_PTR { void* dest }
        virtual PxU32 read(void* dest, PxU32 count);
        virtual PxU32 getLength() const;
        virtual void seek(PxU32 pos);
        virtual PxU32 tell() const;
    };

    class PxMeshScale {
    public:
        PxMeshScale();
        explicit PxMeshScale(PxReal r);
        PxMeshScale(const PxVec3& s);
        PxMeshScale(const PxVec3& s, const PxQuat& r);
        bool isIdentity() const;
        PxMeshScale getInverse() const;
        PxMat33 toMat33() const;
        bool hasNegativeDeterminant() const;
        PxVec3 transform(const PxVec3& v) const;
        bool isValidForTriangleMesh() const;
        bool isValidForConvexMesh() const;
        PxVec3 scale;
        PxQuat rotation;
    };

    struct PxStridedData {
        PxU32 stride;
        %apply void *VOID_INT_PTR { const PxU8* data }
        const PxU8* data;
        %clear const PxU8* data;
        PxStridedData();
        //template<typename TDataType>
        //PX_INLINE const TDataType& at( PxU32 idx ) const
        //{
        //    PxU32 theStride( stride );
        //    if ( theStride == 0 )
        //        theStride = sizeof( TDataType );
        //    PxU32 offset( theStride * idx );
        //    return *(reinterpret_cast<const TDataType*>( reinterpret_cast< const PxU8* >( data ) + offset ));
        //}
    };

    class PxHeightFieldDesc {
    public:
        PxU32 nbRows;
        PxU32 nbColumns;
        PxHeightFieldFormat::Enum format;
        PxStridedData samples;
        PxReal convexEdgeThreshold;
        %extend { PxHeightFieldFlag::Enum flags; %{
            physx::PxHeightFieldFlag::Enum physx_PxHeightFieldDesc_flags_get(const physx::PxHeightFieldDesc* desc) { return (physx::PxHeightFieldFlag::Enum)(uint32_t)desc->flags; }
            void physx_PxHeightFieldDesc_flags_set(physx::PxHeightFieldDesc* desc, physx::PxHeightFieldFlag::Enum flags) { desc->flags.set(flags); }
        %}}
        PxHeightFieldDesc();
        void setToDefault();
        bool isValid() const;
    };

    struct PxHeightFieldSample {
        //PxI16 height;
        //PxU8 materialIndex0;
        PxU8 tessFlag() const;
        void setTessFlag();
        void clearTessFlag();
        //PxU8 materialIndex1;
    };

    struct PxHullPolygon {
        //PxReal mPlane[4];
        //PxU16 mNbVerts;
        //PxU16 mIndexBase;
    };

    class PxGeometry { 
    public:
        PxGeometryType::Enum getType() const;
    protected:
        PxGeometry(PxGeometryType::Enum type);
    };

    class PxBoxGeometry : public PxGeometry {
    public:
        PxBoxGeometry();
        PxBoxGeometry(PxReal hx, PxReal hy, PxReal hz);
        PxBoxGeometry(PxVec3 halfExtents);
        bool isValid() const;
        PxVec3 halfExtents;
    };

    class PxSphereGeometry : public PxGeometry {
    public:
        PxSphereGeometry();
        PxSphereGeometry(PxReal ir);
        bool isValid() const;
        PxReal radius;
    };

    class PxCapsuleGeometry : public PxGeometry {
    public:
        PxCapsuleGeometry();
        PxCapsuleGeometry(PxReal radius, PxReal halfHeight);
        bool isValid() const;
        PxReal radius;
        PxReal halfHeight;
    };

    class PxPlaneGeometry : public PxGeometry {
    public:
        PxPlaneGeometry();
        bool isValid() const;
    };

    class PxConvexMeshGeometry : public PxGeometry {
    public:
        PxConvexMeshGeometry();
        PxConvexMeshGeometry(PxConvexMesh* mesh, const PxMeshScale& scaling = PxMeshScale());
        bool isValid() const;
        PxMeshScale scale;
        PxConvexMesh* convexMesh;
        %extend { PxConvexMeshGeometryFlag::Enum meshFlags; %{
            physx::PxConvexMeshGeometryFlag::Enum physx_PxConvexMeshGeometry_meshFlags_get(const physx::PxConvexMeshGeometry* geom) { return (physx::PxConvexMeshGeometryFlag::Enum)(uint32_t)geom->meshFlags; }
            void physx_PxConvexMeshGeometry_meshFlags_set(physx::PxConvexMeshGeometry* geom, physx::PxConvexMeshGeometryFlag::Enum flags) { geom->meshFlags.set(flags); }
        %}}
    };

    class PxTriangleMeshGeometry : public PxGeometry {
    public:
        PxTriangleMeshGeometry();
        PxTriangleMeshGeometry(PxTriangleMesh* mesh, const PxMeshScale& scaling = PxMeshScale());
        bool isValid() const;
        PxMeshScale scale;
        %extend { PxMeshGeometryFlag::Enum meshFlags; %{
            physx::PxMeshGeometryFlag::Enum physx_PxTriangleMeshGeometry_meshFlags_get(const physx::PxTriangleMeshGeometry* geom) { return (physx::PxMeshGeometryFlag::Enum)(uint32_t)geom->meshFlags; }
            void physx_PxTriangleMeshGeometry_meshFlags_set(physx::PxTriangleMeshGeometry* geom, physx::PxMeshGeometryFlag::Enum flags) { geom->meshFlags.set(flags); }
        %}}
        PxTriangleMesh* triangleMesh;
    };

    class PxHeightFieldGeometry : public PxGeometry {
    public:
        PxHeightFieldGeometry();
        %extend { PxHeightFieldGeometry(physx::PxHeightField* hf, physx::PxReal heightScale, physx::PxReal rowScale, physx::PxReal columnScale) { return new physx::PxHeightFieldGeometry(hf, (physx::PxMeshGeometryFlag::Enum)0, heightScale, rowScale, columnScale); }}
        %extend { PxHeightFieldGeometry(physx::PxHeightField* hf, physx::PxReal heightScale, physx::PxReal rowScale, physx::PxReal columnScale, physx::PxMeshGeometryFlag::Enum flags) { return new physx::PxHeightFieldGeometry(hf, flags, heightScale, rowScale, columnScale); }}
        bool isValid() const;
        PxHeightField* heightField;
        PxReal heightScale;
        PxReal rowScale;
        PxReal columnScale;
        %extend { PxMeshGeometryFlag::Enum heightFieldFlags; %{
            physx::PxMeshGeometryFlag::Enum physx_PxHeightFieldGeometry_heightFieldFlags_get(const physx::PxHeightFieldGeometry* geom) { return (physx::PxMeshGeometryFlag::Enum)(uint32_t)geom->heightFieldFlags; }
            void physx_PxHeightFieldGeometry_heightFieldFlags_set(physx::PxHeightFieldGeometry* geom, physx::PxMeshGeometryFlag::Enum flags) { geom->heightFieldFlags.set(flags); }
        %}}
    };

    class PxGeometryHolder {
    public:
        PxGeometryType::Enum getType() const;
        PxGeometry& any();
        PxSphereGeometry& sphere();
        PxPlaneGeometry& plane();
        PxCapsuleGeometry& capsule();
        PxBoxGeometry& box();
        PxConvexMeshGeometry& convexMesh();
        PxTriangleMeshGeometry& triangleMesh();
        PxHeightFieldGeometry& heightField();
        void storeAny(const PxGeometry& geometry);
        PxGeometryHolder();
        PxGeometryHolder(const PxGeometry& geometry);
    };

    class PxConstraintConnector {
    public:
        //virtual void* prepareData();
        //virtual bool updatePvdProperties(physx::pvdsdk::PvdDataStream& pvdConnection, const PxConstraint* c, PxPvdUpdateType::Enum updateType) const;
        virtual void onConstraintRelease();
        virtual void onComShift(PxU32 actor);
        virtual void onOriginShift(const PxVec3& shift);
        //virtual void* getExternalReference(PxU32& typeID);
        virtual PxBase* getSerializable();
        //virtual PxConstraintSolverPrep getPrep() const;
        //virtual const void* getConstantBlock() const;
        virtual ~PxConstraintConnector() {}
    };

    struct Px1DConstraint {
        PxVec3 linear0;
        PxReal geometricError;
        PxVec3 angular0;
        PxReal velocityTarget;
        PxVec3 linear1;
        PxReal minImpulse;
        PxVec3 angular1;
        PxReal maxImpulse;
        union {
            struct SpringModifiers {
                PxReal stiffness;
                PxReal damping;
            } spring;
            struct RestitutionModifiers {
                PxReal restitution;
                PxReal velocityThreshold;
            } bounce;
        } mods;
        PxReal forInternalUse;
        PxU16 flags;
        PxU16 solveHint;
    };

    struct PxConstraintInvMassScale {
        PxReal linear0;
        PxReal angular0;
        PxReal linear1;
        PxReal angular1;
        PxConstraintInvMassScale();
        PxConstraintInvMassScale(PxReal lin0, PxReal ang0, PxReal lin1, PxReal ang1);
    };

    struct PxConstraintShaderTable {
        enum { eMAX_SOLVERPRPEP_DATASIZE=400 };
        //PxConstraintSolverPrep solverPrep;
        //PxConstraintProject project;
        //PxConstraintVisualize visualize;
        PxConstraintFlag::Enum flag;
    };

    FLAT_STRUCT(physx::PxSpatialForce, PxSpatialForce, public PxVec3 force, torque; )
    struct PxSpatialForce {
        //PxVec3 force;
        //PxVec3 torque;
    };

    FLAT_STRUCT(physx::PxArticulationRootLinkData, PxArticulationRootLinkData, public PxTransform transform; public PxVec3 worldLinVel, worldAngVel, worldLinAccel, worldAngAccel; )
    struct PxArticulationRootLinkData {
        //PxTransform transform;
        //PxVec3 worldLinVel;
        //PxVec3 worldAngVel;
        //PxVec3 worldLinAccel;
        //PxVec3 worldAngAccel;
    };

    class PxArticulationCache {
    public:
        %typemap(csattributes) Enum "[global::System.FlagsAttribute()]"
        enum struct Flags {
            eVELOCITY = (1 << 0),
            eACCELERATION = (1 << 1),
            ePOSITION = (1 << 2),
            eFORCE = (1 << 3),
            eLINKVELOCITY = (1 << 4),
            eLINKACCELERATION = (1 << 5),
            eROOT = (1 << 6),
            eALL = (1<<0)|(1<<1)|(1<<2)|(1<<4)|(1<<5)|(1<<6) //(eVELOCITY | eACCELERATION | ePOSITION| eLINKVELOCITY | eLINKACCELERATION | eROOT )
        };
        using Enum = Flags;
        //PxArticulationCache();
        //PxSpatialForce* externalForces;
        //PxReal* denseJacobian;
        //PxReal* massMatrix;
        //PxReal* jointVelocity;
        %apply float OUTPUT[] { PxReal velocities[] }
        %extend { void readJointVelocities(PxReal velocities[], int start, int count) { memcpy(velocities, self->jointVelocity + start, sizeof(physx::PxReal) * count); }}
        %apply float INPUT[] { const PxReal velocities[] }
        %extend { void writeJointVelocities(const PxReal velocities[], int start, int count) { memcpy(self->jointVelocity + start, velocities, sizeof(physx::PxReal) * count); }}
        //PxReal* jointAcceleration;
        //PxReal* jointPosition;
        %apply float OUTPUT[] { PxReal positions[] }
        %extend { void readJointPositions(PxReal positions[], int start, int count) { memcpy(positions, self->jointPosition + start, sizeof(physx::PxReal) * count); }}
        %apply float INPUT[] { const PxReal positions[] }
        %extend { void writeJointPositions(const PxReal positions[], int start, int count) { memcpy(self->jointPosition + start, positions, sizeof(physx::PxReal) * count); }}
        //PxReal* jointForce;
        //PxArticulationRootLinkData* rootLinkData;
        %extend { PxArticulationRootLinkData& rootLinkData; %{
            const physx::PxArticulationRootLinkData& physx_PxArticulationCache_rootLinkData_get(physx::PxArticulationCache* self) { return self->rootLinkData[0]; }
            void physx_PxArticulationCache_rootLinkData_set(physx::PxArticulationCache* self, const physx::PxArticulationRootLinkData& value) { self->rootLinkData[0] = value; }
        %}}
        //PxReal* coefficientMatrix; 
        //PxReal* lambda;
        //void* scratchMemory;
        //void* scratchAllocator;
        PxU32 version;
    };

    class PxSpring {
    public:
        PxReal stiffness;
        PxReal damping;
        PxSpring(PxReal stiffness_, PxReal damping_);
    };

    class PxD6JointDrive : public PxSpring {
    public:
        PxReal forceLimit;
        //PxD6JointDriveFlags flags;
        %extend { PxD6JointDriveFlag::Enum flags; %{
            physx::PxD6JointDriveFlag::Enum physx_PxD6JointDrive_flags_get(const physx::PxD6JointDrive* drive) { return (physx::PxD6JointDriveFlag::Enum)(uint32_t)drive->flags; }
            void physx_PxD6JointDrive_flags_set(physx::PxD6JointDrive* drive, physx::PxD6JointDriveFlag::Enum flags) { drive->flags.set(flags); }
        %}}
        PxD6JointDrive();
        PxD6JointDrive(PxReal driveStiffness, PxReal driveDamping, PxReal driveForceLimit, bool isAcceleration = false);
        bool isValid() ;
    };

    class PxJointLimitParameters {
    public:
        PxReal restitution;
        PxReal bounceThreshold;
        PxReal stiffness;
        PxReal damping;
        PxReal contactDistance;
        PxJointLimitParameters();
        PxJointLimitParameters(const PxJointLimitParameters& p);
        bool isValid() const;
        bool isSoft();
    protected:
        ~PxJointLimitParameters() {}
    };

    class PxJointLinearLimit : public PxJointLimitParameters {
    public:
        PxReal value;
        PxJointLinearLimit(const PxTolerancesScale& scale, PxReal extent, PxReal contactDist = -1.0f);
        PxJointLinearLimit(PxReal extent, const PxSpring& spring);
        bool isValid() const;
    };

    class PxJointLinearLimitPair : public PxJointLimitParameters {
    public:
        PxReal upper, lower;
        PxJointLinearLimitPair(const PxTolerancesScale& scale, PxReal lowerLimit = -PX_MAX_F32/3.0f, PxReal upperLimit = PX_MAX_F32/3.0f, PxReal contactDist = -1.0f);
        PxJointLinearLimitPair(PxReal lowerLimit, PxReal upperLimit, const PxSpring& spring);
        bool isValid() const;
    };

    class PxJointAngularLimitPair : public PxJointLimitParameters {
    public:
        PxReal upper, lower;
        PxJointAngularLimitPair(PxReal lowerLimit, PxReal upperLimit, PxReal contactDist = -1.0f);
        PxJointAngularLimitPair(PxReal lowerLimit, PxReal upperLimit, const PxSpring& spring);
        bool isValid() const;
    };

    class PxJointLimitCone : public PxJointLimitParameters {
    public:
        PxReal yAngle;
        PxReal zAngle;
        PxJointLimitCone(PxReal yLimitAngle, PxReal zLimitAngle, PxReal contactDist = -1.0f);
        PxJointLimitCone(PxReal yLimitAngle, PxReal zLimitAngle, const PxSpring& spring);
        bool isValid() const;
    };

    class PxJointLimitPyramid : public PxJointLimitParameters {
    public:
        PxReal yAngleMin;
        PxReal yAngleMax;
        PxReal zAngleMin;
        PxReal zAngleMax;
        PxJointLimitPyramid(PxReal yLimitAngleMin, PxReal yLimitAngleMax, PxReal zLimitAngleMin, PxReal zLimitAngleMax, PxReal contactDist = -1.0f);
        PxJointLimitPyramid(PxReal yLimitAngleMin, PxReal yLimitAngleMax, PxReal zLimitAngleMin, PxReal zLimitAngleMax, const PxSpring& spring);
        bool isValid() const;
    };

    class PxArticulationDriveCache {
    protected:
        PxArticulationDriveCache();
    };

    struct PxDominanceGroupPair {
        PxDominanceGroupPair(PxU8 a, PxU8 b);
        PxU8 dominance0;
        PxU8 dominance1;
    };

    class PxSimulationStatistics {
    public:
        enum RbPairStatsType {
            eDISCRETE_CONTACT_PAIRS,
            eCCD_PAIRS,
            eMODIFIED_CONTACT_PAIRS,
            eTRIGGER_PAIRS
        };
        PxU32 nbActiveConstraints;
        PxU32 nbActiveDynamicBodies;
        PxU32 nbActiveKinematicBodies;
        PxU32 nbStaticBodies;
        PxU32 nbDynamicBodies;
        PxU32 nbKinematicBodies;
        //PxU32 nbShapes[PxGeometryType::eGEOMETRY_COUNT];
        PxU32 nbAggregates;
        PxU32 nbArticulations;
        PxU32 nbAxisSolverConstraints;
        PxU32 compressedContactSize;
        PxU32 requiredContactConstraintMemory;
        PxU32 peakConstraintMemory;
        PxU32 getNbBroadPhaseAdds() const;
        PxU32 getNbBroadPhaseRemoves() const;
        PxU32 getRbPairStats(RbPairStatsType pairType, PxGeometryType::Enum g0, PxGeometryType::Enum g1) const;
        PxU32 nbDiscreteContactPairsTotal;
        PxU32 nbDiscreteContactPairsWithCacheHits;
        PxU32 nbDiscreteContactPairsWithContacts;
        PxU32 nbNewPairs;
        PxU32 nbLostPairs;
        PxU32 nbNewTouches;
        PxU32 nbLostTouches;
        PxU32 nbPartitions;
        PxSimulationStatistics();
        //PxU32    nbBroadPhaseAdds;
        //PxU32    nbBroadPhaseRemoves;
        //PxU32   nbDiscreteContactPairs[PxGeometryType::eGEOMETRY_COUNT][PxGeometryType::eGEOMETRY_COUNT];
        //PxU32   nbCCDPairs[PxGeometryType::eGEOMETRY_COUNT][PxGeometryType::eGEOMETRY_COUNT];
        //PxU32   nbModifiedContactPairs[PxGeometryType::eGEOMETRY_COUNT][PxGeometryType::eGEOMETRY_COUNT];
        //PxU32   nbTriggerPairs[PxGeometryType::eGEOMETRY_COUNT][PxGeometryType::eGEOMETRY_COUNT];
    };

    struct PxBroadPhaseRegion {
        PxBounds3 bounds;
        //void* userData;
    };

    struct PxBroadPhaseRegionInfo {
        PxBroadPhaseRegion region;
        PxU32 nbStaticObjects;
        PxU32 nbDynamicObjects;
        bool active;
        bool overlap;
    };

    struct PxBroadPhaseCaps {
        PxU32 maxNbRegions;
        PxU32 maxNbObjects;
        bool needsPredefinedBounds;
    };

    %feature("director") PxErrorCallback;
    class PxErrorCallback {
    public:
        PxErrorCallback();
        virtual ~PxErrorCallback();
        virtual void reportError(PxErrorCode::Enum code, const char* message, const char* file, int line) = 0;
    };

    class PxDefaultErrorCallback : public PxErrorCallback {
    public:
        PxDefaultErrorCallback();
        ~PxDefaultErrorCallback();
        virtual void reportError(PxErrorCode::Enum code, const char* message, const char* file, int line);
    };

    //class PxErrorToExceptionCallback : public PxErrorCallback {
    //public:
    //    PxErrorToExceptionCallback();
    //    virtual void reportError(physx::PxErrorCode::Enum code, const char* message, const char* file, int line);
    //};

    //%feature("director") PxAllocatorCallback;
    class PxAllocatorCallback {
    public:
        virtual ~PxAllocatorCallback();
        //virtual void* allocate(size_t size, const char* typeName, const char* filename, int line) = 0;
        //%apply void *VOID_INT_PTR { void* ptr }
        //virtual void deallocate(void* ptr) = 0;
    };

    class PxDefaultAllocator : public PxAllocatorCallback {
    public:
        PxDefaultAllocator();
        //void* allocate(size_t size, const char*, const char*, int);
        //void deallocate(void* ptr);
    };

    enum PxVersion {
        ePX_PHYSICS_VERSION = PX_PHYSICS_VERSION
    };

    struct PxActorShape {
        PxActorShape();
        PxActorShape(PxRigidActor* a, PxShape* s);
        PxRigidActor* actor;
        PxShape* shape;
    };

    struct PxQueryHit : public PxActorShape {
        PxQueryHit();
        PxU32 faceIndex;
    };

    struct PxLocationHit : public PxQueryHit {
        PxLocationHit();
        bool hadInitialOverlap();
        //PxHitFlags flags;
        %extend { PxHitFlag::Enum flags; %{
            physx::PxHitFlag::Enum physx_PxLocationHit_flags_get(const physx::PxLocationHit* hit) { return (physx::PxHitFlag::Enum)(uint32_t)hit->flags; }
            void physx_PxLocationHit_flags_set(physx::PxLocationHit* hit, physx::PxHitFlag::Enum flags) { hit->flags.set(flags); }
        %}}
        PxVec3 position;
        PxVec3 normal;
        PxF32 distance;
    };

    struct PxRaycastHit : public PxLocationHit {
        PxRaycastHit();
        PxReal u, v;
    };

    struct PxOverlapHit: public PxQueryHit
    {
        //PxU32 padTo16Bytes;
    };

    struct PxSweepHit : public PxLocationHit {
        PxSweepHit();
        //PxU32 padTo16Bytes;
    };

    template<typename HitType> struct PxHitCallback {
        HitType block;
        bool hasBlock;
        //HitType* touches;
        PxU32 maxNbTouches;
        PxU32 nbTouches;
        PxHitCallback(HitType* aTouches, PxU32 aMaxNbTouches);
        virtual PxAgain processTouches(const HitType* buffer, PxU32 nbHits) = 0;
        virtual void finalizeQuery() {}
        virtual ~PxHitCallback() {}
        bool hasAnyHits() { return (hasBlock || (nbTouches > 0)); }
    };

    %feature("notabstract") PxHitBuffer;
    template<typename HitType> struct PxHitBuffer : public PxHitCallback<HitType> {
        PxHitBuffer(HitType* aTouches = NULL, PxU32 aMaxNbTouches = 0);
        PxU32 getNbAnyHits() const;
        const HitType& getAnyHit(const PxU32 index) const;
        PxU32 getNbTouches() const;
        const HitType* getTouches() const;
        const HitType& getTouch(const PxU32 index) const;
        PxU32 getMaxNbTouches() const;
        virtual ~PxHitBuffer() {}
    };

    %template(PxRaycastCallback) PxHitCallback<PxRaycastHit>;
    typedef PxHitCallback<PxRaycastHit> PxRaycastCallback;
    %template(PxOverlapCallback) PxHitCallback<PxOverlapHit>;
    typedef PxHitCallback<PxOverlapHit> PxOverlapCallback;
    %template(PxSweepCallback) PxHitCallback<PxSweepHit>;
    typedef PxHitCallback<PxSweepHit> PxSweepCallback;

    %template(PxRaycastBuffer) PxHitBuffer<PxRaycastHit>;
    typedef PxHitBuffer<PxRaycastHit> PxRaycastBuffer;
    %template(PxOverlapBuffer) PxHitBuffer<PxOverlapHit>;
    typedef PxHitBuffer<PxOverlapHit> PxOverlapBuffer;
    %template(PxSweepBuffer) PxHitBuffer<PxSweepHit>;
    typedef PxHitBuffer<PxSweepHit> PxSweepBuffer;

    %feature("notabstract") PxRaycastBufferN;
    template <int N> struct PxRaycastBufferN : public PxHitBuffer<PxRaycastHit> {
        //PxRaycastHit hits[N];
        PxRaycastBufferN();
    };

    %template(PxRaycastBuffer1) PxRaycastBufferN<1>;
    typedef PxRaycastBufferN<1> PxRaycastBuffer1;
    %template(PxRaycastBuffer10) PxRaycastBufferN<10>;
    typedef PxRaycastBufferN<10> PxRaycastBuffer10;
    %template(PxRaycastBuffer100) PxRaycastBufferN<100>;
    typedef PxRaycastBufferN<100> PxRaycastBuffer100;

    %feature("notabstract") PxOverlapBufferN;
    template <int N> struct PxOverlapBufferN : public PxHitBuffer<PxOverlapHit> {
        //PxOverlapHit hits[N];
        PxOverlapBufferN() : PxHitBuffer<PxOverlapHit>(hits, N) {}
    };

    %template(PxOverlapBuffer1) PxOverlapBufferN<1>;
    typedef PxOverlapBufferN<1> PxOverlapBuffer1;
    %template(PxOverlapBuffer10) PxOverlapBufferN<10>;
    typedef PxOverlapBufferN<10> PxOverlapBuffer10;
    %template(PxOverlapBuffer100) PxOverlapBufferN<100>;
    typedef PxOverlapBufferN<100> PxOverlapBuffer100;

    %feature("notabstract") PxSweepBufferN;
    template <int N> struct PxSweepBufferN : public PxHitBuffer<PxSweepHit> {
        //PxSweepHit hits[N];
        PxSweepBufferN() : PxHitBuffer<PxSweepHit>(hits, N) {}
    };

    %template(PxSweepBuffer1) PxSweepBufferN<1>;
    typedef PxSweepBufferN<1> PxSweepBuffer1;
    %template(PxSweepBuffer10) PxSweepBufferN<10>;
    typedef PxSweepBufferN<10> PxSweepBuffer10;
    %template(PxSweepBuffer100) PxSweepBufferN<100>;
    typedef PxSweepBufferN<100> PxSweepBuffer100;

    struct PxQueryFilterData {
        explicit PxQueryFilterData();
        //explicit PxQueryFilterData(const PxFilterData& fd, PxQueryFlags f);
        //explicit PxQueryFilterData(PxQueryFlags f);
        PxFilterData data;
        //PxQueryFlags flags;
        %extend { PxQueryFlag::Enum flags; %{
            physx::PxQueryFlag::Enum physx_PxQueryFilterData_flags_get(const physx::PxQueryFilterData* data) { return (physx::PxQueryFlag::Enum)(uint32_t)data->flags; }
            void physx_PxQueryFilterData_flags_set(physx::PxQueryFilterData* data, physx::PxQueryFlag::Enum flags) { data->flags.set(flags); }
        %}}
    };

    struct PxQueryCache {
        PxQueryCache();
        PxQueryCache(PxShape* s, PxU32 findex);
        PxShape* shape;
        PxRigidActor* actor;
        PxU32 faceIndex;
    };

    struct PxBVH33MidphaseDesc {
        PxF32 meshSizePerformanceTradeOff;
        PxMeshCookingHint::Enum meshCookingHint;
        void setToDefault();
        bool isValid() const;
    };

    struct PxBVH34MidphaseDesc {
        PxU32 numPrimsPerLeaf;
        void setToDefault();
        bool isValid() const;
    };

    class PxMidphaseDesc {
    public:
        PxMidphaseDesc();
        PxMeshMidPhase::Enum getType() const;
        union {
            PxBVH33MidphaseDesc  mBVH33Desc;
            PxBVH34MidphaseDesc  mBVH34Desc;
        };
        void setToDefault(PxMeshMidPhase::Enum type);
        bool isValid() const;
        PxMidphaseDesc& operator=(PxMeshMidPhase::Enum descType);
    protected:
        //PxMeshMidPhase::Enum mType;
    };

    struct PxCookingParams {
        float areaTestEpsilon;
        float planeTolerance;
        PxConvexMeshCookingType::Enum convexMeshCookingType;
        bool suppressTriangleMeshRemapTable;
        bool buildTriangleAdjacencies;
        bool buildGPUData;
        PxTolerancesScale scale;
        //PxMeshPreprocessingFlags meshPreprocessParams;
        %extend { PxMeshPreprocessingFlag::Enum meshPreprocessParams; %{
            physx::PxMeshPreprocessingFlag::Enum physx_PxCookingParams_meshPreprocessParams_get(const physx::PxCookingParams* params) { return (physx::PxMeshPreprocessingFlag::Enum)(uint32_t)params->meshPreprocessParams; }
            void physx_PxCookingParams_meshPreprocessParams_set(physx::PxCookingParams* params, physx::PxMeshPreprocessingFlag::Enum flags) { params->meshPreprocessParams.set(flags); }
        %}}
        PxReal meshWeldTolerance;
        PxMidphaseDesc midphaseDesc;
        PxU32 gaussMapLimit;
        PxCookingParams(const PxTolerancesScale& sc);
    };

    struct PxBoundedData : public PxStridedData
    {
        PxU32 count;
        PxBoundedData();
    };

    class PxSimpleTriangleMesh {
    public:
        PxBoundedData points;
        PxBoundedData triangles;
        //PxMeshFlags flags;
        %extend { PxMeshFlag::Enum flags; %{
            physx::PxMeshFlag::Enum physx_PxSimpleTriangleMesh_flags_get(const physx::PxSimpleTriangleMesh* mesh) { return (physx::PxMeshFlag::Enum)(uint32_t)mesh->flags; }
            void physx_PxSimpleTriangleMesh_flags_set(physx::PxSimpleTriangleMesh* mesh, physx::PxMeshFlag::Enum flags) { mesh->flags.set(flags); }
        %}}
        PxSimpleTriangleMesh();
        void setToDefault();
        bool isValid() const;
    };

    template<typename TDataType> struct PxTypedStridedData {
        PxU32 stride;
        %apply void *VOID_INT_PTR { const TDataType* data }
        const TDataType* data;
        PxTypedStridedData();
    };

    %template(PxMaterialIndices) PxTypedStridedData<PxMaterialTableIndex>;

    class PxTriangleMeshDesc : public PxSimpleTriangleMesh {
    public:
        PxTypedStridedData<PxMaterialTableIndex> materialIndices;
        PxTriangleMeshDesc();
        void setToDefault();
        bool isValid() const;
    };

    class PxOutputStream {
    public:
        %apply void* VOID_INT_PTR { const void* src }
        virtual uint32_t write(const void* src, uint32_t count) = 0;
        virtual ~PxOutputStream() {}
    };

    class PxDefaultMemoryOutputStream : public PxOutputStream {
    public:
        PxDefaultMemoryOutputStream();
        virtual ~PxDefaultMemoryOutputStream();

        %apply void* VOID_INT_PTR { const void* src }
        virtual uint32_t write(const void* src, uint32_t count);

        virtual PxU32 getSize() const;
        %apply void* VOID_INT_PTR { void* getData }
        virtual void* getData() const;
    };

    class PxPhysicsInsertionCallback {
    public:
        //PxPhysicsInsertionCallback() {}
        //%apply void* VOID_INT_PTR { void* data }
        //virtual PxBase* buildObjectFromData(PxConcreteType::Enum type, void* data) = 0;
    protected:
        //virtual ~PxPhysicsInsertionCallback() {}
    private:
        ~PxPhysicsInsertionCallback();
    };

    class PxConvexMeshDesc {
    public:
        PxBoundedData points;
        PxBoundedData polygons;
        PxBoundedData indices;
        //PxConvexFlags flags;
        %extend { PxConvexFlag::Enum flags; %{
            physx::PxConvexFlag::Enum physx_PxConvexMeshDesc_flags_get(const physx::PxConvexMeshDesc* desc) { return (physx::PxConvexFlag::Enum)(uint32_t)desc->flags; }
            void physx_PxConvexMeshDesc_flags_set(physx::PxConvexMeshDesc* desc, physx::PxConvexFlag::Enum flags) { desc->flags.set(flags); }
        %}}
        PxU16 vertexLimit;
        PxU16 quantizedCount;
        PxConvexMeshDesc();
        void setToDefault();
        bool isValid() const;
    };

    class PxBVHStructureDesc {
    public:
        PxBVHStructureDesc();
        PxBoundedData bounds;
        void setToDefault();
        bool isValid() const;
    };

    class PxPvdSceneClient {
    public:
        void setScenePvdFlag(PxPvdSceneFlag::Enum flag, bool value);
        //void setScenePvdFlags(PxPvdSceneFlags flags);
        //PxPvdSceneFlags getScenePvdFlags() const;
        %extend { void setScenePvdFlags(PxPvdSceneFlag::Enum flags) { self->setScenePvdFlags(flags); }}
        %extend { PxPvdSceneFlag::Enum getScenePvdFlags() { return (physx::PxPvdSceneFlag::Enum)(uint32_t)self->getScenePvdFlags(); }}
        void updateCamera(const char* name, const PxVec3& origin, const PxVec3& up, const PxVec3& target);
        //void drawPoints(const physx::pvdsdk::PvdDebugPoint* points, PxU32 count);
        //void drawLines(const physx::pvdsdk::PvdDebugLine* lines, PxU32 count);
        //void drawTriangles(const physx::pvdsdk::PvdDebugTriangle* triangles, PxU32 count);
        //void drawText(const physx::pvdsdk::PvdDebugText& text);
        //physx::pvdsdk::PvdClient* getClientInternal();
    private:
        virtual ~PxPvdSceneClient(){}
    };

    class PxRigidActorList {
    public:
        PxRigidActorList();
        //static PxRigidActorList* create();
        void addRigidActor(PxRigidActor* actor);
        PxU32 getNbRigidActors() const;
        PxRigidActor* getRigidActor(PxU32 index);
        %apply float BYREF[] { PxF32 matrices[] }
        void getRigidActorMatrices(PxF32 matrices[], PxU32 start, PxU32 count);
        void releaseRigidActors();
        //void release();
    private:
        //~PxRigidActorList();
    };

    class PxSceneCollision {
    public:
        PxSceneCollision() {}
        bool canCollide(PxU32 index0, PxU32 index1) const;
        void setCanCollide(PxU32 index0, PxU32 index1, bool yes);
    };

    class PxActorCollision {
    public:
        PxU32 index = 0;
        bool collisionEvents = false;
        PxActorCollision() {}
        bool canCollide(PxU32 index0, PxU32 index1) const;
        void setCanCollide(PxU32 index0, PxU32 index1, bool yes);
    };

    class PxShapeCollision {
    public:
        PxU32 index = 0;
        bool solveContacts = true;
        PxShapeCollision() {}
    };

    class PxUnityCollisionFiltering : public wrap::PxSimulationFilterCallback {
    public:
        static void setCollision(PxShape* shape, PxShapeCollision* collision);
        static PxShapeCollision* getCollision(const PxShape* shape);
        static void setCollision(PxActor* actor, PxActorCollision* collision);
        static PxActorCollision* getCollision(const PxActor* actor);
        static void setCollision(PxScene* scene, PxSceneCollision* collision);
        static PxSceneCollision* getCollision(const PxScene* scene);
        static PxUnityCollisionFiltering const instance;
        %extend { static physx::PxSimulationFilterShader* const function; %{
            physx::PxSimulationFilterShader* physx_PxUnityCollisionFiltering_function_get() { thread_local physx::PxSimulationFilterShader s; s = physx::PxUnityCollisionFiltering::shader; return &s; }
        %}}
    };

    //REFERENCE_SEMANTIC // =========================================================================

    class PxFoundation {
    public:
        %extend { static PxFoundation* create(PxVersion physicsVersion, PxAllocatorCallback& allocatorCallback, PxErrorCallback& errorCallback) { return PxCreateFoundation(physicsVersion, allocatorCallback, errorCallback); }}
        %extend { PxPhysics* createPhysics(PxVersion physicsVersion, const physx::PxTolerancesScale& scale = physx::PxTolerancesScale(), PxPvd* pvd = nullptr) { return PxCreatePhysics(physicsVersion, *self, scale, false, pvd); }}
        %extend { PxCudaContextManager* createCudaContextManager(const char* dllPath) { return PxFoundation_createCudaContextManager(*self, dllPath); }}
        %extend { PxPvd* createPvd() { return physx::PxCreatePvd(*self); }}
        %extend { PxCooking* createCooking(PxVersion physicsVersion, const PxCookingParams& params) { return PxCreateCooking(physicsVersion, *self, params); }}
        void release();
        //PxErrorCallback& getErrorCallback();
        void setErrorLevel(PxErrorCode::Enum mask = PxErrorCode::eMASK_ALL);
        PxErrorCode::Enum getErrorLevel() const;
        //PxAllocatorCallback& getAllocatorCallback();
        bool getReportAllocationNames() const;
        void setReportAllocationNames(bool value);
    protected:
        virtual ~PxFoundation();
    };

    class PxPhysics {
    public:
        void release();
        PxFoundation& getFoundation();
        PxAggregate* createAggregate(PxU32 maxSize, bool enableSelfCollision);
        const PxTolerancesScale& getTolerancesScale() const;
        PxTriangleMesh* createTriangleMesh(PxInputStream& stream);
        PxU32 getNbTriangleMeshes() const;
        //PxU32 getTriangleMeshes(PxTriangleMesh** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxTriangleMesh* getTriangleMesh(PxU32 index) { physx::PxTriangleMesh* tm; self->getTriangleMeshes(&tm, 1, index); return tm; }}
        PxHeightField* createHeightField(PxInputStream& stream);
        PxU32 getNbHeightFields() const;
        //PxU32 getHeightFields(PxHeightField** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxHeightField* getHeightField(PxU32 index) { physx::PxHeightField* hf; self->getHeightFields(&hf, 1, index); return hf; }}
        PxConvexMesh* createConvexMesh(PxInputStream &stream);
        PxU32 getNbConvexMeshes() const;
        //PxU32 getConvexMeshes(PxConvexMesh** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxConvexMesh* getConvexMesh(PxU32 index) { physx::PxConvexMesh* cm; self->getConvexMeshes(&cm, 1, index); return cm; }}
        PxBVHStructure* createBVHStructure(PxInputStream &stream);
        PxU32 getNbBVHStructures() const;
        //PxU32 getBVHStructures(PxBVHStructure** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxBVHStructure* getBVHStructure(PxU32 index) { physx::PxBVHStructure* bvh; self->getBVHStructures(&bvh, 1, index); return bvh; }}
        PxScene* createScene(const PxSceneDesc& sceneDesc);
        PxU32 getNbScenes() const;
        //PxU32 getScenes(PxScene** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxScene* getScene(PxU32 index) { physx::PxScene* s; self->getScenes(&s, 1, index); return s; }}
        PxRigidStatic* createRigidStatic(const PxTransform& pose);
        PxRigidDynamic* createRigidDynamic(const PxTransform& pose);
        %apply physx::PxRigidActor* INPUT[] { PxRigidActor* actors[] }
        PxPruningStructure* createPruningStructure(PxRigidActor* actors[], PxU32 nbActors);
        //PxShape* createShape(const PxGeometry& geometry, const PxMaterial& material, bool isExclusive = false, PxShapeFlags shapeFlags = physx::PxShapeFlag::eVISUALIZATION | physx::PxShapeFlag::eSCENE_QUERY_SHAPE | physx::PxShapeFlag::eSIMULATION_SHAPE);
        %extend { PxShape* createShape(const PxGeometry& geometry, const PxMaterial& material, bool isExclusive = false, PxShapeFlag::Enum shapeFlags = (physx::PxShapeFlag::Enum)(uint32_t)(physx::PxShapeFlag::eVISUALIZATION|physx::PxShapeFlag::eSCENE_QUERY_SHAPE|physx::PxShapeFlag::eSIMULATION_SHAPE)) { return self->createShape(geometry, material, isExclusive, shapeFlags); }}
        //PxShape* createShape(const PxGeometry& geometry, PxMaterial*const * materials, PxU16 materialCount, bool isExclusive = false,PxShapeFlags shapeFlags = PxShapeFlag::eVISUALIZATION | PxShapeFlag::eSCENE_QUERY_SHAPE | PxShapeFlag::eSIMULATION_SHAPE);
        %apply physx::PxMaterial* INPUT[] { PxMaterial* materials[] }
        %extend { PxShape* createShape(const PxGeometry& geometry, PxMaterial* materials[], PxU16 materialCount, bool isExclusive = false,PxShapeFlag::Enum shapeFlags = (physx::PxShapeFlag::Enum)(uint32_t)(physx::PxShapeFlag::eVISUALIZATION|physx::PxShapeFlag::eSCENE_QUERY_SHAPE|physx::PxShapeFlag::eSIMULATION_SHAPE)) { return self->createShape(geometry, materials, materialCount, isExclusive, shapeFlags); }}
        PxU32 getNbShapes() const;
        //PxU32 getShapes(PxShape** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxShape* getShape(PxU32 index) { physx::PxShape* s; self->getShapes(&s, 1, index); return s; }}
        //PxConstraint* createConstraint(PxRigidActor* actor0, PxRigidActor* actor1, PxConstraintConnector& connector, const PxConstraintShaderTable& shaders, PxU32 dataSize);
        %extend { PxFixedJoint* createFixedJoint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxFixedJointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        %extend { PxRevoluteJoint* createRevoluteJoint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxRevoluteJointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        %extend { PxDistanceJoint* createDistanceJoint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxDistanceJointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        %extend { PxSphericalJoint* createSphericalJoint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxSphericalJointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        %extend { PxPrismaticJoint* createPrismaticJoint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxPrismaticJointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        %extend { PxD6Joint* createD6Joint(PxRigidActor* actor0, const PxTransform& localFrame0, PxRigidActor* actor1, const PxTransform& localFrame1) { return physx::PxD6JointCreate(*self, actor0, localFrame0, actor1, localFrame1); }}
        PxArticulation* createArticulation();
        PxArticulationReducedCoordinate* createArticulationReducedCoordinate();
        PxMaterial* createMaterial(PxReal staticFriction, PxReal dynamicFriction, PxReal restitution);
        PxU32 getNbMaterials() const;
        //PxU32 getMaterials(PxMaterial** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxMaterial* getMaterial(PxU32 index) { physx::PxMaterial* m; self->getMaterials(&m, 1, index); return m; }}
        //void registerDeletionListener(PxDeletionListener& observer, const PxDeletionEventFlags& deletionEvents, bool restrictedObjectSet = false);
        //void unregisterDeletionListener(PxDeletionListener& observer);
        //void registerDeletionListenerObjects(PxDeletionListener& observer, const PxBase* const* observables, PxU32 observableCount);
        //void unregisterDeletionListenerObjects(PxDeletionListener& observer, const PxBase* const* observables, PxU32 observableCount);
        PxPhysicsInsertionCallback& getPhysicsInsertionCallback();
        %extend { bool initExtensions(PxPvd* pvd = nullptr) { return PxInitExtensions(*self, pvd); }}
        %extend { void closeExtensions() { PxCloseExtensions(); }}
    private:
        virtual ~PxPhysics() {}
    };

    class PxScene {
    protected:
        PxScene();
        virtual ~PxScene() {}
    public:
        void release();
        void setFlag(PxSceneFlag::Enum flag, bool value);
        //PxSceneFlags getFlags() const;
        %extend { physx::PxSceneFlag::Enum getFlags() const { return (physx::PxSceneFlag::Enum)(uint32_t)self->getFlags(); }}
        void setLimits(const PxSceneLimits& limits);
        PxSceneLimits getLimits() const;
        PxPhysics& getPhysics();
        PxU32 getTimestamp() const;
        void addArticulation(PxArticulationBase& articulation);
        void removeArticulation(PxArticulationBase& articulation, bool wakeOnLostTouch = true);
        void addActor(PxActor& actor, const PxBVHStructure* bvhStructure = NULL);
        %apply physx::PxActor* INPUT[] { PxActor* actors[] }
        void addActors(PxActor* actors[], PxU32 nbActors);
        void addActors(const PxPruningStructure& pruningStructure);
        void removeActor(PxActor& actor, bool wakeOnLostTouch = true);
        void removeActors(PxActor* actors[], PxU32 nbActors, bool wakeOnLostTouch = true);
        void addAggregate(PxAggregate& aggregate);
        void removeAggregate(PxAggregate& aggregate, bool wakeOnLostTouch = true);
        //void addCollection(const PxCollection& collection);
        //PxU32 getNbActors(PxActorTypeFlags types) const;
        //PxU32 getActors(PxActorTypeFlags types, PxActor** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        //PxActor** getActiveActors(PxU32& nbActorsOut);
        %extend { PxU32 getNbActors() { return self->getNbActors(physx::PxActorTypeFlag::Enum::eRIGID_STATIC | physx::PxActorTypeFlag::Enum::eRIGID_DYNAMIC); }}
        %extend { physx::PxActor* getActor(PxU32 index) { physx::PxActor* actor; self->getActors(physx::PxActorTypeFlag::Enum::eRIGID_STATIC | physx::PxActorTypeFlag::Enum::eRIGID_DYNAMIC, &actor, 1, index); return actor; }}
        %extend { PxU32 getNbStaticActors() { return self->getNbActors(physx::PxActorTypeFlag::Enum::eRIGID_STATIC); }}
        %extend { physx::PxActor* getStaticActor(PxU32 index) { physx::PxActor* actor; self->getActors(physx::PxActorTypeFlag::Enum::eRIGID_STATIC, &actor, 1, index); return actor; }}
        %extend { PxU32 getNbDynamicActors() { return self->getNbActors(physx::PxActorTypeFlag::Enum::eRIGID_DYNAMIC); }}
        %extend { physx::PxActor* getDynamicActor(PxU32 index) { physx::PxActor* actor; self->getActors(physx::PxActorTypeFlag::Enum::eRIGID_DYNAMIC, &actor, 1, index); return actor; }}
        PxU32 getNbArticulations() const;
        //PxU32 getArticulations(PxArticulationBase** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxArticulationBase* getArticulation(PxU32 index) { physx::PxArticulationBase* a; self->getArticulations(&a, 1, index); return a; }}
        PxU32 getNbConstraints() const;
        //PxU32 getConstraints(PxConstraint** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxConstraint* getConstraint(PxU32 index) { physx::PxConstraint* c; self->getConstraints(&c, 1, index); return c; }}
        PxU32 getNbAggregates() const;
        //PxU32 getAggregates(PxAggregate** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxAggregate* getAggregate(PxU32 index) { physx::PxAggregate* a; self->getAggregates(&a, 1, index); return a; }}
        void setDominanceGroupPair(PxDominanceGroup group1, PxDominanceGroup group2, const PxDominanceGroupPair& dominance);
        PxDominanceGroupPair getDominanceGroupPair(PxDominanceGroup group1, PxDominanceGroup group2) const;
        PxCpuDispatcher* getCpuDispatcher() const;
        PxCudaContextManager* getCudaContextManager() const;
        PxClientID createClient();
        void setSimulationEventCallback(wrap::PxSimulationEventCallback* callback);
        wrap::PxSimulationEventCallback* getSimulationEventCallback() const;
        //void setContactModifyCallback(PxContactModifyCallback* callback);
        //void setCCDContactModifyCallback(PxCCDContactModifyCallback* callback);
        //PxContactModifyCallback* getContactModifyCallback() const;
        //PxCCDContactModifyCallback* getCCDContactModifyCallback() const;
        void setBroadPhaseCallback(PxBroadPhaseCallback* callback);
        PxBroadPhaseCallback* getBroadPhaseCallback() const;
        %apply void* VOID_INT_PTR { const void* data }
        void setFilterShaderData(const void* data, PxU32 dataSize);
        %apply void* VOID_INT_PTR { const void* getFilterShaderData }
        const void* getFilterShaderData() const;
        PxU32 getFilterShaderDataSize() const;
        PxSimulationFilterShader getFilterShader() const;
        wrap::PxSimulationFilterCallback* getFilterCallback() const;
        void resetFiltering(PxActor& actor);
        %apply physx::PxShape* INPUT[] { PxShape* shapes[] }
        void resetFiltering(PxRigidActor& actor, PxShape* shapes[], PxU32 shapeCount);
        PxPairFilteringMode::Enum getKinematicKinematicFilteringMode() const;
        PxPairFilteringMode::Enum getStaticKinematicFilteringMode() const;
        //void simulate(PxReal elapsedTime, physx::PxBaseTask* completionTask = NULL, void* scratchMemBlock = 0, PxU32 scratchMemBlockSize = 0, bool controlSimulation = true);
        void simulate(PxReal elapsedTime);
        //void advance(physx::PxBaseTask* completionTask = 0);
        void advance();
        //void collide(PxReal elapsedTime, physx::PxBaseTask* completionTask = 0, void* scratchMemBlock = 0, PxU32 scratchMemBlockSize = 0, bool controlSimulation = true);
        void collide(PxReal elapsedTime);
        bool checkResults(bool block = false);
        bool fetchCollision(bool block = false);
        %apply PxU32* OUTPUT { PxU32* errorState }
        bool fetchResults(bool block = false, PxU32* errorState = 0);
        //bool fetchResultsStart(const PxContactPairHeader*& contactPairs, PxU32& nbContactPairs, bool block = false);
        //void processCallbacks(physx::PxBaseTask* continuation);
        //void fetchResultsFinish(PxU32* errorState = 0);
        void flushSimulation(bool sendPendingReports = false);
        void setGravity(const PxVec3& vec);
        PxVec3 getGravity() const;
        void setBounceThresholdVelocity(const PxReal t);
        PxReal getBounceThresholdVelocity() const;
        void setCCDMaxPasses(PxU32 ccdMaxPasses);
        PxU32 getCCDMaxPasses() const;
        PxReal getFrictionOffsetThreshold() const;
        void setFrictionType(PxFrictionType::Enum frictionType);
        PxFrictionType::Enum getFrictionType() const;
        //bool setVisualizationParameter(PxVisualizationParameter::Enum param, PxReal value);
        //PxReal getVisualizationParameter(PxVisualizationParameter::Enum paramEnum) const;
        //void setVisualizationCullingBox(const PxBounds3& box);
        //PxBounds3 getVisualizationCullingBox() const;
        //const PxRenderBuffer& getRenderBuffer();
        void getSimulationStatistics(PxSimulationStatistics& stats) const;
        PxPruningStructureType::Enum getStaticStructure() const;
        PxPruningStructureType::Enum getDynamicStructure() const;
        void flushQueryUpdates();
        void setDynamicTreeRebuildRateHint(PxU32 dynamicTreeRebuildRateHint);
        PxU32 getDynamicTreeRebuildRateHint() const;
        void forceDynamicTreeRebuild(bool rebuildStaticStructure, bool rebuildDynamicStructure);
        void setSceneQueryUpdateMode(PxSceneQueryUpdateMode::Enum updateMode);
        PxSceneQueryUpdateMode::Enum getSceneQueryUpdateMode() const;
        //void sceneQueriesUpdate(physx::PxBaseTask* completionTask = NULL, bool controlSimulation = true);
        void sceneQueriesUpdate();
        bool checkQueries(bool block = false);
        bool fetchQueries(bool block = false);
        //bool raycast(const PxVec3& origin, const PxVec3& unitDir, const PxReal distance, PxRaycastCallback& hitCall, PxHitFlags hitFlags = PxHitFlags(PxHitFlag::eDEFAULT), const PxQueryFilterData& filterData = PxQueryFilterData(), PxQueryFilterCallback* filterCall = NULL, const PxQueryCache* cache = NULL) const;
        %extend { bool raycast(const PxVec3& origin, const PxVec3& unitDir, const PxReal distance, PxRaycastCallback& hitCall, PxHitFlag::Enum hitFlags = physx::PxHitFlag::eDEFAULT, const PxQueryFilterData& filterData = PxQueryFilterData(),const PxQueryCache* cache = NULL) { return self->raycast(origin, unitDir, distance, hitCall, hitFlags, filterData, nullptr, cache); }}
        //bool sweep(const PxGeometry& geometry, const PxTransform& pose, const PxVec3& unitDir, const PxReal distance, PxSweepCallback& hitCall, PxHitFlags hitFlags = PxHitFlags(PxHitFlag::eDEFAULT), const PxQueryFilterData& filterData = PxQueryFilterData(), PxQueryFilterCallback* filterCall = NULL, const PxQueryCache* cache = NULL, const PxReal inflation = 0.f) const;
        %extend { bool sweep(const PxGeometry& geometry, const PxTransform& pose, const PxVec3& unitDir, const PxReal distance, PxSweepCallback& hitCall, PxHitFlag::Enum hitFlags = physx::PxHitFlag::eDEFAULT, const PxQueryFilterData& filterData = PxQueryFilterData(), const PxQueryCache* cache = NULL, const PxReal inflation = 0.f) { return self->sweep(geometry, pose, unitDir, distance, hitCall, hitFlags, filterData, nullptr, cache, inflation); }}
        //bool overlap(const PxGeometry& geometry, const PxTransform& pose, PxOverlapCallback& hitCall, const PxQueryFilterData& filterData = PxQueryFilterData(), PxQueryFilterCallback* filterCall = NULL) const;
        %extend { bool overlap(const PxGeometry& geometry, const PxTransform& pose, PxOverlapCallback& hitCall, const PxQueryFilterData& filterData = PxQueryFilterData()) { return self->overlap(geometry, pose, hitCall, filterData); }}
        PxU32 getSceneQueryStaticTimestamp() const;
        PxBroadPhaseType::Enum getBroadPhaseType() const;
        bool getBroadPhaseCaps(PxBroadPhaseCaps& caps) const;
        PxU32 getNbBroadPhaseRegions() const;
        //PxU32 getBroadPhaseRegions(PxBroadPhaseRegionInfo* userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxBroadPhaseRegionInfo getBroadPhaseRegion(PxU32 index) { physx::PxBroadPhaseRegionInfo r; self->getBroadPhaseRegions(&r, 1, index); return r; }}
        PxU32 addBroadPhaseRegion(const PxBroadPhaseRegion& region, bool populateRegion=false);
        bool removeBroadPhaseRegion(PxU32 handle);
        PxTaskManager* getTaskManager() const;
        void lockRead(const char* file=NULL, PxU32 line=0);
        void unlockRead();
        void lockWrite(const char* file=NULL, PxU32 line=0);
        void unlockWrite();
        void setNbContactDataBlocks(PxU32 numBlocks);
        PxU32 getNbContactDataBlocksUsed() const;
        PxU32 getMaxNbContactDataBlocksUsed() const;
        PxU32 getContactReportStreamBufferSize() const;
        void setSolverBatchSize(PxU32 solverBatchSize);
        PxU32 getSolverBatchSize() const;
        void setSolverArticulationBatchSize(PxU32 solverBatchSize);
        PxU32 getSolverArticulationBatchSize() const;
        PxReal getWakeCounterResetValue() const;
        void shiftOrigin(const PxVec3& shift);
        PxPvdSceneClient* getScenePvdClient();
        //%apply void* VOID_INT_PTR { void* userData }
        //void* userData; //!< user can assign this to whatever, usually to create a 1:1 relationship with a user object.
    };

    class PxBase {
    public:
        virtual void release() = 0;
        virtual const char* getConcreteTypeName() const = 0;
        PxType getConcreteType() const;
        %extend {
            void setBaseFlags(PxBaseFlag::Enum inFlags) { self->setBaseFlags(inFlags); }
            PxBaseFlag::Enum getBaseFlags() const { return (physx::PxBaseFlag::Enum)(uint32_t)self->getBaseFlags(); }
            //PxActor* getActor() { return self->is<physx::PxActor>(); }
            PxRigidActor* getRigidActor() { return self->is<physx::PxRigidActor>(); }
            PxRigidStatic* getRigidStatic() { return self->is<physx::PxRigidStatic>(); }
            PxRigidBody* getRigidBody() { return self->is<physx::PxRigidBody>(); }
            PxRigidDynamic* getRigidDynamic() { return self->is<physx::PxRigidDynamic>(); }
            PxArticulationLink* getArticulationLink() { return self->is<physx::PxArticulationLink>(); }
            //PxShape* getShape() { return self->is<physx::PxShape>(); }
            PxConvexMesh* getConvexMesh() { return self->is<physx::PxConvexMesh>(); }
            PxTriangleMesh* getTriangleMesh() { return self->is<physx::PxTriangleMesh>(); }
            PxHeightField* getHeightField() { return self->is<physx::PxHeightField>(); }
            PxMaterial* getMaterial() { return self->is<physx::PxMaterial>(); }
            //PxBVHStructure* getBVHStructure() { return self->is<physx::PxBVHStructure>(); }
        }
        bool isReleasable() const;
    protected:
        virtual ~PxBase() { }
    };

    class PxActor : public PxBase {
    public:
        PxScene* getScene() const;
        //void setName(const char* name);
        //const char* getName() const;
        PxBounds3 getWorldBounds(float inflation=1.01f) const;
        void setActorFlag(PxActorFlag::Enum flag, bool value);
        %extend { void setActorFlags(PxActorFlag::Enum inFlags) { self->setActorFlags(inFlags); }}
        %extend { PxActorFlag::Enum getActorFlags() { return (physx::PxActorFlag::Enum)(uint32_t)self->getActorFlags(); }}
        void setDominanceGroup(PxDominanceGroup dominanceGroup);
        PxDominanceGroup getDominanceGroup() const;
        PxAggregate* getAggregate() const;
    protected:
        virtual ~PxActor() { }
    };

    class PxRigidActor : public PxActor {
    public:
        PxTransform getGlobalPose() const;
        %extend { const physx::PxMat44& getGlobalMatrix() { thread_local physx::PxMat44 m; m = physx::PxMat44(self->getGlobalPose()); return m; }}
        void setGlobalPose(const PxTransform& pose, bool autowake = true);
        bool attachShape(PxShape& shape);
        void detachShape(PxShape& shape, bool wakeOnLostTouch = true);
        PxU32 getNbShapes() const;
        //PxU32 getShapes(PxShape* userBuffer[], PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxShape* getShape(PxU32 index) { physx::PxShape* s; self->getShapes(&s, 1, index); return s; }}
        PxU32 getNbConstraints() const;
        //PxU32 getConstraints(PxConstraint* userBuffer[], PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { physx::PxConstraint* getConstraint(PxU32 index) { physx::PxConstraint* c; self->getConstraints(&c, 1, index); return c; }}
        %apply physx::PxMaterial* INPUT[] { physx::PxMaterial* materials[] }
        %extend { physx::PxShape* createExclusiveShape(const physx::PxGeometry& geometry, physx::PxMaterial* materials[], PxU16 materialCount, physx::PxShapeFlag::Enum shapeFlags = (physx::PxShapeFlag::Enum)(uint32_t)(physx::PxShapeFlag::eVISUALIZATION|physx::PxShapeFlag::eSCENE_QUERY_SHAPE|physx::PxShapeFlag::eSIMULATION_SHAPE)) { return physx::PxRigidActorExt::createExclusiveShape(*self, geometry, materials, materialCount, shapeFlags); }}
        %extend { physx::PxShape* createExclusiveShape(const physx::PxGeometry& geometry, physx::PxMaterial& material, physx::PxShapeFlag::Enum shapeFlags = (physx::PxShapeFlag::Enum)(uint32_t)(physx::PxShapeFlag::eVISUALIZATION|physx::PxShapeFlag::eSCENE_QUERY_SHAPE|physx::PxShapeFlag::eSIMULATION_SHAPE)) { return physx::PxRigidActorExt::createExclusiveShape(*self, geometry, material, shapeFlags); }}
    protected:
        virtual ~PxRigidActor() { }
    };

    class PxRigidStatic : public PxRigidActor {
    public:
    protected:
        virtual ~PxRigidStatic() { }
    };

    class PxRigidBody : public PxRigidActor {
    public:
        void setCMassLocalPose(const PxTransform& pose);
        PxTransform getCMassLocalPose() const;
        void setMass(PxReal mass);
        PxReal getMass() const;
        PxReal getInvMass() const;
        void setMassSpaceInertiaTensor(const PxVec3& m);
        PxVec3 getMassSpaceInertiaTensor() const;
        PxVec3 getMassSpaceInvInertiaTensor() const;
        void setLinearDamping(PxReal linDamp);
        PxReal getLinearDamping() const;
        void setAngularDamping(PxReal angDamp);
        PxReal getAngularDamping() const;
        PxVec3 getLinearVelocity() const;
        void setLinearVelocity(const PxVec3& linVel, bool autowake = true);
        PxVec3 getAngularVelocity() const;
        void setAngularVelocity(const PxVec3& angVel, bool autowake = true);
        void setMaxAngularVelocity(PxReal maxAngVel);
        PxReal getMaxAngularVelocity() const;
        void setMaxLinearVelocity(PxReal maxLinVel);
        PxReal getMaxLinearVelocity() const;
        void addForce(const PxVec3& force, PxForceMode::Enum mode = PxForceMode::eFORCE, bool autowake = true);
        void addTorque(const PxVec3& torque, PxForceMode::Enum mode = PxForceMode::eFORCE, bool autowake = true);
        void clearForce(PxForceMode::Enum mode = PxForceMode::eFORCE);
        void clearTorque(PxForceMode::Enum mode = PxForceMode::eFORCE);
        void setForceAndTorque(const PxVec3& force, const PxVec3& torque, PxForceMode::Enum mode = PxForceMode::eFORCE);
        void setRigidBodyFlag(PxRigidBodyFlag::Enum flag, bool value);
        %extend { void setRigidBodyFlags(PxRigidBodyFlag::Enum inFlags) { self->setRigidBodyFlags(inFlags); }}
        %extend { PxRigidBodyFlag::Enum getRigidBodyFlags() { return (physx::PxRigidBodyFlag::Enum)(uint32_t)self->getRigidBodyFlags(); }}
        void setMinCCDAdvanceCoefficient(PxReal advanceCoefficient);
        PxReal getMinCCDAdvanceCoefficient() const;
        void setMaxDepenetrationVelocity(PxReal biasClamp);
        PxReal getMaxDepenetrationVelocity() const;
        void setMaxContactImpulse(PxReal maxImpulse);
        PxReal getMaxContactImpulse() const;
        %apply float INPUT[] { const PxReal shapeDensities[], const PxReal shapeMasses[] }
        %extend { bool updateMassAndInertia(const PxReal shapeDensities[], PxU32 shapeDensityCount, const PxVec3* massLocalPose = NULL, bool includeNonSimShapes = false) { return physx::PxRigidBodyExt::updateMassAndInertia(*self, shapeDensities, shapeDensityCount, massLocalPose, includeNonSimShapes); }}
        %extend { bool updateMassAndInertia(PxReal density = 1000.0f, const PxVec3* massLocalPose = NULL, bool includeNonSimShapes = false) { return physx::PxRigidBodyExt::updateMassAndInertia(*self, density, massLocalPose, includeNonSimShapes); }}
        %extend { bool setMassAndUpdateInertia(const PxReal shapeMasses[], PxU32 shapeMassCount, const PxVec3* massLocalPose = NULL, bool includeNonSimShapes = false) { return physx::PxRigidBodyExt::setMassAndUpdateInertia(*self, shapeMasses, shapeMassCount, massLocalPose, includeNonSimShapes); }}
        %extend { bool setMassAndUpdateInertia(PxReal mass, const PxVec3* massLocalPose = NULL, bool includeNonSimShapes = false) { return physx::PxRigidBodyExt::setMassAndUpdateInertia(*self, mass, massLocalPose, includeNonSimShapes); }}
        %apply physx::PxShape* INPUT[] { PxShape* shapes[] }
        %extend { static physx::PxMassProperties computeMassPropertiesFromShapes(physx::PxShape* shapes[], int shapeCount) { return physx::PxRigidBodyExt::computeMassPropertiesFromShapes(shapes, shapeCount); }}
        %extend { void addForceAtPos(const PxVec3& force, const PxVec3& pos, PxForceMode::Enum mode = physx::PxForceMode::eFORCE, bool wakeup = true) { physx::PxRigidBodyExt::addForceAtPos(*self, force, pos, mode, wakeup); }}
        %extend { void addForceAtLocalPos(const PxVec3& force, const PxVec3& pos, PxForceMode::Enum mode = physx::PxForceMode::eFORCE, bool wakeup = true) { physx::PxRigidBodyExt::addForceAtLocalPos(*self, force, pos, mode, wakeup); }}
        %extend { void addLocalForceAtPos(const PxVec3& force, const PxVec3& pos, PxForceMode::Enum mode = physx::PxForceMode::eFORCE, bool wakeup = true) { physx::PxRigidBodyExt::addLocalForceAtPos(*self, force, pos, mode, wakeup); }}
        %extend { void addLocalForceAtLocalPos(const PxVec3& force, const PxVec3& pos, PxForceMode::Enum mode = physx::PxForceMode::eFORCE, bool wakeup = true) { physx::PxRigidBodyExt::addLocalForceAtLocalPos(*self, force, pos, mode, wakeup); }}
        %extend { PxVec3 getVelocityAtPos(const PxVec3& pos) { return physx::PxRigidBodyExt::getVelocityAtPos(*self, pos); }}
        %extend { PxVec3 getLocalVelocityAtLocalPos(const PxVec3& pos) { return physx::PxRigidBodyExt::getLocalVelocityAtLocalPos(*self, pos); }}
        %extend { PxVec3 getVelocityAtOffset(const PxVec3& pos) { return physx::PxRigidBodyExt::getVelocityAtOffset(*self, pos); }}
        %apply PxVec3& OUTPUT { PxVec3& deltaLinearVelocity, PxVec3& deltaAngularVelocity }
        %extend { void computeVelocityDeltaFromImpulse(const PxVec3& impulsiveForce, const PxVec3& impulsiveTorque, PxVec3& deltaLinearVelocity, PxVec3& deltaAngularVelocity) { physx::PxRigidBodyExt::computeVelocityDeltaFromImpulse(*self, impulsiveForce, impulsiveTorque, deltaLinearVelocity, deltaAngularVelocity); }}
        %extend { void computeVelocityDeltaFromImpulse(const PxTransform& globalPose, const PxVec3& point, const PxVec3& impulse, const PxReal invMassScale, const PxReal invInertiaScale, PxVec3& deltaLinearVelocity, PxVec3& deltaAngularVelocity) { physx::PxRigidBodyExt::computeVelocityDeltaFromImpulse(*self, globalPose, point, impulse, invMassScale, invInertiaScale, deltaLinearVelocity, deltaAngularVelocity); }}
        %clear PxVec3& deltaLinearVelocity, PxVec3& deltaAngularVelocity;
        %apply PxVec3& OUTPUT { PxVec3& linearImpulse, PxVec3& angularImpulse }
        %extend { void computeLinearAngularImpulse(const PxTransform& globalPose, const PxVec3& point, const PxVec3& impulse, const PxReal invMassScale, const PxReal invInertiaScale, PxVec3& linearImpulse, PxVec3& angularImpulse) { physx::PxRigidBodyExt::computeLinearAngularImpulse(*self, globalPose, point, impulse, invMassScale, invInertiaScale, linearImpulse, angularImpulse); }}
        %clear PxVec3& linearImpulse, PxVec3& angularImpulse;
    protected:
        virtual ~PxRigidBody() { }
    };

    class PxRigidDynamic : public PxRigidBody {
    public:
        void setKinematicTarget(const PxTransform& destination);
        bool getKinematicTarget(PxTransform& target) const;
        bool isSleeping() const;
        void setSleepThreshold(PxReal threshold);
        PxReal getSleepThreshold() const;
        void setStabilizationThreshold(PxReal threshold);
        PxReal getStabilizationThreshold() const;
        %extend { void setRigidDynamicLockFlags(PxRigidDynamicLockFlag::Enum flags) { self->setRigidDynamicLockFlags(flags); }}
        %extend { PxRigidDynamicLockFlag::Enum getRigidDynamicLockFlags() { return (physx::PxRigidDynamicLockFlag::Enum)(uint32_t)self->getRigidDynamicLockFlags(); }}
        void setWakeCounter(PxReal wakeCounterValue);
        PxReal getWakeCounter() const;
        void wakeUp();
        void putToSleep();
        void setSolverIterationCounts(PxU32 minPositionIters, PxU32 minVelocityIters = 1);
        %apply PxU32& OUTPUT { PxU32& minPositionIters, PxU32& minVelocityIters }
        void getSolverIterationCounts(PxU32& minPositionIters, PxU32& minVelocityIters) const;
        %clear PxU32& minPositionIters, PxU32& minVelocityIters;
        PxReal getContactReportThreshold() const;
        void setContactReportThreshold(PxReal threshold);
    protected:
        virtual ~PxRigidDynamic() {}
    };

    class PxShape : public PxBase {
    public:
        PxU32 getReferenceCount() const;
        void acquireReference();
        PxGeometryType::Enum getGeometryType() const;
        void setGeometry(const PxGeometry& geometry);
        PxGeometryHolder getGeometry();
        //bool getBoxGeometry(PxBoxGeometry& geometry) const;
        //bool getSphereGeometry(PxSphereGeometry& geometry) const;
        //bool getCapsuleGeometry(PxCapsuleGeometry& geometry) const;
        //bool getPlaneGeometry(PxPlaneGeometry& geometry) const;
        //bool getConvexMeshGeometry(PxConvexMeshGeometry& geometry) const;
        //bool getTriangleMeshGeometry(PxTriangleMeshGeometry& geometry) const;
        //bool getHeightFieldGeometry(PxHeightFieldGeometry& geometry) const;
        PxRigidActor* getActor() const;
        void setLocalPose(const PxTransform& pose);
        PxTransform getLocalPose() const;
        void setSimulationFilterData(const PxFilterData& data);
        PxFilterData getSimulationFilterData() const;
        void setQueryFilterData(const PxFilterData& data);
        PxFilterData getQueryFilterData() const;
        %apply physx::PxMaterial* INPUT[] { PxMaterial* materials[] }
        void setMaterials(PxMaterial* materials[], PxU16 materialCount);
        PxU16 getNbMaterials() const;
        //PxU32 getMaterials(PxMaterial* userBuffer[], PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxMaterial* getMaterial(PxU32 index) { physx::PxMaterial* m; self->getMaterials(&m, 1, index); return m; }}
        PxMaterial* getMaterialFromInternalFaceIndex(PxU32 faceIndex) const;
        void setContactOffset(PxReal contactOffset);
        PxReal getContactOffset() const;
        void setRestOffset(PxReal restOffset);
        PxReal getRestOffset() const;
        void setTorsionalPatchRadius(PxReal radius);
        PxReal getTorsionalPatchRadius() const;
        void setMinTorsionalPatchRadius(PxReal radius);
        PxReal getMinTorsionalPatchRadius() const;
        void setFlag(PxShapeFlag::Enum flag, bool value);
        %extend { void setFlags(PxShapeFlag::Enum inFlags) { self->setFlags(inFlags); }}
        %extend { PxShapeFlag::Enum getFlags() { return (physx::PxShapeFlag::Enum)(uint32_t)self->getFlags(); }}
        bool isExclusive() const;
        void setName(const char* name);
        const char* getName() const;
    protected:
        virtual ~PxShape() {}
    };

    class PxConvexMesh : public PxBase {
    public:
        PxU32 getNbVertices() const;
        //const PxVec3* getVertices() const;
        %extend { const PxVec3& getVertex(PxU32 index) { return self->getVertices()[index]; }}
        //const PxU8* getIndexBuffer() const;
        %extend { PxU8 getIndex(PxU32 index) { return self->getIndexBuffer()[index]; }}
        PxU32 getNbPolygons() const;
        %apply PxHullPolygon& OUTPUT { PxHullPolygon& data }
        bool getPolygonData(PxU32 index, PxHullPolygon& data) const;
        PxU32 getReferenceCount() const;
        void acquireReference();
        %apply float& OUTPUT { PxReal& mass }
        %apply PxMat33& OUTPUT { PxMat33& localInertia }
        %apply PxVec3& OUTPUT { PxVec3& localCenterOfMass }
        void getMassInformation(PxReal& mass, PxMat33& localInertia, PxVec3& localCenterOfMass) const;
        %extend { PxReal getMass() { physx::PxReal m; physx::PxMat33 i; physx::PxVec3 c; self->getMassInformation(m, i, c); return m; }}
        %extend { const PxMat33& getLocalInertia() { physx::PxReal m; thread_local physx::PxMat33 i; physx::PxVec3 c; self->getMassInformation(m, i, c); return i; }}
        %extend { const PxVec3& getLocalCenterOfMass() { physx::PxReal m; physx::PxMat33 i; thread_local physx::PxVec3 c; self->getMassInformation(m, i, c); return c; }}
        PxBounds3 getLocalBounds() const;
        bool isGpuCompatible() const;
    protected:
        ~PxConvexMesh() {}
    };

    class PxTriangleMesh : public PxBase {
    public:
        PxU32 getNbVertices() const;
        //const PxVec3* getVertices() const;
        %extend { PxVec3 getVertex(PxU32 index) { return self->getVertices()[index]; }}
        //PxVec3* getVerticesForModification();
        %extend { void setVertex(PxU32 index, const PxVec3& position) { self->getVerticesForModification()[index] = position; }}
        PxBounds3 refitBVH();
        PxU32 getNbTriangles() const;
        //%apply void *VOID_INT_PTR { void* getTriangles }
        //const void* getTriangles() const;
        %extend { int getTriangleVertex(PxU32 index, PxU32 vertexIndex) { if (self->getTriangleMeshFlags().isSet(physx::PxTriangleMeshFlag::e16_BIT_INDICES)) return ((physx::PxU16*)self->getTriangles())[index * 3 + vertexIndex]; else return ((physx::PxU32*)self->getTriangles())[index * 3 + vertexIndex]; }}
        %extend { PxTriangleMeshFlag::Enum getTriangleMeshFlags() { return (physx::PxTriangleMeshFlag::Enum)(uint32_t)self->getTriangleMeshFlags(); }}
        //%apply void *VOID_INT_PTR { PxU32* getTrianglesRemap }
        //const PxU32* getTrianglesRemap() const;
        PxMaterialTableIndex getTriangleMaterialIndex(PxTriangleID triangleIndex) const;
        PxBounds3 getLocalBounds() const;
        PxU32 getReferenceCount() const;
        void acquireReference();
    protected:
        virtual ~PxTriangleMesh() {}
    };

    class PxHeightField : public PxBase {
    public:
        %apply void *VOID_INT_PTR { void* destBuffer }
        PxU32 saveCells(void* destBuffer, PxU32 destBufferSize) const;
        bool modifySamples(PxI32 startCol, PxI32 startRow, const PxHeightFieldDesc& subfieldDesc, bool shrinkBounds = false);
        PxU32 getNbRows() const;
        PxU32 getNbColumns() const;
        PxHeightFieldFormat::Enum getFormat() const;
        PxU32 getSampleStride() const;
        PxReal getConvexEdgeThreshold() const;
        %extend { PxHeightFieldFlag::Enum getFlags() { return (physx::PxHeightFieldFlag::Enum)(uint32_t)self->getFlags(); }}
        PxReal getHeight(PxReal x, PxReal z) const;
        PxU32 getReferenceCount() const;
        void acquireReference();
        PxMaterialTableIndex getTriangleMaterialIndex(PxTriangleID triangleIndex) const;
        PxVec3 getTriangleNormal(PxTriangleID triangleIndex) const;
        const PxHeightFieldSample& getSample(PxU32 row, PxU32 column) const;
        PxU32 getTimestamp() const;
    protected:
        virtual ~PxHeightField() { }
    };

    class PxMaterial : public PxBase {
    public:
        void release();
        PxU32 getReferenceCount() const;
        void acquireReference();
        void setDynamicFriction(PxReal coef);
        PxReal getDynamicFriction() const;
        void setStaticFriction(PxReal coef);
        PxReal getStaticFriction() const;
        void setRestitution(PxReal rest);
        PxReal getRestitution() const;
        void setFlag(PxMaterialFlag::Enum flag, bool);
        //void setFlags(PxMaterialFlags inFlags);
        %extend { void setFlags(PxMaterialFlag::Enum inFlags) { self->setFlags(inFlags); }}
        //PxMaterialFlags getFlags() const;
        %extend { PxMaterialFlag::Enum getFlags() { return (physx::PxMaterialFlag::Enum)(uint32_t)self->getFlags(); }}
        void setFrictionCombineMode(PxCombineMode::Enum combMode);
        PxCombineMode::Enum getFrictionCombineMode() const;
        void setRestitutionCombineMode(PxCombineMode::Enum combMode);
        PxCombineMode::Enum getRestitutionCombineMode();
    protected:
        virtual ~PxMaterial() {}
    };

    class PxBVHStructure : public PxBase {
    public:
        %apply PxU32 *OUTPUT { PxU32* rayHits, PxU32* sweepHits, PxU32* overlapHits }
        PxU32 raycast(const PxVec3& origin, const PxVec3& unitDir, PxReal maxDist, PxU32 maxHits, PxU32* rayHits);
        PxU32 sweep(const PxBounds3& aabb, const PxVec3& unitDir, PxReal maxDist, PxU32 maxHits, PxU32* sweepHits);
        PxU32 overlap(const PxBounds3& aabb, PxU32 maxHits, PxU32* overlapHits) const;
        const PxBounds3* getBounds() const;
        PxU32 getNbBounds() const;
        const char* getConcreteTypeName() const;
    protected:
        ~PxBVHStructure() {}
    };

    class PxPruningStructure : public PxBase {
    public:
        %extend { PxRigidActor* getRigidActor(PxU32 index) { physx::PxRigidActor* ra; self->getRigidActors(&ra, 1, index); return ra; }}
        PxU32 getNbRigidActors() const;
    protected:
        virtual ~PxPruningStructure() {}
    };

    class PxConstraint : public PxBase {
    public:
        PxScene* getScene() const;
        //void getActors(PxRigidActor*& actor0, PxRigidActor*& actor1) const;
        %extend { PxRigidActor* getActor0() { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); return a0; }}
        %extend { PxRigidActor* getActor1() { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); return a1; }}
        //void setActors(PxRigidActor* actor0, PxRigidActor* actor1);
        %extend { void setActor0(PxRigidActor* actor) { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); self->setActors(actor, a1); }}
        %extend { void setActor1(PxRigidActor* actor) { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); self->setActors(a0, actor); }}
        void markDirty();
        //void setFlags(PxConstraintFlags flags);
        %extend { void setFlags(PxConstraintFlag::Enum flags) { self->setFlags(flags); }}
        //PxConstraintFlags getFlags() const;
        %extend { PxConstraintFlag::Enum getFlags() { return (physx::PxConstraintFlag::Enum)(uint32_t)self->getFlags(); }}
        void setFlag(PxConstraintFlag::Enum flag, bool value);
        void getForce(PxVec3& linear, PxVec3& angular) const;
        bool isValid() const;
        void setBreakForce(PxReal linear, PxReal angular);
        %apply PxReal& OUTPUT { PxReal& linear, PxReal& angular }
        void getBreakForce(PxReal& linear, PxReal& angular) const;
        void setMinResponseThreshold(PxReal threshold);
        PxReal getMinResponseThreshold() const;
        //void* getExternalReference(PxU32& typeID);
        void setConstraintFunctions(PxConstraintConnector& connector, const PxConstraintShaderTable& shaders);
    protected:
        virtual ~PxConstraint() {}
    };

    class PxArticulationJointBase : public PxBase {
    public:
        PxArticulationLink& getParentArticulationLink() const;
        void setParentPose(const PxTransform& pose);
        PxTransform getParentPose() const;
        PxArticulationLink& getChildArticulationLink() const;
        void setChildPose(const PxTransform& pose);
        PxTransform getChildPose() const;
        %extend { PxArticulationJoint* getArticulationJoint() { return static_cast<physx::PxArticulationJoint*>(self); }}
        %extend { PxArticulationJointReducedCoordinate* getArticulationJointReducedCoordinate() { return static_cast<physx::PxArticulationJointReducedCoordinate*>(self); }}
        ~PxArticulationJointBase() {}
    };

    class PxArticulationJoint : public PxArticulationJointBase {
    public:
        void setTargetOrientation(const PxQuat& orientation);
        PxQuat getTargetOrientation() const;
        void setTargetVelocity(const PxVec3& velocity);
        PxVec3 getTargetVelocity() const;
        void setDriveType(PxArticulationJointDriveType::Enum driveType);
        PxArticulationJointDriveType::Enum getDriveType() const;
        void setStiffness(PxReal spring);
        PxReal getStiffness() const;
        void setDamping(PxReal damping);
        PxReal getDamping() const;
        void setInternalCompliance(PxReal compliance);
        PxReal getInternalCompliance() const;
        void setExternalCompliance(PxReal compliance);
        PxReal getExternalCompliance() const;
        void setSwingLimit(PxReal zLimit, PxReal yLimit);
        %apply PxReal& OUTPUT { PxReal& zLimit, PxReal& yLimit }
        void getSwingLimit(PxReal& zLimit, PxReal& yLimit) const;
        void setTangentialStiffness(PxReal spring);
        PxReal getTangentialStiffness() const;
        void setTangentialDamping(PxReal damping);
        PxReal getTangentialDamping() const;
        void setSwingLimitContactDistance(PxReal contactDistance);
        PxReal getSwingLimitContactDistance() const;
        void setSwingLimitEnabled(bool enabled);
        bool getSwingLimitEnabled() const;
        void setTwistLimit(PxReal lower, PxReal upper);
        %apply PxReal& OUTPUT { PxReal &lower, PxReal &upper }
        void getTwistLimit(PxReal &lower, PxReal &upper) const;
        void setTwistLimitEnabled(bool enabled);
        bool getTwistLimitEnabled() const;
        void setTwistLimitContactDistance(PxReal contactDistance);
        PxReal getTwistLimitContactDistance() const;
    protected:
        virtual ~PxArticulationJoint() {}
    };

    OUTPUT_TYPEMAP(physx::PxArticulationDriveType::Enum, int, PxArticulationDriveType, INT32_PTR)
    class PxArticulationJointReducedCoordinate : public PxArticulationJointBase {
    public:
        void setJointType(PxArticulationJointType::Enum jointType);
        PxArticulationJointType::Enum getJointType() const;
        void setMotion(PxArticulationAxis::Enum axis, PxArticulationMotion::Enum motion);
        PxArticulationMotion::Enum getMotion(PxArticulationAxis::Enum axis) const;
        void setLimit(PxArticulationAxis::Enum axis, const PxReal lowLimit, const PxReal highLimit);
        %apply PxReal& OUTPUT { PxReal& lowLimit, PxReal& highLimit }
        void getLimit(PxArticulationAxis::Enum axis, PxReal& lowLimit, PxReal& highLimit);
        void setDrive(PxArticulationAxis::Enum axis, const PxReal stiffness, const PxReal damping, const PxReal maxForce, PxArticulationDriveType::Enum driveType = PxArticulationDriveType::eFORCE);
        %apply PxReal& OUTPUT { PxReal& stiffness, PxReal& damping, PxReal& maxForce }
        %apply PxArticulationDriveType::Enum& OUTPUT { PxArticulationDriveType::Enum& driveType }
        void getDrive(PxArticulationAxis::Enum axis, PxReal& stiffness, PxReal& damping, PxReal& maxForce, PxArticulationDriveType::Enum& driveType);
        void setDriveTarget(PxArticulationAxis::Enum axis, const PxReal target);
        void setDriveVelocity(PxArticulationAxis::Enum axis, const PxReal targetVel);
        PxReal getDriveTarget(PxArticulationAxis::Enum axis);
        PxReal getDriveVelocity(PxArticulationAxis::Enum axis);
        void setFrictionCoefficient(const PxReal coefficient);
        PxReal getFrictionCoefficient() const;
        void setMaxJointVelocity(const PxReal maxJointV);
        PxReal getMaxJointVelocity() const;
    protected:
        virtual ~PxArticulationJointReducedCoordinate() {}
    };

    class PxArticulationLink : public PxRigidBody {
    public:
        PxArticulationBase& getArticulation() const;
        PxArticulationJointBase* getInboundJoint() const;
        %extend { PxArticulationJointReducedCoordinate* getInboundJointReducedCoordinate() { return static_cast<physx::PxArticulationJointReducedCoordinate*>(self->getInboundJoint()); }}
        PxU32 getInboundJointDof() const;
        PxU32 getNbChildren() const;
        PxU32 getLinkIndex() const;
        //PxU32 getChildren(PxArticulationLink** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxArticulationLink* getChild(PxU32 index) { physx::PxArticulationLink* c; self->getChildren(&c, 1, index); return c; }}
    protected:
        virtual ~PxArticulationLink()    {}
    };

    class PxArticulationBase : public PxBase {
    public:
        PxScene* getScene() const;
        void setSolverIterationCounts(PxU32 minPositionIters, PxU32 minVelocityIters = 1);
        %apply PxU32& OUTPUT { PxU32& minPositionIters, PxU32& minVelocityIters }
        void getSolverIterationCounts(PxU32& minPositionIters, PxU32& minVelocityIters) const;
        bool isSleeping() const;
        void setSleepThreshold(PxReal threshold);
        PxReal getSleepThreshold() const;
        void setStabilizationThreshold(PxReal threshold);
        PxReal getStabilizationThreshold() const;
        void setWakeCounter(PxReal wakeCounterValue);
        PxReal getWakeCounter() const;
        void wakeUp();
        void putToSleep();
        PxArticulationLink* createLink(PxArticulationLink* parent, const PxTransform& pose);
        //PxU32 getLinks(PxArticulationLink** userBuffer, PxU32 bufferSize, PxU32 startIndex = 0) const;
        %extend { PxArticulationLink* getLink(PxU32 index) { physx::PxArticulationLink* l; self->getLinks(&l, 1, index); return l; }}
        void setName(const char* name);
        const char* getName() const;
        PxBounds3 getWorldBounds(float inflation = 1.01f) const;
        PxAggregate* getAggregate() const;
        //PxArticulationImpl* getImpl();
        //const PxArticulationImpl* getImpl() const;
        //virtual ~PxArticulationBase() {}
    protected:
        //PxArticulationBase(PxType concreteType, PxBaseFlags baseFlags) : PxBase(concreteType, baseFlags), userData(NULL) {}
        //PxArticulationBase(PxBaseFlags baseFlags) : PxBase(baseFlags) {}
    public:
        PxArticulationJointBase* createArticulationJoint(PxArticulationLink& parent, const PxTransform& parentFrame, PxArticulationLink& child, const PxTransform& childFrame);
        void releaseArticulationJoint(PxArticulationJointBase* joint);
    };

    class PxArticulation : public PxArticulationBase {
    public:
        void release();
        void setMaxProjectionIterations(PxU32 iterations);
        PxU32 getMaxProjectionIterations() const;
        void setSeparationTolerance(PxReal tolerance);
        PxReal getSeparationTolerance() const;
        void setInternalDriveIterations(PxU32 iterations);
        PxU32 getInternalDriveIterations() const;
        void setExternalDriveIterations(PxU32 iterations);
        PxU32 getExternalDriveIterations() const;
        PxArticulationDriveCache* createDriveCache(PxReal compliance, PxU32 driveIterations) const;
        void updateDriveCache(PxArticulationDriveCache& driveCache, PxReal compliance, PxU32 driveIterations) const;
        void releaseDriveCache(PxArticulationDriveCache& driveCache) const;
        void applyImpulse(PxArticulationLink* link, const PxArticulationDriveCache& driveCache, const PxVec3& linearImpulse, const PxVec3& angularImpulse);
        void computeImpulseResponse(PxArticulationLink*link, PxVec3& linearResponse, PxVec3& angularResponse, const PxArticulationDriveCache& driveCache, const PxVec3& linearImpulse, const PxVec3& angularImpulse) const;
    protected:
        virtual ~PxArticulation() {}
    };

    class PxArticulationReducedCoordinate : public PxArticulationBase {
    public:
        //void setArticulationFlags(PxArticulationFlags flags);
        %extend { void setArticulationFlags(physx::PxArticulationFlag::Enum flags) { self->setArticulationFlags(flags); }}
        void setArticulationFlag(PxArticulationFlag::Enum flag, bool value);
        //PxArticulationFlags getArticulationFlags() const;
        %extend { physx::PxArticulationFlag::Enum getArticulationFlags() { return (physx::PxArticulationFlag::Enum)(uint32_t)self->getArticulationFlags(); }}
        PxU32 getDofs() const;
        PxArticulationCache* createCache() const;
        PxU32 getCacheDataSize() const;
        void zeroCache(PxArticulationCache& cache);
        //void applyCache(PxArticulationCache& cache, const PxArticulationCacheFlags flag, bool autowake = true);
        %extend { void applyCache(PxArticulationCache& cache, const PxArticulationCache::Enum flag, bool autowake = true) { self->applyCache(cache, flag, autowake); }}
        //void copyInternalStateToCache(PxArticulationCache& cache, const PxArticulationCacheFlags flag) const;
        %extend { void copyInternalStateToCache(PxArticulationCache& cache, const PxArticulationCache::Enum flag) { self->copyInternalStateToCache(cache, flag); }}
        void releaseCache(PxArticulationCache& cache) const;
        //void packJointData(const PxReal* maximum, PxReal* reduced) const;
        //void unpackJointData(const PxReal* reduced, PxReal* maximum) const;
        void commonInit() const;
        void computeGeneralizedGravityForce(PxArticulationCache& cache) const;
        void computeCoriolisAndCentrifugalForce(PxArticulationCache& cache) const;
        void computeGeneralizedExternalForce(PxArticulationCache& cache) const;
        void computeJointAcceleration(PxArticulationCache& cache) const;
        void computeJointForce(PxArticulationCache& cache) const;
        %apply PxU32& OUTPUT { PxU32& nRows, PxU32& nCols }
        void computeDenseJacobian(PxArticulationCache& cache, PxU32& nRows, PxU32& nCols) const;
        void computeCoefficientMatrix(PxArticulationCache& cache) const;
        //bool computeLambda(PxArticulationCache& cache, PxArticulationCache& initialState, const PxReal* const jointTorque, const PxU32 maxIter) const;
        void computeGeneralizedMassMatrix(PxArticulationCache& cache) const;
        void addLoopJoint(PxJoint* joint);
        void removeLoopJoint(PxJoint* joint);
        PxU32 getNbLoopJoints() const;
        //PxU32 getLoopJoints(PxJoint** userBuffer, PxU32 bufferSize, PxU32 startIndex = 0) const;
        %extend { physx::PxJoint* getLoopJoint(PxU32 index) { physx::PxJoint* j; self->getLoopJoints(&j, 1, index); return j; }}
        PxU32 getCoefficientMatrixSize() const;
        void teleportRootLink(const PxTransform& pose, bool autowake);
    protected:
        virtual ~PxArticulationReducedCoordinate() {}
    };

    class PxAggregate : public PxBase {
    public:
        bool addActor(PxActor& actor, const PxBVHStructure* bvhStructure = NULL);
        bool removeActor(PxActor& actor);
        bool addArticulation(PxArticulationBase& articulation);
        bool removeArticulation(PxArticulationBase& articulation);
        PxU32 getNbActors() const;
        PxU32 getMaxNbActors() const;
        //PxU32 getActors(PxActor** userBuffer, PxU32 bufferSize, PxU32 startIndex=0) const;
        %extend { PxActor* getActor(PxU32 index) { physx::PxActor* a; self->getActors(&a, 1, index); return a; }}
        PxScene* getScene();
        bool getSelfCollision() const;
    protected:
        virtual ~PxAggregate() {}
    };

    class PxCudaContextManager {
    public:
        bool contextIsValid() const;
        void release();
    protected:
        virtual ~PxCudaContextManager();
    };

    class PxJoint : public PxBase {
    public:
        //void setActors(PxRigidActor* actor0, PxRigidActor* actor1);
        //void getActors(PxRigidActor*& actor0, PxRigidActor*& actor1) const;
        %extend { PxRigidActor* getActor0() { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); return a0; }}
        %extend { PxRigidActor* getActor1() { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); return a1; }}
        %extend { void setActor0(PxRigidActor* actor) { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); self->setActors(actor, a1); }}
        %extend { void setActor1(PxRigidActor* actor) { physx::PxRigidActor *a0, *a1; self->getActors(a0, a1); self->setActors(a0, actor); }}
        void setLocalPose(PxJointActorIndex::Enum actor, const PxTransform& localPose);
        PxTransform getLocalPose(PxJointActorIndex::Enum actor) const;
        PxTransform getRelativeTransform() const;
        PxVec3 getRelativeLinearVelocity() const;
        PxVec3 getRelativeAngularVelocity() const;
        void setBreakForce(PxReal force, PxReal torque);
        %apply PxReal& OUTPUT { PxReal& force, PxReal& torque }
        void getBreakForce(PxReal& force, PxReal& torque) const;
        //void setConstraintFlags(PxConstraintFlags flags);
        %extend { void setConstraintFlags(PxConstraintFlag::Enum flags) { self->setConstraintFlags(flags); }}
        void setConstraintFlag(PxConstraintFlag::Enum flag, bool value);
        //PxConstraintFlags getConstraintFlags() const;
        %extend { physx::PxConstraintFlag::Enum getConstraintFlags() { return (physx::PxConstraintFlag::Enum)(uint32_t)self->getConstraintFlags(); }}
        void setInvMassScale0(PxReal invMassScale);
        PxReal getInvMassScale0() const;
        void setInvInertiaScale0(PxReal invInertiaScale);
        PxReal getInvInertiaScale0() const;
        void setInvMassScale1(PxReal invMassScale);
        PxReal getInvMassScale1() const;
        void setInvInertiaScale1(PxReal invInertiaScale);
        PxReal getInvInertiaScale1() const;
        PxConstraint* getConstraint() const;
        //void setName(const char* name);
        //const char* getName() const;
        void release();
        PxScene* getScene() const;
        //void* userData;
        //static void getBinaryMetaData(PxOutputStream& stream);
    protected:
        virtual ~PxJoint() {}
    };

    class PxFixedJoint : public PxJoint {
    public:
        void setProjectionLinearTolerance(PxReal tolerance);
        PxReal getProjectionLinearTolerance() const;
        void setProjectionAngularTolerance(PxReal tolerance);
        PxReal getProjectionAngularTolerance() const;
    protected:
        virtual ~PxFixedJoint();
    };

    class PxRevoluteJoint : public PxJoint {
    public:
        PxReal getAngle() const;
        PxReal getVelocity() const;
        void setLimit(const PxJointAngularLimitPair& limits);
        PxJointAngularLimitPair getLimit() const;
        void setDriveVelocity(PxReal velocity, bool autowake = true);
        PxReal getDriveVelocity() const;
        void setDriveForceLimit(PxReal limit);
        PxReal getDriveForceLimit() const;
        void setDriveGearRatio(PxReal ratio);
        PxReal getDriveGearRatio() const;
        //void setRevoluteJointFlags(PxRevoluteJointFlags flags);
        %extend { void setRevoluteJointFlags(PxRevoluteJointFlag::Enum flags) { self->setRevoluteJointFlags(flags); }}
        void setRevoluteJointFlag(PxRevoluteJointFlag::Enum flag, bool value);
        //PxRevoluteJointFlags getRevoluteJointFlags() const;
        %extend { PxRevoluteJointFlag::Enum getRevoluteJointFlags() { return (physx::PxRevoluteJointFlag::Enum)(uint32_t)self->getRevoluteJointFlags(); }}
        void setProjectionLinearTolerance(PxReal tolerance);
        PxReal getProjectionLinearTolerance() const;
        void setProjectionAngularTolerance(PxReal tolerance);
        PxReal getProjectionAngularTolerance() const;
    protected:
        virtual PxRevoluteJoint() {}
    };

    class PxDistanceJoint : public PxJoint {
    public:
        PxReal getDistance() const;
        void setMinDistance(PxReal distance);
        PxReal getMinDistance() const;
        void setMaxDistance(PxReal distance);
        PxReal getMaxDistance() const;
        void setTolerance(PxReal tolerance);
        PxReal getTolerance() const;
        void setStiffness(PxReal stiffness);
        PxReal getStiffness() const;
        void setDamping(PxReal damping);
        PxReal getDamping() const;
        //void setDistanceJointFlags(PxDistanceJointFlags flags);
        %extend { void setDistanceJointFlags(PxDistanceJointFlag::Enum flags) { self->setDistanceJointFlags(flags); }}
        void setDistanceJointFlag(PxDistanceJointFlag::Enum flag, bool value);
        //PxDistanceJointFlags getDistanceJointFlags() const;
        %extend { PxDistanceJointFlag::Enum getDistanceJointFlags() { return (physx::PxDistanceJointFlag::Enum)(uint32_t)self->getDistanceJointFlags(); }}
    protected:
        virtual ~PxDistanceJoint();
    };

    class PxSphericalJoint : public PxJoint {
    public:
        PxJointLimitCone getLimitCone() const;
        void setLimitCone(const PxJointLimitCone& limit);
        PxReal getSwingYAngle() const;
        PxReal getSwingZAngle() const;
        //void setSphericalJointFlags(PxSphericalJointFlags flags);
        %extend { void setSphericalJointFlags(PxSphericalJointFlag::Enum flags) { self->setSphericalJointFlags(flags); }}
        void setSphericalJointFlag(PxSphericalJointFlag::Enum flag, bool value);
        //PxSphericalJointFlags getSphericalJointFlags() const;
        %extend { PxSphericalJointFlag::Enum getSphericalJointFlags() { return (physx::PxSphericalJointFlag::Enum)(uint32_t)self->getSphericalJointFlags(); }}
        void setProjectionLinearTolerance(PxReal tolerance);
        PxReal getProjectionLinearTolerance() const;
    protected:
        virtual ~PxSphericalJoint();
    };

    class PxPrismaticJoint : public PxJoint {
    public:
        PxReal getPosition() const;
        PxReal getVelocity() const;
        void setLimit(const PxJointLinearLimitPair&);
        PxJointLinearLimitPair getLimit() const;
        //void setPrismaticJointFlags(PxPrismaticJointFlags flags);
        %extend { void setPrismaticJointFlags(PxPrismaticJointFlag::Enum flags) { self->setPrismaticJointFlags(flags); }}
        void setPrismaticJointFlag(PxPrismaticJointFlag::Enum flag, bool value);
        //PxPrismaticJointFlags getPrismaticJointFlags() const;
        %extend { PxPrismaticJointFlag::Enum getPrismaticJointFlags() { return (physx::PxPrismaticJointFlag::Enum)(uint32_t)self->getPrismaticJointFlags(); }}
        void setProjectionLinearTolerance(PxReal tolerance);
        PxReal getProjectionLinearTolerance() const;
        void setProjectionAngularTolerance(PxReal tolerance);
        PxReal getProjectionAngularTolerance() const;
    protected:
        virtual ~PxPrismaticJoint();
    };

    class PxD6Joint : public PxJoint {
    public:
        void setMotion(PxD6Axis::Enum axis, PxD6Motion::Enum type);
        PxD6Motion::Enum getMotion(PxD6Axis::Enum axis) const;
        PxReal getTwistAngle() const;
        PxReal getSwingYAngle() const;
        PxReal getSwingZAngle() const;
        void setDistanceLimit(const PxJointLinearLimit& limit);
        PxJointLinearLimit getDistanceLimit() const;
        void setLinearLimit(PxD6Axis::Enum axis, const PxJointLinearLimitPair& limit);
        PxJointLinearLimitPair getLinearLimit(PxD6Axis::Enum axis) const;
        void setTwistLimit(const PxJointAngularLimitPair& limit);
        PxJointAngularLimitPair getTwistLimit() const;
        void setSwingLimit(const PxJointLimitCone& limit);
        PxJointLimitCone getSwingLimit() const;
        void setPyramidSwingLimit(const PxJointLimitPyramid& limit);
        PxJointLimitPyramid getPyramidSwingLimit() const;
        void setDrive(PxD6Drive::Enum index, const PxD6JointDrive& drive);
        PxD6JointDrive getDrive(PxD6Drive::Enum index) const;
        void setDrivePosition(const PxTransform& pose, bool autowake = true);
        PxTransform getDrivePosition() const;
        void setDriveVelocity(const PxVec3& linear, const PxVec3& angular, bool autowake = true);
        void getDriveVelocity(PxVec3& linear, PxVec3& angular) const;
        void setProjectionLinearTolerance(PxReal tolerance);
        PxReal getProjectionLinearTolerance() const;
        void setProjectionAngularTolerance(PxReal tolerance);
        PxReal getProjectionAngularTolerance() const;
    protected:
        virtual ~PxD6Joint() {}
    };

    class PxPvdTransport {
    public:
        %extend { static PxPvdTransport* createDefaultSocketTransport(const char* host, int port, int timeout) { thread_local std::string sl_host; sl_host = host; return physx::PxDefaultPvdSocketTransportCreate(sl_host.c_str(), port, timeout); }}
        bool connect();
        void disconnect();
        bool isConnected();
        //bool write(const uint8_t* inBytes, uint32_t inLength);
        //PxPvdTransport& lock();
        //void unlock();
        //void flush();
        //uint64_t getWrittenDataSize();
        void release();
    protected:
        virtual ~PxPvdTransport();
    };

    class PxProfilerCallback {
    protected:
        PxProfilerCallback();
        virtual ~PxProfilerCallback() {}
    };

    class PxPvd : public physx::PxProfilerCallback {
    public:
        //bool connect(PxPvdTransport& transport, PxPvdInstrumentationFlags flags);
        %extend { bool connect(PxPvdTransport& transport, PxPvdInstrumentationFlag::Enum flags) { return self->connect(transport, physx::PxPvdInstrumentationFlag::ePROFILE/*flags*/); }}
        void disconnect();
        bool isConnected(bool useCachedStatus = true);
        PxPvdTransport* getTransport();
        //PxPvdInstrumentationFlags getInstrumentationFlags();
        %extend { PxPvdInstrumentationFlag::Enum getInstrumentationFlags() { return (physx::PxPvdInstrumentationFlag::Enum)(uint32_t)self->getInstrumentationFlags(); }}
        void release();
      protected:
        virtual ~PxPvd() {}
    };

    class PxCpuDispatcher {
    public:
        virtual void submitTask( PxBaseTask& task ) = 0;
        virtual uint32_t getWorkerCount() const = 0;
        virtual ~PxCpuDispatcher() {}
        %apply PxU32 INPUT[] { PxU32 affinityMasks[] }
        %extend { static physx::PxDefaultCpuDispatcher* createDefault(PxU32 numThreads, PxU32 affinityMasks[] = NULL) { return physx::PxDefaultCpuDispatcherCreate(numThreads, affinityMasks); }}
    };

    class PxDefaultCpuDispatcher : public PxCpuDispatcher {
    public:
        //%apply PxU32 INPUT[] { PxU32 affinityMasks[] }
        //%extend { static physx::PxDefaultCpuDispatcher* create(PxU32 numThreads, PxU32 affinityMasks[] = NULL) { return physx::PxDefaultCpuDispatcherCreate(numThreads, affinityMasks); }}
        void release();
        void setRunProfiled(bool runProfiled);
        bool getRunProfiled() const;
    };

    class PxBaseTask {
    public:
        PxBaseTask();
        virtual ~PxBaseTask();
        virtual void run() = 0;
        virtual const char* getName() const = 0;
        virtual void addReference() = 0;
        virtual void removeReference() = 0;
        virtual int32_t getReference() const = 0;
        virtual void release() = 0;
        PxTaskManager* getTaskManager() const;
        void setContextId(PxU64 id);
        PxU64 getContextId();
    };

    class PxTask : public PxBaseTask {
    public:
        PxTask();
        virtual ~PxTask() {}
        void release();
        void finishBefore( PxTaskID taskID );
        void startAfter( PxTaskID taskID );
        void addReference();
        void removeReference();
        int32_t getReference() const;
        PxTaskID getTaskID() const;
        virtual void submitted();
    };

    class PxTaskManager {
    public:
        void setCpuDispatcher(PxCpuDispatcher& ref);
        PxCpuDispatcher* getCpuDispatcher() const;
        void resetDependencies();
        void startSimulation();
        void stopSimulation();
        void taskCompleted(PxTask& task);
        PxTaskID getNamedTask(const char* name);
        PxTaskID submitNamedTask(PxTask* task, const char* name, PxTaskType::Enum type = PxTaskType::TT_CPU);
        PxTaskID submitUnnamedTask(PxTask& task, PxTaskType::Enum type = PxTaskType::TT_CPU);
        PxTask* getTaskFromID(PxTaskID id);
        void release();
        static PxTaskManager* createTaskManager(PxErrorCallback& errorCallback, PxCpuDispatcher* = 0);
    protected:
        virtual ~PxTaskManager() {}
    };

    class PxLightCpuTask : public PxBaseTask {
    public:
        PxLightCpuTask();
        virtual ~PxLightCpuTask();
        void setContinuation(PxTaskManager& tm, PxBaseTask* c);
        void setContinuation( PxBaseTask* c );
        PxBaseTask* getContinuation() const;
        void removeReference();
        int32_t getReference() const;
        void addReference();
        void release();
    };

    OUTPUT_TYPEMAP(physx::PxTriangleMeshCookingResult::Enum, int, PxTriangleMeshCookingResult, INT32_PTR)
    OUTPUT_TYPEMAP(physx::PxConvexMeshCookingResult::Enum, int, PxConvexMeshCookingResult, INT32_PTR)
    class PxCooking {
    public:
        void release();
        void setParams(const PxCookingParams& params);
        const PxCookingParams& getParams() const;
        bool platformMismatch() const;
        %apply PxTriangleMeshCookingResult::Enum* OUTPUT { PxTriangleMeshCookingResult::Enum* condition }
        bool cookTriangleMesh(const PxTriangleMeshDesc& desc, PxOutputStream& stream, PxTriangleMeshCookingResult::Enum* condition = NULL) const;
        PxTriangleMesh* createTriangleMesh(const PxTriangleMeshDesc& desc, PxPhysicsInsertionCallback& insertionCallback, PxTriangleMeshCookingResult::Enum* condition = NULL) const;
        bool validateTriangleMesh(const PxTriangleMeshDesc& desc) const;
        %apply PxConvexMeshCookingResult::Enum* OUTPUT { PxConvexMeshCookingResult::Enum* condition }
        bool cookConvexMesh(const PxConvexMeshDesc& desc, PxOutputStream& stream, PxConvexMeshCookingResult::Enum* condition = NULL) const;
        PxConvexMesh* createConvexMesh(const PxConvexMeshDesc& desc, PxPhysicsInsertionCallback& insertionCallback, PxConvexMeshCookingResult::Enum* condition = NULL) const;
        bool validateConvexMesh(const PxConvexMeshDesc& desc) const;
        //bool computeHullPolygons(const PxSimpleTriangleMesh& mesh, PxAllocatorCallback& inCallback, PxU32& nbVerts, PxVec3*& vertices, PxU32& nbIndices, PxU32*& indices, PxU32& nbPolygons, PxHullPolygon*& hullPolygons) const;
        bool cookHeightField(const PxHeightFieldDesc& desc, PxOutputStream& stream) const;
        PxHeightField* createHeightField(const PxHeightFieldDesc& desc, PxPhysicsInsertionCallback& insertionCallback) const;
        bool cookBVHStructure(const PxBVHStructureDesc& desc, PxOutputStream& stream) const;
        PxBVHStructure* createBVHStructure(const PxBVHStructureDesc& desc, PxPhysicsInsertionCallback& insertionCallback) const;
    protected:
        virtual ~PxCooking(){}
    };

}