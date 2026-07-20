using ExceptionsLab;
using ExceptionsLab.Examples;

// ─────────────────────────────────────────────────────────────────────────────
//  EXCEPTIONS  —  part 07 of the C# practice series
// ─────────────────────────────────────────────────────────────────────────────

var demos = new (string Title, Action Method)[]
{
    ("01 — try / catch / finally mechanics",      MechanicsDemo.Run),
    ("02 — Exception filters & rethrow",          FiltersDemo.Run),
    ("03 — Custom exceptions",                    CustomDemo.Run),
    ("04 — Best practices & anti-patterns",       PracticesDemo.Run),
};

if (args.Length > 0 && int.TryParse(args[0], out var number) && number >= 1 && number <= demos.Length)
{
    RunDemo(demos[number - 1]);
}
else
{
    Console.WriteLine("Running ALL examples. (Tip: `dotnet run 3` runs only example #3.)");
    foreach (var demo in demos)
        RunDemo(demo);
}

static void RunDemo((string Title, Action Method) demo)
{
    Console.WriteLine();
    Console.WriteLine(new string('═', 62));
    Console.WriteLine($"  {demo.Title}");
    Console.WriteLine(new string('═', 62));
    demo.Method();
}
