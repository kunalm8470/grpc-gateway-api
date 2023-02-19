using AutoMapper;
using Domain.Entities;
using Domain.Models.Requests.v1;
using Google.Protobuf.WellKnownTypes;
using Infrastructure.Protos;

namespace Infrastructure.RemoteProcedureServices.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductModel>()
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => y.ManufacturedDate.ToDateTime()))
            .ForMember(x => x.CreatedAt, x => x.MapFrom(y => y.CreatedAt.ToDateTime()))
            .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => y.UpdatedAt != null ? y.UpdatedAt.ToDateTime() : default(DateTime?)));

            CreateMap<AddProductPayload, AddProductDto>()
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => y.ManufacturedDate.ToDateTime()));

            CreateMap<UpdateProductPayloadObject, UpdateProductDto>()
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => y.ManufacturedDate.ToDateTime()));
        }
    }
}
