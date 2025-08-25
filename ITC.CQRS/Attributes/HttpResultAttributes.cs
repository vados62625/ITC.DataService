using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.CQRS.Attributes;

public class OkAttribute : ProducesResponseTypeAttribute
{
    public OkAttribute() : base(StatusCodes.Status200OK)
    {
    }
}

public class NotFoundAttribute : ProducesResponseTypeAttribute
{
    public NotFoundAttribute() : base(StatusCodes.Status404NotFound)
    {
    }
}

public class ConflictAttribute : ProducesResponseTypeAttribute
{
    public ConflictAttribute() : base(StatusCodes.Status409Conflict)
    {
    }
}
public class UnauthAttribute : ProducesResponseTypeAttribute
{
    public UnauthAttribute() : base(StatusCodes.Status401Unauthorized)
    {
    }
}
public class NoContentAttribute : ProducesResponseTypeAttribute
{
    public NoContentAttribute() : base(StatusCodes.Status204NoContent)
    {
    }
}

public class CreatedAttribute : ProducesResponseTypeAttribute
{
    public CreatedAttribute() : base(StatusCodes.Status201Created)
    {
    }
}