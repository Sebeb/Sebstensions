using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine.UI;
using UnityEngine;
using Object = UnityEngine.Object;
using Quaternion = UnityEngine.Quaternion;
using Random = UnityEngine.Random;
#if UNITY_EDITOR
using UnityEditor;
#endif


// public class Lazy<T>
// {
// 	private T obj;
// 	private Func<T> func;
// 	public Lazy(Func<T> aFunc)
// 	{
// 		func = aFunc;
// 	}
// 	
// 	public T value
// 	{
// 		get
// 		{
// 			if (obj == null)
// 			{
// 				obj = func();
// 			}
//
// 			return obj;
// 		}
// 	}
// 	
// 	public static explicit operator T(Lazy<T> aLazy) => aLazy.value;
// }


public static class Seb
{
	public static Vector2 screenSize => new(Screen.width, Screen.height);

	private static Dictionary<string, Vector2> keyTimeFrame = new();
	public static void StartTimer(string key)
	{
	#if UNITY_EDITOR
		keyTimeFrame[key] = new Vector2(Time.realtimeSinceStartup, Time.frameCount);
	#endif
	}
	public static void StopTimer(string key, string debugLabel = "")
	{
	#if UNITY_EDITOR
		if (!keyTimeFrame.TryGetValue(key, out Vector2 timeFrame))
		{
			Debug.LogError($"No timer \'{key}\' started");
			return;
		}
		float time = Time.realtimeSinceStartup - timeFrame.x;
		int frames = (int)(Time.frameCount - timeFrame.y);

		Debug.Log(
			$"{(debugLabel.IsNullOrEmpty() ? key : debugLabel)}: {time.Round(3)}s - {(frames / time).Round((2))}FPS - {frames} frames");
	#endif
	}

#region Vectors

	//https://stackoverflow.com/questions/44362083/transform-a-string-to-vector3
	public static Vector3 ToVector3(this string input)
	{
		if (input != null)
		{
			input = input.Replace('(', '\0');
			input = input.Replace(')', '\0');
			var vals = input.Split(',').Select(s => s.Trim()).ToArray();
			if (vals.Length == 3)
			{
				float v1, v2, v3;
				if (float.TryParse(vals[0], out v1) &&
				    float.TryParse(vals[1], out v2) &&
				    float.TryParse(vals[2], out v3))
					return new Vector3(v1, v2, v3);
				else
					throw new ArgumentException();
			}
			else
				throw new ArgumentException();
		}
		else
			throw new ArgumentException();
	}
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
	public static Vector2 Rotate(this Vector2 aVec, float aDegree) => ComplexMult(aVec, Rotation(aDegree));

	public static Vector2 SetX(this Vector2 input, float value) => new(value, input.y);
	public static Vector2 SetY(this Vector2 input, float value) => new(input.x, value);
	public static Vector2 MoveX(this Vector2 input, float value) => new(input.x + value, input.y);
	public static Vector2 MoveY(this Vector2 input, float value) => new(input.x, input.y + value);
	public static Vector2 ScaleX(this Vector2 input, float value) => new(input.x * value, input.y);
	public static Vector2 ScaleY(this Vector2 input, float value) => new(input.x, input.y * value);
	public static Vector2 CapMax(this Vector2 input, float value) =>
		new(input.x.CapMax(value), input.y.CapMax(value));
	public static Vector2 CapMin(this Vector2 input, float value) =>
		new(input.x.CapMin(value), input.y.CapMin(value));

	public static Vector2 CapMin(this Vector2 input, float xMin, float yMin) =>
		new(input.x.CapMin(xMin), input.y.CapMin(yMin));

	public static Vector2 Multiply(this Vector2 input, Vector2 value) => new(input.x * value.x, input.y * value.y);

	public static Vector2 DivideComponents(this Vector2 input, Vector2 value) =>
		new(input.x / value.x, input.y / value.y);

	public static Vector3 SetX(this Vector3 input, float value) => new(value, input.y, input.z);
	public static Vector3 SetY(this Vector3 input, float value) => new(input.x, value, input.z);
	public static Vector3 SetZ(this Vector3 input, float value) => new(input.x, input.y, value);

	public static Vector3 SetMagnitude(this Vector3 input, float value) =>
		new Vector3(input.x, input.y, input.z).normalized * value;

	public static Vector3 SetZ(this Vector2 input, float z) => new(input.x, input.y, z);
	public static Vector3 MoveX(this Vector3 input, float value) => new(input.x + value, input.y, input.z);
	public static Vector3 MoveY(this Vector3 input, float value) => new(input.x, input.y + value, input.z);
	public static Vector3 MoveZ(this Vector3 input, float value) => new(input.x, input.y, input.z + value);
	public static Vector3 ScaleX(this Vector3 input, float value) => new(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3 input, float value) => new(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3 input, float value) => new(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3 input, float value) =>
		new(input.x * value, input.y * value, input.z);
	public static Vector3 ScaleX(this Vector3Int input, float value) => new(input.x * value, input.y, input.z);
	public static Vector3 ScaleY(this Vector3Int input, float value) => new(input.x, input.y * value, input.z);
	public static Vector3 ScaleZ(this Vector3Int input, float value) => new(input.x, input.y, input.z * value);
	public static Vector3 ScaleXY(this Vector3Int input, float value) =>
		new(input.x * value, input.y * value, input.z);

