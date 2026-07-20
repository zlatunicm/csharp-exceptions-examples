namespace ExceptionsLab.Examples;

/// <summary>
/// EXCEPTION FILTERS (C# 6) &amp; RETHROWING — the sharp tools.
///
///   catch (HttpException e) when (e.Status == 404)   — the filter runs BEFORE
///   the stack unwinds; if false, the exception keeps flying as if this catch
///   didn't exist. Filters let you catch by CONDITION, not just by type.
///
/// Rethrowing — the eternal code-review comment:
///   throw;        preserves the original stack trace  ✔
///   throw e;      RESETS the stack trace to this line ✘ (evidence destroyed!)
/// </summary>
public static class FiltersDemo
{
    private sealed class HttpException(int status) : Exception($"HTTP {status}")
    {
        public int Status { get; } = status;
    }

    public static void Run()
    {
        // ── Filters catch by condition ───────────────────────────────────────
        Helper.Heading("catch ... when (condition)");
        foreach (var status in new[] { 404, 500, 503 })
        {
            try
            {
                throw new HttpException(status);
            }
            catch (HttpException e) when (e.Status == 404)
            {
                Helper.Info($"{status}: tretiram kao 'nema podatka' — vracam prazno");
            }
            catch (HttpException e) when (e.Status >= 500)
            {
                Helper.Info($"{status}: server error — ovo ide u retry");
            }
        }

        // ── Filters for logging without catching ─────────────────────────────
        Helper.Heading("The log-and-keep-flying trick");
        try
        {
            try
            {
                throw new InvalidOperationException("nesto duboko puklo");
            }
            catch (Exception e) when (Log(e))     // Log returns false!
            {
                Helper.Info("(never runs — the filter said false)");
            }
        }
        catch (InvalidOperationException)
        {
            Helper.Info("outer handler still got the ORIGINAL exception — we logged");
            Helper.Info("mid-flight without disturbing the stack. Sneaky and useful.");
        }

        // ── throw vs throw e ─────────────────────────────────────────────────
        Helper.Heading("throw; vs throw e; — the stack trace evidence");
        try { RethrowCorrectly(); }
        catch (Exception e)
        {
            var origin = e.StackTrace!.Split('\n')[0].Trim();
            Helper.Info($"throw;   origin frame: {Shorten(origin)}   <- still DeepWork!");
        }
        try { RethrowWrong(); }
        catch (Exception e)
        {
            var origin = e.StackTrace!.Split('\n')[0].Trim();
            Helper.Info($"throw e; origin frame: {Shorten(origin)}   <- lies! points at the rethrow");
        }
        Helper.Info("Wrapping instead? throw new AppException(\"context\", e) keeps the");
        Helper.Info("original as InnerException — best of both (example 03).");
    }

    private static bool Log(Exception e)
    {
        Helper.Info($"[log] vidio sam: '{e.Message}' (filter je bio false)");
        return false;                             // never actually catch
    }

    private static void DeepWork() => throw new InvalidOperationException("izvor greske");

    private static void RethrowCorrectly()
    {
        try { DeepWork(); }
        catch (Exception) { throw; }              // ✔ preserves origin
    }

    private static void RethrowWrong()
    {
        try { DeepWork(); }
        catch (Exception e) { throw e; }          // ✘ resets stack trace (CA2200 warns!)
    }

    private static string Shorten(string frame) =>
        frame.Length > 66 ? frame[..66] + "..." : frame;
}
