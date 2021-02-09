Build
-----

- Get PhysX 4.1 from GitHub (https://github.com/NVIDIAGameWorks/PhysX),

- In <PhysX Folder>\physx\buildtools\presets\public\vc15win64.xml change PX_GENERATE_STATIC_LIBRARIES to True.

- Build PhysX and create PHYSX_DIR environment variable pointng to its root.

- Install CMake 3.14+ (https://cmake.org/) and SWIG 4.0+ (http://www.swig.org/) and add them to your PATH environment variable.

- Run .\generate_projects.cmd file.

- Open and build .\Build\NVIDIA.PhysX.sln solution in Visual Studio 2017.

Run
---

- Start Unity and open .\Unity\PhysX project.