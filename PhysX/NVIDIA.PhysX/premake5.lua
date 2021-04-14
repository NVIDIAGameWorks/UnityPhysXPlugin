project "NVIDIA.PhysX"
    language "C#"
    kind "SharedLib"
    clr "Unsafe"
    files "**.cs"
    dotnetframework "4.7.2"

    basedir = os.getcwd()

    links { "NVIDIA.PhysX.Native" }

    filter "configurations:Debug"
        defines { "DEBUG" }
        symbols "On"

        targetdir (basedir .. "/../Bin/Debug")

        postbuildcommands { 
            "{COPY} " .. basedir .. "/../Bin/Debug/NVIDIA.PhysX.dll " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Runtime/",

            "{COPY} " .. basedir .. "/../Bin/Debug/NVIDIA.PhysX.pdb " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Runtime/"
        }

    filter "configurations:Release"
        defines { "NDEBUG" }
        optimize "On"

        targetdir (basedir .. "/../Bin/Release")

        postbuildcommands { 
            "{COPY} " .. basedir .. "/../Bin/Release/NVIDIA.PhysX.dll " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Runtime/",

            "{COPY} " .. basedir .. "/../Bin/Release/NVIDIA.PhysX.pdb " .. basedir ..
            "/../Unity/PhysX/Assets/NVIDIA/PhysX/Runtime/"
        }