using System;
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
public class BindSettingAttribute : Attribute
{
	public Type settingsType;
	public BindSettingAttribute(Type settingsType = null)
	{
		this.settingsType = settingsType;
	}
}