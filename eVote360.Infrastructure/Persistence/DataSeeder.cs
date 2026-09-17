using eVote360.Domain.Entities;
using eVote360.Domain.Entities.Candidate;
using eVote360.Domain.Entities.Citizen;
using eVote360.Domain.Entities.Party;
using eVote360.Domain.Entities.Position;
using eVote360.Domain.Entities.Assignments;
using eVote360.Domain.Enums;
using eVote360.Domain.ValueObjects;
using eVote360.Infrastructure.Security;

// Alias de tipo: las clases Vote/VoteItem/Election viven en un namespace que
// termina con su mismo nombre (eVote360.Domain.Entities.Vote.Vote), lo que
// generaría ambigüedad al usarlas sin calificar.
using ElectionEntity = eVote360.Domain.Entities.Election.Election;
using VoteEntity = eVote360.Domain.Entities.Vote.Vote;
using VoteItemEntity = eVote360.Domain.Entities.Vote.VoteItem;

namespace eVote360.Infrastructure.Persistence;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        var hasher = new Pbkdf2PasswordHasher();

        // Usuarios
        var usuarios = new[]
        {
            ("admin","Admin","Sistema","admin@evote360.com","Admin123!","Administrador"),
            ("abinader","Luis","Abinader","abinader@prm.do","Dirigente123!","Dirigente"),
            ("medina","Danilo","Medina","medina@pld.do","Dirigente123!","Dirigente"),
            ("leonel","Leonel","Fernandez","leonel@fp.do","Dirigente123!","Dirigente"),
        };

        foreach (var u in usuarios)
        {
            if (!context.Usuarios.Any(x => x.NombreUsuario == u.Item1))
            {
                context.Usuarios.Add(new Usuario
                {
                    Nombre = u.Item2,
                    Apellido = u.Item3,
                    Email = new EmailAddress(u.Item4),
                    NombreUsuario = u.Item1,
                    PasswordHash = hasher.HashPassword(u.Item5),
                    Rol = u.Item6,
                    IsActive = true
                });
            }
        }
        await context.SaveChangesAsync();


        // Posiciones
        var puestos = new[]
        {
            ("Presidente","Presidente de la República Dominicana"),
            ("Vicepresidente","Vicepresidente de la República"),
            ("Alcalde","Alcalde Municipal")
        };

        foreach (var p in puestos)
        {
            if (!context.Positions.Any(x => x.Nombre == p.Item1))
                context.Positions.Add(new Position { Nombre = p.Item1, Descripcion = p.Item2, IsActive = true });
        }
        await context.SaveChangesAsync();


        // Partidos
        var partidos = new[]
        {
            ("Partido Revolucionario Moderno","Partido político dominicano","PRM","/images/parties/prm.png"),
            ("Partido de la Liberación Dominicana","Partido político dominicano","PLD","/images/parties/pld.png"),
            ("Fuerza del Pueblo","Partido político dominicano","FP","/images/parties/fp.png"),
        };

        foreach (var p in partidos)
        {
            if (!context.Parties.Any(x => x.Siglas == p.Item3))
                context.Parties.Add(new Party { Nombre=p.Item1, Descripcion=p.Item2, Siglas=p.Item3, LogoPath=p.Item4, IsActive=true });
        }
        await context.SaveChangesAsync();


        // Asignaciones de usuarios a partidos
        var asignaciones = new[]
        {
            ("abinader","PRM"),
            ("medina","PLD"),
            ("leonel","FP"),
        };

        foreach (var a in asignaciones)
        {
            var user = context.Usuarios.FirstOrDefault(x => x.NombreUsuario == a.Item1);
            var party = context.Parties.FirstOrDefault(x => x.Siglas == a.Item2);

            if (user != null && party != null &&
                !context.PartyAssignments.Any(x => x.UsuarioId == user.Id))
            {
                context.PartyAssignments.Add(new PartyAssignments
                {
                    UsuarioId = user.Id,
                    PartyId = party.Id
                });
            }
        }
        await context.SaveChangesAsync();


        // Candidatos
        if (!context.Candidates.Any())
        {
            var prm = context.Parties.First(x => x.Siglas == "PRM");
            var pld = context.Parties.First(x => x.Siglas == "PLD");
            var fp  = context.Parties.First(x => x.Siglas == "FP");

            var pte  = context.Positions.First(x => x.Nombre == "Presidente");
            var vice = context.Positions.First(x => x.Nombre == "Vicepresidente");
            var alc  = context.Positions.First(x => x.Nombre == "Alcalde");

            context.Candidates.AddRange(new[]
            {
                // PRM
                new Candidate { Nombre="Luis",Apellido="Abinader Corona",PartyId=prm.Id,PositionId=pte.Id,IsActive=true },
                new Candidate { Nombre="Raquel",Apellido="Peña",PartyId=prm.Id,PositionId=vice.Id,IsActive=true },
                new Candidate { Nombre="Carolina",Apellido="Mejía",PartyId=prm.Id,PositionId=alc.Id,IsActive=true },

                // PLD
                new Candidate { Nombre="Danilo",Apellido="Medina",PartyId=pld.Id,PositionId=pte.Id,IsActive=true },
                new Candidate { Nombre="Margarita",Apellido="Cedeño",PartyId=pld.Id,PositionId=vice.Id,IsActive=true },
                new Candidate { Nombre="Abel",Apellido="Martínez",PartyId=pld.Id,PositionId=alc.Id,IsActive=true },

                // FP
                new Candidate { Nombre="Leonel",Apellido="Fernández",PartyId=fp.Id,PositionId=pte.Id,IsActive=true },
                new Candidate { Nombre="Omar",Apellido="Fernández",PartyId=fp.Id,PositionId=vice.Id,IsActive=true },
                new Candidate { Nombre="Radhamés",Apellido="Jiménez",PartyId=fp.Id,PositionId=alc.Id,IsActive=true },
            });

            await context.SaveChangesAsync();
        }


        // Ciudadanos
        var ciudadanosSeed = new[]
        {
            ("Albertson","Terrero López","20241949@itla.edu.do","001-1234567-1"),
            ("María","Rodríguez López","maria.rodriguez@email.com","001-1234568-2"),
            ("Pedro","Martínez Santos","pedro.martinez@email.com","001-1234569-3"),
            ("Ana","González Jiménez","ana.gonzalez@email.com","001-1234570-4"),
            ("Carlos","Hernández Díaz","carlos.hernandez@email.com","001-1234571-5"),
            ("Laura","López Martín","laura.lopez@email.com","001-1234572-6"),
            ("José","García Fernández","jose.garcia@email.com","001-1234573-7"),
            ("Carmen","Sánchez Ruiz","carmen.sanchez@email.com","001-1234574-8"),
            ("Miguel","Díaz Moreno","miguel.diaz@email.com","001-1234575-9"),
            ("Isabel","Muñoz Álvarez","isabel.munoz@email.com","001-1234576-0"),
            ("Francisco","Romero Silva","francisco.romero@email.com","001-1234577-1"),
            ("Lucía","Torres Castillo","lucia.torres@email.com","001-1234578-2"),
            ("Antonio","Ramírez Herrera","antonio.ramirez@email.com","001-1234579-3"),
            ("Rosa","Vargas Medina","rosa.vargas@email.com","001-1234580-4"),
            ("Manuel","Castro Ortega","manuel.castro@email.com","001-1234581-5"),
            ("Teresa","Rubio Delgado","teresa.rubio@email.com","001-1234582-6"),
            ("Javier","Morales Ramos","javier.morales@email.com","001-1234583-7"),
            ("Elena","Serrano Gil","elena.serrano@email.com","001-1234584-8"),
            ("Rafael","Blanco Vega","rafael.blanco@email.com","001-1234585-9"),
            ("Pilar","Molina Suárez","pilar.molina@email.com","001-1234586-0"),
            ("Roberto","Navarro Cruz","roberto.navarro@email.com","001-1234587-1"),
            ("Dolores","Pascual Rojas","dolores.pascual@email.com","001-1234588-2"),
            ("Fernando","Sanz Cortés","fernando.sanz@email.com","001-1234589-3"),
            ("Cristina","Cano Flores","cristina.cano@email.com","001-1234590-4"),
            ("Sergio","Prieto León","sergio.prieto@email.com","001-1234591-5"),
            ("Beatriz","Calvo Peña","beatriz.calvo@email.com","001-1234592-6"),
            ("Raúl","Hidalgo Mora","raul.hidalgo@email.com","001-1234593-7"),
            ("Mónica","Campos Reyes","monica.campos@email.com","001-1234594-8"),
            ("Alberto","Vidal Méndez","alberto.vidal@email.com","001-1234595-9"),
            ("Silvia","Parra Domínguez","silvia.parra@email.com","001-1234596-0"),
            ("Andrés","Jiménez Carrillo","andres.jimenez@email.com","001-1234597-1"),
            ("Gloria","Méndez Estévez","gloria.mendez@email.com","001-1234598-2"),
            ("Emilio","Luna Fuentes","emilio.luna@email.com","001-1234599-3"),
            ("Rocío","Santos Marín","rocio.santos@email.com","001-1234600-4"),
            ("Víctor","Peña Aguilar","victor.pena@email.com","001-1234601-5"),
            ("Amparo","Guerrero Cabrera","amparo.guerrero@email.com","001-1234602-6"),
            ("Ricardo","Benítez Iglesias","ricardo.benitez@email.com","001-1234603-7"),
            ("Guadalupe","Ortiz Núñez","guadalupe.ortiz@email.com","001-1234604-8"),
            ("Tomás","Velasco Santiago","tomas.velasco@email.com","001-1234605-9"),
            ("Inmaculada","Ferrer Lorenzo","inmaculada.ferrer@email.com","001-1234606-0"),
            ("Pablo","Ibáñez Vicente","pablo.ibanez@email.com","001-1234607-1"),
            ("Encarna","Carmona Pascual","encarna.carmona@email.com","001-1234608-2"),
            ("Óscar","Durán Lozano","oscar.duran@email.com","001-1234609-3"),
            ("Montserrat","Garrido Soler","montserrat.garrido@email.com","001-1234610-4"),
            ("Ignacio","Caballero Montero","ignacio.caballero@email.com","001-1234611-5"),
            ("Remedios","Moya Gallego","remedios.moya@email.com","001-1234612-6"),
            ("Julio","Márquez Vázquez","julio.marquez@email.com","001-1234613-7"),
            ("Consuelo","León Gutiérrez","consuelo.leon@email.com","001-1234614-8"),
            ("Alfredo","Herrero Bravo","alfredo.herrero@email.com","001-1234615-9"),
            ("Milagros","Román Giménez","milagros.roman@email.com","001-1234616-0"),
        };
        foreach (var c in ciudadanosSeed)
        {
            var cedula = new NationalId(c.Item4);

            if (!context.Citizens.Any(x => x.NumeroDocumento == cedula))
            {
                context.Citizens.Add(new Citizen
                {
                    Nombre = c.Item1,
                    Apellido = c.Item2,
                    Email = new EmailAddress(c.Item3),
                    NumeroDocumento = cedula,
                    IsActive = true
                });
            }
        }

        await context.SaveChangesAsync();


        // =========================================================
        // ELECCIÓN CON VOTACIÓN COMPLETA Y RESULTADOS
        // Se siembra una elección ya FINALIZADA con votos reales, de modo
        // que la pantalla "Ver Resultados" muestre un conteo con ganadores.
        // Resultado buscado:
        //   Presidente      -> Luis Abinader Corona (PRM)
        //   Vicepresidente  -> Raquel Peña (PRM)
        //   Alcalde         -> Abel Martínez (PLD)  <- gana otro partido
        // =========================================================
        const string nombreEleccion = "Elecciones Generales 2024";

        if (!context.Elections.Any(x => x.Nombre == nombreEleccion))
        {
            var eleccion = new ElectionEntity
            {
                Nombre = nombreEleccion,
                FechaRealizacion = new DateTime(2024, 5, 19),
                Estado = ElectionState.Finalizada,
                CreatedAt = DateTime.UtcNow
            };
            context.Elections.Add(eleccion);
            await context.SaveChangesAsync();

            var puestoPte  = context.Positions.First(x => x.Nombre == "Presidente");
            var puestoVice = context.Positions.First(x => x.Nombre == "Vicepresidente");
            var puestoAlc  = context.Positions.First(x => x.Nombre == "Alcalde");

            // Candidatos por puesto
            var abinader = context.Candidates.First(x => x.Apellido == "Abinader Corona");
            var medina   = context.Candidates.First(x => x.Apellido == "Medina");
            var leonel   = context.Candidates.First(x => x.Nombre == "Leonel" && x.Apellido == "Fernández");

            var raquel   = context.Candidates.First(x => x.Apellido == "Peña");
            var margarita= context.Candidates.First(x => x.Apellido == "Cedeño");
            var omar     = context.Candidates.First(x => x.Nombre == "Omar");

            var carolina = context.Candidates.First(x => x.Apellido == "Mejía");
            var abel     = context.Candidates.First(x => x.Apellido == "Martínez");
            var radhames = context.Candidates.First(x => x.Nombre == "Radhamés");

            // Construye una lista de "papeletas" repitiendo cada opción tantas
            // veces como votos deba recibir. null = voto por "Ninguno".
            static List<eVote360.Domain.Entities.Candidate.Candidate?> Papeletas(
                params (eVote360.Domain.Entities.Candidate.Candidate? candidato, int votos)[] reparto)
            {
                var lista = new List<eVote360.Domain.Entities.Candidate.Candidate?>();
                foreach (var (candidato, votos) in reparto)
                    for (int i = 0; i < votos; i++)
                        lista.Add(candidato);
                return lista;
            }

            // 40 votantes de los 50 ciudadanos del padrón (abstención realista)
            var votantes = context.Citizens.OrderBy(x => x.Id).Take(40).ToList();

            var votosPresidente = Papeletas(
                (abinader, 20),   // PRM gana
                (medina,   12),
                (leonel,    7),
                (null,      1));  // Ninguno

            var votosVice = Papeletas(
                (raquel,   19),   // PRM gana
                (margarita,13),
                (omar,      7),
                (null,      1));

            var votosAlcalde = Papeletas(
                (abel,     18),   // PLD gana -> partido distinto al presidencial
                (carolina, 14),
                (radhames,  7),
                (null,      1));

            var random = new Random(2024); // semilla fija: resultados reproducibles
            votosPresidente = votosPresidente.OrderBy(_ => random.Next()).ToList();
            votosVice       = votosVice.OrderBy(_ => random.Next()).ToList();
            votosAlcalde    = votosAlcalde.OrderBy(_ => random.Next()).ToList();

            for (int i = 0; i < votantes.Count; i++)
            {
                var voto = new VoteEntity
                {
                    CitizenId = votantes[i].Id,
                    ElectionId = eleccion.Id,
                    FechaVoto = eleccion.FechaRealizacion.AddHours(8).AddMinutes(i * 7)
                };

                voto.VoteItems.Add(new VoteItemEntity
                {
                    PositionId = puestoPte.Id,
                    CandidateId = votosPresidente[i]?.Id,
                    PartyId = votosPresidente[i]?.PartyId
                });
                voto.VoteItems.Add(new VoteItemEntity
                {
                    PositionId = puestoVice.Id,
                    CandidateId = votosVice[i]?.Id,
                    PartyId = votosVice[i]?.PartyId
                });
                voto.VoteItems.Add(new VoteItemEntity
                {
                    PositionId = puestoAlc.Id,
                    CandidateId = votosAlcalde[i]?.Id,
                    PartyId = votosAlcalde[i]?.PartyId
                });

                context.Votes.Add(voto);
            }

            await context.SaveChangesAsync();
        }

        Console.WriteLine("====================================================");
        Console.WriteLine("  Base de datos inicializada correctamente");
        Console.WriteLine("====================================================");
        Console.WriteLine("  admin / Admin123!");
        Console.WriteLine("  abinader / Dirigente123! (PRM)");
        Console.WriteLine("  medina / Dirigente123! (PLD)");
        Console.WriteLine("  leonel / Dirigente123! (FP)");
        Console.WriteLine("  Elector: 001-1234567-1 (Albertson Terrero López)");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine("  Elección sembrada: Elecciones Generales 2024 (Finalizada)");
        Console.WriteLine("  40 votantes de 50 ciudadanos del padrón");
        Console.WriteLine("  Presidente     -> Luis Abinader Corona (PRM)  20 votos");
        Console.WriteLine("  Vicepresidente -> Raquel Peña (PRM)           19 votos");
        Console.WriteLine("  Alcalde        -> Abel Martínez (PLD)         18 votos");
        Console.WriteLine("====================================================\n");
    }
}