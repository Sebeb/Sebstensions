using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Runtime.Serialization;
using UnityEngine.UI;
using UnityEngine;
using Object=UnityEngine.Object;
using Random=UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif

public static class Seb
{
	public static Vector2 screenSize => new Vector2(Screen.width, Screen.height);

#region Vectors
	public static Vector2 XZ(this Vector3 input)
	{
		return new Vector2(input.x, input.z);
	}

	public static Vector3 Y2Z(this Vector2 input, float newY = 0)
	{
		return new Vector3(input.x, newY, input.y);
	}

	public static Vector2Int XZ(this Vector3Int input)
	{
		return new Vector2Int(input.x, input.z);
	}

	public static Vector3Int Y2Z(this Vector2Int input, int newY)
	{
		return new Vector3Int(input.x, newY, input.y);
	}

	public static Vector2 ComplexMult(this Vector2 aVec, Vector2 aOther)
	{
		return new Vector2(aVec.x * aOther.x - aVec.y * aOther.y,
			aVec.x * aOther.y + aVec.y * aOther.x);
	}
	public static Vector2 Rotation(float aDegree)
	{
		var a = aDegree * Mathf.Deg2Rad;
		return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
	}
	public static Vector2 Rotate(this Vector2 aVec, float aDegree) =>
		ComplexMult(aVec, Rotation(aDegree));

	public static Vector2 SetX(this Vector2 input, float value) => new Vector2(value, input.y);
	public static Vector2 SetY(this Vector2 input, float value) => new Vector2(input.x, value);
	public static Vector2 MoveX(this Vector2 input, float value) =>
		new Vector2(input.x + value, input.y);
	public static Vector2 MoveY(this Vector2 input, float value) =>
		new Vector2(input.x, input.y + value);
	public static Vector2 ScaleX(this Vector2 input, float value) =>
		new Vector2(input.x * value, input.y);
	public static Vector2 ScaleY(this Vector2 input, float value) =>
		new Vector2(input.x, input.y * value);
	public static Vector2 CapMax(this Vector2 input, float value) =>
		new Vector2(input.x.CapMax(value), input.y.CapMax(value));
	public static Vector2 CapMin(this Vector2 input, float value) =>
		new Vector2(input.x.CapMin(value), input.y.CapMin(value));
	public static Vector2 CapMin(this Vector2 input, float xMin, float yMin) =>
		new Vector2(input.x.CapMin(xMin), input.y.CapMin(yMin));
	public static Vector2 Multiply(this Vector2 input, Vector2 value) =>
		new Vector2(input.x * value.x, input.y * value.y);
	public static Vector2 DivideComponents(this Vector2 input, Vector2 value) =>
		new Vector2(input.x / value.x, input.y / value.y);

