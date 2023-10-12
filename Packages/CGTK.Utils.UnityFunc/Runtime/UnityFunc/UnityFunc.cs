namespace CGTK.Utils.UnityFunc
{
	using System;

	[Serializable]
	public sealed class UnityFunc<TReturn> : UnityFuncBase<TReturn> 
	{
		public TReturn Invoke()
		{
			if (func == null) Cache();
			
			if (_dynamic) 
			{
				InvokableCallback<TReturn> call = func as InvokableCallback<TReturn>;
				return call.Invoke();
			}
			else
			{
				return func.Invoke(args: Args);
			}
		}

		protected override void Cache() 
		{
			if (_target == null || String.IsNullOrEmpty(value: _methodName)) 
			{
				func = new InvokableCallback<TReturn>(target: null, methodName: null);
			}
			else
			{
				func = _dynamic ? new InvokableCallback<TReturn>(target: target, methodName: methodName) : GetPersistentMethod();
			}
		}
	}

	[Serializable]
	public sealed class UnityFunc<T0, TReturn> : UnityFuncBase<TReturn>
	{
		public TReturn Invoke(T0 arg0) 
		{
			if (func == null) Cache();
		
			if (_dynamic) 
			{
				InvokableCallback<T0, TReturn> call = func as InvokableCallback<T0, TReturn>;
				return call.Invoke(arg0: arg0);
			} 
			else 
			{
				return func.Invoke(args: Args);
			}
		}

		protected override void Cache() 
		{
			if (_target == null || String.IsNullOrEmpty(value: _methodName)) 
			{
				func = new InvokableCallback<T0, TReturn>(target: null, methodName: null);
			}
			else
			{
				func = _dynamic ? new InvokableCallback<T0, TReturn>(target: target, methodName: methodName) : GetPersistentMethod();
			}
		}
	}

	[Serializable]
	public sealed class UnityFunc<T0, T1, TReturn> : UnityFuncBase<TReturn>
	{
		public TReturn Invoke(T0 arg0, T1 arg1) 
		{
			if (func == null) Cache();
		
			if (_dynamic) 
			{
				InvokableCallback<T0, T1, TReturn> call = func as InvokableCallback<T0, T1, TReturn>;
				return call.Invoke(arg0: arg0, arg1: arg1);
			} 
			else 
			{
				return func.Invoke(args: Args);
			}
		}

		protected override void Cache() 
		{
			if (_target == null || String.IsNullOrEmpty(value: _methodName)) 
			{
				func = new InvokableCallback<T0, T1, TReturn>(target: null, methodName: null);
			}
			else
			{
				func = _dynamic ? new InvokableCallback<T0, T1, TReturn>(target: target, methodName: methodName) : GetPersistentMethod();
			}
		}
	}

	[Serializable]
	public sealed class UnityFunc<T0, T1, T2, TReturn> : UnityFuncBase<TReturn> 
	{
		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2) 
		{
			if (func == null) Cache();
			if (_dynamic) 
			{
				InvokableCallback<T0, T1, T2, TReturn> call = func as InvokableCallback<T0, T1, T2, TReturn>;
				return call.Invoke(arg0: arg0, arg1: arg1, arg2: arg2);
			} 
			else 
			{
				return func.Invoke(args: Args);
			}
		}

		protected override void Cache() 
		{
			if (_target == null || String.IsNullOrEmpty(value: _methodName)) 
			{
				func = new InvokableCallback<T0, T1, T2, TReturn>(target: null, methodName: null);
			}
			else
			{
				func = _dynamic ? new InvokableCallback<T0, T1, T2, TReturn>(target: target, methodName: methodName) : GetPersistentMethod();
			}
		}
	}

	[Serializable]
	public sealed class UnityFunc<T0, T1, T2, T3, TReturn> : UnityFuncBase<TReturn> 
	{
		public TReturn Invoke(T0 arg0, T1 arg1, T2 arg2, T3 arg3) 
		{
			if (func == null) Cache();
			if (_dynamic) 
			{
				InvokableCallback<T0, T1, T2, T3, TReturn> call = func as InvokableCallback<T0, T1, T2, T3, TReturn>;
				return call.Invoke(arg0: arg0, arg1: arg1, arg2: arg2, arg3: arg3);
			} 
			else 
			{
				return func.Invoke(args: Args);
			}
		}

		protected override void Cache() 
		{
			if (_target == null || String.IsNullOrEmpty(value: _methodName)) 
			{
				func = new InvokableCallback<T0, T1, T2, T3, TReturn>(target: null, methodName: null);
			}
			else
			{
				func = _dynamic ? new InvokableCallback<T0, T1, T2, T3, TReturn>(target: target, methodName: methodName) : GetPersistentMethod();
			}
		}
	}
}