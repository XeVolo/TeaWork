﻿namespace TeaWork.Components.Schedule
{
    public class PrivateAppointment
    {
        public int? Id { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Text { get; set; }
        public string? Description { get; set; }
        public string? Color { get; set; }
    }
}
