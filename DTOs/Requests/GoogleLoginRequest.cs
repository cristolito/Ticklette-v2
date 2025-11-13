// DTO para login con Google
public class GoogleLoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string GoogleToken { get; set; } = string.Empty; // Para validación en producción
}
