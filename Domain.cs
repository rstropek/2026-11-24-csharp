using System.Text.Json.Serialization;

[JsonPolymorphic(InferClosedTypePolymorphism = true)]
closed record Vaccine
{
    public sealed override string ToString() => this switch
    {
        Influenza { HighDose: true } => "Influenza (high dose)",
        Influenza => "Influenza",
        Covid => "COVID-19",
    };
}

sealed record Influenza(bool HighDose) : Vaccine;

sealed record Covid : Vaccine;

sealed record Person(string InsuranceNumber, string Name, DateOnly BirthDate, bool RiskFactors)
{
    public int AgeOn(DateOnly day)
    {
        int age = day.Year - BirthDate.Year;
        return BirthDate > day.AddYears(-age) ? age - 1 : age;
    }
}

sealed record VaccinationRecord(string InsuranceNumber, Vaccine Vaccine, DateOnly Date);
