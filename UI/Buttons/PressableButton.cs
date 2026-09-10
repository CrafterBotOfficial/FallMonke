using UnityEngine;

namespace FallMonke.UI.Buttons;

public abstract class PressableButton : GorillaPressableButton
{
    public new virtual void Start()
    {
        base.Start();
        base.gameObject.layer = 18;
        pressedMaterial = GetComponent<Material>();
        unpressedMaterial = GetComponent<Material>();
    }
}
