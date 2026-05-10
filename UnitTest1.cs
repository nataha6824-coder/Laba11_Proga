using Arch.EFCore;
using Lab_10_Proga;
using Microsoft.EntityFrameworkCore;

namespace Test_Crud1;

public class CrudTests
{
    public CrudTests()
    {
        using var db = new DataContext();
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();
    }

    [Fact]
    public async Task CreateNote()
    {
        int id = 100;
        string text = "text";
        DateTimeOffset date = DateTimeOffset.Now;

        var note = await Crud.Create(id, text, date);

        using var db = new DataContext();
        var noteDb = await db.Note.FindAsync(id);
        Assert.NotNull(note);
        Assert.Equal(id, noteDb.Id);
        Assert.Equal(text, noteDb.Name);
        Assert.Equal(date, noteDb.CreatedAt);
    }

    [Theory]
    [InlineData("le", new int[] { 1, 2 })]
    [InlineData("ti", new int[] { 3 })]
    [InlineData("", new int[] { 1, 2, 3 })]
    [InlineData("abc", new int[] { })]
    public async Task ReadNote(string searchText, int[] expectedIds)
    {
        using var context = new DataContext();
        var notes = new[]
        {
            new Note { Id = 1, Name = "elephant", CreatedAt = DateTimeOffset.Now },
            new Note { Id = 2, Name = "leo", CreatedAt = DateTimeOffset.Now },
            new Note { Id = 3, Name = "tiger", CreatedAt = DateTimeOffset.Now }
        };
        context.Note.AddRange(notes);
        await context.SaveChangesAsync();

        List<Note> result = await Crud.Read(searchText);

        Assert.Equal(expectedIds.Length, result.Count);
        foreach (int id in expectedIds)
        {
            Assert.Contains(result, n => n.Id == id);
        }
    }

    [Fact]
    public async Task UpdateNote()
    {
        int id = 10;
        string oldText = "Random text";
        DateTimeOffset oldDate = DateTimeOffset.Now;

        using var db = new DataContext();
        var note = new Note
        {
            Id = id,
            Name = oldText,
            CreatedAt = oldDate
        };

        db.Note.Add(note);
        await db.SaveChangesAsync();

        string newText = "Noviy text";
        DateTimeOffset newDate = DateTimeOffset.UtcNow.AddHours(1);

        await Crud.Update(note, id, newText, newDate);

        var updatedNote = await db.Note.FindAsync(id);
        Assert.NotNull(updatedNote);
        Assert.Equal(newText, updatedNote.Name);
        Assert.Equal(newDate, updatedNote.CreatedAt);
    }

    [Fact]
    public async Task DeleteNote()
    {
        int id = 2;
        string text = "text";
        DateTimeOffset date = DateTimeOffset.Now;

        Note createdNote = await Crud.Create(id, text, date);

        await Crud.Delete(createdNote);

        using var db = new DataContext();
        Note? deletedNote = await db.Note.FindAsync(id);
        Assert.Null(deletedNote);
    }
}