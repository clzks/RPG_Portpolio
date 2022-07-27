using UnityEngine;
using UnityEngine.UI;

public class DialogBlinker : MonoBehaviour
{
    public Image blinkerImage;
    public float time = 0.3f;
    private float timer = 0f;

    private void Update()
    {
        timer += Time.unscaledDeltaTime;
        
        if(timer >= time)
        {
            time = 0f;
            blinkerImage.enabled = !blinkerImage.enabled;
        }
    }
}
