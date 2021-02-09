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


physx::PxCudaContextManager* PxFoundation_createCudaContextManager(physx::PxFoundation& foundation, const char* dllPath);

namespace physx
{
    enum PxVersion {
        ePHYSX_VERSION = PX_PHYSICS_VERSION
    };

    //class PxErrorToExceptionCallback : public physx::PxErrorCallback
    //{
    //public:
    //    PxErrorToExceptionCallback() {}
    //    virtual void reportError(physx::PxErrorCode::Enum code, const char* message, const char* file, int line);
    //};

    class PxRigidActorList
    {
    public:
        void addRigidActor(PxRigidActor* actor);
        PxU32 getNbRigidActors() const;
        PxRigidActor* getRigidActor(PxU32 index);
        void getRigidActorMatrices(PxF32* matrices, PxU32 start, PxU32 count);
        void releaseRigidActors();
    private:
        std::vector<PxRigidActor*> m_actors;
    };

    typedef PxRaycastBufferN<1> PxRaycastBuffer1;
    typedef PxRaycastBufferN<10> PxRaycastBuffer10;
    typedef PxRaycastBufferN<100> PxRaycastBuffer100;

    typedef PxOverlapBufferN<1> PxOverlapBuffer1;
    typedef PxOverlapBufferN<10> PxOverlapBuffer10;
    typedef PxOverlapBufferN<100> PxOverlapBuffer100;

    typedef PxSweepBufferN<1> PxSweepBuffer1;
    typedef PxSweepBufferN<10> PxSweepBuffer10;
    typedef PxSweepBufferN<100> PxSweepBuffer100;

    PxFilterFlags PxCustomSimulationFilterShader(PxFilterObjectAttributes attributes0, PxFilterData filterData0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, PxPairFlags& pairFlags, const void* constantBlock, PxU32 constantBlockSize);

    void PxSetCallbacksEnabled(PxActor& actor, bool yes);
    bool PxGetCallbacksEnabled(const PxActor& actor);

    template<typename _Type>
    struct PxList
    {
        const _Type* array; PxU32 count;
        PxList(const _Type* array, PxU32 count) : array(array), count(count) {}
        const _Type& get(PxU32 index) const { return array[index]; }
    };

    using PxConstraintInfoList = PxList<PxConstraintInfo>;
    using PxActorList = PxList<PxActor*>;
    using PxContactPairList = PxList<PxContactPair>;
    using PxTriggerPairList = PxList<PxTriggerPair>;
    using PxRigidBodyList = PxList<const PxRigidBody*>;
    using PxTransformList = PxList<PxTransform>;

    namespace wrap
    {
        class PxSimulationEventCallback : public physx::PxSimulationEventCallback {
        public:
            virtual void onConstraintBreak(const PxConstraintInfoList& constraints) {}
            virtual void onConstraintBreak(PxConstraintInfo* constraints, PxU32 count) { onConstraintBreak(PxConstraintInfoList(constraints, count)); }

            virtual void onWake(const PxActorList& actors) {}
            virtual void onWake(PxActor** actors, PxU32 count) { onWake(PxActorList(actors, count)); }

            virtual void onSleep(const PxActorList& actors) {}
            virtual void onSleep(PxActor** actors, PxU32 count) { onSleep(PxActorList(actors, count)); }

            virtual void onContact(const PxContactPairHeader& pairHeader, const PxContactPairList& pairs) {}
            virtual void onContact(const PxContactPairHeader& pairHeader, const PxContactPair* pairs, PxU32 nbPairs) { onContact(pairHeader, PxContactPairList(pairs, nbPairs)); }

            virtual void onTrigger(const PxTriggerPairList& pairs) {}
            virtual void onTrigger(PxTriggerPair* pairs, PxU32 count) { onTrigger(PxTriggerPairList(pairs, count)); }

            virtual void onAdvance(const PxRigidBodyList& bodyBuffer, const PxTransformList& poseBuffer) {}
            virtual void onAdvance(const PxRigidBody*const* bodyBuffer, const PxTransform* poseBuffer, const PxU32 count) { onAdvance(PxRigidBodyList(bodyBuffer, count), PxTransformList(poseBuffer, count)); }

            virtual ~PxSimulationEventCallback() {}
        };

        class PxSimulationFilterCallback : public physx::PxSimulationFilterCallback {
        public:
            virtual PxFilterFlag::Enum pairFound(PxU32 pairID, const PxFilterObjectAttributes& attributes0, const PxFilterData& filterData0, const PxActor* a0, const PxShape* s0, const PxFilterObjectAttributes& attributes1, const PxFilterData& filterData1, const PxActor* a1, const PxShape* s1, PxPairFlag::Enum& pairFlags) { return PxFilterFlag::eDEFAULT; }
            virtual PxFilterFlags pairFound(PxU32 pairID, PxFilterObjectAttributes attributes0, PxFilterData filterData0, const PxActor* a0, const PxShape* s0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, const PxActor* a1, const PxShape* s1, PxPairFlags& _pairFlags)
            {
                PxPairFlag::Enum pairFlags;
                PxFilterFlag::Enum ret = pairFound(pairID, attributes0, filterData0, a0, s0, attributes1, filterData1, a1, s1, pairFlags);
                _pairFlags.set(pairFlags);
                return ret;
            }
            virtual void pairLost(PxU32 pairID, bool objectRemoved, const PxFilterObjectAttributes& attributes0, const PxFilterData& filterData0, const PxFilterObjectAttributes& attributes1, const PxFilterData& filterData1) {}
            virtual void pairLost(PxU32 pairID, PxFilterObjectAttributes attributes0, PxFilterData filterData0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, bool objectRemoved)
            {
                pairLost(pairID, attributes0, filterData0, attributes1, filterData1, objectRemoved);
            }
            virtual bool statusChange(PxU32& pairID, PxPairFlag::Enum& pairFlags, PxFilterFlag::Enum& filterFlags) { return false; }
            virtual bool statusChange(PxU32& pairID, PxPairFlags& _pairFlags, PxFilterFlags& _filterFlags)
            {
                PxPairFlag::Enum pairFlags; PxFilterFlag::Enum filterFlags;
                bool ret = statusChange(pairID, pairFlags, filterFlags);
                _pairFlags.set(pairFlags); _filterFlags.set(filterFlags);
                return ret;
            }
        };
    }

