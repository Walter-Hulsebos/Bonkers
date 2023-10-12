using System;

namespace CGTK.Utils.UnityFunc
{
    public abstract class SerializableEventBase : UnityFuncBase
    {
        public InvokableEventBase invokable;

        public override void ClearCache()
        {
            base.ClearCache();
            invokable = null;
        }

        protected InvokableEventBase GetPersistentMethod()
        {
            Type[] types = new Type[ArgTypes.Length];
            Array.Copy(sourceArray: ArgTypes, destinationArray: types, length: ArgTypes.Length);

            Type genericType = types.Length switch
            {
                0 => typeof(InvokableEvent),
                1 => typeof(InvokableEvent<>).MakeGenericType(typeArguments: types),
                2 => typeof(InvokableEvent<,>).MakeGenericType(typeArguments: types),
                3 => typeof(InvokableEvent<, ,>).MakeGenericType(typeArguments: types),
                4 => typeof(InvokableEvent<, , ,>).MakeGenericType(typeArguments: types),
                _ => throw new ArgumentException(message: types.Length + "args")
            };

            return Activator.CreateInstance(type: genericType, args: new Object[] { target, methodName }) as InvokableEventBase;
        }
    }
}