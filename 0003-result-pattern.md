# ADR-0003 - Result Pattern

## Status

Aceito.

## Contexto

A aplicação precisa representar erros esperados de negócio, como
recurso não encontrado, conflito ou operação inválida.

Usar exceptions para todos esses casos mistura erros esperados com
falhas inesperadas de execução.

## Decisão

Utilizar Result Pattern para erros esperados.

Tipos principais:

- Error
- Result
- Result<T>

Exceptions serão reservadas para falhas inesperadas ou violações
que não pertencem ao fluxo normal da aplicação.

## Consequências

Benefícios:

- erros explícitos;
- melhor testabilidade;
- melhor mapeamento para HTTP;
- menor dependência de exceptions.

Riscos:

- uso excessivo pode deixar o código verboso;
- o padrão deve ser aplicado com consistência.