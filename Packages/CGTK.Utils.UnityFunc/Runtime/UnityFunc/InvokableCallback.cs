namespace CGTK.Utils.UnityFunc
{
    using System;

    public class InvokableCallback<TReturn> : InvokableCallbackBase<TReturn>
    {
        public readonly Func<TReturn> func;

        public TReturn Invoke() => func();

        public override TReturn Invoke(params Object[] args) => func();

        /// <summary> Constructor </summary>
        public InvokableCallback(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                func = () => default(TReturn);
            }
            else
            {
                func = (Func<TReturn>) Delegate.CreateDelegate(type: typeof(Func<TReturn>), target: target, method: methodName);
            }
        }
    }

    public class InvokableCallback<T0, TReturn> : InvokableCallbackBase<TReturn>
    {
        public readonly Func<T0, TReturn> func;

        public TReturn Invoke(T0 arg0) => func(arg: arg0);

        public override TReturn Invoke(params Object[] args)
        {
            // Convert from special "unity-nulls" to true null
            if (args[0] is UnityEngine.Object && (UnityEngine.Object) args[0] == null) { args[0] = null; }

            return func(arg: (T0) args[0]);
        }

        /// <summary> Constructor </summary>
        public InvokableCallback(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                func = x => default(TReturn);
            }
            else
            {
                func = (Func<T0, TReturn>) Delegate.CreateDelegate(type: typeof(Func<T0, TReturn>), target: target, method: methodName);
            }
        }
    }

    public class InvokableCallback<T0, T1, TReturn> : InvokableCallbackBase<TReturn>
    {
        public readonly Func<T0, T1, TReturn> func;

        public TReturn Invoke(T0 arg0, T1 arg1) => func(arg1: arg0, arg2: arg1);

        public override TReturn Invoke(params Object[] args)
        {
            // Convert from special "unity-nulls" to true null
            if (args[0] is UnityEngine.Object && (UnityEngine.Object) args[0] == null) { args[0] = null; }

            if (args[1] is UnityEngine.Object && (UnityEngine.Object) args[1] == null) { args[1] = null; }

            return func(arg1: (T0) args[0], arg2: (T1) args[1]);
        }

        /// <summary> Constructor </summary>
        public InvokableCallback(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                func = (x, y) => default(TReturn);
            }
            else
            {
                func = (Func<T0, T1, TReturn>) Delegate.CreateDelegate(type: typeof(Func<T0, T1, TReturn>), target: target, method: methodName);
            }
        }
    }

    public class InvokableCallback<T0, T1, T2, TReturn> : InvokableCallbackBase<TReturn>
    {
        public readonly Func<T0, T1, T2, TReturn> func;

        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2) => func(arg1: arg0, arg2: arg1, arg3: arg2);

        public override TReturn Invoke(params Object[] args)
        {
            // Convert from special "unity-nulls" to true null
            if (args[0] is UnityEngine.Object && (UnityEngine.Object) args[0] == null) { args[0] = null; }

            if (args[1] is UnityEngine.Object && (UnityEngine.Object) args[1] == null) { args[1] = null; }

            if (args[2] is UnityEngine.Object && (UnityEngine.Object) args[2] == null) { args[2] = null; }

            return func(arg1: (T0) args[0], arg2: (T1) args[1], arg3: (T2) args[2]);
        }

        /// <summary> Constructor </summary>
        public InvokableCallback(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                func = (x, y, z) => default(TReturn);
            }
            else
            {
                func = (Func<T0, T1, T2, TReturn>) Delegate.CreateDelegate(type: typeof(Func<T0, T1, T2, TReturn>), target: target, method: methodName);
            }
        }
    }

    public class InvokableCallback<T0, T1, T2, T3, TReturn> : InvokableCallbackBase<TReturn>
    {
        public readonly Func<T0, T1, T2, T3, TReturn> func;

        public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) => func(arg1: arg0, arg2: arg1, arg3: arg2, arg4: arg3);

        public override TReturn Invoke(params Object[] args)
        {
            // Convert from special "unity-nulls" to true null
            if (args[0] is UnityEngine.Object && (UnityEngine.Object) args[0] == null) { args[0] = null; }

            if (args[1] is UnityEngine.Object && (UnityEngine.Object) args[1] == null) { args[1] = null; }

            if (args[2] is UnityEngine.Object && (UnityEngine.Object) args[2] == null) { args[2] = null; }

            if (args[3] is UnityEngine.Object && (UnityEngine.Object) args[3] == null) { args[3] = null; }

            return func(arg1: (T0) args[0], arg2: (T1) args[1], arg3: (T2) args[2], arg4: (T3) args[3]);
        }

        /// <summary> Constructor </summary>
        public InvokableCallback(Object target, String methodName)
        {
            if (target == null || String.IsNullOrEmpty(value: methodName))
            {
                func = (x, y, z, w) => default(TReturn);
            }
            else
            {
                func = (Func<T0, T1, T2, T3, TReturn>) Delegate.CreateDelegate(type: typeof(Func<T0, T1, T2, T3, TReturn>), target: target, method: methodName);
            }
        }
    }
}