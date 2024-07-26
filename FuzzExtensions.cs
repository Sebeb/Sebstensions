using FuzzySharp;
using FuzzySharp.PreProcess;


public static class Fuzzy
{
	public static int FuzzyScore(this string source1, string source2) =>
		Fuzz.PartialRatio(source1, source2, PreprocessMode.Full);
}