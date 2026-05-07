using System;


namespace LSH.Utils
{
    public interface IInteractable
    {
        event Action OnClick;
        event Action OnHover;
    }
}
