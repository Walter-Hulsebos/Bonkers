using System;
using Unity.Collections;
using Unity.Jobs;
using Unity.Collections.LowLevel.Unsafe;
using ProjectDawn.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Unity.Burst;

namespace ProjectDawn.Collections
{
    /// <summary>
    /// An unmanaged, resizable list that is presented in memory as structure of arrays, without any thread safety check features.
    /// Structure of arrays (SoA) is a layout separating elements of a record into one parallel array per field.
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerDisplay("ArrayCount = {ArrayCount}, Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct NativeStructureList
        : INativeDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        UnsafeStructureList* m_Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
#if REMOVE_DISPOSE_SENTINEL
#else
        [NativeSetClassTypeToNullOnSchedule]
        internal DisposeSentinel m_DisposeSentinel;
#endif
#endif

        /// <summary>
        /// The number of arrays.
        /// </summary>
        /// <value>The number of arrays.</value>
        public int ArrayCount
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
                return m_Data->ArrayCount;
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
        /// Whether the list is empty.
        /// </summary>
        /// <value>True if the list is empty or the list has not been constructed.</value>
        public bool IsEmpty => !IsCreated || Length == 0;

        /// <summary>
        /// Whether this list has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this queue has been allocated (and not yet deallocated).</value>
        public bool IsCreated => m_Data != null;

        /// <summary>
        /// Initializes and returns an instance of NativeStructureList.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        /// <param name="allocator">The allocator to use.</param>
        public NativeStructureList(int initialCapacity, NativeArray<int> sizes, Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CollectionChecks.CheckCapacity(initialCapacity);
#if REMOVE_DISPOSE_SENTINEL
            m_Safety = AtomicSafetyHandle.Create();
#else
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 2, allocator);
#endif
#endif

            m_Data = UnsafeStructureList.Create(initialCapacity, sizes, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.SetBumpSecondaryVersionOnScheduleWrite(m_Safety, true);
#endif
        }

        /// <summary>
        /// Returns the internal unsafe structure array.
        /// </summary>
        /// <remarks>Internally, the elements of a UnsafeStructureArray are stored in an UnsafeStructureArray.</remarks>
        /// <returns>The internal unsafe vertex data.</returns>
        public UnsafeStructureList* GetUnsafeStructureArray() => m_Data;

        /// <summary>
        /// Returns a native array that aliases the array at the index.
        /// </summary>
        /// <returns>A native array that aliases the content of this list.</returns>
        public NativeArray<T> AsArray<T>(int index) where T : unmanaged
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckGetSecondaryDataPointerAndThrow(m_Safety);
            var arraySafety = m_Safety;
            AtomicSafetyHandle.UseSecondaryVersion(ref arraySafety);
#endif
            var ptr = m_Data->GetUnsafeArrayPointer<T>(index);
            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(ptr, Length, Allocator.None);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, arraySafety);
#endif
            return array;
        }

        /// <summary>
        /// Sets the length of this list, increasing the capacity if necessary.
        /// </summary>
        /// <param name="length">The new length of this list.</param>
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
        /// Removes all elements of this queue.
        /// </summary>
        /// <remarks>Does not change the capacity.</remarks>
        public void Clear()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_Data->Clear();
        }

        /// <summary>
        /// Creates and schedules a job that frees the memory of this list.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and frees the memory of this list.</returns>
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

            var job = new DisposeJob { List = m_Data, m_Safety = m_Safety };

#else
            var job = new DisposeJob { List = m_Data };
            
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
            UnsafeStructureList.Destroy(m_Data);
            m_Data = null;
        }

        [BurstCompile]
        unsafe struct DisposeJob : IJob
        {
            [NativeDisableUnsafePtrRestriction]
            public UnsafeStructureList* List;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal AtomicSafetyHandle m_Safety;
#endif

            public void Execute()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.Release(m_Safety);
#endif
                UnsafeStructureList.Destroy(List);
            }
        }
    }
}
