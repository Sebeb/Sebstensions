using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = System.Object;


[Serializable, InlineProperty]
public class Sub<T> : ISub
{
    [SerializeField, OnValueChanged("OnChange"), HideLabel]
    internal T _value;

    public T value
    {
        get => _value;
        set
        {
            bool callChange = !_value.Equals(value);
            _value = value;
            if (callChange) OnChange(value);
        }
    }

    public Action<T> onChangeValue;
    
    public Sub(T value, Action<T> onChangeValue = null)
    {
        _value = value;
        if (onChangeValue != null) this.onChangeValue += onChangeValue;
    }

    protected Sub()
    {
    }

    private void OnChange(T value)
    {
        onChangeValue?.Invoke(value);
        onChange?.Invoke();
    }

    public Action onChange
    {
        get => _onChange;
        set => _onChange = value;
    }

    private Action _onChange;

    public Object valueGeneric
    {
        get => _value;
        set => this.value = (T)value;
    }
}

public interface ISub
{
    public Action onChange { get; set; }
    public Object valueGeneric { get; set; }
}

[Serializable]
public class SubBool : Sub<bool>
{
    public SubBool(bool b) => _value = b;

    public static implicit operator bool(SubBool subBool) => subBool?._value ?? false;
    
}

[Serializable]
public class SubFloat : Sub<float>
{
    public SubFloat(float f) => _value = f;

    public static implicit operator float(SubFloat subFloat) => subFloat._value;
}

[Serializable]
public class SubInt : Sub<int>
{
    public SubInt(int i, Action<int> onChangeValue = null)
    {
        _value = i;
        if (onChangeValue != null) this.onChangeValue += onChangeValue;
    }

    public static implicit operator int(SubInt subInt) => subInt._value;
    public override string ToString() => _value.ToString();
}

[Serializable]
public class SubString : Sub<string>
{
    public SubString(string character) => _value = character;

    public static implicit operator string(SubString subString) => subString._value;

    public override string ToString() => _value;
}

[Serializable]
public class SubVector2 : Sub<Vector2>
{
    public static implicit operator Vector2(SubVector2 subVector2) => subVector2._value;
}

[Serializable]
public class SubVector3 : Sub<Vector3>
{
    public static implicit operator Vector3(SubVector3 subVector3) => subVector3._value;
}

[Serializable]
public class SubVector4 : Sub<Vector4>
{
    public static implicit operator Vector4(SubVector4 subVector4) => subVector4._value;
}

[Serializable]
public class SubColor : Sub<Color>
{
    public SubColor(Color color, Action<Color> onChangeValue = null)
    {
        _value = color;
        if (onChangeValue != null) this.onChangeValue += onChangeValue;
    }

    public static implicit operator Color(SubColor subColor) => subColor._value;
}

[Serializable]
public class SubImage : Sub<UnityEngine.UI.Image>
{
    public static implicit operator UnityEngine.UI.Image(SubImage subImage) => subImage._value;
}

[Serializable]
public class SubSprite : Sub<Sprite>
{
    public SubSprite(Sprite data)
    {
        _value = data;
    }
    public SubSprite(Texture2D data)
    {
        _value = Sprite.Create(data, new Rect(0, 0, data.width, data.height), new Vector2(0.5f, 0.5f));
    }

    public static implicit operator Sprite(SubSprite subSprite) => subSprite._value;
}