public class Paciente
{
    public string? PacienteId { get; set; }

    // Propiedad para el nombre del paciente
    public string NombreCompleto { get; set; }

    // Propiedad sugerida para los síntomas (requerido en el POST)
    public string Sintomas { get; set; }

    public int NivelGravedad { get; set; }

    public string Estado { get; set; }

    public string MedicoResponsable { get; set; }

    public DateTime FechaIngreso { get; set; }
}