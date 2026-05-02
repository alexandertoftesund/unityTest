using UnityEngine;
using UnityEngine.UI;

public class MuteButton : MonoBehaviour
{
    public Image iconImage;
    public Sprite soundOn;
    public Sprite soundOff;

    bool muted = false;

    public void ToggleMute()
    {
        muted = !muted;

        AudioListener.volume = muted ? 0f : 1f;

        iconImage.sprite = muted ? soundOff : soundOn;
    }
}