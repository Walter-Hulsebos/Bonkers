namespace Bonkers
{
    using Bonkers.Shared;

    using JetBrains.Annotations;

    using UnityEngine;
    
    using F32 = System.Single;
    
    #if UNITY_EDITOR
    using UnityEditor;
    #endif
    
    #if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
    #endif
    
    public sealed class SpawnPoint : Multiton<SpawnPoint>
    {
        #if ODIN_INSPECTOR
        [BoxGroup]
        #endif
        [SerializeField] private F32 radius = 3;
        
        #if ODIN_INSPECTOR
        [InlineEditor]
        #endif
        [field:SerializeField] public  Team Team { get; set; }
        
        #if UNITY_EDITOR
        //Add menu in editor to spawn one from the hierarchy.
        [MenuItem(itemName: "GameObject/Bonkers/Spawn Point")]
        private static void CreateSpawnPoint(MenuCommand menuCommand)
        {
            GameObject __spawnPoint = new (name: "Spawn Point");
            __spawnPoint.AddComponent<SpawnPoint>();
            
            GameObjectUtility.SetParentAndAlign(child: __spawnPoint, parent: menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(objectToUndo: __spawnPoint, name: "Create " + __spawnPoint.name);
            Selection.activeObject = __spawnPoint;
        }
        
        private Vector3? randomPointOnSpawnCircleDebug;
        private F32      randomPointOnSpawnCircleDebugTime;
        
        
        #if ODIN_INSPECTOR
        [Button]
        #else
        [ContextMenu(itemName: "Debug Random Point On Spawn")]
        #endif
        private void DebugRandomPointOnSpawn()
        {
            randomPointOnSpawnCircleDebug     = RandomPointOnSpawnCircle();
            randomPointOnSpawnCircleDebugTime = Time.time;
        }
        
        private void OnDrawGizmos()
        {
            DrawDisc();
            DrawSpawnDebug();
            DrawScalingKnob();
        }

        private void DrawDisc()
        {
            Color32 __teamColor = (Team != null) ? Team.Color : Color.white; 
            
            Vector3 __position = transform.position;
            Vector3 __normal   = Vector3.up;
            
            Color32 __centerColor = __teamColor;
            __centerColor.a = 50;
            
            Color32 __borderColor = __teamColor;
            __borderColor.a = 255;

            Handles.color = __centerColor;
            Handles.DrawSolidDisc(center: __position, normal: __normal, radius: radius);
            
            Handles.color = __borderColor;
            Handles.DrawWireDisc( center: __position, normal: __normal, radius: radius, thickness: 5);
            
            GUIStyle __style = new() { alignment = TextAnchor.MiddleCenter, richText = true, fontSize = 20, normal = { textColor = __teamColor, }, };
            Handles.Label(position: __position, text: (Team != null) ? Team.name : "No Team", style: __style);
        }

        private void DrawSpawnDebug()
        {
            if (randomPointOnSpawnCircleDebug == null) return;
            
            if (randomPointOnSpawnCircleDebugTime + 1 <= Time.time)
            {
                randomPointOnSpawnCircleDebug = null;
                return;
            }

            Handles.color = Color.white;
            
            Vector3 __randomPoint = randomPointOnSpawnCircleDebug.Value;
            Gizmos.DrawSphere(center: __randomPoint, radius: 0.3f);
        }

        private void DrawScalingKnob()
        {
            Vector3 __handlePosition = transform.position + Vector3.right * radius;
            
            // Draw a handle to scale the radius.
            EditorGUI.BeginChangeCheck();
            __handlePosition = Handles.Slider(position: __handlePosition, direction: Vector3.right, size: 0.25f, snap: 0, capFunction: Handles.DotHandleCap);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(objectToUndo: this, name: "Change Radius");
                radius = Vector3.Distance(a: transform.position, b: __handlePosition);
                EditorUtility.SetDirty(target: this);
            }
        }

        #endif
        
        [PublicAPI]
        public Vector3 RandomPointOnSpawnCircle()
        {
            Vector2 __random   = Random.insideUnitCircle * radius;
            Vector3 __spawnPos = transform.position + new Vector3(x: __random.x, y: 0, z: __random.y);
            return __spawnPos;
        }
    }
}