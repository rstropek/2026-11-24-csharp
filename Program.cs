using System.Text.Json;

// A fixed "today" (the workshop day), so the output is the same on every run.
DateOnly today = new(2026, 11, 24);

var history = Storage.Read(Path.Combine("data", "history.json"), StorageContext.Default.ListVaccinationRecord);

foreach (var file in Directory.GetFiles(Path.Combine("data", "inbox"), "*.json").Order())
{
    Console.WriteLine(Path.GetFileName(file));

    PortalRequest request;
    try { request = Storage.Read(file, StorageContext.Default.PortalRequest); }
    catch (JsonException ex)
    {
        Console.WriteLine($"  rejected: {ex.Message}");
        Console.WriteLine();
        continue;
    }

    foreach (var member in request.Members)
    {
        var person = member.ToPerson();
        foreach (var requested in member.Vaccines)
        {
            Eligibility decision = Rules.Check(person, requested, history, today);
            Console.WriteLine($"  {person.Name,-16} {requested,-6} {decision switch
            {
                Vaccine v => $"yes: {v}",
                TooSoon t => $"not yet, earliest {t.EarliestDate:d MMM yyyy}",
                NotOffered n => $"no: '{n.Requested}' is not offered here",
                TooYoung => "no: under 18, please see your paediatrician",
                CovidNotIndicated => "no: COVID-19 under 60 without risk factors, ask your doctor",
            }}");
        }
    }
    Console.WriteLine();
}