namespace WSRS_SWAFO.ViewModels
{
    public class ToastViewModel
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public string CssClassName { get; set; }
        public ToastButton? Button { get; set; } = null;
    }

    public class ToastButton
    {
        public string Name { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public Dictionary<string, object>? RouteValues { get; set; }
    }
}
