using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System;
using UnityEditor;
using UnityEngine.Internal;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public static class Sebstentions
{
	public static Vector2 screenSize { get { return new Vector2(Screen.width, Screen.height); } }

#region Vectors

	public static Vector2 XZ(this Vector3 input)
	{
		return new Vector2(input.x, input.z);
	}

	public static Vector3 Y2Z(this Vector2 input, float newY)
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
	public static Vector2 Rotate(this Vector2 aVec, float aDegree) => ComplexMult(aVec, Rotation(aDegree));

	public static Vector2 SetX(this Vector2 input, float value) => new Vector2(value, input.y);
	public static Vector2 SetY(this Vector2 input, float value) => new Vector2(input.x, value);
	public static Vector2 MoveX(this Vector2 input, float value) => new Vector2(input.x + value, input.y);
	public static Vector2 MoveY(this Vector2 input, float value) => new Vector2(input.x, input.y + value);
	public static Vector2 ScaleX(this Vector2 input, float value) => new Vector2(input.x * value, input.y);
	public static Vector2 ScaleY(this Vector2 input, float value) => new Vector2(input.x, input.y * value);
	public static Vector2 CapMax(this Vector2 input, float value) => new Vector2(input.x.CapMax(value), input.y.CapMax(value));
	public static Vector2 CapMin(this Vector2 input, float value) => new Vector2(input.x.CapMin(value), input.y.CapMin(value));
	public static Vector2 Multiply(this Vector2 input, Vector2 value) => new Vector2(input.x * value.x, input.y * value.y);
	public static Vector2 Divide(this Vector2 input, Vector2 value) => new Vector2(input.x / value.x, input.y / value.y);

	public static Vector3 SetX(this Vector3 input, float value) => new Vector3(value, input.y, input.z);
	public static Vector3 SetY(this Vector3 input, float value) => new Vector3(input.x, value, input.z);
	public static Vector3 SetZ(this Vector3 input, float value) => new Vector3(input.x, input.y, value);
	public static Vector3 SetMagnitude(this Vector3 input, float value) => new Vector3(input.x, input.y, input.z).normalized * value;
	public static Vector3 SetZ(this Vector2 input, float value) => new Vector3(input.x, input.y, value);
	public static Vector3 MoveX(this Vector3 input, float value) => new Vector3(input.x + value, input.y, input.z);
	public static Vector3 MoveY(this Vector3 input, float value) => new Vector3(input.x, input.y + value, input.z);
	public static Vector3 MoveZ(this Vector3 input, float value) => new Vector3(input.x, input.y, input.z + value);
	public static Vector3 ScaleX(this Vector3 input, float value) => new Vector3(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3 input, float value) => new Vector3(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3 input, float value) => new Vector3(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3 input, float value) => new Vector3(input.x * value, input.y * value, input.z);
	public static Vector3 ScaleX(this Vector3Int input, float value) => new Vector3(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3Int input, float value) => new Vector3(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3Int input, float value) => new Vector3(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3Int input, float value) => new Vector3(input.x * value, input.y * value, input.z);
	public static Vector3 CapMax(this Vector3 input, float value) =>
		new Vector3(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));
	public static Vector3 CapMin(this Vector3 input, float value) =>
		new Vector3(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));
	public static Vector3 Multiply(this Vector3 input, Vector3 value) => new Vector3(input.x * value.x, input.y * value.y, input.z * value.z);
	public static Vector3 Divide(this Vector3 input, Vector3 value) => new Vector3(input.x / value.x, input.y / value.y, input.z / value.z);
	public static Vector3 AsVec3X(this float x) => new Vector3(x, 0, 0);
	public static Vector3 AsVec3Y(this float y) => new Vector3(0, y, 0);
	public static Vector3 AsVec3Z(this float z) => new Vector3(0, 0, z);

	public static Vector2Int SetX(this Vector2Int input, int value) => new Vector2Int(value, input.y);
	public static Vector2Int SetY(this Vector2Int input, int value) => new Vector2Int(input.x, value);
	public static Vector2Int MoveX(this Vector2Int input, int value) => new Vector2Int(input.x + value, input.y);
	public static Vector2Int MoveY(this Vector2Int input, int value) => new Vector2Int(input.x, input.y + value);
	public static Vector2Int ScaleX(this Vector2Int input, int value) => new Vector2Int(input.x * value, input.y);
	public static Vector2Int ScaleY(this Vector2Int input, int value) => new Vector2Int(input.x, input.y * value);
	public static Vector2Int CapMax(this Vector2Int input, int value) => new Vector2Int(input.x.CapMax(value), input.y.CapMax(value));
	public static Vector2Int CapMin(this Vector2Int input, int value) => new Vector2Int(input.x.CapMin(value), input.y.CapMin(value));

	public static Vector3Int SetX(this Vector3Int input, int value) => new Vector3Int(value, input.y, input.z);
	public static Vector3Int SetY(this Vector3Int input, int value) => new Vector3Int(input.x, value, input.z);
	public static Vector3Int SetZ(this Vector3Int input, int value) => new Vector3Int(input.x, input.y, value);
	public static Vector3Int SetZ(this Vector2Int input, int value) => new Vector3Int(input.x, input.y, value);
	public static Vector3Int MoveX(this Vector3Int input, int value) => new Vector3Int(input.x + value, input.y, input.z);
	public static Vector3Int MoveY(this Vector3Int input, int value) => new Vector3Int(input.x, input.y + value, input.z);
	public static Vector3Int MoveZ(this Vector3Int input, int value) => new Vector3Int(input.x, input.y, input.z + value);
	public static Vector3Int CapMax(this Vector3Int input, int value) =>
		new Vector3Int(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));
	public static Vector3Int CapMin(this Vector3Int input, int value) =>
		new Vector3Int(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));


	public static Vector4 SetX(this Vector4 input, float value) => new Vector4(value, input.y, input.z, input.w);
	public static Vector4 SetY(this Vector4 input, float value) => new Vector4(input.x, value, input.z, input.w);
	public static Vector4 SetZ(this Vector4 input, float value) => new Vector4(input.x, input.y, value, input.w);
	public static Vector4 SetW(this Vector4 input, float value) => new Vector4(input.x, input.y, input.z, value);
	public static Vector4 MoveX(this Vector4 input, float value) => new Vector4(input.x + value, input.y, input.z, input.w);
	public static Vector4 MoveY(this Vector4 input, float value) => new Vector4(input.x, input.y + value, input.z, input.w);
	public static Vector4 MoveZ(this Vector4 input, float value) => new Vector4(input.x, input.y, input.z + value, input.w);
	public static Vector4 MoveW(this Vector4 input, float value) => new Vector4(input.x, input.y, input.z, input.w + value);
	public static Vector4 ScaleX(this Vector4 input, float value) => new Vector4(input.x * value, input.y, input.z, input.w);
	public static Vector4 ScaleY(this Vector4 input, float value) => new Vector4(input.x, input.y * value, input.z, input.w);
	public static Vector4 ScaleZ(this Vector4 input, float value) => new Vector4(input.x, input.y, input.z * value, input.w);
	public static Vector4 ScaleW(this Vector4 input, float value) => new Vector4(input.x, input.y, input.z, input.w * value);
	public static Vector4 ScaleXY(this Vector4 input, float value) => new Vector4(input.x * value, input.y * value, input.z, input.w);
	public static Vector4 CapMax(this Vector4 input, float value) =>
		new Vector4(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value), input.w.CapMax(value));
	public static Vector4 CapMin(this Vector4 input, float value) =>
		new Vector4(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value), input.w.CapMin(value));




	public static void SetLocalPositionAndRotation(this Transform t, Vector3 position, Vector3 rotation)
	{
		t.localPosition = position;
		t.localEulerAngles = rotation;
	}
	public static void SetLocalPositionAndRotation(this Transform t, Vector3 position, Quaternion rotation)
	{
		t.localPosition = position;
		t.localRotation = rotation;
	}

	public static Vector2 RandomTo(this Vector2 min, Vector2 max) => new Vector2(Random.Range(min.x, max.x), Random.Range(min.y, max.y));
	public static float RandomRange(this Vector2 input) => Random.Range(input.x, input.y);
	public static int RandomRange(this Vector2Int input) => Random.Range(input.x, input.y);

	public static Vector3 Modulo(this Vector3 a, Vector3 b) => new Vector3(a.x % b.x, a.y % b.y, a.z % b.z);

	/// <summary>Converts an euler rotation to be between 180 and -180, as it appears in the inspector </summary>
	public static Vector3 WrapAngle(this Vector3 angle) => new Vector3(WrapAngle(angle.x), WrapAngle(angle.y), WrapAngle(angle.z));
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

