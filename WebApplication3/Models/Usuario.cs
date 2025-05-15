using Microsoft.AspNetCore.Identity;

public class Usuario : IdentityUser
{
    public bool Activo { get; set; } = true;  // Campo para saber si el usuario está activo
    public string Nombre { get; set; }
    public string Rol { get; set; }
}