	public static Vector3 SetX(this Vector3 input, float value) =>
		new Vector3(value, input.y, input.z);
	public static Vector3 SetY(this Vector3 input, float value) =>
		new Vector3(input.x, value, input.z);
	public static Vector3 SetZ(this Vector3 input, float value) =>
		new Vector3(input.x, input.y, value);
	public static Vector3 SetMagnitude(this Vector3 input, float value) =>
		new Vector3(input.x, input.y, input.z).normalized * value;
	public static Vector3 SetZ(this Vector2 input, float z) => new Vector3(input.x, input.y, z);
	public static Vector3 MoveX(this Vector3 input, float value) =>
		new Vector3(input.x + value, input.y, input.z);
	public static Vector3 MoveY(this Vector3 input, float value) =>
		new Vector3(input.x, input.y + value, input.z);
	public static Vector3 MoveZ(this Vector3 input, float value) =>
		new Vector3(input.x, input.y, input.z + value);
	public static Vector3 ScaleX(this Vector3 input, float value) =>
		new Vector3(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3 input, float value) =>
		new Vector3(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3 input, float value) =>
		new Vector3(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3 input, float value) =>
		new Vector3(input.x * value, input.y * value, input.z);
	public static Vector3 ScaleX(this Vector3Int input, float value) =>
		new Vector3(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3Int input, float value) =>
		new Vector3(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3Int input, float value) =>
		new Vector3(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3Int input, float value) =>
		new Vector3(input.x * value, input.y * value, input.z);
	public static Vector3 CapMax(this Vector3 input, float value) =>
		new Vector3(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));
	public static Vector3 CapMin(this Vector3 input, float value) =>
		new Vector3(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));
	public static Vector3 Multiply(this Vector3 input, Vector3 value) =>
		new Vector3(input.x * value.x, input.y * value.y, input.z * value.z);
	public static Vector3 Divide(this Vector3 input, Vector3 value) =>
		new Vector3(input.x / value.x, input.y / value.y, input.z / value.z);
	public static Vector3 AsVec3X(this float x) => new Vector3(x, 0, 0);
	public static Vector3 AsVec3Y(this float y) => new Vector3(0, y, 0);
	public static Vector3 AsVec3Z(this float z) => new Vector3(0, 0, z);

	public static Vector2Int SetX(this Vector2Int input, int value) =>
		new Vector2Int(value, input.y);
	public static Vector2Int SetY(this Vector2Int input, int value) =>
		new Vector2Int(input.x, value);
	public static Vector2Int MoveX(this Vector2Int input, int value) =>
		new Vector2Int(input.x + value, input.y);
	public static Vector2Int MoveY(this Vector2Int input, int value) =>
		new Vector2Int(input.x, input.y + value);
	public static Vector2Int ScaleX(this Vector2Int input, int value) =>
		new Vector2Int(input.x * value, input.y);
	public static Vector2Int ScaleY(this Vector2Int input, int value) =>
		new Vector2Int(input.x, input.y * value);
	public static Vector2Int CapMax(this Vector2Int input, int value) =>
		new Vector2Int(input.x.CapMax(value), input.y.CapMax(value));
	public static Vector2Int CapMin(this Vector2Int input, int value) =>
		new Vector2Int(input.x.CapMin(value), input.y.CapMin(value));

	public static Vector3Int SetX(this Vector3Int input, int value) =>
		new Vector3Int(value, input.y, input.z);
	public static Vector3Int SetY(this Vector3Int input, int value) =>
		new Vector3Int(input.x, value, input.z);
	public static Vector3Int SetZ(this Vector3Int input, int value) =>
		new Vector3Int(input.x, input.y, value);
	public static Vector3Int SetZ(this Vector2Int input, int value) =>
		new Vector3Int(input.x, input.y, value);
	public static Vector3Int MoveX(this Vector3Int input, int value) =>
		new Vector3Int(input.x + value, input.y, input.z);
	public static Vector3Int MoveY(this Vector3Int input, int value) =>
		new Vector3Int(input.x, input.y + value, input.z);
	public static Vector3Int MoveZ(this Vector3Int input, int value) =>
		new Vector3Int(input.x, input.y, input.z + value);
	public static Vector3Int CapMax(this Vector3Int input, int value) =>
		new Vector3Int(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));
	public static Vector3Int CapMin(this Vector3Int input, int value) =>
		new Vector3Int(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));


	public static Vector4 SetX(this Vector4 input, float value) =>
		new Vector4(value, input.y, input.z, input.w);
	public static Vector4 SetY(this Vector4 input, float value) =>
		new Vector4(input.x, value, input.z, input.w);
	public static Vector4 SetZ(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, value, input.w);
	public static Vector4 SetW(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, input.z, value);
	public static Vector4 MoveX(this Vector4 input, float value) =>
		new Vector4(input.x + value, input.y, input.z, input.w);
	public static Vector4 MoveY(this Vector4 input, float value) =>
		new Vector4(input.x, input.y + value, input.z, input.w);
	public static Vector4 MoveZ(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, input.z + value, input.w);
	public static Vector4 MoveW(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, input.z, input.w + value);
	public static Vector4 ScaleX(this Vector4 input, float value) =>
		new Vector4(input.x * value, input.y, input.z, input.w);
	public static Vector4 ScaleY(this Vector4 input, float value) =>
		new Vector4(input.x, input.y * value, input.z, input.w);
	public static Vector4 ScaleZ(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, input.z * value, input.w);
	public static Vector4 ScaleW(this Vector4 input, float value) =>
		new Vector4(input.x, input.y, input.z, input.w * value);
	public static Vector4 ScaleXY(this Vector4 input, float value) =>
		new Vector4(input.x * value, input.y * value, input.z, input.w);
	public static Vector4 CapMax(this Vector4 input, float value) =>
		new Vector4(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value),
			input.w.CapMax(value));
	public static Vector4 CapMin(this Vector4 input, float value) =>
		new Vector4(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value),
			input.w.CapMin(value));




	public static void SetLocalPositionAndRotation(this Transform t, Vector3 position,
		Vector3 rotation)
	{
		t.localPosition = position;
		t.localEulerAngles = rotation;
	}
	public static void SetLocalPositionAndRotation(this Transform t, Vector3 position,
		Quaternion rotation)
	{
		t.localPosition = position;
		t.localRotation = rotation;
	}

	public static Vector2 RandomTo(this Vector2 min, Vector2 max) =>
		new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	public static float RandomRange(this Vector2 input) => Random.Range(input.x, input.y);
	public static int RandomRange(this Vector2Int input, bool maxInclusive = false) =>
		Random.Range(input.x, input.y + maxInclusive.AsInt());

	public static Vector3 Modulo(this Vector3 a, Vector3 b) =>
		new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);

	/// <summary>Converts an euler rotation to be between 180 and -180, as it appears in the inspector </summary>
	public static Vector3 WrapAngle(this Vector3 angle) =>
		new Vector3(WrapAngle(angle.x), WrapAngle(angle.y), WrapAngle(angle.z));
	/// <summary>Converts an euler rotation to be between 180 and -180, as it appears in the inspector </summary>
	public static float WrapAngle(this float angle)
	{
		angle %= 360;
		return angle > 180 ? angle - 360 : angle;
	}

	public static float UnwrapAngle(this float angle)
	{
		if (angle >= 0)
			return angle;

		angle = -angle % 360;

		return 360 - angle;
	}

	public static Vector3 Average(this IEnumerable<Vector3> input) =>
		input.Aggregate(new Vector3(0, 0, 0), (s, v) => s + v) / input.Count();

	public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
	{
		float multiplier = 1;
		for (int i = 0; i < decimalPlaces; i++)
		{
			multiplier *= 10f;
		}
		return new Vector3(
			Mathf.Round(vector3.x * multiplier) / multiplier,
			Mathf.Round(vector3.y * multiplier) / multiplier,
			Mathf.Round(vector3.z * multiplier) / multiplier);
	}

	public static Vector2 Round(this Vector2 vector3, int decimalPlaces = 2)
	{
		float multiplier = 1;
		for (int i = 0; i < decimalPlaces; i++)
		{
			multiplier *= 10f;
		}
		return new Vector2(
			Mathf.Round(vector3.x * multiplier) / multiplier,
			Mathf.Round(vector3.y * multiplier) / multiplier);
	}

	public static float Round(this float input, int decimalPlaces = 2)
	{
		float multiplier = 1;
		for (int i = 0; i < decimalPlaces; i++)
		{
			multiplier *= 10f;
		}
		return
			Mathf.Round(input * multiplier) / multiplier;
	}


	public static float Lerp(this Vector2 vec, float t) => Mathf.Lerp(vec.x, vec.y, t);
	public static float InverseLerp(this Vector2 vec, float t) =>
		Mathf.InverseLerp(vec.x, vec.y, t);
#endregion

#region Rect Transform
	public static Bounds CalculateBounds(this RectTransform rectT, Space space = Space.World)
	{
		Bounds bounds = new Bounds(rectT.position,
			new Vector3(rectT.rect.width, rectT.rect.height, 0.0f));

		foreach (RectTransform rectTChild in
		         rectT.gameObject.GetComponentsInChildren<RectTransform>())
		{
			Bounds childBounds = new Bounds(rectTChild.position,
				new Vector3(rectTChild.rect.width, rectTChild.rect.height, 0.0f));
			bounds.Encapsulate(childBounds);
		}

		if (space == Space.Self)
		{
			bounds.center = rectT.InverseTransformPoint(bounds.center);
		}

		return bounds;
	}
#endregion

