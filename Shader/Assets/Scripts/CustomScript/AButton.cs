using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class AButton : MonoBehaviour, IPointerEnterHandler
{
    private Button m_button = null;

    //==================================================

    private void Awake()
    {
        this.m_button = this.GetComponent<Button>();
    }

    private void OnEnable()
    {
        this.m_button.onClick.AddListener(this.OnPress);
    }

    private void OnDisable()
    {
        this.m_button.onClick.RemoveListener(this.OnPress);
    }

    //==================================================

    public virtual void OnPress() {  }

    public virtual void OnPointerEnter(PointerEventData eventData) { }

    //==================================================
}
