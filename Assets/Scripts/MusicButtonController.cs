using UnityEngine;
using UnityEngine.UI;

public class MusicButtonController : MonoBehaviour
{
    public Sprite musicOnSprite;
    public Sprite musicOffSprite;
    private Image buttonImage;

    private void Start()
    {
        buttonImage = GetComponent<Image>();
        UpdateButtonSprite();
    }

    public void OnButtonClick()
    {
        if (MusicController.instance != null)
        {
            MusicController.instance.ToggleMusic();
            UpdateButtonSprite();
        }
    }

    private void UpdateButtonSprite()
    {
        if (MusicController.instance.IsPlaying())
        {
            buttonImage.sprite = musicOnSprite;
        }
        else
        {
            buttonImage.sprite = musicOffSprite;
        }
    }

}
