using System;
using Unity.Collections;
using Unity.Mathematics;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;

namespace ProjectDawn.Collections.LowLevel.Unsafe
{
    /// <summary>
    /// An unmanaged, resizable list that is presented in memory as structure of arrays, without any thread safety check features.
    /// Structure of arrays (SoA) is a layout separating elements of a record into one parallel array per field.
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("ArrayCount = {ArrayCount}, Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct UnsafeStructureList : IDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        void* m_DataPtr;
        int m_Length;
        int m_Capacity;

        [NativeDisableUnsafePtrRestriction]
        int* m_HeaderPtr;
        int m_ArrayCount;
        int* m_ArraySizes;
        byte** m_ArrayPointers;
        int m_Size;

        AllocatorManager.AllocatorHandle m_Allocator;

        /// <summary>
        /// The number of arrays.
        /// </summary>
        /// <value>The number of arrays.</value>
        public int ArrayCount => m_ArrayCount;

        /// <summary>
        /// The number of elements.
        /// </summary>
        /// <value>The number of elements.</value>
        public int Length => m_Length;

        /// <summary>
        /// The number of elements that fit in the current allocation.
        /// </summary>
        /// <value>The number of elements that fit in the current allocation.</value>
        public int Capacity => m_Capacity;

        /// <summary>
        /// Whether the list is empty.
        /// </summary>
        /// <value>True if the list is empty or the list has not been constructed.</value>
        public bool IsEmpty => m_Length == 0;

        /// <summary>
        /// Whether this list has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this queue has been allocated (and not yet deallocated).</value>
        public bool IsCreated => m_DataPtr != null;

        /// <summary>
        /// Creates a new container with the specified initial capacity and type of memory allocation.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list. If the list grows larger than its capacity,
        /// the internal array is copied to a new, larger array.</param>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        public static UnsafeStructureList* Create(int initialCapacity, NativeArray<int> sizes, AllocatorManager.AllocatorHandle allocator)
        {
            UnsafeStructureList* data = AllocatorManager.Allocate<UnsafeStructureList>(allocator);

            data->m_ArrayCount = 0;
            data->m_HeaderPtr = null;
            data->m_ArraySizes = null;
            data->m_ArrayPointers = null;
            data->m_Size = 0;

            data->m_DataPtr = null;
            data->m_Length = 0;
            data->m_Capacity = 0;
            data->m_Allocator = allocator;

            data->CreateHeader(sizes);

            data->SetCapacity(initialCapacity);

            return data;
        }

        /// <summary>
        /// Destroys container.
        /// </summary>
        public static void Destroy(UnsafeStructureList* data)
        {
            CollectionChecks.CheckNull(data);
            var allocator = data->m_Allocator;
            data->Dispose();
            AllocatorManager.Free(allocator, data);
        }

        /// <summary>
        /// Initializes and returns an instance of UnsafeStructureList.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list.</param>
        /// <param name="allocator">The allocator to use.</param>
        public UnsafeStructureList(int initialCapacity, NativeArray<int> sizes, AllocatorManager.AllocatorHandle allocator)
        {
            m_ArrayCount = 0;
            m_HeaderPtr = null;
            m_ArraySizes = null;
            m_ArrayPointers = null;
            m_Size = 0;

            m_DataPtr = null;
            m_Length = 0;
            m_Capacity = 0;
            m_Allocator = allocator;

            CreateHeader(sizes);

            SetCapacity(initialCapacity);
        }

        /// <summary>
        /// Returns unsafe pointer to array at index.
        /// </summary>
        /// <typeparam name="T">The type of the array.</typeparam>
        /// <param name="index">The index of array.</param>
        /// <returns>Returns unsafe pointer to array at index.</returns>
        public T* GetUnsafeArrayPointer<T>(int index) where T : unmanaged
        {
            CollectionChecks.CheckIndexInRange(index, m_ArrayCount);
            CheckTypeSizeMatch<T>(index);
            return (T*)m_ArrayPointers[index];
        }

        /// <summary>
        /// Sets the length, expanding the capacity if necessary.
        /// </summary>
        /// <param name="length">The new length.</param>
        /// <param name="options">Whether newly allocated bytes should be zeroed out.</param>
        public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            var oldLength = m_Length;

            // Handle case if there is not enough capacity to resize
            if (length > m_Capacity)
            {
                SetCapacity(length);
            }

            m_Length = length;

            // Handle clear logic
            if (options == NativeArrayOptions.ClearMemory && oldLength < length)
            {
                var num = length - oldLength;
                for (int attributeIndex = 0; attributeIndex < m_ArrayCount; ++attributeIndex)
                {
                    byte* ptr = m_ArrayPointers[attributeIndex];
                    var sizeOf = m_ArraySizes[attributeIndex];
                    UnsafeUtility.MemClear(ptr + oldLength * sizeOf, num * sizeOf);
                }
            }
        }

