using OnBoardy.API.DTOs;
using OnBoardy.API.Models;

namespace OnBoardy.API.Services.Infrastructure
{
    public interface IMapperService
    {
        UserResponse ToUserResponse(User user);
        OrganizationResponse ToOrganizationResponse(Organization organization);
        MembershipResponse ToMembershipResponse(Membership membership);
        InvitationResponse ToInvitationResponse(Invitation invitation);
        ModuleResponse ToModuleResponse(Module module);
    }
}