	public static Vector3 CapMax(this Vector3 input, float value) =>
		new(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));

	public static Vector3 CapMin(this Vector3 input, float value) =>
		new(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));
	public static Vector3 Clamp(this Vector3 input, Vector3 min, Vector3 max)
	{
		return new Vector3(Mathf.Clamp(input.x, min.x, max.x),
			Mathf.Clamp(input.y, min.y, max.y),
			Mathf.Clamp(input.z, min.z, max.z));
	}
	public static Vector3 Multiply(this Vector3 input, Vector3 value) =>
		new(input.x * value.x, input.y * value.y, input.z * value.z);

	public static Vector3 Divide(this Vector3 input, Vector3 value) =>
		new(input.x / value.x, input.y / value.y, input.z / value.z);

	public static Vector3 AsVec3X(this float x) => new(x, 0, 0);
	public static Vector3 AsVec3Y(this float y) => new(0, y, 0);
	public static Vector3 AsVec3Z(this float z) => new(0, 0, z);

	public static Vector2Int SetX(this Vector2Int input, int value) => new(value, input.y);
	public static Vector2Int SetY(this Vector2Int input, int value) => new(input.x, value);
	public static Vector2Int MoveX(this Vector2Int input, int value) => new(input.x + value, input.y);
	public static Vector2Int MoveY(this Vector2Int input, int value) => new(input.x, input.y + value);
	public static Vector2Int ScaleX(this Vector2Int input, int value) => new(input.x * value, input.y);
	public static Vector2Int ScaleY(this Vector2Int input, int value) => new(input.x, input.y * value);

	public static Vector2Int CapMax(this Vector2Int input, int value) =>
		new(input.x.CapMax(value), input.y.CapMax(value));

	public static Vector2Int CapMin(this Vector2Int input, int value) =>
		new(input.x.CapMin(value), input.y.CapMin(value));

	public static Vector3Int SetX(this Vector3Int input, int value) => new(value, input.y, input.z);
	public static Vector3Int SetY(this Vector3Int input, int value) => new(input.x, value, input.z);
	public static Vector3Int SetZ(this Vector3Int input, int value) => new(input.x, input.y, value);
	public static Vector3Int SetZ(this Vector2Int input, int value) => new(input.x, input.y, value);
	public static Vector3Int MoveX(this Vector3Int input, int value) => new(input.x + value, input.y, input.z);
	public static Vector3Int MoveY(this Vector3Int input, int value) => new(input.x, input.y + value, input.z);
	public static Vector3Int MoveZ(this Vector3Int input, int value) => new(input.x, input.y, input.z + value);

	public static Vector3Int CapMax(this Vector3Int input, int value) =>
		new(input.x.CapMax(value), input.y.CapMax(value), input.z.CapMax(value));

	public static Vector3Int CapMin(this Vector3Int input, int value) =>
		new(input.x.CapMin(value), input.y.CapMin(value), input.z.CapMin(value));


	public static Vector4 SetX(this Vector4 input, float value) => new(value, input.y, input.z, input.w);
	public static Vector4 SetY(this Vector4 input, float value) => new(input.x, value, input.z, input.w);
	public static Vector4 SetZ(this Vector4 input, float value) => new(input.x, input.y, value, input.w);
	public static Vector4 SetW(this Vector4 input, float value) => new(input.x, input.y, input.z, value);
	public static Vector4 MoveX(this Vector4 input, float value) =>
		new(input.x + value, input.y, input.z, input.w);
	public static Vector4 MoveY(this Vector4 input, float value) =>
		new(input.x, input.y + value, input.z, input.w);
	public static Vector4 MoveZ(this Vector4 input, float value) =>
		new(input.x, input.y, input.z + value, input.w);
	public static Vector4 MoveW(this Vector4 input, float value) =>
		new(input.x, input.y, input.z, input.w + value);
	public static Vector4 ScaleX(this Vector4 input, float value) =>
		new(input.x * value, input.y, input.z, input.w);
	public static Vector4 ScaleY(this Vector4 input, float value) =>
		new(input.x, input.y * value, input.z, input.w);
	public static Vector4 ScaleZ(this Vector4 input, float value) =>
		new(input.x, input.y, input.z * value, input.w);
	public static Vector4 ScaleW(this Vector4 input, float value) =>
		new(input.x, input.y, input.z, input.w * value);

	public static Vector4 ScaleXY(this Vector4 input, float value) =>
		new(input.x * value, input.y * value, input.z, input.w);

	public static Vector4 CapMax(this Vector4 input, float value) =>
		new(input.x.CapMax(value),
			input.y.CapMax(value),
			input.z.CapMax(value),
			input.w.CapMax(value));

	public static Vector4 CapMin(this Vector4 input, float value) =>
		new(input.x.CapMin(value),
			input.y.CapMin(value),
			input.z.CapMin(value),
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
		new(Random.Range(min.x, max.x), Random.Range(min.y, max.y));

	public static float RandomRange(this Vector2 input) => Random.Range(input.x, input.y);

	public static int RandomRange(this Vector2Int input, bool maxInclusive = false) =>
		Random.Range(input.x, input.y + maxInclusive.AsInt());

	public static Vector3 Modulo(this Vector3 a, Vector3 b) => new(a.x % b.x, a.y % b.y, a.z % b.z);

	/// <summary>Converts an euler rotation to be between 180 and -180, as it appears in the inspector </summary>
	public static Vector3 WrapAngle(this Vector3 angle) =>
		new(WrapAngle(angle.x), WrapAngle(angle.y), WrapAngle(angle.z));

	public static Vector3 UnwrapAngle(this Vector3 angle) =>
		new(UnwrapAngle(angle.x), UnwrapAngle(angle.y), UnwrapAngle(angle.z));
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

	public static Vector3 GetMeanVector(this IEnumerable<Vector3> vectors) =>
		vectors.Aggregate(Vector3.zero, (current, vector) => current + vector) / vectors.Count();

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

	public static void SetHeight(this RectTransform rectT, float height) =>
		rectT.sizeDelta = rectT.sizeDelta.SetY(height);

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
		if (includeParent)
		{
			to.parent = from.parent;
		}

		to.position = from.position;
		to.localScale = from.localScale;
		to.rotation = from.rotation;
		to.SetSiblingIndex(from.GetSiblingIndex());
	}

	public static void CopyFrom(this RectTransform to, RectTransform from, bool includeParent = true)
	{
		if (includeParent)
		{
			to.parent = from.parent;
		}

		to.anchorMin = from.anchorMin;
		to.anchorMax = from.anchorMax;
		to.position = from.position;
		to.localScale = from.localScale;
		to.rotation = from.rotation;
		to.rect.SetWidth(from.rect.width);
		to.rect.SetHeight(from.rect.height);
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

	public static void DestroyChildren(this Transform trans, bool silent = false)
	{
		int childs = trans.childCount;
		for (int i = childs - 1; i >= 0; i--)
		{
			trans.GetChild(i).gameObject.Destroy(silent: silent);
		}
	}

	public static void Destroy(this Object _go, bool immediate = false, bool silent = false)
	{
		if (Application.isPlaying)
		{
			if (immediate) { Object.DestroyImmediate(_go); }
			else { Object.Destroy(_go); }
		}
	#if UNITY_EDITOR
		else if (PrefabUtility.IsPartOfAnyPrefab(_go))
		{
			if (!silent)
			{
				Debug.Log(
					$"Could not destroy \"{_go.name}\" as it is in a prefab. Disabling instead");
			}
			switch (_go)
			{
				case GameObject gameObject:
					gameObject.SetActive(false);
					break;
				case MonoBehaviour script:
					script.gameObject.SetActive(false);
					break;
			}
		}
	#endif
		else { Object.DestroyImmediate(_go); }
	}

	public static Quaternion InverseTransformRotation(this Transform trans,
		Quaternion worldRotation) =>
		Quaternion.Inverse(trans.rotation) * worldRotation;

	public static IEnumerable<Transform> GetChildrenWithTag(this Transform parent, string tag,
		bool allowGrandChildren = false) =>
		(allowGrandChildren
			? parent.GetComponentsInChildren<Transform>(includeInactive: true)
			: parent.GetChildren())
		.Where(t => t.gameObject.CompareTag(tag));
	//https://www.loekvandenouweland.com/content/finding-gameobjects-and-transforms-recursively-linq-style.html
	public static Transform FirstChildOrDefault(this Transform parent, Func<Transform, bool> query,
		bool includeDisabled = true)
	{
		if (parent.childCount == 0)
		{
			return null;
		}

		Transform result = null;
		for (int i = 0; i < parent.childCount; i++)
		{
			var child = parent.GetChild(i);
			if (!includeDisabled && !child.gameObject.activeInHierarchy) { continue; }
			if (query(child))
			{
				return child;
			}
			result = FirstChildOrDefault(child, query);
		}

		return result;
	}
	///Searches each child recursively for a transform which fulfills the query. Returns max one result per parent child count
	public static IEnumerable<Transform> FirstChildrenWhere(this Transform parent,
		Func<Transform, bool> query,
		bool includeDisabled = true, bool includeParent = false) => includeParent && query(parent)
		? new[] { parent } :
		parent.GetChildren().Select(c =>
			query(c) && (includeDisabled || c.gameObject.activeInHierarchy) ? c
				: c.FirstChildOrDefault(query, includeDisabled)).WhereNotNull();

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

		return input + extension;
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
		return s == null || s.Length == 0 || Char.IsUpper(s[0])
			? s
			: Char.ToUpper(s[0]) + s.Substring(1);

	}

	readonly static string[] NoCapitalize = new string[]
		{ "for", "and", "the", "of", "in", "a", "an", "or" };

	public static string CapitalizeTitle(this string @in, bool capitalizeStart = true)
	{

		List<int> spaces = @in.AllIndexesOf(' ');
		if (capitalizeStart)
		{
			@in = @in.CapitalizeFirstWord();
		}
		else
		{
			spaces.Insert(0, -1);
		}

		for (int i = 0; i < spaces.Count; i++)
		{
			int c = spaces[i] + 1;
			if (c >= @in.Length - 1)
			{
				break;
			}

			if (!Char.IsLower(@in[c]))
			{
				continue;
			}

			string word = @in.Substring(c);
			if (i < spaces.Count - 1)
			{
				word = word.Remove(spaces[i + 1] - c);
			}

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

	public static List<int> AllIndexesOf(this string str, string value,
		StringComparison comparison = StringComparison.Ordinal)
	{
		if (String.IsNullOrEmpty(value))
			throw new ArgumentException("the string to find may not be empty", "value");

		List<int> indexes = new List<int>();
		for (int index = 0;; index += value.Length)
		{
			index = str.IndexOf(value, index, comparison);
			if (index == -1) { return indexes; }
			else { indexes.Add(index); }
		}
	}

	public static IEnumerable<int> IndexOfAll(this string sourceString, char subchar) =>
		IndexOfAll(sourceString, "" + subchar);
	public static IEnumerable<int> IndexOfAll(this string sourceString, string subString)
	{
		return Regex.Matches(sourceString, subString).Cast<Match>().Select(m => m.Index);
	}

	public static List<int> AllIndexesOf(this string str, params char[] value)
	{
		List<int> indexes = new List<int>();
		for (int index = 0;; index++)
		{
			index = str.IndexOfAny(value, index);
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
		new(input.Where(c => !char.IsPunctuation(c)).ToArray());

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

	public static string Join(this IEnumerable<string> input, string separator = ", ") =>
		string.Join(separator, input);

#endregion

#region Colour

	public static Color MoveA(this Color input, float a) => input.SetA(Mathf.Clamp01(input.a + a));

	public static Color SetA(this Color input, float a)
	{
		input.a = a;
		return input;
	}

	//https://www.extensionmethod.net/csharp/color/getcontrastingcolor
	public static Color GetContrastingBorW(this Color value)
	{
		var d = 0;

		// Counting the perceptive luminance - human eye favors green color...
		double a = 1 - (0.299 * value.r + 0.587 * value.g + 0.114 * value.b);

		if (a < 0.5)
			d = 0; // bright colors - black font
		else
			d = 1; // dark colors - white font

		return new Color(d, d, d);
	}

	public static Color GetContrastingColor(this Color value)
	{
		var colorHsv = new Vector3();
		Color.RGBToHSV(value, out colorHsv.x, out colorHsv.y, out colorHsv.z);

		return Color.HSVToRGB((colorHsv.x + 0.5f) % 1, colorHsv.y, colorHsv.z);
	}

	//https://stackoverflow.com/questions/2395438/convert-system-drawing-color-to-rgb-and-hex-value
	public static string AsHex(this Color c) =>
		"#" + c.r.ToString("X2") + c.g.ToString("X2") + c.b.ToString("X2");

	public static Vector4 AsVec4(this Color c) => new(c.r, c.g, c.b, c.a);
	public static Vector3 AsVec3(this Color c) => new(c.r, c.g, c.b);

	public static Color AsColor(this Vector3 c) => new(c.x, c.y, c.z);


	public static Color Randomize(this Color c) => new(Random.value, Random.value, Random.value, c.a);

	public static Color GetRandom(this Gradient g) => g.Evaluate(Random.value);

#endregion

#region Collections

	[Serializable]
	public class ObservedList<T> : IList, IList<T>
	{
		public event Action<T> OnAdd, OnRemove;
		public event Action<List<T>> OnChange;
		[SerializeReference, SubclassSelector, InlineProperty,
		 ListDrawerSettings(ListElementLabelName = "ToString")]
		private List<T> _value = new();
		private HashSet<IList<T>> subbedLists = new();
		public int Count => _value.Count;
		public bool IsSynchronized { get; }
		public object SyncRoot { get; }

		private static IList rootOList;

		/// <summary>
		/// Links a list to this one, so that any changes to this list are reflected in the other.
		/// </summary>
		/// <param name="catchUp">Adds items already in the list</param>
		public void Link(IList<T> list, bool catchUp = false)
		{
			subbedLists.Add(list);
			if (catchUp) list.AddRange(_value);
		}

		public void Unlink(IList<T> list) => subbedLists.Remove(list);

		public bool AddIfNew(object item)
		{
			if (item is T t) return AddIfNew(t);
			else throw new InvalidCastException();
		}

		/// <summary>
		/// Adds an item to the list and any subbed lists.
		/// </summary>
		/// <returns>True if added to list OR any subbed lists</returns>
		public bool AddIfNew(T item)
		{
			bool changed = _value.AddIfNew(item);
			changed = OnChangeItem(item, add: true, ifNew: true) || changed;
			return changed;
		}

		public int Add(object value)
		{
			if (value is T item)
			{
				Add(item);
				return 0;
			}
			else throw new InvalidCastException();
		}

		public void Add(T item)
		{
			_value.Add(item);
			OnChangeItem(item, add: true);
		}

		public void AddRange(IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				Add(item);
			}
		}

		private bool OnChangeItem(T item, bool add, bool ifNew = false)
		{
			rootOList ??= this;
			bool changed = false;
			OnAdd?.Invoke(item);
			OnChange?.Invoke(_value);
			foreach (IList<T> list in subbedLists)
			{
				if (add && (!ifNew || !list.Contains(item)))
				{
					if (ifNew && changed) changed = true;
					list.Add(item);
				}
				else
				{
					if (list.Remove(item)) changed = true;
				}
			}

			rootOList = null;
			return changed;
		}

		void IList.Clear() => Clear();

		void ICollection<T>.Clear() => Clear();

		public void Clear()
		{
			foreach (T item in _value)
			{
				Remove(item);
			}
		}

		public bool Contains(object value) => value is T item && _value.Contains(item);
		public bool Contains(T item) => _value.Contains(item);
		public void CopyTo(T[] array, int arrayIndex) { _value.CopyTo(array, arrayIndex); }

		public int IndexOf(object value)
		{
			if (value is T item) return IndexOf(item);
			else throw new InvalidCastException();
		}

		public int IndexOf(T item) => _value.IndexOf(item);

		public void Insert(int index, object value)
		{
			if (value is T item) Insert(index, item);
			else throw new InvalidCastException();
		}

		public void Insert(int index, T item)
		{
			_value.Insert(index, item);
			OnChangeItem(item, add: true);
		}

		public void Remove(object value)
		{
			if (value is T item) Remove(item);
			else throw new InvalidCastException();
		}

		/// <returns>True if item was removed from this or any linked lists</returns>
		public bool Remove(T item)
		{
			bool result = _value.Remove(item);
			result = OnChangeItem(item, add: false) || result;
			return result;
		}

		void IList<T>.RemoveAt(int index) => RemoveAt(index);
		void IList.RemoveAt(int index) => RemoveAt(index);

		public void RemoveAt(int index)
		{
			if (index < 0 || index >= _value.Count)
			{
				throw new IndexOutOfRangeException();
			}

			rootOList ??= this;
			T item = _value[index];
			_value.RemoveAt(index);
			OnChangeItem(item, add: false);
			rootOList = null;
		}

		public bool IsFixedSize => false;
		public bool IsReadOnly => false;
		object IList.this[int index]
		{
			get => this[index];
			set
			{
				if (value is T t)
				{
					this[index] = t;
				}
				else
				{
					Debug.LogError("Casting error");
				}
			}
		}
		public T this[int index]
		{
			get => _value[index];
			set
			{
				_value[index] = value;
				OnChange?.Invoke(_value);
			}
		}

		public void CopyTo(Array array, int index)
		{
			if (array is T[] arrayT) _value.CopyTo(arrayT, index);
		}

		public IEnumerator<T> GetEnumerator() { return _value.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
		public List<T> GetRange(int iStart, int iEnd) => _value.GetRange(iStart, iEnd);
	}

	//https://stackoverflow.com/questions/5248254/in-linq-select-all-values-of-property-x-where-x-null
	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> sequence)
	{
		return sequence.Where(e => e != null);
	}

	public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> sequence)
		where T : struct
	{
		return sequence.Where(e => e != null).Select(e => e.Value);
	}
	public static bool ExistsAndAny<T>(this IEnumerable<T> thing) => thing != null && thing.Any();
	public static T RandomOrDefault<T>(this IEnumerable<T> list, Func<T, bool> predicate = null)
	{
		// If there are no elements in the collection, return the default value of T
		list = predicate != null ? list.Where(predicate).ToList() : list.ToList();
		return !list.ExistsAndAny()
			? default
			: list.ElementAt(Random.Range(0, list.Count()));

	}

	public static int Range(this IEnumerable<int> input) => input.Max() - input.Min();
	public static float Range(this IEnumerable<float> input) => input.Max() - input.Min();

	/// <returns>True if item was removed</returns>
	public static bool TryRemove<T>(this List<T> list, T item)
	{
		if (list.Contains(item))
		{
			list.Remove(item);
			return true;
		}
		else return false;
	}

	/// <returns>True if removal occured</returns>
	public static bool TryRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
	{
		if (key != null && dict.ContainsKey(key))
		{
			dict.Remove(key);
			return true;
		}
		else return false;
	}

	private static System.Random _random = new();

	public static T GetWeightedRandom<T>(this IEnumerable<T> itemsEnumerable,
		Func<T, int> weightKey)
	{
		List<T> items = itemsEnumerable.ToList();

		int totalWeight = items.Sum(x => weightKey(x));
		int randomWeightedIndex = _random.Next(totalWeight);
		int itemWeightedIndex = 0;

		if (!items.Any())
		{
			return default;
		}

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

	/// <returns>True if added</returns>
	public static bool AddIfNew<T>(this ICollection<T> collection, T value)
	{
		if (!collection.Contains(value))
		{
			collection.Add(value);
			return true;
		}
		else return false;
	}


	/// <returns>True if any nulls removed</returns>
	public static bool ClearNulls<T>(this ICollection<T> collection)
	{
		List<T> nulls = collection.Where(x => x == null).ToList();
		foreach (T item in nulls)
		{
			collection.Remove(item);
		}

		return nulls.Any();
	}

	public static bool RemoveAll<T>(this List<T> list, T item)
	{
		bool removed = false;
		while (list.Remove(item))
		{
			removed = true;
		}

		return removed;
	}

	private static System.Random _rng = new();

	public static IList<T> Shuffle<T>(this IList<T> list, int seed = -1)
	{
		int n = list.Count;
		if (seed != -1) { _rng = new System.Random(seed); }
		while (n > 1)
		{
			n--;
			int k = (_rng).Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
		return list;
	}

	public static Dictionary<T1, T2> ToDictionary<T1, T2>(this IEnumerable<T1> keys,
		IEnumerable<T2> values) =>
		keys.Zip(values, (key, value) => new { key, value })
			.ToDictionary(val => val.key, val => val.value);

	/// <summary>
	/// Adds a value to a list in a dictionary, creating a new list set if necessary
	/// </summary>
	/// <returns>True if the dictionary already contained this key</returns>
	public static bool AddToCollectionUnique<Tkey, Tvalue, TCollection>(this Dictionary<Tkey, TCollection> dic,
		Tkey key, Tvalue value) where TCollection : ICollection<Tvalue>, new() =>
		AddToCollection(dic, key, value, true);

	/// <summary>
	/// Adds a value to a list in a dictionary, creating a new list set if necessary
	/// </summary>
	/// <returns>True if the dictionary already contained this key</returns>
	public static bool AddToCollection<Tkey, Tvalue, TCollection>(this IDictionary<Tkey, TCollection> dic,
		Tkey key, Tvalue value, bool uniqueOnly = false)
		where TCollection : ICollection<Tvalue>, new()
	{
		dic ??= new Dictionary<Tkey, TCollection>();
		if (dic.ContainsKey(key))
		{
			if (uniqueOnly) { dic[key].AddIfNew(value); }
			else { dic[key].Add(value); }
			return true;
		}
		else
		{
			dic.Add(key, new TCollection());
			dic[key].Add(value);
			return false;
		}
	}

	public static Dictionary<TKey, TCollection> ToCollectionDictionary<TElement, TKey, TValue,
		TCollection>(this IEnumerable<TElement> source,
		Func<TElement, TKey> keySelector,
		Func<TElement, TValue> valueSelector) where TCollection : ICollection<TValue>, new()
	{
		Dictionary<TKey, TCollection> dic = new Dictionary<TKey, TCollection>();
		foreach (TElement e in source)
		{
			TKey key = keySelector(e);
			TValue value = valueSelector(e);
			if (!dic.ContainsKey(key))
			{
				dic.Add(key, new TCollection());
			}
			dic[key].Add(value);
		}
		return dic;
	}

	/// <returns>True if added, false if already added</returns>
	public static bool AddIfNew<T>(this ICollection<T> list, T value, IEqualityComparer<T> comparer = null)
	{
		if ((comparer != null && !list.Contains(value, comparer)) || !list.Contains(value))
		{
			list.Add(value);
			return true;
		}
		else { return false; }
	}

	/// <returns>True if added, false if already added</returns>
	public static bool AddIfNew(this ICollection<string> list, string value, StringComparison comparison)
	{
		if (!list.Any(v => v.Equals(value, comparison)))
		{
			list.Add(value);
			return true;
		}
		else { return false; }
	}

	public static void AddRangeIfNew<T>(this ICollection<T> list, IEnumerable<T> values)
	{
		foreach (T value in values.Where(v => !list.Contains(v)))
		{
			list.Add(value);
		}
	}

	/// <returns>True if added, false if already added</returns>
	///<param name="forcePosition">If the list already contains this item at a different position, the item will be re-added at the correct position</param>
	public static bool InsertIfNew<T>(this List<T> list, int i, T value, bool forcePosition = true)
	{
		i = i.CapMax(list.Count - 1).CapMin(0);
		if (!list.Contains(value))
		{
			list.Insert(i, value);
			return true;
		}
		else
		{
			if (forcePosition && list.IndexOf(value) != i)
			{
				list.Remove(value);
				list.Insert(i, value);
			}
			return false;
		}
	}

	public static void AddOrReplace<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key,
		TValue value)
	{
		if (dict.ContainsKey(key))
			dict[key] = value;
		else dict.Add(key, value);
	}

	public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key) =>
		dic.TryGetValue(key, out TValue valueOut) ? valueOut : default;

	public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source) =>
		source.Select((item, index) => (item, index));


	//https://stackoverflow.com/questions/12172162/how-to-insert-item-into-list-in-order
	public static void SortedAdd<T>(this List<T> @this, T item) where T : IComparable<T>
	{
		if (@this.Count == 0)
		{
			@this.Add(item);
			return;
		}

		if (@this[@this.Count - 1].CompareTo(item) <= 0)
		{
			@this.Add(item);
			return;
		}

		if (@this[0].CompareTo(item) >= 0)
		{
			@this.Insert(0, item);
			return;
		}

		int index = @this.BinarySearch(item);
		if (index < 0)
			index = ~index;
		@this.Insert(index, item);
	}

	public static int IndexOf<T>(this T[] array, T value)
		=> Array.IndexOf(array, value);


	public static bool Contains<T>(this IList<T> haystack, IList<T> needle, IEqualityComparer<T> cmp = null)
	{
		return haystack.IndexOf(needle, cmp) != -1;
	}

	public static int IndexOf<T>(this IList<T> haystack, IList<T> needle)
	{
		return IndexOf(haystack, needle, null);
	}

	public static int IndexOf<T>(this IList<T> haystack, IList<T> needle, IEqualityComparer<T> cmp)
	{
		if (haystack == null || needle == null)
			throw new ArgumentNullException();

		int needleCount = needle.Count;
		if (needleCount == 0)
			return 0; //empty lists are everywhere!

		if (cmp == null)
			cmp = EqualityComparer<T>.Default;
		int count = haystack.Count;
		if (needleCount == 1) //can't beat just spinning through for it
		{
			T item = needle[0];
			for (int idx = 0; idx != count; ++idx)
				if (cmp.Equals(haystack[idx], item))
					return idx;

			return -1;
		}

		int m = 0;
		int i = 0;
		int[] table = KMPTable(needle, cmp);
		while (m + i < count)
		{
			if (cmp.Equals(needle[i], haystack[m + i]))
			{
				if (i == needleCount - 1)
					return m == needleCount ? -1 : m; //match -1 = failure to find conventional in .NET

				++i;
			}
			else
			{
				m = m + i - table[i];
				i = table[i] > -1 ? table[i] : 0;
			}
		}

		return -1;
	}

	private static int[] KMPTable<T>(IList<T> sought, IEqualityComparer<T> cmp)
	{
		int[] table = new int[sought.Count];
		int pos = 2;
		int cnd = 0;
		table[0] = -1;
		table[1] = 0;
		while (pos < table.Length)
			if (cmp.Equals(sought[pos - 1], sought[cnd]))
				table[pos++] = ++cnd;
			else if (cnd > 0)
				cnd = table[cnd];
			else
				table[pos++] = 0;
		return table;
	}

#endregion

#region Comparison

	public struct OffsetEqualityComparer : IEqualityComparer<int>
	{
		private int offset;

		public OffsetEqualityComparer(int offset) => this.offset = offset;
		public bool Equals(int a, int b) => a == b - offset;
		public int GetHashCode(int obj) => 0;
	}

#endregion

#region GameObject

	public static GameObject GetOrMakeChild(this GameObject go, string name, bool hidden = false) =>
		(go == null ? null : go.transform).GetOrMakeChild(name).gameObject;
	public static Transform GetOrMakeChild(this Transform trans, string name, bool hidden = false)
	{

		Transform ret;
		if (trans)
		{
			ret = trans.Find(name);
			if (!ret) { ret = trans.AddChild(name); }
		}
		else
		{
			ret = (GameObject.Find(name)
				?? new GameObject(name)).transform;
		}

		if (hidden) { ret.gameObject.hideFlags = HideFlags.HideInHierarchy; }
		return ret;
	}

	public static Transform
		AddChild(this Transform trans, string name, bool resetLocalScale = true) => trans
		? trans.gameObject.AddChild(name, resetLocalScale).transform
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

	public static Bounds OrthographicBoundsWS(this Camera camera)
	{
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float cameraHeight = camera.orthographicSize * 2;
		Bounds bounds = new Bounds(camera.transform.position,
			new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
		return bounds;
	}

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
				new Rect(0, 0, 1, 1),
				-camera.transform.position.z + z,
				Camera.MonoOrStereoscopicEye.Mono,
				corners);

			Vector3 bottomLeft = camera.transform.position + camera.transform.TransformVector(corners[0]);

			Vector3 topRight = camera.transform.position + camera.transform.TransformVector(corners[2]);

			return new Rect(bottomLeft, topRight - bottomLeft);
		}

	}

	public static Vector2Int Size(this RenderTexture rt) => new(rt.width, rt.height);

