namespace SemanticKernelTest.Utils;

/// <summary>
/// Simple class for counting progress in percents
/// </summary>
public static class PercentageCounter
{
    /// <summary>
    /// do the math
    /// </summary>
    /// <param name="wholeNumber"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static double CountPercentage(int wholeNumber, int index)
    {
        return (index / (double)wholeNumber) * 100;
    }
}