-- Template de query Salux (SqlServer).
-- Ajuste nomes de view/colunas com a TI do hospital.
-- Colunas obrigatórias: PatientCode, PatientName, BirthDate, GenderCode, UpdatedAtUtc
-- Parâmetros: @Checkpoint, @BatchSize

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