#endregion

#region Audio

	private static AudioSource audioSource;
	public static void Play(this AudioClip clip, float volume = 1)
	{
		if (clip == null)
		{
			Debug.Log("No audio given to play");
			return;
		}
		if (audioSource == null)
		{
			GameObject go = ((GameObject)null).GetOrMakeChild("Audio Player", true);
			go.hideFlags = HideFlags.HideAndDontSave;
			audioSource = go.GetComponent<AudioSource>();
			if (audioSource == null) { audioSource = go.AddComponent<AudioSource>(); }
		}
		audioSource.PlayOneShot(clip, volume);
	}

#endregion

#region Logic

	public static bool LiesBetween(this int num, int a, int b, bool inclusiveMax = false)
	{
		if (a > b) (a, b) = (b, a);
		return inclusiveMax ? a <= num && num <= b : a <= num && num < b;
	}

	public static bool LiesBetween(this float num, float a, float b, bool inclusiveMax = false)
	{
		if (a > b) (a, b) = (b, a);
		return inclusiveMax
			? a <= num && num <= b
			: a <= num && num < b;
	}

#endregion

#region Angles

	public static Quaternion ClampWithinEuler(this Quaternion angleA, Quaternion angleB,
		Vector3 upperBounds)
	{
		angleA *= Quaternion.Inverse(angleB);
		Vector3 euler = angleA.eulerAngles.WrapAngle();
		Vector3 eulerClamped = euler.Clamp(-upperBounds, upperBounds);

		return Quaternion.Euler(eulerClamped) * angleB;
	}

	public static Vector3 RotateToEuler(this Vector3 angleA, Vector3 angleB) => new(
		Mathf.DeltaAngle(angleA.x, angleB.x),
		Mathf.DeltaAngle(angleA.y, angleB.y),
		Mathf.DeltaAngle(angleA.z, angleB.z));

