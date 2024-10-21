namespace SemanticKernelTest;

/// <summary>
/// The core of the app.
/// Runs actual code after host is created.
/// </summary>
public interface ICore
{
    /// <summary>
    /// Run the whole routine
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RunAsync(CancellationToken cancellationToken);
}