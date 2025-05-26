using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class DisplayButton : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text continueText;
    [SerializeField] bool isTarget;
    [SerializeField] Button button;
    [SerializeField] Sprite plusSign;

    public ImageSO imageData;

    //---Set icon images when game starts
    public void SetImageData(ImageSO data)
    {
        imageData = data;
        image.sprite = data.image;
        if (isTarget)
        {
            button.enabled = false;
            continueText.enabled = false;
        }
    }

    // Set to plus sign
    public void ResetImage()
    {
        if (isTarget) image.sprite = plusSign;
    }

    // Reactivate button 
    public void ResetButton()
    {
        if (isTarget)
        {
            button.enabled = true;
            continueText.enabled = true;
        }
    }

    public void SetToNullImage()
    {
        if (isTarget) image.sprite = null;
    }
}
