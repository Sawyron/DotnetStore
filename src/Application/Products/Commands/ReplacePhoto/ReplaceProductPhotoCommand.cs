using Application.Files.Commands.CreateFile;
using Domain.Products;
using MediatR;

namespace Application.Products.Commands.ReplacePhoto;

public sealed record ReplaceProductPhotoCommand(ProductId ProductId, CreateFileCommand Photo) : IRequest;
