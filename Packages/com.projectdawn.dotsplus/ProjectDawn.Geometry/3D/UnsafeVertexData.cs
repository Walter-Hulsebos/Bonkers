using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using ProjectDawn.Collections;
using System.Diagnostics;
using UnityEngine.Rendering;
using Unity.Collections.LowLevel.Unsafe;

namespace ProjectDawn.Geometry3D.LowLevel.Unsafe
{
    /// <summary>
    /// An managed, resizable vertex data, without any thread safety check features.
    /// Stores generic vertex information in interleaved array.
    /// As example if structure is created with Position+Normal attributes as result data will be stored in memory as follows: position0/normal0/position1/normal1...
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    [DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}")]
    public unsafe struct UnsafeVertexData : IDisposable
    {
        [NativeDisableUnsafePtrRestriction]
        void* m_DataPtr;
        int m_Length;
        int m_Capacity;
        VertexAttributes m_Flags;
        int m_Size;

        [NativeDisableUnsafePtrRestriction]
        VertexAttributeDescriptor* m_AttributePtr;
        int m_AttributeLength;

        AllocatorManager.AllocatorHandle m_Allocator;

        /// <summary>
        /// Vertex attribute count.
        /// </summary>
        public int AttributesLength => m_AttributeLength;

        /// <summary>
        /// Vertex attribute mask used for creating this vertex data.
        /// </summary>
        public VertexAttributes Flags => m_Flags;

        /// <summary>
        /// Size of the single vertex.
        /// It is a sum of all attributes size.
        /// </summary>
        public int Size => m_Size;

        /// <summary>
        /// The count of elements.
        /// </summary>
        /// <value>The current count of elements. Always less than or equal to the capacity.</value>
        /// <remarks>To decrease the memory used by a list, set <see cref="Capacity"/> after reducing the length of the list.</remarks>
        /// <param name="value>">The new length. If the new length is greater than the current capacity, the capacity is increased.
        /// Newly allocated memory is cleared.</param>
        public int Length => m_Length;

        /// <summary>
        /// The number of elements that fit in the current allocation.
        /// </summary>
        /// <value>The number of elements that fit in the current allocation.</value>
        /// <param name="value">The new capacity. Must be greater or equal to the length.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the new capacity is smaller than the length.</exception>
        public int Capacity => m_Capacity;

        /// <summary>
        /// Whether the vertex data is empty.
        /// </summary>
        /// <value>True if the list is empty or the list has not been constructed.</value>
        public bool IsEmpty => !IsCreated || Length == 0;

        /// <summary>
        /// Whether this the vertex data has been allocated (and not yet deallocated).
        /// </summary>
        /// <value>True if this the vertex data has been allocated (and not yet deallocated).</value>
        public bool IsCreated => m_DataPtr != null;

        /// <summary>
        /// Creates a new container with the specified initial capacity and type of memory allocation.
        /// </summary>
        /// <param name="initialCapacity">The initial capacity of the list. If the list grows larger than its capacity,
        /// the internal array is copied to a new, larger array.</param>
        /// <param name="allocator">A member of the
        /// [Unity.Collections.Allocator](https://docs.unity3d.com/ScriptReference/Unity.Collections.Allocator.html) enumeration.</param>
        public static UnsafeVertexData* Create(int initialCapacity, NativeArray<VertexAttributeDescriptor> attributes, AllocatorManager.AllocatorHandle allocator)
        {
            CheckAttributes(attributes);

            UnsafeVertexData* data = AllocatorManager.Allocate<UnsafeVertexData>(allocator);

            data->m_DataPtr = null;
            data->m_Length = 0;
            data->m_Capacity = 0;
            data->m_Allocator = allocator;

            data->m_AttributePtr = null;
            data->m_AttributeLength = 0;

            data->m_Flags = VertexAttributes.None;
            data->m_Size = 0;

            data->CreateHeader(attributes);

            data->SetCapacity(initialCapacity);

            return data;
        }

        /// <summary>
        /// Destroys container.
        /// </summary>
        public static void Destroy(UnsafeVertexData* data)
        {
            CollectionChecks.CheckNull(data);
            var allocator = data->m_Allocator;
            data->Dispose();
            AllocatorManager.Free(allocator, data);
        }

