namespace ProjektManagement.Repositories;


using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DataClass;

public abstract class _Repository<T>
    where T : DataClass
{
    protected readonly string FilePath;

    protected RepositoryBase(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be null or empty.", nameof(filePath));

        FilePath = filePath;
    }

    // -----------------------------
    // Protected JSON I/O Utilities
    // -----------------------------

    protected virtual List<T> ReadAllFromFile()
    {
        if (!File.Exists(FilePath))
            return new List<T>();

        var json = File.ReadAllText(FilePath);

        if (string.IsNullOrWhiteSpace(json))
            return new List<T>();

        return JsonSerializer.Deserialize<List<T>>(json, JsonOptions)
               ?? new List<T>();
    }

    protected virtual void WriteAllToFile(IEnumerable<T> entities)
    {
        var json = JsonSerializer.Serialize(entities, JsonOptions);
        File.WriteAllText(FilePath, json);
    }

    protected virtual JsonSerializerOptions JsonOptions => new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    protected virtual int GenerateNextUid(IEnumerable<T> entities)
    {
        int max = 0;

        foreach (var e in entities)
        {
            if (e.Uid > max)
                max = e.Uid;
        }

        return max + 1;
    }

    // -----------------------------
    // Abstract CRUD API
    // -----------------------------

    public abstract IReadOnlyList<T> GetAll();

    public abstract T? GetByUid(int uid);

    public abstract T Insert(T entity);

    public abstract T Update(T entity);

    public abstract bool Delete(int uid);
}