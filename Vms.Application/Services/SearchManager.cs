namespace Vms.Application.Services;

public interface ISearchManager
{
    void Add(string companyCode, string entityKey, EntityKind entityKind, string name, string content);
    Task Update(string entityKey, EntityKind entityKind, string name, string content, CancellationToken cancellationToken);
}

public class SearchManager(VmsDbContext context) : ISearchManager
{
    public void Add(string companyCode, string entityKey, EntityKind entityKind, string name, string content)
    {
        var entityTag = new EntityTag(companyCode, entityKey, entityKind, name, content);
        context.EntityTags.Add(entityTag);
    }
    public async Task Update(string entityKey, EntityKind entityKind, string name, string content,
        CancellationToken cancellationToken)
    {
        var tag = await context.EntityTags.FindAsync(new object[] { entityKey, entityKind }, cancellationToken);
        tag?.Update(name, content);
    }
}