#region MeshRenderer
	public static Bounds CalculateMeshBounds(this GameObject obj, Space space = Space.World)
	{
		MeshRenderer rend = obj.GetComponent<MeshRenderer>();
		Bounds bounds = rend != null
			? space == Space.World ? rend.bounds : rend.localBounds
			: new Bounds();

		bounds.Encapsulate(rend.GetComponentsInChildren<MeshRenderer>().CalculateMeshBounds(space));

		return bounds;
	}

	public static Bounds CalculateMeshBounds(this MeshRenderer[] meshes, Space space = Space.World)
	{
		if (meshes == null || meshes.Length == 0)
		{
			return new Bounds(Vector3.negativeInfinity, Vector3.zero);
		}

		Bounds ret = space == Space.World ? meshes.First().bounds : meshes.First().localBounds;

		foreach (MeshRenderer mesh in meshes.Skip(1))
		{
			ret.Encapsulate(space == Space.World ? mesh.bounds : mesh.localBounds);
		}
		return ret;
	}
#endregion

#region Transform
	public static void Reset(this Transform input, Space space = Space.Self)
	{
		if (space == Space.Self)
		{
			input.localPosition = Vector3.zero;
			input.localRotation = Quaternion.identity;
			input.localScale = Vector3.one;
		}
		else
		{
			input.position = Vector3.zero;
			input.rotation = Quaternion.identity;
			input.localScale = Vector3.one;
		}
	}

	public static void CopyFrom(this Transform to, Transform from, bool includeParent = true)
	{
		if (includeParent) { to.parent = from.parent; }
		to.position = from.position;
		to.localScale = from.localScale;
		to.rotation = from.rotation;
		to.SetSiblingIndex(from.GetSiblingIndex());
	}

	public static Transform GetSceneLevelParent(this Transform t)
	{
		while (t.parent != null) { t = t.parent; }
		return t;
	}

	public static Transform GetNthParent(this Transform t, int levels)
	{
		Transform parent = t;
		for (int j = 0; j < levels; j++)
		{
			parent = parent.transform.parent;

			if (parent == null) { break; }
		}
		return parent;
	}

	public static IEnumerable<Transform> GetChildren(this Transform trans)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < trans.childCount; i++)
		{
			list.Add(trans.GetChild(i));
		}
		return list;
	}

	public static IEnumerable<GameObject> GetChildren(this GameObject go)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < go.transform.childCount; i++)
		{
			list.Add(go.transform.GetChild(i).gameObject);
		}
		return list;
	}

	public static void DestroyChildren(this Transform trans)
	{
		int childs = trans.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			trans.GetChild(i).gameObject.Destroy();
		}
	}

	public static void Destroy(this Object _go)
	{
		if (Application.isPlaying) { Object.Destroy(_go); }
		else { Object.DestroyImmediate(_go); }
	}
	public static Quaternion InverseTransformRotation(this Transform trans,
		Quaternion worldRotation) =>
		Quaternion.Inverse(trans.rotation) * worldRotation;

	public static IEnumerable<Transform> GetChildrenWithTag(this Transform parent, string tag,
		bool allowGrandChildren = false) =>
		(allowGrandChildren ? parent.GetComponentsInChildren<Transform>(includeInactive: true)
			: parent.GetChildren())
		.Where(t => t.gameObject.CompareTag(tag));
#endregion

#region String
	public static string TryRemove(this string input, int removeAt, bool appendEllipsis = false)
	{
		if (removeAt > 0 && removeAt < input.Length)
		{
			return input.Remove(removeAt) + (appendEllipsis ? "..." : "");
		}
		else return input;
	}

	public static string MakeFilename(this string input, int maxLength = -1,
		string extension = null)
	{
		input = input.TrimStart();
		input = input.Replace('.', '_');
		while (true)
		{
			int removeAt = input.IndexOfAny(Path.GetInvalidFileNameChars());
			if (removeAt == -1) { break; }
			else { input = input.Remove(removeAt, 1); }
		}

		input = input.TryRemove(maxLength, true);

		return input;
	}

	public static bool Contains(this string[] source, string toCheck, StringComparison comp) =>
		source.Any(s => s.Equals(toCheck, comp));

	public static bool Contains(this string source, string toCheck, StringComparison comp)
	{
		if (string.IsNullOrEmpty(toCheck) || string.IsNullOrEmpty(source))
			return true;

		return source.IndexOf(toCheck, comp) >= 0;
	}

