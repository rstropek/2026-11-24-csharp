using System.Text.Json.Serialization;

sealed record PortalPerson(
    string Name, 
    DateOnly BirthDate, 
    string InsuranceNumber, 
    string[] Vaccines, 
    bool RiskFactors = false);

sealed record SingleRequest(string Email, PortalPerson Person);
sealed record FamilyRequest(string Email, PortalPerson[] Members);

[JsonUnion(TypeClassifier = typeof(JsonUnionTypeStructuralClassifier))]
readonly union PortalRequest(SingleRequest, FamilyRequest)
{
    public string Email => this switch
    {
        SingleRequest s => s.Email,
        FamilyRequest f => f.Email,
    };

    public PortalPerson[] Members => this switch
    {
        SingleRequest s => [s.Person],
        FamilyRequest f => f.Members,
    };
}