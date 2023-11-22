using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.AI;

namespace Bonkers
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] private float currentHealth;
        [Header("Variables")]
        [SerializeField] private float maxHealth;
        public GameObject[] locations;
        public NavMeshAgent enemyAgent;
        public states state;
        private Timer attackCooldown;

        private Rigidbody rb;
        private float percentageDamage;
        private float knockbackForce;

        public float distance = 10;
        public float angle = 30;
        public float height = 10.0f;
        public UnityEngine.Color meshColor = UnityEngine.Color.red;
        public int scanFrequency = 30;
        public LayerMask layers;
        public LayerMask occlusionLayers;
        public List<GameObject> objects = new List<GameObject>();
        public int segments;


        private Collider[] colliders = new Collider[5];
        private int count;
        private float scanInterval;
        private float scanTimer;
        private Mesh mesh;

        private int posNumber;
        private GameObject nextPoint;

        private RaycastHit rayHit;

        // Start is called before the first frame update
        public virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
            attackCooldown = new Timer();
            currentHealth = maxHealth;
            nextPoint = locations[0];
            state = states.Wandering;
            scanInterval = 1.0f / scanFrequency;

        }

        Mesh CreateWedgeMesh()
        {
            Mesh mesh = new Mesh();
            int numTriangles = (segments * 4) + 2 + 2;
            int numvertices = numTriangles * 3;

            Vector3[] vertices = new Vector3[numvertices];
            int[] triangles = new int[numvertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;

            int vert = 0;

            //Left Side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            //Right Side
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2) / segments;

            for (int i = 0; i < segments; i++)
            {

                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;

                //far side
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                //Top side
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                //Bottom
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }

            for (int i = 0; i < numvertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private void OnValidate()
        {
            mesh = CreateWedgeMesh();
            scanInterval = 1.0f / scanFrequency;
        }

        private void OnDrawGizmos()
        {
            if (mesh)
            {
                Gizmos.color = meshColor;
                Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
            }

            Gizmos.DrawWireSphere(transform.position, distance);
            for (int i = 0; i < count; i++)
            {
                Gizmos.DrawWireSphere(colliders[i].transform.position, 1f);
            }
            Gizmos.color = UnityEngine.Color.green;
            foreach (var obj in objects)
            {
                Gizmos.DrawSphere(obj.transform.position, 1f);
            }
        }


        public bool IsInSight(GameObject obj)
        {
            Vector3 origin = transform.position;
            Vector3 destination = obj.transform.position;
            Vector3 direction = destination - origin;

            if (direction.y > height)
            {
                return false;
            }
            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);
            if (deltaAngle > angle)
            {
                return false;
            }
            return true;
        }

        public void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
            objects.Clear();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = colliders[i].gameObject;
                if (IsInSight(obj))
                {
                    objects.Add(obj);
                    state = states.Attacking;
                }
            }

        }

        // Update is called once per frame
        public virtual void Update()
        {
            //Reset if target isnt in sight
            if (objects.Count <= 0)
            {
                if (state == states.Attacking)
                {
                    state = states.Wandering;
                }
            }

            Scan();
            MoveAI();

            if (!attackCooldown.isActive)
            {
                attackCooldown.SetTimer(1);
            }

            if (attackCooldown.isActive && attackCooldown.TimerDone())
            {
                attackCooldown.RestartTimer();
            }
        }


        public void FixedUpdate()
        {
            if (Physics.Raycast(gameObject.transform.position, Vector3.down, out rayHit, 10f, LayerMask.NameToLayer("Ground"), QueryTriggerInteraction.Collide))
            {
                if (rayHit.transform.gameObject.layer != LayerMask.NameToLayer("Ground"))
                {
                    print(rayHit.transform.gameObject.layer + " | Layer");
                    if (gameObject.GetComponent<Rigidbody>().isKinematic)
                    {
                        gameObject.GetComponent<Rigidbody>().isKinematic = false;
                        gameObject.GetComponent<Rigidbody>().useGravity = true;
                        gameObject.GetComponent<NavMeshAgent>().enabled = false;
                    }
                }
            }




        }

        public void MoveAI()
        {
            switch (state)
            {
                case states.Wandering:

                    enemyAgent.speed = 3f;
                    enemyAgent.SetDestination(nextPoint.transform.position);

                    if (enemyAgent.transform.position.x == nextPoint.transform.position.x && enemyAgent.transform.position.z == nextPoint.transform.position.z)
                    {
                        posNumber++;

                        if (posNumber >= locations.Length)
                        {
                            posNumber = 0;
                        }
                        nextPoint = locations[posNumber];
                    }

                    break;
                case states.Attacking:

                    for (int i = 0; i < objects.Count; i++)
                    {
                        if (objects[i].tag.Equals("Player"))
                        {
                            if (CheckIfValidVision(transform.position, objects[i].transform.position))
                            {
                                enemyAgent.SetDestination(objects[i].transform.position);

                                if (Vector3.Distance(objects[i].transform.position, gameObject.transform.position) <= 5)
                                {
                                    if (attackCooldown.isActive && attackCooldown.TimerDone())
                                    {
                                        objects[i].transform.GetComponent<CampaignPlayer>().SetKnockBackPercentage(1f, gameObject.transform.position);
                                    }
                                }
                            }
                        }
                    }
                    enemyAgent.speed = 7f;

                    //Add attacks within certain distance.

                    break;
                default:
                    break;
            }

        }

        public bool CheckIfValidVision(Vector3 fromPosition, Vector3 toPosition)
        {
            if (Physics.Linecast(fromPosition, toPosition, occlusionLayers))
            {
                return false;
            }
            return true;
        }

        public void TakeDamage(float value)
        {
            float calcValue = value * 0.1f;
            percentageDamage += calcValue;
            knockbackForce = percentageDamage;

            if (rb != null)
            {
                Vector3 direction = CampaignManager.instance.player.transform.position - transform.position;
                rb.AddForce(direction * knockbackForce, ForceMode.VelocityChange);
            }
            print("Enemy took damage");
        }

        public void Die()
        {
            Destroy(gameObject);
        }

        //   public void SetCurrentHealth(float value)
        //  {
        //     currentHealth = value;
        //  }

        //  public float GetCurrentHealth()
        //{
        //   return currentHealth;
        //}
        public enum states
        {
            Wandering,
            Attacking,
        }
    }
}
