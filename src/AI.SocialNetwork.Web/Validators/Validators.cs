using FluentValidation;
using AI.SocialNetwork.Web.Controllers;

namespace AI.SocialNetwork.Web.Validators;

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Country).MaximumLength(100);
    }
}

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Bio).MaximumLength(2000);
        RuleFor(x => x.City).MaximumLength(100);
        RuleFor(x => x.Country).MaximumLength(100);
    }
}

public class CreateAgentRequestValidator : AbstractValidator<CreateAgentRequest>
{
    public CreateAgentRequestValidator()
    {
        RuleFor(x => x.OwnerId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.SystemPrompt).MaximumLength(4000);
        RuleFor(x => x.CommunicationStyle).MaximumLength(100);
    }
}

public class SendAgentMessageRequestValidator : AbstractValidator<SendAgentMessageRequest>
{
    public SendAgentMessageRequestValidator()
    {
        RuleFor(x => x.Content).NotEmpty().MaximumLength(4000);
    }
}

public class CreateSkillRequestValidator : AbstractValidator<CreateSkillRequest>
{
    public CreateSkillRequestValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
        RuleFor(x => x.CategoryId).GreaterThan(0);
    }
}

public class AddUserSkillRequestValidator : AbstractValidator<AddUserSkillRequest>
{
    public AddUserSkillRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.SkillId).GreaterThan(0);
        RuleFor(x => x.Level).InclusiveBetween(0, 10);
    }
}

public class CreateDealRequestValidator : AbstractValidator<CreateDealRequest>
{
    public CreateDealRequestValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0);
        RuleFor(x => x.ContractorId).GreaterThan(0);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.ContractTerms).MaximumLength(4000);
    }
}

public class EscrowRequestValidator : AbstractValidator<EscrowRequest>
{
    public EscrowRequestValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class MilestoneRequestValidator : AbstractValidator<MilestoneRequest>
{
    public MilestoneRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(150);
        RuleFor(x => x.Amount).GreaterThan(0);
    }
}

public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostRequestValidator()
    {
        RuleFor(x => x.AuthorId).GreaterThan(0);
        RuleFor(x => x.GroupId).GreaterThan(0).When(x => x.GroupId.HasValue);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(5000);
        RuleFor(x => x.ContentType).NotEmpty().MaximumLength(50);
    }
}

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.OwnerId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}

public class JoinGroupRequestValidator : AbstractValidator<JoinGroupRequest>
{
    public JoinGroupRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class CreateForumRequestValidator : AbstractValidator<CreateForumRequest>
{
    public CreateForumRequestValidator()
    {
        RuleFor(x => x.GroupId).GreaterThan(0);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(150);
    }
}

public class CreateTopicRequestValidator : AbstractValidator<CreateTopicRequest>
{
    public CreateTopicRequestValidator()
    {
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.AuthorId).GreaterThan(0);
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
    }
}

public class SubscribeRequestValidator : AbstractValidator<SubscribeRequest>
{
    public SubscribeRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.TargetUserId).GreaterThan(0);
        RuleFor(x => x.TargetType).NotEmpty().MaximumLength(30);
    }
}

public class CreateConversationRequestValidator : AbstractValidator<CreateConversationRequest>
{
    public CreateConversationRequestValidator()
    {
        RuleFor(x => x.UserIdA).GreaterThan(0);
        RuleFor(x => x.UserIdB).GreaterThan(0);
        RuleFor(x => x.UserIdA).NotEqual(x => x.UserIdB).WithMessage("UserIdA и UserIdB не могут совпадать");
    }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.SenderId).GreaterThan(0);
        RuleFor(x => x.Body).NotEmpty().MaximumLength(4000);
    }
}

public class CreateNotificationRequestValidator : AbstractValidator<CreateNotificationRequest>
{
    public CreateNotificationRequestValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Type).NotEmpty().MaximumLength(50);
        RuleFor(x => x.PayloadJson).NotEmpty().MaximumLength(8000);
        RuleFor(x => x.Channel).NotEmpty().MaximumLength(30);
    }
}

public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(128)
            .Matches(@"[A-Z]").WithMessage("Пароль должен содержать заглавную букву")
            .Matches(@"[a-z]").WithMessage("Пароль должен содержать строчную букву")
            .Matches(@"[0-9]").WithMessage("Пароль должен содержать цифру");
    }
}

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(254);
        RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
    }
}