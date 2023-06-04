using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CarCrawler.Database;

internal static class DbContextExtensions
{
    public static void BulkMerge<T>(this DbContext @this, IEnumerable<T> entities, string primaryKeyName = "Id")
        where T : class
    {
        var dbSet = @this.Set<T>();
        var type = typeof(T);
        var tableName = @this.Model.FindEntityType(type)!.GetTableName();

        foreach (var entity in entities)
        {
            var entityKeyValue = type.GetProperty(primaryKeyName)!.GetValue(entity, null);
            if (entityKeyValue == null) continue;

            var query = $"SELECT * FROM {tableName} WHERE {primaryKeyName} = @param";
            var sqlParam = new SqliteParameter("@param", entityKeyValue);
            var existingEntity = dbSet.FromSqlRaw(query, sqlParam).FirstOrDefault();

            if (existingEntity == null)
            {
                AddEntity(dbSet, entity);
            }
            else
            {
                UpdateEntity(@this, existingEntity, entity);
            }
        }

        @this.SaveChanges();
    }

    private static void AddEntity<T>(DbSet<T> dbSet, T entity) where T : class => dbSet.Add(entity);

    private static void UpdateEntity<T>(DbContext dbContext, T targetEntity, T sourceEntity)
        where T : class
    {
        var entry = dbContext.Entry(targetEntity);
        RestorePrimaryKeys(entry, targetEntity, sourceEntity);
        entry.CurrentValues.SetValues(sourceEntity);
    }

    private static void RestorePrimaryKeys<T>(EntityEntry<T> entry, T targetEntity, T sourceEntity)
        where T : class
    {
        var type = typeof(T);
        var primaryKeys = entry.Metadata.FindPrimaryKey()!.Properties;

        foreach (var primaryKey in primaryKeys)
        {
            RestorePrimaryKey(targetEntity, sourceEntity, type, primaryKey);
        }
    }

    private static void RestorePrimaryKey<T>(T targetEntity, T sourceEntity, Type type, IProperty primaryKey)
        where T : class
    {
        var property = type.GetProperty(primaryKey.Name)!;
        var value = property.GetValue(targetEntity, null);
        property!.SetValue(sourceEntity, value, null);
    }
}