#endregion

#region Maths

	public static string ToString(this float value, int decimals) =>
		value.ToString($"F{decimals}").TrimEnd('0').TrimEnd('.');
	public static float SetSign(this float value, float sign) => Mathf.Abs(value) * Mathf.Sign(sign);
	public static float SetSign(this float value, bool positive) =>
		positive ? Mathf.Abs(value) : -Mathf.Abs(value);


	public static float RoundToMultiple(this float value, float multipleOf) =>
		Mathf.Round(value / multipleOf) * multipleOf;

	public static float RoundDownToMultiple(this float value, float multipleOf) => value - value % multipleOf;

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
		if (isAngle) { target = target.UnwrapAngle() % 360; }
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
		} while (File.Exists(newPath));

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
			newPath = string.Concat(pathReturn,
				brackets ? string.Concat(path, " (", folderCount, ')') : "" + ++folderCount);
		} while (Directory.Exists(newPath));

		return newPath;

	}

	/// <returns>True if folder already exists, false if it was created</returns>
	public static bool EnsureFolderExists(this string path)
	{
		if (Directory.Exists(path)) { return true; }

		Directory.CreateDirectory(path);
		return false;
	}

	public static string CleanPathString(this string path) =>
		string.Concat(path.Select(c => !Path.GetInvalidPathChars().Contains(c) ? c : '-'));

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

	public static IEnumerable<T> GetFlags<T>(this T input) where T : Enum => Enum.GetValues(input.GetType())
		.Cast<Enum>().Where(value => input.HasFlag(value)).Cast<T>();

	public static T[] GetEnumValues<T>()
	{
		return Enum.GetValues(typeof(T)).Cast<T>().ToArray();
	}

