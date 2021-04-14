project "NVIDIA.PhysX.UnityEditor"
    language "C#"
    kind "SharedLib"
    clr "Unsafe"
    files "**.cs"
    dotnetframework "4.7.2"

    basedir = os.getcwd()

    links { 
        "NVIDIA.PhysX", 
        "NVIDIA.PhysX.Unity",
        basedir .. "/../External/UnityEditor.dll",
        basedir .. "/../External/UnityEngine.dll" }

    filter "configurations:Debug"
        defines { "DEBUG" }
        symbols "On"
        
        targetdir (basedir .. "/../Bin/Debug")

        postbuildcommands { 
            "{COPY} " .. basedir .. "/../Bin/Debug/NVIDIA.PhysX.UnityEditor.dll " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Editor/",

            "{COPY} " .. basedir .. "/../Bin/Debug/NVIDIA.PhysX.UnityEditor.pdb " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Editor/"
        }

    filter "configurations:Release"
        defines { "NDEBUG" }
        optimize "On"

        targetdir (basedir .. "/../Bin/Release")

        postbuildcommands { 
            "{COPY} " .. basedir .. "/../Bin/Release/NVIDIA.PhysX.UnityEditor.dll " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Editor/",

            "{COPY} " .. basedir .. "/../Bin/Release/NVIDIA.PhysX.UnityEditor.pdb " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Editor/"
        }
