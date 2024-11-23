namespace TeaWork.Logic.Dto
{
    public class ReportForm
    {
        public string? Summary { get; set; }
        public List<UserForm> UserForms { get; set; }
        public string? AdditionalComment { get; set; }
    }
    public class UserForm
    {
        public string? User {  get; set; }
        public string? Descriprion { get; set; }
    }
}
