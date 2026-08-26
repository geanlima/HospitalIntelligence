# ADR-0005 — Fundação de IA desacoplada do Domain clínico

## Status

Aceito.

## Contexto

O Hospital Intelligence precisa evoluir para casos de uso de IA
(resumo clínico, busca em linguagem natural, auditoria, alertas),
sem contaminar o Domain clínico com detalhes de provedores
(OpenAI, Azure, embeddings, vector DB).

Este projeto também é trilha de estudo de IA aplicada.

## Decisão

1. Criar o módulo `Hospital.AI` separado:
   - `Contracts`
   - `Application` (ports + use cases)
   - `Infrastructure` (adapters)

2. A Application define portas:
   - `ILlmProvider`
   - `IEmbeddingService`
   - `IVectorStore`
   - `IPromptCatalog`
   - `IAiGuardrail`
   - `IRagRetriever`
   - `IAiAuditStore`

3. O Domain clínico (Patients, Admissions, etc.) **não** referencia IA.

4. Nesta fase, o LLM continua Mock (estudo sem chave de API),
   mas o vector store já usa **PostgreSQL + pgvector**:

```text
Pergunta
  → Guardrail (entrada)
  → Prompt Catalog
  → Embedding
  → pgvector (cosine distance)
  → LLM Mock
  → Guardrail (saída)
  → Auditoria + citações de fonte
```

5. Trocar Mock LLM por OpenAI/Azure no futuro deve ser troca de
   Infrastructure, sem reescrever o use case `AskAiHandler`.

## Conceitos estudados

- **LLM**: modelo de linguagem que gera texto.
- **Embedding**: vetor numérico que representa significado semântico.
- **Vector Store / pgvector**: busca por similaridade entre vetores.
- **RAG**: Retrieval-Augmented Generation — recupera contexto antes de gerar.
- **Prompt Management**: templates versionáveis e reutilizáveis.
- **Guardrails**: filtros de segurança/qualidade na entrada e saída.
- **Source grounding**: resposta acompanhada das fontes usadas.

## Consequências

Benefícios:

- aprendizado incremental sem custo de API no início;
- arquitetura pronta para provedor real;
- Domain clínico preservado;
- auditoria e rastreamento de fontes desde o início.

Riscos / próximos passos:

- Mock não avalia qualidade clínica real;
- InMemory não substitui pgvector em produção;
- Fase 15 deve indexar dados reais do prontuário.
