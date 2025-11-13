using CSharpFunctionalExtensions;
using LeadPipe.Application.DataInterfaces.Dto;
namespace LeadPipe.Application.Services;

public interface ILeafClientService
{
    Task<Result<List<ILeafDto>>> GetAsync(HttpClient client, int offset = 0, int errorLimit = 5, int limit = 1000);
    HttpClient GetClient(IHttpClientFactory factory);
    Result<List<IMessage>>[] GetMessages(HttpClient client, List<ILeafDto> leafs);
}