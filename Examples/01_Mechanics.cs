namespace ExceptionsLab.Examples;

/// <summary>
/// try / catch / finally — the mechanics, precisely.
///
/// - An exception UNWINDS the stack until a matching catch is found; every
///   finally on the way runs. No handler anywhere -> process crashes.
/// - catch blocks match TOP-DOWN — order from most to least specific.
/// - finally runs on success, on exception, even on 'return' from try.
/// - Exceptions are for EXCEPTIONAL situations: they cost ~microseconds when
///   thrown (stack trace capture!) but ~nothing when not thrown.
/// </summary>
public static class MechanicsDemo
{
    public static void Run()
    {
        // ── Matching order matters ───────────────────────────────────────────
        Helper.Heading("catch blocks match top-down");
        try
        {
            int[] numbers = [1, 2, 3];
            Console.WriteLine(numbers[10]);
        }
        catch (IndexOutOfRangeException e)      // specific FIRST
        {
            Helper.Info($"specific catch: {e.GetType().Name}");
        }
        catch (Exception)                       // general LAST (compiler enforces order)
        {
            Helper.Info("(never reached for index errors)");
        }

        // ── finally always runs ──────────────────────────────────────────────
        Helper.Heading("finally — success, failure, even early return");
        Helper.Info($"WithReturn() gave: {WithReturn()}");

        try
        {
            try
            {
                throw new InvalidOperationException("dublja greska");
            }
            finally
            {
                Helper.Info("inner finally ran WHILE the exception was in flight");
            }
        }
        catch (InvalidOperationException)
        {
            Helper.Info("...and only then did the outer catch get it");
        }

        // ── The stack unwinds through methods ────────────────────────────────
        Helper.Heading("Unwinding through the call stack");
        try
        {
            Level1();
        }
        catch (InvalidOperationException e)
        {
            Helper.Info($"caught at the top: '{e.Message}'");
            var frames = e.StackTrace!.Split('\n').Length;
            Helper.Info($"StackTrace recorded {frames} frames (Level3 -> Level2 -> Level1)");
        }

        // ── What an Exception object carries ─────────────────────────────────
        Helper.Heading("Anatomy of an Exception");
        try { Level1(); }
        catch (Exception e)
        {
            Helper.Info($"Message:    {e.Message}");
            Helper.Info($"Type:       {e.GetType().FullName}");
            Helper.Info($"TargetSite: {e.TargetSite?.Name}");
            Helper.Info("StackTrace: (captured at THROW, not at catch)");
            Helper.Info("InnerException: the original cause when wrapped (example 03)");
        }

        // ── Cost model ───────────────────────────────────────────────────────
        Helper.Heading("Cost: cheap to have, expensive to throw");
        Helper.Info("A try block costs ~nothing at runtime. THROWING costs a lot");
        Helper.Info("(object + stack trace + unwinding) — that's why exceptions are");
        Helper.Info("for exceptional paths, and TryParse exists for the expected ones.");
    }

    private static string WithReturn()
    {
        try { return "iz try-a"; }
        finally { Helper.Info("finally ran even though try RETURNED"); }
    }

    private static void Level1() => Level2();
    private static void Level2() => Level3();
    private static void Level3() => throw new InvalidOperationException("puklo na dnu");
}