//https://social.msdn.microsoft.com/Forums/vstudio/en-US/791963c8-9e20-4e9e-b184-f0e592b943b0/split-a-camel-case-string?forum=csharpgeneral
	public static string NormalizeCamel(this string input)
	{
		string words = string.Empty;
		if (!string.IsNullOrEmpty(input))
		{
			foreach (char ch in input)
			{
				if (char.IsLower(ch) || words.Length == 0)
				{
					words += ch.ToString();
				}
				else
				{
					words += " " + ch.ToString();
				}

			}
			return words;
		}
		else
			return string.Empty;
	}

	public static string CapitalizeFirstWord(this string s)
	{
		return s == null || s.Length == 0 || Char.IsUpper(s[0]) ? s
			: Char.ToUpper(s[0]) + s.Substring(1);

	}

	readonly static string[] NoCapitalize = new string[]
		{ "for", "and", "the", "of", "in", "a", "an", "or" };
	public static string CapitalizeTitle(this string @in, bool capitalizeStart = true)
	{

		List<int> spaces = @in.AllIndexesOf(' ');
		if (capitalizeStart) { @in = @in.CapitalizeFirstWord(); }
		else { spaces.Insert(0, -1); }
		for (int i = 0; i < spaces.Count; i++)
		{
			int c = spaces[i] + 1;
			if (c >= @in.Length - 1) { break; }

			if (!Char.IsLower(@in[c])) { continue; }

			string word = @in.Substring(c);
			if (i < spaces.Count - 1) { word = word.Remove(spaces[i + 1] - c); }

			if (NoCapitalize.Any(s =>
				    String.Equals(s, word, StringComparison.CurrentCultureIgnoreCase)))
			{
				continue;
			}

			@in = @in.Insert(c, Char.ToUpper(@in[c]).ToString());
			@in = @in.Remove(c + 1, 1);

		}
		return @in;

	}

	public static int NthIndexOf(this string s, char c, int n)
	{
		var takeCount = s.TakeWhile(x => (n -= (x == c ? 1 : 0)) > 0).Count();
		return takeCount == s.Length ? -1 : takeCount;
	}

	public static List<int> AllIndexesOf(this string str, string value)
	{
		if (String.IsNullOrEmpty(value))
			throw new ArgumentException("the string to find may not be empty", "value");
		List<int> indexes = new List<int>();
		for (int index = 0;; index += value.Length)
		{
			index = str.IndexOf(value, index);
			if (index == -1) { return indexes; }
			else { indexes.Add(index); }
		}
	}

	public static List<int> AllIndexesOf(this string str, char value)
	{
		List<int> indexes = new List<int>();
		for (int index = 0;; index++)
		{
			index = str.IndexOf(value, index);
			if (index == -1) { return indexes; }
			else { indexes.Add(index); }
		}
	}

	public static int IndexOfAnyString(this string input, string[] strings,
		bool indexAfterString = false, bool caseSensitive = true)
	{
		int breakPoint = -1;
		for (int i = 0; i < strings.Length; i++)
		{
			int newBreak = input.IndexOf(strings[i],
				caseSensitive ? StringComparison.CurrentCulture
					: StringComparison.CurrentCultureIgnoreCase);
			if (newBreak == -1) { continue; }

			if (indexAfterString) { newBreak += strings[i].Length; }
			if (breakPoint < 0 || newBreak < breakPoint) { breakPoint = newBreak; }
		}
		return breakPoint;
	}

	public static int LastIndexOfAnyString(this string input, string[] strings,
		bool indexAfterString = false, bool caseSensitive = true)
	{
		int breakPoint = -1;
		for (int i = 0; i < strings.Length; i++)
		{
			int newBreak = input.LastIndexOf(strings[i],
				caseSensitive ? StringComparison.CurrentCulture
					: StringComparison.CurrentCultureIgnoreCase);
			if (newBreak == -1) { continue; }

			if (indexAfterString) { newBreak += strings[i].Length; }
			if (breakPoint < 0 || newBreak > breakPoint) { breakPoint = newBreak; }
		}
		return breakPoint;
	}

	public static string StripPunctuation(this string input) =>
		new string(input.Where(c => !char.IsPunctuation(c)).ToArray());

	public static string CombinePaths(this string path1, params string[] paths)
	{
		if (path1 == null)
		{
			throw new ArgumentNullException("path1");
		}
		if (paths == null)
		{
			throw new ArgumentNullException("paths");
		}
		return paths.Aggregate(path1, (acc, p) => Path.Combine(acc, p));
	}

	public static string Range(this string @in, int start, int end) =>
		@in.Remove(end).Substring(start);

	public static int WordCount(this string test)
	{
		int count = 0;
		bool inWord = false;

		foreach (char t in test)
		{
			if (char.IsWhiteSpace(t))
			{
				inWord = false;
			}
			else
			{
				if (!inWord) count++;
				inWord = true;
			}
		}
		return count;
	}

	public static float GetPunctuationRatio(this string input) =>
		(float)input.Count(c => !Char.IsLetterOrDigit(c)) / (float)input.Length;

	public static float GetWhitespaceRatio(this string input) =>
		(float)input.Count(c => !Char.IsWhiteSpace(c)) / (float)input.Length;

	public static string AddOrdinal(this int num)
	{
		if (num <= 0) return num.ToString();

		switch (num % 100)
		{
			case 11:
			case 12:
			case 13:
				return num + "th";
		}

		switch (num % 10)
		{
			case 1:
				return num + "st";
			case 2:
				return num + "nd";
			case 3:
				return num + "rd";
			default:
				return num + "th";
		}
	}
	public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);
	public static bool IsNullOrWhiteSpace(this string input) => string.IsNullOrWhiteSpace(input);
#endregion

#region Colour
	public static Color MoveA(this Color input, float a) => input.SetA(Mathf.Clamp01(input.a + a));
	public static Color SetA(this Color input, float a)
	{
		input.a = a;
		return input;
	}

	//https://www.extensionmethod.net/csharp/color/getcontrastingcolor
	public static Color GetContrastingColor(this Color value)
	{
		var d = 0;

		// Counting the perceptive luminance - human eye favors green color...
		double a = 1 - (0.299 * value.r + 0.587 * value.g + 0.114 * value.b) / 255;

		if (a < 0.5)
			d = 0; // bright colors - black font
		else
			d = 255; // dark colors - white font

		return new Color(d, d, d);
	}

	//https://stackoverflow.com/questions/2395438/convert-system-drawing-color-to-rgb-and-hex-value
	public static string AsHex(this Color c) =>
		"#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");

	public static Vector4 AsVec4(this Color c) => new Vector4(c.r, c.g, c.b, c.a);

	public static Color Randomize(this Color c) =>
		new Color(Random.value, Random.value, Random.value, c.a);
	
	public static Color GetRandom(this Gradient g) => g.Evaluate(Random.value);
#endregion

#region Collections
	private static System.Random _random = new System.Random();

	public static T GetWeightedRandom<T>(this IEnumerable<T> itemsEnumerable,
		Func<T, int> weightKey)
	{
		List<T> items = itemsEnumerable.ToList();

		int totalWeight = items.Sum(x => weightKey(x));
		int randomWeightedIndex = _random.Next(totalWeight);
		int itemWeightedIndex = 0;

		if (!items.Any()) { return default; }

		foreach (T item in items)
		{
			itemWeightedIndex += weightKey(item);
			if (randomWeightedIndex < itemWeightedIndex)
				return item;
		}
		throw new ArgumentException("Collection count and weights must be greater than 0");
	}

	public static T GetRandomOrDefault<T>(this IEnumerable<T> list)
	{
		return !list.Any() ? default : list.ElementAt(_random.Next(list.Count()));

	}

	public static void TryRemove<T>(this List<T> list, T item)
	{
		if (list.Contains(item)) { list.Remove(item); }
	}

	private static System.Random _rng = new System.Random();

	public static void Shuffle<T>(this IList<T> list)
	{
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = _rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}

	public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<T1> keys,
		IEnumerable<T2> values) =>
		keys.Zip(values, (key, value) => new { key, value })
			.ToDictionary(val => val.key, val => val.value);
#endregion

#region GameObject
	public static Transform GetOrMakeChild(this Transform trans, string name)
	{
		if (trans)
		{
			Transform ret = trans.Find(name);
			if (ret) return ret;
		}

		GameObject go = GameObject.Find(name);
		return go ? go.transform : trans.AddChild(name);

	}

	public static Transform
		AddChild(this Transform trans, string name, bool resetLocalScale = true) =>
		trans ? trans.gameObject.AddChild(name, resetLocalScale).transform
			: new GameObject(name).transform;
	public static GameObject AddChild(this GameObject input, string name,
		bool resetLocalScale = true)
	{

		GameObject ret = new GameObject(name);
		ret.transform.parent = input.transform;
		if (resetLocalScale) ret.transform.Reset();
		else
		{
			ret.transform.localPosition = Vector3.zero;
			ret.transform.localRotation = Quaternion.identity;
		}

		return ret;
	}

	public static void SetLayersRecursively(this GameObject go, int layer)
	{
		go.layer = layer;
		for (int i = 0; i < go.transform.childCount; i++)
		{
			GameObject child = go.transform.GetChild(i).gameObject;
			child.layer = layer;
			SetLayersRecursively(child, layer);
		}
	}

	public static bool Contains(this LayerMask mask, int layer)
	{
		return mask == (mask | (1 << layer));
	}
