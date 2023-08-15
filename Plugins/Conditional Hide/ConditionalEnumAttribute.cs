using UnityEngine;
using System;

//Original version of the ConditionalHideAttribute created by Brecht Lecluyse (www.brechtos.com)
//Modified by: Sebastian Lague

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property |
    AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
public class ConditionalEnumAttribute : PropertyAttribute
{
    public string conditionalSourceField;
    public int enumIndex;

    public ConditionalEnumAttribute(string boolVariableName)
    {
        conditionalSourceField = boolVariableName;
    }

    public ConditionalEnumAttribute(string enumVariableName, int enumIndex)
    {
        conditionalSourceField = enumVariableName;
        this.enumIndex = enumIndex;
    }

}



