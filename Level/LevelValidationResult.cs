namespace Swinburne_OOP_HD
{
    public class LevelValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> ErrorMessages { get; set; } = new List<string>();
    }
}