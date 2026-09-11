namespace AI.SocialNetwork.Domain.Entities;

// Язык (справочник локализации)
public class Language
{
    public long Id { get; set; }
    public string Code { get; set; } = string.Empty;       // "en", "ru", "zh-Hans"
    public string NameNative { get; set; } = string.Empty;
    public string NameEn { get; set; } = string.Empty;
    public bool IsRtl { get; set; }                        // арабский, иврит
    public bool IsDefault { get; set; }
}

// Языки пользователя
public class UserLanguage
{
    public long UserId { get; set; }
    public long LanguageId { get; set; }
    public string Level { get; set; } = "native";          // native | fluent | intermediate | beginner
    public bool IsPrimary { get; set; }

    public User? User { get; set; }
    public Language? Language { get; set; }
}

// Перевод строки (для UI и контента)
public class Translation
{
    public long Id { get; set; }
    public string Key { get; set; } = string.Empty;        // "nav.feed", "deal.status.created"
    public string Culture { get; set; } = string.Empty;    // "ru", "en", "zh-Hans"
    public string Value { get; set; } = string.Empty;
    public string? Context { get; set; }                   // модуль: "deal", "kanban", "profile"
}

// Язык контента (байдинг постов, комментариев)
public class ContentLanguage
{
    public long Id { get; set; }
    public long ContentId { get; set; }
    public string LanguageCode { get; set; } = string.Empty;
}