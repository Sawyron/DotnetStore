using Application.Core;
using Application.Exceptions.Products;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

internal class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>
{
    private readonly IProductRepository _productRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        IUnitOfWork unitOfWork)
    {
        _productRepository = productRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.FindByIdAsync(request.Id)
            ?? throw new ProductNotFoundException(request.Id);
        product = product.Update(
            name: request.Name,
            price: request.Price,
            description: request.Description);
        await _productRepository.UpdateAsync(product);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
