using Istapio.Domain.Entities.Common;

namespace Istapio.Domain.Entities;

public class Setting : BaseAuditableEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}

