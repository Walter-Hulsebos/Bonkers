using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using ProjectDawn.Collections;
using System.Diagnostics;
using UnityEngine.Rendering;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using ProjectDawn.Geometry3D.LowLevel.Unsafe;

namespace ProjectDawn.Geometry3D
{
    /// <summary>
    /// An managed, resizable vertex data.
    /// Stores generic vertex information in interleaved array.
    /// As example if structure is created with Position+Normal attributes as result data will be stored in memory as follows: position0/normal0/position1/normal1...
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct VertexData : INativeDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        UnsafeVertexData* m_Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
#if REMOVE_DISPOSE_SENTINEL
#else
        [NativeSetClassTypeToNullOnSchedule]
        internal DisposeSentinel m_DisposeSentinel;
#endif
#endif

        /// <summary>
        /// Vertex attribute flags used for creating this vertex data.
        /// </summary>
        public VertexAttributes Flags
        {
            get
            {
                return m_Data->Flags;
            }
        }

        /// <summary>
        /// Size of the single vertex.
        /// It is a sum of all attributes size.
        /// </summary>
        public int Size
        {
            get
            {
                return m_Data->Size;
            }
        }

        /// <summary>
        /// The count of elements.
        /// </summary>
        /// <value>The current count of elements. Always less than or equal to the capacity.</value>
        /// <remarks>To decrease the memory used by a list, set <see cref="Capacity"/> after reducing the length of the list.</remarks>
        /// <param name="value>">The new length. If the new length is greater than the current capacity, the capacity is increased.
        /// Newly allocated memory is cleared.</param>
        public int Length
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_Data->Length;
            }

            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
                m_Data->Resize(value, NativeArrayOptions.ClearMemory);
            }
        }

        /// <summary>
        /// The number of elements that fit in the current allocation.
        /// </summary>
        /// <value>The number of elements that fit in the current allocation.</value>
        /// <param name="value">The new capacity. Must be greater or equal to the length.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new capacity is smaller than the length.</exception>
        public int Capacity
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_Data->Capacity;
            }

            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
                m_Data->SetCapacity(value);
            }
        }

        /// <summary>
        /// Whether the vertex data is empty.
        /// </summary>
        /// <value>True if the list is empty or the list has not been constructed.</value>
        public bool IsEmpty => !IsCreated || Length == 0;

        /// <summary>
        /// Whether this the vertex data has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this the vertex data has been allocated (and not yet deallocated).</value>
        public bool IsCreated => m_Data != null;

        public VertexData(int initialCapacity, NativeArray<VertexAttributeDescriptor> attributes, Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CollectionChecks.CheckCapacity(initialCapacity);
#if REMOVE_DISPOSE_SENTINEL
            m_Safety = AtomicSafetyHandle.Create();
#else
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 2, allocator);
#endif
#endif

            m_Data = UnsafeVertexData.Create(initialCapacity, attributes, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.SetBumpSecondaryVersionOnScheduleWrite(m_Safety, true);
#endif
        }

        /// <summary>
        /// Returns the internal unsafe vertex data.
        /// </summary>
        /// <remarks>Internally, the elements of a VertexData are stored in an UnsafeVertexData.</remarks>
        /// <returns>The internal unsafe vertex data.</returns>
        public UnsafeVertexData* GetUnsafeVertexData() => m_Data;

        /// <summary>
        /// Returns vertex position at index.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <returns>Returns vertex position at index.</returns>
        public float3 GetVertexAt(int index)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return *(float3*)m_Data->ElementPointerAt(index);
        }

        /// <summary>
        /// Returns a reference to the element at an index.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <returns>A reference to the element at the index.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown if index is out of bounds.</exception>
        public ref T ElementAt<T>(int index) where T : unmanaged
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
            CollectionChecks.CheckReinterpret<T>(Size);
            return ref UnsafeUtility.AsRef<T>(m_Data->ElementPointerAt(index));
        }

        /// <summary>
        /// Returns a native array that aliases the array at the index.
        /// </summary>
        /// <returns>A native array that aliases the content of this list.</returns>
        public NativeArray<T> AsArray<T>() where T : unmanaged
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);
            var arraySafety = m_Safety;
            AtomicSafetyHandle.UseSecondaryVersion(ref arraySafety);
