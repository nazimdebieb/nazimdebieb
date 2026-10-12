using System.Collections.Generic;

namespace BloomGarden.Core.Levels
{
    /// <summary>Outcome of loading or validating a level. Collects every error rather than stopping at the first.</summary>
    public sealed class ValidationResult
    {
        private readonly List<string> _errors = new List<string>();

        public IReadOnlyList<string> Errors => _errors;
        public bool IsValid => _errors.Count == 0;

        public void AddError(string message) => _errors.Add(message);

        public override string ToString() => IsValid ? "valid" : string.Join("; ", _errors);
    }
}
