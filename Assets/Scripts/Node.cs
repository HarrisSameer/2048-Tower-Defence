using UnityEngine;

public class Node : MonoBehaviour
{
    public Vector2 Pos => transform.position;
    public Block OccupiedBlock;
    public bool IsStatic { get; set; }

    private SpriteRenderer _visualRenderer;

    private void Awake()
    {
        var visual = transform.Find("Visual");
        if (visual != null)
        {
            _visualRenderer = visual.GetComponent<SpriteRenderer>();
            if (_visualRenderer == null)
            {
                Debug.LogError("SpriteRenderer component not found on 'Visual' child.");
            }
        }
        else
        {
            Debug.LogError("'Visual' child not found.");
        }
    }

    public void SetColor(Color color)
    {
        if (_visualRenderer != null)
        {
            _visualRenderer.color = color;
        }
    }
}
