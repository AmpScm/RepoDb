namespace RepoDb.Interfaces;

/// <summary>
/// An interfaced that is used to mark a class to be a resolver.
/// </summary>
/// <typeparamref name="TResult">The type of the result value.</typeparamref>
public interface IResolver<TResult>
{
    /// <summary>
    /// Resolves an input value to a target result type.
    /// </summary>
    /// <returns>The resolved value.</returns>
    TResult? Resolve();
}

/// <summary>
/// An interfaced that is used to mark a class to be a resolver.
/// </summary>
/// <typeparamref name="TInput">The type of the input value.</typeparamref>
/// <typeparamref name="TResult">The type of the result value.</typeparamref>
public interface IResolver<TInput, TResult>
{
    /// <summary>
    /// Resolves an input value to a target result type.
    /// </summary>
    /// <param name="input">The input value.</param>
    /// <returns>The resolved value.</returns>
    TResult? Resolve(TInput input);
}

/// <summary>
/// An interfaced that is used to mark a class to be a resolver.
/// </summary>
/// <typeparamref name="TInput1">The type of the first input value.</typeparamref>
/// <typeparamref name="TInput2">The type of the second input value.</typeparamref>
/// <typeparamref name="TResult">The type of the result value.</typeparamref>
public interface IResolver<TInput1, TInput2, TResult>
{
    /// <summary>
    /// Resolves an input value to a target result type.
    /// </summary>
    /// <param name="input1">The first input value.</param>
    /// <param name="input2">The second input value.</param>
    /// <returns>The resolved value.</returns>
    TResult? Resolve(TInput1 input1,
        TInput2 input2);
}
