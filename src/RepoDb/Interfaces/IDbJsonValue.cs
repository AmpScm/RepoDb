using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;

namespace RepoDb.Interfaces;

public interface IDbJsonValue
{
    JsonNode? JsonNode { get; }
}
