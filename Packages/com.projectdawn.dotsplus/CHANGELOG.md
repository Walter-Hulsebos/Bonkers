# Changelog
All notable changes to this package will be documented in this file. The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)

## [1.8.2] - 2023-08-03
- Added to geometry structures Equals(object) implementations
- Added Box and Box transformed overlap test
- Added Box and Triangle overlap test

## [1.8.1] - 2023-06-22
- Fixed UnsafeHeap and NativeHeapPriorityQueue correctly work with structure keys

## [1.8.0] - 2023-02-26
- Added support to be compatible with 2.0 collections

## [1.7.3] - 2023-02-15
- Added Dispose job to NativeLinkedList, NativeAABBTree, NativeStack, NativeKdTree, NativeLinkedPriorityQueue and NativeHeapPriorityQueue
- Added math overloads that uses double (Credit to Walter H.)

## [1.7.2] - 2023-01-4
- Fixed linked list clear to set length to correct value

## [1.7.1] - 2023-01-4
- Changed documentation from physical to webpage

## [1.7.0] - 2022-12-21
- Added NativeHeapPriorityQueue that uses heap
- Added NativeLinkedPriorityQueue that uses linked list
- Deprecated NativePriorityQueue should use now either NativeHeapPriorityQueue or NativeLinkedPriorityQueue

## [1.6.0] - 2022-10-23
- Added to AABBTree methods: CountLeafs, GetDepth, GetBalancedTreeFactor

## [1.5.0] - 2022-09-27
- Changed IVoronoiOutput.ProcessVertex signature
- Improved performance of DelaunayTriangulation drastically
- Added VertexData structure for processing vertex information
- Added MesSurface structure for processing/reading/writing Mesh
- Added box/icosphere/icohedron/icocapsule generation suing MeshSurface
- Added NativeStructureList/UnsafeStructureList for building SoA (structure of array)
- Added Plane structure (Exposed from Unity Collection package)
- Added 3d/2s Capsule structure

## [1.4.1] - 2022-09-21
- Fixed AABBTree failing compilation in player build

## [1.4.0] - 2022-08-25
- Changed geometry overlap/intersection not include borders
- Changed overlap logic to be more faster
- Added to geometry structures debug display
- Added AABBTree

## [1.3.0] - 2022-08-10
- Changed NativeLinkedList Add/Insert to return iterator
- Changed com.unity.burst dependency version from 1.3.0-preview.12 to 1.6.6
- Changed com.unity.mathematics dependency version from 1.1.0 to 1.2.1
- Changed com.unity.collections dependency version from 0.9.0-preview.6 to 1.4.0
- Added Sort/Middle/Count to NativeLinkedList
- Added EnqueueUnique to the NativePriorityQueue
- Added new helper class ShapeGizmos for drawing 2d geometry
- Added VoronoiBuilder/DelaunayTriangulation/VoronoiDiagram
- Added to math functions isclockwise/iscclockwise/iscollinear/sq/sort/line/curve/parabola/bazier/swap
- Added Voronoi sample
- Added Curves sample

## [1.2.0] - 2022-08-03
- Adding geometry 3d

## [1.1.0] - 2022-08-01
- Fixed compilation errors for 2021

## [1.0.0] - 2022-07-28
- Package released

