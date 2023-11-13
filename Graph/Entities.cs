using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Bangumi.Graph;


public enum CharacterType
{
    Character = 1,
    Mechanic = 2,
    Ship = 3,
    Organization = 4,
}

public class Character
{
    [Key]
    [GraphQLType(typeof(IntType))]
    public long id { get; set; }
    public CharacterType role { get; set; }
    public string name { get; set; } = null!;
    public string infobox { get; set; } = null!;
    public string summary { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Person_Character> person_characters { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Subject_Character> subject_characters { get; set; } = null!;
}

public enum EpType
{
    MainStory = 0,
    SP = 1,
    OP = 2,
    ED = 3,
    PV = 4,
    MAD = 5,
    Other = 6,
}

public class Episode
{
    [Key]
    [GraphQLType(typeof(IntType))]
    public long id { get; set; }
    public string name { get; set; } = null!;
    public string name_cn { get; set; } = null!;
    public string description { get; set; } = null!;
    public string airdate { get; set; } = null!;
    public int disc { get; set; }

    [ForeignKey(nameof(subject))]
    public long subject_id { get; set; }
    public double sort { get; set; }
    public EpType type { get; set; }

    public Subject subject { get; set; } = null!;
}


public enum SubjectType
{
    Book = 1,
    Anime = 2,
    Music = 3,
    Game = 4,
    Real = 6,
}

public class Subject
{
    [Key]
    [GraphQLType(typeof(IntType))]
    public long id { get; set; }
    public SubjectType type { get; set; }
    public string name { get; set; } = null!;
    public string name_cn { get; set; } = null!;
    public string infobox { get; set; } = null!;
    public int platform { get; set; }
    public string summary { get; set; } = null!;
    public bool nsfw { get; set; }

    [UseFiltering]
    [UseSorting]
    public IList<Episode> episodes { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Subject_Character> subject_characters { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Person_Character> person_characters { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Subject_Person> subject_persons { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    [InverseProperty(nameof(Subject_Relation.subject))]
    public IList<Subject_Relation> subject_relations { get; set; } = null!;
}


public enum PersonType
{
    Individual = 1,
    Corporation = 2,
    Association = 3,
}

public class Person
{
    [Key]
    [GraphQLType(typeof(IntType))]
    public long id { get; set; }
    public string name { get; set; } = null!;
    public PersonType type { get; set; }
    public List<string> career { get; set; } = null!;
    public string infobox { get; set; } = null!;
    public string summary { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Person_Character> person_characters { get; set; } = null!;

    [UseFiltering]
    [UseSorting]
    public IList<Subject_Person> subject_persons { get; set; } = null!;
}


// =========== relations ===============


[PrimaryKey(nameof(person_id), nameof(subject_id), nameof(character_id))]
public class Person_Character
{
    [ForeignKey(nameof(person))]
    public long person_id { get; set; }
    public Person person { get; set; } = null!;

    [ForeignKey(nameof(subject))]
    public long subject_id { get; set; }
    public Subject subject { get; set; } = null!;

    [ForeignKey(nameof(character))]
    public long character_id { get; set; }
    public Character character { get; set; } = null!;

    public string summary { get; set; } = null!;
}

[PrimaryKey(nameof(character_id), nameof(subject_id))]
public class Subject_Character
{
    [ForeignKey(nameof(character))]
    public long character_id { get; set; }
    public Character character { get; set; } = null!;

    [ForeignKey(nameof(subject))]
    public long subject_id { get; set; }
    public Subject subject { get; set; } = null!;

    public int type { get; set; }
    public int order { get; set; }
}

[PrimaryKey(nameof(person_id), nameof(subject_id))]
public class Subject_Person
{
    [ForeignKey(nameof(person))]
    public long person_id { get; set; }
    public Person person { get; set; } = null!;

    [ForeignKey(nameof(subject))]
    public long subject_id { get; set; }
    public Subject subject { get; set; } = null!;

    public int position { get; set; }
}

[PrimaryKey(nameof(subject_id), nameof(related_subject_id))]
public class Subject_Relation
{
    [ForeignKey(nameof(subject))]
    public long subject_id { get; set; }
    public Subject subject { get; set; } = null!;

    public int relation_type { get; set; }

    [ForeignKey(nameof(related_subject))]
    public long related_subject_id { get; set; }
    public Subject related_subject { get; set; } = null!;

    public int order { get; set; }
}
