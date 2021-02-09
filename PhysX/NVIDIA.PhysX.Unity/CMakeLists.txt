cmake_minimum_required(VERSION 3.14)

project(NVIDIA.PhysX.Unity VERSION 1.0 LANGUAGES CSharp)

set(CMAKE_CSharp_FLAGS "/platform:anycpu")

file(GLOB_RECURSE PROJECT_SOURCES RELATIVE "${CMAKE_CURRENT_LIST_DIR}" *.cs)

add_library(${PROJECT_NAME} SHARED ${PROJECT_SOURCES})
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_TARGET_FRAMEWORK_VERSION "v4.6.1")
set_property(TARGET ${PROJECT_NAME} PROPERTY VS_DOTNET_REFERENCES "System" "System.Core" "../../External/UnityEngine.dll")
add_dependencies(${PROJECT_NAME} NVIDIA.PhysX)

target_compile_options(${PROJECT_NAME} PRIVATE "/unsafe")

add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ../../../Bin/${PROJECT_NAME}.dll
                                                     COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ../../../Unity/PhysX/Assets/NVIDIA/PhysX/Runtime/${PROJECT_NAME}.dll)
