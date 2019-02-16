namespace RedirectProcessor.Model
{
    class Route
    {
        public string Path { get; set; }
        public string RedirectsTo { get; set; }
        public bool IsRedirect => !string.IsNullOrEmpty(RedirectsTo);
    }
}
