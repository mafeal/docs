using docs.Dto;

namespace docs.Repositories;

public static class DocsRepository
{
    public static Dictionary<int, ExemploDto> bancoDados = new()
    {
        { 1, new ExemploDto("Planilha de Vendas", "xlsx") },
        { 2, new ExemploDto("Relatório de Estoque", "docx") },
        { 3, new ExemploDto("Relação de Clientes", "pdf") }
    };
    
}