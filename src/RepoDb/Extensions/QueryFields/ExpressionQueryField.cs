using RepoDb;
using RepoDb.Enumerations;
using RepoDb.Interfaces;

namespace RepoDb.Extensions.QueryFields
{
    /// <summary>
    /// Abstract base for query fields representing expressions (e.g., field-to-field, arithmetic, etc.).
    /// </summary>
    public abstract class ExpressionQueryField : QueryField
    {
        public IEnumerable<Field> Fields { get; }

        protected ExpressionQueryField(IEnumerable<string> fields, Operation operation)
            : this(fields.Select(x => new Field(x)), operation)
        {
        }

        protected ExpressionQueryField(IEnumerable<Field> fields, Operation operation)
                    : base(fields.First(), operation, null, null)
        {
            Fields = fields;
        }

        public override string GetString(int index, IDbSetting dbSetting)
        {
            return GetString(dbSetting);
        }

        protected override string GetString(int index, string? functionFormat, IDbSetting dbSetting)
        {
            return GetString(dbSetting);
        }

        public abstract override string GetString(IDbSetting dbSetting);
    }
}
