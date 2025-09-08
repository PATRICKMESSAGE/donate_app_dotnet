public class CreateUserViewModel
{
    public string Email { get; set; }
    public string Password { get; set; }
    public List<string> Roles { get; set; } = new List<string>();

    // Add this property to fix the bug
    public List<string> AllRoles { get; set; } = new List<string>();
}

// This line is not part of the C# class and should not be included in the final file
// dotnet add package Microsoft.AspNetCore.Identity.UI