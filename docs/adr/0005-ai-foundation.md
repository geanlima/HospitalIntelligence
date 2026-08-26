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

4. Pipeline padrão:

```text
Pergunta
  → Guardrail (entrada)
  → Prompt Catalog
  → Embedding
  → pgvector (cosine distance)
  → LLM (Mock | OpenAI-compatible)
  → Guardrail (saída)
  → Auditoria + citações de fonte
```

5. LLM:
   - **Default**: `Mock` (estudo sem chave de API).
   - **Real**: `OpenAiCompatibleLlmProvider` via Chat Completions HTTP.
     Aliases de config: `OpenAICompatible`, `OpenAI`, `Ollama`
     (ex.: Ollama em `http://localhost:11434/v1`).

6. Trocar de Mock para provedor real é só configuração /
   Infrastructure — o use case `AskAiHandler` não muda.

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
- adapter real plugável (Ollama local ou OpenAI/Azure);
- Domain clínico preservado;
- auditoria e rastreamento de fontes desde o início.

Riscos / próximos passos:

- embedding ainda é determinístico (hash), não modelo semântico real;
- Fase 15 deve indexar dados reais do prontuário.
