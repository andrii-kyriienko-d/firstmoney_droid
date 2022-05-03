namespace FirstMoney.Extentions
{
    internal static class ObjectExtentions
    {
        public static string ToPropertiesString(this object obj)
        {
            var props = obj.GetType().GetProperties();
            var result = string.Empty;

            foreach (var prop in props)
            {
                result += $" {prop.Name} = {prop.GetValue(obj)} ";
            }

            return result;
        }

        public static string GetName(this object obj)
        {
            return obj.GetType().Name;
        }
    }
}
