using UnityEngine;

[ExecuteAlways]
public class ColourAnimator : MonoBehaviour
{
    [SerializeField]
    private Gradient gradient;

    [SerializeField]
    private Material sphereMat;
    
    [SerializeField]
    private Light pointLight;

    [SerializeField]
    private float speed = 0.1f;

    private void Update()
    {
        float t = Mathf.Sin(Time.time * speed);
        t = (t + 1) / 2f;
        Color col = gradient.Evaluate(t);
        sphereMat.color = col;
        pointLight.color = col;
    }
}