#endregion

#region Coroutines

	public static void WaitForFrames(int frames, Action action)
	{
		ScriptHelper.StartCoroutine(WaitForFramesCoroutine(frames, action));

		IEnumerator WaitForFramesCoroutine(int frames, Action action)
		{
			for (int i = 0; i < frames; i++) yield return null;

			action();
		}
	}

#endregion

#region Scripts

#if UNITY_EDITOR
	public static class FindMissingScriptsRecursively
	{
		[MenuItem("Tools/Remove Missing Scripts Recursively Visit Prefabs")]
		private static void FindAndRemoveMissingInSelected()
		{
			// EditorUtility.CollectDeepHierarchy does not include inactive children
			var deeperSelection = Selection.gameObjects
				.SelectMany(go => go.GetComponentsInChildren<Transform>(true))
				.Select(t => t.gameObject);
			var prefabs = new HashSet<Object>();
			int compCount = 0;
			int goCount = 0;
			foreach (var go in deeperSelection)
			{
				int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
				if (count > 0)
				{
					if (PrefabUtility.IsPartOfAnyPrefab(go))
					{
						RecursivePrefabSource(go, prefabs, ref compCount, ref goCount);
						count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(go);
						// if count == 0 the missing scripts has been removed from prefabs
						if (count == 0)
							continue;
						// if not the missing scripts must be prefab overrides on this instance
					}

					Undo.RegisterCompleteObjectUndo(go, "Remove missing scripts");
					GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
					compCount += count;
					goCount++;
				}
			}

			Debug.Log($"Found and removed {compCount} missing scripts from {goCount} GameObjects");
		}

		// Prefabs can both be nested or variants, so best way to clean all is to go through them all
		// rather than jumping straight to the original prefab source.
		private static void RecursivePrefabSource(GameObject instance, HashSet<Object> prefabs,
			ref int compCount,
			ref int goCount)
		{
			var source = PrefabUtility.GetCorrespondingObjectFromSource(instance);
			// Only visit if source is valid, and hasn't been visited before
			if (source == null || !prefabs.Add(source))
				return;

			// go deep before removing, to differantiate local overrides from missing in source
			RecursivePrefabSource(source, prefabs, ref compCount, ref goCount);

			int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(source);
			if (count > 0)
			{
				Undo.RegisterCompleteObjectUndo(source, "Remove missing scripts");
				GameObjectUtility.RemoveMonoBehavioursWithMissingScript(source);
				compCount += count;
				goCount++;
			}
		}
	}
#endif

#endregion

#region Editor Tools

#if UNITY_EDITOR
	public static T GetEditorSelectedComponent<T>() where T : Component =>
		Selection.GetFiltered<T>(SelectionMode.Editable).FirstOrDefault();

	private static Dictionary<Type, Component> lastSelected = new();

	[InitializeOnLoadMethod, InitializeOnEnterPlayMode]
	private static void OnStartEditor()
	{
		Selection.selectionChanged += OnSelectionChange;
		OnSelectionChange();
	}

	private static void OnSelectionChange()
	{
		foreach (Type type in lastSelected.Keys.ToList())
		{
			if (Selection.GetFiltered(type, SelectionMode.Editable).FirstOrDefault() is Component comp
			    && comp.GetType() == type)
			{
				lastSelected[type] = comp;
			}
		}
	}

	public static void RequestSelectionTracking(Type type) => lastSelected.TryAdd(type, null);

	public static T GetLastSelected<T>() where T : Component
	{
		if (lastSelected.TryGetValue(typeof(T), out Component comp))
		{
			return comp as T;
		}
		else
		{
			lastSelected.Add(typeof(T), null);
			return null;
		}
	}
