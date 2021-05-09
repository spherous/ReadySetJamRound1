using System;
using UnityEngine;

public class Segment : MonoBehaviour
{
    public Vector3 lastPosition;
    public Quaternion lastRotation;
    public SpriteRenderer spriteRenderer;

    public Transform leader;
    public float lagSeconds;
    Vector3[] positionBuffer;
    Quaternion[] rotationBuffer;
    float[] timeBuffer;
    int oldestIndex;
    int newestIndex;

    public bool follow = true;

    private void FixedUpdate()
    {
        if(lagSeconds == 0)
        {
            transform.position = leader.position;
            transform.rotation = leader.rotation;
            return;
        }

        if(follow)
        {
            int newIndex = (newestIndex + 1) % positionBuffer.Length;
            if(newIndex != oldestIndex)
                newestIndex = newIndex;

            timeBuffer[newestIndex] = Time.time;
            float targetTime = Time.time - lagSeconds;
            int nextIndex;
            while(timeBuffer[nextIndex = (oldestIndex + 1) % timeBuffer.Length] < targetTime)
                oldestIndex = nextIndex;

            float span = timeBuffer[nextIndex] - timeBuffer[oldestIndex];
            float progress = span > 0f
                ? (targetTime - timeBuffer[oldestIndex]) / span
                : 0f;
                
            UpdatePosition(nextIndex, progress);
            UpdateRotation(nextIndex, progress);
        }
    }

    private void UpdateRotation(int nextIndex, float progress)
    {
        rotationBuffer[newestIndex] = leader.rotation;
        if(rotationBuffer[oldestIndex].IsValid() && rotationBuffer[oldestIndex].IsValid())
            transform.rotation = Quaternion.Lerp(rotationBuffer[oldestIndex], rotationBuffer[nextIndex], progress);
    }

    private void UpdatePosition(int nextIndex, float progress)
    {
        positionBuffer[newestIndex] = leader.position;
        transform.position = Vector3.Lerp(positionBuffer[oldestIndex], positionBuffer[nextIndex], progress);
    }

    private void OnDrawGizmos()
    {
        if(positionBuffer == null || positionBuffer.Length == 0)
            return;
        
        Gizmos.color = Color.green;

        Vector3 oldPosition = positionBuffer[oldestIndex];
        int next;
        for(int i = oldestIndex; i != newestIndex; i = next)
        {
            next = (i + 1) % positionBuffer.Length;
            Vector3 newPosition = positionBuffer[next];
            Gizmos.DrawLine(oldPosition, newPosition);
            oldPosition = newPosition;
        }
    }

    public void SetLag(float seconds)
    {
        lagSeconds = seconds;
        if(lagSeconds == 0)
            return;
           
        int bufferLength = Mathf.CeilToInt(lagSeconds * 60);
        bool updateIndices = positionBuffer == null;

        UpdateBuffer<Vector3>(ref positionBuffer, new Vector3[bufferLength]);
        UpdateBuffer<Quaternion>(ref rotationBuffer, new Quaternion[bufferLength]);
        UpdateBuffer<float>(ref timeBuffer, new float[bufferLength]);

        if(updateIndices)
        {
            oldestIndex = 0;
            newestIndex = 1;
            positionBuffer[0] = positionBuffer[1] = leader.position;
            timeBuffer[0] = timeBuffer[1] = Time.time;
        }
    }

    public void UpdateBuffer<T>(ref T[] orgBuffer, T[] newBuffer)
    {
        if(orgBuffer == null)
            orgBuffer = newBuffer;
        else if(newBuffer.Length > orgBuffer.Length)
        {
            Array.Copy(orgBuffer, newBuffer, orgBuffer.Length);
            orgBuffer = newBuffer;
        }
    }
}