#endregion

#region Transform

	public static void ResetLS(this Transform input)
	{
		input.localPosition = Vector3.zero;
		input.localRotation = Quaternion.identity;
		input.localScale = Vector3.one;
	}
	public static void ResetWS(this Transform input)
	{
		input.position = Vector3.zero;
		input.rotation = Quaternion.identity;
		input.localScale = Vector3.one;
	}

	public static void CopyFrom(this Transform to, Transform from, bool includeParent = true)
	{
		if (includeParent) to.parent = from.parent;
		to.position = from.position;
		to.localScale = from.localScale;
		to.rotation = from.rotation;
		to.SetSiblingIndex(from.GetSiblingIndex());
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

	public static Transform[] GetChildren(this Transform trans)
	{
		List<Transform> list = new List<Transform>();
		for (int i = 0; i < trans.childCount; i++)
		{
			list.Add(trans.GetChild(i));
		}
		return list.ToArray();
	}

	public static GameObject[] GetChildren(this GameObject go)
	{
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < go.transform.childCount; i++)
		{
			list.Add(go.transform.GetChild(i).gameObject);
		}
		return list.ToArray();
	}

	public static void DestroyChildren(this Transform trans)
	{
		int childs = trans.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			trans.GetChild(i).gameObject.EditorSafeDestroy();
		}
	}

	public static void EditorSafeDestroy(this GameObject _go)
	{
		if (Application.isPlaying) { GameObject.Destroy(_go); }
		else { GameObject.DestroyImmediate(_go); }
	}

