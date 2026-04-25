public class Paciente
{
    public string? PacienteId { get; set; }

    public int NivelGravedad { get; set; }

    public string Estado { get; set; }

    public string MedicoResponsable { get; set; }

    public DateTime FechaIngreso { get; set; }
}