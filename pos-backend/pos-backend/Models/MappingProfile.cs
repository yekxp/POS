using AutoMapper;
using pos_backend.Models.DTOs;

namespace pos_backend.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();

            CreateMap<Product, ProductDto>().ReverseMap();

            CreateMap<Payment, PaymentDto>().ReverseMap();

            CreateMap<RefundDto, Payment>()
                .ForMember(dest => dest.RefundAmount, opt => opt.MapFrom(src => src.RefundAmount))
                .ForMember(dest => dest.RefundReason, opt => opt.MapFrom(src => src.Reason))
                .ForMember(dest => dest.RefundDate, opt => opt.MapFrom(src => src.RefundDate));

            CreateMap<CashRegister, CashRegisterDto>().ReverseMap();

            CreateMap<Receipt, ReceiptDto>().ReverseMap();

        }
    }
}
