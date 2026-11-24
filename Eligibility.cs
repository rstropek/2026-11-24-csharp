sealed record TooSoon(DateOnly EarliestDate);
sealed record NotOffered(string Requested);
sealed record TooYoung;
sealed record CovidNotIndicated;

readonly union Eligibility(Vaccine, TooSoon, NotOffered, TooYoung, CovidNotIndicated);

static class Rules
{
    public static Eligibility Check(Person person, string requested, IEnumerable<VaccinationRecord> history, DateOnly today)
    {
        Vaccine? wanted = requested.ToLowerInvariant() switch
        {
            "flu" => new Influenza(HighDose: false),
            "covid" => new Covid(),
            _ => null,
        };
        if (wanted is null) return new NotOffered(requested);

        int age = person.AgeOn(today);
        if (age < 18) return new TooYoung();

        var past = history.Where(r => r.InsuranceNumber == person.InsuranceNumber).ToList();
        DateOnly seasonStart = new(today.Month >= 9 ? today.Year : today.Year - 1, 9, 1);

        return wanted switch
        {
            Influenza when LastDose<Influenza>(past) >= seasonStart
                => new TooSoon(seasonStart.AddYears(1)),
            Influenza
                => new Influenza(HighDose: age >= 60),
            Covid when age < 60 && !person.RiskFactors
                => new CovidNotIndicated(),
            Covid when LastDose<Covid>(past) is { } last && last.AddYears(1) > today
                => new TooSoon(last.AddYears(1)),
            Covid c
                => c,
        };
    }

    private static DateOnly? LastDose<T>(IEnumerable<VaccinationRecord> past) where T : Vaccine =>
        past.Where(r => r.Vaccine is T).Max(r => (DateOnly?)r.Date);
}
