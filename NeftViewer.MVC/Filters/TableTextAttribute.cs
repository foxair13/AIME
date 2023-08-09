namespace NeftViewer.MVC.Filters
{
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class TableTextAttribute : Attribute
    {
        public string Text { get; }

        public TableTextAttribute(string text)
        {
            Text = text;
        }
    }

}
