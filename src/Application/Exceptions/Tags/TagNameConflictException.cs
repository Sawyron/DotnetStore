using Domain.Categories;

namespace Application.Exceptions.Tags;

internal class TagNameConflictException : ResourceDublicationException
{
    public TagNameConflictException(string name, CategoryId categoryId)
        : base($"Tag with name '{name}' already exists in category with id '{categoryId.Value}'")
    { }
}
