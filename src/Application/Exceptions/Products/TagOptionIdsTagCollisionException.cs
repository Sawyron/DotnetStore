namespace Application.Exceptions.Products;

internal class TagOptionIdsTagCollisionException : ResourceStateConflictException
{
    public TagOptionIdsTagCollisionException()
        : base("More than one tag options ids belong to the same tag") { }
}
