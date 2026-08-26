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
