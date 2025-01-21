using Microsoft.AspNetCore.Http;
using System.Text.Json;

public static class SessionExtensions
{
    public static void SetObject(this ISession session, string key, object value)
    {
        // Serializa el objeto a JSON y lo guarda en la sesión
        session.SetString(key, JsonSerializer.Serialize(value));
    }

    public static T GetObject<T>(this ISession session, string key)
    {
        var value = session.GetString(key);

        // Si el valor no existe, devuelve el valor predeterminado de T
        return value == null ? default(T) : JsonSerializer.Deserialize<T>(value);
    }
}