        /// <summary>
        /// Sets the capacity.
        /// </summary>
        /// <param name="capacity">The new capacity.</param>
        public void SetCapacity(int capacity)
        {
            CollectionChecks.CheckCapacityInRange(capacity, m_Length);

            var sizeOf = m_Size;
            var newCapacity = math.max(capacity, 64 / sizeOf);
            newCapacity = math.ceilpow2(newCapacity);

            if (newCapacity == m_Capacity)
            {
                return;
            }

            Realloc(m_Allocator, capacity);
        }

        /// <summary>
        /// Removes all elements of this queue.
        /// </summary>
        /// <remarks>Does not change the capacity.</remarks>
        public void Clear()
        {
            m_Length = 0;
        }

        /// <summary>
        /// Releases all resources (memory and safety handles).
        /// </summary>
        public void Dispose()
        {
            if (CollectionChecks.ShouldDeallocate(m_Allocator))
            {
                AllocatorManager.Free(m_Allocator, m_DataPtr, m_Size, m_Size, m_Capacity);
                AllocatorManager.Free(m_Allocator, m_HeaderPtr, m_ArrayCount);
                m_Allocator = AllocatorManager.Invalid;
            }

            m_DataPtr = null;
            m_Length = 0;
            m_Capacity = 0;

            m_HeaderPtr = null;
            m_ArraySizes = null;
            m_ArrayPointers = null;
        }

        void CreateHeader(NativeArray<int> sizes)
        {
            m_ArrayCount = sizes.Length;

            int sizeOfHeader = sizeof(IntPtr) + sizeof(int);
            m_HeaderPtr = (int*)AllocatorManager.Allocate(m_Allocator, sizeOfHeader, sizeOfHeader, m_ArrayCount);

            m_ArraySizes = (int*)m_HeaderPtr;
            m_ArrayPointers = (byte**)(m_ArraySizes + m_ArrayCount);

            m_Size = 0;
            for (int attributeIndex = 0; attributeIndex < sizes.Length; ++attributeIndex)
            {
                int size = sizes[attributeIndex];
                m_ArraySizes[attributeIndex] = size;
                m_Size += size;
            }
        }

        void Realloc(AllocatorManager.AllocatorHandle allocator, int newCapacity)
        {
            void* newPointer = null;

            var alignOf = m_Size;
            var sizeOf = m_Size;

            if (newCapacity > 0)
            {
                newPointer = AllocatorManager.Allocate(allocator, sizeOf, alignOf, newCapacity);

                if (m_Capacity > 0)
                {
                    byte* src = (byte*)m_DataPtr;
                    byte* dst = (byte*)newPointer;
                    for (int index = 0; index < m_ArrayCount; ++index)
                    {
                        // Copy array to new array
                        var itemsToCopy = math.min(newCapacity, m_Capacity);
                        var bytesToCopy = itemsToCopy * m_ArraySizes[index];
                        UnsafeUtility.MemCpy(dst, src, bytesToCopy);

                        // Update array pointer
                        m_ArrayPointers[index] = dst;

                        // Offset for next array
                        src += m_ArraySizes[index] * m_Capacity;
                        dst += m_ArraySizes[index] * newCapacity;
                    }
                }
                else
                {
                    byte* dst = (byte*)newPointer;
                    for (int index = 0; index < m_ArrayCount; ++index)
                    {
                        // Update array pointer
                        m_ArrayPointers[index] = dst;

                        // Offset for next array
                        dst += m_ArraySizes[index] * newCapacity;
                    }
                }
            }

            /*for (int index = 0; index < m_ArrayCount; ++index)
            {
                UnityEngine.Debug.Log($"{index} {(IntPtr)m_ArrayPointers[index]}");
            }*/

            AllocatorManager.Free(allocator, m_DataPtr, sizeOf, alignOf, m_Capacity);

            m_DataPtr = newPointer;
            m_Capacity = newCapacity;
            m_Length = math.min(m_Length, newCapacity);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        void CheckTypeSizeMatch<T>(int index) where T : unmanaged
        {
            int arraySize = m_ArraySizes[index];
            int typeSize = sizeof(T);
            if (typeSize < arraySize)
                throw new InvalidOperationException($"Type {typeof(T).Name} size is {typeSize} missmatch the size array was created at index {index} with size {arraySize}! ");
        }
    }
}
