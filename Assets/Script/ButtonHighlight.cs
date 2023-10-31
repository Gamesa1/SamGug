using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlight : MonoBehaviour
{
    public Button cityButton;
    public float highlightedScale = 1.2f; // 강조된 스케일
    private Vector3 originalScale;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Highlight()
    {
        // 버튼을 강조 (크기 변경)
        transform.localScale = originalScale * highlightedScale;
    }

    public void Unhighlight()
    {
        // 버튼의 강조 효과 제거 (크기 원래대로)
        transform.localScale = originalScale;
    }
}
