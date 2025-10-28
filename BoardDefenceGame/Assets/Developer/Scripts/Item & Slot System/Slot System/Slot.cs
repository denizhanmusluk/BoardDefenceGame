using System.Collections;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [Header("References")]
    public SlotPanel slotPanel;
    public Item currentItem;
    public SpriteRenderer spriteBG;

    [Header("Settings")]
    public int slotID = 0;

    [Header("Colors")]
    public Color transparentColor;
    public Color normalColor;
    public Color selectableColor;
    public Color noneSelectableColor;
    public Color fullColor;

    #region Color State Methods

    public void SelectableColorInit()
    {
        if (spriteBG != null)
            spriteBG.color = selectableColor;
    }

    public void NormalColorInit()
    {
        if (spriteBG != null)
            spriteBG.color = normalColor;

        if (currentItem != null && currentItem.itemAnimation != null)
            currentItem.itemAnimation.SetBool("shake", false);
    }

    public void TransparentColorInit()
    {
        StopAllCoroutines();
        StartCoroutine(TransparentDelay());
    }

    private IEnumerator TransparentDelay()
    {
        yield return new WaitForSeconds(0.1f);
        if (spriteBG != null)
            spriteBG.color = transparentColor;
    }

    #endregion

    #region Panel Interaction

    public void SlotPanelReset()
    {
        if (SlotManager.Instance != null)
            SlotManager.Instance.SlotPanelReset(slotID);
    }

    #endregion
}
