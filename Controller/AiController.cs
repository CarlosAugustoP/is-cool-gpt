using IsCool.Abstractions;
using IsCool.DTO;
using IsCool.Middlewares;
using IsCool.Services;
using Microsoft.AspNetCore.Mvc;

namespace IsCool.Controller
{
    [ApiController]
    [Route("api/chats")]
    public class ChatController : IsCoolController
    {
        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Inicia um novo chat para o usuário atual.
        /// </summary>
        [HttpPost("start")]
        [RequireProfileFilter]
        public IActionResult StartChat()
        {
            var chatId = _chatService.StartChat(CurrentUser!);
            return CreatedAtAction(nameof(GetChatName), new { chatId }, Result<Guid>.Success(chatId));
        }

        /// <summary>
        /// Envia uma mensagem para o chat e obtém a resposta da IA.
        /// </summary>
        [HttpPost("{chatId:guid}/talk")]
        [RequireProfileFilter]
        public async Task<IActionResult> TalkToAi(Guid chatId, [FromBody] MessageRequestDTO request)
        {
            var response = await _chatService.TalkToAi(request.Message, CurrentUser!, chatId);
            return Ok(Result<IsCoolResponseDto>.Success(response));
        }

        /// <summary>
        /// Obtém o nome gerado automaticamente para o chat.
        /// </summary>
        [HttpGet("{chatId:guid}/name")]
        [RequireProfileFilter]
        public async Task<IActionResult> GetChatName(Guid chatId)
        {
            var name = await _chatService.GetChatName(chatId, CurrentUser!);
            return Ok(Result<string>.Success(name));
        }
    }
}