#endregion

#region Physics
	/// <summary>
	/// Sets a joint's targetRotation to match a given local rotation.
	/// The joint transform's local rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetTargetRotationLocal(this ConfigurableJoint joint,
		Quaternion targetLocalRotation, Quaternion startLocalRotation)
	{
		if (joint.configuredInWorldSpace)
		{
			Debug.LogError(
				"SetTargetRotationLocal should not be used with joints that are configured in world space. For world space joints, use SetTargetRotation.",
				joint);
		}
		SetTargetRotationInternal(joint, targetLocalRotation, startLocalRotation, Space.Self);
	}

	/// <summary>
	/// Sets a joint's targetRotation to match a given world rotation.
	/// The joint transform's world rotation must be cached on Start and passed into this method.
	/// </summary>
	public static void SetTargetRotation(this ConfigurableJoint joint,
		Quaternion targetWorldRotation, Quaternion startWorldRotation)
	{
		if (!joint.configuredInWorldSpace)
		{
			Debug.LogError(
				"SetTargetRotation must be used with joints that are configured in world space. For local space joints, use SetTargetRotationLocal.",
				joint);
		}
		SetTargetRotationInternal(joint, targetWorldRotation, startWorldRotation, Space.World);
	}

	static void SetTargetRotationInternal(this ConfigurableJoint joint, Quaternion targetRotation,
		Quaternion startRotation, Space space)
	{
		// Calculate the rotation expressed by the joint's axis and secondary axis
		var right = joint.axis;
		var forward = Vector3.Cross(joint.axis, joint.secondaryAxis).normalized;
		var up = Vector3.Cross(forward, right).normalized;
		Quaternion worldToJointSpace = Quaternion.LookRotation(forward, up);

		// Transform into world space
		Quaternion resultRotation = Quaternion.Inverse(worldToJointSpace);

		// Counter-rotate and apply the new local rotation.
		// Joint space is the inverse of world space, so we need to invert our value
		if (space == Space.World)
		{
			resultRotation *= startRotation * Quaternion.Inverse(targetRotation);
		}
		else
		{
			resultRotation *= Quaternion.Inverse(targetRotation) * startRotation;
		}

		// Transform back into joint space
		resultRotation *= worldToJointSpace;

		// Set target rotation to our newly calculated rotation
		joint.targetRotation = resultRotation;
	}
#endregion

#region Cameras
// public static Bounds OrthographicBoundsWS(this Camera camera)
// {
// 	float screenAspect = (float)Screen.width / (float)Screen.height;
// 	float cameraHeight = camera.orthographicSize * 2;
// 	Bounds bounds = new Bounds(
// 		camera.transform.position,
// 		new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
// 	return bounds;
// }

	public static Rect GetCornersWs(this Camera camera, float z = 0)
	{
		if (camera.orthographic)
		{
			Vector2 bottomLeft = camera.ScreenToWorldPoint(Vector2.zero);
			Vector2 topRight
				= camera.ScreenToWorldPoint(new Vector2(camera.pixelWidth, camera.pixelHeight));


			return new Rect(bottomLeft, topRight - bottomLeft);
		}
		else
		{
			Vector3[] corners = new Vector3[4];
			camera.CalculateFrustumCorners(
				new Rect(0, 0, 1, 1), -camera.transform.position.z + z,
				Camera.MonoOrStereoscopicEye.Mono, corners);

			Vector3 bottomLeft = camera.transform.position +
				camera.transform.TransformVector(corners[0]);

			Vector3 topRight = camera.transform.position +
				camera.transform.TransformVector(corners[2]);

			return new Rect(bottomLeft, topRight - bottomLeft);
		}

	}
#endregion

#region Logic
	public static bool LiesBetween(this int num, int lower, int upper, bool inclusive = false)
	{
		return inclusive ? lower <= num && num <= upper : lower < num && num < upper;
	}
	public static bool LiesBetween(this float num, float lower, float upper, bool inclusive = false)
	{
		return inclusive ? lower <= num && num <= upper : lower < num && num < upper;
	}
#endregion

#region Maths
	public static float SetSign(this float value, float sign) =>
		Mathf.Abs(value) * Mathf.Sign(sign);
	public static float SetSign(this float value, bool positive) =>
		positive ? Mathf.Abs(value) : -Mathf.Abs(value);


	public static float RoundToMultiple(this float value, float multipleOf) =>
		Mathf.Round(value / multipleOf) * multipleOf;

	public static float RoundDownToMultiple(this float value, float multipleOf) =>
		value - value % multipleOf;

	public static float RoundUpToMultiple(this float value, float multipleOf) =>
		RoundDownToMultiple(value, multipleOf) + multipleOf;

	public static int Idx(int x, int y, int z, int sX, int sY) => x + y * sX + z * sX * sY;
	public static int Wrap(this int x, int m) => (x % m + m) % m;

	public static bool ToBool(this float f) => f >= 1;
	public static bool ToBool(this int i) => i >= 1;

	/// <summary> 1 if true, otherwise 0  </summary>
	public static int AsInt(this bool b) => b ? 1 : 0;
	/// <summary> Returns 1 if true, else -1   </summary>
	public static int AsDirectionalInt(this bool b) => b ? 1 : -1;

	public static float CapMax(this float f, float max) => Mathf.Min(f, max);
	public static float CapMin(this float f, float min) => Mathf.Max(f, min);
	public static int CapMax(this int i, int max) => Mathf.Min(i, max);
	public static int CapMin(this int i, int min) => Mathf.Max(i, min);
#endregion

