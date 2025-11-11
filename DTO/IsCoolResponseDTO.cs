using System.Text.Json.Serialization;

namespace IsCool.DTO
{
    /// <summary>
    /// Estrutura da resposta do assistente IsCool AI.
    /// </summary>
    public class IsCoolResponseDto
    {
        /// <summary>
        /// Resumo claro e conciso da resposta.
        /// </summary>
        [JsonPropertyName("summary")]
        public string Summary { get; set; } = string.Empty;

        /// <summary>
        /// Explicação mais detalhada ou contextual.
        /// </summary>
        [JsonPropertyName("details")]
        public string? Details { get; set; }
    }
}
