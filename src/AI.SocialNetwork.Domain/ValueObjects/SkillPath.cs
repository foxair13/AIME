namespace AI.SocialNetwork.Domain.ValueObjects;

// Путь в дереве навыков (Value Object)
public class SkillPath
{
    public string FullPath { get; }
    public List<long> AncestorIds { get; }

    public SkillPath(string fullPath, List<long> ancestorIds)
    {
        FullPath = fullPath;
        AncestorIds = ancestorIds;
    }
}