#endif

#endregion

}


[Serializable]
public class Timer
{
	[NonSerialized, ShowInInspector, DisplayTime]
	public float startTime = 0;

	public enum State
	{
		Stopped,
		Running,
		Paused,
		Completed
	}

	[NonSerialized, ShowInInspector, DisplayTime(getState: "state")]
	public float elapsedTime;
	public Func<float> timeFunction = () => Time.time;
	[SerializeField] private State state;
	[FoldoutGroup("Settings")] public bool autoUpdate = true;
	[FoldoutGroup("Settings")] public bool stopOnComplete = true;
	[DisplayTimerActions("elapsedTime")]
	public SDictionary<float, Action> actions = new();

	public Timer(float duration, Action onComplete)
	{
		actions.Add(duration, onComplete);
	}

	public void AddAction(float time, Action action)
	{
		if (!actions.ContainsKey(time))
		{
			actions.Add(time, action);
		}
		else
		{
			actions[time] -= action;
			actions[time] += action;
		}
	}

	public void RemoveAction(Action action)
	{
		foreach (float key in actions.Keys)
		{
			actions[key] -= action;
		}
	}

	public Timer() {}

	public void Start()
	{
		Reset();
		Resume();
	}

	public void Reset()
	{
		startTime = timeFunction();
		nextEventTime = -1;
		elapsedTime = 0;
		state = State.Stopped;
	}

	public void Pause()
	{
		state = State.Paused;
	}

	public void Resume()
	{
		if (!Application.isPlaying)
		{
			Debug.LogError("Timer can only run in play mode");
			return;
		}

		if (!autoUpdate) return;

		state = State.Running;
		ScriptHelper.StartCoroutine(CoroAutoUpdate());
	}

	private IEnumerator CoroAutoUpdate()
	{
		while (state == State.Running)
		{
			Update();
			yield return null;
		}
	}

	public void Update()
	{
		if (actions.Any() && elapsedTime >= nextEventTime)
		{
			if (actions.TryGetValue(nextEventTime, out Action action)) action?.Invoke();

			nextEventTime = actions.Keys.OrderBy(k => k).FirstOrDefault(k => k > nextEventTime);
		}

		elapsedTime = timeFunction() - startTime;
		if (nextEventTime == 0)
		{
			if (stopOnComplete)
			{
				Pause();
				state = State.Completed;
			}
			else
			{
				nextEventTime = float.MaxValue;
			}
		}

		if (!autoUpdate)
		{
			state = State.Paused;
		}
	}

	float nextEventTime = -1;

	public static string ToHhMmSs(float time, int decimalPlaces = 2)
	{
		int hours = Mathf.Abs((int)time / 3600);
		int minutes = Mathf.Abs((int)(time - hours * 3600) / 60);
		float seconds = Mathf.Abs((time - hours * 3600 - minutes * 60)).Round(decimalPlaces);
		char sign = time < 0 ? '-' : ' ';
		float decimalPart = seconds - (int)seconds;
		string output = "" + sign;
		if (hours > 0) output += hours + ":";
		output += minutes.ToString("00") + ":";
		output += seconds.ToString("00" + (decimalPlaces <= 0 ? "" : "." + new string('0', decimalPlaces)));
		return output.Trim();
	}

	public static float FromHhMmSs(string time)
	{
		string[] split = time.Split(':');
		float hours = split.Length < 3 ? 0
			: float.TryParse(split[0], out float h) ? h : 0;
		float minutes = split.Length < 2 ? 0
			: float.TryParse(split[^2], out float m) ? m : 0;
		float seconds = float.TryParse(split[^1], out float s) ? s : 0;
		return hours * 3600 + minutes * 60 + seconds;
	}
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class DisplayTimeAttribute : Attribute
{
	public string GetState;
	public int decimalPlaces;

	public DisplayTimeAttribute(int decimalPlaces = 0, string getState = null)
	{
		this.decimalPlaces = decimalPlaces;
		GetState = getState;
	}
}

public class DisplayTimerActionsAttribute : Attribute
{
	public string GetElapsedTime;

	public DisplayTimerActionsAttribute(string getElapsedTime)
	{
		GetElapsedTime = getElapsedTime;
	}
}


public enum Direction3
{
	Right, Left, Up,
	Down, Forward, Backward
}

public enum Axis { X, Y, Z }

public static class Directions
{
	private static Dictionary<Direction3, Vector3> direction3Vector3 =
		new()
		{
			{ Direction3.Right, Vector3.right },
			{ Direction3.Left, Vector3.left },
			{ Direction3.Up, Vector3.up },
			{ Direction3.Down, Vector3.down },
			{ Direction3.Forward, Vector3.forward },
			{ Direction3.Backward, Vector3.back }
		};

	private static Dictionary<Direction3, Vector3Int> direction3VectorInt =
		new()
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
public class SDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField, HideInInspector]
	private List<TKey> keyData = new();

	[SerializeField]
	public List<TValue> valueData = new();

	public SDictionary() {}

	public SDictionary([NotNull] IDictionary<TKey, TValue> dictionary) : base(dictionary) {}

	public SDictionary([NotNull] IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(
		dictionary,
		comparer) {}

	public SDictionary(IEqualityComparer<TKey> comparer) : base(comparer) {}

	public SDictionary(int capacity) : base(capacity) {}

	public SDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer) {}

	protected SDictionary(SerializationInfo info, StreamingContext context) : base(info, context) {}

	public SDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection) : base(collection) {}

	public SDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey> comparer) :
		base(
			collection,
			comparer) {}

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Clear();
		for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
		{
			this[keyData[i]] = valueData[i];
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		keyData.Clear();
		valueData.Clear();

		foreach (KeyValuePair<TKey, TValue> item in this)
		{
			keyData.Add(item.Key);
			valueData.Add(item.Value);
		}
	}
}

[Serializable]
public class SSortedDictionary<TKey, TValue> : SortedDictionary<TKey, TValue>, ISerializationCallbackReceiver
{
	[SerializeField, HideInInspector]
	private List<TKey> keyData = new();

	[SerializeField, HideInInspector]
	private List<TValue> valueData = new();

	void ISerializationCallbackReceiver.OnAfterDeserialize()
	{
		Clear();
		for (int i = 0; i < keyData.Count && i < valueData.Count; i++)
		{
			this[keyData[i]] = valueData[i];
		}
	}

