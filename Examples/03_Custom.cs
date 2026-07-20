namespace ExceptionsLab.Examples;

/// <summary>
/// CUSTOM EXCEPTIONS — when and how to define your own.
///
/// WHEN: when the CALLER might handle your failure differently from generic
/// ones — i.e., the exception TYPE is the API. "OrderNotFoundException" lets
/// a controller return 404 while everything else becomes 500.
///
/// HOW (the modern checklist):
///   - name ends in 'Exception', inherits Exception (NOT ApplicationException —
///     that base class is a historical mistake, nobody uses it),
///   - the three classic constructors (message / message+inner),
///   - carry DATA as properties — handlers should not parse the message,
///   - WRAP lower-level causes as InnerException to keep the evidence chain.
/// </summary>
public static class CustomDemo
{
    /// <summary>Base for all failures of our 'shop' domain.</summary>
    public class ShopException : Exception
    {
        public ShopException() { }
        public ShopException(string message) : base(message) { }
        public ShopException(string message, Exception inner) : base(message, inner) { }
    }

    /// <summary>Typed failure with DATA — the type + properties ARE the API.</summary>
    public sealed class OutOfStockException(string product, int requested, int available)
        : ShopException($"'{product}': trazeno {requested}, na stanju {available}")
    {
        public string Product { get; } = product;
        public int Requested { get; } = requested;
        public int Available { get; } = available;
    }

    public sealed class PaymentFailedException(string reason, Exception inner)
        : ShopException($"placanje odbijeno: {reason}", inner)
    {
        public string Reason { get; } = reason;
    }

    public static void Run()
    {
        // ── The type is the API ──────────────────────────────────────────────
        Helper.Heading("Handlers branch on the TYPE, read the DATA");
        try
        {
            Order("kava", quantity: 5);
        }
        catch (OutOfStockException e)
        {
            Helper.Info($"caught typed error: {e.Message}");
            Helper.Info($"...and structured data: Product='{e.Product}', missing {e.Requested - e.Available} kom");
            Helper.Info("(a web API would map THIS type to 409/404 — no message parsing!)");
        }

        // ── Wrapping preserves the chain ─────────────────────────────────────
        Helper.Heading("Wrap low-level causes (InnerException)");
        try
        {
            Pay(150m);
        }
        catch (PaymentFailedException e)
        {
            Helper.Info($"domain-level: {e.Message}");
            Helper.Info($"root cause:   {e.InnerException?.GetType().Name}: {e.InnerException?.Message}");
            Helper.Info("The caller sees YOUR domain language; the log keeps the truth.");
        }

        // ── A base type for the domain ───────────────────────────────────────
        Helper.Heading("A domain base class enables blanket handling");
        try { Order("kolac", 99); }
        catch (ShopException e)                    // catches every shop failure
        {
            Helper.Info($"catch (ShopException) caught the subtype: {e.GetType().Name}");
        }
        Helper.Info("Middleware can catch ShopException -> ProblemDetails, while");
        Helper.Info("real bugs (NullReference...) stay loud and become 500s.");

        // ── When NOT to create one ───────────────────────────────────────────
        Helper.Heading("When NOT to define a custom exception");
        Helper.Info("Wrong argument?     ArgumentException / ArgumentNullException");
        Helper.Info("Bad object state?   InvalidOperationException");
        Helper.Info("Not supported?      NotSupportedException");
        Helper.Info("The BCL types are well-known — reuse them unless a caller");
        Helper.Info("genuinely needs to HANDLE your case separately.");
    }

    private static void Order(string product, int quantity) =>
        throw new OutOfStockException(product, quantity, available: 2);

    private static void Pay(decimal amount)
    {
        try
        {
            throw new TimeoutException("banka nije odgovorila u 30 s");   // low-level cause
        }
        catch (TimeoutException e)
        {
            throw new PaymentFailedException("timeout prema banci", e);   // wrapped
        }
    }
}
