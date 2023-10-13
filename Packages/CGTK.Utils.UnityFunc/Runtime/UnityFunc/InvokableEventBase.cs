namespace CGTK.Utils.UnityFunc
{
    using System;

    public abstract class InvokableEventBase
    {
        public abstract void Invoke(params Object[] args);
    }
}