using UnityEngine;
using VerletRope;

public class Simulador : MonoBehaviour
{

    [Header("Configuration")]
    [SerializeField] private float gravity = 9.8f;
    private int segments = 20;
    [SerializeField] private int iterations = 20;
    [SerializeField] private float length = 0.5f;

    [Header("Components")]
    [SerializeField] private LineRenderer ropeRenderer;
    [SerializeField] private Camera mainCamera;


    private Transform startTransform;
    private  VerletSimulator simulator;

    public void StringCreator(Transform startObj, int qtdSegments)
    {
        startTransform = startObj;
        segments = qtdSegments;
        simulator = new VerletSimulator(1f, iterations);
        Dot startDot = new Dot(startTransform.position, true);
        Dot lastDot = startDot;
        AddPositionToRenderer(ropeRenderer, lastDot.CurrentPosition);
        simulator.Dots.Add(lastDot);

        for (int i = 0; i < segments; i++)
        {
            Vector3 dotPosition = startTransform.position + Vector3.down * length * (i + 1);
            Dot newDot = new Dot(dotPosition, false);
            Dot.Connect(lastDot, newDot);
            simulator.Dots.Add(newDot);
            AddPositionToRenderer(ropeRenderer, newDot.CurrentPosition);
            lastDot = newDot;
        }
    }

    private void FixedUpdate()
    {
        simulator.AddForce(gravity * Vector3.down);

        Dot firstDot = simulator.Dots[0];
        firstDot.CurrentPosition = startTransform.position;
        simulator.Simulate(Time.fixedDeltaTime);

        for (int i = 0; i < simulator.Dots.Count; i++)
        {
            Dot dot = simulator.Dots[i];
            ropeRenderer.SetPosition(i, dot.CurrentPosition);
        }
    }

    private static void AddPositionToRenderer(LineRenderer lineRenderer, Vector3 position)
    {
        int positionCount = lineRenderer.positionCount;
        int index = positionCount;
        positionCount++;
        lineRenderer.positionCount = positionCount;
        lineRenderer.SetPosition(index, position);
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        Dot lastDot = simulator.Dots[0];
        Gizmos.color = !lastDot.isStatic ? Color.green : Color.red;
        Gizmos.DrawSphere(lastDot.CurrentPosition, 0.1f);

        for (int i = 1; i < simulator.Dots.Count; i++)
        {
            Dot currentDot = simulator.Dots[i];
            Gizmos.color = !currentDot.isStatic ? Color.green : Color.red;
            Gizmos.DrawSphere(currentDot.CurrentPosition, 0.1f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(currentDot.CurrentPosition, lastDot.CurrentPosition);
            lastDot = currentDot;
        }
    }
}