#endif
            CollectionChecks.CheckReinterpret<T>(Size);
            var ptr = m_Data->GetUnsafePointer();
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, Length, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, arraySafety);
#endif
            return array;
        }

        /// <summary>
        /// Returns a native array of attributes used to create this vertex data structure.
        /// </summary>
        /// <returns>Returns a native array of attributes used to create this vertex data structure.</returns>
        public NativeArray<VertexAttributeDescriptor> GetVertexAttributes()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            var arraySafety = AtomicSafetyHandle.GetTempUnsafePtrSliceHandle();
#endif
            var ptr = m_Data->GetVertexAttributesPointer();
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<VertexAttributeDescriptor>(ptr, m_Data->AttributesLength, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, arraySafety);
#endif
            return array;
        }

        /// <summary>
        /// Sets the length of this vertex data, increasing the capacity if necessary.
        /// </summary>
        /// <param name="length">The new length of this vertex data.</param>
        /// <param name="options">Whether to clear any newly allocated bytes to all zeroes.</param>
        public void Resize(int length, NativeArrayOptions options)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            m_Data->Resize(length, options);
        }

        /// <summary>
        /// Sets the length of this list, increasing the capacity if necessary.
        /// </summary>
        /// <remarks>Does not clear newly allocated bytes.</remarks>
        /// <param name="length">The new length of this list.</param>
        public void ResizeUninitialized(int length)
        {
            Resize(length, NativeArrayOptions.UninitializedMemory);
        }

        /// <summary>
        /// Sets the capacity.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        public void SetCapacity(int capacity)
        {
            m_Data->SetCapacity(capacity);
        }

        /// <summary>
        /// Clears the container.
        /// </summary>
        /// <remarks>VertexData Capacity remains unchanged.</remarks>
        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_Data->Clear();
        }

        /// <summary>
        /// Creates and schedules a job that releases all resources (memory and safety handles) of this vertex data.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and releases all resources (memory and safety handles) of this vertex data.</returns>
        public JobHandle Dispose(JobHandle inputDeps)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if REMOVE_DISPOSE_SENTINEL
#else
            // [DeallocateOnJobCompletion] is not supported, but we want the deallocation
            // to happen in a thread. DisposeSentinel needs to be cleared on main thread.
            // AtomicSafetyHandle can be destroyed after the job was scheduled (Job scheduling
            // will check that no jobs are writing to the container).
            DisposeSentinel.Clear(ref m_DisposeSentinel);
#endif

            var job = new VertexDataDisposeJob { VertexData = m_Data, m_Safety = m_Safety };

#else
            var job = new VertexDataDisposeJob { VertexData = m_Data };
            
#endif
            m_Data = null;

            return job.Schedule(inputDeps);
        }

        /// <summary>
        /// Releases all resources (memory and safety handles).
        /// </summary>
        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
#if REMOVE_DISPOSE_SENTINEL
            AtomicSafetyHandle.Release(m_Safety);
#else
            DisposeSentinel.Dispose(ref m_Safety, ref m_DisposeSentinel);
#endif
#endif
            UnsafeVertexData.Destroy(m_Data);
            m_Data = null;
        }
    }

    [BurstCompile]
    unsafe struct VertexDataDisposeJob : IJob
    {
        [NativeDisableUnsafePtrRestriction]
        public UnsafeVertexData* VertexData;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
#endif

        public void Execute()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.Release(m_Safety);
#endif
            UnsafeVertexData.Destroy(VertexData);
        }
    }
}
