using System;

namespace QTFramework
{
	public class UIEntityComponentAttribute : System.Attribute
	{
		public string m_kUIFullPath { get; }

		public UIEntityComponentAttribute(string _UIFullPath)
		{
			this.m_kUIFullPath = _UIFullPath;
		}
	}
}