#region Movement
	/// <returns>-1 or 1 with 50:50 chance</returns>
	public static int RandomDirection() => (Random.Range(0, 2) == 1).AsDirectionalInt();

	public static float SnappedSmoothDampTo(this float current, float target,
		ref float currentVelocity, float snapDistance, ref bool moving
		, float smoothTime, float maxSpeed = Mathf.Infinity, bool isAngle = false)
	{
		if (Mathf.Abs(target - current) < snapDistance)
		{
			moving = false;
			currentVelocity = 0;
			return target;
		}
		else
		{
			moving = true;
			return isAngle
				? Mathf.SmoothDampAngle(current, target, ref currentVelocity, smoothTime, maxSpeed)
				: Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);
		}
	}

	public static Vector3 SnappedSmoothDampTo(this Vector3 current, Vector3 target,
		ref Vector3 currentVelocity, float snapDistance, ref bool moving
		, float smoothTime, float maxSpeed = float.MaxValue)
	{
		if ((target - current).magnitude < snapDistance)
		{
			moving = false;
			return target;
		}
		else
		{
			moving = true;
			return Vector3.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);
		}
	}
#endregion

#region UI
	public static Vector2 SnapScrollToChild(this ScrollRect instance, RectTransform child)
	{
		Canvas.ForceUpdateCanvases();
		Vector2 viewportLocalPosition = instance.viewport.localPosition;
		Vector2 childLocalPosition = child.localPosition;
		Vector2 result = new Vector2(
			0 - (viewportLocalPosition.x + childLocalPosition.x),
			0 - (viewportLocalPosition.y + childLocalPosition.y)
		);
		return result;
	}
#endregion

#region Files
	/// <summary>
	/// Returns a version of the filepath which, if necessary, contains a number to ensure it is unique (e.g. untitlefile3.txt)
	/// </summary>
	public static string GetIncrementalFileNumber(this string path, bool brackets = false,
		bool filenameOnly = false)
	{
		if (!File.Exists(path))
		{
			return filenameOnly ? path.Split('/').Last() : path;
		}
		int fileCount = 1;
		string extensionlessPath = Path.Combine(Path.GetDirectoryName(path),
			Path.GetFileNameWithoutExtension(path));
		string extension = Path.GetExtension(path);

		string newPath;

		do
		{
			newPath = brackets
				? String.Concat(extensionlessPath, " (", ++fileCount, ')', extension)
				: String.Concat(extensionlessPath, ++fileCount, extension);
		}
		while (File.Exists(newPath));

		if (filenameOnly)
		{
			newPath = newPath.Split('/').Last();
		}

		return newPath;

	}

	/// <summary>
	/// Returns a version of the folderpath which, if necessary, contains a number to ensure it is unique (e.g. untitlefile3.txt)
	/// </summary>
	public static string GetIncrementalFolderNumber(this string path, bool brackets = false,
		bool foldernameOnly = false)
	{
		string pathReturn = foldernameOnly ? path.Split('/').Last() : path;

		//Check whether file exists
		if (!Directory.Exists(path)) { return pathReturn; }

		int folderCount = 1;
		string newPath;

		do
		{
			newPath = String.Concat(pathReturn,
				brackets
					? String.Concat(path, " (", folderCount, ')')
					: "" + ++folderCount);
		}
		while (Directory.Exists(newPath));

		return newPath;

	}

	public static bool EnsureFolderExists(this string path)
	{
		if (Directory.Exists(path)) { return true; }

		Directory.CreateDirectory(path);
		return false;
	}
	
	public static string CleanPathString(this string path) => string.Concat(path.Select(c => !Path.GetInvalidPathChars().Contains(c) ? c : '-'));
#endregion

#region Types
	public static IEnumerable<Type> GetInheritanceHierarchy(this Type type)
	{
		for (var current = type; current != null; current = current.BaseType)
			yield return current;
	}

#if UNITY_EDITOR
	public static IEnumerable<T> FindAssetsByType<T>() where T : Object =>
		AssetDatabase.FindAssets($"t:{typeof(T)}")
			.Select(t => AssetDatabase.GUIDToAssetPath(t))
			.Select(assetPath => AssetDatabase.LoadAssetAtPath<T>(assetPath))
			.Where(asset => asset != null).ToList();

	public static IEnumerable<Object> FindAssetsByType(Type type)
	{
		foreach (string t in AssetDatabase.FindAssets($"t:{type}"))
		{
			string assetPath = AssetDatabase.GUIDToAssetPath(t);
			Debug.Log(assetPath);
			Object asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
			if (asset != null)
			{
				yield return asset;
			}
		}
	}
#endif

	public static IEnumerable<Type> GetAllClassDerivatives<T>() where T : class =>
		System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
			.Where(t => t.IsSubclassOf(typeof(T)));
// AppDomain.CurrentDomain.GetAssemblies()
// 	.SelectMany(assembly => assembly.GetTypes())
// 	.Where(type => type.IsSubclassOf(typeof(T)))
// 	.Select(type => Activator.CreateInstance(type) as T);
#endregion

#region Enums
	public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum =>
		Enum.GetValues(input.GetType()).Cast<Enum>().Where(value => input.HasFlag(value)).Cast<T>();
#endregion

}

public enum Direction3
{
	Right,
	Left,
	Up,
	Down,
	Forward,
	Backward
}

public enum Axis { X, Y, Z }
public static class Directions
{
	private static Dictionary<Direction3, Vector3> direction3Vector3 =
		new Dictionary<Direction3, Vector3>
		{
			{ Direction3.Right, Vector3.right },
			{ Direction3.Left, Vector3.left },
			{ Direction3.Up, Vector3.up },
			{ Direction3.Down, Vector3.down },
			{ Direction3.Forward, Vector3.forward },
			{ Direction3.Backward, Vector3.back }
		};

	private static Dictionary<Direction3, Vector3Int> direction3VectorInt =
		new Dictionary<Direction3, Vector3Int>
		{
			{ Direction3.Right, Vector3Int.right },
			{ Direction3.Left, Vector3Int.left },
			{ Direction3.Up, Vector3Int.up },
			{ Direction3.Down, Vector3Int.down },
			{ Direction3.Forward, Vector3Int.forward },
			{ Direction3.Backward, Vector3Int.back }
		};

	public static IEnumerable<Direction3> All => direction3Vector3.Keys;

	public static Vector3 Vec(this Direction3 dir) => direction3Vector3[dir];
	public static Vector3Int VecInt(this Direction3 dir) => direction3VectorInt[dir];

