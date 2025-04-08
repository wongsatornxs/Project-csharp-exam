namespace Exam.Controllers.Model
{
    public class TaskInput
    {
        public List<Dictionary<string, int>> Tasks { get; set; } = new();
        public List<Dictionary<string, int>> Programmers { get; set; } = new();
    }
}
