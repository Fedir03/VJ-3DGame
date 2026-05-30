using UnityEngine;

public class SlimeMeshAnimator : MonoBehaviour
{
    public Mesh[] frames;
    public float framesPerSecond = 24f;
    public bool loop = true;

    private MeshFilter meshFilter;
    private float timer;
    private int currentFrame;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        if (frames.Length > 0)
        {
            meshFilter.mesh = frames[0];
        }
    }

    void Update()
    {
        if (frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameInterval = 1f / framesPerSecond;

        if (timer >= frameInterval)
        {
            timer -= frameInterval;
            currentFrame++;

            if (currentFrame >= frames.Length)
            {
                if (loop)
                {
                    currentFrame = 0;
                }
                else
                {
                    currentFrame = frames.Length - 1;
                }
            }

            meshFilter.mesh = frames[currentFrame];
        }
    }
}