#endregion

#region String

	public static string TryRemove(this string input, int removeAt, bool appendEllipsis = false)
	{
		if (removeAt > 0 && removeAt < input.Length) { return input.Remove(removeAt) + (appendEllipsis ? "..." : ""); }
		else return input;
	}

	public static string MakeFilename(this string input, int maxLength = -1)
	{
		input = input.TrimStart();
		input = input.Replace('.', '_');
		while (true)
		{
			int removeAt = input.IndexOfAny(System.IO.Path.GetInvalidFileNameChars());
			if (removeAt == -1) { break; }
			else { input = input.Remove(removeAt, 1); }
		}

		input = input.TryRemove(maxLength, true);

		return input;
	}

	public static bool Contains(this string[] source, string toCheck, StringComparison comp) => source.Any(s => s.Equals(toCheck, comp));

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
		return s == null || s.Length == 0 || Char.IsUpper(s[0]) ? s : Char.ToUpper(s[0]) + s.Substring(1);

	}

	readonly static string[] NoCapitalize = new string[] { "for", "and", "the", "of", "in", "a", "an", "or" };
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

			if (NoCapitalize.Any(s => String.Equals(s, word, StringComparison.CurrentCultureIgnoreCase))) { continue; }

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

	public static int IndexOfAnyString(this string input, string[] strings, bool indexAfterString = false, bool caseSensitive = true)
	{
		int breakPoint = -1;
		for (int i = 0; i < strings.Length; i++)
		{
			int newBreak = input.IndexOf(strings[i], caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
			if (newBreak == -1) { continue; }

			if (indexAfterString) { newBreak += strings[i].Length; }
			if (breakPoint < 0 || newBreak < breakPoint) { breakPoint = newBreak; }
		}
		return breakPoint;
	}

	public static int LastIndexOfAnyString(this string input, string[] strings, bool indexAfterString = false, bool caseSensitive = true)
	{
		int breakPoint = -1;
		for (int i = 0; i < strings.Length; i++)
		{
			int newBreak = input.LastIndexOf(strings[i], caseSensitive ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase);
			if (newBreak == -1) { continue; }

			if (indexAfterString) { newBreak += strings[i].Length; }
			if (breakPoint < 0 || newBreak > breakPoint) { breakPoint = newBreak; }
		}
		return breakPoint;
	}

	public static string StripPunctuation(this string input) => new string(input.Where(c => !char.IsPunctuation(c)).ToArray());

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

	public static string Range(this string @in, int start, int end) => @in.Remove(end).Substring(start);

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

	public static float GetPunctuationRatio(this string input) => (float)input.Count(c => !Char.IsLetterOrDigit(c)) / (float)input.Length;

	public static float GetWhitespaceRatio(this string input) => (float)input.Count(c => !Char.IsWhiteSpace(c)) / (float)input.Length;

#endregion

#region Colour

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

#endregion

#region Collections

	private static System.Random _random = new System.Random();

	public static T GetRandomOrDefault<T>(this IEnumerable<T> list)
	{
		// If there are no elements in the collection, return the default value of T
		if (list.Count() == 0)
		{
			return default(T);
		}

		return list.ElementAt(_random.Next(list.Count()));
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

	public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<T1> keys, IEnumerable<T2> values)
		=> keys.Zip(values, (key, value) => new { key, value })
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

	public static Transform AddChild(this Transform trans, string name, bool resetLocalScale = true) =>
		trans ? trans.gameObject.AddChild(name, resetLocalScale).transform : new GameObject(name).transform;
	public static GameObject AddChild(this GameObject input, string name, bool resetLocalScale = true)
	{

		GameObject ret = new GameObject(name);
		ret.transform.parent = input.transform;
		if (resetLocalScale) ret.transform.ResetLS();
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

#region Cameras

		public static Bounds OrthographicBoundsWS(this Camera camera)
		{
			float screenAspect = (float)Screen.width / (float)Screen.height;
			float cameraHeight = camera.orthographicSize * 2;
			Bounds bounds = new Bounds(
				camera.transform.position,
				new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
			return bounds;
		}
#endregion


#region Logic

	public static bool LiesBetween(this int num, int lower, int upper, bool inclusive = false)
	{
		return inclusive
			? lower <= num && num <= upper
			: lower < num && num < upper;
	}
	public static bool LiesBetween(this float num, float lower, float upper, bool inclusive = false)
	{
		return inclusive
			? lower <= num && num <= upper
			: lower < num && num < upper;
	}

#endregion

#region Maths

	public static float RoundToMultiple(this float value, float multipleOf)
		=> Mathf.Round(value / multipleOf) * multipleOf;

	public static float RoundDownToMultiple(this float value, float multipleOf)
		=> value - value % multipleOf;

	public static float RoundUpToMultiple(this float value, float multipleOf)
		=> RoundDownToMultiple(value, multipleOf) + multipleOf;

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

	public static float SnappedSmoothDampTo(this float current, float target, ref float currentVelocity, float snapDistance, ref bool moving
		, float smoothTime) => SnappedSmoothDampTo(current, target, ref currentVelocity, snapDistance, ref moving, smoothTime, Mathf.Infinity);
	public static float SnappedSmoothDampTo(this float current, float target, ref float currentVelocity, float snapDistance, ref bool moving
		, float smoothTime, float maxSpeed)
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
			return Mathf.SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed);
		}
	}

	public static Vector3 SnappedSmoothDampTo(this Vector3 current, Vector3 target, ref Vector3 currentVelocity, float snapDistance, ref bool moving
		, float smoothTime) => SnappedSmoothDampTo(current, target, ref currentVelocity, snapDistance, ref moving, smoothTime, Mathf.Infinity);
	public static Vector3 SnappedSmoothDampTo(this Vector3 current, Vector3 target, ref Vector3 currentVelocity, float snapDistance, ref bool moving
		, float smoothTime, float maxSpeed)
	{
		if ((target - current).magnitude < snapDistance)
		{
			moving = false;
			return target;
		}
		else
		{
			moving = true;
			return Vector3.SmoothDamp(current, target, ref currentVelocity, snapDistance, smoothTime, maxSpeed);
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
	public static string GetIncrementalFileNumber(this string path, bool brackets = false, bool filenameOnly = false)
	{
		if (!System.IO.File.Exists(path))
		{
			return filenameOnly ? path.Split('/').Last() : path;
		}
		int fileCount = 1;
		string extensionlessPath = Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path));
		string extension = Path.GetExtension(path);

		string newPath;

		do
		{
			newPath = brackets
				? String.Concat(extensionlessPath, " (", ++fileCount, ')', extension)
				: String.Concat(extensionlessPath, ++fileCount, extension);
		} while (System.IO.File.Exists(newPath));

		if (filenameOnly)
		{
			newPath = newPath.Split('/').Last();
		}

		return newPath;

	}

	/// <summary>
	/// Returns a version of the folderpath which, if necessary, contains a number to ensure it is unique (e.g. untitlefile3.txt)
	/// </summary>
	public static string GetIncrementalFolderNumber(this string path, bool brackets = false, bool foldernameOnly = false)
	{
		string pathReturn = foldernameOnly ? path.Split('/').Last() : path;

		//Check whether file exists
		if (!System.IO.Directory.Exists(path)) { return pathReturn; }

		int folderCount = 1;
		string newPath;

		do
		{
			newPath = String.Concat(pathReturn,
				brackets
					? String.Concat(path, " (", folderCount, ')')
					: "" + ++folderCount);
		} while (System.IO.Directory.Exists(newPath));

		return newPath;

	}

