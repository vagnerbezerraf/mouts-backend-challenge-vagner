using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Profile for mapping between Sale entity and UpdateSaleResult
/// </summary>
public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<UpdateSaleCommand, Sale>();
        CreateMap<UpdateSaleItemCommand, SaleItem>();
        CreateMap<Sale, UpdateSaleResult>();
    }
}
