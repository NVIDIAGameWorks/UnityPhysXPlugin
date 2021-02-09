cmake_minimum_required(VERSION 3.14)

project(NVIDIA.PhysX.UnityEditor VERSION 1.0 LANGUAGES CSharp)

set(CMAKE_CSharp_FLAGS "/platform:anycpu")

file(GLOB_RECURSE PROJECT_SOURCES RELATIVE "${CMAKE_CURRENT_LIST_DIR}" *.cs)

add_library(${PROJECT_NAME} SHARED ${PROJECT_SOURCES})
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.6.1")
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES "System" "System.Core" "../../External/UnityEngine.dll" "../../External/UnityEditor.dll")
add_dependencies(${PROJECT_NAME} NVIDIA.PhysX)
add_dependencies(${PROJECT_NAME} NVIDIA.PhysX.Unity)

add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ../../../Bin/${PROJECT_NAME}.dll
                                                     COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ../../../Unity/PhysX/Assets/NVIDIA/PhysX/Editor/${PROJECT_NAME}.dll)
