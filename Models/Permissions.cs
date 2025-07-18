namespace TrainingCenter_Api.Models
{
    public class Permissions
    {
        public static Dictionary<string, List<string>> AllPermissions => new()
        {
            { "Department", new List<string> { "View", "Create", "Edit", "Delete" } },
            // another module
        };

        public static List<string> GenerateAll()
        {
            return AllPermissions.SelectMany(module =>
                module.Value.Select(action => $"{module.Key}.{action}")
            ).ToList();
        }
    }
}