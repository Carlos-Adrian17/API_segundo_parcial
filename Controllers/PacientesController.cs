using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class PacientesController : ControllerBase
{
    private readonly AppDbContext _context;

    // Lista oficial de médicos autorizados (requisito del examen)
    private static readonly string[] MedicosAutorizados =
    {
        "MED-1010",
        "MED-2020",
        "MED-3030",
        "MED-4040",
        "MED-5050"
    };

    public PacientesController(AppDbContext context)
    {
        _context = context;
    }

    // ✅ GET: api/Pacientes
    // Obtiene los pacientes y los ordena MANUALMENTE (sin ORDER BY)
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Paciente>>> GetPacientes()
    {
        var pacientes = await _context.Pacientes_Carlos_Marroquin_3364
            .ToListAsync();

        // Ordenamiento manual (Burbuja)
        for (int i = 0; i < pacientes.Count - 1; i++)
        {
            for (int j = 0; j < pacientes.Count - i - 1; j++)
            {
                bool cambiar =
                    pacientes[j].NivelGravedad < pacientes[j + 1].NivelGravedad ||
                    (pacientes[j].NivelGravedad == pacientes[j + 1].NivelGravedad &&
                     pacientes[j].FechaIngreso > pacientes[j + 1].FechaIngreso);

                if (cambiar)
                {
                    var temp = pacientes[j];
                    pacientes[j] = pacientes[j + 1];
                    pacientes[j + 1] = temp;
                }
            }
        }

        return Ok(pacientes);
    }

    // ✅ POST: api/Pacientes
    // Registrar nuevo paciente con TODAS las validaciones
    [HttpPost]
    public async Task<ActionResult<Paciente>> PostPaciente([FromBody] Paciente paciente)
    {
        // 🔒 Validación de médico autorizado
        if (!MedicosAutorizados.Contains(paciente.MedicoResponsable))
        {
            return Unauthorized("Médico no autorizado");
        }

        // 🚨 Validación de capacidad crítica
        if (paciente.NivelGravedad == 5)
        {
            int criticosEnEspera = await _context.Pacientes_Carlos_Marroquin_3364
                .CountAsync(p =>
                    p.NivelGravedad == 5 &&
                    p.Estado == "En espera");

            if (criticosEnEspera >= 5)
            {
                return BadRequest(
                    "Capacidad máxima alcanzada. Redirección inmediata a otro hospital sugerida."
                );
            }
        }

        // 🆔 Generación del ID PAC-2026-XXX
        int correlativo =
            await _context.Pacientes_Carlos_Marroquin_3364.CountAsync() + 1;

        paciente.PacienteId = $"PAC-2026-{correlativo:D3}";
        paciente.FechaIngreso = DateTime.Now;

        _context.Pacientes_Carlos_Marroquin_3364.Add(paciente);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetPacientes), paciente);
    }

    // ✅ PUT: api/Pacientes/PAC-2026-001
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPaciente(string id, [FromBody] Paciente paciente)
    {
        if (id != paciente.PacienteId)
        {
            return BadRequest("El ID de la URL no coincide con el del objeto.");
        }

        _context.Entry(paciente).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ DELETE: api/Pacientes/PAC-2026-001
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePaciente(string id)
    {
        var paciente = await _context.Pacientes_Carlos_Marroquin_3364
            .FindAsync(id);

        if (paciente == null)
        {
            return NotFound();
        }

        _context.Pacientes_Carlos_Marroquin_3364.Remove(paciente);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // ✅ Endpoint de prueba (opcional)
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("API funcionando correctamente");
    }
}