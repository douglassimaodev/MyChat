using NotificationApp.BLL.Services.Medias.Query;

namespace NotificationApp.BLL.Services.Medias.Handler
{
    public class GetCommandHandler : IRequestHandler<GetCommandQuery, Task>
    {
        public GetCommandHandler()
        {

        }

        public async Task Handle(GetCommandQuery request, CancellationToken cancellationToken)
        {
            return await _unitOfWork.MediaRepository.GetAll();
        }
    }
}
