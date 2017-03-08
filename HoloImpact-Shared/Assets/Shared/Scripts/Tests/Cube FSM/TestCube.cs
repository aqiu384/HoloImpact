using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TestCubeFSM))]
public class TestCube : MonoBehaviour
{
    public Vector3 minimizedScale = Vector3.one * 0.1f;
    public Vector3 maximizedScale = Vector3.one;
    public float transitionTime = 0.5f;

    private TestCubeFSM m_testCubeFSM;

    protected virtual void Awake()
    {
        m_testCubeFSM = GetComponent<TestCubeFSM>();

        m_testCubeFSM.AddOnEnterListener(() => StartCoroutine(GrowingCoroutine()), TestCubeState.Growing);
        m_testCubeFSM.AddOnEnterListener(() => StartCoroutine(ShrinkingCoroutine()), TestCubeState.Shrinking);
    }

    public IEnumerator GrowingCoroutine()
    {
        var startTime = Time.time;
        
        while (true)
        {
            var interpolant = (Time.time - startTime) / transitionTime;
            transform.localScale = Vector3.Lerp(minimizedScale, maximizedScale, interpolant);

            if (interpolant >= 1) break;
            yield return null;
        }
        
        m_testCubeFSM.OnTransition(TestCubeTransition.FinishedGrowing);
    }

    public IEnumerator ShrinkingCoroutine()
    {
        var startTime = Time.time;

        while (true)
        {
            var interpolant = (Time.time - startTime) / transitionTime;
            transform.localScale = Vector3.Lerp(maximizedScale, minimizedScale, interpolant);

            if (interpolant >= 1) break;
            yield return null;
        }

        m_testCubeFSM.OnTransition(TestCubeTransition.FinishedShrinking);
    }
}