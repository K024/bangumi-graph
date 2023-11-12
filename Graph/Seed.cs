using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Bangumi.Graph;

public static class SeedData
{
    public static async Task SeedDb(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureCreatedAsync();
        if (await context.Subject.AnyAsync())
        {
            Console.WriteLine("info: database already seeded, skipping");
            return;
        }

        var dump_dir = System.IO.Path.Join(Directory.GetCurrentDirectory(), "dump");

        var character_jsonl = System.IO.Path.Join(dump_dir, "character.jsonlines");
        var episode_jsonl = System.IO.Path.Join(dump_dir, "episode.jsonlines");
        var person_characters_jsonl = System.IO.Path.Join(dump_dir, "person-characters.jsonlines");
        var person_jsonl = System.IO.Path.Join(dump_dir, "person.jsonlines");
        var subject_characters_jsonl = System.IO.Path.Join(dump_dir, "subject-characters.jsonlines");
        var subject_persons_jsonl = System.IO.Path.Join(dump_dir, "subject-persons.jsonlines");
        var subject_relations_jsonl = System.IO.Path.Join(dump_dir, "subject-relations.jsonlines");
        var subject_jsonl = System.IO.Path.Join(dump_dir, "subject.jsonlines");

        if (!File.Exists(character_jsonl))
            throw new Exception($"file not found: {character_jsonl}");

        var commitCounter = 0;
        async Task commit()
        {
            commitCounter++;
            if (commitCounter % 1000 == 0)
            {
                await context.SaveChangesAsync();
                context.ChangeTracker.Clear();
            }
        }

        var character_ids = new HashSet<long>();
        Console.WriteLine("info: seeding characters");
        foreach (var character in ReadJsonLines<Character>(character_jsonl))
        {
            character_ids.Add(character.id);
            context.Character.Add(character);
            await commit();
        }

        var subject_ids = new HashSet<long>();
        Console.WriteLine("info: seeding subjects");
        foreach (var subject in ReadJsonLines<Subject>(subject_jsonl))
        {
            subject_ids.Add(subject.id);
            context.Subject.Add(subject);
            await commit();
        }

        // insert after subject
        Console.WriteLine("info: seeding episodes");
        foreach (var episode in ReadJsonLines<Episode>(episode_jsonl))
        {
            if (!subject_ids.Contains(episode.subject_id))
            {
                Console.WriteLine($"warn: episode {episode.id} dependency not found, skipping");
                continue;
            }
            context.Episode.Add(episode);
            await commit();
        }

        var person_ids = new HashSet<long>();
        Console.WriteLine("info: seeding persons");
        foreach (var person in ReadJsonLines<Person>(person_jsonl))
        {
            person_ids.Add(person.id);
            context.Person.Add(person);
            await commit();
        }

        var person_character_ids = new HashSet<(long, long, long)>();
        Console.WriteLine("info: seeding person_character");
        foreach (var person_character in ReadJsonLines<Person_Character>(person_characters_jsonl))
        {
            if (!character_ids.Contains(person_character.character_id) ||
                !person_ids.Contains(person_character.person_id) ||
                !subject_ids.Contains(person_character.subject_id) ||
                person_character_ids.Contains((person_character.character_id, person_character.person_id, person_character.subject_id)))
            {
                Console.WriteLine($"warn: person_character {person_character.character_id} dependency not found, skipping");
                continue;
            }
            person_character_ids.Add((person_character.character_id, person_character.person_id, person_character.subject_id));
            context.Person_Characters.Add(person_character);
            await commit();
        }

        var subject_character_ids = new HashSet<(long, long)>();
        Console.WriteLine("info: seeding subject_character");
        foreach (var subject_character in ReadJsonLines<Subject_Character>(subject_characters_jsonl))
        {
            if (!character_ids.Contains(subject_character.character_id) ||
                !subject_ids.Contains(subject_character.subject_id) ||
                subject_character_ids.Contains((subject_character.character_id, subject_character.subject_id)))
            {
                Console.WriteLine($"warn: subject_character {subject_character.character_id} dependency not found, skipping");
                continue;
            }
            subject_character_ids.Add((subject_character.character_id, subject_character.subject_id));
            context.Subject_Characters.Add(subject_character);
            await commit();
        }

        var subject_person_ids = new HashSet<(long, long)>();
        Console.WriteLine("info: seeding subject_person");
        foreach (var subject_person in ReadJsonLines<Subject_Person>(subject_persons_jsonl))
        {
            if (!person_ids.Contains(subject_person.person_id) ||
                !subject_ids.Contains(subject_person.subject_id) ||
                subject_person_ids.Contains((subject_person.person_id, subject_person.subject_id)))
            {
                Console.WriteLine($"warn: subject_person {subject_person.person_id} dependency not found, skipping");
                continue;
            }
            subject_person_ids.Add((subject_person.person_id, subject_person.subject_id));
            context.Subject_Persons.Add(subject_person);
            await commit();
        }

        var subject_relation_ids = new HashSet<(long, long)>();
        Console.WriteLine("info: seeding subject_relation");
        foreach (var subject_relation in ReadJsonLines<Subject_Relation>(subject_relations_jsonl))
        {
            if (!subject_ids.Contains(subject_relation.subject_id) ||
                !subject_ids.Contains(subject_relation.related_subject_id) ||
                subject_relation_ids.Contains((subject_relation.subject_id, subject_relation.related_subject_id)))
            {
                Console.WriteLine($"warn: subject_relation {subject_relation.subject_id} dependency not found, skipping");
                continue;
            }
            subject_relation_ids.Add((subject_relation.subject_id, subject_relation.related_subject_id));
            context.Subject_Relations.Add(subject_relation);
            await commit();
        }

        await context.SaveChangesAsync();
        Console.WriteLine("info: seed data done.");
    }

    private static IEnumerable<T> ReadJsonLines<T>(string file)
    {
        foreach (var line in File.ReadLines(file))
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;
            var cleaned = line.Replace("\\u0000", "");
            yield return JsonSerializer.Deserialize<T>(cleaned)
                ?? throw new NullReferenceException();
        }
    }
}
