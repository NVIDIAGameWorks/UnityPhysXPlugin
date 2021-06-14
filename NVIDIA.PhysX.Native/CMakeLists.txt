cmake_minimum_required(VERSION 3.14)

project(NVIDIA.PhysX.Native VERSION 1.0 LANGUAGES CXX)

#set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -std=c++1y -Wall -Wextra -Wno-unused-parameter -fPIC")

set(CMAKE_CXX_FLAGS_RELEASE "${CMAKE_CXX_FLAGS_RELEASE} /MT")
set(CMAKE_CXX_FLAGS_PROFILE "${CMAKE_CXX_FLAGS_PROFILE} /MT")
set(CMAKE_CXX_FLAGS_CHECKED "${CMAKE_CXX_FLAGS_CHECKED} /MT")
set(CMAKE_CXX_FLAGS_DEBUG "${CMAKE_CXX_FLAGS_DEBUG} /MTd")

add_definitions(-DPX_PHYSX_STATIC_LIB)

cmake_policy(SET CMP0086 NEW)
cmake_policy(SET CMP0078 NEW)

find_package(SWIG REQUIRED)
include(UseSWIG)

file(GLOB_RECURSE PROJECT_SOURCES RELATIVE "${CMAKE_CURRENT_LIST_DIR}" *.h *.hpp *.c *.cpp *.i *.cxx)

foreach(SOURCE IN LISTS PROJECT_SOURCES)
    get_filename_component(SOURCE_PATH "${SOURCE}" PATH)
    string(REPLACE "/" "//" SOURCE_PATH_MSVC "${SOURCE_PATH}")
    source_group("${SOURCE_PATH_MSVC}" FILES "${SOURCE}")
endforeach()

file(GLOB_RECURSE SWIG_SOURCES RELATIVE "${CMAKE_CURRENT_LIST_DIR}" *.i)

foreach(SOURCE IN LISTS SWIG_SOURCES)
    get_filename_component(SOURCE_PATH "${SOURCE}" PATH)
    set_property(SOURCE "${SOURCE}" PROPERTY CPLUSPLUS ON)
endforeach()

include_directories("${CMAKE_CURRENT_LIST_DIR}"
                    "${CMAKE_CURRENT_LIST_DIR}/Header"
                    "$ENV{PHYSX_DIR}/physx/include"
                    "$ENV{PHYSX_DIR}/pxshared/include")

link_directories("$ENV{PHYSX_DIR}/physx/bin/win.x86_64.vc141.mt/$(Configuration)")

set(CMAKE_SWIG_FLAGS ${CMAKE_SWIG_FLAGS} -namespace NVIDIA.PhysX -nodefaultctor)
set(CMAKE_SWIG_OUTDIR "${CMAKE_CURRENT_LIST_DIR}/../NVIDIA.PhysX/Wrapper")
set(SWIG_OUTFILE_DIR "${CMAKE_CURRENT_LIST_DIR}/Wrapper")

swig_add_library(${PROJECT_NAME} TYPE SHARED LANGUAGE csharp SOURCES ${PROJECT_SOURCES})

target_link_libraries(${PROJECT_NAME} PhysXFoundation_static_64
                                      PhysX_static_64
                                      PhysXCommon_static_64
                                      PhysXCooking_static_64
                                      PhysXExtensions_static_64
                                      PhysXPvdSDK_static_64)

add_custom_command(TARGET ${PROJECT_NAME} PRE_BUILD COMMAND ${CMAKE_COMMAND} -E remove_directory "${CMAKE_CURRENT_LIST_DIR}/../NVIDIA.PhysX/Wrapper")

add_custom_command(TARGET ${PROJECT_NAME} POST_BUILD COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> ../../Bin/${PROJECT_NAME}.dll
                                                     COMMAND ${CMAKE_COMMAND} -E copy $<TARGET_FILE:${PROJECT_NAME}> "../../Unity/PhysX\ Plugin/Assets/NVIDIA/PhysX/Runtime/Plugins/x86_64/${PROJECT_NAME}.dll")
