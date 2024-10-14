using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[RequireComponent(typeof(Mesh))]
[RequireComponent(typeof(MeshRenderer))]
public class CCloth : MonoBehaviour
{
    ClothPhysics clothPhysics;
    ClothRender clothRender;

    [SerializeField]
    [Range(0.1f, 100.0f)]
    private float mass = 1.0f;

    [SerializeField]
    [Range(0,1)]
    private float springK = 1.0f;

    [SerializeField]
    [Range(0,1)]
    private float damping = 0.01f;

    [SerializeField]
    private float timestep = 0.004f;

    [SerializeField]
    private List<int> pinnedVertices;

    [SerializeField]
    private List<CollisionConstraint> collisionConstraints;

    [SerializeField]
    private List<SmartStaticCollisionConstraint> ssCollisionConstraints;

    [SerializeField]
    private int collisionConstraintSteps = 1;

    [SerializeField]
    private int substeps = 1;

    // DEBUGGING.
    [SerializeField]
    private bool drawConstraints = true;

    [SerializeField]
    private bool drawDynamics = true;

    [SerializeField]
    private bool drawCollisionConstraints = true;

    [SerializeField]
    private bool recordRuntime = true;

    private float avgTimePhysics = 0.0f;
    private float avgTimeRender = 0.0f;

    private Transform robotArmTransform; //reference to the robot arm transform

    void Start()
    {
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        WeldMesh.Weld(mesh, 0.000001f, true);
        float fullMass = this.mass * mesh.vertices.Length;
        this.clothRender = new ClothRender(mesh, this.transform);
        Vector3[] worldVerts = this.clothRender.GetWorldVertices();
        int[] meshTriangles = mesh.triangles;
        this.clothPhysics = new ClothPhysics(
                            worldVerts, meshTriangles, fullMass, this.springK, 
                            Physics.gravity, 1.0f-this.damping, this.timestep/this.substeps, 
                            this.pinnedVertices,
                            this.collisionConstraints,
                            this.ssCollisionConstraints,
                            this.collisionConstraintSteps
                            );

        // Pin(0);

        robotArmTransform = GameObject.Find("DobotMagicianRobotArm").transform;
    }

    public void Pin(int index)
    {
        this.clothPhysics.points[index].mass = 0;
        this.clothPhysics.points[index].pinConstraintScalar = 0;
    }

    void FixedUpdate()
    {
        Stopwatch st = Stopwatch.StartNew();
        for(int i = 0; i < this.substeps; ++i)
            this.clothPhysics.PhysicsStep();
        st.Stop();

        avgTimePhysics = avgTimePhysics * 0.9f + st.ElapsedMilliseconds*0.1f;
    }

    public float dummySpeed = 0.10f;

    public void CustomInjectedCode()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 position = this.clothPhysics.points[0].position;
        position.x += h * Time.deltaTime * dummySpeed;
        position.z += v * Time.deltaTime * dummySpeed;
        this.clothPhysics.points[0].position = position;
    }

    void Update()
    {
        CustomInjectedCode();

        Stopwatch st = Stopwatch.StartNew();
        PointMass[] points = this.clothPhysics.points;
        Vector3[] pVerts = new Vector3[points.Length];
        for(int i = 0; i < pVerts.Length; ++i) // TODO: this copying could be avoided with pntrs.
            pVerts[i] = points[i].position;
        this.clothRender.UpdateVerticesFromPhysics(pVerts);
        st.Stop();

        avgTimeRender = avgTimeRender * 0.9f + st.ElapsedMilliseconds*0.1f;
       
        // if(this.recordRuntime)
        //     UnityEngine.Debug.Log(string.Format("physics: {0}ms; Render: {1}ms", avgTimePhysics, avgTimeRender));
    }

    //collision detection between the robot arm and the cloth
        // Collision detection between the robot arm and cloth
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "DobotMagicianRobotArm") // Ensure this matches the robot arm name
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Vector3 hitPoint = contact.point;
                PinClosestVertex(hitPoint);
            }
        }
    }

        // Find the closest cloth vertex to the hit point and pin it
    void PinClosestVertex(Vector3 hitPoint)
    {
        PointMass[] points = this.clothPhysics.points;
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(points[i].position, hitPoint);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        Pin(closestIndex); // Pin the closest vertex
    }

    
    void OnDrawGizmos()
    {
        if(UnityEditor.EditorApplication.isPlaying)
        {
            if(this.drawConstraints)
                DrawConstraints();
            if(this.drawDynamics)
                DrawDynamics();
        }

        if(this.drawCollisionConstraints)
        {
            DrawCollisionConstraints();
            DrawSSCollisionConstraints();
        }
    }

    void DrawConstraints()
    {
        HashSet<Constraint> constraints = this.clothPhysics.contraints;
        foreach(Constraint c in constraints)
        {
            UnityEngine.Debug.DrawLine(c.pA.position, c.pB.position, Color.red);
        }
    }

    void DrawDynamics()
    {
        foreach(PointMass p in this.clothPhysics.points)
        {
            UnityEngine.Debug.DrawLine(p.position, p.lastPosition, Color.blue);
        }
    }

    void DrawCollisionConstraints()
    {
        foreach(CollisionConstraint c in this.collisionConstraints)
        {
            c.Draw(Color.green);
        }
    }

    void DrawSSCollisionConstraints()
    {
        foreach(SmartStaticCollisionConstraint c in this.ssCollisionConstraints)
        {
            c.Draw(Color.yellow, Color.cyan);
        }
    }
}
