namespace AI.SocialNetwork.Domain.Entities;

// Канбан: доска/колонки/карточки
public class Board
{
    public long Id { get; set; }
    public long? DealId { get; set; }
    public long OwnerId { get; set; }
    public string Title { get; set; } = string.Empty;

    public ICollection<BoardColumn> Columns { get; set; } = new List<BoardColumn>();
}

public class BoardColumn
{
    public long Id { get; set; }
    public long BoardId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? WipLimit { get; set; }
    public int Ordinal { get; set; }

    public Board? Board { get; set; }
    public ICollection<BoardCard> Cards { get; set; } = new List<BoardCard>();
}

public class BoardCard
{
    public long Id { get; set; }
    public long ColumnId { get; set; }
    public string Title { get; set; } = string.Empty;
    public long? RequiredSkillId { get; set; }  // связь с компетенцией
    public int? RequiredLevel { get; set; }
    public int Priority { get; set; }
    public DateTime? DueAt { get; set; }
    public int OrderedTimeMinutes { get; set; }
    public string Status { get; set; } = "todo";   // todo/in_progress/review/done
    public long? AssignedToUserId { get; set; }

    public BoardColumn? Column { get; set; }
    public Skill? RequiredSkill { get; set; }
    public User? AssignedToUser { get; set; }
}

// Документ/файл (PDF-резюме)
public class UserFile
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StorageKey { get; set; } = string.Empty; // MinIO/S3
    public string Kind { get; set; } = string.Empty;       // resume | portfolio | certificate
    public bool Verified { get; set; }

    public User? User { get; set; }
}