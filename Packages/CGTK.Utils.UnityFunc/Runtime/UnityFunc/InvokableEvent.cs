namespace CGTK.Utils.UnityFunc
{
    using System;

    public class InvokableEvent : InvokableEventBase
    {
        public readonly Action action;

        public void Invoke() { action(); }

        public override void Invoke(params Object[] args) { action(); }

        /// <summary> Constructor </summary>
        public InvokableEvent(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                action = () => { };
            }
            else
            {
                action = (Action) Delegate.CreateDelegate(type: typeof(Action), target: target, method: methodName);
            }
        }
    }

    public class InvokableEvent<T0> : InvokableEventBase
    {
        public readonly Action<T0> action;

        public void Invoke(T0 arg0) { action(obj: arg0); }

        public override void Invoke(params Object[] args) { action(obj: (T0) args[0]); }

        /// <summary> Constructor </summary>
        public InvokableEvent(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                action = x => { };
            }
            else
            {
                action = (Action<T0>) Delegate.CreateDelegate(type: typeof(Action<T0>), target: target, method: methodName);
            }
        }
    }

    public class InvokableEvent<T0, T1> : InvokableEventBase
    {
        public readonly Action<T0, T1> action;

        public void Invoke(T0 arg0, T1 arg1) { action(arg1: arg0, arg2: arg1); }

        public override void Invoke(params Object[] args) { action(arg1: (T0) args[0], arg2: (T1) args[1]); }

        /// <summary> Constructor </summary>
        public InvokableEvent(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                action = (x, y) => { };
            }
            else
            {
                action = (Action<T0, T1>) Delegate.CreateDelegate(type: typeof(Action<T0, T1>), target: target, method: methodName);
            }
        }
    }

    public class InvokableEvent<T0, T1, T2> : InvokableEventBase
    {
        public readonly Action<T0, T1, T2> action;

        public void Invoke(T0 arg0, T1 arg1, T2 arg2) { action(arg1: arg0, arg2: arg1, arg3: arg2); }

        public override void Invoke(params Object[] args) { action(arg1: (T0) args[0], arg2: (T1) args[1], arg3: (T2) args[2]); }

        /// <summary> Constructor </summary>
        public InvokableEvent(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                action = (x, y, z) => { };
            }
            else
            {
                action = (Action<T0, T1, T2>) Delegate.CreateDelegate(type: typeof(Action<T0, T1, T2>), target: target, method: methodName);
            }
        }
    }

    public class InvokableEvent<T0, T1, T2, T3> : InvokableEventBase
    {
        public readonly Action<T0, T1, T2, T3> action;

        public void Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) { action(arg1: arg0, arg2: arg1, arg3: arg2, arg4: arg3); }

        public override void Invoke(params Object[] args) { action(arg1: (T0) args[0], arg2: (T1) args[1], arg3: (T2) args[2], arg4: (T3) args[3]); }

        /// <summary> Constructor </summary>
        public InvokableEvent(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                action = (x, y, z, w) => { };
            }
            else
            {
                action = (Action<T0, T1, T2, T3>) Delegate.CreateDelegate(type: typeof(Action<T0, T1, T2, T3>), target: target, method: methodName);
            }
        }
    }
}