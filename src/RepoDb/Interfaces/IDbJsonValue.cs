using System.Text.Json.Nodes;

namespace RepoDb.Interfaces;

public interface IDbJsonValue
{
    JsonNode? JsonNode { get; }
}
