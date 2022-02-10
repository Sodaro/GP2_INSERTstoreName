using UnityEngine;

public class UIAudioHandler : MonoBehaviour
{
    public void PlayHoverSound()
    {
        AudioManager.Instance.PlaySFX("UI_Hover");
    }
    public void PlayClickSound()
    {
        AudioManager.Instance.PlaySFX("UI_Confirm");
    }

}
