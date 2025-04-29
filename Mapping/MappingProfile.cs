using AutoMapper;
using MR_power.Data;
using MR_power.DTOs;
using MR_power.Models;

namespace MR_power.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<UserAccount, UserDTO>();
            CreateMap<CreateUserDTO, UserAccount>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());
            CreateMap<UpdateUserDTO, UserAccount>()
                .ForMember(dest => dest.Password, opt => opt.Ignore());

            // Customer mappings
            CreateMap<Customer, CustomerDTO>();
            CreateMap<CreateCustomerDTO, Customer>();
            CreateMap<UpdateCustomerDTO, Customer>();

            // StockItem mappings
            CreateMap<Stook, StockItemDTO>();
            CreateMap<CreateStockItemDTO, Stook>();
            CreateMap<UpdateStockItemDTO, Stook>();

            // Bill mappings
            CreateMap<Bill, BillDTO>();
            CreateMap<CreateBillDTO, Bill>();
            CreateMap<UpdateBillDTO, Bill>();
            CreateMap<BillItem, BillItemDTO>();
            CreateMap<CreateBillItemDTO, BillItem>();
            CreateMap<UpdateBillItemDTO, BillItem>();
        }
    }
} 