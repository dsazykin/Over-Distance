using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("UI References")]
    public Slider healthSlider;
    public Image fillImage;
    
    [Header("Visual Settings")]
    public Color highHealthColor = Color.green;
    public Color lowHealthColor = Color.red;

    public void UpdateHealthUI(int current, int max)
    {
        if (healthSlider == null) return;

        float fillValue = (float)current / max;
        healthSlider.value = fillValue;

        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowHealthColor, highHealthColor, fillValue);
        }
    }
}