	void ISerializationCallbackReceiver.OnBeforeSerialize()
	{
		keyData.Clear();
		valueData.Clear();

		foreach (KeyValuePair<TKey, TValue> item in this)
		{
			keyData.Add(item.Key);
			valueData.Add(item.Value);
		}
	}
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
		get => new(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2Int rangeY
	{
		get => new(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}
	public Vector2Int rangeZ
	{
		get => new(min.z, max.z);
		set
		{
			min = min.SetZ(value.x);
			max = max.SetZ(value.y);
		}
	}

	public Vector3Int GetRandom(bool maxInclusive = false) =>
		new(
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
		get => new(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2 rangeY
	{
		get => new(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}
	public Vector2 rangeZ
	{
		get => new(min.z, max.z);
		set
		{
			min = min.SetZ(value.x);
			max = max.SetZ(value.y);
		}
	}

	public Vector3 GetRandom() =>
		new(Random.Range(min.x, max.x),
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
		get => new(min.x, max.x);
		set
		{
			min = min.SetX(value.x);
			max = max.SetX(value.y);
		}
	}
	public Vector2 rangeY
	{
		get => new(min.y, max.y);
		set
		{
			min = min.SetY(value.x);
			max = max.SetY(value.y);
		}
	}

	public Vector2 GetRandom() =>
		new(Random.Range(min.x, max.x),
			Random.Range(min.y, max.y));
}

public static class Consts
{
	public const float MagOne = 1.41421f;
}

public static class Keywords
{
	public static readonly int MainTex = Shader.PropertyToID("_MainTex");
	public static readonly int Color = Shader.PropertyToID("_Color");
	public static readonly int Albedo = Shader.PropertyToID("_Albedo");
	public static readonly int Normal = Shader.PropertyToID("_Normal");
	public static readonly int RoOcMe = Shader.PropertyToID("_RoOcMe");
	public static readonly int BaseColor = Shader.PropertyToID("_BaseColor");
	public static readonly int BaseMap = Shader.PropertyToID("_BaseMap");
	public static readonly int BaseColorMap = Shader.PropertyToID("_BaseColorMap");
	public static readonly int FaceColor = Shader.PropertyToID("_FaceColor");
	public static readonly int Rotation = Shader.PropertyToID("_Rotation");
	public static readonly int Position = Shader.PropertyToID("_Position");
	public static readonly int FaceTex = Shader.PropertyToID("_Face_Tex");
	public static readonly int SkinTone = Shader.PropertyToID("_Skin_Tone");
	public static readonly int MovementSpeed = Animator.StringToHash("Movement Speed");
	public static readonly int Expression = Animator.StringToHash("Expression");
	public static readonly int Reset = Animator.StringToHash("Reset");
	public static readonly int Shuffle = Animator.StringToHash("Shuffle");
}

[Serializable]
public class RefillingPool<T>
{
	[SerializeField]
	private List<T> basePool,
		activePool;
	public bool shuffle;

	public RefillingPool(IEnumerable<T> basePool, bool shuffle = true)
	{
		this.basePool = new List<T>(basePool);
		this.shuffle = shuffle;
		RefillActivePool();
	}

	public RefillingPool(bool shuffle = true)
	{
		basePool = new List<T>();
		activePool = new List<T>();
		this.shuffle = shuffle;
	}

	private void RefillActivePool()
	{
		activePool = new List<T>(basePool);
		if (shuffle) activePool.Shuffle();
	}


	public T GetNext()
	{
		if (activePool.Count == 0) { RefillActivePool(); }

		T output = activePool.FirstOrDefault();
		activePool.Remove(output);
		return output;
	}

	public T GetNextWhere(Func<T, bool> predicate)
	{
		if (activePool.Count == 0) { RefillActivePool(); }

		IEnumerable<T> ret = activePool.Where(predicate);
		if (ret.Count() == 0) { ret = basePool.Where(predicate); }
		if (ret.Count() == 0) { return default; }

		return ret.FirstOrDefault();
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

	public void Add(T newItem)
	{
		basePool.Add(newItem);
		activePool.Add(newItem);
	}
	public void Add(IEnumerable<T> newItem)
	{
		basePool.AddRange(newItem);
		activePool.AddRange(newItem);
	}

	public void Clear()
	{
		basePool = new List<T>();
		activePool = new List<T>();
	}

	public List<T> GetBase() => basePool;

	public int Count => basePool.Count;
	public int CurrentCount => activePool.Count;
}

public class Bictionary<T1, T2> : Dictionary<T1, T2>
{
	public Bictionary() {}

	public Bictionary(IDictionary<T1, T2> dictionary) : base(dictionary) {}

	public Bictionary(IDictionary<T1, T2> dictionary, IEqualityComparer<T1> comparer) :
		base(dictionary, comparer) {}

	public Bictionary(IEnumerable<KeyValuePair<T1, T2>> collection) : base(collection) {}

	public Bictionary(IEnumerable<KeyValuePair<T1, T2>> collection, IEqualityComparer<T1> comparer)
		: base(collection, comparer) {}

	public Bictionary(IEqualityComparer<T1> comparer) : base(comparer) {}

	public Bictionary(int capacity) : base(capacity) {}

	public Bictionary(int capacity, IEqualityComparer<T1> comparer) : base(capacity, comparer) {}

	protected Bictionary(SerializationInfo info, StreamingContext context) : base(info, context) {}

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

[Serializable]
public class Map<TKey, TValue> : SDictionary<TKey, TValue>
{
	public new TValue this[TKey key]
	{
		get
		{
			if (!ContainsKey(key))
			{
				Add(key, emptyValueIniter != null ? emptyValueIniter.Invoke() : defaultValue);
			}

			return base[key];
		}
		set => base[key] = value;
	}

	private readonly Func<TValue> emptyValueIniter;
	private readonly TValue defaultValue;

	public Map(Func<TValue> emptyValueIniter)
	{
		this.emptyValueIniter = emptyValueIniter;
	}

	public Map(TValue defaultValue = default)
	{
		this.defaultValue = defaultValue;
	}
}

[Serializable]
public struct Translation
{
	public Vector3 position;
	public Quaternion rotation;
	public Vector3 eulerAngles
	{
		get => rotation.eulerAngles;
		set => rotation = Quaternion.Euler(value);
	}
	public Vector3 scale;

	public Translation(Transform transform, Space space = Space.Self)
	{
		position = space == Space.World ? transform.position : transform.localPosition;
		rotation = space == Space.World ? transform.rotation : transform.localRotation;
		scale = space == Space.World ? transform.lossyScale : transform.localScale;
	}

	public Translation(Vector3 position, Quaternion rotation, Vector3 scale)
	{
		this.position = position;
		this.rotation = rotation;
		this.scale = scale;
	}

	public Translation(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
		scale = Vector3.one;
	}

	public Translation(Vector3 position)
	{
		this.position = position;
		rotation = Quaternion.identity;
		scale = Vector3.one;
	}

	public Translation(Quaternion rotation)
	{
		position = Vector3.zero;
		this.rotation = rotation;
		scale = Vector3.one;
	}

	public static Translation operator +(Translation a, Translation b) =>
		new(a.position + b.position,
			a.rotation * b.rotation,
			a.scale.Multiply(b.scale));

	public static implicit operator Translation(Transform t) => new(t);
}

public static class TransformStructOverride
{
	public static void Set(this Transform t, Translation @struct, Space space = Space.Self)
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

	public static void Apply(this Transform t, Translation @struct, Space space = Space.Self)
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

public static class MathS {}

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

		return IsInTriangle(p,
			points[0].position.XZ(),
			points[1].position.XZ(),
			points[2].position.XZ());
	}
}

public static class Reflection
{
	public static IEnumerable<Type> GetAllScriptChildTypes<T>() where T : class
	{
		return TypeCache.GetTypesDerivedFrom<T>()
			.Where(t => !t.ContainsGenericParameters
				&& !t.ContainsGenericParameters
				&& !t.IsAbstract);
	}

	public static IEnumerable<T> ConstructTypes<T>(this IEnumerable<Type> baseTypes)
		where T : class
		=> ConstructTypes<T>(baseTypes, Type.EmptyTypes);
	public static IEnumerable<T> ConstructTypes<T>(this IEnumerable<Type> baseTypes,
		params Type[] siginatureTypes) where T : class
		=> baseTypes
			.Select(t => t.ConstructType<T>(siginatureTypes))
			.WhereNotNull();

	public static T ConstructType<T>(this Type baseType, params Type[] siginatureTypes) where T : class
	{
		if (baseType.GetConstructor(siginatureTypes) is {} constructor)
		{
			return constructor.Invoke(siginatureTypes) as T;
		}
		else return null;
	}

	public static IEnumerable<Type> GetAllSingletonScriptChildrenTypes<T>() where T : class
	{
		return TypeCache.GetTypesDerivedFrom<T>()
			.Where(t => !t.ContainsGenericParameters
				&& t.GetConstructor(Type.EmptyTypes) != null
				&& !t.IsAbstract)
			.Select(t =>
				new Tuple<Type, IEnumerable<Type>>(t,
					t.GetBaseClasses(true)))
			.Where(p => p.Item2.Any(t => t.BaseType is { IsConstructedGenericType: true }
				&& t.BaseType.GenericTypeArguments.Contains(t)))
			.Select(p => p.Item1);
	}

	/// <summary>
	/// Alternative version of <see cref="Type.IsSubclassOf"/> that supports raw generic types (generic types without
	/// any type parameters).
	/// </summary>
	/// <param name="baseType">The base type class for which the check is made.</param>
	/// <param name="toCheck">To type to determine for whether it derives from <paramref name="baseType"/>.</param>
	public static bool IsSubclassOfRawGeneric(this Type toCheck, Type baseType)
	{
		while (toCheck != typeof(object))
		{
			Type cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
			if (baseType == cur)
			{
				return true;
			}

			toCheck = toCheck.BaseType;
		}

		return false;
	}
}

namespace ProbabilityExtensions
{

	public static class ProbabilityExtensions
	{
		public static bool GetPercentChance(this int chance) => Random.value * 100 < chance;
		public static float ToSeededRandomValue(this int seed)
		{
			Random.InitState(seed);
			return Random.value;
		}
	}
}

/// <summary>
/// https://stackoverflow.com/questions/12373800/3-digit-currency-code-to-currency-symbol
/// </summary>
public static class CurrencyCodeMapper
{
	private static readonly Dictionary<string, string> SymbolsByCode;

	public static string GetSymbol(string code) { return SymbolsByCode[code]; }

	static CurrencyCodeMapper()
	{
		SymbolsByCode = new Dictionary<string, string>();

		foreach (RegionInfo region in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
			         .Select(x => new RegionInfo(x.LCID)))
		{
			SymbolsByCode.TryAdd(region.ISOCurrencySymbol, region.CurrencySymbol);
		}
	}
}

public static class LinqExtensions
{
	/// <summary>Calls an action on each item before yielding them.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="action">The action to call for each item.</param>
	public static IEnumerable<T> Examine<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T obj in source)
		{
			action(obj);
			yield return obj;
		}
	}

	/// <summary>Perform an action on each item.</summary>
	/// <param name="source">The source.</param>
	/// <param name="action">The action to perform.</param>
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (T obj in source)
			action(obj);
		return source;
	}

	/// <summary>Perform an action on each item.</summary>
	/// <param name="source">The source.</param>
	/// <param name="action">The action to perform.</param>
	public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
	{
		int num = 0;
		foreach (T obj in source)
			action(obj, num++);
		return source;
	}

	/// <summary>Convert each item in the collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="converter">Func to convert the items.</param>
	public static IEnumerable<T> Convert<T>(this IEnumerable source, Func<object, T> converter)
	{
		foreach (object obj in source)
			yield return converter(obj);
	}


	/// <summary>Add an item to the beginning of a collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="prepend">Func to create the item to prepend.</param>
	public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, Func<T> prepend)
	{
		yield return prepend();
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>Add an item to the beginning of a collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="prepend">The item to prepend.</param>
	public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, T prepend)
	{
		yield return prepend;
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add a collection to the beginning of another collection.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="prepend">The collection to prepend.</param>
	public static IEnumerable<T> PrependWith<T>(this IEnumerable<T> source, IEnumerable<T> prepend)
	{
		foreach (T obj in prepend)
			yield return obj;
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">Func to create the item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		bool condition,
		Func<T> prepend)
	{
		if (condition)
			yield return prepend();
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		bool condition,
		T prepend)
	{
		if (condition)
			yield return prepend;
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add a collection to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The collection to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		bool condition,
		IEnumerable<T> prepend)
	{
		if (condition)
		{
			foreach (T obj in prepend)
				yield return obj;
		}
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">Func to create the item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		Func<T> prepend)
	{
		if (condition())
			yield return prepend();
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		T prepend)
	{
		if (condition())
			yield return prepend;
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add a collection to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The collection to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		IEnumerable<T> prepend)
	{
		if (condition())
		{
			foreach (T obj in prepend)
				yield return obj;
		}
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">Func to create the item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<IEnumerable<T>, bool> condition,
		Func<T> prepend)
	{
		if (condition(source))
			yield return prepend();
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The item to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<IEnumerable<T>, bool> condition,
		T prepend)
	{
		if (condition(source))
			yield return prepend;
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>
	/// Add a collection to the beginning of another collection, if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="prepend">The collection to prepend.</param>
	public static IEnumerable<T> PrependIf<T>(
		this IEnumerable<T> source,
		Func<IEnumerable<T>, bool> condition,
		IEnumerable<T> prepend)
	{
		if (condition(source))
		{
			foreach (T obj in prepend)
				yield return obj;
		}
		foreach (T obj in source)
			yield return obj;
	}

	/// <summary>Add an item to the end of a collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="append">Func to create the item to append.</param>
	public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, Func<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		yield return append();
	}

	/// <summary>Add an item to the end of a collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="append">The item to append.</param>
	public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, T append)
	{
		foreach (T obj in source)
			yield return obj;
		yield return append;
	}

	/// <summary>Add a collection to the end of another collection.</summary>
	/// <param name="source">The collection.</param>
	/// <param name="append">The collection to append.</param>
	public static IEnumerable<T> AppendWith<T>(this IEnumerable<T> source, IEnumerable<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		foreach (T obj in append)
			yield return obj;
	}

	/// <summary>
	/// Add an item to the end of a collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">Func to create the item to append.</param>
	public static IEnumerable<T> AppendIf<T>(
		this IEnumerable<T> source,
		bool condition,
		Func<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition)
			yield return append();
	}

	/// <summary>
	/// Add an item to the end of a collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">The item to append.</param>
	public static IEnumerable<T> AppendIf<T>(this IEnumerable<T> source, bool condition, T append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition)
			yield return append;
	}

	/// <summary>
	/// Add a collection to the end of another collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">The collection to append.</param>
	public static IEnumerable<T> AppendIf<T>(
		this IEnumerable<T> source,
		bool condition,
		IEnumerable<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition)
		{
			foreach (T obj in append)
				yield return obj;
		}
	}

