namespace CharMod.CharModCode.Components.Enhances;

public sealed class EnhanceSuppressionScope : IDisposable
{
    private static int _suppressionCount;

    public static bool IsSuppressed => _suppressionCount > 0;

    public EnhanceSuppressionScope()
    {
        _suppressionCount++;
    }

    public void Dispose()
    {
        _suppressionCount--;
        GC.SuppressFinalize(this);
    }
}
