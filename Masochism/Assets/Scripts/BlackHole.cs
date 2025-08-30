using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Black Hole Basics
// -----------------
// 1. Position: the center of the black hole in world space
// 2. Mass (geometric units): M = r_s / 2 if using G=c=1
// 3. Schwarzschild radius: r_s = 2 * M
//    - This defines the event horizon (point of no return)
// 4. Photon sphere: r_ph = 1.5 * r_s
//    - Where light can orbit the black hole, useful for visuals
// 5. Gravity: use Newtonian-like approximation or integrate geodesics
//    - For geometric units, simple acceleration ~ M / r^2 works for test objects
// 6. Scaling notes:
//    - Unity units don't have to be meters; pick a scale that looks good
//    - Keep numbers small to avoid floating point precision issues
// 7. Simulation control:
//    - Optional: step physics only on demand to avoid freezing the scene
// 8. Visuals:
//    - Event horizon: black sphere of radius r_s
//    - Photon ring: emissive ring at r_ph
//    - Optional lensing: radial warp/shader around black hole

public class BlackHole : MonoBehaviour
{
    [Header("Black Hole Properties")]
    public float schwarzschildRadius = 50f;
    public float mass { get; private set; }

    [Header("Object Spawning")]
    public GameObject objectPrefab;
    public float objAccel = 20f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Camera mainCamera;

    void Start()
    {
        mass = schwarzschildRadius / 2f;
        mass *= 1000f;
        mainCamera = Camera.main;
        
        if (objectPrefab == null)
        {
            Debug.LogWarning("No object prefab assigned to BlackHole script!");
        }
    }

    void Update()
    {
        HandleInput();
        CleanupDestroyedObjects();
    }

    void FixedUpdate()
    {
        ApplyGravitationalForces();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnObject();
        }
    }

    void SpawnObject()
    {
        if (objectPrefab == null || mainCamera == null) return;

        Vector3 mousePos = mainCamera.ScreenToWorldPoint(new Vector3(
            Input.mousePosition.x, 
            Input.mousePosition.y, 
            10f
        ));

        GameObject obj = Instantiate(objectPrefab, mousePos, Quaternion.identity);
        obj.tag = "Object";

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(Vector3.forward * objAccel, ForceMode.Impulse);
            spawnedObjects.Add(obj);
        }
        else
        {
            Debug.LogWarning($"Spawned object {obj.name} has no Rigidbody component!");
            Destroy(obj);
        }
    }

    void ApplyGravitationalForces()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj == null) continue;

            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb == null) continue;

            Vector3 toBlackHole = transform.position - obj.transform.position;
            float distance = toBlackHole.magnitude;

            // Avoid division by zero and extreme forces at very close distances
            if (distance < 0.1f) distance = 0.1f;

            Vector3 gravitationalAccel = toBlackHole.normalized * (mass / (distance * distance));
            
            // Apply the gravitational force
            rb.velocity += gravitationalAccel * Time.fixedDeltaTime;

            // Optional: Debug info (comment out for performance)
             Debug.Log($"Distance: {distance:F2}, Accel: {gravitationalAccel.magnitude:F4}");
        }
    }

    void CleanupDestroyedObjects()
    {
        // Remove null references (destroyed objects) from our list
        spawnedObjects.RemoveAll(obj => obj == null);
    }

    // Optional: Gizmos for visualizing the black hole in scene view
    void OnDrawGizmosSelected()
    {
        // Draw event horizon
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, schwarzschildRadius);
        
        // Draw photon sphere
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, schwarzschildRadius * 1.5f);
    }
}