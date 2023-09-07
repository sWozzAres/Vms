namespace Vms.Application.Services;

public interface ISearchManager
{
    void Add(string? companyCode, string entityKey, EntityKind entityKind, string name, string content);
    Task UpdateOrAdd(string? companyCode, string entityKey, EntityKind entityKind, string name, string content, CancellationToken cancellationToken);
}

public class SearchManager(VmsDbContext context, ILogger<SearchManager> logger) : ISearchManager
{
    public void Add(string? companyCode, string entityKey, EntityKind entityKind, string name, string content)
    {
        logger.LogDebug("Adding search information {companycode} {entitykey} {entitykind}.", companyCode, entityKey, entityKind);

        var entityTag = new EntityTag(companyCode, entityKey, entityKind, name, content);
        context.EntityTags.Add(entityTag);
    }
    public async Task UpdateOrAdd(string? companyCode, string entityKey, EntityKind entityKind, string name, string content,
        CancellationToken cancellationToken)
    {
        var tag = await context.EntityTags
            .SingleOrDefaultAsync(t => t.EntityKey == entityKey && t.EntityKind == entityKind, cancellationToken);
        if (tag is not null)
            tag.Update(name, content);
        else
            Add(companyCode, entityKey, entityKind, name, content);
    }
}
