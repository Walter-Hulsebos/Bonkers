namespace CGTK.Utils.UnityFunc
{
	using System;

	public abstract class InvokableCallbackBase<TReturn> 
	{
		public abstract TReturn Invoke(params Object[] args);
	}
}