	/// <summary>
	/// Add an item to the end of a collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">Func to create the item to append.</param>
	public static IEnumerable<T> AppendIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		Func<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition())
			yield return append();
	}

	/// <summary>
	/// Add an item to the end of a collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">The item to append.</param>
	public static IEnumerable<T> AppendIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		T append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition())
			yield return append;
	}

	/// <summary>
	/// Add a collection to the end of another collection if a condition is met.
	/// </summary>
	/// <param name="source">The collection.</param>
	/// <param name="condition">The condition.</param>
	/// <param name="append">The collection to append.</param>
	public static IEnumerable<T> AppendIf<T>(
		this IEnumerable<T> source,
		Func<bool> condition,
		IEnumerable<T> append)
	{
		foreach (T obj in source)
			yield return obj;
		if (condition())
		{
			foreach (T obj in append)
				yield return obj;
		}
	}

	/// <summary>
	/// Returns and casts only the items of type <typeparamref name="T" />.
	/// </summary>
	/// <param name="source">The collection.</param>
	public static IEnumerable<T> FilterCast<T>(this IEnumerable source)
	{
		foreach (object obj1 in source)
		{
			if (obj1 is T obj2)
				yield return obj2;
		}
	}

	/// <summary>Adds a collection to a hashset.</summary>
	/// <param name="hashSet">The hashset.</param>
	/// <param name="range">The collection.</param>
	public static void AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> range)
	{
		foreach (T obj in range)
			hashSet.Add(obj);
	}

	/// <summary>
	/// Returns <c>true</c> if the list is either null or empty. Otherwise <c>false</c>.
	/// </summary>
	/// <param name="list">The list.</param>
	public static bool IsNullOrEmpty<T>(this IList<T> list) => list == null || list.Count == 0;

	/// <summary>Sets all items in the list to the given value.</summary>
	/// <param name="list">The list.</param>
	/// <param name="item">The value.</param>
	public static void Populate<T>(this IList<T> list, T item)
	{
		int count = list.Count;
		for (int index = 0; index < count; ++index)
			list[index] = item;
	}

	/// <summary>
	/// Adds the elements of the specified collection to the end of the IList&lt;T&gt;.
	/// </summary>
	public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
	{
		if (list is List<T>)
		{
			((List<T>)list).AddRange(collection);
		}
		else
		{
			foreach (T obj in collection)
				list.Add(obj);
		}
	}

	/// <summary>Sorts an IList</summary>
	public static void Sort<T>(this IList<T> list, Comparison<T> comparison)
	{
		if (list is List<T>)
		{
			((List<T>)list).Sort(comparison);
		}
		else
		{
			List<T> objList = new List<T>((IEnumerable<T>)list);
			objList.Sort(comparison);
			for (int index = 0; index < list.Count; ++index)
				list[index] = objList[index];
		}
	}

	/// <summary>Sorts an IList</summary>
	public static void Sort<T>(this IList<T> list)
	{
		if (list is List<T>)
		{
			((List<T>)list).Sort();
		}
		else
		{
			List<T> objList = new List<T>((IEnumerable<T>)list);
			objList.Sort();
			for (int index = 0; index < list.Count; ++index)
				list[index] = objList[index];
		}
	}
}