using System.Text.Json;

foreach (var file in Directory.GetFiles(Path.Combine("data", "inbox"), "*.json").Order())
{
    Console.WriteLine(Path.GetFileName(file));

    PortalRequest request;
    try { request = Storage.Read<PortalRequest>(file); }
    catch (JsonException ex)
    {
        Console.WriteLine($"  rejected: {ex.Message}");
        Console.WriteLine();
        continue;
    }

    Console.WriteLine($"  from {request.Email}");
    foreach (var member in request.Members)
        Console.WriteLine($"  {member.Name,-16} born {member.BirthDate:d MMM yyyy}, wants {string.Join(", ", member.Vaccines)}");
    Console.WriteLine();
}