using UnityEngine;
using UnityEngine.EventSystems;

using DefenceGameSystem.OS.Kernel;

public class CustomButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IInputableObject
{
    public KeyType type;
    private bool m_isPressed;

    public bool BindOnAwake = false;

    protected virtual void Awake()
    {
        if(BindOnAwake)
            Bind(type);
    }

    public virtual void Bind(KeyType key)
    {
        type = key;
        InputModule.AddButton(this, key);
    }

    public virtual void OnPointerDown(PointerEventData data)
    {
        InputModule.SetKeyDown(type);
        
        m_isPressed = true;
    }

    public virtual void OnPointerUp(PointerEventData data)
    {
        if(m_isPressed)
            InputModule.SetKeyUp(type);

        m_isPressed = false;
    }

    public virtual void OnPointerExit(PointerEventData data)
    {
        if(m_isPressed)
            InputModule.SetKeyUp(type);

        m_isPressed = false;
    }
}