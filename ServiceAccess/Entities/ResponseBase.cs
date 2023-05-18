// ServiceAccess.Entities.ResponseBase<T>
using System.Collections.Generic;

namespace ServiceAccess.Entities;

public class ResponseBase<T>
{
    public IList<T>? Lista { get; set; }

    public int StatusCode { get; set; }

    public string? ReasonPhrase { get; set; }
}
