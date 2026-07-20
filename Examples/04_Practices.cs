namespace ExceptionsLab.Examples;

/// <summary>
/// BEST PRACTICES &amp; ANTI-PATTERNS — the code-review checklist, demonstrated.
///
/// 1. Don't SWALLOW exceptions (empty catch = flying blind).
/// 2. Don't use exceptions for CONTROL FLOW (TryXxx exists — and it's faster).
/// 3. Catch only what you can HANDLE; let the rest fly to a global handler.
/// 4. Validate arguments EARLY (guard clauses throw close to the bug).
/// 5. Fail fast beats limping on with corrupt state.
/// </summary>
public static class PracticesDemo
{
    public static void Run()
    {
        // ── 1. The empty catch ───────────────────────────────────────────────
        Helper.Heading("1) catch {} — the crime scene with no evidence");
        Helper.Info("try { SaveInvoice(); } catch { }   // 'radi na mom stroju'");
        Helper.Info("The invoice silently didn't save. You learn about it from an");
        Helper.Info("angry customer, with NO log, NO trace, a week later.");
        Helper.Info("Minimum acceptable: catch (Exception e) { _log.Error(e); throw; }");

        // ── 2. Exceptions as control flow — measured ─────────────────────────
        Helper.Heading("2) Parse vs TryParse on messy input (measured)");
        string[] inputs = Enumerable.Repeat(new[] { "12", "3,5", "krivo", "7" }, 500)
                                    .SelectMany(x => x).ToArray();   // 2000 items, 25% bad

        var sw = System.Diagnostics.Stopwatch.StartNew();
        int okCount = 0;
        foreach (var s in inputs)
        {
            try { _ = int.Parse(s); okCount++; }
            catch (FormatException) { /* preskoci */ }
        }
        var parseMs = sw.Elapsed.TotalMilliseconds;

        sw.Restart();
        int okCount2 = inputs.Count(s => int.TryParse(s, out _));
        sw.Stop();

        Helper.Info($"Parse+catch: {parseMs:F1} ms   TryParse: {sw.Elapsed.TotalMilliseconds:F2} ms   (isti rezultat: {okCount}/{okCount2} valjanih)");
        Helper.Info("Throwing 1000 exceptions costs real time — EXPECTED failures");
        Helper.Info("deserve a Try pattern or Result<T>, not a catch block.");

        // ── 3. Catch narrow, let the rest fly ────────────────────────────────
        Helper.Heading("3) Handle what you can, propagate the rest");
        foreach (var file in new[] { "postoji.txt", "nema.txt" })
        {
            var content = ReadOrDefault(file);
            Helper.Info($"{file,-12} -> '{content}'");
        }
        Helper.Info("FileNotFound? We have a default — HANDLED. OutOfMemory? Not our");
        Helper.Info("problem to fix here — it flies to the global handler. Precision.");

        // ── 4. Guard clauses ─────────────────────────────────────────────────
        Helper.Heading("4) Guard clauses — throw NEXT to the bug");
        try
        {
            CreateUser(name: "", age: 30);
        }
        catch (ArgumentException e)
        {
            Helper.Info($"guard caught it at the DOOR: {e.Message.Split('\n')[0]}");
        }
        Helper.Info("Without the guard, the empty name explodes 5 layers deeper in the");
        Helper.Info("DB insert — and the stack trace points AWAY from the real bug.");
        Helper.Info("Modern helpers: ArgumentNullException.ThrowIfNull(x),");
        Helper.Info("ArgumentException.ThrowIfNullOrWhiteSpace(s) (.NET 8).");

        // ── 5. The global safety net ─────────────────────────────────────────
        Helper.Heading("5) Every app needs ONE global handler");
        Helper.Info("Console:  top-level try in Main -> log + exit code");
        Helper.Info("ASP.NET:  exception middleware  -> log + ProblemDetails response");
        Helper.Info("UI:       Dispatcher/AppDomain handlers -> log + friendly dialog");
        Helper.Info("Its job: LOG LOUDLY, die gracefully. Not: pretend nothing happened.");
    }

    private static string ReadOrDefault(string file)
    {
        try
        {
            if (file != "postoji.txt")
                throw new FileNotFoundException("nema datoteke", file);
            return "sadrzaj datoteke";
        }
        catch (FileNotFoundException)              // ONLY the case we can handle
        {
            return "(zadani sadrzaj)";
        }
    }

    private static void CreateUser(string name, int age)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);   // .NET 8 one-liner guard
        ArgumentOutOfRangeException.ThrowIfNegative(age);
        Helper.Info($"kreiram korisnika {name}, {age}");
    }
}
