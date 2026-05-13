using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Forms;
using Family.Shared;
using System.Text.Json;

namespace WebFamily.Services
{
    public interface IFamilyClientService
    {
        Task<List<FamilyMemberDto>?> GetAllMembersAsync();
        Task<FamilyMemberDto?> AddMemberAsync(FamilyMemberDto dto);
        Task<bool> UpdateMemberAsync(int id, FamilyMemberDto dto);
        Task<bool> DeleteMemberAsync(int id);
        Task<string?> UploadPhotoAsync(string memberId, IBrowserFile file, long maxFileSize = 5242880);
    }


    public class FamilyClientService : IFamilyClientService
    {
        private readonly HttpClient _http;

        public FamilyClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<FamilyMemberDto>?> GetAllMembersAsync()
        {
            try
            {
                var members = await _http.GetFromJsonAsync<List<FamilyMemberDto>>("api/Family/all");
                if (members != null)
                {
                    // Сразу централизованно подготавливаем пути к фото
                    foreach (var m in members.Where(x => !string.IsNullOrEmpty(x.PhotoUrl)))
                    {
                        if (!m.PhotoUrl.StartsWith("http"))
                        {
                            m.PhotoUrl = $"{_http.BaseAddress}{m.PhotoUrl}";
                        }
                    }
                }
                return members;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка получения родственников: {ex.Message}");
                return null;
            }
        }

        public async Task<FamilyMemberDto?> AddMemberAsync(FamilyMemberDto dto)
        {
            var response = await _http.PostAsJsonAsync("api/Family/add", dto);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<FamilyMemberDto>();
            }
            return null;
        }

        public async Task<bool> UpdateMemberAsync(int id, FamilyMemberDto dto)
        {
            // Очищаем от базового адреса перед отправкой в БД (синхронно для всех PUT-запросов)
            if (!string.IsNullOrEmpty(dto.PhotoUrl) && dto.PhotoUrl.StartsWith(_http.BaseAddress?.ToString() ?? ""))
            {
                dto.PhotoUrl = dto.PhotoUrl.Replace(_http.BaseAddress?.ToString() ?? "", "");
            }

            var response = await _http.PutAsJsonAsync($"api/Family/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteMemberAsync(int id)
        {
            var response = await _http.DeleteAsync($"api/Family/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<string?> UploadPhotoAsync(string memberId, IBrowserFile file, long maxFileSize = 5242880)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                content.Add(fileContent, "file", file.Name);

                var response = await _http.PostAsync($"api/Family/{memberId}/upload-photo", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<JsonElement>();
                    if (result.TryGetProperty("url", out var urlProperty))
                    {
                        return urlProperty.GetString();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке фото через сервис: {ex.Message}");
            }
            return null;
        }
    }

}
