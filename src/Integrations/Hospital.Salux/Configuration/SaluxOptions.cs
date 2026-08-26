namespace Hospital.Salux.Configuration;

public sealed class SaluxOptions
{
    public const string SectionName = "Salux";

    /// <summary>
    /// Liga o worker de sincronização read-only do Salux.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// SqlServer ou Oracle (instalação típica do Salux).
    /// </summary>
    public string Provider { get; set; } = "SqlServer";

    /// <summary>
    /// Connection string read-only do banco/views do Salux.
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// Postgres da plataforma (checkpoint + idempotência).
    /// Se vazio, usa ConnectionStrings:PatientsDatabase.
    /// </summary>
    public string CheckpointConnectionString { get; set; } = string.Empty;

    public int PollIntervalSeconds { get; set; } = 60;

    public int BatchSize { get; set; } = 200;

    /// <summary>
    /// SQL incremental. Deve retornar:
    /// PatientCode, PatientName, BirthDate, GenderCode, UpdatedAtUtc
    /// e filtrar por checkpoint (@Checkpoint / :Checkpoint).
    /// </summary>
    public string PatientQuery { get; set; } =
        """
        SELECT TOP (@BatchSize)
            CAST(COD_PACIENTE AS varchar(50)) AS PatientCode,
            NOM_PACIENTE AS PatientName,
            CAST(DAT_NASCIMENTO AS date) AS BirthDate,
            CASE
                WHEN SEXO IN ('M', '1', 'Masculino') THEN 1
                WHEN SEXO IN ('F', '2', 'Feminino') THEN 2
                ELSE 0
            END AS GenderCode,
            CAST(DAT_ALTERACAO AS datetimeoffset) AS UpdatedAtUtc
        FROM dbo.VW_HI_PACIENTES
        WHERE DAT_ALTERACAO > @Checkpoint
        ORDER BY DAT_ALTERACAO;
        """;

    /// <summary>
    /// Caminho opcional para arquivo .sql (útil no Docker).
    /// Se existir, substitui PatientQuery.
    /// </summary>
    public string? PatientQueryFile { get; set; }
}
