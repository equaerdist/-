namespace backend_iGamingBot.Infrastructure
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ColumnLabelAttribute : Attribute
    {
        public string ColumnName { get; }

        public ColumnLabelAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