#endregion

#region Types

	public static IEnumerable<Type> GetInheritanceHierarchy
		(this Type type)
	{
		for (var current = type; current != null; current = current.BaseType)
			yield return current;
	}

	#if UNITY_EDITOR
	public static IEnumerable<T> FindAssetsByType<T>() where T : Object
		=> FindAssetsByType(typeof(T)) as IEnumerable<T>;

	public static IEnumerable<Object> FindAssetsByType(Type type) {
		var guids = AssetDatabase.FindAssets($"t:{type}");
		foreach (var t in guids) {
			var assetPath = AssetDatabase.GUIDToAssetPath(t);
			var asset = AssetDatabase.LoadAssetAtPath(assetPath, type);
			if (asset != null) {
				yield return asset;
			}
		}
	}
	#endif

#endregion

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

	public Vector3Int GetRandom()
		=> new Vector3Int(Random.Range(min.x, max.x),
			Random.Range(min.y, max.y),
			Random.Range(min.z, max.z));
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

	public Vector3 GetRandom()
		=> new Vector3(Random.Range(min.x, max.x),
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

	public Vector2 GetRandom()
		=> new Vector2(Random.Range(min.x, max.x),
			Random.Range(min.y, max.y));
}

public static class Consts
{
	public const float MagOne = 1.41421f;
}

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
	public T1 this[T2 index]
	{
		get
		{
			if (!this.Any(x => x.Value.Equals(index)))
				throw new System.Collections.Generic.KeyNotFoundException();
			return this.First(x => x.Value.Equals(index)).Key;
		}
	}
}

