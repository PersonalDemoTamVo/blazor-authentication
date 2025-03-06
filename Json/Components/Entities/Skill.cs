namespace Json.Components.Entities
{
    public class Skill
    {
        public string Name { get; set; } = "";
        public List<string> Versions { get; set; } = new();
        public string Group { get; set; } = "";
        public string Category { get; set; } = "";
        public List<string> Dependencies { get; set; } = new();
        public List<string> RelatedSkills { get; set; } = new();
    }
}