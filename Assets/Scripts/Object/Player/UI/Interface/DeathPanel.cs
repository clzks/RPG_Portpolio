using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DeathPanel : MonoBehaviour, IPointerClickHandler
{
    private ObjectPoolManager _objectPool;
    public Image background;
    public Text deathText;
    bool isReady = false;

    private void Awake()
    {
        _objectPool = ObjectPoolManager.Get();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        _objectPool.ReturnAllObject();
        SceneManager.LoadScene("MainScene");
    }

    private void Update()
    {
        if(background.color.a < 0.6)
        {
            background.color = new Color(background.color.r, background.color.g, background.color.b, background.color.a + Time.deltaTime * 0.5f);
        }

        if(deathText.color.a < 1f)
        {
            deathText.color = new Color(deathText.color.r, deathText.color.g, deathText.color.b, deathText.color.a + Time.deltaTime * 0.6f);
            isReady = true;
        }
    }
}
