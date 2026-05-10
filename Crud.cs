using Arch.EFCore;
using Lab_10_Proga;
using Microsoft.EntityFrameworkCore;

public class Crud
{
    public static async Task<Note> Create(int id, string text, DateTimeOffset createdat, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var note = new Note
        {
            Id = id,
            Name = text,
            CreatedAt = createdat,
        };
        db.Note.Add(note);
        await db.SaveChangesAsync(ct);
        return note;
    }

    public static async Task<List<Note>> Read(string search, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        var result = await db.Note
            .Where(x => EF.Functions.Like(x.Name, $"%{search}%"))
            .ToListAsync(ct);
        return result;
    }

    public static async Task<Note?> Read(int id, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        return await db.Note.FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public static async Task Update(Note note, int id, string text, DateTimeOffset createdat, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        note.Id = id;
        note.Name = text;
        note.CreatedAt = createdat;
        db.Note.Update(note);
        await db.SaveChangesAsync(ct);
    }

    public static async Task Delete(Note note, CancellationToken ct = default)
    {
        await using var db = new DataContext();
        db.Note.Remove(note);
        await db.SaveChangesAsync(ct);
    }
}
