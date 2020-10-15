using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    protected string tipTitle = null;
    protected string tipSubtitle = null;
    protected string tipContent = null;

    public virtual void UpdateShowing()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.tooltip.Show(tipTitle, tipSubtitle, tipContent);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.tooltip.Hide();
    }
}
