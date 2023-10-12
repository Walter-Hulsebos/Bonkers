using ProjectDawn.Geometry3D;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class CreateIcosphereMesh : MonoBehaviour
{
    public Sphere Sphere = Sphere.Unit;
    [Range(1, 6)]
    public int Subdivision = 1;

    void Update()
    {
        var attributes = new NativeArray<VertexAttributeDescriptor>(1, Allocator.Temp);
        attributes[0] = new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3);

        var surface = new MeshSurface(1, attributes, Allocator.TempJob);

        JobHandle dependency = surface.Icosphere(Sphere, Subdivision, default);

        if (TryGetComponent(out MeshFilter meshFilter))
        {
            var meshDataArray = Mesh.AllocateWritableMeshData(1);

            dependency = surface.Write(meshDataArray[0], MeshUpdateFlags.Default, dependency);

            dependency.Complete();

            if (meshFilter.sharedMesh == null)
                meshFilter.sharedMesh = new Mesh();
            Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, meshFilter.sharedMesh);
        }

        attributes.Dispose();
        surface.Dispose();
    }
}
