using System.Text;

public static class EnumerableExtensions {
    public static string GetHtml<T>(this IEnumerable<T> recipes)
    {
        Type type = typeof(T);

        var props = type.GetProperties();

        StringBuilder sb = new StringBuilder(100);
        sb.Append("<a href=\"Home\">Home</a>");
        sb.Append("<ul>");

        foreach (var recipe in recipes)
        {
            foreach (var prop in props)
            {
                sb.Append($"<li><span>{prop.Name}: </span>{prop.GetValue(recipe)}</li>");
            }
            sb.Append("<br/>");
        }
        sb.Append("</ul>");

        return sb.ToString();
    }
}