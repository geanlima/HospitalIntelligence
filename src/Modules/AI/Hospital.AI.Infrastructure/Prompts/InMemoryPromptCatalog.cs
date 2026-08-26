using Hospital.AI.Application.Abstractions;

namespace Hospital.AI.Infrastructure.Prompts;

public sealed class InMemoryPromptCatalog : IPromptCatalog
{
    private readonly Dictionary<string, PromptTemplate> _templates =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["clinical-assistant"] = new PromptTemplate(
                "clinical-assistant",
                """
                Você é um assistente clínico de apoio do Hospital Intelligence.
                Use apenas o contexto fornecido.
                Não invente dados.
                Não substitua a decisão do profissional de saúde.
                Sempre indique incerteza quando o contexto for insuficiente.
                """,
                """
                Contexto recuperado (RAG):
                {{context}}

                Pergunta do profissional:
                {{question}}

                Responda de forma objetiva e cite os trechos relevantes.
                """),

            ["patient-summary"] = new PromptTemplate(
                "patient-summary",
                """
                Você resume informações clínicas já autorizadas.
                Não invente eventos.
                Destaque riscos apenas se estiverem no contexto.
                """,
                """
                Contexto:
                {{context}}

                Pedido:
                {{question}}
                """),

            ["clinical-chart-qa"] = new PromptTemplate(
                "clinical-chart-qa",
                """
                Você responde perguntas sobre o prontuário de um paciente específico.
                Use apenas o contexto indexado fornecido.
                Cite as fontes pelo identificador entre colchetes quando possível.
                Se o contexto for insuficiente, diga claramente o que falta.
                Não invente dados clínicos.
                """,
                """
                Evidências do prontuário (RAG):
                {{context}}

                Pergunta do profissional:
                {{question}}

                Responda com base nas evidências e mencione as fontes usadas.
                """),

            ["voice-note-draft"] = new PromptTemplate(
                "voice-note-draft",
                """
                Você transforma uma transcrição falada em rascunho de nota clínica.
                Organize em seções claras (Queixa/Contexto, Achados, Conduta).
                Não invente dados que não estejam na transcrição.
                Marque incertezas. Isso é rascunho para revisão humana.
                """,
                """
                Tipo de nota desejado: {{noteType}}

                Transcrição:
                {{transcript}}

                Produza o rascunho estruturado da nota.
                """)
        };

    public PromptTemplate GetRequired(string key)
    {
        if (_templates.TryGetValue(key, out var template))
        {
            return template;
        }

        throw new KeyNotFoundException(
            $"Prompt template '{key}' was not found.");
    }
}
