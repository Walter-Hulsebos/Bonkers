namespace CGTK.Utils.UnityFunc
{
    using System;
    using System.Linq;

    using UnityEngine;

    using Object = UnityEngine.Object;

    public abstract class UnityFuncBase<TReturn> : UnityFuncBase
    {
        public InvokableCallbackBase<TReturn> func;

        public override void ClearCache()
        {
            base.ClearCache();
            func = null;
        }

        protected InvokableCallbackBase<TReturn> GetPersistentMethod()
        {
            Type[] types = new Type[ArgRealTypes.Length + 1];
            Array.Copy(sourceArray: ArgRealTypes, destinationArray: types, length: ArgRealTypes.Length);
            types[^1] = typeof(TReturn);

            Type genericType = null;

            switch (types.Length)
            {
                case 1:
                    genericType = typeof(InvokableCallback<>).MakeGenericType(typeArguments: types);
                    break;

                case 2:
                    genericType = typeof(InvokableCallback<,>).MakeGenericType(typeArguments: types);
                    break;

                case 3:
                    genericType = typeof(InvokableCallback<, ,>).MakeGenericType(typeArguments: types);
                    break;

                case 4:
                    genericType = typeof(InvokableCallback<, , ,>).MakeGenericType(typeArguments: types);
                    break;

                case 5:
                    genericType = typeof(InvokableCallback<, , , ,>).MakeGenericType(typeArguments: types);
                    break;

                default: throw new ArgumentException(message: types.Length + "args");
            }

            return Activator.CreateInstance(type: genericType, args: new System.Object[] { target, methodName, }) as InvokableCallbackBase<TReturn>;
        }
    }

    /// <summary> An inspector-friendly serializable function </summary>
    [Serializable]
    public abstract class UnityFuncBase : ISerializationCallbackReceiver
    {

        /// <summary> Target object </summary>
        public Object target
        {
            get => _target;
            set
            {
                _target = value;
                ClearCache();
            }
        }

        /// <summary> Target method name </summary>
        public String methodName
        {
            get => _methodName;
            set
            {
                _methodName = value;
                ClearCache();
            }
        }

        public System.Object[] Args => args ??= _args.Select(selector: x => x.GetValue()).ToArray();
        public System.Object[] args;
        public Type[]          ArgTypes => argTypes ??= _args.Select(selector: x => Arg.RealType(type: x.argType)).ToArray();
        public Type[]          argTypes;
        public Type[]          ArgRealTypes => argRealTypes ??= _args.Select(selector: x => Type.GetType(typeName: x._typeName)).ToArray();
        public Type[]          argRealTypes;

        public Boolean dynamic
        {
            get => _dynamic;
            set
            {
                _dynamic = value;
                ClearCache();
            }
        }

        [SerializeField] protected Object  _target;
        [SerializeField] protected String  _methodName;
        [SerializeField] protected Arg[]   _args;
        [SerializeField] protected Boolean _dynamic;
        #pragma warning disable 0414
        [SerializeField] private String _typeName;
        #pragma warning restore 0414

        [SerializeField] private Boolean dirty;

        #if UNITY_EDITOR
        protected UnityFuncBase() => _typeName = GetType().AssemblyQualifiedName;
        #endif

        public virtual void ClearCache()
        {
            argTypes = null;
            args     = null;
        }

        public void SetMethod(Object target, String methodName, Boolean dynamic, params Arg[] args)
        {
            _target     = target;
            _methodName = methodName;
            _dynamic    = dynamic;
            _args       = args;
            ClearCache();
        }

        protected abstract void Cache();

        public void OnBeforeSerialize()
        {
            #if UNITY_EDITOR
            if (dirty)
            {
                ClearCache();
                dirty = false;
            }
            #endif
        }

        public void OnAfterDeserialize()
        {
            #if UNITY_EDITOR
            _typeName = GetType().AssemblyQualifiedName;
            #endif
        }
    }

    [Serializable]
    public struct Arg
    {
        public enum ArgType
        {
            Unsupported,
            Bool,
            Int,
            Float,
            String,
            Object,
        }

        public Boolean boolValue;
        public Int32   intValue;
        public Single  floatValue;
        public String  stringValue;
        public Object  objectValue;
        public ArgType argType;
        public String  _typeName;

        public System.Object GetValue() => GetValue(type: argType);

        public System.Object GetValue(ArgType type)
        {
            switch (type)
            {
                case ArgType.Bool:   return boolValue;
                case ArgType.Int:    return intValue;
                case ArgType.Float:  return floatValue;
                case ArgType.String: return stringValue;
                case ArgType.Object: return objectValue;
                default:             return null;
            }
        }

        public static Type RealType(ArgType type)
        {
            switch (type)
            {
                case ArgType.Bool:   return typeof(Boolean);
                case ArgType.Int:    return typeof(Int32);
                case ArgType.Float:  return typeof(Single);
                case ArgType.String: return typeof(String);
                case ArgType.Object: return typeof(Object);
                default:             return null;
            }
        }

        public static ArgType FromRealType(Type type)
        {
            if (type == typeof(Boolean)) { return ArgType.Bool; }
            if (type == typeof(Int32))   { return ArgType.Int; }
            if (type == typeof(Single))  { return ArgType.Float; }
            if (type == typeof(String))  { return ArgType.String; }

            return typeof(Object).IsAssignableFrom(c: type) ? ArgType.Object : ArgType.Unsupported;
        }

        public static Boolean IsSupported(Type type) => FromRealType(type: type) != ArgType.Unsupported;
    }
}