using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using PXU = NVIDIA.PhysX.Unity;
using NVIDIA.PhysX.UnityExtensions;

namespace NVIDIA.PhysX.UnityEditor
{
    public class PxEditor
    {
        #region Constants

        const string MENU_ROOT = "NVIDIA/PhysX/";

        #endregion

        #region Variables

        public static string workingDirectory;

        #endregion

        #region Menu

        // Create Asset

        [MenuItem(MENU_ROOT + "Create Asset/Px Material", priority = 100)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Material", priority = 100)]
        static void CreatePxMaterial()
        {
            CreateAsset<PXU.PxMaterial>("New PxMaterial");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Convex Mesh", priority = 200)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Convex Mesh", priority = 200)]
        static void CreatePxConvexMesh()
        {
            CreateAsset<PXU.PxConvexMesh>("New PxConvexMesh");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Triangle Mesh", priority = 210)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Triangle Mesh", priority = 210)]
        static void CreatePxTriangleMesh()
        {
            CreateAsset<PXU.PxTriangleMesh>("New PxTriangleMesh");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Height Field", priority = 220)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Height Field", priority = 220)]
        static void CreatePxHeightField()
        {
            CreateAsset<PXU.PxHeightField>("New PxHeightField");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Sphere Shape", priority = 300)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Sphere Shape", priority = 300)]
        static void CreatePxSphereShape()
        {
            CreateAsset<PXU.PxSphereShape>("New PxSphereShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Capsule Shape", priority = 310)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Capsule Shape", priority = 310)]
        static void CreatePxCapsuleShape()
        {
            CreateAsset<PXU.PxCapsuleShape>("New PxCapsuleShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Box Shape", priority = 320)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Box Shape", priority = 320)]
        static void CreatePxBoxShape()
        {
            CreateAsset<PXU.PxBoxShape>("New PxBoxShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Plane Shape", priority = 330)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Plane Shape", priority = 330)]
        static void CreatePxPlaneShape()
        {
            CreateAsset<PXU.PxPlaneShape>("New PxPlaneShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Convex Mesh Shape", priority = 340)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Convex Mesh Shape", priority = 340)]
        static void CreatePxConvexMeshShape()
        {
            CreateAsset<PXU.PxConvexMeshShape>("New PxConvexMeshShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Triangle Mesh Shape", priority = 350)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Triangle Mesh Shape", priority = 350)]
        static void CreatePxTriangleMeshShape()
        {
            CreateAsset<PXU.PxTriangleMeshShape>("New PxTriangleMeshShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Height Field Shape", priority = 360)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Height Field Shape", priority = 360)]
        static void CreatePxHeightFieldShape()
        {
            CreateAsset<PXU.PxHeightFieldShape>("New PxHeightFieldShape");
        }

        [MenuItem(MENU_ROOT + "Create Asset/Px Compound Shape", priority = 370)]
        [MenuItem("Assets/Create/NVIDIA/PhysX/Px Compound Shape", priority = 370)]
        static void CreatePxCompoundShape()
        {
            CreateAsset<PXU.PxCompoundShape>("New PxCompoundShape");
        }

        // Create Object

        [MenuItem(MENU_ROOT + "Create Object/PhysX Scene", priority = 100)]
        static void CreateMainPxScene()
        {
            var sceneObject = new GameObject("PhysX Scene", typeof(PXU.PxScene));
            Undo.RegisterCreatedObjectUndo(sceneObject, "Create PhysX Scene");
            AddTag("MainPxScene");
            var mainScene = GameObject.FindGameObjectWithTag("MainPxScene");
            if (mainScene == null) sceneObject.tag = "MainPxScene";
            Selection.activeGameObject = sceneObject;
        }

        [MenuItem(MENU_ROOT + "Create Object/Px Static Plane", priority = 110)]
        static void CreatePxStaticPlane()
        {
            var staticPlaneObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            Object.DestroyImmediate(staticPlaneObject.GetComponent<Collider>());
            var planeShape = AssetDatabase.LoadAssetAtPath<PXU.PxPlaneShape>("Assets/NVIDIA/PhysX/Assets/Shapes/Plane Shape.asset");
            staticPlaneObject.AddComponent<PXU.PxStaticActor>().collisionShape = planeShape;
            int index = 0;
            while (GameObject.Find("Px Static Plane" + (index > 0 ? " (" + index + ")" : ""))) ++index;
            staticPlaneObject.name = "Px Static Plane" + (index > 0 ? " (" + index + ")" : "");
            Undo.RegisterCreatedObjectUndo(staticPlaneObject, "Create Px Static Plane");
            Selection.activeGameObject = staticPlaneObject;
        }

        [MenuItem(MENU_ROOT + "Create Object/Px Static Box", priority = 120)]
        static void CreatePxStaticBox()
        {
            var staticBoxObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Object.DestroyImmediate(staticBoxObject.GetComponent<Collider>());
            staticBoxObject.AddComponent<PXU.PxStaticBoxActor>();
            int index = 0;
            while (GameObject.Find("Px Static Box" + (index > 0 ? " (" + index + ")" : ""))) ++index;
            staticBoxObject.name = "Px Static Box" + (index > 0 ? " (" + index + ")" : "");
            Undo.RegisterCreatedObjectUndo(staticBoxObject, "Create Px Static Box");
            Selection.activeGameObject = staticBoxObject;
        }

        // Add Component

        [MenuItem(MENU_ROOT + "Add Component/Px Scene", priority = 100)]
        static void AddPxSceneComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxScene>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Scene", true)]
        static bool CanAddPxSceneComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                bool valid = o.GetComponent<PXU.PxScene>() == null;
                if (!valid) return false;
            }
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Static Actor", priority = 200)]
        static void AddPxStaticActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxStaticActor>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Static Actor", true)]
        static bool CanAddPxStaticActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                bool valid = o.GetComponent<PXU.PxActor>() == null;
                if (!valid) return false;
            }
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Dynamic Actor", priority = 210)]
        static void AddPxDynamicActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxDynamicActor>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Dynamic Actor", true)]
        static bool CanAddPxDynamicActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                bool valid = o.GetComponent<PXU.PxActor>() == null;
                if (!valid) return false;
            }
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Articulated Actor", priority = 220)]
        static void AddPxArticulatedActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxArticulatedActor>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Articulated Actor", true)]
        static bool CanAddPxArticulatedActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                bool valid = o.GetComponent<PXU.PxActor>() == null;
                if (!valid) return false;
            }
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Static Box Actor", priority = 300)]
        static void AddPxStaticBoxActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxStaticBoxActor>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Static Box Actor", true)]
        static bool CanAddPxStaticBoxActorComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                bool valid = o.GetComponent<PXU.PxActor>() == null;
                if (!valid) return false;
            }
            return Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Fixed Joint", priority = 400)]
        static void AddPxFixedJointComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxFixedJoint>();
            }
        }

        [MenuItem(MENU_ROOT + "Add Component/Px Fixed Joint", true)]
        static bool CanAddPxFixedJointComponent()
        {
            return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
        }

        [MenuItem(MENU_ROOT + "Add Component/Px D6 Joint", priority = 410)]
        static void AddPxD6JointComponent()
        {
            foreach (GameObject o in Selection.gameObjects)
            {
                o.AddComponent<PXU.PxD6Joint>();
            }
        }
        [MenuItem(MENU_ROOT + "Add Component/Px D6 Joint", true)]
        static bool CanAddPxD6JointComponent()
        {
            return Selection.gameObjects != null && Selection.gameObjects.Length > 0;
        }

        // Settings

        [MenuItem(MENU_ROOT + "Settings", priority = 500)]
        static void SelectPxSettings()
        {
            Selection.activeObject = FindOrCreatePxSettingsAsset();
        }

        private static PXU.PxSettings FindOrCreatePxSettingsAsset()
        {
            var settings = AssetDatabase.LoadAssetAtPath<PXU.PxSettings>("Assets/NVIDIA/PhysX/Assets/Resources/PhysX Settings.asset");

            if (settings == null)
            {
                settings = CreateAsset<PXU.PxSettings>("PhysX Settings", "Assets/NVIDIA/PhysX/Assets/Resources/");
            }

            return settings;
        }

        #endregion

        #region Private

        static T CreateAsset<T>(string _name = "", string path = null) where T : ScriptableObject
        {
            T asset = ScriptableObject.CreateInstance<T>();
            asset.name = _name;

            if (path == null)
            {
                path = AssetDatabase.GetAssetPath(Selection.activeObject);
                if (path == "") path = "Assets";
                else if (Path.GetExtension(path) != "") path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
            }

            if (_name == "") _name = typeof(T).ToString();
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/" + _name + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

            return asset;
        }

        static bool AddTag(string tagName)
        {
            // Open tag manager
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            // Tags Property
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            if (tagsProp.arraySize >= 10000)
            {
                Debug.Log("No more tags can be added to the Tags property. You have " + tagsProp.arraySize + " tags");
                return false;
            }

            // if not found, add it
            if (!PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
            {
                int index = tagsProp.arraySize;
                // Insert new array element
                tagsProp.InsertArrayElementAtIndex(index);
                SerializedProperty sp = tagsProp.GetArrayElementAtIndex(index);
                // Set array element to tagName
                sp.stringValue = tagName;
                Debug.Log("Tag: " + tagName + " has been added");
                // Save settings
                tagManager.ApplyModifiedProperties();
                return true;
            }
            else
            {
                //Debug.Log ("Tag: " + tagName + " already exists");
            }

            return false;
        }

        static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (int i = start; i < end; i++)
            {
                SerializedProperty t = property.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(value))
                {
                    return true;
                }
            }

            return false;
        }

        private static void GetWorkingDirectory()
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {

            }
        }

        #endregion
    }
}
