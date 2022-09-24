using UnityEngine;

public class Background : MonoBehaviour
{
    public float scrollSpeed =1;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 offset = meshRenderer.material.mainTextureOffset;

        offset.Set(offset.x, offset.y + scrollSpeed * Time.deltaTime);

        meshRenderer.material.mainTextureOffset = offset;
    }
}
