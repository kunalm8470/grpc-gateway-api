using Api.Protos;
using AutoMapper;
using Domain.Entities;
using Domain.Models.Requests;
using Google.Protobuf.WellKnownTypes;

namespace Api.Mappers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ProductModel, Product>()
            .ForMember(x => x.Id, x => x.MapFrom(y => y.Id.ToString()))
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => Timestamp.FromDateTime(y.ManufacturedDate.ToUniversalTime())))
            .ForMember(x => x.CreatedAt, x => x.MapFrom(y => Timestamp.FromDateTime(y.CreatedAt.ToUniversalTime())))
            .ForMember(x => x.UpdatedAt, x => x.MapFrom(y => 
                y.UpdatedAt.HasValue ?
                Timestamp.FromDateTime(y.CreatedAt.ToUniversalTime())
                : null
            ));

            CreateMap<AddProductPayload, AddProductDto>()
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => y.ManufacturedDate.ToDateTime()));

            CreateMap<UpdateProductPayloadObject, UpdateProductDto>()
            .ForMember(x => x.ManufacturedDate, x => x.MapFrom(y => y.ManufacturedDate.ToDateTime()));
        }
    }
}