        public UnsafeVertexData(int initialCapacity, NativeArray<VertexAttributeDescriptor> attributes, AllocatorManager.AllocatorHandle allocator)
        {
            CheckAttributes(attributes);

            m_DataPtr = null;
            m_Length = 0;
            m_Capacity = 0;
            m_Allocator = allocator;

            m_AttributePtr = null;
            m_AttributeLength = 0;

            m_Flags = VertexAttributes.None;
            m_Size = 0;

            CreateHeader(attributes);

            SetCapacity(initialCapacity);
        }

        /// <summary>
        /// Returns pointer to data.
        /// </summary>
        /// <returns>Returns pointer to data.</returns>
        public void* GetUnsafePointer() => m_DataPtr;

        /// <summary>
        /// Returns pointer to attributes.
        /// </summary>
        /// <returns>Returns pointer to attributes.</returns>
        public VertexAttributeDescriptor* GetVertexAttributesPointer() => m_AttributePtr;

        /// <summary>
        /// Returns pointer to element at index.
        /// </summary>
        /// <param name="index">An index.</param>
        /// <returns>Returns pointer to element at index.</returns>
        public void* ElementPointerAt(int index)
        {
            CollectionChecks.CheckIndexInRange(index, m_Length);
            return (byte*)m_DataPtr + m_Size * index;
        }

        /// <summary>
        /// Sets the length of this vertex data, increasing the capacity if necessary.
        /// </summary>
        /// <param name="length">The new length of this vertex data.</param>
        /// <param name="options">Whether to clear any newly allocated bytes to all zeroes.</param>
        public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
        {
            var oldLength = m_Length;

            if (length > m_Capacity)
            {
                SetCapacity(length);
            }

            m_Length = length;

            if (options == NativeArrayOptions.ClearMemory && oldLength < length)
            {
                var num = length - oldLength;
                byte* ptr = (byte*)m_DataPtr;
                var sizeOf = m_Size;
                UnsafeUtility.MemClear(ptr + oldLength * sizeOf, num * sizeOf);
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
        /// Clears the container.
        /// </summary>
        /// <remarks>Stack Capacity remains unchanged.</remarks>
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
                AllocatorManager.Free(m_Allocator, m_AttributePtr, m_AttributeLength);
                m_Allocator = AllocatorManager.Invalid;
            }

            m_DataPtr = null;
            m_AttributePtr = null;
            m_Length = 0;
            m_Capacity = 0;
            m_Size = 0;
        }

        void CreateHeader(NativeArray<VertexAttributeDescriptor> attributes)
        {
            int sizeOf = sizeof(VertexAttributeDescriptor);
            int alignOf = UnsafeUtility.AlignOf<VertexAttributeDescriptor>();
            m_AttributePtr = (VertexAttributeDescriptor*)AllocatorManager.Allocate(m_Allocator, sizeOf, alignOf, attributes.Length);
            m_AttributeLength = attributes.Length;

            for (int attributeIndex = 0; attributeIndex < attributes.Length; ++attributeIndex)
            {
                var attribute = attributes[attributeIndex];
                m_AttributePtr[attributeIndex] = attribute;
                m_Flags |= attribute.attribute.ToFlag();
                m_Size += attribute.format.SizeOf() * attribute.dimension;
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
                    var itemsToCopy = math.min(newCapacity, Capacity);
                    var bytesToCopy = itemsToCopy * sizeOf;
                    UnsafeUtility.MemCpy(newPointer, m_DataPtr, bytesToCopy);
                }
            }

            AllocatorManager.Free(allocator, m_DataPtr, sizeOf, alignOf, m_Capacity);

            m_DataPtr = newPointer;
            m_Capacity = newCapacity;
            m_Length = math.min(m_Length, newCapacity);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        static void CheckAttributes(NativeArray<VertexAttributeDescriptor> attributes)
        {
            if (!attributes.IsCreated || attributes.Length == 0)
                throw new InvalidOperationException("Attributes array must be created and non zero length!");
        }
    }
}