    template <PxU32 _Size, typename _Packs = PxU32>
    struct PxFlagMatrix
    {
        using PackType = _Packs;
        enum { PACK_SIZE = sizeof(PackType) * 8 };
        enum { MAX_COUNT = _Size };
        enum { BIT_COUNT = MAX_COUNT * (MAX_COUNT + 1) / 2 };
        enum { PACK_COUNT = BIT_COUNT / PACK_SIZE + (BIT_COUNT % PACK_SIZE ? 1 : 0) };

        inline void set(PxU32 _i, PxU32 _j)
        {
            assert(_i < MAX_COUNT && _j < MAX_COUNT);
            PxU32 i = _i > _j ? _i : _j, j = _i > _j ? _j : _i;
            PxU32 bit_index = j * MAX_COUNT - j * (j + 1) / 2 + i;
            PxU32 pack_index = bit_index / PACK_SIZE;
            PxU32 sub_index = bit_index % PACK_SIZE;
            m_packs[pack_index] |= PackType(1ULL << sub_index);
        }
        inline void set(PxU32 _i, PxU32 _j, bool _yes)
        {
            assert(_i < MAX_COUNT && _j < MAX_COUNT);
            PxU32 i = _i > _j ? _i : _j, j = _i > _j ? _j : _i;
            PxU32 bit_index = j * MAX_COUNT - j * (j + 1) / 2 + i;
            PxU32 pack_index = bit_index / PACK_SIZE;
            PxU32 sub_index = bit_index % PACK_SIZE;
            if (_yes) m_packs[pack_index] |= PackType(1ULL << sub_index);
            else m_packs[pack_index] &= PackType(~(1ULL << sub_index));
        }
        inline void reset(PxU32 _i, PxU32 _j)
        {
            assert(_i < MAX_COUNT && _j < MAX_COUNT);
            PxU32 i = _i > _j ? _i : _j, j = _i > _j ? _j : _i;
            PxU32 bit_index = j * MAX_COUNT - j * (j + 1) / 2 + i;
            PxU32 pack_index = bit_index / PACK_SIZE;
            PxU32 sub_index = bit_index % PACK_SIZE;
            m_packs[pack_index] &= PackType(~(1ULL << sub_index));
        }
        inline bool check(PxU32 _i, PxU32 _j) const
        {
            assert(_i < MAX_COUNT && _j < MAX_COUNT);
            PxU32 i = _i > _j ? _i : _j, j = _i > _j ? _j : _i;
            PxU32 bit_index = j * MAX_COUNT - j * (j + 1) / 2 + i;
            PxU32 pack_index = bit_index / PACK_SIZE;
            PxU32 sub_index = bit_index % PACK_SIZE;
            return (m_packs[pack_index] & PackType(1ULL << sub_index)) != 0;
        }
        inline void clear()
        {
            for (PxU32 i = 0; i < pack_count; ++i)
            {
                m_packs[i] = 0;
            }
        }

    private:

        PackType m_packs[PACK_COUNT];
    };

    class PxSceneCollision {
    public:
        static const PxU32 SIGNATURE = 0x12345678;
        const PxU32 signature = SIGNATURE;
        PxFlagMatrix<16> collisionMatrix;
        PxSceneCollision() {}
        bool canCollide(PxU32 index0, PxU32 index1) const { return collisionMatrix.check(index0, index1); }
        void setCanCollide(PxU32 index0, PxU32 index1, bool yes) { collisionMatrix.set(index0, index1, yes); }
    };

    class PxActorCollision {
    public:
        static const PxU32 SIGNATURE = 0x87654321;
        const PxU32 signature = SIGNATURE;
        PxU32 index = 0;
        bool collisionEvents = false;
        PxFlagMatrix<8> collisionMatrix;
        PxActorCollision() {}
        bool canCollide(PxU32 index0, PxU32 index1) const { return collisionMatrix.check(index0, index1); }
        void setCanCollide(PxU32 index0, PxU32 index1, bool yes) { collisionMatrix.set(index0, index1, yes); }
    };

    class PxShapeCollision {
    public:
        static const PxU32 SIGNATURE = 0x18273645;
        const PxU32 signature = SIGNATURE;
        bool solveContacts = true;
        PxU32 index = 0;
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
        static PxFilterFlags shader(PxFilterObjectAttributes attributes0, PxFilterData filterData0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, PxPairFlags& pairFlags, const void* constantBlock, PxU32 constantBlockSize);
        virtual PxFilterFlags pairFound(PxU32 pairID, PxFilterObjectAttributes attributes0, PxFilterData filterData0, const PxActor* a0, const PxShape* s0, PxFilterObjectAttributes attributes1, PxFilterData filterData1, const PxActor* a1, const PxShape* s1, PxPairFlags& _pairFlags);
        static PxUnityCollisionFiltering instance;
    };
}