	public static float GetAxis(this Vector3 vector3, Axis axis) =>
		GetDirection(vector3, (Direction3)((int)axis * 2));
	public static float GetDirection(this Vector3 vector3, Direction3 dir) =>
		dir switch
		{
			<= Direction3.Left => vector3.x * dir.IsPositive().AsDirectionalInt(),
			<= Direction3.Down => vector3.y * dir.IsPositive().AsDirectionalInt(),
			_ => vector3.z * dir.IsPositive().AsDirectionalInt(),
		};
	public static Direction3 Opposite(this Direction3 dir) =>
		dir + ((int)dir % 2 == 0).AsDirectionalInt();

	public static int Sign(this Direction3 dir) => dir.IsPositive() ? 1 : -1;
	public static bool IsPositive(this Direction3 dir) => (int)dir % 2 == 0;
	public static Axis Axis(this Direction3 dir) =>
		dir switch
		{
			<= Direction3.Left => global::Axis.X,
			<= Direction3.Down => global::Axis.Y,
			_ => global::Axis.Z
		};


	public static Vector3 SetAxis(this Vector3 vec, Axis axis, float value) =>
		axis switch
		{
			global::Axis.X => vec.SetX(value),
			global::Axis.Y => vec.SetY(value),
			_ => vec.SetZ(value)
		};
}



[Serializable]
public struct Vector3IntRange
{
	public Vector3Int min;
	public Vector3Int max;

	public Vector3IntRange(Vector3Int min, Vector3Int max)
	{
		this.min = min;
		this.max = max;
	}

	public Vector3IntRange CapMaxMax(int cap)
	{
		Vector3IntRange ret = this;
		ret.max.CapMax(cap);
		return ret;
	}
	public Vector3IntRange CapMaxMin(int cap)
	{
		Vector3IntRange ret = this;
		ret.max.CapMin(cap);
		return ret;
	}
	public Vector3IntRange CapMinMin(int cap)
	{
		Vector3IntRange ret = this;
		ret.min.CapMin(cap);
		return ret;
	}
	public Vector3IntRange CapMinMax(int cap)
	{
		Vector3IntRange ret = this;
		ret.min.CapMax(cap);
		return ret;
	}

