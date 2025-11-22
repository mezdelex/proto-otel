namespace Application.Profiles;

public class CategoriesProfile : Profile
{
    public CategoriesProfile()
    {
        CreateMap<Category, CategoryDTO>();
        CreateMap<Category, PatchedCategoryEvent>();
        CreateMap<Category, PostedCategoryEvent>();
        CreateMap<PatchCategoryCommand, Category>();
        CreateMap<PostCategoryCommand, Category>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid().ToString()));
    }
}
