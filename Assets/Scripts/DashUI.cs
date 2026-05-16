using UnityEngine;
using UnityEngine.UI;

public class DashUI : MonoBehaviour
{
    [Header("UI References")]
    public Image cooldownFill;
    public CanvasGroup canvasGroup; // Optional: to fade out when ready

    public void UpdateDashUI(float progress)
    {
        if (cooldownFill != null)
        {
            cooldownFill.fillAmount = progress;
        }

        if (canvasGroup != null)
        {
            // Simple visual feedback: slightly transparent while on cooldown
            canvasGroup.alpha = progress < 1f ? 0.5f : 1f;
        }
    }
}