	public Vector2Int rangeX
	{
		get => new Vector2Int(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2Int rangeY
	{
		get => new Vector2Int(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}
	public Vector2Int rangeZ
	{
		get => new Vector2Int(min.z, max.z);
		set
		{
			min = min.SetZ(value.x);
			max = max.SetZ(value.y);
		}
	}

	public Vector3Int GetRandom(bool maxInclusive = false) =>
		new Vector3Int(
			Random.Range(min.x, max.x + maxInclusive.AsInt()),
			Random.Range(min.y, max.y + maxInclusive.AsInt()),
			Random.Range(min.z, max.z + maxInclusive.AsInt()));
}

[Serializable]
public struct Vector3Range
{
	public Vector3 min;
	public Vector3 max;

	public Vector3Range(Vector3 min, Vector3 max)
	{
		this.min = min;
		this.max = max;
	}

	public Vector2 rangeX
	{
		get => new Vector2(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2 rangeY
	{
		get => new Vector2(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}
	public Vector2 rangeZ
	{
		get => new Vector2(min.z, max.z);
		set
		{
			min = min.SetZ(value.x);
			max = max.SetZ(value.y);
		}
	}

	public Vector3 GetRandom() =>
		new Vector3(Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			Random.Range(min.z, max.z));
}

[Serializable]
public struct Vector2Range
{
	public Vector2 min;
	public Vector2 max;

	public Vector2Range(Vector2 min, Vector2 max)
	{
		this.min = min;
		this.max = max;
	}

	public Vector2 rangeX
	{
		get => new Vector2(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2 rangeY
	{
		get => new Vector2(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}

	public Vector2 GetRandom() =>
		new Vector2(Random.Range(min.x, max.x),
			Random.Range(min.y, max.y));
}

public static class Consts
{
	public const float MagOne = 1.41421f;
}

public static class ShaderKeyword
{
	public static readonly int MainTex = Shader.PropertyToID("_MainTex");
	public static readonly int Color = Shader.PropertyToID("_Color");
	public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
	public static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
	public static readonly int BaseColorMap = Shader.PropertyToID("_BaseColorMap");
	public static readonly int FaceColor = Shader.PropertyToID("_FaceColor");
}

[Serializable]
public class RefillingPool<T>
{
	public List<T> basePool,
		activePool;
	public bool shuffle;

	public RefillingPool(IEnumerable<T> basePool)
	{
		this.basePool = new List<T>(basePool);
		RefillActivePool();
	}

	public RefillingPool()
	{
		basePool = new List<T>();
		activePool = new List<T>();
	}

	private void RefillActivePool()
	{
		activePool = new List<T>(basePool);
		if (shuffle) activePool.Shuffle();
	}

	public T GetRandom()
	{
		if (activePool.Count == 0) { RefillActivePool(); }

		T output = activePool.GetRandomOrDefault();
		activePool.Remove(output);
		return output;
	}

	public T GetRandomWhere(Func<T, bool> @where)
	{
		if (activePool.Count == 0) { RefillActivePool(); }

		IEnumerable<T> output = activePool.Where(@where);
		if (output.Count() == 0) { output = basePool.Where(@where); }
		if (output.Count() == 0) { return default(T); }

		return output.GetRandomOrDefault();
	}

	public void CheckOut(T obj)
	{
		if (activePool.Contains(obj))
		{
			activePool.Remove(obj);
		}
		else
		{
			Debug.LogError("Pool does not contain " + obj.ToString());
		}
	}

	public void Add(IEnumerable<T> @new)
	{
		basePool.AddRange(@new);
		activePool.AddRange(@new);
	}

	public int Count => basePool.Count;
	public int CurrentCount => activePool.Count;
}

public class Bictionary<T1, T2> : Dictionary<T1, T2>
{
	public Bictionary()
	{
	}
	public Bictionary(IDictionary<T1, T2> dictionary) : base(dictionary)
	{
	}
	public Bictionary(IDictionary<T1, T2> dictionary, IEqualityComparer<T1> comparer) :
		base(dictionary, comparer)
	{
	}
	public Bictionary(IEnumerable<KeyValuePair<T1, T2>> collection) : base(collection)
	{
	}
	public Bictionary(IEnumerable<KeyValuePair<T1, T2>> collection, IEqualityComparer<T1> comparer)
		: base(collection, comparer)
	{
	}
	public Bictionary(IEqualityComparer<T1> comparer) : base(comparer)
	{
	}
	public Bictionary(int capacity) : base(capacity)
	{
	}
	public Bictionary(int capacity, IEqualityComparer<T1> comparer) : base(capacity, comparer)
	{
	}
	protected Bictionary(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
	public T1 this[T2 index]
	{
		get
		{
			if (!this.Any(x => x.Value.Equals(index)))
				throw new KeyNotFoundException();
			return this.First(x => x.Value.Equals(index)).Key;
		}
	}
}

public class Map<TKey,TValue> : Dictionary<TKey,TValue>
{
	public new TValue this [TKey key]
	{
		get => ContainsKey(key) || !isDefaultValueSet ? base[key] : defaultValue;
		set => base[key] = value;
	}

	private bool isDefaultValueSet;
	private TValue _defaultValue;
	public TValue defaultValue
	{
		get => _defaultValue;
		set { _defaultValue = value; isDefaultValueSet = true; }
	}

	public Map(TValue defaultValue)
	{
		this.defaultValue = defaultValue;
	}
}

[Serializable]
public struct Transformation
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 eulerAngles
	{
		get => rotation.eulerAngles;
		set => rotation = Quaternion.Euler(value);
	}
	public Vector3 scale;

	public Transformation(Transform transform, Space space = Space.Self)
	{
		position = space == Space.World ? transform.position : transform.localPosition;
		rotation = space == Space.World ? transform.rotation : transform.localRotation;
		scale = space == Space.World ? transform.lossyScale : transform.localScale;
	}
	
	public Transformation(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public Transformation(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
		scale = Vector3.one;
	}

	public Transformation(Vector3 position)
	{
		this.position = position;
		rotation = Quaternion.identity;
		scale = Vector3.one;
	}

	public Transformation(Quaternion rotation)
	{
		position = Vector3.zero;
		this.rotation = rotation;
		scale = Vector3.one;
	}

	public static Transformation operator+(Transformation a, Transformation b) =>
		new Transformation(a.position + b.position, a.rotation * b.rotation,
			a.scale.Multiply(b.scale));
}

public static class TransformStructOverride
{
	public static void Set(this Transform t, Transformation @struct, Space space = Space.Self)
	{
		if (space == Space.Self)
		{
			t.localPosition = @struct.position;
			t.localRotation = @struct.rotation;
			t.localScale = @struct.scale;
		}
		else
		{
			t.position = @struct.position;
			t.rotation = @struct.rotation;
			t.localScale = @struct.scale.Multiply(t.lossyScale / 1);
		}
	}

	public static void Apply(this Transform t, Transformation @struct, Space space = Space.Self)
	{
		if (space == Space.Self)
		{
			t.localPosition += @struct.position;
			t.localRotation *= @struct.rotation;
			t.localScale.Scale(@struct.scale);
		}
		else
		{
			t.position += @struct.position;
			t.rotation *= @struct.rotation;
			t.localScale.Scale(@struct.scale.Multiply(t.lossyScale / 1));
		}
	}
}

public static class MathS
{

}

// https://www.habrador.com/tutorials/math/5-line-line-intersection/

//How to figure out if two lines are intersecting
public static class Lines
{
	//Line segment-line segment intersection in 2d space by using the dot product
	//p1 and p2 belongs to line 1, and p3 and p4 belongs to line 2
	public static bool Do2DLinesIntersect(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
	{
		return IsPointsOnDifferentSides(p1, p2, p3, p4) && IsPointsOnDifferentSides(p3, p4, p1, p2);
	}

	//Are the points on different sides of a line?
	public static bool IsPointsOnDifferentSides(Vector2 l1, Vector2 l2, Vector2 p1, Vector2 p2)
	{
		//The direction of the line
		Vector2 lineDir = l2 - l1;

		//The normal to a line is just flipping x and y and making y negative
		Vector2 lineNormal = new Vector2(lineDir.y, -lineDir.x);

		//Now we need to take the dot product between the normal and the points on the other line
		float dot1 = Vector2.Dot(lineNormal, p1 - l1);
		float dot2 = Vector2.Dot(lineNormal, p2 - l1);

		//If you multiply them and get a negative value then p3 and p4 are on different sides of the line
		return dot1 * dot2 < 0f;
	}

	//Is the point on the left side of a line?
	public static bool IsPointOnLeftSide(this Vector2 p, Vector2 l1, Vector2 l2)
	{
		Vector2 lineDir = l2 - l1;

		Vector2 lineNormal = new Vector2(lineDir.y, -lineDir.x);

		float dot1 = Vector2.Dot(lineNormal, p - l1);

		return dot1 < 0;
	}

	public static bool IsInTriangle(this Vector2 p, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		float s = p0.y * p2.x - p0.x * p2.y + (p2.y - p0.y) * p.x + (p0.x - p2.x) * p.y;
		float t = p0.x * p1.y - p0.y * p1.x + (p0.y - p1.y) * p.x + (p1.x - p0.x) * p.y;

		if ((s < 0) != (t < 0))
			return false;

		float a = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

		return a < 0
			? (s <= 0 && s + t >= a)
			: (s >= 0 && s + t <= a);
	}
	public static bool IsInTriangle(this Vector2 p, Vector2[] points)
	{
		return IsInTriangle(p, points[0], points[1], points[2]);
	}
	public static bool IsInTriangle(this Vector2 p, Transform[] points)
	{
		if (points?.Length < 3) return false;

		return IsInTriangle(p, points[0].position.XZ(), points[1].position.XZ(),
			points[2].position.XZ());
	}
}

public static class Reflection
{
	public static IEnumerable<T> GetAllScriptChildren<T>() where T : class
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(T))
				&& !type.ContainsGenericParameters
				&& !type.IsAbstract
				&& type.GetConstructor(Type.EmptyTypes) != null)
			.Select(type => Activator.CreateInstance(type) as T);
	}

	public static IEnumerable<T> GetAllSingletonScriptChildren<T>() where T : class
	{
		return AppDomain.CurrentDomain.GetAssemblies()
			.SelectMany(assembly => assembly.GetTypes())
			.Where(type => type.IsSubclassOf(typeof(T))
				&& type.BaseType?.GenericTypeArguments.FirstOrDefault() == type)
			.Select(type => Activator.CreateInstance(type) as T);
	}
}

namespace ProbabilityExtensions
{
	public static class ProbabilityExtensions
	{
		public static bool getPercentChance(this int chance) => Random.value * 100 < chance;
	}
}