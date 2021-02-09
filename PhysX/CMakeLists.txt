cmake_minimum_required(VERSION 3.14)

project(NVIDIA.PhysX VERSION 1.0)

if(CMAKE_CONFIGURATION_TYPES)
	set(CMAKE_CONFIGURATION_TYPES Debug Release Profile Checked)
	set(CMAKE_CONFIGURATION_TYPES "${CMAKE_CONFIGURATION_TYPES}" CACHE STRING "Reset the configurations to what we need" FORCE)
	set(CMAKE_CXX_FLAGS_PROFILE "${CMAKE_CXX_FLAGS_RELEASE}")
	set(CMAKE_CXX_FLAGS_CHECKED "${CMAKE_CXX_FLAGS_RELEASE}")
endif()

add_subdirectory(NVIDIA.PhysX.Native)

add_subdirectory(NVIDIA.PhysX)

add_subdirectory(NVIDIA.PhysX.Unity)

add_subdirectory(NVIDIA.PhysX.UnityEditor)


