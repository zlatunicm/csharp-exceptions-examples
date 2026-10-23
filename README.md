# C# Exceptions — Hands-on Examples

Part **07** of a hands-on C# practice series. Four runnable, heavily commented examples: precise try/catch/finally mechanics (finally vs `return`, stack unwinding), **exception filters** (`when`) including the log-without-catching trick, `throw;` vs `throw e;` proven with stack traces, custom exception design done right, and a measured demonstration of why `TryParse` beats Parse+catch for expected failures.

## Running

```bash
dotnet run        # all examples
dotnet run 2      # just one (e.g. filters)
```

## What's inside

| # | Topic | Highlights |
|---|-------|------------|
| 01 | Mechanics | top-down catch matching, finally with `return`, unwinding, cost model |
| 02 | Filters & rethrow | `catch...when`, log-and-keep-flying, **`throw;` vs `throw e;` proven live** |
| 03 | Custom exceptions | type-as-API, data as properties, `InnerException` chains, when NOT to |
| 04 | Best practices | empty-catch crime scene, **Parse vs TryParse measured**, guards, global handler |

## The review checklist to remember

1. Never swallow — log **and** rethrow (`throw;`, never `throw e;`).
2. Expected failure? `TryXxx`/`Result<T>`. Exceptional failure? Exception.
3. Catch **only** what you can handle; one global handler catches the rest.
4. Guard clauses throw next to the bug (`ArgumentException.ThrowIfNullOrWhiteSpace`).
5. Custom exception only when a caller handles that case **differently** — and then: data as properties, wrap causes as `InnerException`.

## Further reading

- [Exceptions (C# guide)](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/exceptions/)
- [Best practices for exceptions](https://learn.microsoft.com/en-us/dotnet/standard/exceptions/best-practices-for-exceptions)

## The series

← Previous: [06 Types & memory](https://github.com/zlatunicm/csharp-types-memory-examples) · Next: [08 Modern C# 8–12](https://github.com/zlatunicm/csharp-modern-examples) →

| Part | Topic |
|---|---|
| 01 | [Collections](https://github.com/zlatunicm/csharp-collections-examples) |
| 02 | [LINQ](https://github.com/zlatunicm/csharp-linq-examples) |
| 03 | [Delegates & events](https://github.com/zlatunicm/csharp-delegates-events-examples) |
| 04 | [Async/await](https://github.com/zlatunicm/csharp-async-examples) |
| 05 | [Generics](https://github.com/zlatunicm/csharp-generics-examples) |
| 06 | [Types & memory](https://github.com/zlatunicm/csharp-types-memory-examples) |
| 07 | **Exceptions** (this repo) |
| 08 | [Modern C# 8–12](https://github.com/zlatunicm/csharp-modern-examples) |
| 09 | [Strings & text](https://github.com/zlatunicm/csharp-strings-examples) |
| 10 | [Files & JSON](https://github.com/zlatunicm/csharp-files-json-examples) |
| 11 | [MiniShop microservices (finale)](https://github.com/zlatunicm/minishop-microservices) |

## Author

**Matej Zlatunić**

## License

Released under the [MIT License](LICENSE). Feel free to use it for learning.
