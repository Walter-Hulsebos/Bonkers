using System;

using UnityEngine;

namespace CGTK.Utils.UnityFunc.Attributes
{
	/// <summary> Add to fields of your class extending UnityFuncBase<T,..> to limit which types can be assigned to it. </summary>
	public class TargetConstraintAttribute : PropertyAttribute 
	{
		public Type targetType;

		/// <summary> Add to fields of your class extending UnityFuncBase<T,..> to limit which types can be assigned to it. </summary>
		public TargetConstraintAttribute(Type targetType) 
		{
			this.targetType = targetType;
		}
	}
}
