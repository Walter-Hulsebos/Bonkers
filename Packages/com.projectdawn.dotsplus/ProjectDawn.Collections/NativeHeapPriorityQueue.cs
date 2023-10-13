using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using ProjectDawn.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Burst;

namespace ProjectDawn.Collections
{
    /// <summary>
    /// An unmanaged, resizable priority queue.
    /// Priority queue main difference from regular queue that it is sorted.
    /// It is implemented using heap. Peek = O(1), Enqueue = O(log n), Dequeue = O(log n).
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the element.</typeparam>
    [StructLayout(LayoutKind.Sequential)]
    [NativeContainer]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct NativeHeapPriorityQueue<TKey, TValue>
        : INativeDisposable
        where TKey : unmanaged, IComparable<TKey>
        where TValue : unmanaged
    {
        [NativeDisableUnsafePtrRestriction]
        UnsafeHeap<TKey, TValue>* m_Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_Safety;
#if REMOVE_DISPOSE_SENTINEL
#else
        [NativeSetClassTypeToNullOnSchedule]
        internal DisposeSentinel m_DisposeSentinel;
#endif
#endif

        /// <summary>
        /// Whether the queue is empty.
        /// </summary>
        /// <value>True if the queue is empty or the queue has not been constructed.</value>
        public bool IsEmpty => !IsCreated || Length == 0;

        /// <summary>
        /// The number of elements.
        /// </summary>
        /// <value>The number of elements.</value>
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
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                m_Data->Length = value;
            }
        }

        /// <summary>
        /// The number of elements that fit in the current allocation.
        /// </summary>
        /// <value>The number of elements that fit in the current allocation.</value>
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
                AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif
                m_Data->Capacity = value;
            }
        }

        /// <summary>
        /// Whether this queue has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this queue has been allocated (and not yet deallocated).</value>
        public bool IsCreated => m_Data != null;

        /// <summary>
        /// Initialized and returns an instance of NativePriorityQueue.
        /// </summary>
        /// <param name="allocator">The allocator to use.</param>
        public NativeHeapPriorityQueue(Allocator allocator) : this(1, allocator) {}

        /// <summary>
        /// Initialized and returns an instance of NativePriorityQueue.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the priority queue.</param>
        /// <param name="allocator">The allocator to use.</param>
        public NativeHeapPriorityQueue(int initialCapacity, Allocator allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            CollectionChecks.CheckCapacity(initialCapacity);
            CollectionChecks.CheckIsUnmanaged<TValue>();
#if REMOVE_DISPOSE_SENTINEL
            m_Safety = AtomicSafetyHandle.Create();
#else
            DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 2, allocator);
#endif
#endif

            m_Data = UnsafeHeap<TKey, TValue>.Create(initialCapacity, allocator);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.SetBumpSecondaryVersionOnScheduleWrite(m_Safety, true);
#endif
        }

        /// <summary>
        /// Adds an element at the front of the queue.
        /// </summary>
        /// <param name="value">The value to be added.</param>
        public void Enqueue(in TKey key, in TValue value)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            m_Data->Push(key, value);
        }

        /// <summary>
        /// Returns the element at the end of this queue without removing it.
        /// </summary>
        /// <returns>The element at the end of this queue.</returns>
        public TValue Peek()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return m_Data->Peek();
        }

        /// <summary>
        /// Removes the element from the end of the queue.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the queue was empty.</exception>
        /// <returns>Returns the removed element.</returns>
        public TValue Dequeue()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            return m_Data->Pop();
        }

        /// <summary>
        /// Removes the element from the end of the queue.
        /// </summary>
        /// <remarks>Does nothing if the queue is empty.</remarks>
        /// <param name="item">Outputs the element removed.</param>
        /// <returns>True if an element was removed.</returns>
        public bool TryDequeue(out TValue value)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndBumpSecondaryVersion(m_Safety);
#endif
            return m_Data->TryPop(out value);
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
            UnsafeHeap<TKey, TValue>.Destroy(m_Data);
            m_Data = null;
        }

        /// <summary>
        /// Creates and schedules a job that releases all resources (memory and safety handles) of this queue.
        /// </summary>
        /// <param name="inputDeps">The dependency for the new job.</param>
        /// <returns>The handle of the new job. The job depends upon `inputDeps` and releases all resources (memory and safety handles) of this queue.</returns>
#if UNITY_COLLECTIONS_2_0_0
#else
        [NotBurstCompatible /* This is not burst compatible because of IJob's use of a static IntPtr. Should switch to IJobBurstSchedulable in the future */]
#endif
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

            var job = new DisposeJob { Data = m_Data, m_Safety = m_Safety };

#else
            var job = new DisposeJob { Data = m_Data };
            
#endif
            m_Data = null;

            return job.Schedule(inputDeps);
        }

        /// <summary>
        /// Returns an array with a copy of all this heap map's keys (in no particular order).
        /// </summary>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>An array with a copy of all this hash map's keys (in no particular order).</returns>
        public NativeArray<TKey> GetKeyArray(AllocatorManager.AllocatorHandle allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return m_Data->GetKeyArray(allocator);
        }

        /// <summary>
        /// Returns an array with a copy of all this heap map's values (in no particular order).
        /// </summary>
        /// <param name="allocator">The allocator to use.</param>
        /// <returns>An array with a copy of all this hash map's values (in no particular order).</returns>
        public NativeArray<TValue> GetValueArray(AllocatorManager.AllocatorHandle allocator)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif
            return m_Data->GetValueArray(allocator);
        }

        [BurstCompile]
        unsafe struct DisposeJob : IJob
        {
            [NativeDisableUnsafePtrRestriction]
            public UnsafeHeap<TKey, TValue>* Data;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            internal AtomicSafetyHandle m_Safety;
#endif

            public void Execute()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.Release(m_Safety);
#endif
                UnsafeHeap<TKey, TValue>.Destroy(Data);
            }
        }
    }
}