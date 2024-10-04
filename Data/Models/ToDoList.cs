namespace TeaWork.Data.Models
{
    public class ToDoList
    {
        public int Id { get; set; }
        public virtual Project? Project { get; set; }
        public virtual List<ProjectTask>? Tasks { get; set; }
    }
}
