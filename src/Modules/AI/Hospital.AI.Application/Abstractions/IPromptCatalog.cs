namespace Hospital.AI.Application.Abstractions;

public sealed record PromptTemplate(
    string Key,
    string SystemInstruction,
    string UserTemplate);

public interface IPromptCatalog
{
    PromptTemplate GetRequired(string key);
}