[Serializable]
public struct TransformStruct
{

	public Vector3 position;
	public Quaternion rotation;
	public Vector3 eulerAngles
	{
		get => rotation.eulerAngles;
		set => rotation = Quaternion.Euler(value);
	}
	public Vector3 scale;

	public TransformStruct(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public TransformStruct(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = Vector3.one;
	}

	public TransformStruct(Vector3 position)
	{
		this.position = position;
		this.rotation = Quaternion.identity;
		this.scale = Vector3.one;
	}

	public TransformStruct(Quaternion rotation)
	{
		this.position = Vector3.zero;
		this.rotation = rotation;
		this.scale = Vector3.one;
	}

	public static TransformStruct operator +(TransformStruct a, TransformStruct b) =>
		new TransformStruct(a.position + b.position, a.rotation * b.rotation, a.scale.Multiply(b.scale));
}

public static class TransformStructOverride
{
	public static void Set(this Transform t, TransformStruct @struct, Space space = Space.Self)
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

	public static void Apply(this Transform t, TransformStruct @struct, Space space = Space.Self)
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

		var a = -p1.y * p2.x + p0.y * (p2.x - p1.x) + p0.x * (p1.y - p2.y) + p1.x * p2.y;

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

		return IsInTriangle(p, points[0].position.XZ(), points[1].position.XZ(), points[2].position.XZ());
	}
}